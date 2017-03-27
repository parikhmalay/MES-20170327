(function (angular, factory) {
    if (typeof define === 'function' && define.amd) {
        define(['angular', 'ckeditor'], function (angular) {
            return factory(angular);
        });
    } else {
        return factory(angular);
    }
}(angular || null, function (angular) {
    var app = angular.module('app');
    var $defer, loaded = false;

    app.run(['$q', '$timeout', function ($q, $timeout) {
        $defer = $q.defer();

        if (angular.isUndefined(CKEDITOR)) {
            throw new Error('CKEDITOR not found');
        }
        CKEDITOR.disableAutoInline = true;
        function checkLoaded() {
            if (CKEDITOR.status == 'loaded') {
                loaded = true;
                $defer.resolve();
            } else {
                checkLoaded();
            }
        }
        CKEDITOR.on('loaded', checkLoaded);
        $timeout(checkLoaded, 100);
    }])

    app.directive('ckeditor', ['$timeout', '$q', function ($timeout, $q) {
        'use strict';

        return {
            restrict: 'AC',
            require: ['ngModel', '^?form'],
            scope: false,
            link: function (scope, element, attrs, ctrls) {
                var ngModel = ctrls[0];
                var objParent = element[0].parentElement;
                var form = ctrls[1] || null;
                var EMPTY_HTML = '<p></p>',
                    isTextarea = element[0].tagName.toLowerCase() == 'textarea',
                    data = [],
                    isReady = false;

                if (!isTextarea) {
                    element.attr('contenteditable', true);
                }

                var onLoad = function () {
                    var options = {
                        toolbar: 'full',
                        toolbar_full: [
                            { name: 'document', groups: ['mode', 'document', 'doctools'], items: ['Source', '-', 'Save', 'NewPage', 'Preview', 'Print', '-', 'Templates'] },
                            { name: 'clipboard', groups: ['clipboard', 'undo'], items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
                            { name: 'editing', groups: ['find', 'selection', 'spellchecker'], items: ['Find', 'Replace', '-', 'SelectAll', '-', 'Scayt'] },
                            { name: 'forms', items: ['Form', 'Checkbox', 'Radio', 'TextField', 'Textarea', 'Select', 'Button', 'ImageButton', 'HiddenField'] },
                            '/',
                            { name: 'basicstyles', groups: ['basicstyles', 'cleanup'], items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] },
                            { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'], items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl', 'Language'] },
                            { name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
                            { name: 'insert', items: ['Image', 'Flash', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak', 'Iframe'] },
                            '/',
                            { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
                            { name: 'colors', items: ['TextColor', 'BGColor'] },
                            { name: 'tools', items: ['Maximize', 'ShowBlocks'] },
                            { name: 'others', items: ['-'] },
                            { name: 'about', items: ['About'] }
                        ],
                        disableNativeSpellChecker: false,
                        basicEntities: false,
                        uiColor: '#FAFAFA',
                        height: '200px',
                        width: '100%',
                        extraPlugins: 'image2',
                        filebrowserImageUploadUrl: '/Account/upload',
                    };

                    options = angular.extend(options, scope[attrs.ckeditor]);

                    var instance = (isTextarea) ? CKEDITOR.replace(element[0], options) : CKEDITOR.inline(element[0], options),
                        configLoaderDef = $q.defer();



                    element.bind('$destroy', function () {
                        instance.destroy(
                            false //If the instance is replacing a DOM element, this parameter indicates whether or not to update the element with the instance contents.
                        );
                    });
                    var setModelData = function (setPristine) {
                        var data = instance.getData();
                        if (data == EMPTY_HTML) {
                            data = null;
                        }
                        $timeout(function () { // for key up event
                            ngModel.$setViewValue(data);
                            (setPristine === true && form) && form.$setPristine();
                        }, 0);
                    }, onUpdateModelData = function (setPristine) {
                        if (!data.length) { return; }


                        var item = data.pop() || EMPTY_HTML;
                        isReady = false;
                        instance.setData(item, function () {
                            setModelData(setPristine);
                            isReady = true;
                        });
                    }

                    //instance.on('pasteState',   setModelData);
                    instance.on('change', setModelData);
                    instance.on('blur', setModelData);
                    instance.on('key', setModelData); // for source view
                    instance.on('instanceReady', setModelData);
                    instance.on('instanceReady', function () {
                        scope.$apply(function () {
                            onUpdateModelData(true);
                        });
                        var heightValue = 0;
                        var topHeight = objParent.getElementsByClassName("cke_top")[0].offsetHeight;
                        if (objParent.attributes.editorHeight) {
                            heightValue = objParent.attributes.editorHeight.value - (topHeight + 40);
                        }
                        if (heightValue > 100) {
                            objParent.getElementsByClassName("cke_contents")[0].setAttribute("style", "height:" + heightValue + "px");
                        }
                    });
                    instance.on('customConfigLoaded', function () {
                        configLoaderDef.resolve();
                        scope.$broadcast("ckeditorReady");
                    });

                    ngModel.$render = function () {
                        if (ngModel.$viewValue === undefined) {
                            ngModel.$setViewValue(null);
                            ngModel.$viewValue = null;
                        }

                        data.push(ngModel.$viewValue);
                        if (isReady) {
                            onUpdateModelData();
                        }
                    };
                };

                if (CKEDITOR.status == 'loaded') {
                    loaded = true;
                }
                if (loaded) {
                    onLoad();
                } else {
                    $defer.promise.then(onLoad);
                }
            }
        };
    }]);

    app.directive('ckeditorsmall', ['$timeout', '$q', function ($timeout, $q) {
        'use strict';

        return {
            restrict: 'AC',
            require: ['ngModel', '^?form'],
            scope: false,
            link: function (scope, element, attrs, ctrls) {
                var ngModel = ctrls[0];
                var objParent = element[0].parentElement;
                var form = ctrls[1] || null;
                var EMPTY_HTML = '<p></p>',
                    isTextarea = element[0].tagName.toLowerCase() == 'textarea',
                    data = [],
                    isReady = false;
                if (!isTextarea) {
                    element.attr('contenteditable', true);
                }

                var onLoad = function () {
                    var options = {
                        toolbar: 'full',
                        toolbar_full: [
                            {
                                name: 'editing', groups: ['tools', 'find', 'selection', 'spellchecker'],
                                items: ['Maximize', 'ShowBlocks', 'Find', 'Replace', '-', 'SelectAll', '-', 'Scayt']
                            },
                            {
                                name: 'basicstyles', groups: ['basicstyles', 'cleanup'],
                                items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat']
                            },
                            { name: 'styles', items: ['Font', 'FontSize'] },
                            {
                                name: 'Paragraph', groups: ['list', 'align'],
                                items: ['NumberedList', 'BulletedList', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock']
                            },
                            {
                                name: 'document', groups: ['mode'],
                                items: ['Source', '-']
                            },
                            {
                                name: 'insert',
                                items: ['Table']
                            },
                            {
                                name: 'links',
                                items: ['Link', 'Unlink', 'Anchor']
                            },
                        ],
                        disableNativeSpellChecker: false,
                        basicEntities: false,
                        uiColor: '#FAFAFA',
                        height: 'auto',
                        width: '100%',
                        extraPlugins: 'image2',
                        filebrowserImageUploadUrl: '/api/FileUploader/upload',
                    };

                    options = angular.extend(options, scope[attrs.ckeditor]);

                    var instance = (isTextarea) ? CKEDITOR.replace(element[0], options) : CKEDITOR.inline(element[0], options),
                        configLoaderDef = $q.defer();



                    element.bind('$destroy', function () {
                        instance.destroy(
                            false //If the instance is replacing a DOM element, this parameter indicates whether or not to update the element with the instance contents.
                        );
                    });
                    var setModelData = function (setPristine) {
                        var data = instance.getData();
                        if (data == EMPTY_HTML) {
                            data = null;
                        }
                        $timeout(function () { // for key up event
                            ngModel.$setViewValue(data);
                            (setPristine === true && form) && form.$setPristine();
                        }, 0);
                    }, onUpdateModelData = function (setPristine) {
                        if (!data.length) { return; }


                        var item = data.pop() || EMPTY_HTML;
                        isReady = false;
                        instance.setData(item, function () {
                            setModelData(setPristine);
                            isReady = true;
                        });
                    }

                    //instance.on('pasteState',   setModelData);
                    instance.on('change', setModelData);
                    instance.on('blur', setModelData);
                    instance.on('key', setModelData); // for source view
                    instance.on('instanceReady', setModelData);
                    instance.on('instanceReady', function () {
                        scope.$apply(function () {
                            onUpdateModelData(true);
                        });
                        var heightValue = 0;
                        var topHeight = objParent.getElementsByClassName("cke_top")[0].offsetHeight
                        if (objParent.attributes.editorheight) {
                            heightValue = objParent.attributes.editorheight.value - (topHeight + 40);
                        }
                        if (heightValue > 100) {
                            objParent.getElementsByClassName("cke_contents")[0].setAttribute("style", "height:" + heightValue + "px");
                        }

                    });
                    instance.on('customConfigLoaded', function () {
                        scope.$broadcast("ckeditorReady");
                        configLoaderDef.resolve();
                    });

                    ngModel.$render = function () {
                        if (ngModel.$viewValue === undefined) {
                            ngModel.$setViewValue(null);
                            ngModel.$viewValue = null;
                        }

                        data.push(ngModel.$viewValue);
                        if (isReady) {
                            onUpdateModelData();
                        }
                    };
                };

                if (CKEDITOR.status == 'loaded') {
                    loaded = true;
                }
                if (loaded) {
                    onLoad();
                } else {
                    $defer.promise.then(onLoad);
                }
            }
        };
    }]);
    return app;
}));