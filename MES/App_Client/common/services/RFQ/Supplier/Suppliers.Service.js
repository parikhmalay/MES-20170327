var app = angular.module('app');
app.factory('SuppliersSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'SuppliersApi/';
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
        GetSuppliersList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetSuppliersList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (suppliers) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: suppliers
            });
        },
        Delete: function (suppliersId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { suppliersId: suppliersId }
            });
            return promise;
        },
        getData: function (supplierId) {
            var promise = common.$http({
                url: urlCore + "/Get",
                method: "Get",
                params: { Id: supplierId }
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
        DeleteMultiple: function (SupplierIDs) {
            var promise = common.$http({
                url: urlCore + "/DeleteMultiple",
                method: "Post",
                params: { SupplierIDs: SupplierIDs }
            });
            return promise;
        },
    }
}]);
