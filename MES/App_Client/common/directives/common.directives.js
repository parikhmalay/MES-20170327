//app.directive('customPopover', function () {
//    return {
//        restrict: 'A',
//        template: '<span>{{label}}</span>',
//        link: function (scope, el, attrs) {
//            console.log("Test")
//            scope.label = attrs.popoverLabel;
//            $(el).popover({
//                trigger: 'mouseenter',
//                html: true,
//                content: attrs.popoverHtml,
//                placement: attrs.popoverPlacement
//            });
//        }
//    };
//});

//app.directive("helpText", [function () {
//    var DDO = {
//        restrict: 'E',
//        transclude: true,
//        replace: true,
//        scope: true,
//        templateUrl: '/App_Client/common/directives/HelpText/helpText.html?v=' + Version,
//    }
//    return DDO;
//}]);

app.directive("accessibleForm", [function () {
    return {
        restrict: 'A',
        scope: true,
        link: function (scope, elem) {

            // set up event handler on the form element
            elem.on('submit', function () {
                // find the first invalid element
                var firstInvalid = elem[0].querySelector('.ng-invalid');
                var firstInvalidSupplierQuote = elem[0].querySelector('.ng-invalid.hiddenfromui');

                // if we find one, set focus
                if (firstInvalidSupplierQuote)
                    firstInvalidSupplierQuote.focus();
                else if (firstInvalid) {
                    firstInvalid.focus();
                }
                else {
                    firstInvalid = elem[0].querySelector('.invalid-class');
                    if (firstInvalid) {
                        firstInvalid.focus();
                    }
                }
            });
        }
    };
}]);