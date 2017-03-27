var app = angular.module('app');
app.factory('CommodityTypeSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'CommodityTypeApi/';
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
        GetCommodityTypesList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetCommodityTypesList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (commodityType) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: commodityType
            });
        },
        Delete: function (commodityTypeId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { commodityTypeId: commodityTypeId }
            });
            return promise;
        },
    }
}]);
