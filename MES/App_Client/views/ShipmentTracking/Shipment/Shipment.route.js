function getRoutes() {
    return [
        {
            url: '/', config: { templateUrl: '/App_Client/views/ShipmentTracking/Shipment/ShipmentList.html?v=' + Version, areaName: 'ShipmentTracking/Shipment' }
        },
        {
            url: '/AddEdit/',
            config: { templateUrl: '/App_Client/views/ShipmentTracking/Shipment/AddEditShipment.html?v=' + Version, areaName: 'ShipmentTracking/Shipment' }
        },
       {
           url: '/AddEdit/:Id',
           config: { templateUrl: '/App_Client/views/ShipmentTracking/Shipment/AddEditShipment.html?v=' + Version, areaName: 'ShipmentTracking/Shipment' }
       },
    ];
}