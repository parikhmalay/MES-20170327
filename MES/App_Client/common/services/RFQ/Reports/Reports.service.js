app.factory('RFQReportSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'ReportsApi/';
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
        GetRFQList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetRFQList',
                method: "Post",
                data: Paging
            });
        },
        // start get RFQAnalysisReport methods
        GetRFQAnalysisReport: function (Paging) {
            return common.$http({
                url: urlCore + '/GetRFQAnalysisReport',
                method: 'Post',
                data: Paging
            });
        },
        exportRFQAnalysisReport: function (Paging) {
            return common.$http({
                url: urlCore + '/exportRFQAnalysisReport',
                method: "Post",
                data: Paging
            });
        },
        // End get RFQAnalysisReport methods

        // start get RFQNonAwardReasonReport methods
        GetRFQNonAwardReasonReport: function (Paging) {
            return common.$http({
                url: urlCore + '/GetRFQNonAwardReasonReport',
                method: 'Post',
                data: Paging
            });
        },
        exportRFQNonAwardReasonReport: function (Paging) {
            return common.$http({
                url: urlCore + '/exportRFQNonAwardReasonReport',
                method: "Post",
                data: Paging
            });
        },
        // End get RFQNonAwardReasonReport methods

        // start Quote's Total $ Quoted report
        GetQuoteTotalDollarQuotedReport: function (Paging) {
            return common.$http({
                url: urlCore + '/GetQuoteTotalDollarQuotedReport',
                method: 'Post',
                data: Paging
            });
        },
        exportQuoteTotalDollarQuotedReport: function (Paging) {
            return common.$http({
                url: urlCore + '/exportQuoteTotalDollarQuotedReport',
                method: "Post",
                data: Paging
            });
        },
        // End Quote's Total $ Quoted report

        // start Quotes done Report
        GetQuotesDoneReport: function (Paging) {
            return common.$http({
                url: urlCore + '/GetQuotesDoneReport',
                method: 'Post',
                data: Paging
            });
        },
        exportQuotesDoneReport: function (Paging) {
            return common.$http({
                url: urlCore + '/exportQuotesDoneReport',
                method: "Post",
                data: Paging
            });
        },
        // End Quotes done Report

        // start RFQ Quote Report by Supplier
        GetRFQPartsSupplierWiseReport: function (Paging) {
            return common.$http({
                url: urlCore + '/GetRFQPartsSupplierWiseReport',
                method: 'Post',
                data: Paging
            });
        },
        exportRFQPartsSupplierWiseReport: function (Paging) {
            return common.$http({
                url: urlCore + '/exportRFQPartsSupplierWiseReport',
                method: "Post",
                data: Paging
            });
        },
        // End RFQ Quote Report by Supplier

        // start Supplier Parts Quote Report
        GetRFQSupplierPartsQuoteReport: function (Paging) {
            return common.$http({
                url: urlCore + '/GetRFQSupplierPartsQuoteReport',
                method: 'Post',
                data: Paging
            });
        },
        exportRFQSupplierPartsQuoteReport: function (Paging) {
            return common.$http({
                url: urlCore + '/exportRFQSupplierPartsQuoteReport',
                method: "Post",
                data: Paging
            });
        },
        // End Supplier Parts Quote Report

        // start RFQ Part Cost Comparison Report
        GetRFQPartCostComparisonReport: function (Paging) {
            return common.$http({
                url: urlCore + '/GetRFQPartCostComparisonReport',
                method: 'Post',
                data: Paging
            });
        },
        exportRFQPartCostComparisonReport: function (Paging) {
            return common.$http({
                url: urlCore + '/exportRFQPartCostComparisonReport',
                method: "Post",
                data: Paging
            });
        },
        // End RFQ Part Cost Comparison Report

        // start RFQs Quoted By Supplier
        GetRFQQuotedBySupplierReport: function (Paging) {
            return common.$http({
                url: urlCore + '/GetRFQQuotedBySupplierReport',
                method: 'Post',
                data: Paging
            });
        },
        exportRFQQuotedBySupplierReport: function (Paging) {
            return common.$http({
                url: urlCore + '/exportRFQQuotedBySupplierReport',
                method: "Post",
                data: Paging
            });
        },
        // End RFQs Quoted By Supplier

        // start RFQ Supplier list report
        GetRFQSupplierReport: function (Paging) {
            return common.$http({
                url: urlCore + '/GetRFQSupplierReport',
                method: 'Post',
                data: Paging
            });
        },
        exportRFQSupplierReport: function (Paging) {
            return common.$http({
                url: urlCore + '/exportRFQSupplierReport',
                method: "Post",
                data: Paging
            });
        },
        // End RFQ Supplier list report

        // start RFQ Supplier Activity Level Report
        GetRFQSupplierActivityLevelReport: function (Paging) {
            return common.$http({
                url: urlCore + '/GetRFQSupplierActivityLevelReport',
                method: 'Post',
                data: Paging
            });
        },
        exportRFQSupplierActivityLevelReport: function (Paging) {
            return common.$http({
                url: urlCore + '/exportRFQSupplierActivityLevelReport',
                method: "Post",
                data: Paging
            });
        },
        // End RFQ Supplier Activity Level Report

        // start open RFQ report
        GetOpenRFQsReport: function (Paging) {
            return common.$http({
                url: urlCore + '/GetOpenRFQsReport',
                method: 'Post',
                data: Paging
            });
        },
        exportOpenRFQsReport: function (Paging) {
            return common.$http({
                url: urlCore + '/exportOpenRFQsReport',
                method: "Post",
                data: Paging
            });
        },
        // End open RFQ report

        // start Detailed Supplier Quote report    
        GetRFQDetailedSupplierReport: function (Paging) {
            return common.$http({
                url: urlCore + '/GetDetailedSupplierQuoteReport',
                method: 'Post',
                data: Paging
            });
        },
        GetRfqPartQuoteDetailsReport: function (Paging) {
            return common.$http({
                url: urlCore + '/GetRfqPartQuoteDetailsReport',
                method: 'Post',
                data: Paging
            });
        },
        exportRFQDetailedSupplierReport: function (Paging) {
            return common.$http({
                url: urlCore + '/exportDetailedSupplierQuoteReport',
                method: "Post",
                data: Paging
            });
        },
        // End Detailed Supplier Quote report
    }
}]);