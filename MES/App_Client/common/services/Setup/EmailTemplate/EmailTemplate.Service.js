var app = angular.module('app');
app.factory('EmailTemplateSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'EmailTemplateApi/';
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
        GetEmailTemplateList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetEmailTemplateList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (emailTemplate) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: emailTemplate
            });
        },
        Delete: function (emailTemplateId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { emailTemplateId: emailTemplateId }
            });
            return promise;
        },
        SendEmail: function (emailTemplate) {
            return common.$http({
                url: urlCore + '/SendEmail',
                method: "Post",
                data: emailTemplate
            });
        },
        getData: function (Id) {
            var promise = common.$http({
                url: urlCore + "/Get",
                method: "Get",
                params: { Id: Id }
            });
            return promise;
        },
    }
}]);
