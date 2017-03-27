app.factory('StatusSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'StatusApi/';
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

        GetStatusList: function (Paging) {

            return common.$http({
                url: urlCore + '/GetStatusList',
                method: "POST",
                data: Paging
            });
        },
        Search: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetStatus',
                method: "POST",
                data: Parameters
            });
        },
        Save: function (Status) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: Status
            });
        },

        GetInfo: function () {
            var promise = common.$http({
                url: urlCore + '/GetInfo',
                method: "get"
            });
            return promise;
        },
        Delete: function (StatusId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { StatusId: StatusId }
            });
            return promise;
        }
    }
}]);