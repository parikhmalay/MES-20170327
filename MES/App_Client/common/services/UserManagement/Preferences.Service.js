app.factory('PreferencesSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'PreferencesApi/';
    var url = $resource(urlCore + '/:Id',
     {
         Id: '@Id'
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
        Save: function (user) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: user
            });
        },
        Delete: function (Id) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { Id: Id }
            });
            return promise;
        },
        getData: function (userId) {
            var promise = common.$http({
                url: urlCore + "/Get",
                method: "Get",
                params: { userId: userId }
            });
            return promise;
        },
    }
}
]);