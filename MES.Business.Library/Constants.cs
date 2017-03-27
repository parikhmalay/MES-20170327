using NPE.Core;
using NPE.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Library
{
    public static class Constants
    {
        public static readonly Dictionary<string, Type> Contexts = new Dictionary<string, Type>()
        {
            {ContextNamesConstants.DataContextName, typeof(MES.Data.Library.MESDataEntities)},
            {ContextNamesConstants.IdentityContextName, typeof(MES.Identity.Data.Library.IdentityContext)}
        };

        public static string DOCUMENTTYPE = "document type";
        public static string REMARKS = "remarks";
        public static string STATUS = "status";
        public static string SUPPLIERSTATUS = "Supplier Status";

        public const string PASSWORDENCRYPTIONMETHOD = "SHA1";

        #region RFQ Constants
        public const string DEFAULTCURRENCY = "INR";
        #endregion
        public const string TEMPFILESEMAILATTACHMENTSFOLDER = "/TempFilesEmailAttachments/";
        public const string TEMPFILESEMAILATTACHMENTSFOLDERName = "TempFilesEmailAttachments/";
        #region RFQ File/Folder Path
        public const string EmailTemplateFolder = @"EmailTemplates\";
        public const string RFQPartAttachmentFolder = @"\Uploads\RFQPartAttachments\";
        public const string RFQAttachmentFolder = @"\Uploads\RFQFiles\";
        public const string RFQSUPPLIERQUOTEFOLDER = @"\Uploads\RFQSupplierQuoteFiles\";

        public const string RFQSUPPLIERQUOTEFILEPATH = @"Uploads/RFQSupplierQuoteFiles/";
        public const string RFQATTACHMENTFILEPATH = @"/Uploads/RFQFiles/";

        public const string REPORTSTEMPLATEFOLDER = @"\Uploads\Reports\Templates\";
        public const string REPORTSTEMPLATEFILEPATH = @"/Uploads/Reports/TempFolder/";
        public const string RFQFILESFOLDER = "/RFQFiles/";
        public const string RFQFILESFOLDERNAME = "RFQFiles/";
        public const string IMAGEFOLDER = @"~\Images\";
        public const string LOGOIMAGEFOLDERINAPI = @"Images/";
        #endregion

        #region
        public const string SHIPMENTTEMPLATE = @"~\Uploads\ShipmentFiles\Template\";
        public const string SHIPMENTPhyFOLDER = @"~\Uploads\ShipmentFiles\TempFolder\";
        public const string SHIPMENTFOLDER = @"Uploads/ShipmentFiles/TempFolder/";

        #endregion

        #region QUOTE File/Folder Path
        public const string QUOTEFILEPATH = @"/QuoteFiles/";
        public const string QUOTEFILEFOLDER = @"QuoteFiles/";
        #endregion
        /*--------- APQP Document File Path Start ----------*/
        public const string KickOffDocumentFolder = @"KickOff/";
        public const string ToolingLaunchDocumentFolder = @"ToolingLaunch/";
        public const string ProjectTrackingDocumentFolder = @"ProjectTracking/";
        public const string PPAPDocumentFolder = @"PpapSubmission\";
        public const string PSWDocumentPhyFolder = @"/Uploads/APQPFile/PSW/";
        public const string NPIFDocumentFolder = @"Uploads/APQPFile/NPIF/";

        public const string PSWDocumentFolder = @"APQPFile/PSW/";
        public const string APQPDocFolder = "APQPFile/APQPTracking";
        public const string APQPTemplateFolder = @"~\Uploads\APQPFile\Template\";
        public const string CRDocumentFolder = @"~\Uploads\APQPFile\CR\";

        public const string DTPartDocumentFolder = @"~\Uploads\APQPFile\DTPartDocument\";
        public const string DTPartMediumSizeImageFolder = @"~\Uploads\APQPFile\DTPartDocument\MediumSizeImage\";
        public const string DTPartMediumSizeImagePhyFolder = @"Uploads/APQPFile/DTPartDocument/MediumSizeImage/";

        public const string DTCorrectiveActionDocumentFolder = @"~\Uploads\APQPFile\DTCorrectiveAction\";
        public const string DTCAMediumSizeImageFolder = @"~\Uploads\APQPFile\DTCorrectiveAction\MediumSizeImage\";
        public const string DTCAMediumSizeImagePhyFolder = @"Uploads/APQPFile/DTCorrectiveAction/MediumSizeImage/";

        public const string DTRMAFormDocumentFolder = @"APQPFile/DTRMAFormDocument/";
        public const string DTRMAFormDocumentPhyFolder = @"Uploads/APQPFile/DTRMAFormDocument/";

        public const string APQPTEMPORARYFILEFOLDER = "APQPTemporaryFileUpload/";
        public const string MESFULLADDRESS = "625 Bear Run Lane,<br />Lewis Center, OH 43035<br />(740) 201-8112, sales@mesinc.net";

        public const string SAPExportXMLFolder = @"~\Uploads\APQPFile\SAPExportXML\";
        /*--------- APQP Document File Path End ----------*/

        /* Trigger Point title for APQP Step*/
        public const string TriggerPointAPQPStep1 = "Trigger Point - APQP Step 1";
        public const string TriggerPointAPQPStep2 = "Trigger Point - APQP Step 2";
        public const string TriggerPointAPQPStep3 = "Trigger Point - APQP Step 3";
        public const string TriggerPointAPQPStep4 = "Trigger Point - APQP Step 4";
        /* end here */

        /*Email Group Short Code for APQP Intimation Email*/
        public const string APQPEmailStep1 = "APQP Step1";
        public const string APQPEmailStep2 = "APQP Step2";
        public const string APQPEmailStep3 = "APQP Step3";
        public const string APQPEmailStep4 = "APQP Step4";
        public const string APQPEmailStep5 = "APQP Step5";
        /* end here*/

        public const string NPIFDOCUSIGNFILEPATH = @"/DocuSignDocs/NPIF/";
        public const string NPIFDOCUSIGNFOLDERNAME = @"DocuSignDocs/NPIF/";
        public const string PPAPDOCUSIGNFILEPATH = @"/DocuSignDocs/PPAP/";
        public const string PPAPDOCUSIGNFOLDERNAME = @"DocuSignDocs/PPAP/";

        public const string PPAPDocusign = "PPAP";
        public const string NPIFDocusign = "NPIF";

        #region CAPA - ReviewCtrlPlan
        public const string CATEXT = "CA";
        public const string PATEXT = "PA";
        public const string CITEXT = "CI";
        #endregion

        #region Privileges
        public const string READPRIVILEGE = "Read";
        public const string WRITEPRIVILEGE = "Write";
        public const string NONEPRIVILEGE = "None";
        #endregion

        #region Other Constants
        public const string ACTIVETEXT = "ACTIVE";
        public const string INACTIVETEXT = "INACTIVE";
        public const string YESTEXT = "YES";
        public const string NOTEXT = "NO";
        public const string DATEFORMAT = "MM/dd/yyyy";
        #endregion

        #region PageControllerActionMapping
        public const string DashboardController = "Dashboard";//126
        public const string RFQController = @"RFQ/RFQ";//63
        public const string ShipmentController = "ShipmentTracking";
        public const string APQPController = "APQP";//92
        public const string QuoteController = "RFQ/Quote";//72
        #endregion
        #region SHORT CODE FOR EMAIL TEMPALTES
        public const string RFQINVITETOSUPPLIER = "RFQInviteToSupplier";
        #endregion
    }
}
