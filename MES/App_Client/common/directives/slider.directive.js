app.directive('slider', ['$compile', "$animate", function ($compile, $animate) {
    return {
        restrict: 'E',
        transclude: true,
        replace: true,
        scope: {
            sModel: "=sliderModel"
        },
        template: function (tElement, tAttrs) {
            var _align = tAttrs.lsAlign || "right";
            var _size = tAttrs.sliderSize || "default";
            var template = '<div ng-show="sModel" ng-animate="\'animate\'" class="slider-' + _align + ' size-' + _size + ' slideIn slideOut">';
            template += '<div class="slider-container">' +
              //'<div ng-transclude>' +
              '</div></div></div>';
            return template;
        }
    };
}]);