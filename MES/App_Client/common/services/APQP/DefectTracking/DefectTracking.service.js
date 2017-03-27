var app = angular.module('app');
app.factory('DefectTrackingSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'DefectTrackingApi/';
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
        GetDefectTrackingList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetDefectTrackingList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (defectTracking) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: defectTracking
            });
        },
        Delete: function (DefectTrackingId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { id: DefectTrackingId }
            });
            return promise;
        },
        getData: function (DefectTrackingId) {
            var promise = common.$http({
                url: urlCore + "/Get",
                method: "Get",
                params: { Id: DefectTrackingId }
            });
            return promise;
        },
        GetNewRMANumber: function () {
            var promise = common.$http({
                url: urlCore + "/GetNewRMANumber",
                method: "Get"
            });
            return promise;
        },
        GenerateCAPAForm: function (defectTracking) {
            var promise = common.$http({
                url: urlCore + "/GenerateCAPAForm",
                method: "Post",
                data: defectTracking
            });
            return promise;
        },
        GetPartDocumentList: function (defectTrackingDetailId, sectionName) {
            return common.$http({
                url: urlCore + '/GetPartDocumentList',
                method: "POST",
                params: { defectTrackingDetailId: defectTrackingDetailId, SectionName: sectionName }
            });
        },
        SaveDocument: function (document) {
            return common.$http({
                url: urlCore + '/SavePartDocument',
                method: "Post",
                data: document
            });
        },
        DeleteDocument: function (documentId) {
            var promise = common.$http({
                url: urlCore + "/DeletePartDocument",
                method: "Post",
                params: { documentId: documentId }
            });
            return promise;
        },
        DeleteDefectTrackingDetail: function (defectTrackingDetailId) {
            var promise = common.$http({
                url: urlCore + "/DeleteDefectTrackingDetail",
                method: "Post",
                params: { DefectTrackingDetailId: defectTrackingDetailId }
            });
            return promise;
        },
    }
}]);
