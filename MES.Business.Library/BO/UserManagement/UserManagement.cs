using Account.DTO.Library;
using NPE.Core;
using NPE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Core.Extensions;
using System.Data.Entity.Core.Objects;
using Microsoft.AspNet.Identity;
using MES.Business.Repositories.UserManagement;
using MES.DTO.Library.UserManagement;
using MES.Identity.Data.Library;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using MES.DTO.Library.Identity;
using System.IO;
using MES.Business.Library.Enums;
using MES.Business.Repositories.RoleManagement;
using MES.DTO.Library.RoleManagement;
using MES.DTO.Library.Common;
using System.Data.Entity.Validation;
namespace MES.Business.Library.BO.UserManagement
{
    public class UserManagement : ContextBusinessBase, IUserManagementRepository
    {
        UserManager<MES.Identity.Data.Library.User> userManager;
        RoleManager<IdentityRole> roleManager;
        public UserManagement()
            : base("UserManagement")
        {
            userManager = new UserManager<MES.Identity.Data.Library.User>(new UserStore<MES.Identity.Data.Library.User>(new ApplicationDbContext()));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
        }

        public ITypedResponse<bool?> ChangePassword(MES.DTO.Library.UserManagement.ChangePassword user)
        {
            ITypedResponse<bool?> toReturn = null;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var selectedUser = db.Users.Where(item => item.Id == CurrentUser).FirstOrDefault();
                if (selectedUser != null)
                {
                    string hashedNewPassword = userManager.PasswordHasher.HashPassword(user.Password);
                    userManager.RemovePassword(selectedUser.Id);
                    var res = userManager.AddPassword(selectedUser.Id, user.Password);

                    if ((res.Succeeded))
                    {
                        DTO.Library.Setup.EmailTemplate.EmailTemplate emailTemplate = new DTO.Library.Setup.EmailTemplate.EmailTemplate();

                        emailTemplate.EmailSubject = "Change Password";
                        emailTemplate.EmailBody = "<p>User Name: " + selectedUser.UserName + "</p><p>Password: " + user.Password + "</p>";

                        bool IsSuccess = false;
                        MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
                        List<string> lstToAddress = new List<string>();
                        lstToAddress.Add(selectedUser.Email);
                        emailRepository.SendEmail(lstToAddress, "", emailTemplate.EmailSubject, emailTemplate.EmailBody, out IsSuccess, null, null);

                        if (IsSuccess)
                        {
                            toReturn = (res.Succeeded)
                               ? SuccessBoolResponse(Languages.GetResourceText("ChangePassword_Success"))
                                : FailedBoolResponse(Languages.GetResourceText("ChangePassword_Failed") + "-" + string.Join(",", res.Errors.ToList().ToArray()));
                        }
                        else
                            toReturn = (res.Succeeded)
                              ? SuccessBoolResponse(Languages.GetResourceText("ChangePassword_Success"))
                               : FailedBoolResponse(Languages.GetResourceText("ChangePassword_SendMailFailure"));

                    }
                    else
                        toReturn = FailedBoolResponse(Languages.GetResourceText("ChangePassword_Failed") + "-" + string.Join(",", res.Errors.ToList().ToArray()));

                }
                else
                    toReturn = FailedBoolResponse(Languages.GetResourceText("ChangePassword_Failed"));

            }

