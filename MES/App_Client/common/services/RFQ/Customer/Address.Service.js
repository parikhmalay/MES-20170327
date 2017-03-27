var app = angular.module('app');
app.factory('AddressSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'CustomerAddressApi/';
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
        GetAddressList: function (customerId) {
            return common.$http({
                url: urlCore + '/GetAddressList',
                method: "POST",
                params: { customerId: customerId }
            });
        },
        Save: function (address) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: address
            });
        },
        Delete: function (addressId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { addressId: addressId }
            });
            return promise;
        },
        DeleteMultipleAddress: function (addressIds) {
            var promise = common.$http({
                url: urlCore + "/DeleteMultipleAddress",
                method: "Post",
                params: { addressIds: addressIds }
            });
            return promise;
        },
        getData: function (addressId) {
            var promise = common.$http({
                url: urlCore + "/Get",
                method: "Get",
                params: { Id: addressId }
            });
            return promise;
        },
    }
}]);
