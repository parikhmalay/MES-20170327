'use strict';

var PRISTINE_CLASS = 'ng-pristine',
    DIRTY_CLASS = 'ng-dirty';

var Module = angular.module('datePicker');

Module.constant('dateTimeConfig', {
    template: function (attrs) {
        return '' +
            '<div ' +
            'date-picker="' + attrs.ngModel + '" ' +
            (attrs.view ? 'view="' + attrs.view + '" ' : '') +
            (attrs.maxView ? 'max-view="' + attrs.maxView + '" ' : '') +
            (attrs.template ? 'template="' + attrs.template + '" ' : '') +
            (attrs.minView ? 'min-view="' + attrs.minView + '" ' : '') +
            (attrs.format ? 'format="' + attrs.format + '" ' : '') +
            (attrs.min ? 'min=' + attrs.min + ' ' : '') +
            (attrs.max ? 'max=' + attrs.max + ' ' : '') +
            'class="dropdown-menu"></div>';
    },
    format: 'shortDate',
    views: ['date', 'year', 'month', 'hours', 'minutes'],
    dismiss: true,
    position: 'relative'
});

Module.directive('dateTimeAppend', function () {
    return {
        link: function (scope, element) {
            element.bind('click', function () {
                element.find('input')[0].focus();
            });
        }
    };
});

