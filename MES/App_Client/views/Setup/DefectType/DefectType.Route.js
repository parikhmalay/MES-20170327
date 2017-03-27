function getRoutes() {
    return [
       {
           url: '/',
           config: { templateUrl: '/App_Client/views/Setup/DefectType/DefectTypeList.html?v=' + Version, areaName: 'Setup/DefectType' }
       },
       {
           url: '/Role',
           config: { templateUrl: '/App_Client/views/Setup/DefectType/Role.html?v=' + Version, areaName: 'Setup/DefectType' }
       },
       {
           url: '/LoginHtml',
           config: { templateUrl: '/App_Client/views/Setup/DefectType/LoginHtml.html?v=' + Version, areaName: 'Setup/DefectType' }
       },
    ]
}
