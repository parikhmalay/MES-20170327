function getRoutes() {
    return [
       {
           url: '/:UniqueUrl',
           config: { templateUrl: '/App_Client/views/RFQ/DQ/SubmitQuote.html?v=' + Version, areaName: 'RFQ/RFQ/DetailQuote' }
       },
       {
           url: '/AddEdit/:UniqueUrl',
           config: { templateUrl: '/App_Client/views/RFQ/DQ/SubmitQuote.html?v=' + Version, areaName: 'RFQ/RFQ/DetailQuote' }
       },
       {
           url: '/UploadQuote/:UniqueUrl',
           config: { templateUrl: '/App_Client/views/RFQ/DQ/UploadSubmitQuote.html?v=' + Version, areaName: 'RFQ/RFQ/DetailQuote' }
       },
    ]
}