app.directive('whenScrolled', function () {
    return {
        controller: function ($scope) {

        },
        link: function (scope, elm, attr) {
            console.log('In vs');
            var raw = elm[0];
            elm.bind('scroll', function () { if (raw.scrollTop + raw.offsetHeight >= raw.scrollHeight) { scope.$apply(attr.whenScrolled); } });
        }
    }
});