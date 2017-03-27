using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.APQP.APQP
{
    public interface IAPQPRepository : ICrudMethods<MES.DTO.Library.APQP.APQP.APQP, int?, string,
          MES.DTO.Library.APQP.APQP.APQP, int, bool?, int, MES.DTO.Library.APQP.APQP.APQP>
    {
        ITypedResponse<List<MES.DTO.Library.APQP.APQP.APQP>> GetAPQPList(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.SearchCriteria> searchCriteria);
        ITypedResponse<List<MES.DTO.Library.APQP.APQP.SAPItem>> SearchFromSAPRecords(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.SearchCriteria> searchCriteria);
        ITypedResponse<List<MES.DTO.Library.APQP.APQP.SAPItem>> SearchFromAPQPRecords(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.SearchCriteria> searchCriteria);
        ITypedResponse<int?> InsertFromSAPRecords(string ItemIds);
        ITypedResponse<int?> GetFromSAPAndInsertInLocalSAPTable();
        ITypedResponse<MES.DTO.Library.APQP.APQP.PSWItem> getGeneratePSWData(int Id);
        ITypedResponse<bool?> ExportGeneratePSW(DTO.Library.APQP.APQP.PSWItem pswItem);
        ITypedResponse<List<MES.DTO.Library.APQP.APQP.PredefinedDocumentTypes>> getAPQPPredefinedDocumentTypes(string APQPItemIds);
        ITypedResponse<bool?> SavePredefinedDocumentType(string DocumentTypeIds, string APQPItemIds);
        ITypedResponse<MES.DTO.Library.APQP.APQP.SearchDocument> getDocumentsData(int APQPItemId, string SectionName);
        ITypedResponse<int?> SaveDocument(MES.DTO.Library.APQP.APQP.Document document);
        ITypedResponse<int?> SaveShareDocumentFiles(string APQPItemIds, int documentId);
        ITypedResponse<bool?> DeleteDocument(int documentId);
        ITypedResponse<bool?> GetDocumentsAvailabilityByAPQPItemId(int APQPItemId);
        ITypedResponse<List<DTO.Library.APQP.APQP.QuotePartItem>> GetAPQPQuotePartByQuoteIds(string QuoteIds);
        ITypedResponse<int?> InsertAPQPItemQuotePart(string ItemIds);
        ITypedResponse<bool?> PPAPSubmissionSAPDataExport(string APQPItemIds);
        ITypedResponse<bool?> SendDataToSAP(string APQPItemIds);
        ITypedResponse<bool?> APQPTabWiseDataExport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.SearchCriteria> searchCriteria);
        ITypedResponse<bool?> GenerateNPIF(int APQPItemId);
        ITypedResponse<DTO.Library.RFQ.Supplier.Suppliers> getManufacturerDetails(int manufacturerId);
        #region Update Individual APQP Fields
        ITypedResponse<bool?> UpdateIndividualFields(DTO.Library.APQP.APQP.UpdateIndividualFields updateIndividualFields);
        #endregion
        #region send email
        ITypedResponse<MES.DTO.Library.Common.EmailData> getTemplateAndUserIdsData(string APQPItemIds, string SectionName);
        ITypedResponse<bool?> APQPSendEmail(MES.DTO.Library.Common.EmailData emailData);
        #endregion
        #region Save apqp items list
        ITypedResponse<bool?> SaveAPQPItemList(List<DTO.Library.APQP.APQP.APQP> lstAPQP);
        #endregion

        #region get Next Or Previous APQPItemId
        ITypedResponse<int?> getNextOrPreviousAPQPItemId(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.SearchCriteria> searchCriteria);
        #endregion

        /// <summary>
        /// delete apqp item
        /// </summary>
        /// <param name="APQPItemId"></param>
        /// <returns></returns>
        ITypedResponse<bool?> DeleteAPQPItem(int APQPItemId);

        #region get apqp project status report
        ITypedResponse<List<MES.DTO.Library.APQP.APQP.APQPProjectStatusReport>> GetAPQPProjectStatusReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.ReportSearchCriteria> searchCriteria);
        #endregion

        #region get new business awarded report
        ITypedResponse<List<MES.DTO.Library.APQP.APQP.APQPNewBusinessAwardedReport>> GetAPQPNewBusinessAwardedReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.APQPNewBusinessAwardedReportSearchCriteria> searchCriteria);
        #endregion

        #region project status report Export to Excel
        NPE.Core.ITypedResponse<bool?> ExportAPQPProjectStatusReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.ReportSearchCriteria> searchCriteria);
        #endregion

        #region new business awarded report Export to Excel
        NPE.Core.ITypedResponse<bool?> ExportAPQPNewBusinessAwardedReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.APQPNewBusinessAwardedReportSearchCriteria> searchCriteria);
        #endregion

        #region get apqp Defect tracking report
        ITypedResponse<List<MES.DTO.Library.APQP.DefectTracking.DefectTrackingReport>> GetDefectTrackingReport(NPE.Core.IPage<MES.DTO.Library.APQP.DefectTracking.ReportSearchCriteria> searchCriteria);
        #endregion

        #region  Defect Tracking report Export to Excel
        NPE.Core.ITypedResponse<bool?> ExportDefectTrackingReport(NPE.Core.IPage<MES.DTO.Library.APQP.DefectTracking.ReportSearchCriteria> searchCriteria);
        #endregion

        #region get APQP PPPAP Approval Report
        ITypedResponse<List<MES.DTO.Library.APQP.APQP.PPAPApprovalReport>> GetAPQPPPAPApprovalReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.PPAPApprovalReportSearchCriteria> searchCriteria);
        #endregion

        #region APQP PPPAP Approval Report Export to Excel
        NPE.Core.ITypedResponse<bool?> ExportAPQPPPAPApprovalReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.PPAPApprovalReportSearchCriteria> searchCriteria);
        #endregion

        #region get APQP Weekly meeting Report and Export
        ITypedResponse<MES.DTO.Library.APQP.APQP.APQPWeeklyMeetingReportViewModel> GetAPQPWeeklyMeetingReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.APQPWeeklyMeetingReportSearchCriteria> searchCriteria);
        NPE.Core.ITypedResponse<bool?> ExportAPQPWeeklyMeetingReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.APQPWeeklyMeetingReportSearchCriteria> searchCriteria);
        #endregion

        #region Start NPIF Approvals
        NPE.Core.ITypedResponse<DTO.Library.APQP.APQP.apqpNPIFDocuSign> GetPredefinedNPIFRecipients(string apqpItemId);
        NPE.Core.ITypedResponse<int?> SendNPIF(DTO.Library.APQP.APQP.apqpNPIFDocuSign lstAPQP);
        NPE.Core.ITypedResponse<bool?> CheckNPIFDocuSignStatus();
        NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.apqpNPIFDocuSign>> GetNPIFDocuSignList(string apqpItemId);
        #endregion

    }
}
