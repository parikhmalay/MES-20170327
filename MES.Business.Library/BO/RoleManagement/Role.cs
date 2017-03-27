using Account.DTO.Library;
using MES.Business.Repositories.Setup.SecondaryOperationDesc;
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
using MES.DTO.Library.RoleManagement;
using MES.Business.Repositories.RoleManagement;
using MES.Business.Repositories.UserManagement;
using MES.DTO.Library.Identity;
using MES.Business.Mapping.Extensions;

namespace MES.Business.Library.BO.RoleManagement
{
    public class Role : ContextBusinessBase, IRoleRepository
    {
        public Role() : base("Role") { }

        #region Role Security Objects
        private List<RoleObjectPrivilege> GetChildObject(int? parentId, List<Data.Library.GetSecurityObjectsByRole_Result> securityObjectList)
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
                roleObj.AllowConfidentialDocumentType = item.AllowConfidentialDocumentType.HasValue ? item.AllowConfidentialDocumentType.Value : false;
                roleObj.AllowDeleteRecord = item.AllowDeleteRecord.HasValue ? item.AllowDeleteRecord.Value : false;
                roleObj.AllowExportToSAP = item.AllowExportToSAP.HasValue ? item.AllowExportToSAP.Value : false;
                roleObj.AllowSendDataToSAP = item.AllowSendDataToSAP.HasValue ? item.AllowSendDataToSAP.Value : false;
                roleObj.AllowCheckNPIFStatus = item.AllowCheckNPIFStatus.HasValue ? item.AllowCheckNPIFStatus.Value : false;
              
                roleObj.HasPricingFieldsAccess = item.HasPricingFieldsAccess.HasValue ? item.HasPricingFieldsAccess.Value : false;
                roleObj.HasPricingFieldsAccess = item.HasPricingFieldsAccess.HasValue ? item.HasPricingFieldsAccess.Value : false;
                roleObj.childObject = GetChildObject(item.ObjectId, securityObjectList);
                objectList.Add(roleObj);
            }

