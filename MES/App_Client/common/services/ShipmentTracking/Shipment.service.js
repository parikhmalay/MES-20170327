app.factory('ShipmentSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'ShipmentApi/';
    var url = $resource(urlCore + '/:id',
            {
                id: '@id'
            },
            {
                'save': {
                    method: 'Post',
                    url: urlCore + '/Post'
                },
                'update': {
                    method: 'Put',
                    url: urlCore + '/Put'
                },
                'search': {
                    method: 'Post',
                    url: urlCore + '/Search'
                },
                'get': {
                    method: 'Get',
                    url: urlCore + '/Get/:id'
                },
                'delete': {
                    method: 'Delete',
                    url: urlCore + '/Delete/:id'
                },
            });
    return {
        GetShipmentList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetShipmentList',
                method: "Post",
                data: Paging
            });
        },
        Save: function (shipment) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: shipment
            });
        },
        Delete: function (shipmentId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { shipmentId: shipmentId }
            });
            return promise;
        },
        getData: function (Id) {
            var promise = common.$http({
                url: urlCore + "/Get",
                method: "Get",
                params: { Id: Id }
            });
            return promise;
        },
        downloadShipmentFile: function () {
            return common.$http({
                url: urlCore + '/DownloadShipment',
                method: "Post"
            });
        },
        uploadShipmentFile: function (filePath) {
            return common.$http({
                url: urlCore + '/UploadShipment',
                method: "Post",
                params: { filePath: filePath }
            });
        },
        exportToExcelShipmentReport: function (Paging) {
            return common.$http({
                url: urlCore + '/exportToExcelShipmentReport',
                method: "Post",
                data: Paging
            });
        },

    }
}]);