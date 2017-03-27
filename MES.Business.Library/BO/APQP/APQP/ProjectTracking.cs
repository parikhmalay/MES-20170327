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
    class ProjectTracking : ContextBusinessBase, IProjectTrackingRepository
    {
        public ProjectTracking()
            : base("ProjectTracking")
        { }
        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.APQP.APQP.ProjectTracking projectTracking)
        {
            string errMSg = null;
            string successMsg = null;
            if (projectTracking.Id > 0)
            {
                ObjectParameter ErrorKey = new ObjectParameter("ErrorKey", "");
                this.RunOnDB(context =>
                {
                    int result = context.UpdateProjectTracking(projectTracking.Id, projectTracking.APQPItemId, projectTracking.APQPProjectCategoryId, projectTracking.APQPProjectStageId,
                        projectTracking.CurrentEstimatedToolingCompletionDate, projectTracking.ShipmentTrackingNumber, projectTracking.QualityFeedbackInformation, projectTracking.EstimatedSampleShipmentDate,
                        projectTracking.Remarks, projectTracking.StatusId, projectTracking.ActualToolingCompletionDate, projectTracking.ActualSampleShipmentDate, projectTracking.ToolChangeDetails, projectTracking.UpdatedBy, ErrorKey);

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
            return SuccessOrFailedResponse<int?>(errMSg, projectTracking.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.APQP.APQP.ProjectTracking> FindById(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.APQP.APQP.ProjectTracking projectTracking = new DTO.Library.APQP.APQP.ProjectTracking();
            this.RunOnDB(context =>
            {
                var apqpdata = context.GetAPQPProjectTrackingByItemId(id).SingleOrDefault();
                if (apqpdata == null)
                    errMSg = Languages.GetResourceText("ProjectTrackingNotExists");
                else
                {
                    #region General details
                    projectTracking.Id = apqpdata.Id;
                    projectTracking.APQPItemId = apqpdata.APQPItemId;
                    projectTracking.APQPProjectCategoryId = apqpdata.APQPProjectCategoryId;
                    projectTracking.ProjectCategory = apqpdata.ProjectCategory;
                    projectTracking.APQPProjectStageId = apqpdata.APQPProjectStageId;
                    projectTracking.ProjectStage = apqpdata.ProjectStage;
                    projectTracking.CurrentEstimatedToolingCompletionDate = apqpdata.CurrentEstimatedToolingCompletionDate;
                    projectTracking.ShipmentTrackingNumber = apqpdata.ShipmentTrackingNumber;
                    projectTracking.QualityFeedbackInformation = apqpdata.QualityFeedbackInformation;
                    projectTracking.EstimatedSampleShipmentDate = apqpdata.EstimatedSampleShipmentDate;
                    projectTracking.Remarks = apqpdata.Remarks;
                    projectTracking.ActualToolingCompletionDate = apqpdata.ActualToolingCompletionDate;
                    projectTracking.ActualSampleShipmentDate = apqpdata.ActualSampleShipmentDate;
                    projectTracking.ToolChangeDetails = apqpdata.ToolChangeDetails;
                    //projectTracking.UpdatedFromSource = apqpdata.UpdatedFromSource;
                    projectTracking.RFQNumber = apqpdata.RFQNumber;
                    projectTracking.QuoteNumber = apqpdata.QuoteNumber;
                    projectTracking.ProjectName = apqpdata.ProjectName;
                    projectTracking.CustomerName = apqpdata.CustomerName;
                    projectTracking.ManufacturerName = apqpdata.ManufacturerName;
                    projectTracking.PartNumber = apqpdata.PartNumber;
                    projectTracking.PartDesc = apqpdata.PartDesc;
                    projectTracking.StatusId = apqpdata.StatusId;
                    projectTracking.Status = apqpdata.Status;
                    projectTracking.ToolingLeadtimeDays = apqpdata.ToolingLeadtimeDays;
                    projectTracking.ToolingKickoffDate = apqpdata.ToolingKickoffDate;
                    projectTracking.DrawingNumber = apqpdata.DrawingNumber;
                    projectTracking.RevLevel = apqpdata.RevLevel;
                    projectTracking.PlanToolingCompletionDate = apqpdata.PlanToolingCompletionDate;
                    projectTracking.DaysOld = apqpdata.DaysOld;
                    projectTracking.PNDaysOld = apqpdata.PNDaysOld;
                    #endregion
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.APQP.APQP.ProjectTracking>(errMSg, projectTracking);
            return response;
        }


        public NPE.Core.ITypedResponse<bool?> Delete(int projectTrackingId)
        {
            //set the out put param
            ObjectParameter Result = new ObjectParameter("Result", 0);
            this.RunOnDB(context =>
            {
                //context.DeleteMultipleCustomer(CustomerIds, CurrentUser, Result);
            });
            if (Convert.ToInt32(Result.Value) > 0)
                return SuccessBoolResponse(Languages.GetResourceText("ProjectTrackingDeletedSuccess"));
            else
                return FailedBoolResponse(Languages.GetResourceText("ProjectTrackingDeleteFail"));
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.ProjectTracking>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.ProjectTracking>> GetProjectTrackingList(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.SearchCriteria> searchCriteria)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.APQP.APQP.ProjectTracking> lstProjectTracking = new List<DTO.Library.APQP.APQP.ProjectTracking>();
            DTO.Library.APQP.APQP.ProjectTracking projectTracking;
            this.RunOnDB(context =>
             {
                 var ProjectTrackingsList = context.GetAPQPProjectTrackings(searchCriteria.Criteria.RFQNumber, searchCriteria.Criteria.QuoteNumber, searchCriteria.Criteria.CustomerName, searchCriteria.Criteria.PartNo,
                     searchCriteria.Criteria.ProjectName, searchCriteria.Criteria.APQPStatusIds, searchCriteria.Criteria.SAMUserId, searchCriteria.Criteria.APQPQualityEngineerId,
                     searchCriteria.Criteria.SupplyChainCoordinatorId, searchCriteria.Criteria.AllowConfidentialDocumentType, searchCriteria.PageNo, searchCriteria.PageSize, totalRecords, "").ToList();
                 if (ProjectTrackingsList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     searchCriteria.TotalRecords = Convert.ToInt32(totalRecords.Value);

                     foreach (var apqpdata in ProjectTrackingsList)
                     {
                         projectTracking = new DTO.Library.APQP.APQP.ProjectTracking();
                         projectTracking.Id = apqpdata.Id;
                         projectTracking.APQPItemId = apqpdata.APQPItemId;
                         projectTracking.APQPProjectCategoryId = apqpdata.APQPProjectCategoryId;
                         projectTracking.ProjectCategory = apqpdata.ProjectCategory;
                         projectTracking.APQPProjectStageId = apqpdata.APQPProjectStageId;
                         projectTracking.ProjectStage = apqpdata.ProjectStage;
                         projectTracking.CurrentEstimatedToolingCompletionDate = apqpdata.CurrentEstimatedToolingCompletionDate;
                         projectTracking.ShipmentTrackingNumber = apqpdata.ShipmentTrackingNumber;
                         projectTracking.QualityFeedbackInformation = apqpdata.QualityFeedbackInformation;
                         projectTracking.EstimatedSampleShipmentDate = apqpdata.EstimatedSampleShipmentDate;
                         projectTracking.Remarks = apqpdata.Remarks;
                         projectTracking.ActualToolingCompletionDate = apqpdata.ActualToolingCompletionDate;
                         projectTracking.ActualSampleShipmentDate = apqpdata.ActualSampleShipmentDate;
                         projectTracking.ToolChangeDetails = apqpdata.ToolChangeDetails;
                         //projectTracking.UpdatedFromSource = apqpdata.UpdatedFromSource;
                         projectTracking.RFQNumber = apqpdata.RFQNumber;
                         projectTracking.QuoteNumber = apqpdata.QuoteNumber;
                         projectTracking.ProjectName = apqpdata.ProjectName;
                         projectTracking.CustomerName = apqpdata.CustomerName;
                         projectTracking.ManufacturerName = apqpdata.ManufacturerName;
                         projectTracking.PartNumber = apqpdata.PartNumber;
                         projectTracking.PartDesc = apqpdata.PartDesc;
                         projectTracking.StatusId = apqpdata.StatusId;
                         projectTracking.Status = apqpdata.Status;
                         projectTracking.ToolingLeadtimeDays = apqpdata.ToolingLeadtimeDays;
                         projectTracking.ToolingKickoffDate = apqpdata.ToolingKickoffDate;
                         projectTracking.DrawingNumber = apqpdata.DrawingNumber;
                         projectTracking.RevLevel = apqpdata.RevLevel;
                         projectTracking.PlanToolingCompletionDate = apqpdata.PlanToolingCompletionDate;
                         projectTracking.DaysOld = apqpdata.DaysOld;
                         projectTracking.PNDaysOld = apqpdata.PNDaysOld;
                         projectTracking.IsDocument = apqpdata.IsDocument;
                         lstProjectTracking.Add(projectTracking);
                     }
                 }
             });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.APQP.ProjectTracking>>(errMSg, lstProjectTracking);
            response.PageInfo = searchCriteria.ToPage();
            return response;
        }
    }
}
