var app = angular.module('app');
app.factory('MachiningDescriptionSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'MachiningDescriptionApi/';
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
        GetMachiningDescriptionList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetMachiningDescriptionList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (machiningDescription) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: machiningDescription
            });
        },
        Delete: function (machiningDescriptionId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { machiningDescriptionId: machiningDescriptionId }
            });
            return promise;
        },
    }
}]);
