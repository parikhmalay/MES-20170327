function getRoutes() {
    return [
       {
           url: '/',
           config: { templateUrl: '/App_Client/views/APQP/DefectTracking/DefectTrackingList.html?v=' + Version, areaName: 'APQP/APQP/DefectTracking' }
       },
       {
           url: '/DefectTrackingReport/',
           config: { templateUrl: '/App_Client/views/APQP/DefectTracking/DefectTrackingReport.html?v=' + Version, areaName: 'APQP/APQP/DefectTracking' }
       },
       {
           url: '/DefectTrackingList/',
           config: { templateUrl: '/App_Client/views/APQP/DefectTracking/DefectTrackingList.html?v=' + Version, areaName: 'APQP/APQP/DefectTracking' }
       },
       {
           url: '/AddEdit/',
           config: { templateUrl: '/App_Client/views/APQP/DefectTracking/AddEditDT.html?v=' + Version, areaName: 'APQP/APQP/DefectTracking' }
       },
        {
            url: '/AddEdit/:Id',
            config: { templateUrl: '/App_Client/views/APQP/DefectTracking/AddEditDT.html?v=' + Version, areaName: 'APQP/APQP/DefectTracking' }
        },
    ]
}