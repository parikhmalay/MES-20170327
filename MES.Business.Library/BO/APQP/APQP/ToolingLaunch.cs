using Account.DTO.Library;
using MES.Business.Repositories.APQP.APQP;
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
using System.Data.Entity.Validation;

namespace MES.Business.Library.BO.APQP.APQP
{
    class ToolingLaunch : ContextBusinessBase, IToolingLaunchRepository
    {
        public ToolingLaunch()
            : base("ToolingLaunch")
        { }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.APQP.APQP.ToolingLaunch toolingLaunch)
        {
            string errMSg = null;
            string successMsg = null;
            if (toolingLaunch.Id > 0)
            {
                ObjectParameter ErrorKey = new ObjectParameter("ErrorKey", "");
                this.RunOnDB(context =>
                {
                    int result = context.UpdateToolingLaunch(toolingLaunch.Id, toolingLaunch.APQPItemId, toolingLaunch.MESToolingPONumber, toolingLaunch.RevLevel, toolingLaunch.RevisionDate,
                        toolingLaunch.ToolingKickoffDate, toolingLaunch.PlanToolingCompletionDate, toolingLaunch.APQPDrawingStatus, toolingLaunch.PPAPSubmissonPreparationDays, toolingLaunch.StatusId,
                        toolingLaunch.Comments, toolingLaunch.DrawingNumber, toolingLaunch.UpdatedBy, ErrorKey);

                    if (!string.IsNullOrEmpty(Convert.ToString(ErrorKey.Value)))
                    {
                        errMSg = Languages.GetResourceText("APQPSaveFail");
                    }
                });
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                successMsg = Languages.GetResourceText("APQPSavedSuccess");
            }
            return SuccessOrFailedResponse<int?>(errMSg, toolingLaunch.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.APQP.APQP.ToolingLaunch> FindById(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.APQP.APQP.ToolingLaunch toolingLaunch = new DTO.Library.APQP.APQP.ToolingLaunch();
            this.RunOnDB(context =>
            {
                var apqpData = context.GetAPQPToolingLaunchByItemId(id).SingleOrDefault();
                if (apqpData == null)
                    errMSg = Languages.GetResourceText("ToolingLaunchNotExists");
                else
                {
                    #region General details
                    toolingLaunch.Id = apqpData.Id;
                    toolingLaunch.APQPItemId = apqpData.APQPItemId;
                    toolingLaunch.MESToolingPONumber = apqpData.MESToolingPONumber;
                    toolingLaunch.RevLevel = apqpData.RevLevel;
                    toolingLaunch.RevisionDate = apqpData.RevisionDate;
                    toolingLaunch.ToolingKickoffDate = apqpData.ToolingKickoffDate;
                    toolingLaunch.PlanToolingCompletionDate = apqpData.PlanToolingCompletionDate;
                    toolingLaunch.APQPDrawingStatus = apqpData.APQPDrawingStatus;
                    toolingLaunch.PPAPSubmissonPreparationDays = apqpData.PPAPSubmissonPreparationDays;
                    toolingLaunch.Comments = apqpData.Comments;
                    //toolingLaunch.UpdatedFromSource = apqpData.UpdatedFromSource;
                    toolingLaunch.RFQNumber = apqpData.RFQNumber;
                    toolingLaunch.QuoteNumber = apqpData.QuoteNumber;
                    toolingLaunch.ProjectName = apqpData.ProjectName;
                    toolingLaunch.CustomerName = apqpData.CustomerName;
                    toolingLaunch.ManufacturerName = apqpData.ManufacturerName;
                    toolingLaunch.PartNumber = apqpData.PartNumber;
                    toolingLaunch.PartDesc = apqpData.PartDesc;
                    toolingLaunch.StatusId = apqpData.StatusId;
                    toolingLaunch.Status = apqpData.Status;
                    toolingLaunch.DrawingNumber = apqpData.DrawingNumber;
                    toolingLaunch.DaysOld = apqpData.DaysOld;
                    toolingLaunch.PNDaysOld = apqpData.PNDaysOld;
                    #endregion
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.APQP.APQP.ToolingLaunch>(errMSg, toolingLaunch);
            return response;
        }


        public NPE.Core.ITypedResponse<bool?> Delete(int toolingLaunchId)
        {
            //set the out put param
            ObjectParameter Result = new ObjectParameter("Result", 0);
            this.RunOnDB(context =>
            {
                //context.DeleteMultipleCustomer(CustomerIds, CurrentUser, Result);
            });
            if (Convert.ToInt32(Result.Value) > 0)
                return SuccessBoolResponse(Languages.GetResourceText("ToolingLaunchDeletedSuccess"));
            else
                return FailedBoolResponse(Languages.GetResourceText("ToolingLaunchDeleteFail"));
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.ToolingLaunch>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.ToolingLaunch>> GetToolingLaunchList(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.SearchCriteria> searchCriteria)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.APQP.APQP.ToolingLaunch> lstToolingLaunch = new List<DTO.Library.APQP.APQP.ToolingLaunch>();
            DTO.Library.APQP.APQP.ToolingLaunch toolingLaunch;
            this.RunOnDB(context =>
             {
                 var ToolingLaunchesList = context.GetAPQPToolingLaunches(searchCriteria.Criteria.RFQNumber, searchCriteria.Criteria.QuoteNumber, searchCriteria.Criteria.CustomerName, searchCriteria.Criteria.PartNo,
                     searchCriteria.Criteria.ProjectName, searchCriteria.Criteria.APQPStatusIds, searchCriteria.Criteria.SAMUserId, searchCriteria.Criteria.APQPQualityEngineerId,
                     searchCriteria.Criteria.SupplyChainCoordinatorId, searchCriteria.Criteria.AllowConfidentialDocumentType, searchCriteria.PageNo, searchCriteria.PageSize, totalRecords, "").ToList();
                 if (ToolingLaunchesList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     searchCriteria.TotalRecords = Convert.ToInt32(totalRecords.Value);

                     foreach (var apqpData in ToolingLaunchesList)
                     {
                         toolingLaunch = new DTO.Library.APQP.APQP.ToolingLaunch();
                         toolingLaunch.Id = apqpData.Id;
                         toolingLaunch.APQPItemId = apqpData.APQPItemId;
                         toolingLaunch.MESToolingPONumber = apqpData.MESToolingPONumber;
                         toolingLaunch.RevLevel = apqpData.RevLevel;
                         toolingLaunch.RevisionDate = apqpData.RevisionDate;
                         toolingLaunch.ToolingKickoffDate = apqpData.ToolingKickoffDate;
                         toolingLaunch.PlanToolingCompletionDate = apqpData.PlanToolingCompletionDate;
                         toolingLaunch.APQPDrawingStatus = apqpData.APQPDrawingStatus;
                         toolingLaunch.PPAPSubmissonPreparationDays = apqpData.PPAPSubmissonPreparationDays;
                         toolingLaunch.Comments = apqpData.Comments;
                         //toolingLaunch.UpdatedFromSource = apqpData.UpdatedFromSource;
                         toolingLaunch.RFQNumber = apqpData.RFQNumber;
                         toolingLaunch.QuoteNumber = apqpData.QuoteNumber;
                         toolingLaunch.ProjectName = apqpData.ProjectName;
                         toolingLaunch.CustomerName = apqpData.CustomerName;
                         toolingLaunch.ManufacturerName = apqpData.ManufacturerName;
                         toolingLaunch.PartNumber = apqpData.PartNumber;
                         toolingLaunch.PartDesc = apqpData.PartDesc;
                         toolingLaunch.StatusId = apqpData.StatusId;
                         toolingLaunch.Status = apqpData.Status;
                         toolingLaunch.DrawingNumber = apqpData.DrawingNumber;
                         toolingLaunch.DaysOld = apqpData.DaysOld;
                         toolingLaunch.PNDaysOld = apqpData.PNDaysOld;
                         toolingLaunch.IsDocument = apqpData.IsDocument;
                         lstToolingLaunch.Add(toolingLaunch);
                     }
                 }
             });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.APQP.ToolingLaunch>>(errMSg, lstToolingLaunch);
            response.PageInfo = searchCriteria.ToPage();
            return response;
        }

    }
}
