function getRoutes() {
    return [
       {
           url: '/',
           config: { templateUrl: '/App_Client/views/Setup/EmailTemplate/EmailTemplateList.html?v=' + Version, areaName: 'Setup/EmailTemplate' }
       },
    ]
}
