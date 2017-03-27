app.factory('RFQSupplierPartQuoteSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'RFQSupplierPartQuoteApi/';
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
        GetRFQSupplierPartQuoteList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetRFQSupplierPartQuoteList',
                method: "Post",
                data: Paging
            });
        },
        SaveSubmitQuote: function (RFQSupplierPartQuoteList) {
            return common.$http({
                url: urlCore + '/SaveSubmitQuote',
                method: "Post",
                data: RFQSupplierPartQuoteList
            });
        },
        SaveSubmitNoQuote: function (Paging) {
            return common.$http({
                url: urlCore + '/SaveSubmitNoQuote',
                method: "Post",
                data: Paging
            });
        },
        Save: function (SubmitQuote) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: SubmitQuote
            });
        },
        Delete: function (rfqId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { rfqId: rfqId }
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
        downloadRfqSupplierPartQuote: function (Paging) {
            return common.$http({
                url: urlCore + '/DownloadRfqSupplierPartQuote',
                method: "Post",
                data: Paging
            });
        },
        uploadRfqSupplierPartQuote: function (Paging) {
            return common.$http({
                url: urlCore + '/UploadRfqSupplierPartQuote',
                method: "Post",
                data: Paging
            });
        },
    }
}]);