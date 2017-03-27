app.directive('auditLog', ['$http', '$templateCache', '$parse', '$compile',
    function ($http, $templateCache, $parse, $compile) {
        var compiler = function (tElement, tAttrs) {
            return function (scope, element, attrs) {
                scope.$watch("TransactionId", function (newValue, oldValue) {
                    var tplURL = '/App_Client/common/directives/AuditLog/AuditLog.html';
                    templateLoader = $http.get(tplURL, { cache: $templateCache }).success(function (html) {
                        tElement.html(html);
                    });
                    templateLoader.then(function (templateText) {
                        element.html($compile(tElement.html())(scope));
                    });
                });
            };
        };
        return {
            restrict: 'E',
            replace: true,
            scope: {
                TransactionId: '@transactionid',
                TableName: '@tablename',
                SchemaName: '@schemaname',
                SpinnerKeyName: '@spinnerkeyname',
            },
            compile: compiler,
            controller: function ($scope, $element, common, AuditLogsSvc) {
                //code for get audit log details
                $scope.InitializeAuditLogObjects = function () {
                    if ($scope.TransactionId != undefined && $scope.TransactionId != null && $scope.TransactionId > 0) {
                        $scope.AuditLogPaging = GetDefaultPageObject();
                        $scope.AuditLogSearchCriteria = { ItemId: $scope.TransactionId, tableName: $scope.TableName, schemaName: $scope.SchemaName };
                        $scope.AuditLogPaging.Criteria = $scope.AuditLogSearchCriteria;
                        $scope.ViewAuditLogs();
                    }
                }
                $scope.ViewAuditLogs = function () {
                    common.usSpinnerService.spin($scope.SpinnerKeyName);
                    AuditLogsSvc.GetAuditLogs($scope.AuditLogPaging).then(
                         function (response) {
                             common.usSpinnerService.stop($scope.SpinnerKeyName);
                             if (response.data.StatusCode == 200) {
                                 $scope.AuditLogsList = response.data.Result;
                                 $scope.AuditLogPaging = response.data.PageInfo;
                                 $scope.AuditLogsList = _.groupBy($scope.AuditLogsList, 'UpdatedOn');
                                 $scope.IsAccordionObjectEmpty = IsObjectEmpty($scope.AuditLogsList);
                             }
                             else {
                                 $scope.IsAccordionObjectEmpty = true;
                                 console.log(response.data.ErrorText);
                             }
                         },
                         function (error) {
                             $scope.IsAccordionObjectEmpty = true;
                             common.usSpinnerService.stop($scope.SpinnerKeyName);
                             //common.aaNotify.error(error);
                         });
                };
                $scope.auditLogPageSizeChanged = function (PageSize) {
                    $scope.AuditLogPaging.PageSize = PageSize;
                    $scope.ViewAuditLogs();
                };
                $scope.auditLogPageChanged = function (PageNo) {
                    $scope.AuditLogPaging.PageNo = PageNo;
                    $scope.ViewAuditLogs();
                };
                $scope.InitializeAuditLogObjects();


                // code for get audit logs end here
            }
        };


    }]);