            return objectList;
        }
        public List<RoleObjectPrivilege> GetChildObjectForRole(int? parentId, List<Data.Library.GetSecurityObjects_Result> securityObjectList)
        {
            List<RoleObjectPrivilege> objectList = new List<RoleObjectPrivilege>();
            RoleObjectPrivilege roleObj = null;
            var list = securityObjectList.Where(i => i.ParentId == parentId).ToList();
            foreach (var item in list)
            {

                roleObj = new RoleObjectPrivilege();
                roleObj.Id = 0;
                //roleObj.Id = item.Id;
                roleObj.ObjectId = item.ObjectId;
                roleObj.ObjectName = item.ObjectName;
                roleObj.PageName = item.PageName;
                roleObj.ParentId = item.ParentId;
                roleObj.Parent = item.ParentId;
                roleObj.MenuClass = item.MenuClass;
                roleObj.HasChild = Convert.ToBoolean(item.HasChild);
                roleObj.PrivilegeId = item.PrivilegeId;
                roleObj.AllowConfidentialDocumentType = item.AllowConfidentialDocumentType.HasValue ? item.AllowConfidentialDocumentType.Value : false;
                roleObj.AllowDeleteRecord = item.AllowDeleteRecord.HasValue ? item.AllowDeleteRecord.Value : false;
                roleObj.AllowExportToSAP = item.AllowExportToSAP.HasValue ? item.AllowExportToSAP.Value : false;
                roleObj.AllowSendDataToSAP = item.AllowSendDataToSAP.HasValue ? item.AllowSendDataToSAP.Value : false;
                roleObj.HasPricingFieldsAccess = item.HasPricingFieldsAccess.HasValue ? item.HasPricingFieldsAccess.Value : false;
                roleObj.childObject = GetChildObjectForRole(item.ObjectId, securityObjectList);
                objectList.Add(roleObj);

            }

            return objectList;
        }
        #endregion Role Security Objects

        #region Menu Security Objects
        public List<RoleObjectPrivilege> GetMenuList()
        {
            List<RoleObjectPrivilege> lstObject = new List<RoleObjectPrivilege>();
            this.RunOnDB(context =>
            {
                var securityObjectList = context.GetSecurityObjects(CurrentUserInfo.GetName()).ToList();//CurrentUser
                if (securityObjectList == null)
                { }//errMSg = Languages.GetResourceText("RecordNotExist");
                else  //setup total records
                {
                    securityObjectList = securityObjectList.Where(item => item.IsMenuItem == true).ToList();

                    lstObject = GetChildObject(null, securityObjectList);
                }
            });

            return (lstObject);
        }
        public List<RoleObjectPrivilege> GetChildObject(int? parentId, List<Data.Library.GetSecurityObjects_Result> securityObjectList)
        {
            string pagePrivilege = Constants.NONEPRIVILEGE;
            List<RoleObjectPrivilege> objectList = new List<RoleObjectPrivilege>();
            RoleObjectPrivilege roleObj = null;
            var list = securityObjectList.Where(i => i.ParentId == parentId).ToList();
            foreach (var item in list)
            {
                if (parentId != null || (item.ObjectId == Convert.ToInt32(MES.Business.Library.Enums.Pages.RFQM)
                    || item.ObjectId == Convert.ToInt32(MES.Business.Library.Enums.Pages.Dashboard)
                    || item.ObjectId == Convert.ToInt32(MES.Business.Library.Enums.Pages.Setup) || item.ObjectId == Convert.ToInt32(MES.Business.Library.Enums.Pages.APQPDashboard)
                    || item.ObjectId == Convert.ToInt32(MES.Business.Library.Enums.Pages.BusinessPartners) || item.ObjectId == Convert.ToInt32(MES.Business.Library.Enums.Pages.Shipment)))
                {
                    pagePrivilege = item.Privilege;
                    switch (pagePrivilege)
                    {
                        case Constants.NONEPRIVILEGE: break;
                        case Constants.READPRIVILEGE:
                        case Constants.WRITEPRIVILEGE:

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
                            roleObj.AllowConfidentialDocumentType = item.AllowConfidentialDocumentType.HasValue ? item.AllowConfidentialDocumentType.Value : false;
                            roleObj.AllowDeleteRecord = item.AllowDeleteRecord.HasValue ? item.AllowDeleteRecord.Value : false;
                            roleObj.AllowExportToSAP = item.AllowExportToSAP.HasValue ? item.AllowExportToSAP.Value : false;
                            roleObj.AllowSendDataToSAP = item.AllowSendDataToSAP.HasValue ? item.AllowSendDataToSAP.Value : false;
                            roleObj.HasPricingFieldsAccess = item.HasPricingFieldsAccess.HasValue ? item.HasPricingFieldsAccess.Value : false;
                            roleObj.childObject = GetChildObject(item.ObjectId, securityObjectList);
                            objectList.Add(roleObj);

                            break;
                        default:
                            break;
                    }
                }
            }

            return objectList;
        }
        #endregion

        #region
        public List<RoleObjectPrivilege> GetSecurityObjects()
        {
            List<RoleObjectPrivilege> lstObject = new List<RoleObjectPrivilege>();
            RoleObjectPrivilege objItem = null;
            this.RunOnDB(context =>
            {
                var securityObjectList = context.GetSecurityObjects(CurrentUserInfo.GetName()).ToList();
                if (securityObjectList == null)
                { }
                else
                {
                    foreach (var item in securityObjectList)
                    {
                        objItem = new RoleObjectPrivilege();
                        objItem = ObjectLibExtensions.AutoConvert<DTO.Library.RoleManagement.RoleObjectPrivilege>(item);

                        lstObject.Add(objItem);
                    }

                }
            });

            return (lstObject);
        }
        #endregion

        #region Page Methods

        public NPE.Core.ITypedResponse<List<DTO.Library.RoleManagement.Role>> GetRoleList(NPE.Core.IPage<DTO.Library.RoleManagement.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RoleManagement.Role> lstRole = new List<DTO.Library.RoleManagement.Role>();
            DTO.Library.RoleManagement.Role role;
            this.RunOnDB(context =>
             {
                 var Roles = context.GetRoles(paging.Criteria.RoleId == 0 ? (int?)null : paging.Criteria.RoleId, paging.Criteria.Active, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (Roles == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in Roles)
                     {
                         role = new DTO.Library.RoleManagement.Role();

                         role.RoleName = item.RoleName;
                         role.ObjectName = item.ObjectName;
                         role.RoleId = item.RoleId;
                         role.Status = item.Active ? Constants.ACTIVETEXT : Constants.INACTIVETEXT;
                         lstRole.Add(role);
                     }
                 }
             });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RoleManagement.Role>>(errMSg, lstRole);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

        public ITypedResponse<DTO.Library.RoleManagement.Role> FindById(int? id)
        {
            string errMSg = null;
            DTO.Library.RoleManagement.Role roleItem = new DTO.Library.RoleManagement.Role();

            this.RunOnDB(context =>
            {
                var role = context.Roles.Where(item => item.Id == id).SingleOrDefault();
                if (role == null)
                {
                    roleItem.IsActive = true;
                    roleItem.DefaultObjectId = Convert.ToInt32(MES.Business.Library.Enums.Pages.RFQ);
                }
                else  //setup total records
                {
                    #region Role Details
                    roleItem.RoleId = role.Id;
                    roleItem.RoleName = role.RoleName;
                    roleItem.IsActive = role.Active;
                    roleItem.DefaultObjectId = role.DefaultObjectId;
                    #endregion
                }
                #region Role List

                if (id == null)
                {
                    var securityObjectList = context.GetSecurityObjects(CurrentUserInfo.GetName()).ToList();
                    foreach (var item in securityObjectList)
                    {
                        item.PrivilegeId = Convert.ToInt32(MES.Business.Library.Enums.Privileges.None);
                    }
                    roleItem.lstRoleObjectPrivilege = GetChildObjectForRole(null, securityObjectList);
                }
                else
                {
                    var securityObjectList = context.GetSecurityObjectsByRole(id.Value).ToList();

                    if (securityObjectList == null)
                    {
                        errMSg = Languages.GetResourceText("RecordNotExist");
                    }
                    else  //setup total records
                    {
                        roleItem.lstRoleObjectPrivilege = GetChildObject(null, securityObjectList);
                    }
                }
                #endregion

            });

            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.RoleManagement.Role>(errMSg, roleItem);

            return response;
        }

        public ITypedResponse<int?> Save(DTO.Library.RoleManagement.Role data)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.Role();
            bool IsNewRecord = true;
            if (data.RoleId > 0)
            {
                recordToBeUpdated = this.DataContext.Roles.Where(a => a.Id == data.RoleId).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("RoleNotExists");
                else
                {
                    IsNewRecord = false;
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
                this.DataContext.Roles.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                #region Save general details
                recordToBeUpdated.RoleName = data.RoleName;
                recordToBeUpdated.Active = data.IsActive;
                recordToBeUpdated.DefaultObjectId = data.DefaultObjectId;
                this.DataContext.SaveChanges();
                data.RoleId = recordToBeUpdated.Id;
                #endregion

                #region
                BO.RoleManagement.Role roleObj = new Role();
                foreach (DTO.Library.RoleManagement.RoleObjectPrivilege roleObjPriv in data.lstRoleObjectPrivilege)
                {
                    roleObjPriv.RoleId = data.RoleId;
                    SaveRoleObjectPrivilege(roleObjPriv);
                    if (roleObjPriv.HasChild)
                        roleObj.RoleChildObject(roleObjPriv.childObject, roleObjPriv.RoleId);
                }
                #endregion


                successMsg = Languages.GetResourceText("RoleSavedSuccess");
            }
            return SuccessOrFailedResponse<int?>(errMSg, data.RoleId, successMsg);
        }

        public NPE.Core.ITypedResponse<int?> SaveRoleObjectPrivilege(DTO.Library.RoleManagement.RoleObjectPrivilege roleObjPriv)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.RoleObjectPrivilege();

            if (roleObjPriv.Id > 0)
            {
                recordToBeUpdated = this.DataContext.RoleObjectPrivileges.Where(a => a.Id == roleObjPriv.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("RoleObjectPrivilegeNotExists");
                else
                {
                    this.DataContext.Entry(recordToBeUpdated).State = EntityState.Modified;
                }
            }
            else
            {
                this.DataContext.RoleObjectPrivileges.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                #region Save general details
                recordToBeUpdated.RoleId = roleObjPriv.RoleId;
                recordToBeUpdated.ObjectId = roleObjPriv.ObjectId;
                recordToBeUpdated.PrivilegeId = roleObjPriv.PrivilegeId.Value;
                recordToBeUpdated.HasPricingFieldsAccess = roleObjPriv.HasPricingFieldsAccess;
                recordToBeUpdated.AllowExportToSAP = roleObjPriv.AllowExportToSAP;
                recordToBeUpdated.AllowSendDataToSAP = roleObjPriv.AllowSendDataToSAP;
                recordToBeUpdated.AllowDeleteRecord = roleObjPriv.AllowDeleteRecord;
                recordToBeUpdated.AllowConfidentialDocumentType = roleObjPriv.AllowConfidentialDocumentType;
                recordToBeUpdated.AllowCheckNPIFStatus = roleObjPriv.AllowCheckNPIFStatus;

                this.DataContext.SaveChanges();
                roleObjPriv.Id = recordToBeUpdated.Id;
                #endregion

                successMsg = Languages.GetResourceText("RoleObjectPrivilegeSavedSuccess");
            }

            return SuccessOrFailedResponse<int?>(errMSg, roleObjPriv.Id, successMsg);
        }

        private void RoleChildObject(List<MES.DTO.Library.RoleManagement.RoleObjectPrivilege> childObject, int roleId)
        {
            foreach (DTO.Library.RoleManagement.RoleObjectPrivilege roleObjPriv in childObject)
            {
                roleObjPriv.RoleId = roleId;
                SaveRoleObjectPrivilege(roleObjPriv);
                RoleChildObject(roleObjPriv.childObject, roleId);
            }
        }

        public ITypedResponse<bool?> Delete(int Id)
        {
            var RFQToBeDeleted = this.DataContext.Roles.Where(a => a.Id == Id).SingleOrDefault();
            if (RFQToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("RFQNotExists"));
            }
            else
            {
                this.DataContext.Entry(RFQToBeDeleted).State = EntityState.Deleted;

                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("RFQDeletedSuccess"));
            }
        }

        public ITypedResponse<bool?> RoleNameExists(string roleName)
        {
            bool roleNameExists = false;

            roleNameExists = DataContext.Roles.Any(usr => usr.RoleName == roleName);

            if (roleNameExists)
                return SuccessBoolResponse(Languages.GetResourceText("roleNameExists"));
            else
                return FailedBoolResponse(Languages.GetResourceText("roleNameNotExists"));
        }
        #endregion

        public ITypedResponse<List<DTO.Library.RoleManagement.Role>> Search(IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }
    }
}
