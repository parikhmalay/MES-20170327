function getRoutes() {
    return [
        {
            url: '/', config: { templateUrl: '/App_Client/views/RoleManagement/Roles.html?v=' + Version, areaName: 'RoleManagement/RoleManagement' }
        },
        {
            url: '/AddEdit/',
            config: { templateUrl: '/App_Client/views/RoleManagement/AddEditRole.html?v=' + Version, areaName: 'RoleManagement/RoleManagement' }
        },
       {
           url: '/AddEdit/:Id',
           config: { templateUrl: '/App_Client/views/RoleManagement/AddEditRole.html?v=' + Version, areaName: 'RoleManagement/RoleManagement' }
       },
    ];
}