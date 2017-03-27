var app = angular.module('app');
app.factory('TriggerPointSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'TriggerPointApi/';
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
        GetTriggerPointsList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetTriggerPointsList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (triggerPoint) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: triggerPoint
            });
        },
        Delete: function (triggerPointId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { triggerPointId: triggerPointId }
            });
            return promise;
        },
    }
}]);
