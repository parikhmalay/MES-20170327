function getRoutes() {
    return [
       {
           url: '/',
           config: { templateUrl: '/App_Client/views/Setup/CoatingType/CoatingTypeList.html?v=' + Version, areaName: 'Setup/CoatingType' }
       },
       {
           url: '/Role',
           config: { templateUrl: '/App_Client/views/Setup/CoatingType/Role.html?v=' + Version, areaName: 'Setup/CoatingType' }
       },
       {
           url: '/LoginHtml',
           config: { templateUrl: '/App_Client/views/Setup/CoatingType/LoginHtml.html?v=' + Version, areaName: 'Setup/CoatingType' }
       },
    ]
}
