function getRoutes() {
    return [
       {
           url: '/',
           config: { templateUrl: '/App_Client/views/APQP/DocumentManagement/DocumentManagement.html?v=' + Version, areaName: 'APQP/APQP/DocumentManagement' }
       },
        {
            url: '/DocumentManagement/',
            config: { templateUrl: '/App_Client/views/APQP/DocumentManagement/DocumentManagement.html?v=' + Version, areaName: 'APQP/APQP/DocumentManagement' }
        },
    ]
}