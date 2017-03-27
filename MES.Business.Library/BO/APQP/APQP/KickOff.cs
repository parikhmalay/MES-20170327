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
using MES.Business.Library.Enums;

namespace MES.Business.Library.BO.APQP.APQP
{
    class KickOff : ContextBusinessBase, IKickOffRepository
    {
        public KickOff()
            : base("KickOff")
        { }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.APQP.APQP.KickOff kickOff)
        {
            string errMSg = null;
            string successMsg = null;
            if (kickOff.Id > 0)
            {
                ObjectParameter ErrorKey = new ObjectParameter("ErrorKey", "");
                this.RunOnDB(context =>
                {
                    int result = context.UpdateAPQPItem(kickOff.Id, kickOff.CustomerToolingPOAuthRcvdDate, kickOff.ProjectKickoffDate, kickOff.RevLevel, kickOff.RFQNumber, kickOff.QuoteNumber,
                        kickOff.ProjectName, kickOff.CustomerId, kickOff.CustomerCode, kickOff.CustomerName, kickOff.CustomerCity, kickOff.CustomerState, kickOff.CustomerManufacturingLocation,
                        kickOff.CustomerProjectLeadId, kickOff.CustomerProjectLead, kickOff.CustomerProjectLeadEmail, kickOff.CustomerProjectLeadPhone, kickOff.CustomerEngineerId,
                        kickOff.CustomerEngineer, kickOff.CustomerEngineerEmail, kickOff.CustomerEngineerPhone, kickOff.ShipToLocation, kickOff.PPAPSubmissionLevel, kickOff.NumberOfSampleRequired,
                        kickOff.SupplierId, kickOff.SupplierCode, kickOff.SupplierName, kickOff.SupplierAddress1, kickOff.SupplierAddress2, kickOff.SupplierCity, kickOff.SupplierState, kickOff.SupplierCountry,
                        kickOff.SupplierZip, kickOff.ManufacturerId, kickOff.ManufacturerCode, kickOff.ManufacturerName, kickOff.ManufacturerAddress1, kickOff.ManufacturerAddress2, kickOff.ManufacturerCity,
                        kickOff.ManufacturerState, kickOff.ManufacturerCountry, kickOff.ManufacturerZip, kickOff.EAUUsage, kickOff.PartNumber, kickOff.PartDesc, kickOff.MaterialType, kickOff.PartWeight, kickOff.ProjectNotes,
                        kickOff.CustomerToolingPONumber, kickOff.PurchasePieceCost, kickOff.PurchaseToolingCost, kickOff.SellingPiecePrice, kickOff.SellingToolingPrice, kickOff.ToolingLeadtimeDays,
                        kickOff.APQPEngineerId, kickOff.APQPEngineer, kickOff.SupplyChainCoordinatorId, kickOff.SupplyChainCoordinator, kickOff.PartClassification, kickOff.CommodityTypeId, kickOff.CommodityType,
                        kickOff.StatusId, kickOff.SAMUserId, kickOff.DestinationId, kickOff.DrawingNumber, CurrentUser, ErrorKey);
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
            return SuccessOrFailedResponse<int?>(errMSg, kickOff.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.APQP.APQP.KickOff> FindById(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.APQP.APQP.KickOff kickOff = new DTO.Library.APQP.APQP.KickOff();
            this.RunOnDB(context =>
            {
                var apqpKickOffData = context.GetAPQPItemByItemId(id).SingleOrDefault();
                if (apqpKickOffData == null)
                    errMSg = Languages.GetResourceText("KickOffNotExists");
                else
                {
                    #region General details
                    kickOff.Id = apqpKickOffData.Id;
                    kickOff.CustomerToolingPOAuthRcvdDate = apqpKickOffData.CustomerToolingPOAuthRcvdDate;
                    kickOff.ProjectKickoffDate = apqpKickOffData.ProjectKickoffDate;
                    kickOff.RFQNumber = apqpKickOffData.RFQNumber;
                    kickOff.QuoteNumber = apqpKickOffData.QuoteNumber;
                    kickOff.ProjectName = apqpKickOffData.ProjectName;
                    kickOff.CustomerId = apqpKickOffData.CustomerId;
                    kickOff.CustomerCode = apqpKickOffData.CustomerCode;
                    kickOff.CustomerName = apqpKickOffData.CustomerName;
                    kickOff.CustomerCity = apqpKickOffData.CustomerCity;
                    kickOff.CustomerState = apqpKickOffData.CustomerState;
                    kickOff.CustomerManufacturingLocation = apqpKickOffData.CustomerManufacturingLocation;
                    kickOff.CustomerProjectLeadId = apqpKickOffData.CustomerProjectLeadId;
                    kickOff.CustomerProjectLead = apqpKickOffData.CustomerProjectLead;
                    kickOff.CustomerProjectLeadEmail = apqpKickOffData.CustomerProjectLeadEmail;
                    kickOff.CustomerProjectLeadPhone = apqpKickOffData.CustomerProjectLeadPhone;
                    kickOff.CustomerEngineerId = apqpKickOffData.CustomerEngineerId;
                    kickOff.CustomerEngineer = apqpKickOffData.CustomerEngineer;
                    kickOff.CustomerEngineerEmail = apqpKickOffData.CustomerEngineerEmail;
                    kickOff.CustomerEngineerPhone = apqpKickOffData.CustomerEngineerPhone;
                    kickOff.ShipToLocation = apqpKickOffData.ShipToLocation;
                    kickOff.PPAPSubmissionLevel = apqpKickOffData.PPAPSubmissionLevel;
                    kickOff.NumberOfSampleRequired = apqpKickOffData.NumberOfSampleRequired;
                    kickOff.SupplierId = apqpKickOffData.SupplierId;
                    kickOff.SupplierCode = apqpKickOffData.SupplierCode;
                    kickOff.SupplierName = apqpKickOffData.SupplierName;
                    kickOff.SupplierAddress1 = apqpKickOffData.SupplierAddress1;
                    kickOff.SupplierAddress2 = apqpKickOffData.SupplierAddress2;
                    kickOff.SupplierCity = apqpKickOffData.SupplierCity;
                    kickOff.SupplierState = apqpKickOffData.SupplierState;
                    kickOff.SupplierCountry = apqpKickOffData.SupplierCountry;
                    kickOff.SupplierZip = apqpKickOffData.SupplierZip;
                    kickOff.ManufacturerId = apqpKickOffData.ManufacturerId;
                    kickOff.ManufacturerCode = apqpKickOffData.ManufacturerCode;
                    kickOff.ManufacturerName = apqpKickOffData.ManufacturerName;
                    kickOff.ManufacturerAddress1 = apqpKickOffData.ManufacturerAddress1;
                    kickOff.ManufacturerAddress2 = apqpKickOffData.ManufacturerAddress2;
                    kickOff.ManufacturerCity = apqpKickOffData.ManufacturerCity;
                    kickOff.ManufacturerState = apqpKickOffData.ManufacturerState;
                    kickOff.ManufacturerCountry = apqpKickOffData.ManufacturerCountry;
                    kickOff.ManufacturerZip = apqpKickOffData.ManufacturerZip;
                    kickOff.EAUUsage = apqpKickOffData.EAUUsage;
                    kickOff.PartNumber = apqpKickOffData.PartNumber;
                    kickOff.PartDesc = apqpKickOffData.PartDesc;
                    kickOff.MaterialType = apqpKickOffData.MaterialType;
                    kickOff.PartWeight = apqpKickOffData.PartWeight;
                    kickOff.ProjectNotes = apqpKickOffData.ProjectNotes;
                    kickOff.CustomerToolingPONumber = apqpKickOffData.CustomerToolingPONumber;
                    kickOff.PurchasePieceCost = apqpKickOffData.PurchasePieceCost;
                    kickOff.PurchaseToolingCost = apqpKickOffData.PurchaseToolingCost;
                    kickOff.SellingPiecePrice = apqpKickOffData.SellingPiecePrice;
                    kickOff.SellingToolingPrice = apqpKickOffData.SellingToolingPrice;
                    kickOff.ToolingLeadtimeDays = apqpKickOffData.ToolingLeadtimeDays;
                    kickOff.APQPEngineerId = apqpKickOffData.APQPEngineerId;
                    kickOff.APQPEngineer = apqpKickOffData.APQPEngineer;
                    kickOff.SupplyChainCoordinatorId = apqpKickOffData.SupplyChainCoordinatorId;
                    kickOff.SupplyChainCoordinator = apqpKickOffData.SupplyChainCoordinator;
                    kickOff.PartClassification = apqpKickOffData.PartClassification;
                    kickOff.CommodityTypeId = apqpKickOffData.WorkTypeId;
                    kickOff.CommodityType = apqpKickOffData.WorkType;
                    kickOff.CommodityId = apqpKickOffData.CommodityId;
                    kickOff.Commodity = apqpKickOffData.Commodity;
                    kickOff.Process = apqpKickOffData.Process;
                    kickOff.AdditionalPartDescription = apqpKickOffData.AdditionalPartDescription;
                    kickOff.StatusId = apqpKickOffData.StatusId;
                    kickOff.Status = apqpKickOffData.Status;
                    kickOff.IsSourceFromSAP = apqpKickOffData.IsSourceFromSAP;
                    kickOff.SAMUserId = apqpKickOffData.SAMUserId;
                    kickOff.SAMUser = apqpKickOffData.SAMUser;
                    //kickOff.QuoteDetailId = apqpKickOffData.QuoteDetailId;
                    //kickOff.UpdatedFromSource = apqpKickOffData.UpdatedFromSource;
                    kickOff.DrawingNumber = apqpKickOffData.DrawingNumber;
                    kickOff.DestinationId = apqpKickOffData.DestinationId;
                    //kickOff.APQPSAPItemId = apqpKickOffData.APQPSAPItemId;
                    // kickOff.KickOffDocument = apqpKickOffData.KickOffDocument;
                    kickOff.RevisionDate = apqpKickOffData.KickOffRevDate;
                    kickOff.RevLevel = apqpKickOffData.KickOffRevLevel;
                    kickOff.DaysOld = apqpKickOffData.DaysOld;
                    kickOff.PNDaysOld = apqpKickOffData.PNDaysOld;
                    kickOff.DocuSignStatus = GetNPIFDocuSignStatus(id);

                    if (string.IsNullOrEmpty(kickOff.DocuSignStatus) || kickOff.DocuSignStatus == DocuSignEnvelopeStatus.completed.ToString() || kickOff.DocuSignStatus == DocuSignEnvelopeStatus.declined.ToString())
                        kickOff.showDocuSignApprovalBtn = true;
                    else
                        kickOff.showDocuSignApprovalBtn = false;
                    #endregion

                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.APQP.APQP.KickOff>(errMSg, kickOff);
            return response;
        }


        public string GetNPIFDocuSignStatus(int Id)
        {
            string status = null;
            MES.DTO.Library.APQP.APQP.apqpNPIFDocuSign npifDocuSign = new DTO.Library.APQP.APQP.apqpNPIFDocuSign();
            try
            {
                this.RunOnDB(context =>
                {
                    var docuSign = context.NPIFDocusigns.Where(x => x.APQPItemId == Id).OrderByDescending(item => item.CreatedDate).FirstOrDefault();
                    if (docuSign != null)
                    {
                        status = docuSign.Status;
                    }
                });


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;
        }
        public NPE.Core.ITypedResponse<bool?> Delete(int kickOffId)
        {
            //set the out put param
            ObjectParameter Result = new ObjectParameter("Result", 0);
            this.RunOnDB(context =>
            {
                //context.DeleteMultipleCustomer(CustomerIds, CurrentUser, Result);
            });
            if (Convert.ToInt32(Result.Value) > 0)
                return SuccessBoolResponse(Languages.GetResourceText("KickOffDeletedSuccess"));
            else
                return FailedBoolResponse(Languages.GetResourceText("KickOffDeleteFail"));
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.KickOff>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.KickOff>> GetKickOffList(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.SearchCriteria> searchCriteria)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.APQP.APQP.KickOff> lstKickOff = new List<DTO.Library.APQP.APQP.KickOff>();
            DTO.Library.APQP.APQP.KickOff kickOff;
            this.RunOnDB(context =>
             {
                 var KickoffList = context.GetAPQPItems(searchCriteria.Criteria.RFQNumber, searchCriteria.Criteria.QuoteNumber, searchCriteria.Criteria.CustomerName, searchCriteria.Criteria.PartNo,
                     searchCriteria.Criteria.ProjectName, searchCriteria.Criteria.APQPStatusIds, searchCriteria.Criteria.SAMUserId, searchCriteria.Criteria.APQPQualityEngineerId,
                     searchCriteria.Criteria.SupplyChainCoordinatorId, searchCriteria.Criteria.AllowConfidentialDocumentType, searchCriteria.PageNo, searchCriteria.PageSize, totalRecords, "").ToList();
                 if (KickoffList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     searchCriteria.TotalRecords = Convert.ToInt32(totalRecords.Value);

                     foreach (var apqpKickOffData in KickoffList)
                     {
                         #region assign data
                         kickOff = new DTO.Library.APQP.APQP.KickOff();
                         kickOff.Id = apqpKickOffData.Id;
                         kickOff.CustomerToolingPOAuthRcvdDate = apqpKickOffData.CustomerToolingPOAuthRcvdDate;
                         kickOff.ProjectKickoffDate = apqpKickOffData.ProjectKickoffDate;
                         kickOff.RFQNumber = apqpKickOffData.RFQNumber;
                         kickOff.QuoteNumber = apqpKickOffData.QuoteNumber;
                         kickOff.ProjectName = apqpKickOffData.ProjectName;
                         kickOff.CustomerId = apqpKickOffData.CustomerId;
                         kickOff.CustomerCode = apqpKickOffData.CustomerCode;
                         kickOff.CustomerName = apqpKickOffData.CustomerName;
                         kickOff.CustomerCity = apqpKickOffData.CustomerCity;
                         kickOff.CustomerState = apqpKickOffData.CustomerState;
                         kickOff.CustomerManufacturingLocation = apqpKickOffData.CustomerManufacturingLocation;
                         kickOff.CustomerProjectLeadId = apqpKickOffData.CustomerProjectLeadId;
                         kickOff.CustomerProjectLead = apqpKickOffData.CustomerProjectLead;
                         kickOff.CustomerProjectLeadEmail = apqpKickOffData.CustomerProjectLeadEmail;
                         kickOff.CustomerProjectLeadPhone = apqpKickOffData.CustomerProjectLeadPhone;
                         kickOff.CustomerEngineerId = apqpKickOffData.CustomerEngineerId;
                         kickOff.CustomerEngineer = apqpKickOffData.CustomerEngineer;
                         kickOff.CustomerEngineerEmail = apqpKickOffData.CustomerEngineerEmail;
                         kickOff.CustomerEngineerPhone = apqpKickOffData.CustomerEngineerPhone;
                         kickOff.ShipToLocation = apqpKickOffData.ShipToLocation;
                         kickOff.PPAPSubmissionLevel = apqpKickOffData.PPAPSubmissionLevel;
                         kickOff.NumberOfSampleRequired = apqpKickOffData.NumberOfSampleRequired;
                         kickOff.SupplierId = apqpKickOffData.SupplierId;
                         kickOff.SupplierCode = apqpKickOffData.SupplierCode;
                         kickOff.SupplierName = apqpKickOffData.SupplierName;
                         kickOff.SupplierAddress1 = apqpKickOffData.SupplierAddress1;
                         kickOff.SupplierAddress2 = apqpKickOffData.SupplierAddress2;
                         kickOff.SupplierCity = apqpKickOffData.SupplierCity;
                         kickOff.SupplierState = apqpKickOffData.SupplierState;
                         kickOff.SupplierCountry = apqpKickOffData.SupplierCountry;
                         kickOff.SupplierZip = apqpKickOffData.SupplierZip;
                         kickOff.ManufacturerId = apqpKickOffData.ManufacturerId;
                         kickOff.ManufacturerCode = apqpKickOffData.ManufacturerCode;
                         kickOff.ManufacturerName = apqpKickOffData.ManufacturerName;
                         kickOff.ManufacturerAddress1 = apqpKickOffData.ManufacturerAddress1;
                         kickOff.ManufacturerAddress2 = apqpKickOffData.ManufacturerAddress2;
                         kickOff.ManufacturerCity = apqpKickOffData.ManufacturerCity;
                         kickOff.ManufacturerState = apqpKickOffData.ManufacturerState;
                         kickOff.ManufacturerCountry = apqpKickOffData.ManufacturerCountry;
                         kickOff.ManufacturerZip = apqpKickOffData.ManufacturerZip;
                         kickOff.EAUUsage = apqpKickOffData.EAUUsage;
                         kickOff.PartNumber = apqpKickOffData.PartNumber;
                         kickOff.PartDesc = apqpKickOffData.PartDesc;
                         kickOff.MaterialType = apqpKickOffData.MaterialType;
                         kickOff.PartWeight = apqpKickOffData.PartWeight;
                         kickOff.ProjectNotes = apqpKickOffData.ProjectNotes;
                         kickOff.CustomerToolingPONumber = apqpKickOffData.CustomerToolingPONumber;
                         kickOff.PurchasePieceCost = apqpKickOffData.PurchasePieceCost;
                         kickOff.PurchaseToolingCost = apqpKickOffData.PurchaseToolingCost;
                         kickOff.SellingPiecePrice = apqpKickOffData.SellingPiecePrice;
                         kickOff.SellingToolingPrice = apqpKickOffData.SellingToolingPrice;
                         kickOff.ToolingLeadtimeDays = apqpKickOffData.ToolingLeadtimeDays;
                         kickOff.APQPEngineerId = apqpKickOffData.APQPEngineerId;
                         kickOff.APQPEngineer = apqpKickOffData.APQPEngineer;
                         kickOff.SupplyChainCoordinatorId = apqpKickOffData.SupplyChainCoordinatorId;
                         kickOff.SupplyChainCoordinator = apqpKickOffData.SupplyChainCoordinator;
                         kickOff.PartClassification = apqpKickOffData.PartClassification;
                         kickOff.CommodityTypeId = apqpKickOffData.WorkTypeId;
                         kickOff.CommodityType = apqpKickOffData.WorkType;
                         kickOff.CommodityId = apqpKickOffData.CommodityId;
                         kickOff.Commodity = apqpKickOffData.Commodity;
                         kickOff.ProcessId = apqpKickOffData.ProcessId;
                         kickOff.Process = apqpKickOffData.Process;
                         kickOff.AdditionalPartDescription = apqpKickOffData.AdditionalPartDescription;
                         kickOff.StatusId = apqpKickOffData.StatusId;
                         kickOff.Status = apqpKickOffData.Status;
                         kickOff.IsSourceFromSAP = apqpKickOffData.IsSourceFromSAP;
                         kickOff.SAMUserId = apqpKickOffData.SAMUserId;
                         kickOff.SAMUser = apqpKickOffData.SAMUser;
                         //kickOff.QuoteDetailId = apqpKickOffData.QuoteDetailId;
                         //kickOff.UpdatedFromSource = apqpKickOffData.UpdatedFromSource;
                         kickOff.DrawingNumber = apqpKickOffData.DrawingNumber;
                         kickOff.DestinationId = apqpKickOffData.DestinationId;
                         kickOff.Destination = apqpKickOffData.Destination;
                         //kickOff.APQPSAPItemId = apqpKickOffData.APQPSAPItemId;
                         // kickOff.KickOffDocument = apqpKickOffData.KickOffDocument;
                         kickOff.RevisionDate = apqpKickOffData.RevisionDate;
                         kickOff.RevLevel = apqpKickOffData.RevLevel;
                         kickOff.IsDocument = apqpKickOffData.IsDocument;
                         kickOff.DaysOld = apqpKickOffData.DaysOld;
                         kickOff.PNDaysOld = apqpKickOffData.PNDaysOld;
                         kickOff.PageNumber = apqpKickOffData.PageNumber;
                         kickOff.DocuSignStatus = GetNPIFDocuSignStatus(apqpKickOffData.Id);

                         if (string.IsNullOrEmpty(kickOff.DocuSignStatus))
                             kickOff.showDocuSignInitiatedIcon = false;
                         else
                         {
                             kickOff.showDocuSignInitiatedIcon = true;
                             if (kickOff.DocuSignStatus == DocuSignEnvelopeStatus.completed.ToString())
                                 kickOff.showDocuSignCompletedIcon = true;
                             else
                                 kickOff.showDocuSignInitiatedIcon = true;
                         }
                         lstKickOff.Add(kickOff);
                         #endregion
                     }
                 }
             });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.APQP.KickOff>>(errMSg, lstKickOff);
            response.PageInfo = searchCriteria.ToPage();
            return response;
        }
    }
}
