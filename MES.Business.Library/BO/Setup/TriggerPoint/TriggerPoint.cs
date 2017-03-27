using Account.DTO.Library;
using MES.Business.Repositories.Setup.TriggerPoint;
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

namespace MES.Business.Library.BO.Setup.TriggerPoint
{
    class TriggerPoint : ContextBusinessBase, ITriggerPointRepository
    {
        public TriggerPoint()
            : base("TriggerPoint")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.Setup.TriggerPoint.TriggerPoint triggerPoint)
        {
            string errMSg = null;
            string successMsg = null;
            try
            {
                //check for the uniqueness
                if (this.DataContext.TriggerPoints.AsNoTracking().Any(a => a.TriggerPoint1 == triggerPoint.triggerPoint && a.IsDeleted == false && a.Id != triggerPoint.Id))
                {
                    errMSg = Languages.GetResourceText("TriggerPointExists");
                }
                else
                {
                    var recordToBeUpdated = new MES.Data.Library.TriggerPoint();
                    if (triggerPoint.Id > 0)
                    {
                        recordToBeUpdated = this.DataContext.TriggerPoints.Where(a => a.Id == triggerPoint.Id).SingleOrDefault();
                        if (recordToBeUpdated == null)
                            errMSg = Languages.GetResourceText("TriggerPointNotExists");
                        else
                        {
                            #region "Delete trigger point users Details"
                            var deleteTriggerPointUsersList = this.DataContext.TriggerPointUsers.Where(a => a.TriggerPointId == triggerPoint.Id).ToList();
                            foreach (var item in deleteTriggerPointUsersList)
                            {
                                this.DataContext.TriggerPointUsers.Remove(item);
                            }
                            #endregion

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
                        this.DataContext.TriggerPoints.Add(recordToBeUpdated);
                    }
                    if (string.IsNullOrEmpty(errMSg))
                    {
                        recordToBeUpdated.TriggerPoint1 = triggerPoint.triggerPoint;
                        this.DataContext.SaveChanges();
                        triggerPoint.Id = recordToBeUpdated.Id;

                        #region "Save trigger point users Details"
                        MES.Data.Library.TriggerPointUser dboTriggerPointUser = null;
                        if (triggerPoint.TriggerPointUsersList != null && triggerPoint.TriggerPointUsersList.Count > 0)
                        {
                            bool AnyTriggerPointUser = false;
                            foreach (var triggerPointUser in triggerPoint.TriggerPointUsersList)
                            {
                                if (triggerPoint.Id > 0)
                                {
                                    AnyTriggerPointUser = true;
                                    dboTriggerPointUser = new MES.Data.Library.TriggerPointUser();
                                    dboTriggerPointUser.TriggerPointId = triggerPoint.Id;
                                    dboTriggerPointUser.UserId = triggerPointUser.Id;
                                    dboTriggerPointUser.CreatedBy = CurrentUser;
                                    dboTriggerPointUser.CreatedDate = AuditUtils.GetCurrentDateTime();
                                    this.DataContext.TriggerPointUsers.Add(dboTriggerPointUser);
                                }
                            }



                            if (AnyTriggerPointUser)
                                this.DataContext.SaveChanges();
                        }
                        #endregion

                        successMsg = Languages.GetResourceText("TriggerPointSavedSuccess");
                    }
                }
                return SuccessOrFailedResponse<int?>(errMSg, triggerPoint.Id, successMsg);
            }
            catch (Exception ex)
            {
                errMSg = ex.Message.ToString();
                return SuccessOrFailedResponse<int?>(errMSg, triggerPoint.Id, successMsg);
            }
        }

        public NPE.Core.ITypedResponse<DTO.Library.Setup.TriggerPoint.TriggerPoint> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int triggerPointId)
        {
            var triggerPointToBeDeleted = this.DataContext.TriggerPoints.Where(a => a.Id == triggerPointId).SingleOrDefault();
            if (triggerPointToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("TriggerPointNotExists"));
            }
            else
            {
                triggerPointToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                triggerPointToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(triggerPointToBeDeleted).State = EntityState.Modified;
                triggerPointToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("TriggerPointDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.TriggerPoint.TriggerPoint>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.TriggerPoint.TriggerPoint>> GetTriggerPointsList(NPE.Core.IPage<DTO.Library.Setup.TriggerPoint.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.Setup.TriggerPoint.TriggerPoint> lsttriggerPoint = new List<DTO.Library.Setup.TriggerPoint.TriggerPoint>();
            DTO.Library.Setup.TriggerPoint.TriggerPoint triggerPoint;
            this.RunOnDB(context =>
             {
                 var triggerPointList = context.SearchTriggerPoint(paging.Criteria.triggerPoint, paging.Criteria.UserId, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (triggerPointList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in triggerPointList)
                     {
                         triggerPoint = new DTO.Library.Setup.TriggerPoint.TriggerPoint();
                         triggerPoint.Id = item.Id;
                         triggerPoint.triggerPoint = item.TriggerPoint;
                         #region Bind User details
                         triggerPoint.TriggerPointUsersList = new List<DTO.Library.Setup.TriggerPoint.TriggerPointUsers>();
                         context.GetUsersByTriggerPointId(item.Id).ToList().ForEach(tl => triggerPoint.TriggerPointUsersList.Add(
                             new DTO.Library.Setup.TriggerPoint.TriggerPointUsers()
                             {
                                 Id = tl.UserId,
                                 Name = tl.User
                             }));
                         #endregion
                         lsttriggerPoint.Add(triggerPoint);
                     }
                 }
             });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.TriggerPoint.TriggerPoint>>(errMSg, lsttriggerPoint);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }
    }
}
