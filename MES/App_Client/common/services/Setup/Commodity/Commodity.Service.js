var app = angular.module('app');
app.factory('CommoditySvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'CommodityApi/';
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
        GetCommodityList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetCommodityList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (commodity) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: commodity
            });
        },
        Delete: function (commodityId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { commodityId: commodityId }
            });
            return promise;
        },
    }
}]);
