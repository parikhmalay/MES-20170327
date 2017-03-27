using Account.DTO.Library;
using MES.Business.Mapping.Extensions;
using MES.Business.Repositories.APQP.CAPA;
using NPE.Core;
using NPE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Winnovative.ExcelLib;
using NPE.Core.Extensions;
using System.IO;
using MES.Business.Repositories.RFQ.Supplier;

namespace MES.Business.Library.BO.APQP.CAPA
{
    class CAPA : ContextBusinessBase, ICAPARepository
    {
        public CAPA()
            : base("CAPA")
        { }
        public ITypedResponse<string> CheckPartAssociationWithSupplier(DTO.Library.APQP.CAPA.CAPA cAPA)
        {
            string retMsg = string.Empty;
            if (!string.IsNullOrEmpty(cAPA.PartCodes))
            {
                this.RunOnDB(context =>
                    {
                        retMsg = context.capaPartsNotAssociated(cAPA.SupplierCodeWithName, cAPA.CustomerCodeWithName, cAPA.PartCodes).SingleOrDefault();
                    });
                if (!string.IsNullOrEmpty(retMsg))
                    retMsg = retMsg + " of " + cAPA.CustomerName + " is/are not associated with " + cAPA.SupplierName + ". CAPA will not be created. Please select parts associated with one Supplier only.";
                //"<Part Nos.>"  of "<CustomerName>" is/are not associated with "<SupplierName>". CAPA will not be created. Please select parts associated with one Supplier only.
                else
                    retMsg = string.Empty;
            }
            return SuccessResponse(retMsg);
        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.APQP.CAPA.CAPA cAPA)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.capaItemMaster();
            //bool IsNewRecord = true;
            if (cAPA.Id > 0)
            {
                recordToBeUpdated = this.DataContext.capaItemMasters.Where(a => a.Id == cAPA.Id).SingleOrDefault();
                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("CAPANotExists");
                else
                {
                    //IsNewRecord = false;
                    if (cAPA.Mode != "DropdownButton" && string.IsNullOrEmpty(cAPA.PartNumber))
                    {
                        #region "Delete Detailed description of problem (date shipped, part numbers)"
                        var deleteCAPAProblemDescription = this.DataContext.capaProblemDescriptions.Where(a => a.CorrectiveActionId == cAPA.Id).ToList();
                        foreach (var item in deleteCAPAProblemDescription)
                        {
                            this.DataContext.capaProblemDescriptions.Remove(item);
                        }
                        #endregion
                        #region "Delete Temporary Countermeasure"
                        var deletecapaTempCountermeasures = this.DataContext.capaTempCountermeasures.Where(a => a.CorrectiveActionId == cAPA.Id).ToList();
                        foreach (var item in deletecapaTempCountermeasures)
                        {
                            this.DataContext.capaTempCountermeasures.Remove(item);
                        }
                        #endregion
                        #region "Delete Verification: (Action) "
                        var deleteCAPAVerification = this.DataContext.capaVerifications.Where(a => a.CorrectiveActionId == cAPA.Id).ToList();
                        foreach (var item in deleteCAPAVerification)
                        {
                            this.DataContext.capaVerifications.Remove(item);
                        }
                        #endregion
                        #region "Delete Root Cause- Why Made?"
                        var deletecapaRootCauseWhyMade = this.DataContext.capaRootCauseWhyMades.Where(a => a.CorrectiveActionId == cAPA.Id).ToList();
                        foreach (var item in deletecapaRootCauseWhyMade)
                        {
                            this.DataContext.capaRootCauseWhyMades.Remove(item);
                        }
                        #endregion
                        #region "Delete Root Cause- Why shipped?"
                        var deletecapaRootCauseWhyShipped = this.DataContext.capaRootCauseWhyShippeds.Where(a => a.CorrectiveActionId == cAPA.Id).ToList();
                        foreach (var item in deletecapaRootCauseWhyShipped)
                        {
                            this.DataContext.capaRootCauseWhyShippeds.Remove(item);
                        }
                        #endregion
                        #region "Delete Permanent Countermeasure"
                        var deleteCAPAPermanentCountermeasure = this.DataContext.capaPermanentCountermeasures.Where(a => a.CorrectiveActionId == cAPA.Id).ToList();
                        foreach (var item in deleteCAPAPermanentCountermeasure)
                        {
                            this.DataContext.capaPermanentCountermeasures.Remove(item);
                        }
                        #endregion
                        #region "Delete Feed forward"
                        var deleteCAPAFeedForward = this.DataContext.capaFeedForwards.Where(a => a.CorrectiveActionId == cAPA.Id).ToList();
                        foreach (var item in deleteCAPAFeedForward)
                        {
                            this.DataContext.capaFeedForwards.Remove(item);
                        }
                        #endregion
                    }
                    #region "Approvals"
                    var deletecapaApprovals = this.DataContext.capaApprovals.Where(a => a.CorrectiveActionId == cAPA.Id).ToList();
                    foreach (var item in deletecapaApprovals)
                    {
                        this.DataContext.capaApprovals.Remove(item);
                    }
                    #endregion

                    cAPA.DefectTrackingId = recordToBeUpdated.DefectTrackingId;
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
                this.DataContext.capaItemMasters.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                #region Save general details
                recordToBeUpdated.IncludeInPPM = cAPA.IncludeInPPM;
                recordToBeUpdated.CorrectiveActionType = cAPA.CorrectiveActionType;
                recordToBeUpdated.CorrectiveActionInitiatedBy = cAPA.CorrectiveActionInitiatedBy;
                recordToBeUpdated.CorrectiveActionInitiatedDate = cAPA.CorrectiveActionInitiatedDate;
                recordToBeUpdated.SupplierName = cAPA.SupplierName;
                recordToBeUpdated.SupplierCode = cAPA.SupplierCode;
                recordToBeUpdated.CustomerCode = cAPA.CustomerCode;
                recordToBeUpdated.CustomerName = cAPA.CustomerName;
                recordToBeUpdated.SupplierContactName = cAPA.SupplierContactName;
                recordToBeUpdated.ReviewCtrlPlan = cAPA.ReviewCtrlPlan;
                recordToBeUpdated.ReviewFMEA = cAPA.ReviewFMEA;
                this.DataContext.SaveChanges();
                cAPA.Id = recordToBeUpdated.Id;
                #endregion

                bool AnyRecord = false;
                //TO DO save document
                if (cAPA.Mode != "DropdownButton" && string.IsNullOrEmpty(cAPA.PartNumber))
                {
                    #region "Save Part Name(s) and Part # (S) Affected"
                    AnyRecord = false;
                    if (cAPA.lstcapaPartAffectedDetails != null && cAPA.lstcapaPartAffectedDetails.Count > 0)
                    {
                        MES.Business.Repositories.APQP.CAPA.ICAPAPartAffectedDetailRepository objICAPAPartAffectedDetailRepository = null;
                        foreach (var objcapaPartAffectedDetail in cAPA.lstcapaPartAffectedDetails.Where(a => a.IsDeletedFromObject == false).ToList())
                        {
                            objICAPAPartAffectedDetailRepository = new MES.Business.Library.BO.APQP.CAPA.CAPAPartAffectedDetail();
                            objcapaPartAffectedDetail.CorrectiveActionId = cAPA.Id;
                            var objResult = objICAPAPartAffectedDetailRepository.Save(objcapaPartAffectedDetail);
                            if (objResult == null || objResult.StatusCode != 200)
                            {
                                errMSg = Languages.GetResourceText("CAPASaveFail");
                            }
                        }
                        foreach (var objcapaPartAffectedDetail in cAPA.lstcapaPartAffectedDetails.Where(a => a.IsDeletedFromObject == true).ToList())
                        {
                            var objResult = DeleteCAPAPartAffectedDetail(objcapaPartAffectedDetail.Id);
                            if (objResult == null || objResult.StatusCode != 200)
                            {
                                errMSg = Languages.GetResourceText("CAPASaveFail");
                            }
                        }
                    }
                    #endregion

                    #region "Save Detailed description of problem (date shipped, part numbers)"
                    AnyRecord = false;
                    MES.Data.Library.capaProblemDescription dbocapaProblemDescription = null;
                    if (cAPA.lstcapaProblemDescriptions != null && cAPA.lstcapaProblemDescriptions.Count > 0)
                    {
                        foreach (var objcapaProblemDescription in cAPA.lstcapaProblemDescriptions)
                        {
                            AnyRecord = true;
                            dbocapaProblemDescription = new MES.Data.Library.capaProblemDescription();
                            dbocapaProblemDescription.CorrectiveActionId = cAPA.Id;
                            dbocapaProblemDescription.QueryId = objcapaProblemDescription.QueryId;
                            dbocapaProblemDescription.Reason = objcapaProblemDescription.Reason;
                            dbocapaProblemDescription.CreatedBy = CurrentUser;
                            dbocapaProblemDescription.CreatedDate = AuditUtils.GetCurrentDateTime();
                            this.DataContext.capaProblemDescriptions.Add(dbocapaProblemDescription);
                        }
                        if (AnyRecord)
                            this.DataContext.SaveChanges();
                    }
                    #endregion

                    #region "Save Temporary Countermeasure"
                    AnyRecord = false;
                    MES.Data.Library.capaTempCountermeasure dbocapaTempCountermeasure = null;
                    if (cAPA.lstcapaTempCountermeasures != null && cAPA.lstcapaTempCountermeasures.Count > 0)
                    {
                        foreach (var objcapaTempCountermeasure in cAPA.lstcapaTempCountermeasures)
                        {
                            AnyRecord = true;
                            dbocapaTempCountermeasure = new MES.Data.Library.capaTempCountermeasure();
                            dbocapaTempCountermeasure.CorrectiveActionId = cAPA.Id;
                            dbocapaTempCountermeasure.Description = objcapaTempCountermeasure.Description;
                            dbocapaTempCountermeasure.EffectiveDate = objcapaTempCountermeasure.EffectiveDate;
                            dbocapaTempCountermeasure.CreatedBy = CurrentUser;
                            dbocapaTempCountermeasure.CreatedDate = AuditUtils.GetCurrentDateTime();
                            this.DataContext.capaTempCountermeasures.Add(dbocapaTempCountermeasure);
                        }
                        if (AnyRecord)
                            this.DataContext.SaveChanges();
                    }
                    #endregion

                    #region "Save Verification: (Action)"
                    AnyRecord = false;
                    MES.Data.Library.capaVerification dbocapaVerification = null;
                    if (cAPA.lstcapaVerifications != null && cAPA.lstcapaVerifications.Count > 0)
                    {
                        foreach (var objcapaVerification in cAPA.lstcapaVerifications)
                        {
                            AnyRecord = true;
                            dbocapaVerification = new MES.Data.Library.capaVerification();
                            dbocapaVerification.CorrectiveActionId = cAPA.Id;
                            dbocapaVerification.Description = objcapaVerification.Description;
                            dbocapaVerification.VerificationDate = objcapaVerification.VerificationDate;
                            dbocapaVerification.CreatedBy = CurrentUser;
                            dbocapaVerification.CreatedDate = AuditUtils.GetCurrentDateTime();
                            this.DataContext.capaVerifications.Add(dbocapaVerification);
                        }
                        if (AnyRecord)
                            this.DataContext.SaveChanges();
                    }
                    #endregion

                    #region "Save Root Cause- Why Made?"
                    AnyRecord = false;
                    MES.Data.Library.capaRootCauseWhyMade dbocapaRootCauseWhyMade = null;
                    if (cAPA.lstcapaRootCauseWhyMade != null && cAPA.lstcapaRootCauseWhyMade.Count > 0)
                    {
                        foreach (var objcapaRootCauseWhyMade in cAPA.lstcapaRootCauseWhyMade)
                        {
                            AnyRecord = true;
                            dbocapaRootCauseWhyMade = new MES.Data.Library.capaRootCauseWhyMade();
                            dbocapaRootCauseWhyMade.CorrectiveActionId = cAPA.Id;
                            dbocapaRootCauseWhyMade.QueryId = objcapaRootCauseWhyMade.QueryId;
                            dbocapaRootCauseWhyMade.Reason = objcapaRootCauseWhyMade.Reason;
                            dbocapaRootCauseWhyMade.CreatedBy = CurrentUser;
                            dbocapaRootCauseWhyMade.CreatedDate = AuditUtils.GetCurrentDateTime();
                            this.DataContext.capaRootCauseWhyMades.Add(dbocapaRootCauseWhyMade);
                        }
                        if (AnyRecord)
                            this.DataContext.SaveChanges();
                    }
                    #endregion

                    #region "Save Root Cause- Why shipped?"
                    AnyRecord = false;
                    MES.Data.Library.capaRootCauseWhyShipped dbocapaRootCauseWhyShipped = null;
                    if (cAPA.lstcapaRootCauseWhyShipped != null && cAPA.lstcapaRootCauseWhyShipped.Count > 0)
                    {
                        foreach (var objcapaRootCauseWhyShipped in cAPA.lstcapaRootCauseWhyShipped)
                        {
                            AnyRecord = true;
                            dbocapaRootCauseWhyShipped = new MES.Data.Library.capaRootCauseWhyShipped();
                            dbocapaRootCauseWhyShipped.CorrectiveActionId = cAPA.Id;
                            dbocapaRootCauseWhyShipped.QueryId = objcapaRootCauseWhyShipped.QueryId;
                            dbocapaRootCauseWhyShipped.Reason = objcapaRootCauseWhyShipped.Reason;
                            dbocapaRootCauseWhyShipped.CreatedBy = CurrentUser;
                            dbocapaRootCauseWhyShipped.CreatedDate = AuditUtils.GetCurrentDateTime();
                            this.DataContext.capaRootCauseWhyShippeds.Add(dbocapaRootCauseWhyShipped);
                        }
                        if (AnyRecord)
                            this.DataContext.SaveChanges();
                    }
                    #endregion

                    #region "Save Permanent Countermeasure"
                    AnyRecord = false;
                    MES.Data.Library.capaPermanentCountermeasure dbocapaPermanentCountermeasure = null;
                    if (cAPA.lstcapaPermanentCountermeasures != null && cAPA.lstcapaPermanentCountermeasures.Count > 0)
                    {
                        foreach (var objcapaPermanentCountermeasure in cAPA.lstcapaPermanentCountermeasures)
                        {
                            AnyRecord = true;
                            dbocapaPermanentCountermeasure = new MES.Data.Library.capaPermanentCountermeasure();
                            dbocapaPermanentCountermeasure.CorrectiveActionId = cAPA.Id;
                            dbocapaPermanentCountermeasure.Description = objcapaPermanentCountermeasure.Description;
                            dbocapaPermanentCountermeasure.EffectiveDate = objcapaPermanentCountermeasure.EffectiveDate;
                            dbocapaPermanentCountermeasure.CreatedBy = CurrentUser;
                            dbocapaPermanentCountermeasure.CreatedDate = AuditUtils.GetCurrentDateTime();
                            this.DataContext.capaPermanentCountermeasures.Add(dbocapaPermanentCountermeasure);
                        }
                        if (AnyRecord)
                            this.DataContext.SaveChanges();
                    }
                    #endregion

                    #region "Save Feed forward"
                    AnyRecord = false;
                    MES.Data.Library.capaFeedForward dbocapaFeedForward = null;
                    if (cAPA.lstcapaFeedForwards != null && cAPA.lstcapaFeedForwards.Count > 0)
                    {
                        foreach (var objcapaFeedForward in cAPA.lstcapaFeedForwards)
                        {
                            AnyRecord = true;
                            dbocapaFeedForward = new MES.Data.Library.capaFeedForward();
                            dbocapaFeedForward.CorrectiveActionId = cAPA.Id;
                            dbocapaFeedForward.Description = objcapaFeedForward.Description;
                            dbocapaFeedForward.CreatedBy = CurrentUser;
                            dbocapaFeedForward.CreatedDate = AuditUtils.GetCurrentDateTime();
                            this.DataContext.capaFeedForwards.Add(dbocapaFeedForward);
                        }
                        if (AnyRecord)
                            this.DataContext.SaveChanges();
                    }
                    #endregion

                }
                else
                {
                    ///// insert affected parts on the basis of part selection
                    #region "Save Part Name(s) and Part # (S) Affected"
                    if (!string.IsNullOrEmpty(cAPA.PartNumber))
                    {
                        MES.Business.Repositories.APQP.CAPA.ICAPAPartAffectedDetailRepository objICAPAPartAffectedDetailRepository = null;
                        DTO.Library.APQP.CAPA.capaPartAffectedDetail objCAPAPartAffectedDetail = null;
                        foreach (var appqItemId in cAPA.PartNumber.Split(',').ToList())
                        {
                            objICAPAPartAffectedDetailRepository = new MES.Business.Library.BO.APQP.CAPA.CAPAPartAffectedDetail();
                            objCAPAPartAffectedDetail = new DTO.Library.APQP.CAPA.capaPartAffectedDetail();
                            objCAPAPartAffectedDetail.CorrectiveActionId = cAPA.Id;
                            objCAPAPartAffectedDetail.APQPItemId = Convert.ToInt32(appqItemId);
                            objCAPAPartAffectedDetail.PartName = this.DataContext.SAPItemFlatTables.Where(a => a.Id == objCAPAPartAffectedDetail.APQPItemId).SingleOrDefault().ItemName;
                            var objResult = objICAPAPartAffectedDetailRepository.Save(objCAPAPartAffectedDetail);
                            if (objResult == null || objResult.StatusCode != 200)
                            {
                                errMSg = Languages.GetResourceText("CAPASaveFail");
                            }
                        }
                    }
                    #endregion
                }
                #region "Save Approvals"
                AnyRecord = false;
                MES.Data.Library.capaApproval dbocapaApproval = null;
                if (cAPA.lstcapaApprovals != null && cAPA.lstcapaApprovals.Count > 0)
                {
                    foreach (var objcapaApproval in cAPA.lstcapaApprovals)
                    {
                        AnyRecord = true;
                        dbocapaApproval = new MES.Data.Library.capaApproval();
                        dbocapaApproval.CorrectiveActionId = cAPA.Id;
                        dbocapaApproval.TitleId = objcapaApproval.TitleId;
                        dbocapaApproval.Name = objcapaApproval.Name;
                        dbocapaApproval.ApprovalDate = objcapaApproval.ApprovalDate;
                        dbocapaApproval.CreatedBy = CurrentUser;
                        dbocapaApproval.CreatedDate = AuditUtils.GetCurrentDateTime();
                        this.DataContext.capaApprovals.Add(dbocapaApproval);
                    }
                    if (AnyRecord)
                        this.DataContext.SaveChanges();
                }
                #endregion

                successMsg = Languages.GetResourceText("CAPASavedSuccess");
            }
            if (cAPA.AddToDT == 1)
            {
                if (cAPA.DefectTrackingId.HasValue && cAPA.DefectTrackingId.Value > 0)  // if defectTracking item is already created then just redirect to DT in Edit mode. so returning only DT Id.
                {
                    cAPA.Id = cAPA.DefectTrackingId.Value; //return DT Id 
                }
                else
                {
                    try
                    {
                        int dtId = CreateDTFromCAPA(cAPA.Id);
                        recordToBeUpdated = this.DataContext.capaItemMasters.Where(a => a.Id == cAPA.Id).SingleOrDefault();
                        recordToBeUpdated.DefectTrackingId = dtId;
                        this.DataContext.SaveChanges();
                        cAPA.Id = dtId; // return DT Id
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, cAPA.Id, successMsg);
        }
        public int CreateDTFromCAPA(int capaId)
        {
            #region "Create new defect tracking record. Note : followed the step of Defect tracking Save function"
            #region "Save dt master table data"
            int dtId = 0;
            MES.Business.Repositories.APQP.DefectTracking.IDefectTrackingRepository objIDefectTrackingRepository = new MES.Business.Library.BO.APQP.DefectTracking.DefectTracking();
            var objCAPA = FindById(capaId).Result;
            var objCreateDefectTracking = new MES.Data.Library.dtDefectTracking();
            objCreateDefectTracking.IncludeInPPM = objCAPA.IncludeInPPM;
            objCreateDefectTracking.Finding = objCAPA.CorrectiveActionType;
            objCreateDefectTracking.QualityOrDeliveryIssue = "Quality";
            objCreateDefectTracking.CustomerCode = objCAPA.CustomerCode;
            objCreateDefectTracking.CustomerName = objCAPA.CustomerName;
            objCreateDefectTracking.RMANumber = objIDefectTrackingRepository.GetNewRMANumber().Result;
            objCreateDefectTracking.RMAInitiatedBy = CurrentUser;
            objCreateDefectTracking.RMADate = objCreateDefectTracking.CreatedDate = AuditUtils.GetCurrentDateTime();
            objCreateDefectTracking.CreatedBy = objCreateDefectTracking.UpdatedBy = CurrentUser;
            this.DataContext.dtDefectTrackings.Add(objCreateDefectTracking);
            this.DataContext.SaveChanges();
            dtId = objCreateDefectTracking.Id;
            #endregion
            #region "Save defect tracking Details"
            MES.Data.Library.dtDefectTrackingDetail objdtDefectTrackingDetail = null;
            string WeightPerPiece = string.Empty;
            decimal SellingPricePerPiece = 0;
            ObjectParameter ErrorKey = new ObjectParameter("ErrorKey", "");
            foreach (var objcapaPartAffectedDetail in objCAPA.lstcapaPartAffectedDetails)
            {
                #region "Get data from SAP"
                WeightPerPiece = string.Empty;
                SellingPricePerPiece = 0;
                var objSAPItemFlatTables = this.DataContext.SAPItemFlatTables.Where(a => a.Id == objcapaPartAffectedDetail.APQPItemId).SingleOrDefault();
                if (!string.IsNullOrEmpty(objSAPItemFlatTables.ItemWeight) && objSAPItemFlatTables.ItemWeight.ToUpper() != "NULL")
                {
                    WeightPerPiece = objSAPItemFlatTables.ItemWeight;
                }
                if (!string.IsNullOrEmpty(objSAPItemFlatTables.SalesPrice) && objSAPItemFlatTables.SalesPrice.ToUpper() != "NULL")
                {
                    SellingPricePerPiece = Convert.ToDecimal(objSAPItemFlatTables.SalesPrice);
                }
                #endregion
                #region "Save in DT detail table"
                objdtDefectTrackingDetail = new MES.Data.Library.dtDefectTrackingDetail();
                objdtDefectTrackingDetail.DefectTrackingId = dtId;
                objdtDefectTrackingDetail.APQPItemId = objcapaPartAffectedDetail.APQPItemId;
                objdtDefectTrackingDetail.PartName = objcapaPartAffectedDetail.PartName;
                objdtDefectTrackingDetail.SupplierCode = objCAPA.SupplierCode;
                objdtDefectTrackingDetail.SupplierName = objCAPA.SupplierName;
                objdtDefectTrackingDetail.SupplierContactName = objCAPA.SupplierContactName;
                objdtDefectTrackingDetail.CorrectiveActionNumber = Convert.ToString(objCAPA.Id);
                objdtDefectTrackingDetail.CorrectiveActionInitiatedDate = objCAPA.CorrectiveActionInitiatedDate;
                objdtDefectTrackingDetail.CorrectiveActionInitiatedBy = objCAPA.CorrectiveActionInitiatedBy;

                objdtDefectTrackingDetail.WeightPerPiece = WeightPerPiece;
                objdtDefectTrackingDetail.SellingPricePerPiece = SellingPricePerPiece;
                if (objcapaPartAffectedDetail.DefectTypeId.HasValue && objcapaPartAffectedDetail.DefectTypeId.Value > 0)
                    objdtDefectTrackingDetail.DefectDescription = this.DataContext.DefectTypes.Where(a => a.Id == objcapaPartAffectedDetail.DefectTypeId.Value).SingleOrDefault().DefectType1;

                objdtDefectTrackingDetail.CustomerInitialRejectQty = objdtDefectTrackingDetail.TotalNumberOfPartsRejected = objdtDefectTrackingDetail.CustomerRejectedPartQty = objcapaPartAffectedDetail.CustomerRejectedPartQty;
                objdtDefectTrackingDetail.DispositionOfParts = "Scrap";
                objdtDefectTrackingDetail.Region = "USA";

                objdtDefectTrackingDetail.CreatedDate = objCreateDefectTracking.CreatedDate;
                objdtDefectTrackingDetail.CreatedBy = objdtDefectTrackingDetail.UpdatedBy = CurrentUser;
                this.DataContext.dtDefectTrackingDetails.Add(objdtDefectTrackingDetail);
                this.DataContext.SaveChanges();

                #region "Insert capa docs into DT document table"
                this.DataContext.capaSaveCAPADocumentInToDTDocument(objdtDefectTrackingDetail.Id, objcapaPartAffectedDetail.Id, CurrentUser, ErrorKey);
                #endregion

                #endregion
            }

            #region "Generate RMA Form"
            try
            {
                objIDefectTrackingRepository.GenerateRMAFormFromCAPA(dtId);
            }
            catch (Exception ex)
            {
            }
            #endregion

            #endregion

            #endregion
            return dtId;
        }

        public DTO.Library.APQP.CAPA.capaQuery QueryFindById(short id)
        {
            DTO.Library.APQP.CAPA.capaQuery capaQuery = new DTO.Library.APQP.CAPA.capaQuery();
            this.RunOnDB(context =>
            {
                var objcapaQuery = context.capaQueries.Where(a => a.Id == id).SingleOrDefault();
                if (objcapaQuery == null)
                {
                    //errMSg = Languages.GetResourceText("QueryNotExists"); 
                }
                else
                {
                    capaQuery = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.CAPA.capaQuery>(objcapaQuery);
                }
            });
            return capaQuery;
        }

        public NPE.Core.ITypedResponse<DTO.Library.APQP.CAPA.CAPA> FindById(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.APQP.CAPA.CAPA cAPA = new DTO.Library.APQP.CAPA.CAPA();
            this.RunOnDB(context =>
            {
                var objCAPA = context.capaItemMasters.Where(a => a.Id == id).SingleOrDefault();
                if (objCAPA == null)
                    errMSg = Languages.GetResourceText("CAPANotExists");
                else
                {
                    #region general details
                    cAPA = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.CAPA.CAPA>(objCAPA);
                    #endregion

                    #region Bind Part Name(s) and Part # (S) Affected
                    var lstcapaPartAffectedDetail = context.capaPartAffectedDetails.Where(a => a.CorrectiveActionId == id).ToList();
                    cAPA.lstcapaPartAffectedDetails = new List<DTO.Library.APQP.CAPA.capaPartAffectedDetail>();
                    DTO.Library.APQP.CAPA.capaPartAffectedDetail objcapaPartAffectedDetail = null;
                    foreach (var item in lstcapaPartAffectedDetail)
                    {
                        objcapaPartAffectedDetail = new DTO.Library.APQP.CAPA.capaPartAffectedDetail();
                        objcapaPartAffectedDetail = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.CAPA.capaPartAffectedDetail>(item);
                        objcapaPartAffectedDetail.PartCode = context.SAPItemFlatTables.Where(a => a.Id == objcapaPartAffectedDetail.APQPItemId).SingleOrDefault().ItemCode;
                        cAPA.lstcapaPartAffectedDetails.Add(objcapaPartAffectedDetail);
                    }

                    #endregion
                    #region Detailed description of problem (date shipped, part numbers)
                    var lstcapaProblemDescription = context.capaProblemDescriptions.Where(a => a.CorrectiveActionId == id).ToList();
                    cAPA.lstcapaProblemDescriptions = ObjectLibExtensions.AutoConvert<List<DTO.Library.APQP.CAPA.capaProblemDescription>>(lstcapaProblemDescription);
                    #endregion
                    #region Temporary Countermeasure
                    var lstcapaTempCountermeasure = context.capaTempCountermeasures.Where(a => a.CorrectiveActionId == id).ToList();
                    cAPA.lstcapaTempCountermeasures = ObjectLibExtensions.AutoConvert<List<DTO.Library.APQP.CAPA.capaTempCountermeasure>>(lstcapaTempCountermeasure);
                    #endregion
                    #region Verification: (Action)
                    var lstcapaVerification = context.capaVerifications.Where(a => a.CorrectiveActionId == id).ToList();
                    cAPA.lstcapaVerifications = ObjectLibExtensions.AutoConvert<List<DTO.Library.APQP.CAPA.capaVerification>>(lstcapaVerification);
                    #endregion
                    #region Root Cause- Why Made?
                    var lstcapaRootCauseWhyMade = context.capaRootCauseWhyMades.Where(a => a.CorrectiveActionId == id).ToList();
                    cAPA.lstcapaRootCauseWhyMade = ObjectLibExtensions.AutoConvert<List<DTO.Library.APQP.CAPA.capaRootCauseWhyMade>>(lstcapaRootCauseWhyMade);
                    #endregion
                    #region Root Cause- Why shipped?
                    var lstcapaRootCauseWhyShipped = context.capaRootCauseWhyShippeds.Where(a => a.CorrectiveActionId == id).ToList();
                    cAPA.lstcapaRootCauseWhyShipped = ObjectLibExtensions.AutoConvert<List<DTO.Library.APQP.CAPA.capaRootCauseWhyShipped>>(lstcapaRootCauseWhyShipped);
                    #endregion
                    #region Permanent Countermeasure
                    var lstcapaPermanentCountermeasure = context.capaPermanentCountermeasures.Where(a => a.CorrectiveActionId == id).ToList();
                    cAPA.lstcapaPermanentCountermeasures = ObjectLibExtensions.AutoConvert<List<DTO.Library.APQP.CAPA.capaPermanentCountermeasure>>(lstcapaPermanentCountermeasure);
                    #endregion
                    #region Feed forward
                    var lstcapaFeedForward = context.capaFeedForwards.Where(a => a.CorrectiveActionId == id).ToList();
                    cAPA.lstcapaFeedForwards = ObjectLibExtensions.AutoConvert<List<DTO.Library.APQP.CAPA.capaFeedForward>>(lstcapaFeedForward);
                    #endregion
                    #region Approvals
                    var lstcapaApproval = context.capaApprovals.Where(a => a.CorrectiveActionId == id).Include(a => a.capaApproverTitle).ToList();
                    cAPA.lstcapaApprovals = new List<DTO.Library.APQP.CAPA.capaApproval>();
                    DTO.Library.APQP.CAPA.capaApproval objcapaApproval = null;
                    foreach (var item in lstcapaApproval)
                    {
                        objcapaApproval = new DTO.Library.APQP.CAPA.capaApproval();
                        //item.TitleId
                        objcapaApproval = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.CAPA.capaApproval>(item);
                        objcapaApproval.Title = item.capaApproverTitle.Value;
                        cAPA.lstcapaApprovals.Add(objcapaApproval);
                    }
                    #endregion
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.APQP.CAPA.CAPA>(errMSg, cAPA);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int cAPAId)
        {
            try
            {
                var capaItemMasterToBeDeleted = this.DataContext.capaItemMasters.Where(a => a.Id == cAPAId).SingleOrDefault();
                if (capaItemMasterToBeDeleted == null)
                {
                    return FailedBoolResponse(Languages.GetResourceText("CAPAPNotExists"));
                }
                else
                {
                    capaItemMasterToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    capaItemMasterToBeDeleted.UpdatedBy = CurrentUser;
                    this.DataContext.Entry(capaItemMasterToBeDeleted).State = EntityState.Modified;
                    capaItemMasterToBeDeleted.IsDeleted = true;
                    this.DataContext.SaveChanges();
                    return SuccessBoolResponse(Languages.GetResourceText("CAPADeletedSuccess"));
                }
            }
            catch (Exception ex)
            {
                return FailedBoolResponse(Languages.GetResourceText("CAPADeleteFail"));
            }
        }

        public NPE.Core.ITypedResponse<bool?> DeleteCAPAPartAffectedDetail(int CAPAPartAffectedDetailId)
        {
            #region Delete CAPAPartAffectedDetail
            ICAPAPartAffectedDetailRepository objICAPAPartAffectedDetailRepository = new MES.Business.Library.BO.APQP.CAPA.CAPAPartAffectedDetail();
            return objICAPAPartAffectedDetailRepository.Delete(CAPAPartAffectedDetailId);
            #endregion
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.CAPA.CAPA>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.CAPA.CAPA>> GetCAPAList(NPE.Core.IPage<DTO.Library.APQP.CAPA.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            var httpContext = System.Web.HttpContext.Current;
            List<DTO.Library.APQP.CAPA.CAPA> lstCAPA = new List<DTO.Library.APQP.CAPA.CAPA>();
            DTO.Library.APQP.CAPA.CAPA cAPA;
            this.RunOnDB(context =>
             {
                 var CAPAList = context.capaSearchCAPA(paging.Criteria.CorrectiveActionNumber, paging.Criteria.CustomerName, paging.Criteria.SupplierCode
                 , paging.Criteria.SupplierName, paging.Criteria.APQPItemId, paging.Criteria.CAPAInitiatedBy,
                  paging.Criteria.DefectTypeId, paging.Criteria.OpenDateFrom, paging.Criteria.OpenDateTo,
                  paging.PageNo, paging.PageSize, totalRecords, "").ToList();

                 if (CAPAList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in CAPAList)
                     {
                         cAPA = new DTO.Library.APQP.CAPA.CAPA();
                         cAPA = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.CAPA.CAPA>(item);
                         cAPA.PartNumber = item.PartNumber;
                         cAPA.Supplier = item.Supplier;
                         cAPA.CustomerRejectedPartQty = item.CustomerRejectedPartQty;
                         lstCAPA.Add(cAPA);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.CAPA.CAPA>>(errMSg, lstCAPA);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<List<MES.DTO.Library.Common.LookupFields>> getSAPPartsList(string supplierName, string customerName)
        {
            //set the out put param
            string errMSg = null;
            List<MES.DTO.Library.Common.LookupFields> lstParts = new List<DTO.Library.Common.LookupFields>();
            DTO.Library.Common.LookupFields parts;
            this.RunOnDB(context =>
            {
                var PartsList = context.capaGetSAPPartsList(supplierName, customerName).ToList();
                if (PartsList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var item in PartsList)
                    {
                        parts = new DTO.Library.Common.LookupFields();
                        parts.Id = item.Id;
                        parts.Name = item.ItemCode + " - " + item.ItemName;
                        parts.ParentId = item.ItemName;
                        parts.Code = item.ItemCode;
                        lstParts.Add(parts);
                    }
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Common.LookupFields>>(errMSg, lstParts);
            return response;
        }

        public ITypedResponse<bool?> GenerateCAPAForm(DTO.Library.APQP.CAPA.CAPA capa)
        {
            string errMSg = null, filepath = string.Empty;
            var context = HttpContext.Current;
            string directoryPath = context.Server.MapPath(Constants.APQPTemplateFolder)
                       , filePath = context.Server.MapPath(Constants.APQPTemplateFolder) + "DTCAPAFormTemplate.xls";

            try
            {
                bool isValid = true;
                //A CAPA form can be associated with only one supplier. Please select the same supplier for all parts.
                string supplierName = string.Empty, supplierTemp = string.Empty, supplierCode = string.Empty;

                if (isValid)
                {

                    if (System.IO.Directory.Exists(directoryPath))
                    {
                        if (File.Exists(filePath))
                        {
                            System.IO.FileStream sourceXlsDataStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                            ExcelWorkbook ew = new ExcelWorkbook(sourceXlsDataStream);
                            ew.LicenseKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["wnvkey"]);

                            ExcelWorksheet ws = ew.Worksheets[0]; ws.Name = "Summary Sheet";
                            ExcelWorksheet ws1 = ew.Worksheets[1]; ws1.Name = "Defective Part or Process Pics.";
                            ExcelWorksheet ws2 = ew.Worksheets[2]; ws2.Name = "Corrected Part or Process Pics.";
                            ExcelWorksheet ws3 = ew.Worksheets[3]; ws3.Name = "Completed CAPA";


                            #region Section - One Elements


                            if (!string.IsNullOrEmpty(capa.CorrectiveActionInitiatedBy))
                            {
                                MES.Business.Repositories.UserManagement.IUserManagementRepository userObj = new UserManagement.UserManagement();
                                MES.DTO.Library.Identity.LoginUser info = userObj.FindById(capa.CorrectiveActionInitiatedBy).Result;
                                /*Corrective Actions Issuer - A4
                                 *Dan Promen:Issuer Name should populate from RMA Initiated by field in portal*/
                                ws[4, 1].Value = info.FullName;
                                ws[4, 2].Value = "001-740-201-8112";//Phone - B4
                                /*E-Mail - C4
                                 *Dan Promen:email address should populate from email listed in User list.*/

                                ws[4, 3].Value = info.Email;
                            }

                            //CAPA Type: E6, F6, G6
                            if (!string.IsNullOrEmpty(capa.ReviewCtrlPlan))
                            {
                                switch (capa.ReviewCtrlPlan)
                                {
                                    case Constants.CATEXT:
                                        ws[6, 5].Style.Fill.SolidFillOptions.BackColor = System.Drawing.Color.GreenYellow; //Corrective Action

                                        break;
                                    case Constants.PATEXT:
                                        ws[6, 6].Style.Fill.SolidFillOptions.BackColor = System.Drawing.Color.GreenYellow; //Preventive Action

                                        break;
                                    case Constants.CITEXT:
                                        ws[6, 7].Style.Fill.SolidFillOptions.BackColor = System.Drawing.Color.GreenYellow; //Continuous Improvement

                                        break;
                                    default:
                                        ws[6, 5].Style.Fill.SolidFillOptions.BackColor = System.Drawing.Color.GreenYellow; //Corrective Action

                                        break;
                                }
                            }

                            ws[5, 9].Value = capa.ReviewFMEA;//Review FMEA I5

                            /*Supplier Name - D3
                             *Should be linked to Defect Portal 4. Other page Supplier Contact field. Can supplier contact be linked to contacts in SAP?
                             */
                            supplierCode = capa.SupplierCode;
                            supplierName = supplierCode + "-" + capa.SupplierName;

                            ws[3, 4].Value = supplierName;
                            ISuppliersRepository SuppliersRepository = new MES.Business.Library.BO.RFQ.Supplier.Suppliers();
                            MES.DTO.Library.RFQ.Supplier.Suppliers sItem = SuppliersRepository.FindByCode(supplierCode).Result;

                            if (sItem != null)
                            {
                                /*Supplier Contact - A8
                                 *editable field to add name of China Supplier Quality Engineer 
                                 */
                                ws[8, 1].Value = sItem.SQName;

                                /*Phone - B8
                                 *make field editable  
                                 */
                                ws[8, 2].Value = "";

                                /*Email - C8
                                 *editable field
                                 */
                                ws[8, 3].Value = sItem.SQEmail;

                                /*Supplier Contact - A10
                                 *Should be linked to Defect Portal 4. Other page Supplier Contact field. Can supplier contact be linked to contacts in SAP?
                                 */
                                ws[10, 1].Value = sItem.SCName;

                                /*Phone - B10
                                 *Can this field read from contact information in SAP? If not, Manual Entry 
                                 */
                                ws[10, 2].Value = sItem.SCPhone;

                                /*Email - C10
                                 *Can this field read from SAP contact list? If not Manual Entry.
                                 */
                                ws[10, 3].Value = sItem.SCEmail;
                            }
                            else { ws[10, 1].Value = capa.SupplierContactName; }


                            /*Corrective Actions Issuer - F2
                             *Dan Promen:date field should automatically populate as the date form is created. This should link to the RMA Date in defect tracking portal.*/
                            ws[2, 6].Value = capa.CorrectiveActionInitiatedDate;

                            /*Due Date: - H2
                             *Dan Promen: Field should auto populate from Open Date + 30 calendar days as default. Example:Open Date 14-JAN-16 Due Date - 13-FEB-16*/
                            if (capa.CorrectiveActionInitiatedDate.HasValue)
                                ws[2, 8].Value = capa.CorrectiveActionInitiatedDate.Value.AddDays(30);

                            /*CAPA# - E3
                             *Dan Promen:CAPA # field needs to auto populate with year and sequential number. It should be the RMA # from Defect Portal.
                             *Example: 2016-0001, 2016-0002 etc.
                             *In case of Multiple line items in Defect Tracking Record - add and L# (For line item on Defect tracking record) onto the CAPA.  For example, CAPA # 2016-0002-L1.  
                             *If this is too complicated, just keep the same CAPA number for all line items that generate a CAPA.*/
                            ws[3, 6].Value = capa.Id;

                            /*Quality - D4 | Delivery - E4 | Other - F4
                             *Dan Promen:Quality, Delivery, Other need to have active check boxes included in the field.  Default should be based on what is elected in Defect tracking header (Defect Type field).*/
                            //TODO

                            //
                            //
                            //
                            #endregion

                            #region Section - One - Part Details
                            /*Parts details*/
                            int rowIndex = 13;
                            List<MES.DTO.Library.APQP.CAPA.capaPartAffectedDetail> lstCAParts = capa.lstcapaPartAffectedDetails.ToList();

                            if (lstCAParts.Count > 0)
                            {
                                foreach (MES.DTO.Library.APQP.CAPA.capaPartAffectedDetail item in lstCAParts)
                                {
                                    /*Part Name and Part #'s Pull in part number listed for that Line item and pull in Part Description from SAP.*/
                                    ws[rowIndex, 1].Value = item.PartCode;
                                    ws[rowIndex, 2].Value = item.PartName;

                                    /*pull from Defect Description field in page 1 for the line item CAPA is being generated for.*/
                                    if (item.DefectTypeId.HasValue && item.DefectTypeId.Value > 0)
                                    {
                                        MES.Business.Repositories.Setup.DefectType.IDefectTypeRepository dtRep = new BO.Setup.DefectType.DefectType();
                                        DTO.Library.Setup.DefectType.DefectType dtItem = dtRep.FindById(item.DefectTypeId.Value).Result;
                                        ws[rowIndex, 3].Value = dtItem.defectType;
                                    }

                                    /*Initial Parts quantity NO GOOD @ Customer? - M8
                                     *Page 1 customer sort / rework page link to Initial Rejects at Customer field. Auto Populate
                                     */
                                    ws[rowIndex, 4].Value = item.CustomerRejectedPartQty.HasValue ? item.CustomerRejectedPartQty.Value : 0;
                                    ws[rowIndex, 5].Value = item.SupplierRejectedPartQty.HasValue ? item.SupplierRejectedPartQty.Value : 0;
                                    ws[rowIndex, 7].Value = item.PartsDeliveryLateQty.HasValue ? item.PartsDeliveryLateQty.Value : 0;

                                    rowIndex++;
                                    ws.InsertRow(rowIndex, true, true);
                                }

                                ws.DeleteRow(rowIndex);
                            }
                            #endregion

                            #region Section - Two - Detailed description of problem (date shipped, part numbers)
                            //18
                            rowIndex = rowIndex + 1;
                            List<MES.DTO.Library.APQP.CAPA.capaProblemDescription> lstCAProbDesc = capa.lstcapaProblemDescriptions.ToList();

                            if (lstCAProbDesc.Count > 0)
                            {
                                foreach (MES.DTO.Library.APQP.CAPA.capaProblemDescription item in lstCAProbDesc.OrderBy(item => item.QueryId))
                                {
                                    DTO.Library.APQP.CAPA.capaQuery queryItem = QueryFindById(item.QueryId);

                                    // ws[(rowIndex + item.QueryId), 1].Value = queryItem.Value;
                                    ws[(rowIndex + item.QueryId), 2].Value = item.Reason;
                                }
                            }
                            #endregion

                            #region Section - Three - Temporary Countermeasure:
                            //25
                            int savedForAdjacentSection = rowIndex;
                            rowIndex = rowIndex + 8;
                            List<MES.DTO.Library.APQP.CAPA.capaTempCountermeasure> lstCATempCountermeasure = capa.lstcapaTempCountermeasures.ToList();
                            int i = 1;
                            if (lstCATempCountermeasure.Count > 0)
                            {
                                foreach (MES.DTO.Library.APQP.CAPA.capaTempCountermeasure item in lstCATempCountermeasure.Take(7))
                                {
                                    ws[rowIndex, 1].Value = i;
                                    ws[rowIndex, 2].Value = item.Description;
                                    ws[rowIndex, 5].Value = item.EffectiveDate;
                                    rowIndex++;
                                    i++;

                                    //ws.InsertRow(rowIndex, true, true);
                                }
                            }
                            int lastUsedRowPrevSection = savedForAdjacentSection + 7;

                            #endregion

                            #region Section - Four - Verification: (Action):

                            rowIndex = savedForAdjacentSection;
                            rowIndex = rowIndex + 8;
                            List<MES.DTO.Library.APQP.CAPA.capaVerification> lstCAVerfifications = capa.lstcapaVerifications.ToList();

                            #region
                            /*bool requiresRowInsert = false;
                            if (lstCAVerfifications.Count > 0)
                            {
                                if (lstCAProbDesc.Count > 0)
                                {
                                    if (lstCAProbDesc.Count > lstCAVerfifications.Count)
                                    {
                                        requiresRowInsert = false;
                                    }
                                    else
                                    {
                                        if (lstCAVerfifications.Count > 7)
                                            requiresRowInsert = true;
                                    }

                                    if (requiresRowInsert)
                                    {

                                        foreach (MES.DTO.Library.APQP.CAPA.capaVerification item in lstCAVerfifications)
                                        {
                                            ws[rowIndex, 6].Value = i;
                                            ws[rowIndex, 7].Value = item.Description;
                                            ws[rowIndex, 8].Value = item.VerificationDate;
                                            i++;
                                            rowIndex++;
                                            if (rowIndex == lastUsedRowPrevSection + 1)
                                            {
                                                ws.InsertRow(rowIndex, true, true);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (MES.DTO.Library.APQP.CAPA.capaVerification item in lstCAVerfifications)
                                        {
                                            ws[rowIndex, 6].Value = i;
                                            ws[rowIndex, 7].Value = item.Description;
                                            ws[rowIndex, 8].Value = item.VerificationDate;
                                            i++;
                                            rowIndex++;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (MES.DTO.Library.APQP.CAPA.capaVerification item in lstCAVerfifications)
                                    {
                                        ws[rowIndex, 6].Value = i;
                                        ws[rowIndex, 7].Value = item.Description;
                                        ws[rowIndex, 8].Value = item.VerificationDate;
                                        i++;
                                        rowIndex++;
                                        ws.InsertRow(rowIndex, true, true);
                                    }
                                }
                            }*/
                            #endregion
                            if (lstCAVerfifications.Count > 0)
                            {
                                i = 1;
                                foreach (MES.DTO.Library.APQP.CAPA.capaVerification item in lstCAVerfifications.Take(5))
                                {
                                    ws[rowIndex, 6].Value = i;
                                    ws[rowIndex, 7].Value = item.Description;
                                    ws[rowIndex, 8].Value = item.VerificationDate;

                                    rowIndex++;
                                    i++;
                                    //ws.InsertRow(rowIndex, true, true);
                                }
                            }
                            #endregion
                            lastUsedRowPrevSection = lastUsedRowPrevSection + 1 + 7;
                            #region Section - Five - Root Cause- Why Made?
                            //33
                            rowIndex = lastUsedRowPrevSection + 1;
                            List<MES.DTO.Library.APQP.CAPA.capaRootCauseWhyMade> lstCARootCauseWhyMade = capa.lstcapaRootCauseWhyMade.ToList();
                            if (lstCARootCauseWhyMade.Count > 0)
                            {
                                foreach (MES.DTO.Library.APQP.CAPA.capaRootCauseWhyMade item in lstCARootCauseWhyMade.Take(5))
                                {
                                    DTO.Library.APQP.CAPA.capaQuery queryItem = QueryFindById(item.QueryId);
                                    ws[rowIndex, 2].Value = item.Reason;

                                    rowIndex++;
                                    //ws.InsertRow(rowIndex, true, true);
                                }
                            }
                            lastUsedRowPrevSection = lastUsedRowPrevSection + 1 + 5;

                            #endregion

                            #region Section - Six - Root Cause- Why shipped?
                            //39
                            rowIndex = lastUsedRowPrevSection + 1;
                            List<MES.DTO.Library.APQP.CAPA.capaRootCauseWhyShipped> lstCARootCauseWhyShipped = capa.lstcapaRootCauseWhyShipped.ToList();

                            if (lstCARootCauseWhyShipped.Count > 0)
                            {
                                foreach (MES.DTO.Library.APQP.CAPA.capaRootCauseWhyShipped item in lstCARootCauseWhyShipped.Take(5))
                                {
                                    DTO.Library.APQP.CAPA.capaQuery queryItem = QueryFindById(item.QueryId);
                                    ws[rowIndex, 2].Value = item.Reason;

                                    rowIndex++;
                                    //ws.InsertRow(rowIndex, true, true);
                                }
                            }
                            lastUsedRowPrevSection = lastUsedRowPrevSection + 1 + 5;

                            #endregion

                            #region Section - Seven - Permanent Countermeasure
                            //45
                            rowIndex = lastUsedRowPrevSection + 1; savedForAdjacentSection = rowIndex;
                            List<MES.DTO.Library.APQP.CAPA.capaPermanentCountermeasure> lstCAPermanentCountermeasure = capa.lstcapaPermanentCountermeasures.ToList();

                            if (lstCAPermanentCountermeasure.Count > 0)
                            {
                                i = 1;
                                foreach (MES.DTO.Library.APQP.CAPA.capaPermanentCountermeasure item in lstCAPermanentCountermeasure.Take(5))
                                {
                                    ws[rowIndex, 1].Value = i;
                                    ws[rowIndex, 2].Value = item.Description;
                                    ws[rowIndex, 3].Value = item.EffectiveDate;

                                    rowIndex = rowIndex + 2;
                                    i++;
                                    //ws.InsertRow(rowIndex, true, true);
                                }
                            }

                            #endregion

                            #region Section - Eight - Feed forward

                            rowIndex = savedForAdjacentSection;
                            List<MES.DTO.Library.APQP.CAPA.capaFeedForward> lstCAFeedForward = capa.lstcapaFeedForwards.ToList();

                            if (lstCAFeedForward.Count > 0)
                            {
                                i = 1;
                                foreach (MES.DTO.Library.APQP.CAPA.capaFeedForward item in lstCAFeedForward.Take(5))
                                {
                                    ws[rowIndex, 6].Value = i;
                                    ws[rowIndex, 7].Value = item.Description;

                                    rowIndex++;
                                    i++;
                                    //ws.InsertRow(rowIndex, true, true);
                                }
                            }
                            lastUsedRowPrevSection = lastUsedRowPrevSection + 1 + 5;


                            #endregion

                            #region Section - Nine - Approvals

                            rowIndex = lastUsedRowPrevSection + 1;
                            List<MES.DTO.Library.APQP.CAPA.capaApproval> lstCAApproval = capa.lstcapaApprovals.ToList();

                            if (lstCAApproval.Count > 0)
                            {
                                foreach (MES.DTO.Library.APQP.CAPA.capaApproval item in lstCAApproval.Take(2))
                                {
                                    ws[rowIndex, 7].Value = item.Name;
                                    ws[rowIndex, 9].Value = item.ApprovalDate;

                                    rowIndex = rowIndex + 2;

                                    //ws.InsertRow(rowIndex, true, true);
                                }
                            }

                            #endregion

                            #region Add Defective Parts Pictures & CA Verification Pictures

                            if (!System.IO.Directory.Exists(context.Server.MapPath("~") + Constants.DTRMAFormDocumentPhyFolder))
                                System.IO.Directory.CreateDirectory(context.Server.MapPath("~") + Constants.DTRMAFormDocumentPhyFolder);
                            else
                            {
                                try
                                {
                                    System.IO.DirectoryInfo di = new DirectoryInfo(context.Server.MapPath("~") + Constants.DTRMAFormDocumentPhyFolder);
                                    foreach (FileInfo file in di.GetFiles())
                                    {
                                        file.Delete();
                                    }
                                }
                                catch { }
                            }
                            bool isFirst = true;
                            int leftColumnIndex = 0, topRowIndex = 0, rightColumnIndex = 0, bottomRowIndex = 0;
                            ExcelPicture ePicture = null;

                            string fileTemp = string.Empty;
                            HashSet<string> imageExtensions; string extension = string.Empty; bool isImage;
                            string sectionName = "DTSTEP1";
                            List<MES.DTO.Library.APQP.CAPA.capaPartDocument> lstDocuments = null;
                            foreach (MES.DTO.Library.APQP.CAPA.capaPartAffectedDetail DTDitem in lstCAParts)
                            {
                                lstDocuments = GetPartDocumentList(DTDitem.Id, sectionName).Result;

                                if (lstDocuments.Count > 0)
                                    lstDocuments = lstDocuments.Where(item => item.DocumentTypeId != 45).ToList();

                                if (lstDocuments.Count > 0)
                                    foreach (MES.DTO.Library.APQP.CAPA.capaPartDocument partDocument in lstDocuments)
                                    {
                                        if (partDocument.AssociatedToId.HasValue && partDocument.AssociatedToId.Value == (int)MES.Business.Library.Enums.DocTypeAssociatedTo.DefectTrackingPartDocument)
                                        {
                                            imageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                                            imageExtensions.Add(".jpg"); imageExtensions.Add(".jpeg");
                                            imageExtensions.Add(".bmp"); imageExtensions.Add(".png");
                                            imageExtensions.Add(".tiff"); imageExtensions.Add(".gif");

                                            extension = Path.GetExtension(partDocument.FilePath);
                                            isImage = imageExtensions.Contains(extension);
                                            if (isImage)
                                            {
                                                if (isFirst)
                                                    topRowIndex = ws1.UsedRangeRows.Count + 2;
                                                else
                                                    topRowIndex += 2;
                                                byte[] fileBytes = Helper.BlobHelper.GetBlobStreamByUrl(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                                    , partDocument.FilePath);
                                                fileTemp = context.Server.MapPath("~") + Constants.DTRMAFormDocumentPhyFolder + Guid.NewGuid();// Path.GetFileName(partDocument.FilePath);

                                                try
                                                {
                                                    using (var fileStream = new MemoryStream(fileBytes))
                                                    {
                                                        fileStream.Seek(0, SeekOrigin.Begin);
                                                        File.WriteAllBytes(fileTemp, fileBytes);

                                                        ePicture = ws1.Pictures.AddPicture(1, topRowIndex, fileTemp);
                                                    }

                                                    leftColumnIndex = ePicture.LeftColumnIndex;
                                                    topRowIndex = ePicture.TopRowIndex;
                                                    rightColumnIndex = ePicture.RightColumnIndex;
                                                    bottomRowIndex = ePicture.BottomRowIndex;

                                                    ws1[topRowIndex, 1].RowHeightInPoints = 24.75 * (bottomRowIndex - topRowIndex);
                                                    isFirst = false;
                                                }
                                                catch (Exception ex)
                                                {
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                if (isFirst)
                                                    topRowIndex = ws1.UsedRangeRows.Count + 2;
                                                else
                                                    topRowIndex += 2;

                                                fileTemp = context.Server.MapPath(Constants.IMAGEFOLDER);// Path.GetFileName(partDocument.FilePath);

                                                extension = Path.GetExtension(partDocument.FilePath);
                                                ExcelRange er = ws[topRowIndex, 1, topRowIndex, 1];
                                                switch (extension)
                                                {
                                                    case ".doc":
                                                    case ".docx":
                                                        ePicture = ws1.Pictures.AddPicture(1, topRowIndex, fileTemp + "doc.png");

                                                        break;
                                                    case ".xls":
                                                    case ".xlsx":
                                                        ePicture = ws1.Pictures.AddPicture(1, topRowIndex, fileTemp + "xl.png");

                                                        break;
                                                    case ".pdf":
                                                        ePicture = ws1.Pictures.AddPicture(1, topRowIndex, fileTemp + "pdf.png");

                                                        break;
                                                    default:
                                                        ePicture = ws1.Pictures.AddPicture(1, topRowIndex, fileTemp + "file.png");

                                                        break;
                                                }

                                                ExcelHyperlink hyperlink = ws1.Hyperlinks.AddHyperlink(ExcelHyperlinkType.Url, er, partDocument.FilePath);
                                                hyperlink.Text = "Open File";
                                                leftColumnIndex = ePicture.LeftColumnIndex;
                                                topRowIndex = ePicture.TopRowIndex;
                                                rightColumnIndex = ePicture.RightColumnIndex;
                                                bottomRowIndex = ePicture.BottomRowIndex;

                                                ws1[topRowIndex, 1].RowHeightInPoints = 24.75 * (bottomRowIndex - topRowIndex);

                                                isFirst = false;
                                            }
                                        }
                                    }
                            }

                            isFirst = true;
                            lstDocuments = null;
                            sectionName = "DTSTEP2";
                            foreach (MES.DTO.Library.APQP.CAPA.capaPartAffectedDetail DTDitem in lstCAParts)
                            {
                                lstDocuments = GetPartDocumentList(DTDitem.Id, sectionName).Result;
                                if (lstDocuments.Count > 0)
                                    lstDocuments = lstDocuments.Where(item => item.DocumentTypeId != 45).ToList();

                                if (lstDocuments.Count > 0)
                                    foreach (MES.DTO.Library.APQP.CAPA.capaPartDocument partDocument in lstDocuments)
                                    {
                                        if (partDocument.AssociatedToId.HasValue && partDocument.AssociatedToId.Value == (int)MES.Business.Library.Enums.DocTypeAssociatedTo.DefectTrackingCorrectiveAction)
                                        {
                                            imageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                                            imageExtensions.Add(".jpg"); imageExtensions.Add(".jpeg");
                                            imageExtensions.Add(".bmp"); imageExtensions.Add(".png");
                                            imageExtensions.Add(".tiff"); imageExtensions.Add(".gif");

                                            extension = Path.GetExtension(partDocument.FilePath);
                                            isImage = imageExtensions.Contains(extension);
                                            if (isImage)
                                            {
                                                if (isFirst)
                                                    topRowIndex = ws2.UsedRangeRows.Count + 2;
                                                else
                                                    topRowIndex += 2;

                                                byte[] fileBytes = Helper.BlobHelper.GetBlobStreamByUrl(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                                 , partDocument.FilePath);
                                                fileTemp = context.Server.MapPath("~") + Constants.DTRMAFormDocumentPhyFolder + Guid.NewGuid();// Path.GetFileName(partDocument.FilePath);
                                                try
                                                {
                                                    using (var fileStream = new MemoryStream(fileBytes))
                                                    {
                                                        fileStream.Seek(0, SeekOrigin.Begin);
                                                        File.WriteAllBytes(fileTemp, fileBytes);

                                                        ePicture = ws2.Pictures.AddPicture(1, topRowIndex, fileTemp);
                                                    }
                                                    leftColumnIndex = ePicture.LeftColumnIndex;
                                                    topRowIndex = ePicture.TopRowIndex;
                                                    rightColumnIndex = ePicture.RightColumnIndex;
                                                    bottomRowIndex = ePicture.BottomRowIndex;

                                                    ws2[topRowIndex, 1].RowHeightInPoints = 24.75 * (bottomRowIndex - topRowIndex);
                                                    isFirst = false;
                                                }
                                                catch (Exception ex)
                                                {
                                                    continue;
                                                }

                                            }
                                            else
                                            {
                                                if (isFirst)
                                                    topRowIndex = ws2.UsedRangeRows.Count + 2;
                                                else
                                                    topRowIndex += 2;

                                                fileTemp = context.Server.MapPath(Constants.IMAGEFOLDER);// Path.GetFileName(partDocument.FilePath);

                                                extension = Path.GetExtension(partDocument.FilePath);
                                                ExcelRange er = ws[topRowIndex, 1, topRowIndex, 1];
                                                switch (extension)
                                                {
                                                    case ".doc":
                                                    case ".docx":
                                                        ePicture = ws2.Pictures.AddPicture(1, topRowIndex, fileTemp + "doc.png");

                                                        break;
                                                    case ".xls":
                                                    case ".xlsx":
                                                        ePicture = ws2.Pictures.AddPicture(1, topRowIndex, fileTemp + "xl.png");

                                                        break;
                                                    case ".pdf":
                                                        ePicture = ws2.Pictures.AddPicture(1, topRowIndex, fileTemp + "pdf.png");

                                                        break;
                                                    default:
                                                        ePicture = ws2.Pictures.AddPicture(1, topRowIndex, fileTemp + "file.png");

                                                        break;
                                                }

                                                ExcelHyperlink hyperlink = ws2.Hyperlinks.AddHyperlink(ExcelHyperlinkType.Url, er, partDocument.FilePath);
                                                hyperlink.Text = "Open File";
                                                leftColumnIndex = ePicture.LeftColumnIndex;
                                                topRowIndex = ePicture.TopRowIndex;
                                                rightColumnIndex = ePicture.RightColumnIndex;
                                                bottomRowIndex = ePicture.BottomRowIndex;

                                                ws2[topRowIndex, 1].RowHeightInPoints = 24.75 * (bottomRowIndex - topRowIndex);

                                                isFirst = false;
                                            }
                                        }
                                    }
                            }
                            isFirst = true;
                            lstDocuments = null;
                            sectionName = "DTSTEP3";
                            foreach (MES.DTO.Library.APQP.CAPA.capaPartAffectedDetail DTDitem in lstCAParts)
                            {
                                lstDocuments = GetPartDocumentList(DTDitem.Id, sectionName).Result;
                                if (lstDocuments.Count > 0)
                                    lstDocuments = lstDocuments.Where(item => item.DocumentTypeId == 45).ToList();

                                if (lstDocuments.Count > 0)
                                    foreach (MES.DTO.Library.APQP.CAPA.capaPartDocument partDocument in lstDocuments)
                                    {
                                        if (partDocument.AssociatedToId.HasValue &&
                                            (partDocument.AssociatedToId.Value == (int)MES.Business.Library.Enums.DocTypeAssociatedTo.DefectTrackingCorrectiveAction
                                            || partDocument.AssociatedToId.Value == (int)MES.Business.Library.Enums.DocTypeAssociatedTo.DefectTrackingPartDocument))
                                        {
                                            imageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                                            imageExtensions.Add(".jpg"); imageExtensions.Add(".jpeg");
                                            imageExtensions.Add(".bmp"); imageExtensions.Add(".png");
                                            imageExtensions.Add(".tiff"); imageExtensions.Add(".gif");

                                            extension = Path.GetExtension(partDocument.FilePath);
                                            isImage = imageExtensions.Contains(extension);
                                            if (isImage)
                                            {
                                                if (isFirst)
                                                    topRowIndex = ws3.UsedRangeRows.Count + 2;
                                                else
                                                    topRowIndex += 2;

                                                byte[] fileBytes = Helper.BlobHelper.GetBlobStreamByUrl(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                                 , partDocument.FilePath);
                                                fileTemp = context.Server.MapPath("~") + Constants.DTRMAFormDocumentPhyFolder + Guid.NewGuid();// Path.GetFileName(partDocument.FilePath);
                                                try
                                                {
                                                    using (var fileStream = new MemoryStream(fileBytes))
                                                    {
                                                        fileStream.Seek(0, SeekOrigin.Begin);
                                                        File.WriteAllBytes(fileTemp, fileBytes);

                                                        ePicture = ws3.Pictures.AddPicture(1, topRowIndex, fileTemp);
                                                    }
                                                    leftColumnIndex = ePicture.LeftColumnIndex;
                                                    topRowIndex = ePicture.TopRowIndex;
                                                    rightColumnIndex = ePicture.RightColumnIndex;
                                                    bottomRowIndex = ePicture.BottomRowIndex;

                                                    ws3[topRowIndex, 1].RowHeightInPoints = 24.75 * (bottomRowIndex - topRowIndex);
                                                    isFirst = false;
                                                }
                                                catch (Exception ex)
                                                {
                                                    continue;
                                                }

                                            }
                                            else
                                            {
                                                if (isFirst)
                                                    topRowIndex = ws3.UsedRangeRows.Count + 2;
                                                else
                                                    topRowIndex += 2;

                                                fileTemp = context.Server.MapPath(Constants.IMAGEFOLDER);// Path.GetFileName(partDocument.FilePath);

                                                extension = Path.GetExtension(partDocument.FilePath);
                                                ExcelRange er = ws[topRowIndex, 1, topRowIndex, 1];
                                                switch (extension)
                                                {
                                                    case ".doc":
                                                    case ".docx":
                                                        ePicture = ws3.Pictures.AddPicture(1, topRowIndex, fileTemp + "doc.png");

                                                        break;
                                                    case ".xls":
                                                    case ".xlsx":
                                                        ePicture = ws3.Pictures.AddPicture(1, topRowIndex, fileTemp + "xl.png");

                                                        break;
                                                    case ".pdf":
                                                        ePicture = ws3.Pictures.AddPicture(1, topRowIndex, fileTemp + "pdf.png");

                                                        break;
                                                    default:
                                                        ePicture = ws3.Pictures.AddPicture(1, topRowIndex, fileTemp + "file.png");

                                                        break;
                                                }

                                                ExcelHyperlink hyperlink = ws3.Hyperlinks.AddHyperlink(ExcelHyperlinkType.Url, er, partDocument.FilePath);
                                                hyperlink.Text = "Open File";
                                                leftColumnIndex = ePicture.LeftColumnIndex;
                                                topRowIndex = ePicture.TopRowIndex;
                                                rightColumnIndex = ePicture.RightColumnIndex;
                                                bottomRowIndex = ePicture.BottomRowIndex;

                                                ws3[topRowIndex, 1].RowHeightInPoints = 24.75 * (bottomRowIndex - topRowIndex);

                                                isFirst = false;
                                            }
                                        }
                                    }
                            }
                            #endregion

                            string generatedFileName = "CAPA-" + capa.Id + ".xls";
                            try
                            {

                                byte[] fileBytes = ew.Save();
                                string dtFilePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                                    + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + Constants.TEMPFILESEMAILATTACHMENTSFOLDER + generatedFileName;

                                Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), dtFilePath);

                                filePath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                              , Constants.TEMPFILESEMAILATTACHMENTSFOLDERName
                                              , generatedFileName
                                              , fileBytes);
                                if (sItem != null && !string.IsNullOrEmpty(sItem.SQEmail))
                                {
                                    //SendEmailToSQ(sItem, filePath, capa.Id);
                                }

                            }
                            catch (Exception ex) //Error
                            {
                                return FailedBoolResponse(ex.Message);
                            }
                            finally
                            {
                                sourceXlsDataStream.Flush();
                                sourceXlsDataStream.Close();

                                ew.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return SuccessBoolResponse(filePath);
        }

        public NPE.Core.ITypedResponse<List<MES.DTO.Library.APQP.CAPA.capaPartDocument>> GetPartDocumentList(int cAPAPartAffectedDetailId, string SectionName)
        {
            string errMSg = string.Empty;
            List<DTO.Library.APQP.CAPA.capaPartDocument> lstDocument = new List<DTO.Library.APQP.CAPA.capaPartDocument>();
            IPartDocumentRepository objIPartDocumentRepository = new MES.Business.Library.BO.APQP.CAPA.capaPartDocument();
            lstDocument = objIPartDocumentRepository.GetPartDocumentList(cAPAPartAffectedDetailId, 0);
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.CAPA.capaPartDocument>>(errMSg, lstDocument);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> DeletePartDocument(int documentId)
        {
            #region Delete Document
            IPartDocumentRepository objIPartDocumentRepository = new MES.Business.Library.BO.APQP.CAPA.capaPartDocument();
            return objIPartDocumentRepository.Delete(documentId);
            #endregion
        }

        public NPE.Core.ITypedResponse<int?> SavePartDocument(DTO.Library.APQP.CAPA.capaPartDocument document)
        {
            #region Save Document
            IPartDocumentRepository objIPartDocumentRepository = new MES.Business.Library.BO.APQP.CAPA.capaPartDocument();
            return objIPartDocumentRepository.Save(document);
            #endregion
        }

        public NPE.Core.ITypedResponse<bool?> SendEmail(MES.DTO.Library.Common.EmailData emailData)
        {
            bool IsSuccess = false;
            try
            {
                #region footer text
                string footer = @"<br /><br /><hr />
                                    <span style='color: #FF0000; font-size: 10px; font-family: Tahoma, Geneva, sans-serif;' align='left'> 
                                        Electronic Privacy Notice & Email Disclaimer:
                                    </span><br />
                                    <span style='color: #626465; font-size: 10px; font-family: Tahoma, Geneva, sans-serif;' align='left'>  
                                        This email, and any attachments, contains information that is, or may be, covered by electronic communications privacy laws,
                                        and is also confidential and proprietary in nature. If you are not the intended recipient, please be advised that you are 
                                        legally prohibited from retaining, using, copying, distributing, or otherwise disclosing this information in any manner.
                                        Instead, please reply to the sender that you have received this communication in error and then immediately delete it. 
                                        MES Inc. is not liable for any damage caused by any virus transmitted by this email. Thank you for your co operation.
                                    </span>";
                #endregion

                MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
                List<Attachment> attachments = new List<Attachment>();
                List<string> lstToAddress = new List<string>();
                List<string> lstCCEmail = new List<string>();

                //create list for the cc emails                                
                foreach (var item in emailData.ToEmailIds.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.ToString()))
                        lstToAddress.Add(item.ToString());
                }
                //create list for the cc emails                
                foreach (var item in emailData.CCEmailIds.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.ToString()))
                        lstCCEmail.Add(item.ToString());
                }

                //set attachments
                //If IsTrue, attach CAPA excel wiith the email
                if (emailData.AttachDocument.HasValue && emailData.AttachDocument.Value)
                {
                    string FileName = "CAPA-" + emailData.objCAPA.Id + ".xls";
                    var objGeneratedCapa = GenerateCAPAForm(emailData.objCAPA);
                    if (objGeneratedCapa.StatusCode == 200 && !string.IsNullOrEmpty(objGeneratedCapa.SuccessMessage))
                    {
                        Stream memoryStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(objGeneratedCapa.SuccessMessage);
                        attachments.Add(new System.Net.Mail.Attachment(memoryStream, FileName));
                    }
                }

                if (emailData.lstEmailDocumentAttachment != null && emailData.lstEmailDocumentAttachment.Count > 0)
                {
                    foreach (var item in emailData.lstEmailDocumentAttachment)
                    {
                        if (!string.IsNullOrEmpty(item.FilePath) && !string.IsNullOrEmpty(item.FileName))
                        {
                            Stream memoryStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(item.FilePath);
                            attachments.Add(new System.Net.Mail.Attachment(memoryStream, item.FileName));
                        }
                    }
                }

                emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailData.EmailBody + footer, out IsSuccess, attachments, lstCCEmail, null);
            }
            catch (Exception ex)
            {
            }
            if (IsSuccess)
                return SuccessBoolResponse(Languages.GetResourceText("SendEmailSuccess"));
            else
                return SuccessBoolResponse(Languages.GetResourceText("SendEmailFail"));
        }

    }
}
