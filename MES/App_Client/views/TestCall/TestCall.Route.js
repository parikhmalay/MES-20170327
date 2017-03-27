function getRoutes() {
    return [
       {
           url: '/',
           config: { templateUrl: '/App_Client/views/TestCall/TestCallList.html?v=' + Version, areaName: 'TestCall/TestCall' }
       },
        {
            url: '/Edit:TestId',
            config: { templateUrl: '/App_Client/views/TestCall/AdddEdit.html?v=' + Version, areaName: 'TestCall/TestCall' }
        },
    ]
}
 