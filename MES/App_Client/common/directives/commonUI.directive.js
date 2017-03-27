//app.directive("maxHeight", ['$window', function (window) {
//    var DDO = {
//        restrict: 'A',
//        link: function (scope, element, attr) {
//            var obj = element[0];
//            var minHeight = 0;
//            var adjAmount = 55;
//            var topPos;
//            var heightAttr = "style";
//            var heightProperty = "min-height:";
//            var unit = "px";
//            switch (attr.maxHeight) {
//                case "setForEditor":
//                    setHeight("ckeditorReady");
//                    adjAmount = 65;
//                    unit = heightProperty = "";
//                    heightAttr = "editorheight";
//                    break;
//                case "setForSpinner":
//                    setHeight("us-spinner:stop");
//                    break;
//                default:
//                    console.log("max-height attribute value is wrong.")
//                    ;
//            }
//            if (attr.mxHeightAttribute) {
//                heightProperty = attr.mxHeightAttribute + ":";
//            }
//            if (attr.mxAdjAmount) {
//                adjAmount = attr.mxAdjAmount;
//            }
//            function setHeight(broadcastName) {
//                if (broadcastName != "") {
//                    scope.$on(broadcastName, function () {
//                        topPos = $(obj).offset();
//                        minHeight = window.Math.ceil((window.screenHeight - adjAmount) - topPos.top);
//                        obj.setAttribute(heightAttr, heightProperty + minHeight + unit);
//                    });
//                }
//            }
//        },
//    }
//    return DDO;
//}]);

//app.directive("helptext", [function () {
//    var DDO = {
//        restrict: 'E',
//        transclude: true,
//        replace: true,
//        scope: true,
//        templateUrl: '/App_Client/common/directives/HelpText/helpText.html',
//    }
//    return DDO;
//}]);
//app.directive("collapseBtn", ['$timeout', function ($timeout) {
//    var DDO = {
//        restrict: 'C',
//        link: function (scope, element, attr) {
//            var e = element[0];
//            $timeout(function () {
//                e.onclick = function () {
//                    globalFunctions.navSwitch();
//                }
//            }, 1000);
//        }
//    }
//    return DDO;
//}]);

//app.directive("subnav", ['$timeout', function ($timeout) {
//    var DDO = {
//        restrict: 'C',
//        link: function (scope, element, attr) {
//            var e = element[0];
//            var isActive = false;
//            $timeout(function () {
//                isActive = $(element).hasClass("ng-hide");
//                if (!isActive) {
//                    globalFunctions._isLeftPanel = true;
//                    if (globalFunctions.isIPADResolution()) {
//                        globalFunctions.navMin();
//                    } else {
//                        if (globalFunctions.getCookie("navSliderPos") == "open" || globalFunctions.getCookie("navSliderPos") == null || globalFunctions.getCookie("navSliderPos") == "") {
//                            globalFunctions.navMax();
//                        } else {
//                            globalFunctions.navMin();
//                        }
//                    }
//                }
//            }, 1000);
//        }
//    }
//    return DDO;
//}]);

//app.directive("topdropdown", [function () {
//    var DDO = {
//        restrict: 'A',
//        link: function (scope, element, attr) {
//            var e = element[0];
//            $(e).click(function () {
//                if ($(e).index() != $('.topDropdownMenu.open').index()) {
//                    $('.topDropdownMenu.open').removeClass('open').attr('rel', 'close');
//                }
//                if ($(e).attr("rel") != "open") {
//                    openMenu();
//                    $('#wrapper').bind('click', function () {
//                        closeMenu();
//                    })
//                } else {
//                    closeMenu();

//                }
//            });

//            function openMenu() {
//                $(e).attr("rel", "open");
//                $(e).addClass("open");
//            }
//            function closeMenu() {
//                $(e).removeClass("open");
//                $(e).attr("rel", "close");
//                $('#wrapper').unbind('click');
//            }
//        }
//    }
//    return DDO;
//}]);

//app.directive("slick", ["$timeout", function ($timeout) {
//    var DDO = {
//        restrict: 'A',
//        link: function (scope, element, attr) {
//            var showSlider = attr.allowslider || 'false';
//            if (showSlider == 'true') {
//                if (scope.$last === true) {
//                    $timeout(function () {
//                        scope.$emit('ngRepeatFinished');
//                    });
//                }
//                scope.$on('ngRepeatFinished', function (ngRepeatFinishedEvent) {
//                    $(element[0].parentElement).slick({
//                        dots: false,
//                        infinite: false,
//                        speed: 300,
//                        slidesToShow: 1,
//                        centerMode: false,
//                        variableWidth: true
//                    });
//                });
//            }
//        }
//    }
//    return DDO;
//}]);

