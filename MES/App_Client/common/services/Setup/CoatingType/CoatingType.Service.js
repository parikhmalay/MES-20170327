var app = angular.module('app');
app.factory('CoatingTypeSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'CoatingTypeApi/';
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
        GetCoatingTypeList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetCoatingTypeList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (coatingType) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: coatingType
            });
        },
        Delete: function (coatingTypeId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { coatingTypeId: coatingTypeId }
            });
            return promise;
        },
    }
}]);
