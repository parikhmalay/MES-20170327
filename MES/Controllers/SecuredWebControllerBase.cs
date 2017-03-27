using System;
using Ninject;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MES.Extended;
using NPE.Core.Extensions;
using MES.DTO.Library.RoleManagement;

namespace MES.Controllers
{
    [Authorize]
    public class SecuredWebControllerBase : WebControllerBase
    {
        [Inject]
        public MES.Business.Repositories.UserManagement.IPreferencesRepository PreferencesRepository { get; set; }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!User.Identity.IsAuthenticated)
            {
                RedirectToAction("Login", "Account", new { area = "" });
            }
            else
            {
                try
                {
                    //var userPreferenceData = new MES.Business.Library.BO.UserManagement.Preferences();
                    //var userPreferencePagesize = userPreferenceData.FindById(System.Security.Claims.ClaimsPrincipal.Current.GetSubjectId()).Result.PagesizeId;

                    var userPreference = this.Resolve<MES.Business.Repositories.UserManagement.IPreferencesRepository>(PreferencesRepository).FindById(System.Security.Claims.ClaimsPrincipal.Current.GetSubjectId()).Result;
                    ViewData.Add(new KeyValuePair<string, object>("userPreferencePagesize", userPreference.PagesizeId == null ? 5 : userPreference.PagesizeId));
                    ViewData.Add(new KeyValuePair<string, object>("userPreferenceCulture", userPreference.CulturePreference == "" ? MES.DTO.Library.Constants.MESConstants.DefaultCulture : userPreference.CulturePreference));

                    var currentUser = MES.Extended.ControllerExtensions.GetCurrentUser();
                    if (currentUser != null && ViewData["ParentId"] != null)
                    {
                        int ParentId = (int)ViewData["ParentId"];
                        List<MES.DTO.Library.RoleManagement.RoleObjectPrivilege> currentObjects = new List<DTO.Library.RoleManagement.RoleObjectPrivilege>();
                        MES.Business.Library.BO.RoleManagement.Role objRole = new Business.Library.BO.RoleManagement.Role();
                        currentObjects = GetChildObject(ParentId, currentUser.SecurityObjects);
                        if (currentObjects.Count > 0)
                        {
                            currentObjects.Add(currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == ParentId).Single());   //to get the root object for list page
                            if (ParentId == Convert.ToInt32(MES.Business.Library.Enums.Pages.RFQ))//9
                            {       //when getting RFQ page then we are also needed to get supplier quote and quote to customer object data.
                                //supplier quote
                                foreach (var item in currentUser.SecurityObjects.Where(securityObject => securityObject.ParentId == Convert.ToInt32(MES.Business.Library.Enums.Pages.RFQM)))//63
                                {
                                    currentObjects.Add(item);
                                }
                                currentObjects.Add(currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(MES.Business.Library.Enums.Pages.RFQM)).Single());//63
                                //quote to customer
                                foreach (var item in currentUser.SecurityObjects.Where(securityObject => securityObject.ParentId == Convert.ToInt32(MES.Business.Library.Enums.Pages.QuoteList)))//72
                                {
                                    currentObjects.Add(item);
                                }
                                currentObjects.Add(currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(MES.Business.Library.Enums.Pages.QuoteList)).Single());//72
                            }

                            //to get the top most parent of APQP for apqp, changerequest, defect tracking, document, reports, CAPA
                            if (ParentId == Convert.ToInt32(MES.Business.Library.Enums.Pages.APQP) //105
                                || ParentId == Convert.ToInt32(MES.Business.Library.Enums.Pages.ChangeRequest) //106
                                || ParentId == Convert.ToInt32(MES.Business.Library.Enums.Pages.DefectTracking) //112
                                || ParentId == Convert.ToInt32(MES.Business.Library.Enums.Pages.DocumentManagement) //115
                                || ParentId == Convert.ToInt32(MES.Business.Library.Enums.Pages.APQPReports) //116
                                || ParentId == Convert.ToInt32(MES.Business.Library.Enums.Pages.CAPA)) //146
                            {
                                currentObjects.Add(currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(MES.Business.Library.Enums.Pages.APQPDashboard)).Single());//92
                            }
                            //to get the apqp object for change request page
                            if (ParentId == Convert.ToInt32(MES.Business.Library.Enums.Pages.ChangeRequest) || ParentId == Convert.ToInt32(MES.Business.Library.Enums.Pages.QuoteList))//106 //72
                            {
                                currentObjects.Add(currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(MES.Business.Library.Enums.Pages.APQP)).Single());//105
                            }
                            //to get the shipment parent
                            if (ParentId == Convert.ToInt32(MES.Business.Library.Enums.Pages.ShipmentList))//81
                            {
                                currentObjects.Add(currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(MES.Business.Library.Enums.Pages.Shipment)).Single());//78
                            }

                            ViewData.Add(new KeyValuePair<string, object>("currentObjects", currentObjects));
                        }
                        else if (ParentId == Convert.ToInt32(MES.Business.Library.Enums.Pages.Dashboard))
                        {
                            currentObjects.Add(currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(MES.Business.Library.Enums.Pages.Dashboard)).Single());//126
                            ViewData.Add(new KeyValuePair<string, object>("currentObjects", currentObjects));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            base.OnActionExecuted(filterContext);
        }
        public List<RoleObjectPrivilege> GetChildObject(int? parentId, List<RoleObjectPrivilege> securityObjectList)
        {
            List<RoleObjectPrivilege> objectList = new List<RoleObjectPrivilege>();
            RoleObjectPrivilege roleObj = null;
            var list = securityObjectList.Where(i => i.ParentId == parentId);

            foreach (var item in list)
            {
                roleObj = new RoleObjectPrivilege();
                roleObj.Id = item.Id;
                roleObj.ObjectId = item.ObjectId;
                roleObj.ObjectName = item.ObjectName;
                roleObj.PageName = item.PageName;
                roleObj.ParentId = item.ParentId;
                roleObj.Parent = item.ParentId;
                roleObj.MenuClass = item.MenuClass;
                roleObj.HasChild = Convert.ToBoolean(item.HasChild);
                roleObj.PrivilegeId = item.PrivilegeId;
                roleObj.AllowConfidentialDocumentType = item.AllowConfidentialDocumentType;
                roleObj.AllowDeleteRecord = item.AllowDeleteRecord;
                roleObj.AllowExportToSAP = item.AllowExportToSAP;
                roleObj.AllowSendDataToSAP = item.AllowSendDataToSAP;
                roleObj.HasPricingFieldsAccess = item.HasPricingFieldsAccess;
                roleObj.childObject = GetChildObject(item.ObjectId, securityObjectList);
                objectList.Add(roleObj);
            }

            return objectList;
        }
    }
}