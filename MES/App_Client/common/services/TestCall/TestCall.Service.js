var app = angular.module('app');
app.factory('TestCallSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;

    var urlCore = $rootScope.baseAdminUrl + 'TestApi/';
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
        Search: function (Parameters) {
            return common.$http({
                url: urlCore + '/Search',
                method: "POST",
                data: Parameters
            });
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
