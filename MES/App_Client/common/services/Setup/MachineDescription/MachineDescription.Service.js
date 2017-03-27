var app = angular.module('app');
app.factory('MachineDescriptionSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'MachineDescriptionApi/';
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
        GetMachineDescriptionList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetMachineDescriptionList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (machineDescription) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: machineDescription
            });
        },
        Delete: function (machineDescriptionId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { machineDescriptionId: machineDescriptionId }
            });
            return promise;
        },
    }
}]);
