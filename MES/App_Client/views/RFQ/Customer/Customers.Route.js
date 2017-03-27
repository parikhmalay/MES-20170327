function getRoutes() {
    return [
       {
           url: '/',
           config: { templateUrl: '/App_Client/views/RFQ/Customer/CustomersList.html?v=' + Version, areaName: 'RFQ/Customer/Customers' }
       },
       {
           url: '/AddEdit/',
           config: { templateUrl: '/App_Client/views/RFQ/Customer/AddEditCustomers.html?v=' + Version, areaName: 'RFQ/Customer/Customers' }
       },
       {
           url: '/AddEdit/:Id',
           config: { templateUrl: '/App_Client/views/RFQ/Customer/AddEditCustomers.html?v=' + Version, areaName: 'RFQ/Customer/Customers' }
       },
    ]
}