var app = angular.module('app');

app.factory('LookupSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'Lookup/Query';
    return {
        GetLookup: function (lookupName) {
            var promise = common.$http({
                url: urlCore,
                method: "Post",
                data: [{ "Name": lookupName }]
            });
            return promise;
        },
        GetLookupByQuery: function (query) {
            var promise = common.$http({
                url: urlCore,
                method: "Post",
                data: query
            });
            return promise;
        },
    }
}]);