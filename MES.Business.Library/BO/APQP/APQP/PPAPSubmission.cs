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
    class PPAPSubmission : ContextBusinessBase, IPPAPSubmissionRepository
    {
        public PPAPSubmission()
            : base("PPAPSubmission")
        { }
        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.APQP.APQP.PPAPSubmission pPAPSubmission)
        {
            string errMSg = null;
            string successMsg = null;
            if (pPAPSubmission.Id > 0)
            {
                ObjectParameter ErrorKey = new ObjectParameter("ErrorKey", "");
                this.RunOnDB(context =>
                {
                    int result = context.UpdatePPAPSubmission(pPAPSubmission.Id, pPAPSubmission.APQPItemId, pPAPSubmission.PSWDate, pPAPSubmission.ActualPSWDate, pPAPSubmission.PPAPStatus,
                        pPAPSubmission.PartInfoEnteredIntoSAPDate, pPAPSubmission.PartInfoEnteredIntoPPEPDate, pPAPSubmission.PPAPPartsApprovedDate, pPAPSubmission.PackagingDataFilePath,
                        pPAPSubmission.PSWFilePath, pPAPSubmission.PSWFileCreatedDate, pPAPSubmission.Comments, pPAPSubmission.APQPStatusId, pPAPSubmission.UpdatedBy, ErrorKey);
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
            return SuccessOrFailedResponse<int?>(errMSg, pPAPSubmission.Id, successMsg);
        }
        public NPE.Core.ITypedResponse<int?> UpdatePSWDetails(string zipFileNavigationURL, int apqpItemId)
        {
            string errMSg = null; string successMsg = null;
            DTO.Library.APQP.APQP.PPAPSubmission pPAPSubmission = new DTO.Library.APQP.APQP.PPAPSubmission();
            if (apqpItemId > 0)
            {
                ObjectParameter ErrorKey = new ObjectParameter("ErrorKey", "");
                this.RunOnDB(context =>
                {
                    var recordToBeUpdated = context.PPAPSubmissions.Where(item => item.APQPItemId == apqpItemId).SingleOrDefault();
                    if (recordToBeUpdated == null)
                    { }
                    else
                    {
                        string fileName = string.Empty;
                        recordToBeUpdated.PSWFileCreatedDate = AuditUtils.GetCurrentDateTime();
                        recordToBeUpdated.PSWFilePath = zipFileNavigationURL;
                        recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                        recordToBeUpdated.UpdatedBy = CurrentUser;
                        this.DataContext.Entry(recordToBeUpdated).State = EntityState.Modified;
                        this.DataContext.SaveChanges();
                    }
                });
            }

            return SuccessOrFailedResponse<int?>(errMSg, pPAPSubmission.Id, successMsg);
        }
        public NPE.Core.ITypedResponse<DTO.Library.APQP.APQP.PPAPSubmission> FindById(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.APQP.APQP.PPAPSubmission pPAPSubmission = new DTO.Library.APQP.APQP.PPAPSubmission();
            this.RunOnDB(context =>
            {
                var apqpData = context.GetAPQPPPAPSubmissionByItemId(id).SingleOrDefault();
                if (apqpData == null)
                    errMSg = Languages.GetResourceText("PPAPSubmissionNotExists");
                else
                {
                    #region General details
                    pPAPSubmission.Id = apqpData.Id;
                    pPAPSubmission.APQPItemId = apqpData.APQPItemId;
                    pPAPSubmission.PSWDate = apqpData.PSWDate;
                    pPAPSubmission.ActualPSWDate = apqpData.ActualPSWDate;
                    pPAPSubmission.PPAPStatus = apqpData.PPAPStatus;
                    pPAPSubmission.MESWarehouse = apqpData.MESWarehouse;
                    pPAPSubmission.PartInfoEnteredIntoSAPDate = apqpData.PartInfoEnteredIntoSAPDate;
                    pPAPSubmission.PartInfoEnteredIntoPPEPDate = apqpData.PartInfoEnteredIntoPPEPDate;
                    pPAPSubmission.PPAPPartsApprovedDate = apqpData.PPAPPartsApprovedDate;
                    pPAPSubmission.PackagingDataFilePath = apqpData.PackagingDataFilePath;
                    pPAPSubmission.PSWFilePath = !string.IsNullOrEmpty(apqpData.PSWFilePath)
                                                    ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + apqpData.PSWFilePath
                                                    : apqpData.PSWFilePath;

                    pPAPSubmission.PSWFileCreatedDate = apqpData.PSWFileCreatedDate;
                    pPAPSubmission.Comments = apqpData.Comments;
                    pPAPSubmission.RFQNumber = apqpData.RFQNumber;
                    pPAPSubmission.QuoteNumber = apqpData.QuoteNumber;
                    pPAPSubmission.ProjectName = apqpData.ProjectName;
                    pPAPSubmission.CustomerName = apqpData.CustomerName;
                    pPAPSubmission.SupplierName = apqpData.SupplierName;
                    pPAPSubmission.PartNumber = apqpData.PartNumber;
                    pPAPSubmission.PartDesc = apqpData.PartDesc;
                    pPAPSubmission.APQPStatusId = apqpData.APQPStatusId;
                    pPAPSubmission.APQPStatus = apqpData.APQPStatus;
                    pPAPSubmission.DrawingNumber = apqpData.DrawingNumber;
                    pPAPSubmission.RevLevel = apqpData.RevLevel;
                    pPAPSubmission.ActualSampleShipmentDate = apqpData.ActualSampleShipmentDate;
                    pPAPSubmission.DaysOld = apqpData.DaysOld;
                    pPAPSubmission.PNDaysOld = apqpData.PNDaysOld;
                    #endregion
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.APQP.APQP.PPAPSubmission>(errMSg, pPAPSubmission);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int pPAPSubmissionId)
        {
            //set the out put param
            ObjectParameter Result = new ObjectParameter("Result", 0);
            this.RunOnDB(context =>
            {
                //context.DeleteMultipleCustomer(CustomerIds, CurrentUser, Result);
            });
            if (Convert.ToInt32(Result.Value) > 0)
                return SuccessBoolResponse(Languages.GetResourceText("PPAPSubmissionDeletedSuccess"));
            else
                return FailedBoolResponse(Languages.GetResourceText("PPAPSubmissionDeleteFail"));
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.PPAPSubmission>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.PPAPSubmission>> GetPPAPSubmissionList(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.SearchCriteria> searchCriteria)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.APQP.APQP.PPAPSubmission> lstPPAPSubmission = new List<DTO.Library.APQP.APQP.PPAPSubmission>();
            DTO.Library.APQP.APQP.PPAPSubmission pPAPSubmission;
            this.RunOnDB(context =>
             {
                 var PPAPSubmissionsList = context.GetAPQPPPAPSubmissions(searchCriteria.Criteria.RFQNumber, searchCriteria.Criteria.QuoteNumber, searchCriteria.Criteria.CustomerName, searchCriteria.Criteria.PartNo,
                     searchCriteria.Criteria.ProjectName, searchCriteria.Criteria.APQPStatusIds, searchCriteria.Criteria.SAMUserId, searchCriteria.Criteria.APQPQualityEngineerId,
                     searchCriteria.Criteria.SupplyChainCoordinatorId, searchCriteria.Criteria.AllowConfidentialDocumentType, searchCriteria.PageNo, searchCriteria.PageSize, totalRecords, "").ToList();
                 if (PPAPSubmissionsList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     searchCriteria.TotalRecords = Convert.ToInt32(totalRecords.Value);

                     foreach (var apqpData in PPAPSubmissionsList)
                     {
                         pPAPSubmission = new DTO.Library.APQP.APQP.PPAPSubmission();
                         pPAPSubmission.Id = apqpData.Id;
                         pPAPSubmission.APQPItemId = apqpData.APQPItemId;
                         pPAPSubmission.PSWDate = apqpData.PSWDate;
                         pPAPSubmission.ActualPSWDate = apqpData.ActualPSWDate;
                         pPAPSubmission.PPAPStatus = apqpData.PPAPStatus;
                         pPAPSubmission.MESWarehouse = apqpData.MESWarehouse;
                         pPAPSubmission.PartInfoEnteredIntoSAPDate = apqpData.PartInfoEnteredIntoSAPDate;
                         pPAPSubmission.PartInfoEnteredIntoPPEPDate = apqpData.PartInfoEnteredIntoPPEPDate;
                         pPAPSubmission.PPAPPartsApprovedDate = apqpData.PPAPPartsApprovedDate;
                         pPAPSubmission.PackagingDataFilePath = apqpData.PackagingDataFilePath;
                         pPAPSubmission.PSWFilePath = !string.IsNullOrEmpty(apqpData.PSWFilePath)
                                                   ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + apqpData.PSWFilePath
                                                   : apqpData.PSWFilePath;
                         pPAPSubmission.PSWFileCreatedDate = apqpData.PSWFileCreatedDate;
                         pPAPSubmission.Comments = apqpData.Comments;
                         pPAPSubmission.RFQNumber = apqpData.RFQNumber;
                         pPAPSubmission.QuoteNumber = apqpData.QuoteNumber;
                         pPAPSubmission.ProjectName = apqpData.ProjectName;
                         pPAPSubmission.CustomerName = apqpData.CustomerName;
                         pPAPSubmission.SupplierName = apqpData.SupplierName;
                         pPAPSubmission.PartNumber = apqpData.PartNumber;
                         pPAPSubmission.PartDesc = apqpData.PartDesc;
                         pPAPSubmission.APQPStatusId = apqpData.APQPStatusId;
                         pPAPSubmission.APQPStatus = apqpData.APQPStatus;
                         pPAPSubmission.DrawingNumber = apqpData.DrawingNumber;
                         pPAPSubmission.RevLevel = apqpData.RevLevel;
                         pPAPSubmission.ActualSampleShipmentDate = apqpData.ActualSampleShipmentDate;
                         pPAPSubmission.DaysOld = apqpData.DaysOld;
                         pPAPSubmission.PNDaysOld = apqpData.PNDaysOld;
                         pPAPSubmission.IsDocument = apqpData.IsDocument;
                         lstPPAPSubmission.Add(pPAPSubmission);
                     }
                 }
             });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.APQP.PPAPSubmission>>(errMSg, lstPPAPSubmission);
            response.PageInfo = searchCriteria.ToPage();
            return response;
        }
    }
}
