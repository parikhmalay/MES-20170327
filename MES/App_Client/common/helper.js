/*******Get interger in ng-Options*******/
function getIntKey(key) {
    // 10 is the radix, which is the base (assumed to be base 10 here)
    return parseInt(key, 10);
}

/*******Convert Key-Value to Array*******/
function KeyValueToArray(List) {
    var array = [];
    angular.forEach(List, function (V, K) {
        if (K.indexOf('$') >= 0)
        { }
        else {
            array.push({ id: K, name: V })
        }
    })

    return array;
}

function ConvertKeyValueToArray(List) {
    var array = [];
    angular.forEach(List, function (V, K) {
        array.push({ id: K, name: V })
    })
    return array;
}

//Check object is Empty
function IsObjectEmpty(obj) {
    return angular.equals({}, obj);
}

//Check item is null or undefined
function Isundefinedornull(item) {
    return (item == undefined || item == null);
}

//Check item is null or undefined
function IsUndefinedNullOrEmpty(item) {
    return (item == undefined || item == null || item == '');
}

/*******Get length of object*******/
function getLength(item) {
    if (!IsUndefinedNullOrEmpty(item))
        return item.length;
    else
        return 0;
}

/*******Check valid email*******/
function IsValidEmail(item) {
    var EMAIL_REGEXP = /^[_a-z0-9]+(\.[_a-z0-9]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$/;
    var match = item.match(EMAIL_REGEXP)
    return match;
}

/*******Check valid decimal number*******/
function IsValidDecimal(item) {
    var NUMBER_REGEXP = /^[0-9]{0,7}(\.\d{0,3})?$/;
    var match = item.match(NUMBER_REGEXP)
    return match;
}

/*******Check valid website*******/
function IsValidURL(item) {
    var URL_REGEXP = /^((?:http|ftp)s?:\/\/)(?:(?:[A-Z0-9](?:[A-Z0-9-]{0,61}[A-Z0-9])?\.)+(?:[A-Z]{2,6}\.?|[A-Z0-9-]{2,}\.?)|localhost|\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})(?::\d+)?(?:\/?|[\/?]\S+)$/i;
    var match = item.match(URL_REGEXP)
    return match;
}

///Validate Array
function validarray(value) {
    return (angular.isArray(value) && value.length > 0) || (!angular.isArray(value) && value != null);
};

/*******UTC Date to Local Date*******/
function convertUTCDateToLocalDate(date) {
    if (IsNotNullorEmpty(date)) {
        var d = new Date(date);
        //var d = new Date(date.replace(/-/g, "/"))
        d = d - (d.getTimezoneOffset() * 60000);
        d = new Date(d);
        return d;
    }
    return null;
    //return d.toISOString().substr(0, 19) + "Z";
}

/*******Local Date to UTC Date*******/
function convertLocalDateToUTCDate(date) {
    if (IsNotNullorEmpty(date)) {
        return new Date(date);
    }
    return null;
}


function convertDateToclienttimezone(date) {
    if (!(date.indexOf("Z") > -1)) {
        date = date + "Z";
    }
    var d = new Date(date);
    d = d + (d.getTimezoneOffset() * 60000);
    d = new Date(d);
    return d;
    //return d.toISOString().substr(0, 19) + "Z";
}

function IsNotNullorEmptyNew(item) {
    var match = false;
    if (typeof item != 'undefined' && item != '' && item != null && item != 'null')
        match = true;
    if (match && item != null && typeof item.length != 'undefined' && item.length <= 0) {
        match = false;
    }
    return match;
}

function IsNotNullorEmpty(item) {
    var match = false;
    if (typeof item != 'undefined' && item != '' && item != null && item != 'null')
        match = true;
    if (match && item != null && typeof item.length != 'undefined' && item.length <= 0) {
        match = false;
    }
    return match;
}

function IsNullorEmpty(item) {
    return !IsNotNullorEmpty(item);
}

function IsJSON(jsonString) {
    try {
        JSON.parse(jsonString);
        return true;
    } catch (e) {
        return false;
    }
}

function GenerateAddressKey(addressType, address1, address2, address3, address4, city, state, county, country, zip) {
    if (IsNotNullorEmpty(address1) && country == 'USA') {
        var address = (IsNotNullorEmpty(addressType) ? addressType : '') + (IsNotNullorEmpty(address1) ? address1 : '') + (IsNotNullorEmpty(address2) ? address2 : '') + (IsNotNullorEmpty(address3) ? address3 : '') + (IsNotNullorEmpty(address4) ? address4 : '') + (IsNotNullorEmpty(city) ? city : '') + (IsNotNullorEmpty(state) ? state : '') + (IsNotNullorEmpty(county) ? county : '') + (IsNotNullorEmpty(country) ? country : '') + (IsNotNullorEmpty(zip) ? zip : '');
        return address.replace(/([^a-z0-9]+)/gi, '');
    }
    return '';
}

