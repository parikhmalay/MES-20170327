app.factory('OriginSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'OriginApi/';
    console.log(urlCore);
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
            }
        });
    return {

        GetOriginList: function (Paging) {

            return common.$http({
                url: urlCore + '/GetOriginList',
                method: "POST",
                data: Paging
            });
        },
        Search: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetOrigins',
                method: "POST",
                data: Parameters
            });
        },
        Save: function (Origin) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: Origin
            });
        },

        GetInfo: function () {
            var promise = common.$http({
                url: urlCore + '/GetInfo',
                method: "get"
            });
            return promise;
        },
        Delete: function (OriginId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { OriginId: OriginId }
            });
            return promise;
        }
    }
}]);