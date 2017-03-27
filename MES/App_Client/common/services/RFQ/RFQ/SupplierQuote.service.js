app.factory('SupplierQuoteSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'RFQSupplierQuoteApi/';
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
        GetSupplierQuoteList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetSupplierQuoteList',
                method: "Post",
                data: Paging
            });
        },
        SaveSupplierQuoteList: function (RFQSupplierPartQuoteList) {
            return common.$http({
                url: urlCore + '/SaveSupplierQuoteList',
                method: "Post",
                data: RFQSupplierPartQuoteList
            });
        },
        exportToExcelSupplierQuote: function (Paging) {
            return common.$http({
                url: urlCore + '/exportToExcelSupplierQuote',
                method: "Post",
                data: Paging
            });
        },
        getRFQDetails: function (Paging) {
            return common.$http({
                url: urlCore + '/getRFQDetails',
                method: "Post",
                data: Paging
            });
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
        getRFQPartCostComparisonList: function (rfqId) {
            return common.$http({
                url: urlCore + '/getRFQPartCostComparisonList',
                method: "Post",
                params: { rfqId: rfqId }
            });
        },
    }
}]);