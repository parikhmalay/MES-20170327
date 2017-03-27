
app.factory('ProcessSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'ProcessApi/';
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

        GetProcessList: function (Paging) {

            return common.$http({
                method: "POST"
                , url: urlCore + '/GetProcessList'
                , data: Paging
            });
        },
        Search: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetProcesses',
                method: "POST",
                data: Parameters
            });
        },
        Save: function (Process) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: Process
            });
        },

        GetInfo: function () {
            var promise = common.$http({
                url: urlCore + '/GetInfo',
                method: "get"
            });
            return promise;
        },
        Delete: function (ProcessId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { ProcessId: ProcessId }
            });
            return promise;
        }
    }
}]);