function RedirectToPage(basicurl, redirecturl, querystringparameter) {

    if (redirecturl == "ViewOrganization") {
        window.location.href = basicurl + "/Organization/Organization#/ViewOrganization/" + querystringparameter;
    }
    else if (redirecturl == "ViewVolunteer")
        window.location.href = basicurl + "/Volunteer/Volunteer/Details/#/view/" + querystringparameter;

}

function RedirectToAccessDenied() {
    window.location.href = "/Error/AccessDenied";
}

function GetFormattedAddress(address) {
    return FormatAddress(address.AddressLine1, address.AddressLine2, address.AddressLine3, address.AddressLine4, address.City, (IsUndefinedNullOrEmpty(address.StateRegionProvince) ? address.State : address.StateRegionProvince), address.County, address.Country, (IsUndefinedNullOrEmpty(address.PostalCode) ? address.Postalcode : address.PostalCode));
}

function FormatAddress(address1, address2, address3, address4, city, state, county, country, zip) {
    var address = '';
    if (IsNotNullorEmpty(address1) && IsNotNullorEmpty(address1.trim())) {
        if (IsNotNullorEmpty(address))
            address = address + ', ' + address1
        else
            address = address1
    }
    if (IsNotNullorEmpty(address2) && IsNotNullorEmpty(address2.trim())) {
        if (IsNotNullorEmpty(address))
            address = address + ', ' + address2
        else
            address = address2

    }
    if (IsNotNullorEmpty(address3) && IsNotNullorEmpty(address3.trim())) {
        if (IsNotNullorEmpty(address))
            address = address + ', ' + address3
        else
            address = address3

    }
    if (IsNotNullorEmpty(address4) && IsNotNullorEmpty(address4.trim())) {
        if (IsNotNullorEmpty(address))
            address = address + ', ' + address4
        else
            address = address4
    }
    if (IsNotNullorEmpty(city)) {
        if (IsNotNullorEmpty(address))
            address = address + ', ' + city
        else
            address = city

    }
    if (IsNotNullorEmpty(county)) {
        if (IsNotNullorEmpty(address))
            address = address + ', ' + county
        else
            address = county

    }
    if (IsNotNullorEmpty(state)) {
        address = address + ', ' + state
    }
    if (IsNotNullorEmpty(country)) {
        address = address + ', ' + country
    }
    if (IsNotNullorEmpty(zip)) {
        address = address + ', ' + zip
    }
    //address = address.substring(2, address.length);
    return address;
}

var app = angular.module('app');
//app.directive('ngBlur', function () {
//    return {
//        restrict: 'A',
//        link: function (scope, element, attrs) {
//            element.bind("blur", function (event) {
//                scope.$apply(attrs.ngBlur);
//            });
//        }
//    };
//});
function getStateResolveItems(common) {
    return common.$state.$current.locals.resolve.$$values;
}

function getResolveValue(common, property) {
    //console.log(common.$state.$current.locals.resolve.$$values);
    console.log(common.$route.current.locals);
    if (common.$route.current.locals.hasOwnProperty(property))
        return common.$route.current.locals[property];
    else
        return '';
}
// write numbersOnly
app.directive('numbersOnly', function () {
    return function (scope, element, attrs) {
        var keyCode = [8, 9, 37, 39, 46, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 110, 190];
        element.bind("keydown", function (event) {
            //console.log(event.which, $.inArray(event.which, keyCode));
            if ($.inArray(event.which, keyCode) == -1) {
                scope.$apply(function () {
                    scope.$eval(attrs.onlyNum);
                    event.preventDefault();
                });
                event.preventDefault();
            }
        });
    };
});
///format upto required precision 
app.directive('toPrecision', function () {
    return {
        replace: false,
        restrict: 'A',
        scope: {
            ngModel: "="
        },
        controller: function ($scope, $element, $attrs) {
            var prec = $attrs.toPrecision;
            $scope.Init = function () {
                $scope.setPrecision();
            };
            $scope.setPrecision = function () {
                if (isNaN($scope.ngModel)) {
                    $scope.ngModel = 0;
                    $scope.ngModel.toFixed(prec);
                }
                else {
                    var n = parseFloat($scope.ngModel);
                    if (isNaN(n)) {
                        n = 0;
                    }
                    $scope.ngModel = n.toFixed(prec);
                }
            };

            var e = $element;
            e.on('blur', function () {
                if (isNaN(e.val())) {
                    e.val((0).toFixed(prec));
                }
                else {
                    var n = parseFloat(e.val());
                    if (isNaN(n)) {
                        n = 0;
                    }
                    e.val(n.toFixed(prec));
                }
            });

            $scope.Init();
        }
    }
});

app.directive('integerOnly', function () {
    return function (scope, element, attrs) {
        var keyCode = [8, 9, 13, 37, 39, 46, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105];
        element.bind("keydown", function (event) {
            //console.log(event.which, $.inArray(event.which, keyCode));
            if ($.inArray(event.which, keyCode) == -1) {
                scope.$apply(function () {
                    scope.$eval(attrs.onlyNum);
                    event.preventDefault();
                });
                event.preventDefault();
            }
        });
    };
});

