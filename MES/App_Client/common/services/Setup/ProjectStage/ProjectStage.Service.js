var app = angular.module('app');
app.factory('ProjectStageSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'ProjectStageApi/';
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
        GetProjectStagesList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetProjectStagesList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (projectStage) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: projectStage
            });
        },
        Delete: function (projectStageId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { projectStageId: projectStageId }
            });
            return promise;
        },
    }
}]);
