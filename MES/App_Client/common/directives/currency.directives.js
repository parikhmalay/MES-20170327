var app = angular.module('app');

app.directive('formatcurrency', ['$filter', function ($filter) {
    return {
        require: '?ngModel',
        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;

            elem.bind('blur', function (viewValue) {
                if (attrs.formatcurrency == 'currency') {
                    if (typeof ctrl.$modelValue == 'undefined' || ctrl.$modelValue == null)
                        ctrl.$modelValue = 0;
                    var plainNumber = ctrl.$modelValue.toString();
                    plainNumber = plainNumber.replace(/[^\d|\-+|\.+]/g, '');
                    elem.val(symbol + $filter('number')(plainNumber, 2));
                }
                else if (attrs.formatcurrency == 'decimal' ) {
                    if (typeof ctrl.$modelValue == 'undefined' || ctrl.$modelValue == null)
                        ctrl.$modelValue = 0;
                    var plainNumber = ctrl.$modelValue.toString();
                    plainNumber = plainNumber.replace(/[^\d|\-+|\.+]/g, '');
                    elem.val($filter('number')(plainNumber, 2));
                }
            });
            var symbol = "$"; // dummy usage

            ctrl.$formatters.unshift(function (a) {
                if (typeof ctrl.$modelValue != 'undefined')
                    if (attrs.formatcurrency == 'currency') {
                        return $filter(attrs.formatcurrency)(ctrl.$modelValue);
                    }
                    else if(attrs.formatcurrency == 'decimal')
                    {
                        return  ($filter('number')(ctrl.$modelValue, 2));
                        //$filter(attrs.formatcurrency)(ctrl.$modelValue);
                    }

            });

            ctrl.$parsers.unshift(function (viewValue) {
                var plainNumber = viewValue.replace(/[^\d|\-+|\.+]/g, '');
                return plainNumber;
            });
        }
    };
}]);