app.directive('numbersMaxMin', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            element.bind("blur", function (event) {
                if (!modelCtrl || !element.val()) return;

                var min = typeof attrs.min != 'undefined' ? Number(attrs.min) : 0;
                var max = typeof attrs.max != 'undefined' ? Number(attrs.max) : 99999999;

                var currentValue = Number(element.val().replace(/[^\d|\-+|\.+]/g, ''));

                //console.log(max, min, currentValue);
                if (!(currentValue >= min && currentValue <= max)) {
                    modelCtrl.$setViewValue(attrs.min);
                    modelCtrl.$render();
                }
            });
        }
    };
});
app.directive('numbersMaxMinValue', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            element.bind("blur", function (event) {
                if (!modelCtrl || !element.val()) { modelCtrl.$setViewValue(attrs.min); }

                var min = typeof attrs.min != 'undefined' ? Number(attrs.min) : 0;
                var max = typeof attrs.max != 'undefined' ? Number(attrs.max) : 99999999;

                var currentValue = Number(element.val());

                //console.log(max, min, currentValue);
                if (!(currentValue >= min && currentValue <= max)) {
                    modelCtrl.$setViewValue(attrs.min);
                    modelCtrl.$render();
                }
            });
        }
    };
});

// write numbersOnly
app.directive('checkWebsitMode', function () {
    return function (scope, element, attrs, $filter) {
        element.bind("click", function (event) {
            CheckIsWebsiteInEditMode();
        });
    };
});

function CheckIsWebsiteInEditMode() {
    if (IsWebsiteInEditMode) {
        event.preventDefault();
        //alert($filter('translate')('_ConfirmInactiveAgreement_'));
        alert('Functionality disabled in design mode.');
    }
    return IsWebsiteInEditMode;
}

app.directive('validateDate', function ($parse, $locale) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            scope.$watch(attrs.ngModel, function (value) {
                IsValidDate();
            });

            scope.$watch(function () {
                return $parse(attrs.min)(scope);
            }, function (newVal) {
                IsValidDate();
            });

            scope.$watch(function () {
                return $parse(attrs.max)(scope);
            }, function (newVal) {
                IsValidDate();
            });

            function toDate(dateString) {
                var timeComponents = dateString.replace(/\s.*$/, '').split(':');
                if (dateString.indexOf("PM") > -1) {
                    timeComponents[0] = parseInt(timeComponents[0]) + 12;
                }

                var date = new Date();
                date.setHours(timeComponents[0]);
                date.setMinutes(timeComponents[1]);
                if (timeComponents[2])
                    date.setSeconds(timeComponents[2]);
                else
                    date.setSeconds(0);

                //date.setSeconds(timeComponents[2]);
                return date;
            }

            function IsValidDate() {
                if (!modelCtrl || !element.val()) {
                    modelCtrl.$setValidity('invaliddate', true);
                    return;
                }

                //console.log(attrs.format, $locale.DATETIME_FORMATS[attrs.format]);
                var view = typeof attrs.view != 'undefined' ? attrs.view : "date";
                if (view == 'date') {
                    var minDate = typeof attrs.min != 'undefined' ? new Date(attrs.min.replace(/"/g, "")) : new Date(1, 1, 1);
                    minDate = new Date(minDate.getFullYear(), minDate.getMonth(), minDate.getDate());
                    var maxDate = typeof attrs.max != 'undefined' ? new Date(attrs.max.replace(/"/g, "")) : new Date(9999, 1, 1);
                    var inputDate = new Date(element.val()); //new Date(element.val().replace(/-/g, '/'));
                    //console.log(minDate, maxDate, inputDate, element.val());
                    modelCtrl.$setValidity('invaliddate', inputDate.getTime() >= minDate.getTime() && inputDate.getTime() <= maxDate.getTime());
                    modelCtrl.$validate();
                }
                else if (view == 'hours') {
                    var minDate = typeof attrs.min != 'undefined' ? new Date(attrs.min.replace(/"/g, "")) : new Date(1, 1, 1);
                    var maxDate = typeof attrs.max != 'undefined' ? new Date(attrs.max.replace(/"/g, "")) : new Date(9999, 1, 1);
                    var inputTime = toDate(element.val()); //new Date(element.val().replace(/-/g, '/'));
                    console.log(attrs.date, element.val(), inputTime);

                    var inputDate = typeof attrs.date != 'undefined' ? new Date(attrs.date.replace(/"/g, "")) : new Date(1, 1, 1);
                    var inputDate = new Date(inputDate.getFullYear(), inputDate.getMonth(), inputDate.getDate(), inputTime.getHours(), inputTime.getMinutes(), inputTime.getSeconds(), minDate.getMilliseconds());

                    modelCtrl.$setValidity('invaliddate', inputDate.getTime() >= minDate.getTime() && inputDate.getTime() <= maxDate.getTime());
                    modelCtrl.$validate();
                }
            }
        }
    };
});

app.directive('validNumber', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }

            ngModelCtrl.$parsers.push(function (val) {
                if (angular.isUndefined(val)) {
                    var val = '';
                }
                var clean = val.replace(/[^0-9\.]/g, '');
                var decimalCheck = clean.split('.');

                if (!angular.isUndefined(decimalCheck[1])) {
                    decimalCheck[1] = decimalCheck[1].slice(0, 2);
                    clean = decimalCheck[0] + '.' + decimalCheck[1];
                }

                if (val !== clean) {
                    ngModelCtrl.$setViewValue(clean);
                    ngModelCtrl.$render();
                }
                return clean;
            });

            element.bind('keypress', function (event) {
                if (event.keyCode === 32) {
                    event.preventDefault();
                }
            });
        }
    };
});

//http://weblogs.asp.net/dwahlin/archive/2013/09/24/building-a-custom-angularjs-unique-value-directive.aspx
app.directive('uniqueCode', ['UniqueCodeSvc', function (UniqueCodeSvc) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            element.bind('blur', function (e) {
                if (!modelCtrl || !element.val()) return;
                var keyProperty = scope.$eval(attrs.uniqueCode);
                var currentValue = element.val();

                UniqueCodeSvc.checkUniqueCode(keyProperty.id || 0, keyProperty.type, currentValue)
                 .then(function (unique) {
                     //Ensure value that being checked hasn't changed
                     if (currentValue == element.val()) {
                         modelCtrl.$setValidity('unique', unique.Status);
                     }
                 }, function () {
                     //Probably want a more robust way to handle an error
                     modelCtrl.$setValidity('unique', true);
                 });

                //scope.$apply();
            });
        }
    }
}]);

