var app = angular.module('app');
app.factory('DocumentManagementSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'DocumentManagementApi/';
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
        GetDocumentManagementList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetDocumentManagementList',
                method: "POST",
                data: Paging
            });
        },
        GetDocumentList: function (documentManagementId) {
            return common.$http({
                url: urlCore + '/GetDocumentList',
                method: "Post",
                params: { DocumentManagementId: documentManagementId }
            });
        },
        DownloadDocuments: function (documentFilePathList) {
            return common.$http({
                url: urlCore + '/DownloadDocuments',
                method: "POST",
                data: documentFilePathList
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
