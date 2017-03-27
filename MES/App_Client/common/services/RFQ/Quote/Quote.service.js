app.factory('QuoteSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'QuoteApi/';
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
        GetQuoteList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetQuoteList',
                method: "Post",
                data: Paging
            });
        },
        SaveQuote: function (quote) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: quote
            });
        },
        CreateQuote: function (quote) {
            return common.$http({
                url: urlCore + '/Create',
                method: "Post",
                data: quote
            });
        },
        GetPartsToQuote: function (rfqId) {
            var promise = common.$http({
                url: urlCore + "/GetPartsToQuote",
                method: "Post",
                params: { rfqId: rfqId }
            });
            return promise;
        },
        GetQuoteDetails: function (quoteId, isR) {
            var promise = common.$http({
                url: urlCore + "/GetQuoteDetails",
                method: "Post",
                params: { quoteId: quoteId, isR: isR }
            });
            return promise;
        },
        GetQuotePartsDetail: function (quote) {
            return common.$http({
                url: urlCore + "/GetQuotePartsDetail",
                method: "Post",
                data: quote
            });
        },
        GetQuotedDetails: function (qdItem) {
            return common.$http({
                url: urlCore + "/GetSupplierQuotedDetails",
                method: "Post",
                data: qdItem
            });
        },
        exportToExcelQuote: function (quote) {
            return common.$http({
                url: urlCore + '/exportToExcel',
                method: "Post",
                data: quote
            });
        },
        SaveQuoteCalculationHistory: function (quote) {
            return common.$http({
                url: urlCore + '/SaveQuoteCalculationHistory',
                method: "Post",
                data: quote
            });
        }
    }
}]);