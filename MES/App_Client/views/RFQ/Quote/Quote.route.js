function getRoutes() {
    return [
       {
           url: '/',
           config: { templateUrl: '/App_Client/views/RFQ/Quote/QuoteList.html?v=' + Version, areaName: 'RFQ/Quote/Quote' }
       },
       {
           url: '/AddEdit/',
           config: { templateUrl: '/App_Client/views/RFQ/Quote/AddEditQuote.html?v=' + Version, areaName: 'RFQ/Quote/Quote' }
       },     
       {
           url: '/AddEdit/:Id/:IsR',
           config: { templateUrl: '/App_Client/views/RFQ/Quote/AddEditQuote.html?v=' + Version, areaName: 'RFQ/Quote/Quote' }
       },
    ]
}