app.factory('RemarksSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'RemarksApi/';
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

        GetRemarksList: function (Paging) {

            return common.$http({
                method: "POST"
                , url: urlCore + '/GetRemarksList'
                , data: Paging
            });
        },
        Search: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetRemarks',
                method: "POST",
                data: Parameters
            });
        },
        Save: function (Remarks) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: Remarks
            });
        },

        GetInfo: function () {
            var promise = common.$http({
                url: urlCore + '/GetInfo',
                method: "get"
            });
            return promise;
        },
        Delete: function (RemarksId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { RemarksId: RemarksId }
            });
            return promise;
        }
    }
}]);