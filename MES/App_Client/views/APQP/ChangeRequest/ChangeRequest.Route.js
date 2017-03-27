function getRoutes() {
    return [
       {
           url: '/',
           config: { templateUrl: '/App_Client/views/APQP/ChangeRequest/ChangeRequestDashboard.html?v=' + Version, areaName: 'APQP/APQP/ChangeRequest' }
       },
       {
           url: '/ChangeManagementDashboard/',
           config: { templateUrl: '/App_Client/views/APQP/ChangeRequest/ChangeRequestDashboard.html?v=' + Version, areaName: 'APQP/APQP/ChangeRequest' }
       },
       {
           url: '/AddEdit/',
           config: { templateUrl: '/App_Client/views/APQP/ChangeRequest/AddEditCR.html?v=' + Version, areaName: 'APQP/APQP/ChangeRequest' }
       },
        {
            url: '/AddEdit/:CRId/:APQPItemId',
            config: { templateUrl: '/App_Client/views/APQP/ChangeRequest/AddEditCR.html?v=' + Version, areaName: 'APQP/APQP/ChangeRequest' }
        },
        {
            url: '/ChangeRequestList/',
            config: { templateUrl: '/App_Client/views/APQP/ChangeRequest/ChangeRequestList.html?v=' + Version, areaName: 'APQP/APQP/ChangeRequest' }
        },
        {
            url: '/ChangeRequestList/:CallFrom',
            config: { templateUrl: '/App_Client/views/APQP/ChangeRequest/ChangeRequestList.html?v=' + Version, areaName: 'APQP/APQP/ChangeRequest' }
        },
    ]
}