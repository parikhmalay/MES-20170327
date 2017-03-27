app.factory('RFQTypeSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'RFQTypeApi/';
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

        GetRFQTypeList: function (Paging) {

            return common.$http({
                method: "POST"
                , url: urlCore + '/GetRfqTypeList'
                , data: Paging
            });
        },
        Search: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetRFQTypes',
                method: "POST",
                data: Parameters
            });
        },
        Save: function (rfqType) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: rfqType
            });
        },

        GetInfo: function () {
            var promise = common.$http({
                url: urlCore + '/GetInfo',
                method: "get"
            });
            return promise;
        },
        Delete: function (rfqTypeId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { rfqTypeId: rfqTypeId }
            });
            return promise;
        }
    }
}]);