function getRoutes() {
    return [
       {
           url: '/',
           config: { templateUrl: '/App_Client/views/Setup/ProjectCategory/ProjectCategoryList.html?v=' + Version, areaName: 'Setup/ProjectCategory' }
       },
        {
            url: '/Edit:id',
            config: { templateUrl: '/App_Client/views/Setup/ProjectCategory/AdddEdit.html?v=' + Version, areaName: 'ProjectCategory/ProjectCategory' }
        },
    ]
}
 