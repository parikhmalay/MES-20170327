var app = angular.module('app');
app.factory('DivisionsSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'DivisionsApi/';
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
        GetDivisionsList: function (supplierId) {
            return common.$http({
                url: urlCore + '/GetDivisionsList',
                method: "POST",
                params: { supplierId: supplierId }
            });
        },
        Save: function (divisions) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: divisions
            });
        },
        Delete: function (divisionId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { divisionId: divisionId }
            });
            return promise;
        },
        DeleteMultiple: function (DivisionIds) {
            var promise = common.$http({
                url: urlCore + "/DeleteMultiple",
                method: "Post",
                params: { DivisionIds: DivisionIds }
            });
            return promise;
        },
        getData: function (divisionId) {
            var promise = common.$http({
                url: urlCore + "/Get",
                method: "Get",
                params: { Id: divisionId }
            });
            return promise;
        },
    }
}]);
