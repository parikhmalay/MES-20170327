var app = angular.module('app');
app.factory('APQPSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'APQPApi/';
    var url = $resource(urlCore + '/:id',
            {
                id: '@id'
            },
            {
                'save': {
                    method: 'Post',
                    url: urlCore + '/Post'
                },
                'update': {
                    method: 'Put',
                    url: urlCore + '/Put'
                },
                'search': {
                    method: 'Post',
                    url: urlCore + '/Search'
                },
                'get': {
                    method: 'Get',
                    url: urlCore + '/Get/:id'
                },
                'delete': {
                    method: 'Delete',
                    url: urlCore + '/Delete/:id'
                },
            });
    return {
        GetAPQPList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetAPQPList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (aPQP) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: aPQP
            });
        },
        Delete: function (aPQPId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { aPQPId: aPQPId }
            });
            return promise;
        },
        getData: function (aPQPId) {
            var promise = common.$http({
                url: urlCore + "/Get",
                method: "Get",
                params: { Id: aPQPId }
            });
            return promise;
        },
        SearchFromSAPRecords: function (Paging) {
            var promise = common.$http({
                url: urlCore + "/SearchFromSAPRecords",
                method: "Post",
                data: Paging
            });
            return promise;
        },
        GetAPQPList: function (Paging) {
            var promise = common.$http({
                url: urlCore + "/GetAPQPList",
                method: "Post",
                data: Paging
            });
            return promise;
        },
        InsertFromSAPRecords: function (ItemIds) {
            return common.$http({
                url: urlCore + '/InsertFromSAPRecords',
                method: "POST",
                params: { ItemIds: ItemIds }
            });
        },
        getGeneratePSWData: function (aPQPId) {
            var promise = common.$http({
                url: urlCore + "/getGeneratePSWData",
                method: "Get",
                params: { Id: aPQPId }
            });
            return promise;
        },
        ExportGeneratePSW: function (PSWData) {
            return common.$http({
                url: urlCore + '/ExportGeneratePSW',
                method: "Post",
                data: PSWData
            });
        },
        getAPQPPredefinedDocumentTypes: function (APQPItemIds) {
            var promise = common.$http({
                url: urlCore + "/getAPQPPredefinedDocumentTypes",
                method: "Get",
                params: { APQPItemIds: APQPItemIds }
            });
            return promise;
        },
        SavePredefinedDocumentType: function (DocumentTypeIds, APQPItemIds) {
            return common.$http({
                url: urlCore + '/SavePredefinedDocumentType',
                method: "POST",
                params: { DocumentTypeIds: DocumentTypeIds, APQPItemIds: APQPItemIds }
            });
        },
        getDocumentsData: function (aPQPId, sectionName) {
            var promise = common.$http({
                url: urlCore + "/getDocumentsData",
                method: "Get",
                params: { APQPItemId: aPQPId, SectionName: sectionName }
            });
            return promise;
        },
        SaveDocument: function (document) {
            return common.$http({
                url: urlCore + '/SaveDocument',
                method: "Post",
                data: document
            });
        },
        SaveShareDocumentFiles: function (APQPItemIds, documentId) {
            return common.$http({
                url: urlCore + '/SaveShareDocumentFiles',
                method: "Post",
                params: { APQPItemIds: APQPItemIds, documentId: documentId }
            });
        },
        DeleteDocument: function (documentId) {
            var promise = common.$http({
                url: urlCore + "/DeleteDocument",
                method: "Post",
                params: { documentId: documentId }
            });
            return promise;
        },
        GetAPQPQuotePartByQuoteIds: function (QuoteIds) {
            var promise = common.$http({
                url: urlCore + "/GetAPQPQuotePartByQuoteIds",
                method: "Get",
                params: { QuoteIds: QuoteIds }
            });
            return promise;
        },
        InsertAPQPItemQuotePart: function (ItemIds) {
            return common.$http({
                url: urlCore + '/InsertAPQPItemQuotePart',
                method: "POST",
                params: { ItemIds: ItemIds }
            });
        },

        PPAPSubmissionSAPDataExport: function (APQPItemIds) {
            return common.$http({
                url: urlCore + '/PPAPSubmissionSAPDataExport',
                method: "Post",
                params: { APQPItemIds: APQPItemIds }
            });
        },
        SendDataToSAP: function (APQPItemIds) {
            return common.$http({
                url: urlCore + '/SendDataToSAP',
                method: "Post",
                params: { APQPItemIds: APQPItemIds }
            });
        },
        APQPTabWiseDataExport: function (Paging) {
            return common.$http({
                url: urlCore + '/APQPTabWiseDataExport',
                method: "Post",
                data: Paging
            });
        },
        GenerateNPIF: function (data) {
            return common.$http({
                url: urlCore + '/GenerateNPIF',
                method: "Post",
                params: { APQPItemId: data }
            });
        },
        getTemplateAndUserIdsData: function (APQPItemIds, SectionName) {
            var promise = common.$http({
                url: urlCore + "/getTemplateAndUserIdsData",
                method: "Get",
                params: { APQPItemIds: APQPItemIds, SectionName: SectionName }
            });
            return promise;
        },
        APQPSendEmail: function (EmailData) {
            return common.$http({
                url: urlCore + '/APQPSendEmail',
                method: "Post",
                data: EmailData
            });
        },
        UpdateIndividualFields: function (updateIndividualFields) {
            return common.$http({
                url: urlCore + '/UpdateIndividualFields',
                method: "Post",
                data: updateIndividualFields
            });
        },
        SaveAPQPItemList: function (lstAPQP) {
            return common.$http({
                url: urlCore + '/SaveAPQPItemList',
                method: "Post",
                data: lstAPQP
            });
        },
        GetFromSAPAndInsertInLocalSAPTable: function () {
            var promise = common.$http({
                url: urlCore + "/GetFromSAPAndInsertInLocalSAPTable",
                method: "Get"
            });
            return promise;
        },
        GetDocumentsAvailabilityByAPQPItemId: function (APQPItemId) {
            var promise = common.$http({
                url: urlCore + "/GetDocumentsAvailabilityByAPQPItemId",
                method: "Get",
                params: { APQPItemId: APQPItemId }
            });
            return promise;
        },
        getNextOrPreviousAPQPItemId: function (Paging) {
            return common.$http({
                url: urlCore + "/getNextOrPreviousAPQPItemId",
                method: "Post",
                data: Paging
            });
        },
        DeleteAPQPItem: function (APQPItemId) {
            var promise = common.$http({
                url: urlCore + "/DeleteAPQPItem",
                method: "Post",
                params: { APQPItemId: APQPItemId }
            });
            return promise;
        },
        GetAPQPProjectStatusReport: function (Paging) {
            var promise = common.$http({
                url: urlCore + "/GetAPQPProjectStatusReport",
                method: "Post",
                data: Paging
            });
            return promise;
        },
        GetAPQPNewBusinessAwardedReport: function (Paging) {
            var promise = common.$http({
                url: urlCore + "/GetAPQPNewBusinessAwardedReport",
                method: "Post",
                data: Paging
            });
            return promise;
        },
        GetDefectTrackingReport: function (Paging) {
            var promise = common.$http({
                url: urlCore + "/GetDefectTrackingReport",
                method: "Post",
                data: Paging
            });
            return promise;
        },
        ExportAPQPProjectStatusReport: function (Paging) {
            var promise = common.$http({
                url: urlCore + "/ExportAPQPProjectStatusReport",
                method: "Post",
                data: Paging
            });
            return promise;
        },
        ExportDefectTrackingReport: function (Paging) {
            var promise = common.$http({
                url: urlCore + "/ExportDefectTrackingReport",
                method: "Post",
                data: Paging
            });
            return promise;
        },
        GetAPQPPPAPApprovalReport: function (Paging) {
            var promise = common.$http({
                url: urlCore + "/GetAPQPPPAPApprovalReport",
                method: "Post",
                data: Paging
            });
            return promise;
        },
        ExportAPQPPPAPApprovalReport: function (Paging) {
            var promise = common.$http({
                url: urlCore + "/ExportAPQPPPAPApprovalReport",
                method: "Post",
                data: Paging
            });
            return promise;
        },
        GetAPQPWeeklyMeetingReport: function (Paging) {
            var promise = common.$http({
                url: urlCore + "/GetAPQPWeeklyMeetingReport",
                method: "Post",
                data: Paging
            });
            return promise;
        },
        ExportAPQPWeeklyMeetingReport: function (Paging) {
            var promise = common.$http({
                url: urlCore + "/ExportAPQPWeeklyMeetingReport",
                method: "Post",
                data: Paging
            });
            return promise;
        },
        getManufacturerDetails: function (manufacturerId) {
            var promise = common.$http({
                url: urlCore + "/getManufacturerDetails",
                method: "Get",
                params: { manufacturerId: manufacturerId }
            });
            return promise;
        },
        SendNPIF: function (npifData) {
            return common.$http({
                url: urlCore + '/SendNPIF',
                method: "Post",
                data: npifData
            });
        },
        getPredefinedNPIFRecipients: function (APQPItemId) {
            var promise = common.$http({
                url: urlCore + "/getPredefinedNPIFRecipients",
                method: "Get",
                params: { APQPItemId: APQPItemId }
            });
            return promise;
        },
        checkNPIFDocuSignStatus: function () {
            var promise = common.$http({
                url: urlCore + "/CheckNPIFDocuSignStatus",
                method: "Get"
            });
            return promise;
        },
        getNPIFDocuSignList: function (apqpItemId) {            
            return common.$http({
                url: urlCore + '/GetNPIFDocuSignList',
                method: "POST",
                params: { apqpItemId: apqpItemId }
            });
        }
    }
}]);
