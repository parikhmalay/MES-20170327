var app = angular.module('app');
app.factory('AuditLogsSvc', ['common', '$rootScope', function (common, $rootScope) {
    $resource = common.$resource;

    var urlCore = $rootScope.baseAdminUrl + 'AuditLogsApi/';
    return {
        GetAuditLogs: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetAuditLogs',
                method: "POST",
                data: Parameters
            });
        },
        GetAuditLogDefectTracking: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetAuditLogDefectTracking',
                method: "POST",
                data: Parameters
            });
        },
        GetAuditLogDefectTrackingDetails: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetAuditLogDefectTrackingDetails',
                method: "POST",
                data: Parameters
            });
        },

        GetAPQPChangeLogs: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetAPQPChangeLogs',
                method: "POST",
                data: Parameters
            });
        },
        GetChangeRequestLogs: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetChangeRequestLogs',
                method: "POST",
                data: Parameters
            });
        },
        GetAuditLogCAPA: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetAuditLogCAPA',
                method: "POST",
                data: Parameters
            });
        },
        GetAuditLogCAPAAffectedPartDetails: function (Parameters) {
            return common.$http({
                url: urlCore + '/GetAuditLogCAPAAffectedPartDetails',
                method: "POST",
                data: Parameters
            });
        },
    }
}]);
