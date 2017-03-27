app.directive("fileSelect", ['common', '$upload', '$timeout', '$rootScope', function (common, $upload, $timeout, $rootScope) {
    var DDO = {
        restrict: 'E',
        scope: {
            fsModel: "=fsModel",
            fsButtonOnly: '@fsButtonOnly',
            fsButtonText: '@fsButtonText',
            fsButtonClass: '@fsButtonClass',
            fsMultiselect: '@fsMultiselect',
            fsClass: '@fsClass',
            fsUploadPath: '@fsUploadPath',
            fsExtensionValidationMsg: '@fsExtensionValidationMsg',
            fsExtension: '@fsExtension',
            fsMaxSize: '@fsMaxSize',
            fsSpinnderKey: '@fsSpinnderKey',
            fsSaveuploadedfiles: "&",
            fsDragText: "@fsDragText",
            fsFullDragArea: "@fsFullDragArea",
            fsDragHeight: "@fsDragHeight",
            fsShowExtentionLabel: "@fsShowExtentionLabel",
            fsNeedUpdateFileName: "@fsneedupdatefilename",
            fsFileUploadFolderName: "@fsFileUploadFolderName"
        },
        templateUrl: '/App_Client/common/directives/FileUploader/fileSelect.html?v=' + Version,
        link: function (scope, element, attr) {
            var _rootElement = element[0];
            var _eString = attr.fsExtension;
            var _fileExtensions = _eString.split(", ");
            var _btnOnly = attr.fsButtonOnly || '';
            scope.btnOnly = _btnOnly;
            attr.fsButtonOnly || 'false';
            _rootElement.onclick = function () {
                var _input = this.getElementsByTagName("input")[0];
                var _filesInfo = this.getElementsByClassName("file-Names")[0];
                _input.click();

            }

        },
        controller: function ($scope, $element, $attrs) {
            $scope.isValid = false;
            $scope.dragText =
            $scope.howToSend = 1;
            $scope.uploadRightAway = true;
            $scope.errorMsg = "";
            $scope.init = function () {
                if (typeof $scope.fsMaxSize == 'undefined') {
                    $scope.fsMaxSize = 2147483648;//4200000;
                }
                $scope.fsMaxSizeinMB = (($scope.fsMaxSize) / (1024 * 1024)).toFixed(2);

                $scope.dragText = $scope.fsDragText || "Click here OR Drop file here";
                $scope.fsFullDragArea = $scope.fsFullDragArea || 'true';
                $scope.howToSend = 1;
                $scope.uploadRightAway = true;
                $scope.btnOnly = $scope.fsButtonOnly || "false";
                $scope.errorMsg = "";
                if ($scope.fsButtonOnly == "true") {
                    $scope.cssClass = $scope.fsClass + ' onlyBtn';
                }
                if ($scope.fsMultiselect == "true")
                    $scope.MultiSelectAttrb = true;
                else
                    $scope.MultiSelectAttrb = false;
                if ($scope.fsDragHeight)
                    $scope.dragStyle = { 'height': $scope.fsDragHeight }
            }
            $scope.checkExtension = function (fileName, extentionArray, fileSize, maxFilesize) {
                var fileExt = fileName.substring(fileName.lastIndexOf("."), fileName.length);
                $scope.isValid = false;
                for (var i = 0; i < extentionArray.length; i++) {
                    if (fileName.toLowerCase().indexOf(extentionArray[i].toLowerCase()) > -1) {
                        $scope.isValid = true;
                        break;
                    }
                }

                if ($scope.isValid == true && fileSize > parseInt('2147483648')) {
                    $scope.isValid = false;
                    $scope.errorMsg = "Invalid File Size.";
                    //$scope.errorMsg = "Invalid File Size." + "....FS=" + fileSize + "....mFS=" + maxFilesize + "....isValid=" + $scope.isValid;
                }
                return $scope.isValid;
            }
            $scope.onFileSelect = function ($files, ImageFor) {
                if (typeof $scope.fsMaxSize == 'undefined') {
                    $scope.fsMaxSize = 2147483648;//4200000;
                }

                $scope.selectedFiles = [];
                $scope.progress = [];
                if ($scope.upload && $scope.upload.length > 0) {
                    for (var i = 0; i < $scope.upload.length; i++) {
                        if ($scope.upload[i] != null) {
                            $scope.upload[i].abort();
                        }
                    }
                }
                $scope.upload = [];
                $scope.uploadResult = [];
                $scope.selectedFiles = $files;
                $scope.dataUrls = [];
                for (var i = 0; i < $files.length; i++) {
                    var $file = $files[i];

                    var _fileExtensions = $scope.fsExtension.split(", ");
                    if (_fileExtensions.length > 0) {
                        if ($scope.checkExtension($file.name, _fileExtensions, $file.size, $scope.fsMaxSize)) {
                            if (window.FileReader && $file.type.indexOf('image') > -1) {
                                var fileReader = new FileReader();
                                fileReader.readAsDataURL($files[i]);
                                function setPreview(fileReader, index) {
                                    fileReader.onload = function (e) {
                                        $timeout(function () {
                                            $scope.dataUrls[index] = e.target.result;
                                        });
                                    }
                                }
                                setPreview(fileReader, i);
                            }
                            $scope.progress[i] = -1;
                            if ($scope.uploadRightAway) {
                                $scope.start(i, ImageFor);
                            }
                        }
                        else {
                            if ($scope.errorMsg == "") {
                                common.aaNotify.error("Invalid File " + $file.name);
                                $($element).removeClass("dragStart");
                            } else {
                                common.aaNotify.error($scope.errorMsg);
                                $($element).removeClass("dragStart");
                                $scope.errorMsg = "";
                            }
                        }
                    }
                    else {
                        if (window.FileReader && $file.type.indexOf('image') > -1) {
                            var fileReader = new FileReader();
                            fileReader.readAsDataURL($files[i]);
                            function setPreview(fileReader, index) {
                                fileReader.onload = function (e) {
                                    $timeout(function () {
                                        $scope.dataUrls[index] = e.target.result;
                                    });
                                }
                            }
                            setPreview(fileReader, i);
                        }
                        $scope.progress[i] = -1;
                        if ($scope.uploadRightAway) {
                            $scope.start(i, ImageFor);
                        }
                    }

                }
            }
            $scope.start = function (index, property) {
                common.usSpinnerService.spin($scope.fsSpinnderKey);

                var uploadURL = $rootScope.baseUrl + 'FileUploaderApi/Upload?folderName=' + $scope.fsFileUploadFolderName;
                $scope.progress[index] = 0;
                if ($scope.howToSend == 1) {
                    $scope.upload[index] = $upload.upload({
                        url: uploadURL,
                        method: $scope.httpMethod,
                        headers: { 'myHeaderKey': 'myHeaderVal' },
                        data: {
                            myModel: $scope.myModel
                        },
                        file: $scope.selectedFiles[index],
                        fileFormDataName: 'myFile'
                    }).then(function (response) {
                        $timeout(function () {
                            $scope.uploadResult.push(response.data);
                        });
                        var result = response.data.Result[0];
                        $scope.UploadedFilePath = result.Location;
                        $scope.UploadedFileName = result.Name;
                        $scope.UpdateFileName = result.FileName;
                        $scope.UploadedFileSize = $scope.selectedFiles[index].size;
                        if ($scope.fsNeedUpdateFileName == true || $scope.fsNeedUpdateFileName == "true") {
                            $scope.fsSaveuploadedfiles({ UploadedFilePath: $scope.UploadedFilePath, UploadedFileName: $scope.UploadedFileName, UpdateFileName: $scope.UpdateFileName, UploadedFile: $scope.selectedFiles[index], UploadedFileSize: $scope.UploadedFileSize });
                        }
                        else {
                            $scope.fsSaveuploadedfiles({ UploadedFilePath: $scope.UploadedFilePath, UploadedFileName: $scope.UploadedFileName, UploadedFile: $scope.selectedFiles[index], UploadedFileSize: $scope.UploadedFileSize });
                        }
                        common.usSpinnerService.stop($scope.fsSpinnderKey);
                    },
                    function (response) {

                        common.usSpinnerService.stop($scope.fsSpinnderKey);
                    },
                    function (evt) {
                        $scope.progress[index] = parseInt(100.0 * evt.loaded / evt.total);
                        //console.log($scope.progress[index]);
                    });
                }
                $timeout(function () {
                    $($element).removeClass("dragStart");
                }, 500);
            }
            $scope.init();
            if ($scope.fsFullDragArea == 'true') {
                window.ondragenter = function (e) {
                    $($element).addClass("dragStart");
                }
            }

            $scope.closeDragPopup = function (event) {
                event.preventDefault();
                event.stopPropagation();
                $($element).removeClass("dragStart");
            }
        },

    }
    return DDO;
}]);
