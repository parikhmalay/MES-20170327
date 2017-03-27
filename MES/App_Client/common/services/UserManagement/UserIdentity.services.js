app.factory('IdentitySvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'UserIdentityApi/';
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
         'ChangePassword': {
             method: 'Post',
             url: urlCore + '/Post'
         },
     });
    return {
        GetCurrentUserInfo: function () {
            var promise = common.$http({
                url: urlCore + "/GetCurrentUserInfo",
                method: "POST",
                data: ''
            });
            return promise;
        },
        GetUserList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetUserList',
                method: "POST",
                data: Paging
            });
        },
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
        getData: function (Id) {
            var promise = common.$http({
                url: urlCore + "/Get",
                method: "Get",
                params: { Id: Id }
            });
            return promise;
        },
        UserNameExists: function (userName) {
            var promise = common.$http({
                url: urlCore + "/UserNameExists",
                method: "Get",
                params: { userName: userName }
            });
            return promise;
        },
        ChangePassword: function (user) {           
            var promise = common.$http({
                url: urlCore + "/ChangePassword",
                method: "Post",
                data: user
            });
            return promise;
        },
        SearchUsersWithEmail: function (val, userIds) {
            var promise = common.$http({
                url: urlCore + "/SearchUsersWithEmail",
                method: "Post",
                data: {
                    'searchText': val,
                    'userIds': userIds
                }
            });
            return promise;
        },
        DefaultSearchCriteria: function () {
            var promise = common.$http({
                url: urlCore + "/DefaultSearchCriteria",
                method: "Get",
                data: ''
            });
            return promise;
        },
    }
}
]);