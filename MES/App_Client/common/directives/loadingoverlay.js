var app = angular.module('app');

//Modal Group for Event Change
var ModalInstanceAccessDeniedCtrl = function ($scope, common, $modalInstance, ErrorCode) {
    $scope.ErrorCode = ErrorCode;
    $scope.cancel = function () {
        common.$rootScope.ForbiddenIsopen = false;
        $modalInstance.dismiss('cancel');
    }
}
ModalInstanceAccessDeniedCtrl.$inject = ['$scope', 'common', '$modalInstance', 'ErrorCode'];



app.directive('cstLoadingOverlay', ['$timeout', '$q', 'httpInterceptor', 'common', '$modal', function ($timeout, $q, httpInterceptor, common, $modal) {
    return {
        restrict: 'E',
        link: function (scope, element, attribute) {
            var requestQueue = [];

            scope.openErrorPopup = function (errorCode) {
                var modalInstanceAccessDenied = $modal.open({
                    templateUrl: '/App_Client/views/AccessDenied.html?v=' + Version,
                    controller: ModalInstanceAccessDeniedCtrl,
                    resolve: {
                        ErrorCode: function () {
                            return errorCode;
                        }
                    },
                    sizeclass: 'modal-lg HandleError',
                    scope: false,
                    keyboard: false,
                    backdrop: false,
                });
                modalInstanceAccessDenied.result.then(function (result) {
                }, function () {
                });
            }

            httpInterceptor.responseError = function (response) {
                //Bad Request
                if (response.status == 400) {
                    if (response.data.ModelState) {
                        var ErrorList = [];
                        angular.forEach(response.data.ModelState, function (V, K) { ErrorList.push(V); });
                        if (ErrorList.join('\n') != "")
                            common.aaNotify.danger(ErrorList.join('\n'));
                    }
                } //Not Found
                else if (response.status == '412' || response.status == 412) {
                    if (response.data) {
                        var ErrorList = [];
                        angular.forEach(response.data, function (V) { ErrorList.push(V.Message); });
                        if (ErrorList.join('\n') != "")
                            common.aaNotify.danger(ErrorList.join('\n'));
                    }
                }
                else if (response.status == 404 && response.data) {
                    common.aaNotify.error('Oops! There is a server side error. Try again later.');
                    scope.openErrorPopup(404);
                }
                    //Forbidden
                else if (response.status == 403) {
                    common.aaNotify.error('Oops! There is a server side error. Try again later.');
                    scope.openErrorPopup(403);
                }//Internal Server Error
                else if (response.status == 500) {
                    common.aaNotify.error('Oops! There is a server side error. Try again later.');
                    scope.openErrorPopup(500);
                }
                else if (response.status == 401) {
                    common.aaNotify.error('Oops! There is a server side error. Try again later.');
                }
                    //Only when client info is not available.
                else if (response.status == 302) {
                    common.aaNotify.error('Oops! There is a server side error. Try again later.');
                }
                else {
                    if (response != undefined && response != null && response.data !=undefined && response.data !=null && response.data.Message != undefined && response.data.Message != null && response.data.Message != '')
                        common.aaNotify.error(response.data.Message);
                    else
                        common.aaNotify.error('Oops! There is a server side error. Try again later.');
                }
                return $q.reject(response);
            };
        }
    };
}]);


app.factory('httpInterceptor', function () {
    return {

    };
});


////Http call for every request/response/error
////$provide.factory('httpInterceptorCall', ['$q', function ($q) {

////    return {
////        request: function (config) {
////            //console.log('request: ' + config.url);

////            return config || $q.when(config);
////        },
////        response: function (response) {
////            //console.log('response: ' + response.config.url);

////            return response || $q.when(response);
////        },
////        requestError: function (rejectReason) {

////        },
////        responseError: function (response) {

////            if (response.data.ModelState) {
////                ErrorList = [];

////                angular.forEach(response.data.ModelState, function (V, K) { ErrorList.push(V); });

////                console.log(ErrorList.join('\n'));

////                //common.aaNotify.danger(ErrorList.join('\n'))
////                //var logError = common.logger.getLogFn('', 'error');

////                //logError(ErrorList.join('\n'), null, true);  //Display Server Error Message
////            }

////            return $q.reject(response);
////        }
////    };
////}]);