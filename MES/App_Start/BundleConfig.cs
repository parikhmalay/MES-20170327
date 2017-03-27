using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;

namespace MES
{
    public class AsIsBundleOrderer : IBundleOrderer
    {
        public virtual IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862

        public static void RegisterBundles(BundleCollection bundles)
        {
            #region General Script bundle
            var bundleVendorScripts = new Bundle("~/bundles/angularjs");
            bundleVendorScripts.Orderer = new AsIsBundleOrderer();
            bundleVendorScripts.Include(
                "~/Scripts/jquery-2.0.3.min.js",
                "~/Scripts/jquery-ui-1.10.3.min.js",
                "~/Scripts/angular.js",
                "~/Scripts/angular-agility.js",
                "~/Scripts/angular-animate.min.js",
                "~/Scripts/angular-route.min.js",
                "~/Scripts/angular-resource.min.js",
                "~/Scripts/angular-cookies.min.js",
                "~/Scripts/angular-sanitize.min.js",
                "~/Scripts/angular-ui-router.js",
                "~/Scripts/angular-gettext.min.js",
                "~/Scripts/angular-confirm.js",
                "~/Scripts/angular-file-upload.js",
                "~/Scripts/angular-treeview.js",

                "~/Scripts/bootstrap-tagsinput.js",
                "~/Scripts/underscore-1.6.0-min.js",

                "~/App_Client/app.js",
                "~/App_Client/common/Common.js"

                );
            bundles.Add(bundleVendorScripts);

            bundles.Add(new Bundle("~/bundles/CommonScriptsForAllModule").Include(
                "~/App_Client/config.route.js",
                "~/App_Client/common/helper.js",
                "~/App_Client/common/Services/UserAuthService.js",
                "~/App_Client/common/Services/Lookup.service.js",
                "~/App_Client/common/Services/AuditLogs/AuditLogs.service.js",
                "~/App_Client/common/directives/AuditLog/AuditLog.directive.js",
                "~/App_Client/common/directives/loadingoverlay.js",
                "~/App_Client/common/directives/Spinner/angular-spinner.js",
                "~/App_Client/common/directives/common.directives.js",
                "~/Scripts/spin.min.js",
                "~/Scripts/bootstrap-2.3.2.js",
                "~/Scripts/ui-bootstrap-tpls-0.10.0.js",
                "~/App_Client/common/directives/MultiSelect/multiselect.js",
                "~/App_Client/common/directives/bootstrap-tagsinput-angular.js",
                "~/App_Client/common/directives/Pagination/pagination.js",
                "~/App_Client/common/directives/DragDropList.js",
                "~/App_Client/common/directives/StarRating/StarRating.directive.js",
                "~/Scripts/jquery.nicescroll.min.js",
                "~/Scripts/common.ui.js",
                "~/Scripts/jquery.tooltip.js",
                "~/App_Client/common/directives/DateTimePicker/datePicker.js",
                "~/App_Client/common/directives/DateTimePicker/dateRange.js",
                "~/App_Client/common/directives/DateTimePicker/input.js",
                "~/App_Client/common/directives/commonUI.directive.js"
            ));

            #endregion

            #region Destination bundle
            var bundleDestination = new Bundle("~/bundles/Destination");
            bundleDestination.Orderer = new AsIsBundleOrderer();
            bundleDestination.Include(
                "~/App_Client/common/services/Setup/Destination/Destination.Service.js",
                "~/App_Client/views/Setup/Destination/Destination.Controller.js",
                "~/App_Client/views/Setup/Destination/Destination.Route.js",
                "~/App_Client/common/directives/slider.directive.js"
                );
            bundles.Add(bundleDestination);
            #endregion

            #region RFQ Type bundle
            var bundleRFQType = new Bundle("~/bundles/RFQType");
            bundleRFQType.Orderer = new AsIsBundleOrderer();
            bundleRFQType.Include(
                "~/App_Client/common/services/Setup/RFQType/RFQType.services.js",
                "~/App_Client/views/Setup/RFQType/RFQType.controller.js",
                "~/App_Client/views/Setup/RFQType/RFQType.route.js",
                "~/App_Client/common/directives/slider.directive.js"
                );
            bundles.Add(bundleRFQType);
            #endregion

            #region Process bundle
            var bundleProcess = new Bundle("~/bundles/Process");
            bundleProcess.Orderer = new AsIsBundleOrderer();
            bundleProcess.Include(
                "~/App_Client/common/services/Setup/Process/Process.services.js",
                "~/App_Client/views/Setup/Process/Process.controller.js",
                "~/App_Client/views/Setup/Process/Process.route.js"
                );
            bundles.Add(bundleProcess);
            #endregion

            #region Origin bundle
            var bundleOrigin = new Bundle("~/bundles/Origin");
            bundleOrigin.Orderer = new AsIsBundleOrderer();
            bundleOrigin.Include(
                "~/App_Client/common/services/Setup/Origin/Origin.services.js",
                "~/App_Client/views/Setup/Origin/Origin.controller.js",
                "~/App_Client/views/Setup/Origin/Origin.route.js"
                );
            bundles.Add(bundleOrigin);
            #endregion

            #region Remarks bundle
            var bundleRemarks = new Bundle("~/bundles/Remarks");
            bundleRemarks.Orderer = new AsIsBundleOrderer();
            bundleRemarks.Include(
                "~/App_Client/common/services/Setup/Remarks/Remarks.services.js",
                "~/App_Client/views/Setup/Remarks/Remarks.controller.js",
                "~/App_Client/views/Setup/Remarks/Remarks.route.js"
                );
            bundles.Add(bundleRemarks);
            #endregion

            #region Project Category bundle
            var bundleProjectCategory = new Bundle("~/bundles/ProjectCategory");
            bundleProjectCategory.Orderer = new AsIsBundleOrderer();
            bundleProjectCategory.Include(
                "~/App_Client/common/services/Setup/ProjectCategory/ProjectCategory.Service.js",
                "~/App_Client/views/Setup/ProjectCategory/ProjectCategory.Controller.js",
                "~/App_Client/views/Setup/ProjectCategory/ProjectCategory.Route.js"
                );
            bundles.Add(bundleProjectCategory);
            #endregion

            #region Project Stage bundle
            var bundleProjectStage = new Bundle("~/bundles/ProjectStage");
            bundleProjectStage.Orderer = new AsIsBundleOrderer();
            bundleProjectStage.Include(
                "~/App_Client/common/services/Setup/ProjectStage/ProjectStage.Service.js",
                "~/App_Client/views/Setup/ProjectStage/ProjectStage.Controller.js",
                "~/App_Client/views/Setup/ProjectStage/ProjectStage.Route.js"
                );
            bundles.Add(bundleProjectStage);
            #endregion

            #region Coating Type bundle
            var bundleCoatingType = new Bundle("~/bundles/CoatingType");
            bundleCoatingType.Orderer = new AsIsBundleOrderer();
            bundleCoatingType.Include(
                "~/App_Client/common/services/Setup/CoatingType/CoatingType.Service.js",
                "~/App_Client/views/Setup/CoatingType/CoatingType.Controller.js",
                "~/App_Client/views/Setup/CoatingType/CoatingType.Route.js"
                );
            bundles.Add(bundleCoatingType);
            #endregion

            #region Defect Type bundle
            var bundleDefectType = new Bundle("~/bundles/DefectType");
            bundleDefectType.Orderer = new AsIsBundleOrderer();
            bundleDefectType.Include(
                "~/App_Client/common/services/Setup/DefectType/DefectType.Service.js",
                "~/App_Client/views/Setup/DefectType/DefectType.Controller.js",
                "~/App_Client/views/Setup/DefectType/DefectType.Route.js"
                );
            bundles.Add(bundleDefectType);
            #endregion

            #region Designation bundle
            var bundleDesignation = new Bundle("~/bundles/Designation");
            bundleDesignation.Orderer = new AsIsBundleOrderer();
            bundleDesignation.Include(
                "~/App_Client/common/services/Setup/Designation/Designation.Service.js",
                "~/App_Client/views/Setup/Designation/Designation.Controller.js",
                "~/App_Client/views/Setup/Designation/Designation.Route.js"
                );
            bundles.Add(bundleDesignation);
            #endregion

            #region Forwarder bundle
            var bundleForwarder = new Bundle("~/bundles/Forwarder");
            bundleForwarder.Orderer = new AsIsBundleOrderer();
            bundleForwarder.Include(
                "~/App_Client/common/services/Setup/Forwarder/Forwarder.Service.js",
                "~/App_Client/views/Setup/Forwarder/Forwarder.Controller.js",
                "~/App_Client/views/Setup/Forwarder/Forwarder.Route.js"
                );
            bundles.Add(bundleForwarder);
            #endregion

            #region Machine Description bundle
            var bundleMachineDescription = new Bundle("~/bundles/MachineDescription");
            bundleMachineDescription.Orderer = new AsIsBundleOrderer();
            bundleMachineDescription.Include(
                "~/App_Client/common/services/Setup/MachineDescription/MachineDescription.Service.js",
                "~/App_Client/views/Setup/MachineDescription/MachineDescription.Controller.js",
                "~/App_Client/views/Setup/MachineDescription/MachineDescription.Route.js"
                );
            bundles.Add(bundleMachineDescription);
            #endregion

            #region Machining Description bundle
            var bundleMachiningDescription = new Bundle("~/bundles/MachiningDescription");
            bundleMachiningDescription.Orderer = new AsIsBundleOrderer();
            bundleMachiningDescription.Include(
                "~/App_Client/common/services/Setup/MachiningDescription/MachiningDescription.Service.js",
                "~/App_Client/views/Setup/MachiningDescription/MachiningDescription.Controller.js",
                "~/App_Client/views/Setup/MachiningDescription/MachiningDescription.Route.js"
                );
            bundles.Add(bundleMachiningDescription);
            #endregion

            #region Commodity bundle
            var bundleCommodity = new Bundle("~/bundles/Commodity");
            bundleCommodity.Orderer = new AsIsBundleOrderer();
            bundleCommodity.Include(
                "~/App_Client/common/services/Setup/Commodity/Commodity.Service.js",
                "~/App_Client/views/Setup/Commodity/Commodity.Controller.js",
                "~/App_Client/views/Setup/Commodity/Commodity.Route.js"
                );
            bundles.Add(bundleCommodity);
            #endregion

            #region Document Type bundle
            var bundleDocumentType = new Bundle("~/bundles/DocumentType");
            bundleDocumentType.Orderer = new AsIsBundleOrderer();
            bundleDocumentType.Include(
                "~/App_Client/common/services/Setup/DocumentType/DocumentType.Service.js",
                "~/App_Client/views/Setup/DocumentType/DocumentType.Controller.js",
                "~/App_Client/views/Setup/DocumentType/DocumentType.Route.js"
                );
            bundles.Add(bundleDocumentType);
            #endregion

            #region Status bundle
            var bundleStatus = new Bundle("~/bundles/Status");
            bundleStatus.Orderer = new AsIsBundleOrderer();
            bundleStatus.Include(
                "~/App_Client/common/services/Setup/Status/Status.services.js",
                "~/App_Client/views/Setup/Status/Status.controller.js",
                "~/App_Client/views/Setup/Status/Status.route.js"
                );
            bundles.Add(bundleStatus);
            #endregion

            #region SecondaryOperationDesc bundle
            var bundleSecondaryOperationDesc = new Bundle("~/bundles/SecondaryOperationDesc");
            bundleSecondaryOperationDesc.Orderer = new AsIsBundleOrderer();
            bundleSecondaryOperationDesc.Include(
                "~/App_Client/common/services/Setup/SecondaryOperationDesc/SecondaryOperationDesc.services.js",
                "~/App_Client/views/Setup/SecondaryOperationDesc/SecondaryOperationDesc.controller.js",
                "~/App_Client/views/Setup/SecondaryOperationDesc/SecondaryOperationDesc.route.js"
                );
            bundles.Add(bundleSecondaryOperationDesc);
            #endregion

            #region NonAwardFeedback bundle
            var bundleNonAwardFeedback = new Bundle("~/bundles/NonAwardFeedback");
            bundleNonAwardFeedback.Orderer = new AsIsBundleOrderer();
            bundleNonAwardFeedback.Include(
                "~/App_Client/common/services/Setup/NonAwardFeedback/NonAwardFeedback.services.js",
                "~/App_Client/views/Setup/NonAwardFeedback/NonAwardFeedback.controller.js",
                "~/App_Client/views/Setup/NonAwardFeedback/NonAwardFeedback.route.js"
                );
            bundles.Add(bundleNonAwardFeedback);
            #endregion

            #region RFQSource bundle
            var bundleRFQSource = new Bundle("~/bundles/RFQSource");
            bundleRFQSource.Orderer = new AsIsBundleOrderer();
            bundleRFQSource.Include(
                "~/App_Client/common/services/Setup/RFQSource/RFQSource.services.js",
                "~/App_Client/views/Setup/RFQSource/RFQSource.controller.js",
                "~/App_Client/views/Setup/RFQSource/RFQSource.route.js"
                );
            bundles.Add(bundleRFQSource);
            #endregion

            #region RFQPriority bundle
            var bundleRFQPriority = new Bundle("~/bundles/RFQPriority");
            bundleRFQPriority.Orderer = new AsIsBundleOrderer();
            bundleRFQPriority.Include(
                "~/App_Client/common/services/Setup/RFQPriority/RFQPriority.services.js",
                "~/App_Client/views/Setup/RFQPriority/RFQPriority.controller.js",
                "~/App_Client/views/Setup/RFQPriority/RFQPriority.route.js"
                );
            bundles.Add(bundleRFQPriority);
            #endregion

            #region Industry Type bundle
            var bundleIndustryType = new Bundle("~/bundles/IndustryType");
            bundleIndustryType.Orderer = new AsIsBundleOrderer();
            bundleIndustryType.Include(
                "~/App_Client/common/services/Setup/IndustryType/IndustryType.services.js",
                "~/App_Client/views/Setup/IndustryType/IndustryType.controller.js",
                "~/App_Client/views/Setup/IndustryType/IndustryType.route.js"
                );
            bundles.Add(bundleIndustryType);
            #endregion

            #region Commodity Type bundle
            var bundleCommodityType = new Bundle("~/bundles/CommodityType");
            bundleCommodityType.Orderer = new AsIsBundleOrderer();
            bundleCommodityType.Include(
                "~/App_Client/common/services/Setup/CommodityType/CommodityType.Service.js",
                "~/App_Client/views/Setup/CommodityType/CommodityType.Controller.js",
                "~/App_Client/views/Setup/CommodityType/CommodityType.Route.js"
                );
            bundles.Add(bundleCommodityType);
            #endregion

            #region trigger point bundle
            var bundleTriggerPoint = new Bundle("~/bundles/TriggerPoint");
            bundleTriggerPoint.Orderer = new AsIsBundleOrderer();
            bundleTriggerPoint.Include(
                "~/App_Client/common/services/Setup/TriggerPoint/TriggerPoint.Service.js",
                "~/App_Client/views/Setup/TriggerPoint/TriggerPoint.Controller.js",
                "~/App_Client/views/Setup/TriggerPoint/TriggerPoint.Route.js"
                );
            bundles.Add(bundleTriggerPoint);
            #endregion

            #region email template bundle
            var bundleEmailTemplate = new Bundle("~/bundles/EmailTemplate");
            bundleEmailTemplate.Orderer = new AsIsBundleOrderer();
            bundleEmailTemplate.Include(
                "~/App_Client/common/services/Setup/EmailTemplate/EmailTemplate.Service.js",
                "~/App_Client/views/Setup/EmailTemplate/EmailTemplate.Controller.js",
                "~/App_Client/views/Setup/EmailTemplate/EmailTemplate.Route.js"
                );
            bundles.Add(bundleEmailTemplate);
            #endregion

            #region ChangePassword bundle
            var changepswd = new Bundle("~/bundles/ChangePassword");
            changepswd.Orderer = new AsIsBundleOrderer();
            changepswd.Include(
                "~/App_Client/common/services/UserManagement/UserIdentity.services.js",
                "~/App_Client/views/UserManagement/ChangePassword/ChangePassword.controller.js"
                //,"~/App_Client/views/UserManagement/ChangePassword/ChangePassword.route.js"
                );
            bundles.Add(changepswd);
            #endregion

            #region UserManagement bundle
            var usermngt = new Bundle("~/bundles/UserManagement");
            usermngt.Orderer = new AsIsBundleOrderer();
            usermngt.Include(
                "~/App_Client/common/services/UserManagement/UserIdentity.services.js",
                "~/App_Client/views/UserManagement/UserManagement/Users.controller.js",
                "~/App_Client/views/UserManagement/UserManagement/Users.route.js"
                );
            bundles.Add(usermngt);
            #endregion

            #region UserPreferences bundle
            var userPref = new Bundle("~/bundles/Preferences");
            userPref.Orderer = new AsIsBundleOrderer();
            userPref.Include(
                "~/App_Client/common/services/UserManagement/Preferences.Service.js",
                "~/App_Client/views/UserManagement/Preferences/Preferences.Controller.js"
                //"~/App_Client/views/UserManagement/Preferences/Preferences.Route.js"
                );
            bundles.Add(userPref);
            #endregion

            #region RoleManagement bundle
            var rolemngt = new Bundle("~/bundles/RoleManagement");
            rolemngt.Orderer = new AsIsBundleOrderer();
            rolemngt.Include(
                "~/App_Client/common/services/RoleManagement/Roles.services.js",
                "~/App_Client/views/RoleManagement/Roles.controller.js",
                "~/App_Client/views/RoleManagement/Roles.route.js"
                );
            bundles.Add(rolemngt);
            #endregion

            #region RFQ Supplier bundle
            var bundleSuppliers = new Bundle("~/bundles/RFQ/Supplier");
            bundleSuppliers.Orderer = new AsIsBundleOrderer();
            bundleSuppliers.Include(
                "~/App_Client/common/services/RFQ/Supplier/Contacts.Service.js",
                "~/App_Client/common/services/RFQ/Supplier/SupplierAssessment.Service.js",
                "~/App_Client/common/services/RFQ/Supplier/Suppliers.Service.js",
                "~/App_Client/common/services/FileUploader/FileUploader.Service.js",
                "~/App_Client/views/RFQ/Supplier/Suppliers.Controller.js",
                "~/App_Client/views/RFQ/Supplier/Suppliers.Route.js",
                "~/App_Client/common/directives/FileUploader/fileSelect.directive.js"
                );
            bundles.Add(bundleSuppliers);
            #endregion

            #region RFQ Customer bundle
            var bundleCustomers = new Bundle("~/bundles/RFQ/Customer");
            bundleCustomers.Orderer = new AsIsBundleOrderer();
            bundleCustomers.Include(
                "~/App_Client/common/services/RFQ/Customer/Address.Service.js",
                "~/App_Client/common/services/RFQ/Customer/Contacts.Service.js",
                "~/App_Client/common/services/RFQ/Customer/Divisions.Service.js",
                "~/App_Client/common/services/RFQ/Customer/Customers.Service.js",
                "~/App_Client/views/RFQ/Customer/Customers.Controller.js",
                "~/App_Client/views/RFQ/Customer/Customers.Route.js"
                );
            bundles.Add(bundleCustomers);
            #endregion

            #region RFQ bundle
            var bundleRFQ = new Bundle("~/bundles/RFQ/RFQ");
            bundleRFQ.Orderer = new AsIsBundleOrderer();
            bundleRFQ.Include(
                "~/App_Client/common/services/RFQ/RFQ/RFQ.service.js",
                "~/App_Client/views/RFQ/RFQ/RFQ.controller.js",
                "~/App_Client/views/RFQ/RFQ/RFQ.route.js",
                "~/App_Client/common/services/Setup/EmailTemplate/EmailTemplate.Service.js",
                "~/App_Client/common/directives/FileUploader/fileSelect.directive.js"
                );
            bundles.Add(bundleRFQ);
            #endregion

            #region Shipment Tracking bundle
            var bundleShipmentTracking = new Bundle("~/bundles/ShipmentTracking");
            bundleShipmentTracking.Orderer = new AsIsBundleOrderer();
            bundleShipmentTracking.Include(
                "~/App_Client/common/services/ShipmentTracking/Shipment.service.js",
                "~/App_Client/views/ShipmentTracking/Shipment/Shipment.controller.js",
                "~/App_Client/views/ShipmentTracking/Shipment/Shipment.route.js",
                "~/App_Client/common/directives/FileUploader/fileSelect.directive.js"
                );
            bundles.Add(bundleShipmentTracking);
            #endregion

            #region Shipment Report bundle
            var bundleShipmentReports = new Bundle("~/bundles/ShipmentReports");
            bundleShipmentReports.Orderer = new AsIsBundleOrderer();
            bundleShipmentReports.Include(
                "~/App_Client/common/services/ShipmentTracking/Shipment.service.js",
                "~/App_Client/views/ShipmentTracking/Reports/Reports.controller.js",
                "~/App_Client/views/ShipmentTracking/Reports/Reports.route.js",
                "~/App_Client/common/services/ExportToExcel/ExportToExcel.service.js"
                );
            bundles.Add(bundleShipmentReports);
            #endregion

            #region Upload Shipment bundle
            var bundleUploadShipment = new Bundle("~/bundles/UploadShipment");
            bundleUploadShipment.Orderer = new AsIsBundleOrderer();
            bundleUploadShipment.Include(
                "~/App_Client/common/services/ShipmentTracking/Shipment.service.js",
                "~/App_Client/views/ShipmentTracking/UploadShipment/UploadShipment.controller.js",
                "~/App_Client/views/ShipmentTracking/UploadShipment/UploadShipment.route.js",
                "~/App_Client/common/directives/FileUploader/fileSelect.directive.js"
                );
            bundles.Add(bundleUploadShipment);
            #endregion

            #region SubmiteQuote bundle
            var bundleSubmitQuote = new Bundle("~/bundles/RFQ/RFQ/SubmitQuote");
            bundleSubmitQuote.Orderer = new AsIsBundleOrderer();
            bundleSubmitQuote.Include(
                "~/App_Client/common/services/RFQ/RFQ/SubmitQuote.service.js",
                "~/App_Client/views/RFQ/SubmitQuote/SubmitQuote.controller.js",
                "~/App_Client/views/RFQ/SubmitQuote/SubmitQuote.route.js",
                "~/App_Client/common/directives/FileUploader/fileSelect.directive.js"
                );
            bundles.Add(bundleSubmitQuote);
            #endregion

            #region DQ SubmiteQuote bundle
            var bundleDQ = new Bundle("~/bundles/RFQ/RFQ/DQ");
            bundleDQ.Orderer = new AsIsBundleOrderer();
            bundleDQ.Include(
                "~/App_Client/common/services/RFQ/RFQ/DetailSubmitQuote.service.js",
                "~/App_Client/views/RFQ/DQ/SubmitQuote.controller.js",
                "~/App_Client/views/RFQ/DQ/SubmitQuote.route.js",
                "~/App_Client/common/directives/FileUploader/fileSelect.directive.js"
                );
            bundles.Add(bundleDQ);
            #endregion

            #region Supplier Quote bundle
            var bundleSupplierQuote = new Bundle("~/bundles/RFQ/RFQ/SupplierQuote");
            bundleSupplierQuote.Orderer = new AsIsBundleOrderer();
            bundleSupplierQuote.Include(
                "~/App_Client/common/services/RFQ/RFQ/SupplierQuote.service.js",
                "~/App_Client/views/RFQ/SupplierQuote/SupplierQuote.controller.js",
                "~/App_Client/views/RFQ/SupplierQuote/SupplierQuote.route.js",
                "~/App_Client/common/directives/FileUploader/fileSelect.directive.js"
                );
            bundles.Add(bundleSupplierQuote);
            #endregion

            #region Quote bundle
            var bundleQuote = new Bundle("~/bundles/RFQ/Quote");
            bundleQuote.Orderer = new AsIsBundleOrderer();
            bundleQuote.Include(
                "~/App_Client/common/services/RFQ/Quote/Quote.service.js",
                "~/App_Client/common/services/APQP/APQP/APQP.Service.js",
                "~/App_Client/views/RFQ/Quote/Quote.controller.js",
                "~/App_Client/views/RFQ/Quote/Quote.route.js"
                );
            bundles.Add(bundleQuote);
            #endregion

            #region RFQ Reports bundle
            var bundleRFQReports = new Bundle("~/bundles/RFQ/Reports");
            bundleRFQReports.Orderer = new AsIsBundleOrderer();
            bundleRFQReports.Include(
                "~/App_Client/common/services/RFQ/Reports/Reports.service.js",
                "~/App_Client/views/RFQ/Reports/Reports.controller.js",
                "~/App_Client/views/RFQ/Reports/Reports.route.js",
                "~/App_Client/views/RFQ/Reports/RFQQuoteReportBySupplier.contoller.js",
                "~/App_Client/views/RFQ/Reports/RFQAnalysisReport.contoller.js"
                );
            bundles.Add(bundleRFQReports);
            #endregion

            #region RFQ Part Quote Report From Quote bundle
            var partQuoteReportFromQuote = new Bundle("~/bundles/RFQ/PartQuoteReportFromQuote");
            partQuoteReportFromQuote.Orderer = new AsIsBundleOrderer();
            partQuoteReportFromQuote.Include(
                "~/App_Client/common/services/RFQ/Reports/Reports.service.js",
                "~/App_Client/views/RFQ/PartQuoteReportFromQuote/PartQuoteReportFromQuote.controller.js",
                "~/App_Client/views/RFQ/PartQuoteReportFromQuote/PartQuoteReportFromQuote.route.js"
                );
            bundles.Add(partQuoteReportFromQuote);
            #endregion

            #region APQP bundle
            var bundleAPQP = new Bundle("~/bundles/APQP/APQP");
            bundleAPQP.Orderer = new AsIsBundleOrderer();
            bundleAPQP.Include(
                "~/App_Client/common/services/APQP/APQP/APQP.Service.js",
                "~/App_Client/common/services/APQP/ChangeRequest/ChangeRequest.Service.js",
                "~/App_Client/common/services/RFQ/Customer/Contacts.Service.js",
                "~/App_Client/views/APQP/APQP/APQP.Controller.js",
                "~/App_Client/views/APQP/APQP/APQPAddEdit.Controller.js",
                "~/App_Client/views/APQP/APQP/APQPItemList.Controller.js",
                "~/App_Client/views/APQP/APQP/APQPReports.Controller.js",
                "~/App_Client/views/APQP/APQP/APQP.Route.js",
                "~/App_Client/common/directives/FileUploader/fileSelect.directive.js"
                );
            bundles.Add(bundleAPQP);
            #endregion

            #region Change Request bundle
            var bundleChangeRequest = new Bundle("~/bundles/APQP/ChangeRequest");
            bundleChangeRequest.Orderer = new AsIsBundleOrderer();
            bundleChangeRequest.Include(
                "~/App_Client/common/services/APQP/ChangeRequest/ChangeRequest.Service.js",
                "~/App_Client/common/services/FileUploader/FileUploader.Service.js",
                "~/App_Client/views/APQP/ChangeRequest/ChangeRequest.Controller.js",
                "~/App_Client/views/APQP/ChangeRequest/ChangeRequestList.Controller.js",
                "~/App_Client/views/APQP/ChangeRequest/AddEditChangeRequest.Controller.js",
                "~/App_Client/views/APQP/ChangeRequest/ChangeRequest.Route.js",
                "~/App_Client/common/directives/FileUploader/fileSelect.directive.js"
                );
            bundles.Add(bundleChangeRequest);
            #endregion

            #region Defect Tracking bundle
            var bundleDefectTracking = new Bundle("~/bundles/APQP/DefectTracking");
            bundleDefectTracking.Orderer = new AsIsBundleOrderer();
            bundleDefectTracking.Include(
                "~/App_Client/common/services/APQP/DefectTracking/DefectTracking.service.js",
                "~/App_Client/views/APQP/DefectTracking/DefectTracking.Controller.js",
                "~/App_Client/views/APQP/DefectTracking/DefectTracking.Route.js",
                "~/App_Client/common/directives/FileUploader/fileSelect.directive.js"
                );
            bundles.Add(bundleDefectTracking);
            #endregion

            #region Document Management bundle
            var bundleDocumentManagement = new Bundle("~/bundles/APQP/DocumentManagement");
            bundleDocumentManagement.Orderer = new AsIsBundleOrderer();
            bundleDocumentManagement.Include(
                "~/App_Client/common/services/APQP/DocumentManagement/DocumentManagement.service.js",
                "~/App_Client/views/APQP/DocumentManagement/DocumentManagement.controller.js",
                "~/App_Client/views/APQP/DocumentManagement/DocumentManagement.route.js"
                );
            bundles.Add(bundleDocumentManagement);
            #endregion

            #region CAPA bundle
            var bundleCAPA = new Bundle("~/bundles/APQP/CAPA");
            bundleCAPA.Orderer = new AsIsBundleOrderer();
            bundleCAPA.Include(
                "~/App_Client/common/services/APQP/CAPA/CAPA.service.js",
                "~/App_Client/views/APQP/CAPA/CAPA.controller.js",
                "~/App_Client/views/APQP/CAPA/CAPA.route.js",
                "~/App_Client/common/directives/FileUploader/fileSelect.directive.js"
                );
            bundles.Add(bundleCAPA);
            #endregion

            BundleTable.EnableOptimizations = true;
        }
    }
}