app.directive('phoneType', function (UniqueCodeSvc) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            element.bind('blur', function (e) {
                if (!modelCtrl || !element.val()) return;
                var keyProperty = scope.$eval(attrs.uniqueCode);
                var currentValue = element.val();
                if (IsNotNullorEmpty(currentValue))
                    modelCtrl.$setValidity('phontyperequired', false);
            });
        }
    }
});

app.directive('uniqueemail', [function (ContactSelectorSvc) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            element.bind('keydown keypress', function (e) {
                modelCtrl.$setValidity('emailunique', true);
                if (IsNotNullorEmpty(scope.EnableSubmit)) {
                    if (typeof scope.EnableSubmit != 'undefined')
                        scope.EnableSubmit = false;
                    else
                        scope.EnableSubmit = true;
                }
            });
            element.bind('blur', function (e) {
                modelCtrl.$setValidity('emailunique', true);
                if (IsNotNullorEmpty(modelCtrl.$viewValue)) {
                    var volunteerId = IsNotNullorEmpty(attrs.volunteerid) ? attrs.volunteerid : 0;
                    var validateemail = IsNotNullorEmpty(attrs.validateemail) && (attrs.validateemail == 'true' || attrs.validateemail == true) ? true : false;
                    validateemail = false; // set false to remove email unique validation from system.
                    if (validateemail) {
                        //IdentitySvc.CheckVolunteerAccount(modelCtrl.$viewValue).then(function (res) {
                        //    if (res.data == 'true') {
                        //        modelCtrl.$setValidity('emailunique', false);
                        //        if (typeof scope.EnableSubmit != 'undefined')
                        //            scope.EnableSubmit = true;
                        //    }
                        //    else {
                        //        modelCtrl.$setValidity('emailunique', true);
                        //        if (typeof scope.EnableSubmit != 'undefined')
                        //            scope.EnableSubmit = true;
                        //    }
                        //}, function () {
                        //    modelCtrl.$setValidity('emailunique', true);
                        //    if (typeof scope.EnableSubmit != 'undefined')
                        //        scope.EnableSubmit = true;
                        //});
                    }
                    else {
                        modelCtrl.$setValidity('emailunique', true);
                        if (typeof scope.EnableSubmit != 'undefined')
                            scope.EnableSubmit = true;
                    }
                }
            });
        }
    }
}]);

app.directive("typeaheadwatchchanges", function () {
    return {
        restrict: 'A',
        require: ["ngModel"],
        link: function (scope, element, attr, ctrls) {
            element.bind('focus', function () {
                if (IsNotNullorEmpty(ctrls[0].$viewValue))
                    ctrls[0].$setViewValue(ctrls[0].$viewValue);
                else
                    ctrls[0].$setViewValue('#');
            });

            //element.bind("blur", function (event) {
            //    if (IsNotNullorEmpty(attr.required))
            //        if (ctrls[0].$viewValue == '#')
            //            ctrls[0].$setValidity('required', false);
            //});
        }
    };
});

app.directive("typeaheadwatchchangeshousehold", function () {
    return {
        restrict: 'A',
        require: ["ngModel"],
        link: function (scope, element, attr, ctrls) {
            element.bind('focus', function () {
                if (attr.loadnewhousehold) {
                    if (IsNotNullorEmpty(ctrls[0].$viewValue))
                        ctrls[0].$setViewValue(ctrls[0].$viewValue);
                    else
                        ctrls[0].$setViewValue('#');
                }
            });
        }
    };
});