            return toReturn;
        }
        public ITypedResponse<LoginUser> GetCurrentUserInfo()
        {
            string errMSg = string.Empty;
            MES.DTO.Library.Identity.LoginUser currentuser = new MES.DTO.Library.Identity.LoginUser();


            using (var db = new ApplicationDbContext())
            {

                var userlist = db.Users;

                if (CurrentUser == null)
                {
                    if (!(System.Threading.Thread.CurrentPrincipal).GetType().Equals(typeof(System.Security.Claims.ClaimsPrincipal)))
                        System.Threading.Thread.CurrentPrincipal = System.Web.HttpContext.Current.User;
                    string name = "";
                    try
                    {
                        name = System.Security.Claims.ClaimsPrincipal.Current.GetSubjectId();
                    }
                    catch
                    {
                        name = "anonymous";
                    }
                    currentuser = userlist.Where(item => item.Id == name)

                         .Select(users => new MES.DTO.Library.Identity.LoginUser
                         {
                             UserId = users.Id,
                             FirstName = users.FirstName,
                             LastName = users.LastName,
                             Email = users.Email,
                             UserName = users.UserName,
                             RoleId = users.RoleId,
                             FullName = users.FirstName + " " + users.LastName

                         }).SingleOrDefault();
                }
                else
                {

                    currentuser = userlist.Where(item => item.Id == CurrentUser)

                          .Select(users => new MES.DTO.Library.Identity.LoginUser
                          {
                              UserId = users.Id,
                              FirstName = users.FirstName,
                              LastName = users.LastName,
                              Email = users.Email,
                              UserName = users.UserName,
                              RoleId = users.RoleId,
                              FullName = users.FirstName + " " + users.LastName

                          }).SingleOrDefault();
                }
            }
            if (currentuser != null)
            {
                /*Get Designation List*/
                currentuser.lstUserDesignation = new List<DTO.Library.UserManagement.UserDesignationMappings>();
                DataContext.DesignationMappings.Where(dm => dm.UserId == CurrentUser).ToList().ForEach(d => currentuser.lstUserDesignation.Add(
                    new DTO.Library.UserManagement.UserDesignationMappings()
                    {
                        Id = d.DesignationId

                    }));

                /*Get currentuser is AssignedTo which Users List*/
                currentuser.lstAssignedTo = new List<DTO.Library.Identity.LoginUser>();
                DataContext.UserPreferences.Where(dm => dm.AssignmentId == CurrentUser).ToList().ForEach(d => currentuser.lstAssignedTo.Add(
                    new DTO.Library.Identity.LoginUser()
                    {
                        UserId = d.UserId

                    }));
                Dictionary<string, string> controllerAction = new Dictionary<string, string>();

                currentuser.Preferences = DataContext.UserPreferences.Where(dm => dm.UserId == CurrentUser)
                     .Select(users => new MES.DTO.Library.UserManagement.Preferences
                     {
                         Id = users.Id,
                         AssignmentId = users.AssignmentId,
                         DefaultLandingPageId = users.DefaultLandingPageId,
                         UserId = users.UserId

                     }).SingleOrDefault();

                if (currentuser.Preferences != null && currentuser.Preferences.DefaultLandingPageId.HasValue)
                {
                    controllerAction = GetControllerAction(currentuser.Preferences.DefaultLandingPageId.Value);

                    foreach (KeyValuePair<string, string> pair in controllerAction)
                    {
                        currentuser.Preferences.DefaultController = pair.Key;
                        currentuser.Preferences.DefaultAction = pair.Value;
                    }
                }

                /*Get currentuser is AssignedTo which Users List*/
                RoleManagement.Role roleObj = new RoleManagement.Role();
                currentuser.SecurityObjects = roleObj.GetSecurityObjects();
                currentuser.DefaultObjectId = roleObj.FindById(currentuser.RoleId).Result.DefaultObjectId;
                if (currentuser.DefaultObjectId.HasValue && currentuser.DefaultObjectId > 0)
                {
                    controllerAction = new Dictionary<string, string>();
                    controllerAction = GetControllerAction(currentuser.DefaultObjectId.Value);

                    foreach (KeyValuePair<string, string> pair in controllerAction)
                    {
                        currentuser.DefaultController = pair.Key;
                        currentuser.DefaultAction = pair.Value;
                    }
                }
            }

            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.Identity.LoginUser>(errMSg, currentuser);
            return response;
        }

