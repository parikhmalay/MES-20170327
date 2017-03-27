var app = angular.module('app');
app.factory('CustomersSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'CustomersApi/';
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
        GetCustomersList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetCustomersList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (customers) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: customers
            });
        },
        Delete: function (customersId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { customersId: customersId }
            });
            return promise;
        },
        getData: function (customerId) {
            var promise = common.$http({
                url: urlCore + "/Get",
                method: "Get",
                params: { Id: customerId }
            });
            return promise;
        },
        DeleteMultiple: function (CustomerIDs) {
            var promise = common.$http({
                url: urlCore + "/DeleteMultiple",
                method: "Post",
                params: { CustomerIDs: CustomerIDs }
            });
            return promise;
        },
    }
}]);
