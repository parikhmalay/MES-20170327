app.factory('RFQSourceSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'RFQSourceApi/';
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

        GetRFQSourceList: function (Paging) {

            return common.$http({
                method: "POST"
                , url: urlCore + '/GetRfqSourceList'
                , data: Paging
            });
        },
        Search: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetRFQSources',
                method: "POST",
                data: Parameters
            });
        },
        Save: function (rfqSource) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: rfqSource
            });
        },

        GetInfo: function () {
            var promise = common.$http({
                url: urlCore + '/GetInfo',
                method: "get"
            });
            return promise;
        },
        Delete: function (rfqSourceId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { rfqSourceId: rfqSourceId }
            });
            return promise;
        }
    }
}]);