        private Dictionary<string, string> GetControllerAction(int id)
        {
            Dictionary<string, string> controllerAction = new Dictionary<string, string>();
            string defaultAction = string.Empty, defaultController = string.Empty;
            switch (id)
            {
                case 9:
                    defaultAction = "RFQ";
                    defaultController = Constants.RFQController;

                    break;
                case 72:
                    defaultAction = "Quote";
                    defaultController = Constants.QuoteController;

                    break;
                case 81:
                    defaultAction = "Shipment";
                    defaultController = Constants.ShipmentController;

                    break;
                case 105:
                    defaultAction = "APQP";
                    defaultController = Constants.APQPController;

                    break;
                case 126:
                    defaultAction = "Dashboard";
                    defaultController = Constants.DashboardController;

                    break;
                default:
                    break;
            }
            controllerAction.Add(defaultController, defaultAction);
            return controllerAction;
        }

        #region Page Methods
        public ITypedResponse<List<MES.DTO.Library.Identity.LoginUser>> GetUsers(IPage<MES.DTO.Library.Identity.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.Identity.LoginUser> lstusers = new List<DTO.Library.Identity.LoginUser>();
            MES.DTO.Library.Identity.LoginUser loginUser;
            this.RunOnDB(context =>
            {
                var userList = context.SearchUsers(paging.Criteria.UserName, paging.Criteria.FirstName, paging.Criteria.LastName, paging.Criteria.Active, paging.Criteria.RoleId, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (userList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in userList)
                    {
                        loginUser = new MES.DTO.Library.Identity.LoginUser();
                        loginUser.UserId = item.Id;
                        loginUser.FullName = item.FirstName + " " + item.LastName;
                        loginUser.FirstName = item.FirstName;
                        loginUser.LastName = item.LastName;
                        loginUser.UserName = item.UserName;
                        loginUser.Email = item.Email;
                        loginUser.Active = item.Active;
                        loginUser.RoleId = item.RoleId;
                        loginUser.RoleName = item.RoleName;
                        lstusers.Add(loginUser);
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Identity.LoginUser>>(errMSg, lstusers);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

        public ITypedResponse<bool?> Delete(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                var userToBeDeleted = db.Users.Where(a => a.Id == id).SingleOrDefault();
                if (userToBeDeleted != null)
                {
                    userToBeDeleted.UpdatedBy = CurrentUser;
                    userToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    userToBeDeleted.IsDeleted = true;
                    db.Entry(userToBeDeleted).State = EntityState.Modified;
                    db.SaveChanges();
                    return SuccessBoolResponse(Languages.GetResourceText("UserDeletedSuccess"));
                }
                else
                {
                    return FailedBoolResponse(Languages.GetResourceText("UserNotExists"));
                }
            }
        }

        public ITypedResponse<MES.DTO.Library.Identity.LoginUser> FindById(string id)
        {
            string errMSg = string.Empty;
            MES.DTO.Library.Identity.LoginUser userInfo = new MES.DTO.Library.Identity.LoginUser();
            this.RunOnDB(context =>
            {
                var user = context.GetUserInfo(id);
                if (user == null)
                    errMSg = Languages.GetResourceText("UserNotExists");
                else
                {
                    foreach (var item in user)
                    {
                        userInfo.UserId = id;
                        userInfo.FirstName = item.FirstName;
                        userInfo.MiddleName = item.MiddleName;
                        userInfo.LastName = item.LastName;
                        userInfo.FullName = item.FirstName + " " + item.LastName;
                        userInfo.Email = item.Email;
                        userInfo.UserName = item.UserName;
                        userInfo.RoleId = item.RoleId;
                        userInfo.Active = item.Active;
                        userInfo.IsRFQCoordinator = item.IsRFQCoordinator;
                        userInfo.AddressLine1 = item.AddressLine1;
                        userInfo.AddressLine2 = item.AddressLine2;
                        userInfo.City = item.City;
                        userInfo.State = item.State;
                        userInfo.CountryId = item.CountryId;
                        userInfo.ZipCode = item.ZipCode;
                        userInfo.PrefixId = item.PrefixId;
                        userInfo.GenderId = item.GenderId;
                        userInfo.SupplierId = item.SupplierId.HasValue ? item.SupplierId.Value : (int?)null;
                        userInfo.CountryId = item.CountryId;

                        /*Get Designation List*/
                        userInfo.lstUserDesignation = new List<DTO.Library.UserManagement.UserDesignationMappings>();
                        context.DesignationMappings.Where(dm => dm.UserId == id).ToList().ForEach(d => userInfo.lstUserDesignation.Add(
                            new DTO.Library.UserManagement.UserDesignationMappings()
                            {
                                Id = d.DesignationId

                            }));

                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.Identity.LoginUser>(errMSg, userInfo);
            return response;
        }

        public LoginUser Get(string id)
        {
            using (var db = new ApplicationDbContext())
            {

                var userlist = db.Users;
                LoginUser selecteduser = new LoginUser();
                selecteduser = userlist.Where(item => item.Id == id)

                      .Select(users => new MES.DTO.Library.Identity.LoginUser
                      {
                          UserId = users.Id,
                          FirstName = users.FirstName,
                          LastName = users.LastName,
                          Email = users.Email,
                          UserName = users.UserName,
                          RoleId = users.RoleId
                          //UserRole = users.Roles.FirstOrDefault().Role.Name
                      }).SingleOrDefault();
                return selecteduser;

            }
        }

        public ITypedResponse<string> Save(MES.DTO.Library.Identity.LoginUser user)
        {
            ITypedResponse<string> returnVal;
            if (!string.IsNullOrEmpty(user.UserId))
                returnVal = UpdateUser(user);
            else
                returnVal = CreateUser(user);
            return returnVal;
        }

        private ITypedResponse<string> CreateUser(DTO.Library.Identity.LoginUser user)
        {
            string errMSg = null;
            string successMsg = null;

            MES.Identity.Data.Library.User applicationUser = new MES.Identity.Data.Library.User
            {
                DisplayName = user.UserName,
                Culture = user.Culture ?? MES.DTO.Library.Constants.MESConstants.DefaultCulture,
                UserName = user.UserName,
                TitleId = user.PrefixId.HasValue ? user.PrefixId : 0,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                GenderId = user.GenderId.Value,
                Email = user.Email,
                SupplierId = user.SupplierId.HasValue ? user.SupplierId.Value : (int?)null,
                Active = user.Active.Value,
                AddressLine1 = user.AddressLine1,
                AddressLine2 = user.AddressLine2,
                City = user.City,
                State = user.State,
                ZipCode = user.ZipCode,
                CountryId = user.CountryId.Value,
                IsRFQCoordinator = user.IsRFQCoordinator.Value,
                RoleId = user.RoleId,
                CreatedBy = CurrentUser,
                CreatedDate = AuditUtils.GetCurrentDateTime()

            };
            string sendEmailPswd = user.Password;
            string password = user.Password;
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                password = GetPassword(8);
                user.Password = password;
            }
            try
            {

                password = userManager.PasswordHasher.HashPassword(user.Password);
                IdentityResult savedResult = userManager.Create(applicationUser, user.Password);
                if (savedResult.Succeeded)
                {
                    successMsg = Languages.GetResourceText("UserSavedSuccess");

                    //string roleId = roleManager.FindById(Business.Library.Enums.LoginRole.SuperAdmin.ToString()).Id;
                    //userManager.AddToRole(applicationUser.Id, Business.Library.Enums.LoginRole.SuperAdmin.ToString());

                    //user = Get(applicationUser.Id);
                    user.UserId = applicationUser.Id;
                    InsertUserDesignations(user.UserId, user.lstUserDesignation);
                    user.Password = sendEmailPswd;
                    SendEmail(user);
                }
                else
                    errMSg = string.Join(",", savedResult.Errors.ToArray());
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        throw ex;// ("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
            }

            return SuccessOrFailedResponse<string>(errMSg, (user.UserId), successMsg);
        }

        private bool SendEmail(LoginUser user)
        {
            bool IsSuccess = false;

            DTO.Library.Common.EmailData emailData = new DTO.Library.Common.EmailData();
            emailData.EmailBody = GetRfqEmailBody("UserRegistration.htm")
                 .Replace("<%Name%>", user.FirstName + " " + user.LastName)
                 .Replace("<%URL%>", System.Configuration.ConfigurationManager.AppSettings["WebsiteURL"])
                 .Replace("<%UserName%>", user.UserName)
                 .Replace("<%Password%>", user.Password);

            MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
            List<string> lstToAddress = new List<string>();

            emailData.EmailSubject = Languages.GetResourceText("MESUserRegistration");
            lstToAddress.Add(user.Email);

            emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailData.EmailBody, out IsSuccess, null);

            return IsSuccess;
        }
        /// <summary>
        /// Gets the RFQ email body.
        /// </summary>
        /// <returns></returns>
        private string GetRfqEmailBody(string fileName)
        {
            string templatePath = string.Empty;

            templatePath = System.Web.HttpContext.Current.Server.MapPath("~/") + Constants.EmailTemplateFolder + "UserRegistration.htm";

            string emailBody = string.Empty;
            using (StreamReader reader = new StreamReader(templatePath))
            {
                emailBody = reader.ReadToEnd();
                reader.Close();
            }
            return emailBody;

        }
        private void InsertUserDesignations(string userId, List<UserDesignationMappings> list)
        {
            /*Delete All StatusAssociatedTo*/
            var deleteUserDesignationList = this.DataContext.DesignationMappings.Where(a => a.UserId == userId).ToList();
            foreach (var item in deleteUserDesignationList)
            {
                this.DataContext.DesignationMappings.Remove(item);
            }

            /*Insert StatusAssociatedTo*/
            MES.Data.Library.DesignationMapping dboUserDesignation = null;
            if (list != null && list.Count > 0)
            {
                bool AnyDesignationTo = false;
                foreach (var designation in list)
                {
                    if (designation.Id != 0)
                    {
                        AnyDesignationTo = true;
                        dboUserDesignation = new MES.Data.Library.DesignationMapping();
                        dboUserDesignation.DesignationId = Convert.ToInt16(designation.Id);
                        dboUserDesignation.UserId = userId;
                        dboUserDesignation.CreatedBy = CurrentUser;
                        dboUserDesignation.CreatedDate = AuditUtils.GetCurrentDateTime();
                        this.DataContext.DesignationMappings.Add(dboUserDesignation);
                    }
                }
                if (AnyDesignationTo)
                    this.DataContext.SaveChanges();
            }
        }

        private ITypedResponse<string> UpdateUser(MES.DTO.Library.Identity.LoginUser user)
        {
            int resultresponse = -1;
            MES.Identity.Data.Library.User updateUser;
            string errMSg = null;
            string successMsg = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                DbSet<MES.Identity.Data.Library.User> users = db.Set<MES.Identity.Data.Library.User>();
                updateUser = users.Find(user.UserId);

                updateUser.TitleId = user.PrefixId.Value;
                updateUser.FirstName = user.FirstName;
                updateUser.MiddleName = user.MiddleName;
                updateUser.LastName = user.LastName;
                updateUser.GenderId = user.GenderId.Value;
                updateUser.Email = user.Email;
                updateUser.SupplierId = user.SupplierId.HasValue ? user.SupplierId.Value : (int?)null;
                updateUser.Active = user.Active.Value;
                updateUser.AddressLine1 = user.AddressLine1;
                updateUser.AddressLine2 = user.AddressLine2;
                updateUser.City = user.City;
                updateUser.State = user.State;
                updateUser.ZipCode = user.ZipCode;
                updateUser.CountryId = user.CountryId.Value;
                updateUser.IsRFQCoordinator = user.IsRFQCoordinator.Value;
                updateUser.UpdatedBy = CurrentUser;
                updateUser.UpdatedDate = AuditUtils.GetCurrentDateTime();
                updateUser.RoleId = user.RoleId.Value;
                users.Attach(updateUser);
                db.Entry(updateUser).State = System.Data.Entity.EntityState.Modified;
                resultresponse = db.SaveChangesAsync().Result;

                if (resultresponse > 0)
                {
                    InsertUserDesignations(user.UserId, user.lstUserDesignation);
                    successMsg = Languages.GetResourceText("UserUpdatedSuccess");
                }
            }

            return SuccessOrFailedResponse<string>(errMSg, (updateUser.Id), successMsg);
        }

        public ITypedResponse<bool?> UserNameExists(string userName)
        {
            bool userNameExists = false;

            userNameExists = userManager.Users.Any(usr => usr.UserName == userName && usr.IsDeleted == false);

            if (userNameExists)
                return SuccessBoolResponse(Languages.GetResourceText("UserNameExists"));
            else
                return FailedBoolResponse(Languages.GetResourceText("UserNameNotExists"));
        }
        private string GetPassword(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var newPassword = new String(stringChars);

            return newPassword;
        }
        #endregion

        public ITypedResponse<List<DTO.Library.Identity.LoginUser>> Search(IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }
        public MES.DTO.Library.Identity.LoginUser GetUserInfoById(string userId)
        {
            string errMSg = string.Empty;
            MES.DTO.Library.Identity.LoginUser selectedUser = new MES.DTO.Library.Identity.LoginUser();
            using (var db = new ApplicationDbContext())
            {
                var userlist = db.Users;
                selectedUser = userlist.Where(item => item.Id == userId)
                    .Select(users => new MES.DTO.Library.Identity.LoginUser
                      {
                          UserId = users.Id,
                          FirstName = users.FirstName,
                          LastName = users.LastName,
                          Email = users.Email,
                          UserName = users.UserName,
                          Culture = users.Culture,
                          RoleId = users.RoleId,
                          FullName = users.FirstName + " " + users.LastName
                      }).SingleOrDefault();

            }
            if (selectedUser != null)
            {
                /*Get Designation List*/
                selectedUser.lstUserDesignation = new List<DTO.Library.UserManagement.UserDesignationMappings>();
                DataContext.DesignationMappings.Where(dm => dm.UserId == CurrentUser).ToList().ForEach(d => selectedUser.lstUserDesignation.Add(
                    new DTO.Library.UserManagement.UserDesignationMappings()
                    {
                        Id = d.DesignationId

                    }));

                /*Get currentuser is AssignedTo which Users List*/
                selectedUser.lstAssignedTo = new List<DTO.Library.Identity.LoginUser>();
                DataContext.UserPreferences.Where(dm => dm.AssignmentId == CurrentUser).ToList().ForEach(d => selectedUser.lstAssignedTo.Add(
                    new DTO.Library.Identity.LoginUser()
                    {
                        UserId = d.UserId

                    }));
                Dictionary<string, string> controllerAction = new Dictionary<string, string>();

                selectedUser.Preferences = DataContext.UserPreferences.Where(dm => dm.UserId == CurrentUser)
                     .Select(users => new MES.DTO.Library.UserManagement.Preferences
                     {
                         Id = users.Id,
                         AssignmentId = users.AssignmentId,
                         DefaultLandingPageId = users.DefaultLandingPageId,
                         UserId = users.UserId

                     }).SingleOrDefault();

                if (selectedUser.Preferences != null && selectedUser.Preferences.DefaultLandingPageId.HasValue)
                {
                    controllerAction = GetControllerAction(selectedUser.Preferences.DefaultLandingPageId.Value);

                    foreach (KeyValuePair<string, string> pair in controllerAction)
                    {
                        selectedUser.Preferences.DefaultController = pair.Key;
                        selectedUser.Preferences.DefaultAction = pair.Value;
                    }
                }

                /*Get currentuser is AssignedTo which Users List*/
                RoleManagement.Role roleObj = new RoleManagement.Role();
                selectedUser.SecurityObjects = roleObj.GetSecurityObjects();
                selectedUser.DefaultObjectId = roleObj.FindById(selectedUser.RoleId).Result.DefaultObjectId;
                if (selectedUser.DefaultObjectId.HasValue && selectedUser.DefaultObjectId > 0)
                {
                    controllerAction = new Dictionary<string, string>();
                    controllerAction = GetControllerAction(selectedUser.DefaultObjectId.Value);

                    foreach (KeyValuePair<string, string> pair in controllerAction)
                    {
                        selectedUser.DefaultController = pair.Key;
                        selectedUser.DefaultAction = pair.Value;
                    }
                }
            }
            return selectedUser;
        }
        public void UpdateUserInfo(MES.DTO.Library.Identity.LoginUser objUser)
        {
            string errMSg = string.Empty;
            using (var db = new ApplicationDbContext())
            {
                var userInfo = db.Users.Where(u => u.Id == objUser.UserId).SingleOrDefault();
                if (userInfo != null)
                {
                    userInfo.Culture = objUser.Culture;
                    userInfo.UpdatedBy = CurrentUser;
                    userInfo.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    db.Entry(userInfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        public ITypedResponse<DTO.Library.Identity.SearchCriteria> DefaultSearchCriteria()
        {
            DTO.Library.Identity.SearchCriteria criteria = new DTO.Library.Identity.SearchCriteria();
            criteria.SAMItems = new List<ItemList>();
            string errMSg = string.Empty;
            ItemList lstItem = null;
            LoginUser currentUser = GetCurrentUser;
            if (currentUser != null)
            {
                if (currentUser.lstUserDesignation.Count > 0)
                {
                    if (currentUser.lstUserDesignation.Where(item => item.Id == Convert.ToInt32(DesignationFixedId.RFQCoordinatorEstimator)).Any()) { criteria.rfqCoordinator = CurrentUser; }

                    if (currentUser.lstUserDesignation.Where(item => item.Id == Convert.ToInt32(DesignationFixedId.AccountManager)).Any())
                    {
                        lstItem = new ItemList();
                        lstItem.Id = CurrentUser;
                        lstItem.Name = currentUser.UserName;
                        criteria.SAMItems.Add(lstItem);
                        criteria.SAMUserId = CurrentUser;
                    }
                    if (currentUser.lstUserDesignation.Where(item => item.Id == Convert.ToInt32(DesignationFixedId.APQPQualityEngineer)).Any()) { criteria.APQPQualityEngineerId = CurrentUser; }

                    if (currentUser.lstUserDesignation.Where(item => item.Id == Convert.ToInt32(DesignationFixedId.SupplyChainCoordinator)).Any()) { criteria.SupplyChainCoordinatorId = CurrentUser; }
                }

                if (currentUser.lstAssignedTo.Count > 0)
                {
                    criteria.AssignedToItems = new List<ItemList>();
                    foreach (var item in currentUser.lstAssignedTo)
                    {
                        IUserManagementRepository userRep = new UserManagement();
                        MES.DTO.Library.Identity.LoginUser assignedToUser = userRep.FindById(item.UserId).Result;
                        if (assignedToUser.lstUserDesignation.Count > 0)
                        {
                            if (currentUser.lstUserDesignation.Where(d => d.Id == Convert.ToInt32(DesignationFixedId.AccountManager)).Any())
                            {
                                lstItem = new ItemList();
                                lstItem.Id = CurrentUser;
                                lstItem.Name = currentUser.UserName;
                                criteria.SAMItems.Add(lstItem);
                                if (!string.IsNullOrEmpty(criteria.SAMUserId))
                                    criteria.SAMUserId = CurrentUser;
                            }
                        }


                        lstItem = new ItemList();
                        lstItem.Id = CurrentUser;
                        lstItem.Name = currentUser.UserName;
                        criteria.AssignedToItems.Add(lstItem);
                    }
                }
            }


            //criteria.isFirstTimeLoad = false;

            var response = SuccessOrFailedResponse<DTO.Library.Identity.SearchCriteria>(errMSg, criteria);
            return response;
        }

    }
}
