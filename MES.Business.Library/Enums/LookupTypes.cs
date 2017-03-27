using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Library.Enums
{
    public enum LookupTypes //Add your lookup types here with their Description
    {
        [Description("State")]
        State,
        [Description("Country")]
        Country,
        [Description("Supported Culture")]
        SupportedCulture,
        [Description("ProjectCategories")]
        ProjectCategories,
        [Description("Categories")]
        Categories,
        [Description("AssociatedToItems")]
        AssociatedToItems,
        [Description("Suppliers")]
        Suppliers,
        [Description("Customers")]
        Customers,
        [Description("CustomerContacts")]
        CustomerContacts,
        [Description("Users")]
        Users,
        [Description("Countries")]
        Countries,
        [Description("SupplierQuality")]
        SupplierQuality,
        [Description("Status")]
        Status,
        [Description("CommodityTypes")]
        CommodityTypes,
        [Description("Prefixes")]
        Prefixes,
        [Description("DocumentTypes")]
        DocumentTypes,
        [Description("Genders")]
        Genders,
        [Description("Designations")]
        Designations,
        [Description("SAM")]
        SAM,
        [Description("Destinations")]
        Destinations,
        [Description("Origins")]
        Origins,
        [Description("Forwarders")]
        Forwarders,
        [Description("RFQCoordinators")]
        RFQCoordinators,
        [Description("RFQSources")]
        RFQSources,
        [Description("NonAwardReasons")]
        NonAwardReasons,
        [Description("RFQTypes")]
        RFQTypes,
        [Description("Process")]
        Process,
        [Description("Commodity")]
        Commodity,
        [Description("EmailTemplates")]
        EmailTemplates,
        [Description("RFQSuppliers")]
        RFQSuppliers,
        [Description("MachineDesc")]
        MachineDesc,
        [Description("MachiningDesc")]
        MachiningDesc,
        [Description("MachiningSecOperation")]
        MachiningSecOperation,
        [Description("CoatingTypes")]
        CoatingTypes,
        [Description("CustomerForSupplierQuote")]
        CustomerForSupplierQuote,
        [Description("RFQForSupplierQuote")]
        RFQForSupplierQuote,
        [Description("SupplierForSupplierQuote")]
        SupplierForSupplierQuote,
        [Description("EmailTemplateBody")]
        EmailTemplateBody,
        [Description("QuoteAssumptions")]
        QuoteAssumptions,
        [Description("QuoteMESComments")]
        QuoteMESComments,
        [Description("QuoteStatus")]
        QuoteStatus,
        [Description("AddressType")]
        AddressType,
        [Description("Pagesizes")]
        Pagesizes,
        [Description("DefaultLandingPages")]
        DefaultLandingPages,
        [Description("AssignmentUsers")]
        AssignmentUsers,
        [Description("CurrentUser")]
        CurrentUser,
        [Description("UserWithDesignation")]
        UserWithDesignation,

        [Description("SupplierItems")]
        SupplierItems,
        [Description("SQItems")]
        SQItems,
        [Description("CountryItems")]
        CountryItems,
        [Description("CommodityTypesSALR")]
        CommodityTypesSALR,
        [Description("SCUsers")]
        SCUsers,
        [Description("APQPEngineers")]
        APQPEngineers,
        [Description("APQPStatus")]
        APQPStatus,
        [Description("SupplierWithCode")]
        SupplierWithCode,
        [Description("PPAPSubmissionLevel")]
        PPAPSubmissionLevel,
        [Description("MESWarehouses")]
        MESWarehouses,
        [Description("ProjectStages")]
        ProjectStages,
        [Description("ProjectStagesWithoutCategoryId")]
        ProjectStagesWithoutCategoryId,
        [Description("Roles")]
        Roles,
        [Description("APQPDocumentType")]
        APQPDocumentType,
        [Description("Parts")]
        Parts,
        [Description("crDocumentTypes")]
        crDocumentTypes,
        [Description("SAPItemByCustomer")]
        SAPItemByCustomer,
        [Description("SAPCustomers")]
        SAPCustomers,
        [Description("SAPSuppliers")]
        SAPSuppliers,
        [Description("SupplierContacts")]
        SupplierContacts,
        [Description("QuoteNumbers")]
        QuoteNumbers,
        [Description("SAPCustomersName")]
        SAPCustomersName,
        [Description("SAPSuppliersName")]
        SAPSuppliersName,
        [Description("RMAInitiatedBy")]
        RMAInitiatedBy,
        [Description("DefectType")]
        DefectType,
        [Description("CAPAQuery")]
        CAPAQuery,
        [Description("CAPAApproverTitle")]
        CAPAApproverTitle,
        [Description("SAPCustomersBySupplier")]
        SAPCustomersBySupplier,
        [Description("CAPADocumentType")]
        CAPADocumentType,
        [Description("DTDocumentType")]
        DTDocumentType,
        [Description("SAPSuppliersByCustomer")]
        SAPSuppliersByCustomer,
        [Description("SAPSuppliersByPartCode")]
        SAPSuppliersByPartCode,
        [Description("NPIFDesignations")]
        NPIFDesignations,
        [Description("UserByDesignation")]
        UserByDesignation,
        [Description("EmailByUser")]
        EmailByUser,
        [Description("RFQPriority")]
        RFQPriority,
        [Description("IndustryTypes")]
        IndustryTypes
    }

    public enum AssociatedTo
    {

        Shipment = 1,
        Supplier = 2,
        APQPStep2 = 3,
        ChangeRequest = 5,
        DefectTrackingPartDocument = 6,
        DefectTrackingCorrectiveAction = 7,
        APQPStep1 = 8,
        APQPStep3 = 9,
        APQPStep4 = 10
    }

    public enum DocTypeAssociatedTo
    {
        Shipment = 1,
        Supplier = 2,


        [Description("Change Request Management")]
        ChangeRequest = 5,

        //[Description("Defect Tracking")]
        //DefectTracking = 6,

        [Description("Defect Tracking Part Document")]
        DefectTrackingPartDocument = 6,

        [Description("Defect Tracking Corrective Action")]
        DefectTrackingCorrectiveAction = 7,

        [Description("APQP Step-1")]
        APQP_Step_1 = 8,

        [Description("APQP Step-2")]
        APQP_Step_2 = 3,

        [Description("APQP Step-4")]
        APQP_Step_3 = 9,

        [Description("APQP Step-5")]
        APQP_Step_4 = 10
    }
    public enum StatusAssociatedTo
    {
        APQP = 11,
        ChangeRequest = 12,
        PPAP = 13
    }

    public enum StatusFixedId
    {
        APQPClosed = 8,
        ChangeRequestClosed = 13,
        ChangeRequestInReview = 14,
        APQPCancelled = 15, //20151119 : treat same as APQP Closed
    }

    public enum DocumentTypeFixedId
    {
        Drawing2D = 15,
        Data3D = 16
    }
    //1	    APQP / Quality Engineer
    //4	    Sourcing Manager
    //5	    Account Manager
    //6	    Sales Manager
    //8	    Supply Chain Manager
    //10	Quality Manager
    //11	Finance Manager
    public enum DesignationFixedId
    {
        APQPQualityEngineer = 1,
        SupplierQualityEngineer = 2,
        SourcingManager = 4,
        AccountManager = 5,
        SalesManager = 6,
        SupplyChainCoordinator = 7,
        SupplyChainManager = 8,
        QualityManager = 10,
        FinanceManager = 11,
        RFQCoordinatorEstimator = 17
    }
    public enum RemarksAssociatedTo
    {
        QuoteAssumptions = 14,
        QuoteMESComments = 15,
        RFQRemarks = 16,
        RFQOtherAssumptions = 17
    }

    public enum CommodityCategory
    {
        NonFerrousMetals = 1,
        FerrousMetals = 2,
        OtherCommodities = 3
    }

    public enum DocuSignEnvelopeStatus
    {
        sent,
        inprocess,
        completed,
        voided,
        declined,
        deleted
    }

    public enum Privileges
    {
        None = 1,
        Read = 2,
        Write = 3
    }
    public enum Pages
    {
        LoginPage = 1,
        ForgetPassword = 2,
        UserList = 3,
        AddUser = 4,
        EditUser = 5,
        RoleList = 6,
        AddRole = 7,
        EditRole = 8,
        RFQ = 9,
        AddRfq = 10,
        EditRfq = 11,
        CustomerList = 12,
        AddCustomer = 13,
        EditCustomer = 14,
        AddContact = 15,
        EditContact = 16,
        SupplierList = 17,
        AddSupplier = 18,
        EditSupplier = 19,
        AddSupplierContact = 20,
        EditSupplierContact = 21,
        AddRfqPart = 22,
        EditRfqPart = 23,
        SupplierQuote = 24,
        RfqPartsSupplierVise = 25,
        RfqSupplierPartsQuote = 26,
        Reports = 27,
        Post = 28,
        AddPost = 29,
        EditPost = 30,
        SiteContents = 31,
        AddSiteContent = 32,
        EditSiteContent = 33,
        Newsletters = 36,
        AddNewsletter = 37,
        EditNewsletter = 38,
        RFQM = 63,
        Setup = 64,
        SubmitQuote = 67,
        SendMail = 70,
        ThankYou = 71,
        QuoteList = 72,
        AddQuote = 73,
        EditQuote = 74,
        Destination = 75,
        Forwarder = 76,
        DocumentType = 77,
        Shipment = 78,
        ShipmentList = 81,
        AddShipment = 79,
        EditShipment = 80,
        WorkType = 82,
        Origin = 83,
        UploadShipment = 84,
        ShipmentReports = 85,
        ChangePassword = 86,
        EmailTemplate = 87,
        AddEmailTemplate = 88,
        EditEmailTemplate = 89,
        APQPQuotePart = 91,
        APQPDashboard = 92,
        APQPTracking = 93,//not active
        APQPItemForm = 94,//not active
        EmailGroup = 95,
        Designation = 96,
        Status = 97,
        APQPProjectCategory = 98,
        APQPProjectStage = 99,
        ChangeRequestManagement = 100,
        ChangeRequestForm = 101,
        RFQSource = 102,
        DefectTrackingList = 103,
        DefectTrackingForm = 104,
        APQP = 105,
        ChangeRequest = 106,
        KickOff = 107,
        ToolingLaunch = 108,
        ProcessSetup = 109,
        ProjectTracking = 110,
        PPAPSubmission = 111,
        DefectTracking = 112,
        Remarks = 113,
        RFQType = 114,
        DocumentManagement = 115,
        APQPReports = 116,
        dqMachineDescList = 117,
        dqMachiningDescList = 118,
        dqSecondaryOperationDescList = 119,
        dqCoatingTypeList = 120,
        NonAwardFeedback = 121,
        ProcessList = 122,
        CommodityList = 123,
        AddDivision = 124,
        EditDivision = 125,
        Dashboard = 126,
        BusinessPartners = 127,
        DefectTypeList = 145,
        CAPA = 146,
        CAPAList = 147,
        CAPAForm = 146,
    }
}
