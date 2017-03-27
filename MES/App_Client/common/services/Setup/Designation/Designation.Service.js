var app = angular.module('app');
app.factory('DesignationSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'DesignationApi/';
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
        GetDesignationList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetDesignationList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (designation) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: designation
            });
        },
        Delete: function (designationId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { designationId: designationId }
            });
            return promise;
        },
    }
}]);
