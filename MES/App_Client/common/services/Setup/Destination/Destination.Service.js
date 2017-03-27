var app = angular.module('app');
app.factory('DestinationSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'DestinationApi/';
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
        GetDestinationsList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetDestinationsList',
                method: "POST",
                data: Paging
            });
        },
        Search: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetDestinations',
                method: "POST",
                data: Parameters
            });
        },
        Save: function (destination) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: destination
            });
            //return promise;
        },
        Delete: function (destinationId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { destinationId: destinationId }
            });
            return promise;
        },
        GetInfo: function () {
            var promise = common.$http({
                url: urlCore + '/GetInfo',
                method: "get"
            });
            return promise;
        },
        ExportData: function (Parameters) {
            return common.$http({
                url: urlCore + '/ExportData',
                method: "POST",
                data: Parameters
            });
        },
    }
}]);
