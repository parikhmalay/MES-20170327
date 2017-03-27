function getRoutes() {
    return [
       {
           url: '/',
           config: { templateUrl: '/App_Client/views/Setup/Destination/DestinationList.html?v=' + Version, areaName: 'Setup/Destination' }
       },
        {
            url: '/Edit:id',
            config: { templateUrl: '/App_Client/views/Setup/Destination/AdddEdit.html?v=' + Version, areaName: 'Destination/Destination' }
        },
    ]
}
 