Module.directive('dateTime', ['$compile', '$document', '$filter', 'dateTimeConfig', '$parse', '$locale',
    function ($compile, $document, $filter, dateTimeConfig, $parse, $locale) {

        var body = $document.find('body');
        var dateFilter = $filter('date');

        return {
            require: 'ngModel',
            scope: true,
            link: function (scope, element, attrs, ngModel) {

                var format = attrs.format || dateTimeConfig.format;
                var parentForm = element.inheritedData('$formController');
                var views = $parse(attrs.views)(scope) || dateTimeConfig.views.concat();
                var view = attrs.view || views[0];
                var index = views.indexOf(view);
                var dismiss = attrs.dismiss ? $parse(attrs.dismiss)(scope) : dateTimeConfig.dismiss;
                var picker = null;
                var position = attrs.position || dateTimeConfig.position;
                var container = null;
                var jqElement = $(element);
                if (index === -1) {
                    views.splice(index, 1);
                }

                views.unshift(view);



                function formatter(value) {
                    return dateFilter(value, format);
                }

                function parser() {
                    return ngModel.$modelValue;
                }

                ngModel.$formatters.push(formatter);
                ngModel.$parsers.unshift(parser);


                var template = dateTimeConfig.template(attrs);

                function updateInput(event) {
                    event.stopPropagation();
                    if (ngModel.$pristine) {
                        ngModel.$dirty = true;
                        ngModel.$pristine = false;
                        element.removeClass(PRISTINE_CLASS).addClass(DIRTY_CLASS);
                        if (parentForm) {
                            parentForm.$setDirty();
                        }
                        ngModel.$render();
                    }
                }

                function clear() {
                    if (picker) {
                        picker.remove();
                        picker = null;
                    }
                    if (container) {
                        container.remove();
                        container = null;
                    }
                }

                function showPicker() {
                    if (picker) {
                        return;
                    }
                    // create picker element
                    picker = $compile(template)(scope);
                    scope.$digest();

                    scope.$on('setDate', function (event, date, view) {

                        //Date apply and digest issue : when change manually date.
                        //scope.safeApply(function () { attrs.ngModel = date; });

                        updateInput(event);
                        if (dismiss && (views[views.length - 1] === view || attrs.minView == view)) {
                            clear(); //temp commented by alkesh prajapati
                        }
                    });

                    scope.$on('$destroy', clear);

                    // move picker below input element

                    if (position === 'absolute') {
                        var pos = angular.extend(element.offset(), { height: element[0].offsetHeight });
                        picker.css({ top: pos.top + pos.height, left: pos.left, display: 'block', position: position });
                        body.append(picker);
                    } else if (position === 'fixed') {
                        container = angular.element('<div date-picker-wrapper></div>');
                        jqElement.parent().append(container);
                        container.append(picker);
                        var objModalPopup = jqElement.closest('.modal');

                        setTimeout(function () {
                            if (objModalPopup.hasClass("modal")) {
                                var dropdownPos = jqElement.offset();

                                var modalContentEl = objModalPopup.find(".modal-content").first();
                                var modalHeaderEl = objModalPopup.find(".modal-header").first();
                                var modalBodyEl = objModalPopup.find(".modal-body").first();

                                var modalPos = modalContentEl.offset();
                                var dropdownObj = $(container).find(".dropdown-menu").first();
                                var downPos = (dropdownPos.top - modalPos.top) + jqElement.outerHeight();
                                var leftPos = Math.abs(dropdownPos.left - modalPos.left);

                                var modalEndAt = (modalBodyEl.outerHeight() + modalHeaderEl.outerHeight() + modalPos.top);
                                var dropDownEndAt = (downPos + modalHeaderEl.outerHeight() + dropdownObj.height());

                                if (dropDownEndAt > modalEndAt) {
                                    downPos = (dropdownPos.top - modalPos.top) - dropdownObj.outerHeight();
                                    picker.css({ "bottom": "auto", "top": downPos, "left": leftPos, "position": "fixed", display: 'block' });
                                } else {
                                    picker.css({ "bottom": "auto", "top": downPos, "left": leftPos, "position": "fixed", display: 'block' });
                                }

                                //console.log("modalPos.top: " + modalPos.top);
                                //console.log("dropdownPos.top: " + dropdownPos.top);
                                //console.log("objModalPopup.scrollTop: " + objModalPopup.find(".modal-body").scrollTop());
                                //console.log("objModalPopup.height: " + objModalPopup.find(".modal-body").outerHeight());
                                //console.log("dropdownObj.height: " + dropdownObj.outerHeight());
                            } else {
                                var obj = $(element[0]);
                                var dropdownPos = obj.offset();
                                var dropdownObj = $(container).find(".dropdown-menu");
                                var downPos = (dropdownPos.top + obj.height() + 14) - $(window).scrollTop();
                                var topPos = ((dropdownPos.top + $(dropdownObj).height()) - $(window).scrollTop()) + obj.height();
                                if (topPos < $(window).height()) {
                                    picker.css({ "top": downPos, "left": dropdownPos.left, "position": "fixed", display: 'block' });
                                } else {
                                    picker.css({ "bottom": ($(window).height() - dropdownPos.top) + $(window).scrollTop(), "top": 'auto', "left": dropdownPos.left, "position": "fixed", display: 'block' });
                                }
                            }

                        }, 300);
                    }
                    else {
                        setTimeout(function () {
                            container = angular.element('<div date-picker-wrapper></div>');
                            element[0].parentElement.insertBefore(container[0], element[0]);
                            container.append(picker);
                            //          this approach doesn't work
                            //          element.before(picker);
                            picker.css({ top: element[0].offsetHeight + 'px', display: 'block' });
                        }, 300);
                    }

                    picker.bind('mousedown', function (evt) {
                        evt.preventDefault();
                    });
                }


                scope.safeApply = function (fn) {
                    var phase = this.$root.$$phase;
                    if (phase == '$apply' || phase == '$digest') {
                        if (fn && (typeof (fn) === 'function')) {
                            fn();
                        }
                    } else {
                        this.$apply(fn);
                    }
                };

                scope.$watch(function () {
                    return $parse(attrs.min)(scope);
                }, function (newVal) {
                    //if (attrs.required && typeof attrs.required != 'undefined') {
                    template = dateTimeConfig.template(attrs);
                    //}
                });

                scope.$watch(function () {
                    return $parse(attrs.max)(scope);
                }, function (newVal) {
                    //if (attrs.required && typeof attrs.required != 'undefined') {
                    template = dateTimeConfig.template(attrs);
                    //}
                });

                function parseDate(input, format) {
                    var valueFormat = $locale.DATETIME_FORMATS[format];
                    return Date.parseString(input, valueFormat);
                }

                //element.bind('focus', showPicker);

                element.bind('focus', function (e) {
                    //Show date picker on focus.. 
                    //Issue#Manually enter date was not update model..Changed By:JatinLalcheta Date:15-March-2016
                    if (e.target.attributes["onfocusshowpicker"] != null && e.target.attributes["onfocusshowpicker"].value == 'onfocusshowpicker') {
                        showPicker();
                    }
                });

                element.bind('click', showPicker);
                element.bind('blur', clear);

                //https://github.com/g00fy-/angular-datepicker/issues/75
                element.bind('change', function (e) {
                    var newDate = parseDate(e.target.value, e.target.attributes["format"].value);
                    //if (newDate == null) {
                    //    scope.$broadcast('clearDate');
                    //}
                    //else {
                    scope.$broadcast('set-user-typed-date', { date: newDate });
                    clear();
                    //}
                });
            }
        };
    }]);
