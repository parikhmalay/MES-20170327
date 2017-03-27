var app = angular.module('app');
app.factory('CAPASvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'CAPAApi/';
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
        GetCAPAList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetCAPAList',
                method: "POST",
                data: Paging
            });
        },
        CheckPartAssociationWithSupplier: function (capaData) {
            var promise = common.$http({
                url: urlCore + "/CheckPartAssociationWithSupplier",
                method: "Post",
                data: capaData
            });
            return promise;
        },
        GenerateCAPAForm: function (capaData) {
            var promise = common.$http({
                url: urlCore + "/GenerateCAPAForm",
                method: "Post",
                data: capaData
            });
            return promise;
        },
        getData: function (capaId) {
            var promise = common.$http({
                url: urlCore + "/Get",
                method: "Get",
                params: { Id: capaId }
            });
            return promise;
        },
        Save: function (CAPA) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: CAPA
            });
        },
        Delete: function (CAPAId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { id: CAPAId }
            });
            return promise;
        },
        getSAPPartsList: function (supplierName, customerName) {
            var promise = common.$http({
                url: urlCore + "/getSAPPartsList",
                method: "Post",
                params: { supplierName: supplierName, customerName: customerName }
            });
            return promise;
        },
        DeleteCAPAPartAffectedDetail: function (CAPAPartAffectedDetailId) {
            var promise = common.$http({
                url: urlCore + "/DeleteCAPAPartAffectedDetail",
                method: "Post",
                params: { CAPAPartAffectedDetailId: CAPAPartAffectedDetailId }
            });
            return promise;
        },
        GetPartDocumentList: function (cAPAPartAffectedDetailId, sectionName) {
            return common.$http({
                url: urlCore + '/GetPartDocumentList',
                method: "POST",
                params: { cAPAPartAffectedDetailId: cAPAPartAffectedDetailId, SectionName: sectionName }
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
        SendEmail: function (EmailData) {
            return common.$http({
                url: urlCore + '/SendEmail',
                method: "Post",
                data: EmailData
            });
        },
    }
}]);