app.filter('filterMultiple', ['$filter', function ($filter) {
    return function (items, keyObj) {
        var filterObj = {
            data: items,
            filteredData: items,
            applyFilter: function (obj, key) {
                if (typeof obj != 'undefined' && obj != '') {
                    var fData = [];
                    //if (this.filteredData.length == 0)
                    //this.filteredData = this.data;
                    if (obj) {
                        var fObj = {};
                        if (!angular.isArray(obj)) {
                            fObj[key] = obj;
                            fData = fData.concat($filter('filter')(this.filteredData, fObj));
                        } else if (angular.isArray(obj)) {
                            if (obj.length > 0) {
                                for (var i = 0; i < obj.length; i++) {
                                    if (angular.isDefined(obj[i])) {
                                        fObj[key] = obj[i];
                                        fData = fData.concat($filter('filter')(this.filteredData, fObj));
                                    }
                                }
                            }
                        }
                        if (fData.length > 0) {
                            this.filteredData = fData;
                        }
                        else {
                            count++;
                        }
                    }
                }
                else {
                    this.filteredData = this.data;
                }
            }
        };

        var count = 0;
        if (keyObj) {
            angular.forEach(keyObj, function (obj, key) {
                filterObj.applyFilter(obj, key);
            });
        }
        if (_.size(keyObj) == count)
            filterObj.filteredData = [];
        return filterObj.filteredData;
    }
}]);

app.filter('tel', function () {
    return function (tel) {
        if (!tel) { return ''; }

        var value = tel.toString().trim().replace(/^\+/, '');

        if (value.match(/[^0-9]/)) {
            return tel;
        }

        var country, city, number;

        switch (value.length) {
            case 10: // +1PPP####### -> C (PPP) ###-####
                country = 1;
                city = value.slice(0, 3);
                number = value.slice(3);
                break;

            case 11: // +CPPP####### -> CCC (PP) ###-####
                country = value[0];
                city = value.slice(1, 4);
                number = value.slice(4);
                break;

            case 12: // +CCCPP####### -> CCC (PP) ###-####
                country = value.slice(0, 3);
                city = value.slice(3, 5);
                number = value.slice(5);
                break;

            default:
                return tel;
        }

        if (country == 1) {
            country = "";
        }

        number = number.slice(0, 3) + '-' + number.slice(3);

        return (country + " (" + city + ") " + number).trim();
    };
});
app.directive('httpPrefix', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, controller) {
            function ensureHttpPrefix(value) {
                // Need to add prefix if we don't have http:// prefix already AND we don't have part of it
                if (value && !/^(http):\/\//i.test(value)
                   && 'http://'.indexOf(value) === -1) {
                    controller.$setViewValue('http://' + value);
                    controller.$render();
                    return 'http://' + value;
                }
                else
                    return value;
            }
            controller.$formatters.push(ensureHttpPrefix);
            controller.$parsers.push(ensureHttpPrefix);
        }
    };
});

app.directive('clickOnce', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            //var replacementText = attrs.clickOnce;
            element.bind('click', function () {
                $timeout(function () {
                    //if (replacementText) {
                    //    element.html(replacementText);
                    //}
                    element.attr('disabled', true);
                }, 0);
                $timeout(function () {
                    //if (replacementText) {
                    //    element.html(replacementText);
                    //}
                    element.attr('disabled', false);
                }, 1500);
            });
        }
    };
});

function strStartsWith(str, prefix) {
    return str.indexOf(prefix) === 0;
}

app.filter('groupBy', function ($parse) {
    return _.memoize(function (items, field) {
        var getter = $parse(field);
        return _.groupBy(items, function (item) {
            return getter(item);
        });
    });
});

Date.prototype.addDays = function (num) {
    var value = this.valueOf();
    value += 86400000 * num;
    return new Date(value);
}

Date.prototype.addSeconds = function (num) {
    var value = this.valueOf();
    value += 1000 * num;
    return new Date(value);
}

Date.prototype.addMinutes = function (num) {
    var value = this.valueOf();
    value += 60000 * num;
    return new Date(value);
}

Date.prototype.addHours = function (num) {
    var value = this.valueOf();
    value += 3600000 * num;
    return new Date(value);
}

Date.prototype.addMonths = function (num) {
    var value = new Date(this.valueOf());

    var mo = this.getMonth();
    var yr = this.getYear();

    mo = (mo + num) % 12;
    if (0 > mo) {
        yr += (this.getMonth() + num - mo - 12) / 12;
        mo += 12;
    }
    else
        yr += ((this.getMonth() + num - mo) / 12);

    value.setMonth(mo);
    value.setYear(yr);
    return value;
}

