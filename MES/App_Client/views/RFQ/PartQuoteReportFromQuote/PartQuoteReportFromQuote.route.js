function getRoutes() {
    return [
        {
            url: '/:Id', config: { templateUrl: '/App_Client/views/RFQ/PartQuoteReportFromQuote/RFQPartQuoteReport.html?v=' + Version, areaName: 'RFQ/RFQ/Reports' }
        }
    ];
}