app.factory('IndustryTypeSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'IndustryTypeApi/';
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

        GetIndustryTypeList: function (Paging) {

            return common.$http({
                url: urlCore + '/GetIndustryTypeList',
                method: "POST",
                data: Paging
            });
        },
        Search: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetIndustryTypes',
                method: "POST",
                data: Parameters
            });
        },
        Save: function (IndustryType) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: IndustryType
            });
        },

        GetInfo: function () {
            var promise = common.$http({
                url: urlCore + '/GetInfo',
                method: "get"
            });
            return promise;
        },
        Delete: function (IndustryTypeId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { IndustryTypeId: IndustryTypeId }
            });
            return promise;
        }
    }
}]);