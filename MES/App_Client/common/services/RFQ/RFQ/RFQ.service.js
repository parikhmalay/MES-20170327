app.factory('RFQSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'RFQApi/';
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
        GetRFQList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetRFQList',
                method: "Post",
                data: Paging
            });
        },
        GetQuotesToCustomer: function (rfqId) {
            var promise = common.$http({
                url: urlCore + "/GetQuotesToCustomer",
                method: "Post",
                params: { rfqId: rfqId }
            });
            return promise;
        },
        GetQuotedSuppliers: function (rfqId) {
            var promise = common.$http({
                url: urlCore + "/GetQuotedSuppliers",
                method: "Post",
                params: { rfqId: rfqId }
            });
            return promise;
        },
        getQuoteToCustomerList: function (rfqId) {
            var promise = common.$http({
                url: urlCore + "/GetQuoteToCustomerList",
                method: "Get",
                params: { rfqId: rfqId }
            });
            return promise;
        },
        Save: function (rfq) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: rfq
            });
        },
        SaveRfqPart: function (rfqPart) {
            return common.$http({
                url: urlCore + '/SaveRfqPart',
                method: "Post",
                data: rfqPart
            });
        },
        Delete: function (rfqId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { rfqId: rfqId }
            });
            return promise;
        },
        DeleteRfqPart: function (rfqPartId) {
            var promise = common.$http({
                url: urlCore + "/DeleteRfqPart",
                method: "Post",
                params: { rfqPartId: rfqPartId }
            });
            return promise;
        },
        //Step - 1 & 2: Get RFQ & Parts Details By RFQ No.
        getData: function (Id, isR) {
            var promise = common.$http({
                url: urlCore + "/Get",
                method: "Get",
                params: { Id: Id, isR: isR }
            });
            return promise;
        },
        //Step - 2: Upload RFQParts File 
        UploadRFQParts: function (rfq) {
            return common.$http({
                url: urlCore + '/UploadRFQParts',
                method: "Post",
                data: rfq
            });
        },
        //Step - 3: Get Available Suppliers List ("Send To" Grid)
        GetAvailableSuppliersList: function (Paging) {

            return common.$http({
                url: urlCore + '/GetAvailableSuppliersList',
                method: "Post",
                data: Paging
            });
        },
        //Step - 3: Get RFQ Suppliers List ("Sent To" Grid)
        GetRFQSuppliers: function (Paging) {
            return common.$http({
                url: urlCore + '/GetRFQSuppliersList',
                method: "Post",
                data: Paging
            });
        },

        //Step - 3: Send RFQ to Suppliers (Simplified Quote)
        SendRFQToSuppliers: function (supplierData) {
            return common.$http({
                url: urlCore + '/SendRFQToSuppliers',
                method: "Post",
                data: supplierData
            });
        },
        //Step - 3: ReSend RFQ to RfqSuppliers (Simplified Quote)
        ResendRFQToSuppliers: function (rfqSupplierData) {
            return common.$http({
                url: urlCore + '/ResendRFQToRfqSuppliers',
                method: "Post",
                data: rfqSupplierData
            });
        },
        //Step - 3: Send Email To RFQ Suppliers
        SendEmail: function (EmailData) {
            return common.$http({
                url: urlCore + '/SendEmail',
                method: "Post",
                data: EmailData
            });
        },
        DeleteRFQSuppliers: function (Id) {
            var promise = common.$http({
                url: urlCore + "/DeleteRFQSuppliers",
                method: "Post",
                params: { Id: Id }
            });
            return promise;
        },
        //Step - 4: RFQ Closeout
        SendRFQCloseOut: function (EmailData) {
            return common.$http({
                url: urlCore + '/SendRFQCloseOutEmail',
                method: "Post",
                data: EmailData
            });
        },

    }
}]);