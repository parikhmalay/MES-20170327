function getRoutes() {
    return [
       {
           url: '/',
           config: { templateUrl: '/App_Client/views/RFQ/RFQ/RFQList.html?v=' + Version, areaName: 'RFQ/RFQ/RFQ' }
       },
       {
           url: '/AddEdit/',
           config: { templateUrl: '/App_Client/views/RFQ/RFQ/AddEditRFQ.html?v=' + Version, areaName: 'RFQ/RFQ/RFQ' }
       },
       {
           url: '/AddEdit/:Id/:IsR',
           config: { templateUrl: '/App_Client/views/RFQ/RFQ/AddEditRFQ.html?v=' + Version, areaName: 'RFQ/RFQ/RFQ' }
       },
    ]
}