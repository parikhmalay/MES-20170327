function getRoutes() {
    return [
       {
           url: '/',
           config: { templateUrl: '/App_Client/views/APQP/CAPA/CAPAList.html?v=' + Version, areaName: 'APQP/APQP/CAPA' }
       },     
       {
           url: '/CAPAList/',
           config: { templateUrl: '/App_Client/views/APQP/CAPA/CAPAList.html?v=' + Version, areaName: 'APQP/APQP/CAPA' }
       },
       {
           url: '/AddEdit/',
           config: { templateUrl: '/App_Client/views/APQP/CAPA/AddEditCAPA.html?v=' + Version, areaName: 'APQP/APQP/CAPA' }
       },
        {
            url: '/AddEdit/:Id',
            config: { templateUrl: '/App_Client/views/APQP/CAPA/AddEditCAPA.html?v=' + Version, areaName: 'APQP/APQP/CAPA' }
        },
    ]
}