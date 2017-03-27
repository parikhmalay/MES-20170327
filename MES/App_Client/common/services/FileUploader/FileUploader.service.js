var app = angular.module('app');
app.factory('FileUploaderSvc', ['common', '$rootScope', function (common, $rootScope) {

    var urlCore = $rootScope.baseUrl + '/FileUploaderApi/';
    return {
        Deletedocument: function (source) {
            return common.$http({
                url: urlCore + '/DeleteFile?source=' + source,
                method: "DELETE",
            });
        },
        uploadEditedFile: function (filePath) {
            return common.$http({
                url: urlCore + '/UploadEditedFile',
                method: "POST",
                params: { filePath: filePath },
            });
        },
    }
}]);

