app.factory('RFQPrioritySvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'RFQPriorityApi/';
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

        GetRFQPriorityList: function (Paging) {

            return common.$http({
                url: urlCore + '/GetRFQPriorityList',
                method: "POST",
                data: Paging
            });
        },
        Search: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetRFQPrioritys',
                method: "POST",
                data: Parameters
            });
        },
        Save: function (RFQPriority) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: RFQPriority
            });
        },

        GetInfo: function () {
            var promise = common.$http({
                url: urlCore + '/GetInfo',
                method: "get"
            });
            return promise;
        },
        Delete: function (RFQPriorityId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { RFQPriorityId: RFQPriorityId }
            });
            return promise;
        }
    }
}]);