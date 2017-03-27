function getRoutes() {
    return [
       {
           url: '/',
           config: { templateUrl: '/App_Client/views/RFQ/SupplierQuote/SupplierQuote.html?v=' + Version, areaName: 'RFQ/RFQ/SupplierQuote' }
       },
       {
           url: '/:RFQId/:SupplierId',
           config: { templateUrl: '/App_Client/views/RFQ/SupplierQuote/SupplierQuote.html?v=' + Version, areaName: 'RFQ/RFQ/SupplierQuote' }
       },
       {
           url: '/tempSupplierQuote',
           config: { templateUrl: '/App_Client/views/RFQ/SupplierQuote/SupplierQuoteDQ.html?v=' + Version, areaName: 'RFQ/RFQ/SupplierQuote' }
       },
    ]
}