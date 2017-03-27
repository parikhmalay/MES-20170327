var app = angular.module('app');
app.factory('SupplierAssessmentSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'SupplierAssessmentApi/';
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
        GetSupplierAssessmentList: function (Paging) {
            return common.$http({
                url: urlCore + '/GetSupplierAssessmentList',
                method: "POST",
                data: Paging
            });
        },
        GetAssessmentList: function (supplierId) {
            return common.$http({
                url: urlCore + '/GetSupplierAssessmentList',
                method: "POST",
                params: { supplierId: supplierId }
            });
        },
        Save: function (supplierObj) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: supplierObj
            });
        },
        Delete: function (assessmentId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { assessmentId: assessmentId }
            });
            return promise;
        },
        getData: function (Id) {
            var promise = common.$http({
                url: urlCore + "/GetAssessment",
                method: "Get",
                params: { Id: Id }
            });
            return promise;
        },
        CreateRevision: function (assessmentId) {
            return common.$http({
                url: urlCore + '/CreateRevision',
                method: "Post",
                params: { assessmentId: assessmentId }
            });
        },
        GetAssessmentDetail: function (id) {
            var promise = common.$http({
                url: urlCore + "/GetAssessmentDetail",
                method: "Get",
                params: { id: id }
            });
            return promise;
        },
        ViewInstruction:function(){
            var promise = common.$http({
                url: urlCore + "/ViewInstruction",
                method: "Get"
            });
            return promise;
        }
    }
}]);
