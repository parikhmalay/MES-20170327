var app = angular.module('app');
app.factory('ChangeRequestSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'ChangeRequestApi/';
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
        Save: function (changeRequest) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: changeRequest
            });
        },
        Delete: function (changeRequestId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { Id: changeRequestId }
            });
            return promise;
        },
        getData: function (changeRequestId) {
            var promise = common.$http({
                url: urlCore + "/Get",
                method: "Get",
                params: { Id: changeRequestId }
            });
            return promise;
        },
        addToCRFromAPQP: function (apqpItemId) {
            var promise = common.$http({
                url: urlCore + "/AddToCRFromAPQP",
                method: "Get",
                params: { Id: apqpItemId }
            });
            return promise;
        },
        getOnChangeOfPartNumber: function (apqpItemId) {
            var promise = common.$http({
                url: urlCore + "/GetOnChangeOfPartNumber",
                method: "Get",
                params: { Id: apqpItemId }
            });
            return promise;
        },
        AddToAPQP: function (changeRequest) {
            return common.$http({
                url: urlCore + '/AddToAPQP',
                method: "Post",
                data: changeRequest
            });
        },
        SearchFromSAPRecords: function (Paging) {
            var promise = common.$http({
                url: urlCore + "/SearchFromSAPRecords",
                method: "Post",
                data: Paging
            });
            return promise;
        },
        InsertFromSAPRecords: function (ItemIds) {
            return common.$http({
                url: urlCore + '/InsertFromSAPRecords',
                method: "POST",
                params: { ItemIds: ItemIds }
            });
        },
        GetFromSAPAndInsertInLocalSAPTable: function () {
            var promise = common.$http({
                url: urlCore + "/GetFromSAPAndInsertInLocalSAPTable",
                method: "Get"
            });
            return promise;
        },
        GetChangeRequestList: function (Paging) {
            var promise = common.$http({
                url: urlCore + "/GetChangeRequestList",
                method: "Post",
                data: Paging
            });
            return promise;
        },

        GetChangeLog: function (changeRequestId) {
            var promise = common.$http({
                url: urlCore + "/GetChangeLog",
                method: "Get",
                params: { Id: changeRequestId }
            });
            return promise;
        },
        DeleteDocument: function (documentId) {
            var promise = common.$http({
                url: urlCore + "/DeleteDocument",
                method: "Post",
                params: { documentId: documentId }
            });
            return promise;
        },
    }
}]);
