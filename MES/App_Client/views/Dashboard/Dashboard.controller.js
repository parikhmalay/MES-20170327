app.controller('DashboardSummaryCtrl', ['$scope', 'common', 'DashboardSvc', '$window', function ($scope, common, DashboardSvc, $window) {
    //Start implement security role wise
    $scope.setRoleWisePrivilege = function () {
        $scope.currentSecurityObject = currentSecurityObject;
        if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
            angular.forEach($scope.currentSecurityObject, function (obj, index) {
                switch (obj.ObjectId) {
                    case 126:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                RedirectToAccessDenied();
                                break;                           
                        }
                        break;                   
                    default:
                        break;
                }
            });
        }
        else {
            RedirectToAccessDenied();
        }
    }
    //End implement security role wise
    $scope.setRoleWisePrivilege();

    $scope.DashBoardSummary = {};
    $scope.SearchCriteria = { DashDateFrom: null, DashDateTo: null };

    $scope.Init = function () {
        if (localStorage.getItem("DateFrom"))
            localStorage.removeItem("DateFrom");
        if (localStorage.getItem("DateTo"))
            localStorage.removeItem("DateTo");

        var currentDate = new Date();
        $scope.DateFrom = new Date(currentDate.getFullYear(), 0, 1);
        $scope.DateTo = currentDate;
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.SearchCriteria.DashDateFrom = $scope.DateFrom;
        $scope.SearchCriteria.DashDateTo = $scope.DateTo;
        $scope.searchDashboardData();
    };
    $scope.searchDashboardData = function () {
        $scope.DashBoardSummary = {};
        common.usSpinnerService.spin('spnDashboard');
        $scope.InitializeHighChartForQuoteDoneAndAnalysisReport();
        $scope.SearchCriteria.DateFrom = convertUTCDateToLocalDate($scope.SearchCriteria.DashDateFrom);
        $scope.SearchCriteria.DateTo = convertUTCDateToLocalDate($scope.SearchCriteria.DashDateTo);
        $scope.Paging.Criteria = $scope.SearchCriteria;
        DashboardSvc.GetDashboardSummary($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnDashboard');
                 if (response.data.StatusCode == 200) {
                     $scope.DashBoardSummary = response.data.Result;

                     ///RFQ analysis report chart
                     var rndAnalysisData = [];
                     $scope.highchartsNGAnalysisData.series = [];
                     if ($scope.DashBoardSummary.lstRFQAnalysisChart.length > 0) {
                         angular.forEach($scope.DashBoardSummary.lstRFQAnalysisChart, function (o, index) {
                             rndAnalysisData.push({ name: o.DisplayName, y: o.NoOfRfq });
                         });
                     }
                     $scope.highchartsNGAnalysisData.series.push({
                         name: 'RFQ',
                         colorByPoint: true,
                         data: rndAnalysisData
                     });
                     ///End RFQ analysis report chart

                     ///Quotes done report chart
                     var rndQuotesDone = [];
                     $scope.highchartsNGQuotesDone.series = [];
                     if ($scope.DashBoardSummary.lstQuotesDoneChart.length > 0) {
                         angular.forEach($scope.DashBoardSummary.lstQuotesDoneChart, function (o, index) {
                             rndQuotesDone.push({ name: o.DisplayName, y: o.Amount });
                         });
                     }
                     $scope.highchartsNGQuotesDone.series.push({
                         name: 'Quote',
                         colorByPoint: true,
                         data: rndQuotesDone
                     });
                     ///End Quotes done report chart
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnDashboard');
             });
    };

    $scope.InitializeHighChartForQuoteDoneAndAnalysisReport = function () {
        $scope.highchartsNGAnalysisData = {
            chart: {
                renderTo: 'container'
            },
            title: {
                enabled: false,
                text: '',
            },
            tooltip: {
                pointFormat: '<b>{point.percentage:.1f}%</b>'
            },
            options: {
                chart: {
                    type: 'pie'
                }
            },
            colors: ["#00d388", "#3896ff", "#ff4d4d", "#ffc600", "#ac6cff", "#c2c2c2", "#5cd0c9", "#cd6767", "#575e6c", "#ff7048"],
            plotOptions: {
                pie: {
                    allowPointSelect: false,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: false,
                        format: '{point.percentage:.1f} %',
                        style: {
                            color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                        }
                    },
                    showInLegend: true
                }
            },
            series: [],
            size: {
                height: 350
            },
            loading: false,
            legend: {
                enabled: true
            },
            exporting: {
                enabled: false
            }            
        };

        $scope.highchartsNGQuotesDone = {
            chart: {
                renderTo: 'container'
            },
            title: {
                enabled: false,
                text: '',
            },
            tooltip: {
                //pointFormat: '<b>${point.y:.3f}</b>'
                pointFormat: '<b></b>'
            },
            options: {
                chart: {
                    type: 'pie'
                }
            },
            colors: ["#00d388", "#3896ff", "#ff4d4d", "#ffc600", "#ac6cff", "#c2c2c2", "#5cd0c9", "#cd6767", "#575e6c", "#ff7048"],
            plotOptions: {
                pie: {
                    allowPointSelect: false,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: false,
                        format: '{point.percentage:.1f} %',
                        style: {
                            color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                        }
                    },
                    showInLegend: true
                }
            },
            series: [],
            size: {
                height: 350
            },
            loading: false,
            legend: {
                enabled: true
            },
            exporting: {
                enabled: false
            }
        };
    };

    $scope.RedirectToList = function (pageName) {
        localStorage.setItem("DateFrom", $scope.SearchCriteria.DashDateFrom);
        localStorage.setItem("DateTo", $scope.SearchCriteria.DashDateTo);
        if (pageName == 'RFQ')
            $window.location.href = "/RFQ/RFQ/RFQ#/";
        else if (pageName == 'QuoteToCustomer')
            $window.location.href = "/RFQ/Quote/Quote#/";
        else if (pageName == 'APQP')
            $window.location.href = "/APQP/APQP#/APQPItemList/CallFromDashboard";
        else if (pageName == 'DefectTracking')
            $window.location.href = "/APQP/DefectTracking#/DefectTrackingList/";
    };
    $scope.Init();
}]);