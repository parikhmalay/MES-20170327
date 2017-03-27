var app = angular.module('app');
app.factory('ContactsSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'CustomerContactsApi/';
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
        GetContactsList: function (customerId) {
            return common.$http({
                url: urlCore + '/GetContactsList',
                method: "POST",
                params: { customerId: customerId }
            });
        },
        Save: function (contacts) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: contacts
            });
        },
        Delete: function (contactId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { contactId: contactId }
            });
            return promise;
        },
        DeleteMultiple: function (ContactIds) {
            var promise = common.$http({
                url: urlCore + "/DeleteMultiple",
                method: "Post",
                params: { ContactIds: ContactIds }
            });
            return promise;
        },
        getData: function (contactId) {
            var promise = common.$http({
                url: urlCore + "/Get",
                method: "Get",
                params: { Id: contactId }
            });
            return promise;
        },
    }
}]);