var ddlDays = [{ id: 1 }, { id: 2 }, { id: 3 }, { id: 4 }, { id: 5 }, { id: 6 }, { id: 7 }, { id: 8 }, { id: 9 }, { id: 10 }, { id: 11 }, { id: 12 }, { id: 13 }, { id: 14 }, { id: 15 }, { id: 16 }, { id: 17 }, { id: 18 }, { id: 19 }, { id: 20 }, { id: 21 }, { id: 22 }, { id: 23 }, { id: 24 }, { id: 25 }, { id: 26 }, { id: 27 }, { id: 28 }, { id: 29 }, { id: 30 }, { id: 31 }]
var ddlMonths = [{ id: 0, name: 'January' }, { id: 1, name: 'February' }, { id: 2, name: 'March' }, { id: 3, name: 'April' }, { id: 4, name: 'May' }, { id: 5, name: 'June' }, { id: 6, name: 'July' }, { id: 7, name: 'August' }, { id: 8, name: 'September' }, { id: 9, name: 'October' }, { id: 10, name: 'November' }, { id: 11, name: 'December' }]
app.directive('validatedate', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            function watch() {
                var day = IsNotNullorEmpty(attrs.day) ? attrs.day : '';
                var month = IsNotNullorEmpty(attrs.month) ? attrs.month : '';
                if (IsNotNullorEmpty(day) && IsNotNullorEmpty(month)) {
                    var dd = new Date(1900, month, day);
                    if (dd.getMonth() === parseInt(month) && dd.getDate() === parseInt(day))
                        modelCtrl.$setValidity('isvaliddate', true);
                    else {
                        modelCtrl.$setValidity('isvaliddate', false);
                    }
                }
                else
                    modelCtrl.$setValidity('isvaliddate', true);
            }

            scope.$watch(watch);
        }
    }
});

function GetRandomDigit() {
    var id = '';
    for (var i = 1; i <= 10; i++) {
        id += Math.floor((Math.random() * 6) + 1);
    }
    return id;
}

app.filter('customDate', function ($filter) {
    return function (date, format) {
        if (IsNotNullorEmpty(date)) {
            return $filter('date')(new Date(date), format);
        }
        return '-';
    }
});

app.filter('bytes', function () {
    return function (bytes, precision) {
        if (isNaN(parseFloat(bytes)) || !isFinite(bytes)) return '-';
        if (typeof precision === 'undefined') precision = 1;
        var units = ['bytes', 'kB', 'MB', 'GB', 'TB', 'PB'],
        number = Math.floor(Math.log(bytes) / Math.log(1024));
        return (bytes / Math.pow(1024, Math.floor(number))).toFixed(precision) + ' ' + units[number];
    }
});

var FileSizeLimitStringFormat = {};
FileSizeLimitStringFormat = {
    sizelimit: 1050000,
    formatVarString: function () {
        var args = [].slice.call(arguments);
        if (this.toString() != '[object Object]') {
            args.unshift(this.toString());
        }
        args[0] = "File '{1}' is exceeded limit. Please choose file less than 1MB.";
        var pattern = new RegExp('{([1-' + args.length + '])}', 'g');
        return String(args[0]).replace(pattern, function (match, index) { return args[index]; });
    }
}

app.directive('validPrice', function () { //Amount should be greater than zero
    return {
        require: "ngModel",
        link: function (scope, elm, attrs, ctrl) {
            var regex = /^\d{1,18}(\.\d{1,4})?$/;
            ctrl.$parsers.unshift(function (viewValue) {
                var floatValue = parseFloat(viewValue);

                if ((viewValue.length == 0) || (floatValue >= 0 && regex.test(viewValue))) {
                    ctrl.$setValidity('validPrice', true);
                    //return viewValue;
                }
                else {
                    ctrl.$setValidity('validPrice', false);
                }
                return viewValue;
            });
        }
    };
});

app.directive('verifyselectedyear', ['CommunicationSvc', function (CommunicationSvc) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            element.bind('blur', function (e) {
                modelCtrl.$setValidity('verifyyear', true);
                if (IsNotNullorEmpty(scope.local)) {
                    if (typeof scope.local.EnableSubmit != 'undefined')
                        scope.local.EnableSubmit = false;
                    else
                        scope.local.EnableSubmit = true;
                }
                if (IsNotNullorEmpty(modelCtrl.$viewValue)) {
                    CommunicationSvc.IsProcessPendingForYear(modelCtrl.$viewValue.name).then(function (res) {
                        if (res.data == 'true') {
                            modelCtrl.$setValidity('verifyyear', false);
                        }
                        else {
                            modelCtrl.$setValidity('verifyyear', true);
                        }
                        if (IsNotNullorEmpty(scope.local))
                            scope.local.EnableSubmit = true;
                    }, function () {
                        modelCtrl.$setValidity('verifyyear', true);
                        if (IsNotNullorEmpty(scope.local))
                            scope.local.EnableSubmit = true;
                    });
                }
                else {
                    if (IsNotNullorEmpty(scope.local))
                        scope.local.EnableSubmit = true;
                }
            });
        }
    }
}]);
app.filter('unsafe', function ($sce) {
    return function (val) {
        return $sce.trustAsHtml(val);
    };
});

