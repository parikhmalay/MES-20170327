function getRoutes() {
    return [
       {
           url: '/',
           config: { templateUrl: '/App_Client/views/RFQ/Supplier/SuppliersList.html?v=' + Version, areaName: 'RFQ/Supplier/Suppliers' }
       },
       {
           url: '/AddEdit/',
           config: { templateUrl: '/App_Client/views/RFQ/Supplier/AddEditSuppliers.html?v=' + Version, areaName: 'RFQ/Supplier/Suppliers' }
       },
       {
           url: '/AddEdit/:Id',
           config: { templateUrl: '/App_Client/views/RFQ/Supplier/AddEditSuppliers.html?v=' + Version, areaName: 'RFQ/Supplier/Suppliers' }
       },
    ]
}
