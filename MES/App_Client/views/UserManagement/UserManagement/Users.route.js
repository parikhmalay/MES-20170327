function getRoutes() {
    return [
        {
            url: '/', config: { templateUrl: '/App_Client/views/UserManagement/UserManagement/UserList.html?v=' + Version, areaName: 'UserManagement/UserManagement' }
        },
       {
           url: '/AddEdit/',
           config: { templateUrl: '/App_Client/views/UserManagement/UserManagement/AddEditUser.html?v=' + Version, areaName: 'UserManagement/UserManagement' }
       },
       {
           url: '/AddEdit/:Id',
           config: { templateUrl: '/App_Client/views/UserManagement/UserManagement/AddEditUser.html?v=' + Version, areaName: 'UserManagement/UserManagement' }
       },
    ];
}