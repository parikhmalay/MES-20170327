﻿var app = angular.module('app');
app.factory('DocumentTypeSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'DocumentTypeApi/';
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
        GetDocumentTypesList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetDocumentTypesList',
                method: "POST",
                data: Paging
            });
        },
        Save: function (documentType) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: documentType
            });
        },
        Delete: function (documentTypeId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { documentTypeId: documentTypeId }
            });
            return promise;
        },
    }
}]);