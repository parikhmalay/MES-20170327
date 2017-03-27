var app = angular.module('app');
app.factory('ForwarderSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'ForwarderApi/';
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
        GetForwarderList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetForwarderList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (forwarder) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: forwarder
            });
        },
        Delete: function (forwarderId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { forwarderId: forwarderId }
            });
            return promise;
        },
    }
}]);
