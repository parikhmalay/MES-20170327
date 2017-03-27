using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.APQP.APQP;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.APQP.APQP
{
    [AdminPrefix("APQPApi")]
    public class APQPApiController : SecuredApiControllerBase
    {
        [Inject]
        public IAPQPRepository APQPRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.APQP.APQP.APQP> Get(int Id)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the APQP data.
        /// </summary>
        /// <param name="APQP"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.APQP.APQP.APQP aPQP)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).Save(aPQP);
            return type;
        }

        /// <summary>
        /// delete the APQP data.
        /// </summary>
        /// <param name="APQPId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int aPQPId)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).Delete(aPQPId);
            return type;
        }

        /// <summary>
        /// Get APQP list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetAPQPList")]
        public ITypedResponse<List<MES.DTO.Library.APQP.APQP.APQP>> GetAPQPList(GenericPage<MES.DTO.Library.APQP.APQP.SearchCriteria> paging)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).GetAPQPList(paging);
            return type;
        }
        /// <summary>
        /// Get SAP record list.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPostRoute("SearchFromSAPRecords")]
        public ITypedResponse<List<MES.DTO.Library.APQP.APQP.SAPItem>> SearchFromSAPRecords(GenericPage<MES.DTO.Library.APQP.APQP.SearchCriteria> paging)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).SearchFromSAPRecords(paging);
            return type;
        }
        /// <summary>
        /// Get APQP record list.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPostRoute("SearchFromAPQPRecords")]
        public ITypedResponse<List<MES.DTO.Library.APQP.APQP.SAPItem>> SearchFromAPQPRecords(GenericPage<MES.DTO.Library.APQP.APQP.SearchCriteria> paging)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).SearchFromAPQPRecords(paging);
            return type;
        }
        /// <summary>
        /// InsertFromSAPRecords.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPostRoute("InsertFromSAPRecords")]
        public ITypedResponse<int?> InsertFromSAPRecords(string ItemIds)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).InsertFromSAPRecords(ItemIds);
            return type;
        }

        /// <summary>
        /// Get PSW data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("getGeneratePSWData")]
        public ITypedResponse<MES.DTO.Library.APQP.APQP.PSWItem> getGeneratePSWData(int Id)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).getGeneratePSWData(Id);
            return type;
        }
        [HttpPostRoute("ExportGeneratePSW")]
        public ITypedResponse<bool?> ExportGeneratePSW(DTO.Library.APQP.APQP.PSWItem pswItem)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).ExportGeneratePSW(pswItem);
            return type;
        }

        /// <summary>
        /// get APQPSetDocumentsForItem Data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("getAPQPPredefinedDocumentTypes")]
        public ITypedResponse<List<MES.DTO.Library.APQP.APQP.PredefinedDocumentTypes>> getAPQPPredefinedDocumentTypes(string APQPItemIds)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).getAPQPPredefinedDocumentTypes(APQPItemIds);
            return type;
        }

        /// <summary>
        /// SavePredefinedDocumentType.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPostRoute("SavePredefinedDocumentType")]
        public ITypedResponse<bool?> SavePredefinedDocumentType(string DocumentTypeIds, string APQPItemIds)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).SavePredefinedDocumentType(DocumentTypeIds, APQPItemIds);
            return type;
        }

        /// <summary>
        /// Get PSW data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("getDocumentsData")]
        public ITypedResponse<MES.DTO.Library.APQP.APQP.SearchDocument> getDocumentsData(int APQPItemId, string SectionName)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).getDocumentsData(APQPItemId, SectionName);
            return type;
        }
        /// <summary>
        /// save the document data.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPostRoute("SaveDocument")]
        public ITypedResponse<int?> SaveDocument(MES.DTO.Library.APQP.APQP.Document document)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).SaveDocument(document);
            return type;
        }

        [HttpGetRoute("GetDocumentsAvailabilityByAPQPItemId")]
        public ITypedResponse<bool?> GetDocumentsAvailabilityByAPQPItemId(int APQPItemId)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).GetDocumentsAvailabilityByAPQPItemId(APQPItemId);
            return type;
        }

        /// <summary>
        /// save the document data.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPostRoute("SaveShareDocumentFiles")]
        public ITypedResponse<int?> SaveShareDocumentFiles(string APQPItemIds, int documentId)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).SaveShareDocumentFiles(APQPItemIds, documentId);
            return type;
        }

        /// <summary>
        /// delete the document data.
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpPostRoute("DeleteDocument")]
        public ITypedResponse<bool?> DeleteDocument(int documentId)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).DeleteDocument(documentId);
            return type;
        }
        /// <summary>
        /// Get APQP Quote Part By QuoteIds
        /// </summary>
        /// <param name="QuoteIds"></param>
        /// <returns></returns>
        [HttpGetRoute("GetAPQPQuotePartByQuoteIds")]
        public ITypedResponse<List<MES.DTO.Library.APQP.APQP.QuotePartItem>> GetAPQPQuotePartByQuoteIds(string QuoteIds)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).GetAPQPQuotePartByQuoteIds(QuoteIds);
            return type;
        }

        /// <summary>
        /// InsertAPQPItemQuotePart.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPostRoute("InsertAPQPItemQuotePart")]
        public ITypedResponse<int?> InsertAPQPItemQuotePart(string ItemIds)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).InsertAPQPItemQuotePart(ItemIds);
            return type;
        }

        [HttpPostRoute("PPAPSubmissionSAPDataExport")]
        public ITypedResponse<bool?> PPAPSubmissionSAPDataExport(string APQPItemIds)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).PPAPSubmissionSAPDataExport(APQPItemIds);
            return type;
        }

        [HttpPostRoute("SendDataToSAP")]
        public ITypedResponse<bool?> SendDataToSAP(string APQPItemIds)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).SendDataToSAP(APQPItemIds);
            return type;
        }

        [HttpPostRoute("APQPTabWiseDataExport")]
        public ITypedResponse<bool?> APQPTabWiseDataExport(GenericPage<MES.DTO.Library.APQP.APQP.SearchCriteria> paging)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).APQPTabWiseDataExport(paging);
            return type;
        }

        [HttpPostRoute("GenerateNPIF")]
        public ITypedResponse<bool?> GenerateNPIF(int APQPItemId)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).GenerateNPIF(APQPItemId);
            return type;
        }

        [HttpGetRoute("getManufacturerDetails")]
        public ITypedResponse<DTO.Library.RFQ.Supplier.Suppliers> getManufacturerDetails(int manufacturerId)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).getManufacturerDetails(manufacturerId);
            return type;
        }

        #region Send email
        /// <summary>
        /// get Template And UserIds Data
        /// </summary>
        /// <param name="APQPItemIds"></param>
        /// <returns></returns>
        [HttpGetRoute("getTemplateAndUserIdsData")]
        public ITypedResponse<MES.DTO.Library.Common.EmailData> getTemplateAndUserIdsData(string APQPItemIds, string SectionName)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).getTemplateAndUserIdsData(APQPItemIds, SectionName);
            return type;
        }
        /// <summary>
        /// send email.
        /// </summary>
        /// <param name="EmailData"></param>
        /// <returns></returns>
        [HttpPostRoute("APQPSendEmail")]
        public ITypedResponse<bool?> APQPSendEmail(MES.DTO.Library.Common.EmailData EmailData)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).APQPSendEmail(EmailData);
            return type;
        }
        #endregion
        #region Update Individual APQP Fields
        /// <summary>
        /// Update Individual APQP Fields
        /// </summary>
        /// <param name="updateIndividualFields"></param>
        /// <returns></returns>
        [HttpPostRoute("UpdateIndividualFields")]
        public ITypedResponse<bool?> UpdateIndividualFields(MES.DTO.Library.APQP.APQP.UpdateIndividualFields updateIndividualFields)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).UpdateIndividualFields(updateIndividualFields);
            return type;
        }
        #endregion
        #region Save apqp items list
        /// <summary>
        /// Save apqp items list
        /// </summary>
        /// <param name="updateIndividualFields"></param>
        /// <returns></returns>
        [HttpPostRoute("SaveAPQPItemList")]
        public ITypedResponse<bool?> SaveAPQPItemList(List<DTO.Library.APQP.APQP.APQP> lstAPQP)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).SaveAPQPItemList(lstAPQP);
            return type;
        }
        #endregion

        [HttpGetRoute("GetFromSAPAndInsertInLocalSAPTable")]
        public ITypedResponse<int?> GetFromSAPAndInsertInLocalSAPTable()
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).GetFromSAPAndInsertInLocalSAPTable();
            return type;
        }

        #region get Next Or Previous APQPItemId
        [HttpPostRoute("getNextOrPreviousAPQPItemId")]
        public ITypedResponse<int?> getNextOrPreviousAPQPItemId(GenericPage<MES.DTO.Library.APQP.APQP.SearchCriteria> paging)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).getNextOrPreviousAPQPItemId(paging);
            return type;
        }
        #endregion

        /// <summary>
        /// DeleteAPQPItem.
        /// </summary>
        /// <param name="APQPId"></param>
        /// <returns></returns>
        [HttpPostRoute("DeleteAPQPItem")]
        public ITypedResponse<bool?> DeleteAPQPItem(int APQPItemId)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).DeleteAPQPItem(APQPItemId);
            return type;
        }
        #region NPIF DocuSign
        [HttpGetRoute("getPredefinedNPIFRecipients")]
        public NPE.Core.ITypedResponse<DTO.Library.APQP.APQP.apqpNPIFDocuSign> GetPredefinedNPIFRecipients(string apqpItemId)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).GetPredefinedNPIFRecipients(apqpItemId);
            return type;
        }
        /// <summary>
        /// Save apqp items list
        /// </summary>
        /// <param name="updateIndividualFields"></param>
        /// <returns></returns>
        [HttpPostRoute("SendNPIF")]
        public ITypedResponse<int?> SendNPIF(DTO.Library.APQP.APQP.apqpNPIFDocuSign lstAPQP)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).SendNPIF(lstAPQP);
            return type;
        }
        /// <summary>
        /// update NPIF Docusign status.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("CheckNPIFDocuSignStatus")]
        public NPE.Core.ITypedResponse<bool?> CheckNPIFDocuSignStatus()
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).CheckNPIFDocuSignStatus();
            return type;
        }
        /// <summary>
        /// get npif DocuSign List By apqpItemId Data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPostRoute("GetNPIFDocuSignList")]
        public ITypedResponse<List<MES.DTO.Library.APQP.APQP.apqpNPIFDocuSign>> GetNPIFDocuSignList(string apqpItemId)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).GetNPIFDocuSignList(apqpItemId);
            return type;
        }

        #endregion

        #region get apqp project status report
        [HttpPostRoute("GetAPQPProjectStatusReport")]
        public ITypedResponse<List<MES.DTO.Library.APQP.APQP.APQPProjectStatusReport>> GetAPQPProjectStatusReport(GenericPage<MES.DTO.Library.APQP.APQP.ReportSearchCriteria> paging)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).GetAPQPProjectStatusReport(paging);
            return type;
        }
        #endregion

        #region get apqp new business awarded report
        [HttpPostRoute("GetAPQPNewBusinessAwardedReport")]
        public ITypedResponse<List<MES.DTO.Library.APQP.APQP.APQPNewBusinessAwardedReport>> GetAPQPProjectStatusReport(GenericPage<MES.DTO.Library.APQP.APQP.APQPNewBusinessAwardedReportSearchCriteria> paging)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).GetAPQPNewBusinessAwardedReport(paging);
            return type;
        }
        #endregion

        #region project status report Export to Excel
        [HttpPostRoute("ExportAPQPProjectStatusReport")]
        public NPE.Core.ITypedResponse<bool?> ExportAPQPProjectStatusReport(GenericPage<MES.DTO.Library.APQP.APQP.ReportSearchCriteria> searchCriteria)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).ExportAPQPProjectStatusReport(searchCriteria);
            return type;
        }
        #endregion

        #region get apqp Defect tracking report
        [HttpPostRoute("GetDefectTrackingReport")]
        public ITypedResponse<List<MES.DTO.Library.APQP.DefectTracking.DefectTrackingReport>> GetDefectTrackingReport(GenericPage<MES.DTO.Library.APQP.DefectTracking.ReportSearchCriteria> paging)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).GetDefectTrackingReport(paging);
            return type;
        }
        #endregion

        #region  Defect Tracking report Export to Excel
        [HttpPostRoute("ExportDefectTrackingReport")]
        public NPE.Core.ITypedResponse<bool?> ExportDefectTrackingReport(GenericPage<MES.DTO.Library.APQP.DefectTracking.ReportSearchCriteria> searchCriteria)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).ExportDefectTrackingReport(searchCriteria);
            return type;
        }
        #endregion

        #region get APQP PPPAP Approval Report
        [HttpPostRoute("GetAPQPPPAPApprovalReport")]
        public ITypedResponse<List<MES.DTO.Library.APQP.APQP.PPAPApprovalReport>> GetAPQPPPAPApprovalReport(GenericPage<MES.DTO.Library.APQP.APQP.PPAPApprovalReportSearchCriteria> paging)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).GetAPQPPPAPApprovalReport(paging);
            return type;
        }
        #endregion

        #region APQP PPPAP Approval Report Export to Excel
        [HttpPostRoute("ExportAPQPPPAPApprovalReport")]
        public NPE.Core.ITypedResponse<bool?> ExportAPQPPPAPApprovalReport(GenericPage<MES.DTO.Library.APQP.APQP.PPAPApprovalReportSearchCriteria> searchCriteria)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).ExportAPQPPPAPApprovalReport(searchCriteria);
            return type;
        }
        #endregion

        #region get APQP Weekly meeting Report and Export
        [HttpPostRoute("GetAPQPWeeklyMeetingReport")]
        public ITypedResponse<MES.DTO.Library.APQP.APQP.APQPWeeklyMeetingReportViewModel> GetAPQPWeeklyMeetingReport(GenericPage<MES.DTO.Library.APQP.APQP.APQPWeeklyMeetingReportSearchCriteria> paging)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).GetAPQPWeeklyMeetingReport(paging);
            return type;
        }
        [HttpPostRoute("ExportAPQPWeeklyMeetingReport")]
        public NPE.Core.ITypedResponse<bool?> ExportAPQPWeeklyMeetingReport(GenericPage<MES.DTO.Library.APQP.APQP.APQPWeeklyMeetingReportSearchCriteria> searchCriteria)
        {
            var type = this.Resolve<IAPQPRepository>(APQPRepository).ExportAPQPWeeklyMeetingReport(searchCriteria);
            return type;
        }
        #endregion

        #endregion Methods
    }
}