var app = angular.module('app');
app.factory('ProjectCategorySvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'ProjectCategoryApi/';
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
        GetProjectCategoryList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetProjectCategoryList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (projectCategory) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: projectCategory
            });
        },
        Delete: function (projectCategoryId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { projectCategoryId: projectCategoryId }
            });
            return promise;
        },
    }
}]);
