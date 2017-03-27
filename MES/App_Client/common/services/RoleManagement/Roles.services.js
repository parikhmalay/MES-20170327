app.factory('RolesSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'RolesApi/';
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
        GetRoleList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetRoleList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (role) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: role
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
        RoleNameExists: function (roleName) {
            var promise = common.$http({
                url: urlCore + "/RoleNameExists",
                method: "Get",
                params: { roleName: roleName }
            });
            return promise;
        },
        getData: function (Id) {
            var promise = common.$http({
                url: urlCore + "/Get",
                method: "GET",
                params: { Id: Id }
            });
            return promise;
        }
    }

}]);