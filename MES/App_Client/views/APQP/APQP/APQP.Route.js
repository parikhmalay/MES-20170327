function getRoutes() {
    return [
       {
           url: '/',
           config: { templateUrl: '/App_Client/views/APQP/APQP/APQPDashboard.html?v=' + Version, areaName: 'APQP/APQP/APQP' }
       },
         {
             url: '/APQPDashboard/',
             config: { templateUrl: '/App_Client/views/APQP/APQP/APQPDashboard.html?v=' + Version, areaName: 'APQP/APQP/APQP' }
         },
        {
            url: '/APQPDashboard/:CallFrom',
            config: { templateUrl: '/App_Client/views/APQP/APQP/APQPDashboard.html?v=' + Version, areaName: 'APQP/APQP/APQP' }
        },
         {
             url: '/AddEdit/',
             config: { templateUrl: '/App_Client/views/APQP/APQP/AddEditAPQP.html?v=' + Version, areaName: 'APQP/APQP/APQP' }
         },
        {
            url: '/AddEdit/:Id',
            config: { templateUrl: '/App_Client/views/APQP/APQP/AddEditAPQP.html?v=' + Version, areaName: 'APQP/APQP/APQP' }
        },
        {
            url: '/AddEdit/:Id/:APQPStep',
            config: { templateUrl: '/App_Client/views/APQP/APQP/AddEditAPQP.html?v=' + Version, areaName: 'APQP/APQP/APQP' }
        },
       {
           url: '/APQPItemList/:CallFrom',
           config: { templateUrl: '/App_Client/views/APQP/APQP/APQPList.html?v=' + Version, areaName: 'APQP/APQP/APQP' }
       },
       {
           url: '/APQPReports/',
           config: { templateUrl: '/App_Client/views/APQP/APQP/APQPReports.html?v=' + Version, areaName: 'APQP/APQP/APQP' }
       },
       {
           url: '/DefectTrackingList/',
           config: { templateUrl: '/App_Client/views/APQP/APQP/DefectTrackingList.html?v=' + Version, areaName: 'APQP/APQP/DefectTracking' }
       },
       {
           url: '/ChangeRequestForm/',
           config: { templateUrl: '/App_Client/views/APQP/APQP/ChangeRequestForm.html?v=' + Version, areaName: 'APQP/APQP/ChangeRequest' }
       },
    ]
}