/*
function ShowMessage(common, Message) {
    if (Message != null) {
        if (Message.Type == 'Error' || Message.Type == '1') {
            common.aaNotify.error(Message.Description);
            return false;
        }
        else {
            common.aaNotify.success(Message.Description);
            return true;
        }
    }

    return true;
};
*/

//this is the common funcation to display error or success message. it will return the true if there is not error else it return false.
function ShowMessage(common, typedResponse, displayMessage) {
    if (displayMessage == null)
        displayMessage = true;

    if (typedResponse != null) {
        if (Number(typedResponse.StatusCode) == 200) {
            if (typedResponse.SuccessMessage != null && typedResponse.SuccessMessage != "")
                if (displayMessage)
                    common.aaNotify.success(typedResponse.SuccessMessage);
            return true;
        }
        else {
            if (typedResponse.ErrorText != null && typedResponse.ErrorText != "") {
                if (displayMessage)
                    common.aaNotify.error(typedResponse.ErrorText);
            }
            else {
                if (typedResponse.ErrorObject != null && typedResponse.ErrorObject != "") {
                    if (typedResponse.ErrorObject.Message != null) {
                        if (displayMessage)
                            common.aaNotify.error(typedResponse.ErrorObject.Message);
                    }
                    else if (typedResponse.ErrorObject.length > 0)
                        if (displayMessage)
                            common.aaNotify.error(typedResponse.ErrorObject[0].Message);
                }
            }
            return false;
        }
    }
    return true;
};



app.filter('range', function () {
    return function (input, min, max) {
        min = parseInt(min); //Make string input int
        max = parseInt(max);
        for (var i = min; i <= max; i++)
            input.push(i);
        return input;
    };
});

app.directive('focusMe', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            //console.log(element,"TEst");
            ////$timeout(function() {
            //element[0].focus();
            ////});
            $timeout(function () {
                element.focus();
            }, 100);
        }
    };
});

app.directive('digitsOnly', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            modelCtrl.$parsers.push(function (inputValue) {
                // this next if is necessary for when using ng-required on your input.
                // In such cases, when a letter is typed first, this parser will be called
                // again, and the 2nd time, the value will be undefined
                if (inputValue == undefined) return ''
                var transformedInput = inputValue.replace(/[^0-9]/g, '');
                if (transformedInput != inputValue) {
                    modelCtrl.$setViewValue(transformedInput);
                    modelCtrl.$render();
                }

                return transformedInput;
            });
        }
    }
});

app.filter('ordinalnumber', function () {
    return function (input) {
        var s = ["th", "st", "nd", "rd"], v = input % 100;
        return input + (s[(v - 20) % 10] || s[v] || s[0]);
        //var num = parseInt(input, 10);
        //return isNaN(num) ? input : ordinal.english(num);
    };
});

angular.module('app').directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.ngEnter);
                });

                event.preventDefault();
            }
        });
    };
});

//it is used for validate  custom url like roma.com as valid. if http://roma.com is also valid. but only roma will be invalid.
app.directive('myUrl', function () {
    function link(scope, element, attrs, ngModel) {
        function allowSchemelessUrls() {
            var URL_REGEXP = /^((?:http|ftp)s?:\/\/)(?:(?:[A-Z0-9](?:[A-Z0-9-]{0,61}[A-Z0-9])?\.)+(?:[A-Z]{2,6}\.?|[A-Z0-9-]{2,}\.?)|localhost|\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})(?::\d+)?(?:\/?|[\/?]\S+)$/i;
            ngModel.$parsers.unshift(function (value) {
                if (!URL_REGEXP.test(value) && URL_REGEXP.test('http://' + value)) {
                    return 'http://' + value;
                } else {
                    return value;
                }
            });
            ngModel.$validators.url = function (value) {
                return ngModel.$isEmpty(value) || URL_REGEXP.test(value);
            };
        }
        if (ngModel && attrs.type === 'url') {
            allowSchemelessUrls();
        }
    }

    return {
        require: '?ngModel',
        link: link
    };
});


app.filter('setDecimal', function ($filter) {
    return function (input, places) {
        if (isNaN(input)) {
            input = 0;
            return input.toFixed(places);
        }
        else {
            var n = parseFloat(input);
            if (isNaN(n)) {
                n = 0;
            }
            return n.toFixed(places);
        }
    };
});

