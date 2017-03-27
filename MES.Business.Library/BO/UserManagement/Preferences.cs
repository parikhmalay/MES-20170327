using MES.Business.Repositories.UserManagement;
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
using Account.DTO.Library;

namespace MES.Business.Library.BO.UserManagement
{
    public class Preferences : ContextBusinessBase, IPreferencesRepository
    {
        public Preferences()
            : base("Preferences")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.UserManagement.Preferences preferences)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.UserPreferences.AsNoTracking().Any(a => a.Id == preferences.Id && a.IsDeleted == false && a.Id != preferences.Id))
            {
                errMSg = Languages.GetResourceText("PreferencesExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.UserPreference();

                if (preferences.Id > 0)
                {
                    recordToBeUpdated = this.DataContext.UserPreferences.Where(a => a.Id == preferences.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("PreferencesNotExists");
                    else
                    {
                        recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                        recordToBeUpdated.UpdatedBy = CurrentUser;
                        this.DataContext.Entry(recordToBeUpdated).State = EntityState.Modified;
                    }
                }
                else
                {
                    recordToBeUpdated.CreatedBy = recordToBeUpdated.UpdatedBy = CurrentUser;
                    recordToBeUpdated.CreatedDate = AuditUtils.GetCurrentDateTime();
                    recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    this.DataContext.UserPreferences.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.UserId = preferences.UserId;
                    recordToBeUpdated.PagesizeId = preferences.PagesizeId;
                    recordToBeUpdated.DefaultLandingPageId = preferences.DefaultLandingPageId;
                    recordToBeUpdated.AssignmentId = preferences.AssignmentId;
                    this.DataContext.SaveChanges();
                    preferences.Id = recordToBeUpdated.Id;
                    if (preferences.Id > 0 && !string.IsNullOrEmpty(preferences.CulturePreference))
                    {
                        UserManagement objUserManagement = new UserManagement();
                        objUserManagement.UpdateUserInfo(new DTO.Library.Identity.LoginUser() { UserId = preferences.UserId, Culture = preferences.CulturePreference });
                    }
                    successMsg = Languages.GetResourceText("PreferencesSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, preferences.Id, successMsg);
        }
        public NPE.Core.ITypedResponse<DTO.Library.UserManagement.Preferences> FindById(string userId)
        {
            string errMSg = string.Empty;
            DTO.Library.UserManagement.Preferences preferences = new DTO.Library.UserManagement.Preferences();
            this.RunOnDB(context =>
            {
                var userPreferences = context.UserPreferences.Where(s => s.UserId == userId).SingleOrDefault();
                if (userPreferences == null)
                    errMSg = ""; //Languages.GetResourceText("PreferencesNotExists");
                else
                {
                    #region user preference detail
                    preferences.Id = userPreferences.Id;
                    preferences.UserId = userPreferences.UserId;
                    preferences.PagesizeId = userPreferences.PagesizeId;
                    preferences.DefaultLandingPageId = userPreferences.DefaultLandingPageId;
                    preferences.AssignmentId = userPreferences.AssignmentId;
                    UserManagement objUserManagement = new UserManagement();
                    preferences.CulturePreference = objUserManagement.GetUserInfoById(userId).Culture;
                    #endregion
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.UserManagement.Preferences>(errMSg, preferences);
            return response;
        }
        public List<DTO.Library.UserManagement.Preferences> FindByAssignmentId(string userId)
        {
            string errMSg = string.Empty;
            List<DTO.Library.UserManagement.Preferences> lstpreferences = new List<DTO.Library.UserManagement.Preferences>();
            DTO.Library.UserManagement.Preferences preferences = null;
            this.RunOnDB(context =>
            {
                var userPreferencesList = context.UserPreferences.Where(s => s.AssignmentId == userId).ToList();
                if (userPreferencesList == null)
                    errMSg = ""; //Languages.GetResourceText("PreferencesNotExists");
                else
                {
                    foreach (var userPreferences in userPreferencesList)
                    {

                        #region user preference detail
                        preferences = new DTO.Library.UserManagement.Preferences();
                        preferences.Id = userPreferences.Id;
                        preferences.UserId = userPreferences.UserId;
                        preferences.PagesizeId = userPreferences.PagesizeId;
                        preferences.DefaultLandingPageId = userPreferences.DefaultLandingPageId;
                        preferences.AssignmentId = userPreferences.AssignmentId;
                        UserManagement objUserManagement = new UserManagement();
                        preferences.CulturePreference = objUserManagement.GetUserInfoById(userId).Culture;
                        #endregion
                        lstpreferences.Add(preferences);
                    }
                }
            });
            return lstpreferences;
        }
        public NPE.Core.ITypedResponse<bool?> Delete(int preferencesId)
        {
            var PreferencesToBeDeleted = this.DataContext.UserPreferences.Where(a => a.Id == preferencesId).SingleOrDefault();
            if (PreferencesToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("PreferencesNotExists"));
            }
            else
            {
                PreferencesToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                PreferencesToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(PreferencesToBeDeleted).State = EntityState.Modified;
                PreferencesToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("PreferencesDeletedSuccess"));
            }
        }
        public NPE.Core.ITypedResponse<List<DTO.Library.UserManagement.Preferences>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

    }
}