//app.directive("list1", ["$timeout", function ($timeout) {
//    var DDO = {
//        restrict: 'C',
//        link: function (scope, element, attr) {
//            $timeout(function () {
//                globalFunctions.resetLists(element);
//            }, 1000);
//        }
//    }
//    return DDO;
//}]);


app.directive("callout", ['$window', '$timeout', function ($window, $timeout) {
    /* don't use "data-callout" otherwise style will not apply */
    var DDO = {
        restrict: 'A',
        scope: false,
        link: function (scope, element, attrs) {
            var textAlign = attrs.callout || "left"; /* popup bottom align based on text direction */
            var _obj = $(element[0]);
            var _objPopup = _obj.find(" > .popout-window");
            var _margin = parseInt(attrs.calloutMargin || 16);
            var _objWidth = 0;
            var _objHeight = 0;
            var _pos = 0;
            var _newTopPos = 0;
            var _popWidth = 0;
            var _popHeight = 0;
            var _popMaxWidth = attrs.calloutMaxWidth || null;
            var _calloutHoldPopup = attrs.calloutHoldPopup || null;
            var _calloutReleasePopup = attrs.calloutReleasePopup || null;
            _obj.removeClass("open-right").removeClass("open-left").removeClass("open-bottom").removeClass("open-top");
            // added global variable resolving quick hovering issue.
            window["callout-watcher"] = false;
            _obj.mouseenter(function () {
                $(".popout-window").attr("style", "");
                mouseEnter();
            }).mouseleave(function () {
                mouseLeave();
            });

            function mouseEnter() {
                _objPopup.css({ "visibility": "hidden", "position": "fixed", "z-index": 1000 });
                if (_popMaxWidth != null) {
                    _objPopup.css({ "max-width": _popMaxWidth });
                }
                setPos();
            }

            function mouseLeave() {
                $(".popout-window").attr("style", "");
                _obj.removeClass("open");
                _objWidth = _objHeight = _pos = _newTopPos = _popWidth = _popHeight = 0;
                _obj.removeClass("callout-left").removeClass("callout-right").removeClass("callout-bottom");
            }

            function setPos() {
                _objWidth = _obj.width();
                _objHeight = _obj.height();
                _pos = _obj.offset();
                _newTopPos = _pos.top - $($window).scrollTop();
                _popWidth = _objPopup.width();
                _popHeight = _objPopup.height();

                /* set top position */
                var vDirection = {};
                if (getYSide(_popHeight + _newTopPos)) {
                    vDirection = { "top": (_newTopPos - _popHeight - _objHeight - _margin) };
                    if (textAlign == "left") {
                        setXSide((_pos.left + 10) - _popWidth, _pos.left);
                    } else if (textAlign == "center") {
                        setXSide((_pos.left + (_objWidth / 2)) - _popWidth, _pos.left + ((_objWidth / 2) - 20));
                    } else {
                        setXSide((_pos.left + _objWidth - 20) - _popWidth, _pos.left + (_objWidth - 30));
                    }
                    _obj.addClass("callout-bottom");
                } else {
                    vDirection = { "top": _newTopPos - 8 };
                    setXSide((_pos.left - _popWidth - _margin - 14), (_pos.left + _objWidth + _margin));
                }
                _objPopup.css(vDirection);
                _objPopup.css({ "visibility": "visible" });
            }

            function getXSide() {
                var side = true; /* True is left side */
                if ((_objWidth + _pos.left + _popWidth) < $window.screenWidth) {
                    side = false;
                }
                return side;
            }
            function getYSide(height) {
                var side = true; /* True is top side */
                if (height < $window.screenHeight) {
                    side = false; /* bottom */
                }
                return side;
            }
            function setXSide(leftVal, rightVal) {
                var hDirection = {};
                if (getXSide()) {
                    hDirection = { "left": leftVal };
                    _obj.addClass("callout-left");
                } else {
                    hDirection = { "left": rightVal };
                    _obj.addClass("callout-right");
                }
                _objPopup.css(hDirection);
            }
        }
    }
    return DDO;
}]);