app.directive('dirFieldDisabled', function () {
    return {
        compile: function (tElem, tAttrs) {
            var inputs = tElem.find('input');
            var selects = tElem.find('select');
            var textareas = tElem.find('textarea');
            var buttons = tElem.find('button');

            if (!Isundefinedornull(inputs)) {
                inputs.attr('ng-disabled', tAttrs['dirFieldDisabled']);
                inputs.each(function (index, element) {
                    if (($(this).hasClass("always-enable"))) {
                        $(this).attr('ng-disabled', false);
                    }
                    if (($(this).hasClass("always-disable"))) {
                        $(this).attr('ng-disabled', true);
                    }
                });
            }

            if (!Isundefinedornull(selects)) {
                selects.attr('ng-disabled', tAttrs['dirFieldDisabled']);

                selects.each(function (index, element) {
                    if (($(this).hasClass("always-enable"))) {
                        $(this).attr('ng-disabled', false);
                    }
                    if (($(this).hasClass("always-disable"))) {
                        $(this).attr('ng-disabled', true);
                    }
                });
            }

            if (!Isundefinedornull(textareas))
                textareas.attr('ng-disabled', tAttrs['dirFieldDisabled']);
            if (!Isundefinedornull(buttons)) {
                buttons.attr('ng-disabled', tAttrs['dirFieldDisabled']);

                buttons.each(function (index, element) {
                    if (!IsUndefinedNullOrEmpty($(this).attr("name")) && ($(this).attr("name") == "btnPrevious" || $(this).attr("name") == "btnNext")) {
                        $(this).attr('ng-disabled', false);
                    }
                });
            }
        }
    }
});

//return default page object. use this function in all virtual page
//so that we can manage the page size from one place only.
function GetDefaultPageObject() {
    var RecordPerPage = 5;
    try {
        if (!Isundefinedornull(userPreferencePagesize)) {
            RecordPerPage = parseInt(userPreferencePagesize, 10);
        }
    } catch (e) {
        RecordPerPage = 5
    }
    var page = {};
    page = {
        PageNo: 1,
        PageSize: RecordPerPage,
        TotalRecords: 0,
        Criteria: null //setting Criteria to null because it can be any object.
    };
    return page;
}

function stripHTML(html) {
    var tmp = document.createElement("DIV");
    tmp.innerHTML = html;
    return tmp.textContent || tmp.innerText || "";
}

function setfooterBottonPublic() {

    var obj = $(".mainContainer");
    var windowHeight = $(window).height();
    var footerHeight = $(".footer").height() + 30; // 30 is the padding 
    var heightCompare = windowHeight - footerHeight - 5; // 5 is the top border
    obj.css({ 'min-height': heightCompare, 'padding-bottom': footerHeight + 20 });
    //if ($(".mainButton").length > 0) {
    //    $(".mainButton").css({ 'height': $(".footer").height() + 30 });
    //}
}

function IsValidClientPublicSite() {
    var returnValue = false;
    if ($(location).attr('href').toLowerCase().indexOf("/site/") >= 0) {
        returnValue = true;
    }
    return returnValue;
}

function IsValidClientPublicSiteFromPupup() {
    var returnValue = false;
    if (window.parent != null && window.parent.location != null && $(window.parent.location).attr('href').toLowerCase().indexOf("/site/") >= 0) {
        returnValue = true;
    }
    return returnValue;
}
function GetBasePath(common) {
    var returnValue = common.$location.protocol() + "://" + common.$location.host() + ":" + common.$location.port();
    if (IsValidClientPublicSite()) {
        returnValue = returnValue + "/Site/" + ProductUrlId;
    }

    return returnValue;
}

function GetAdminBasePath(common) {
    return common.$location.protocol() + "://" + common.$location.host() + ":" + common.$location.port();
}

function GetLookup(lookupName) {
    var returnValue = null;
    if (Lookups != null) {
        returnValue = _.find(Lookups, function (lookup) { return lookup.Name == lookupName });
        if (returnValue != null)
            returnValue = returnValue.Data;
    }
    return returnValue;
}

function GetAPQPStepName(APQPStep) {
    var returnValue = "KickOff";
    switch (APQPStep) {
        case "APQPSTEP1":
            returnValue = "KickOff";
            break;
        case "APQPSTEP2":
            returnValue = "ToolingLaunch";
            break;
        case "APQPSTEP3":
            returnValue = "ProjectTracking";
            break;
        case "APQPSTEP4":
            returnValue = "PpapSubmission";
            break;
        default:
            break;
    }
    return returnValue;
}
//defect tracking
function GetDTStepName(DTStep) {
    var returnValue = "DTPartDocument";
    switch (DTStep) {
        case "DTSTEP1":
            returnValue = "DTPartDocument";
            break;
        case "DTSTEP2":
            returnValue = "DTCorrectiveAction";
            break;
        default:
            break;
    }
    return returnValue;
}

//get apqp step name by tab name
function GetAPQPStepNameByTabName(APQPTab) {
    var returnValue = "APQPSTEP1";
    switch (APQPTab) {
        case "KickOff":
            returnValue = "APQPSTEP1";
            break;
        case "ToolingLaunch":
            returnValue = "APQPSTEP2";
            break;
        case "ProjectTracking":
            returnValue = "APQPSTEP3";
            break;
        case "PpapSubmission":
            returnValue = "APQPSTEP4";
            break;
        default:
            break;
    }
    return returnValue;
}

/*
app.directive('href', ['$location', '$route', function ($location, $route) {
    return {
        restrict: 'A',
        link: function ($scope, $element, $attrs) {
            $element.bind('click', function () {
                if ($location.$$url == $attrs.activeLink) {
                    $route.reload();
                } else {
                    $location.path($attrs.activeLink);
                }
            });
        }
    };
}]);
*/