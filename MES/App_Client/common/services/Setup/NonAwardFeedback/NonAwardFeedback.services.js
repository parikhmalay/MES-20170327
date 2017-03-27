app.factory('NonAwardFeedbackSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;
    var urlCore = $rootScope.baseAdminUrl + 'NonAwardFeedbackApi/';
    console.log(urlCore);
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
            }
        });
    return {

        GetNonAwardFeedbackList: function (Paging) {

            return common.$http({
                url: urlCore + '/GetNonAwardFeedbackList',
                method: "POST",
                data: Paging
            });
        },
        Search: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetNonAwardFeedbacks',
                method: "POST",
                data: Parameters
            });
        },
        Save: function (NonAwardFeedback) {
            return common.$http({
                url: urlCore + '/Save',
                method: "Post",
                data: NonAwardFeedback
            });
        },

        GetInfo: function () {
            var promise = common.$http({
                url: urlCore + '/GetInfo',
                method: "get"
            });
            return promise;
        },
        Delete: function (NonAwardFeedbackId) {
            var promise = common.$http({
                url: urlCore + "/Delete",
                method: "Post",
                params: { NonAwardFeedbackId: NonAwardFeedbackId }
            });
            return promise;
        }
    }
}]);