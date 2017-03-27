var app = angular.module('app');
app.factory('DefectTypeSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'DefectTypeApi/';
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
        GetDefectTypeList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetDefectTypeList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (defectType) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: defectType
            });
        },
        Delete: function (defectTypeId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { defectTypeId: defectTypeId }
            });
            return promise;
        },
    }
}]);
