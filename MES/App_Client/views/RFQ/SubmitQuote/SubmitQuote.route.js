function getRoutes() {
    return [
       {
           url: '/:UniqueUrl',
           config: { templateUrl: '/App_Client/views/RFQ/SubmitQuote/SubmitQuote.html?v=' + Version, areaName: 'RFQ/RFQ/SubmitQuote' }
       },
       {
           url: '/AddEdit/:UniqueUrl',
           config: { templateUrl: '/App_Client/views/RFQ/SubmitQuote/SubmitQuote.html?v=' + Version, areaName: 'RFQ/RFQ/SubmitQuote' }
       },
       {
           url: '/UploadQuote/:UniqueUrl',
           config: { templateUrl: '/App_Client/views/RFQ/SubmitQuote/UploadSubmitQuote.html?v=' + Version, areaName: 'RFQ/RFQ/SubmitQuote' }
       },
    ]
}