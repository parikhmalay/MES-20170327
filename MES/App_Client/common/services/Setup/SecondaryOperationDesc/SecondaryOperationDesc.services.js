app.factory('SecondaryOperationDescSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'SecondaryOperationDescApi/';
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

        GetSecondaryOperationDescList: function (Paging) {

            return common.$http({
                url: urlCore + '/GetSecondaryOperationDescList',
                method: "POST",
                data: Paging
            });
        },
        Search: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetSecondaryOperationDescs',
                method: "POST",
                data: Parameters
            });
        },
        Save: function (SecondaryOperationDesc) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: SecondaryOperationDesc
            });
        },

        GetInfo: function () {
            var promise = common.$http({
                url: urlCore + '/GetInfo',
                method: "get"
            });
            return promise;
        },
        Delete: function (SecondaryOperationDescId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { SecondaryOperationDescId: SecondaryOperationDescId }
            });
            return promise;
        }
    }
}]);