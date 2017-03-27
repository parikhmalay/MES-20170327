var windowWidth = window["screenWidth"] = innerWidth;
var windowHeight = window["screenHeight"] = innerHeight;
var columnWidth;
var columnMove;
/*-------------------------------------------------------------------------*/
/*-----------------------MAIN FRAME SCRIPT HERE----------------------------*/
/*-------------------------------------------------------------------------*/

var advanceSearch = {
    open: function () {
        $('body').addClass('search-advance-opened');
        // $('body').addClass('sidebar-opened').removeClass('sidebar-minimize');
    },
    close: function () {
        $('body').removeClass('search-advance-opened');
        //$('body').addClass('sidebar-minimize').removeClass('sidebar-opened');
    },
    funEscape: function () {
        document.onkeydown = function (evt) {
            evt = evt || window.event;
            var isEscape = false;
            if ("key" in evt) {
                isEscape = evt.key == "Escape";
            } else {
                isEscape = evt.keyCode == 27;
            }
            if (isEscape) {
                advanceSearch.close();
            }
        };
    }
}

var RedirecttoDashboard = {
    Go: function () {
        window.location.href = "/";
    }
};




$(document).ready(function () {

    /*Burger Menu*/
    $("a.accordion-toggle").click(function () {
        //  alert("Hi");
        $(this).parent().parent().toggleClass('active');
    });

    $('#advanceSearchOpen').click(function () {
        advanceSearch.open();

        // Escape Button Close slider 
        document.onkeydown = function (evt) {
            evt = evt || window.event;
            var isEscape = false;
            if ("key" in evt) {
                isEscape = evt.key == "Escape";
            } else {
                isEscape = evt.keyCode == 27;
            }
            if (isEscape) {
                advanceSearch.close();
            }
        };
    });

    //$("#profileopen").hover(function () {
    //    $(".user-drop-menu").toggle();
    //});
    //$(".user-drop-menu a").click(function () {
    //    $(".user-drop-menu").hide();
    //});



    $("#content-part-main-panel").scroll(function () {
        if ($(this).scrollTop() > 200) {
            $('.scrollToTop').fadeIn();
        } else {
            $('.scrollToTop').fadeOut();
        }
    });
    //Click event to scroll to top
    $('.scrollToTop').click(function () {
        $("#content-part-main-panel").animate({ scrollTop: 0 }, 800);
        return false;
    });



});
/*-----Screen Height Start--------*/
app.directive("setScreenHeightCoomon", ['$timeout', function ($timeout) {
    var DDO = {
        restrict: 'C',
        link: function (scope, element, attr) {
            $timeout(function () {
                resizeContent();
                $(window).resize(function () {
                    resizeContent();
                });
                function resizeContent() {
                    var marginMinus = $('.box-in-body').attr('margin-minus');
                    // console.log("Screen Height", marginMinus);
                    $height = $(window).height() - marginMinus;
                    $('.set-screen-height-box').css('max-height', ($height));
                }
            }, 1000);
        }
    }
    return DDO;
}]);
/*-----Screen Height End--------*/
//app.directive("dropMenuOpenPanel", ['$timeout', function ($timeout) {
//    var DDO = {
//        restrict: 'A',
//        link: function (scope, element, attr) {
//           // alert("Hi");
//            $timeout(function () {
//                $("input", document.body).on("click blur focus", function(event){
//                        var offset = $(this).offset();
//                        var posLeft = 0;
//                        var posTop = 0;
//                        event.stopPropagation();
//                        posTop = offset.left;
//                        top = offset.top;
//                        $(".dropdown-menu").css({ "left": offseF });
//                })
//            }, 1000);
//        }
//    }
//    return DDO;
//}]);
/*-------------------------------------------------------------------------*/
/*----------------------------START DIRECTIVES-----------------------------*/
/*-------------------------------------------------------------------------*/
window["commonDragDivInitialHeight"] = 0;
app.directive("autoHeightGrid", ['$timeout', function ($timeout) {
    var DDO = {
        restrict: 'C',
        link: function (scope, element, attr) {
            $timeout(function () {
                $('.drag-section-panel').on('mousedown', function (e) {
                    var $dragable = $('.drag-common-panel'),
                        $fixedDiv = $(".auto-height-grid"),
                    firstDivHeight = $dragable.height(),
                    fixedDivHeight = $(".fixed-height-grid").height()
                    pX = e.pageY;
                    $(document).on('mouseup', function (e) {
                        $(document).off('mouseup').off('mousemove');
                    });
                    $(document).on('mousemove', function (me) {
                        var mx = (me.pageY - pX);
                        //var my = (me.pageY - pY);
                        // console.log(firstDivHeight + mx);
                        // $(".drag-common-panel").css('max-height', firstDivHeight);
                        $dragable.height(firstDivHeight + mx);
                        $fixedDiv.height(fixedDivHeight - mx);
                    });
                });

                // element.niceScroll({ touchbehavior: false, cursoropacitymax: 1, autohidemode: true, hidecursordelay: 2000, horizrailenabled: false });

                if (window["commonDragDivInitialHeight"] <= 0) {
                    window["commonDragDivInitialHeight"] = $(".drag-common-panel").height();
                }

                resizeContent();
                $(window).resize(function () {
                    resizeContent();
                });

                function resizeContent() {


                    var heightFixedCols = $('.sliderOn').attr('height-fixed-cols');
                    $(".drag-common-panel").css('max-height', '' + heightFixedCols + 'px');
                   // console.log('test', heightFixedCols)

                    $height = $(window).height() - 390;
                    //  $('.fixed-height-grid').height($height);
                    //  $(".drag-common-panel").height(window["commonDragDivInitialHeight"]);
                    // $(".drag-common-panel").css('max-height', $height) - 200;
                    $('.fixed-height-grid').css('height', $height);
                    $('.fixed-height-grid').css('min-height', $height);
                }
                $(".fixed-content-top .panel-title").click(function () {
                    // alert("HI");
                    $(".fixed-content-top-margin").toggleClass("fixed-content-top-margin-remove");
                });

            }, 1000);
            scope.$on("loadScrollerPanel", function () {
                $timeout(function () {
                    //   var marginMinus = $('.sliderOn').attr('margin-minus');
                    //if (window["commonDragDivInitialHeight"] <= 0) {
                    //    window["commonDragDivInitialHeight"] = $(".drag-common-panel").height();
                    //}
                    resizeContent();
                    $(window).resize(function () {
                        resizeContent();
                    });
                    function resizeContent() {

                        var marginMinus = $('.sliderOn').attr('margin-minus');
                        var heightFixedCols = $('.sliderOn').attr('height-fixed-cols');
                       // console.log('test', heightFixedCols)
                        // var heightFixedCols = $('.sliderOn').attr('height-fixed-cols');
                        //  console.log("Screen Height", heightFixedCols);
                        //  console.log('max-height', ($height)-200);
                        //  $(".drag-common-panel").css('max-height', heightFixedCols);
                        //  $(".drag-common-panel").height(window["commonDragDivInitialHeight"]);
                        $height = $(window).height() - marginMinus;
                        $('.fixed-height-grid').css('height', $height);
                        $('.fixed-height-grid').css('min-height', $height);
                        // element.niceScroll().resize();
                        //  element.scrollTop(0);
                    }
                    $(".fixed-height-grid").scroll(function () {
                        if ($(this).scrollTop() > 200) {
                            $('.scrollToTop').fadeIn();
                        } else {
                            $('.scrollToTop').fadeOut();
                        }
                    });

                    $('.scrollToTop').click(function () {
                        $(".fixed-height-grid").animate({ scrollTop: 0 }, 800);
                        return false;
                    });
                }, 1100);
            });
            $(".burger-Panel #mobileMenu").click(function () {
                $(".left-pop-up-panel").toggleClass('widthZero');
                $(".right-pop-up-panel").toggleClass('widthFull');
            });
        }
    }
    return DDO;
}]);


app.directive("clickColPanel", ['$timeout', function ($timeout) {
    var DDO = {
        restrict: 'C',
        link: function (scope, element, attr) {
            $(".commonColPanel").hide();
            $(element).click(function () {
                $(this).parent().parent().parent().parent().find(".commonColPanel").slideToggle("slow");

                return false;
            });
        }
    }
    return DDO;
}]);

app.directive("toolTipTitleCommon", ['$timeout', function ($timeout) {
    var DDO = {
        restrict: 'C',
        link: function (scope, element, attr) {
            $('.ng-pristine, label').tooltip({ position: 'b', cls: 'custom-tooltip' }); //cls: 'custom-tooltip'
            $('.yellow-tooltip .ng-pristine, .yellow-tooltip label, .yellow-tooltip select, .yellow-tooltip a, .yellow-tooltip input, .yellow-tooltip p').tooltip({ position: 'b', cls: 'custom-tooltip' });
            $timeout(function () {
                //  $('.ng-pristine, label').tooltip({ position: 'b', cls: 'custom-tooltip' });
                $('.yellow-tooltip .ng-pristine, .yellow-tooltip select, .yellow-tooltip label, .yellow-tooltip a, .yellow-tooltip input').tooltip({ position: 'b', cls: 'custom-tooltip' });
                resizeContent();
                $(window).resize(function () {
                    resizeContent();
                });
                function resizeContent() {
                    $('.yellow-tooltip  .ng-pristine, .yellow-tooltip select, .yellow-tooltip label, .yellow-tooltip a, .yellow-tooltip input').tooltip({ position: 'b', cls: 'custom-tooltip' });
                }
            }, 1100);
        }
    }
    return DDO;
}]);

app.directive("clickLabelEdit", ['$timeout', function ($timeout) {
    var DDO = {
        restrict: 'C',
        link: function (scope, element, attr) {
            $('.edit-label').on('click', function () {
                //$(this).parent().hide();
                $(this).parent().parent().find(".edit-mode-text").hide();
                $(this).parent().parent().find(".edit-mode-input").show(); //find(".edit-mode-input")
            });
            $('.close-label, .save-label').on('click', function () {
                // $(".edit-mode-input").hide();
                $(this).parent().parent().find(".edit-mode-input").hide();
                $(this).parent().parent().find(".edit-mode-text").show();// find(".edit-mode-text")
            });
        }
    }
    return DDO;
}]);


/*-------------------------------------------------------------------------*/
/*----------------------------Slider For Custome-----------------------------*/
/*-------------------------------------------------------------------------*/
app.directive("row", ['$timeout', function ($timeout) {
    var DDO = {
        restrict: 'A',
        controller: function ($scope, $element, $attrs) {
            columnWidth = $scope.columnWidth = parseInt($attrs.columnWidth);
            columnMove = parseInt($attrs.columnMove);
        },
        link: function (scope, element) {
            var setWidth = 0;
            var outerWidth = 0;
            
            var scrollDivObj = ".scroller-panel";
            $(function () {
                $(".scroller-panel").on('scroll', function (e) {
                    scrollDiv(e.currentTarget, 0);
                });
                /*$("#next").click(function(){
                      scrollDiv(scrollDivObj, 1);
                  });
                    $("#prev").click(function(){
                      scrollDiv(scrollDivObj, 2);
                  });*/
            });
            function scrollDiv(a, b) {
                var obj = $(a);
                var left = obj.scrollLeft();
                //var top = obj.scrollTop();

                if (a == scrollDivObj) {
                    if (b == 1) {
                        $(scrollDivObj).scrollLeft(left + 10);
                    } else {
                        $(scrollDivObj).scrollLeft(left - 10);
                    }
                } else {
                    $(scrollDivObj).scrollLeft(left);
                }
            }
            scope.$watch("RFQSupplierPartQuoteList", function (n, o) {
                outerWidth = $(".scroller-panel").first().width();
                $(".scroller-panel").scrollLeft(0);
                $(".scroller-panel").scrollTop(0);

                if (!angular.isUndefined(scope.RFQSupplierPartQuoteList) && scope.RFQSupplierPartQuoteList != null)
                    setWidth = parseInt(scope.columnWidth) * scope.RFQSupplierPartQuoteList.length;
                if (!angular.isUndefined(n)) {
                    $(element).width(setWidth);
                    if (setWidth <= outerWidth) {
                        $(".leftRightButton").hide();
                        // alert("1");
                    } else {
                        $(".leftRightButton").show();
                        //  alert("2");
                    }
                }
            });
        }
    }
    return DDO;
}]);



//function scrollColumnFocus(move) {
//    $('.scroller-panel').each(function () {
//        var scrollDiv = $(this);
//        scrollDiv.animate({ scrollLeft: move }, 100);
//    });
//}

function scrollColumn(x) {
    $('.scroller-panel').trigger("scrollDiv");
    $('.scroller-panel').each(function () {
        var scrollDiv = $(this);
        var move = parseInt(scrollDiv.scrollLeft());
        // console.log("init Move -> " + columnMove);
        if (x) {
            //console.log("Left" + move);
            move = move - (columnWidth * columnMove);
        } else {
            // console.log("Left" + move);
            move = move + (columnWidth * columnMove);
        }
        scrollDiv.animate({ scrollLeft: move }, 100);
    });
}

/*--Dropment--*/
app.directive("subMenuSlide", ['$timeout', function ($timeout) {
    var DDO = {
        restrict: 'C',
        link: function (scope, element, attr) {
            angular.element(document).ready(function () {
                var isSliderOpen = $("body").hasClass("sidebar-opened") || false;
                var isMainMenuOpen = $(".menu-box li").hasClass("open-sub-menu") || false;
                var minMaxSliderObject = $("#mobileMenu");
                var backButtonObject = $(".back-menu");
                var backBodyObject = $(".body-click-hide");
                var backMenuObject = $(".menu-box > ul > li ul li");
                var subMenuObject = $(".menu-box > ul > li a.sub-menu");
                var leftSidebar = $("#left-sidebar");
                var targetDivForScroll = $(".menu-part");
                var subMenuTargetScroll = null;
                var logoMargin = 70;
                $timeout(function () {
                    var windowResizeID = 0;

                    addEvent(window, "resize", function (event) {
                        clearTimeout(windowResizeID);
                        windowResizeID = setTimeout(windowResized, 1000)
                    });
                    function windowResized() {
                        windowResize();
                    };

                    function checkScrollAvailiblity(object) {
                        var height = parseInt(object.attr("data-height"));
                        var css = {};
                        if ((height + logoMargin) >= windowHeight) {
                            unBindLeftNavScroll(object);
                            bindLeftNavScroll(object);
                            css = { height: windowHeight - logoMargin };
                        } else {
                            unBindLeftNavScroll(object);
                            css = { height: "auto" };
                        }
                        return css;
                    }

                    function init(obj) {
                        if (obj.attr("data-height") != undefined) {
                            obj.css(checkScrollAvailiblity(obj));
                        } else {
                            obj.attr("data-height", obj.height());
                            obj.css(checkScrollAvailiblity(obj));
                        }
                    }

                    function doAction(scrollObj) {
                        if (isSliderOpen) {
                            if (scrollObj == targetDivForScroll) {
                                $('body').addClass('sidebar-opened').removeClass('sidebar-minimize');
                                setTimeout(function () {
                                    if (subMenuTargetScroll) {
                                        unBindLeftNavScroll(subMenuTargetScroll);
                                        bindLeftNavScroll(subMenuTargetScroll);
                                    }
                                }, 100)
                            }
                            setTimeout(function () {
                                init(scrollObj);
                            }, 100);

                        }
                        else {
                            if (scrollObj == targetDivForScroll) {
                                $('#left-sidebar > .menu-part > .menu-box > ul > li').removeClass('open-sub-menu');
                                $('body').addClass('sidebar-minimize').removeClass('sidebar-opened');
                                $('.menu-box > ul').removeClass('slide-menu');
                                setTimeout(function () {
                                    unBindLeftNavScroll(subMenuTargetScroll);
                                    unBindLeftNavScroll(targetDivForScroll);
                                }, 100)
                            }
                            setTimeout(function () {
                                init(scrollObj);
                            }, 100);

                        }
                    }

                    subMenuObject.click(function () {

                        isSliderOpen = $("body").hasClass("sidebar-opened");
                        $('#left-sidebar > .menu-part > .menu-box > ul > li').removeClass('open-sub-menu');
                        $('.menu-box > ul').addClass('slide-menu');
                        $(this).parent().addClass('open-sub-menu');
                        subMenuTargetScroll = $(".slide-menu .open-sub-menu > ul");
                        doAction(subMenuTargetScroll);
                        //setTimeout(function () {

                        //}, 100);
                    });
                    backButtonObject.click(function () {
                        isSliderOpen = $("body").hasClass("sidebar-opened");
                        $('.menu-box > ul').removeClass('slide-menu');
                        //$('#left-sidebar > .menu-part > .menu-box > ul > li').removeClass('open-sub-menu');
                        setTimeout(function () {
                            $('.menu-box li').removeClass('open-sub-menu');
                            unBindLeftNavScroll(subMenuTargetScroll);
                        }, 800);
                    });
                    backBodyObject.click(function () {
                        isSliderOpen = $("body").hasClass("sidebar-opened");
                        $('.menu-box > ul').removeClass('slide-menu');
                        //$('#left-sidebar > .menu-part > .menu-box > ul > li').removeClass('open-sub-menu');
                        setTimeout(function () {
                            $('.menu-box li').removeClass('open-sub-menu');
                            unBindLeftNavScroll(subMenuTargetScroll);
                        }, 400);
                    });
                    backMenuObject.click(function () {
                        isSliderOpen = $("body").hasClass("sidebar-opened");

                        setTimeout(function () {
                            $('.menu-box > ul').removeClass('slide-menu');
                            $('.menu-box > ul li').removeClass('open-sub-menu');
                            $('.menu-box li').removeClass('open-sub-menu');
                            unBindLeftNavScroll(subMenuTargetScroll);
                        }, 400);
                    });

                    document.onkeydown = function (evt) {
                        evt = evt || window.event;
                        var isEscape = false;
                        if ("key" in evt) {
                            isEscape = evt.key == "Escape";
                        } else {
                            isEscape = evt.keyCode == 27;
                        }
                        if (isEscape) {
                            isSliderOpen = $("body").hasClass("sidebar-opened");
                            $('.menu-box > ul').removeClass('slide-menu');
                            //$('#left-sidebar > .menu-part > .menu-box > ul > li').removeClass('open-sub-menu');
                            setTimeout(function () {
                                $('.menu-box li').removeClass('open-sub-menu');
                                unBindLeftNavScroll(subMenuTargetScroll);
                            }, 200);
                        }
                    };

                    minMaxSliderObject.click(function () {
                        isSliderOpen = !$("body").hasClass("sidebar-opened");
                        doAction(targetDivForScroll);
                    });

                    doAction(targetDivForScroll);
                });
            }, 1000);
        }
    }
    return DDO;
}]);

/* Start directives here */
app.directive("freezeBottom", ['$timeout', function ($timeout) {
    var DDO = {
        restrict: 'C',
        link: function (scope, element, attr) {
            $timeout(function () {
                $("body").addClass("haveBottomBar");
            }, 1000);
        }
    }
    return DDO;
}]);
app.directive("advanceSearchPanel", ['$timeout', function ($timeout) {
    var DDO = {
        restrict: 'C',
        link: function (scope, element, attr) {
            $timeout(function () {
                $("body").addClass("haveAdvanceSearch");
            }, 1000);
        }
    }
    return DDO;
}]);

app.directive("lstBody", ['$timeout', function ($timeout) {
    var DDO = {
        restrict: 'C',
        link: function (scope, element, attr) {
            $timeout(function () {
                resetLists()
            }, 1000);
        }
    }
    return DDO;
}]);
app.directive("fixedDatePicker", ['$timeout', function ($timeout) {
    var DDO = {
        restrict: 'C',
        link: function (scope, element, attr) {
            $timeout(function () {
                if ($(element).parents().hasClass("enable-fixed-date-picker")) {
                    fixedDatePicker();
                }
            }, 1000);
            function fixedDatePicker() {
                $(element).click(function () {
                    var topScroll = $(".ds-fs-container .ds-fs-item-container > .ds-fs-item.ds-Matrix").scrollTop();
                    var leftScroll = $(".ds-fs-container .ds-fs-item-container > .ds-fs-item.ds-Matrix").scrollLeft();
                    var bodyScroll = $(window).scrollTop();
                    var position = $(this).offset();
                    var leftPos = position.left;
                    var topPos = position.top - (topScroll + bodyScroll);
                    $(this).prev("[date-picker-wrapper]").css({ top: topPos + "px" });
                    $(this).prev("[date-picker-wrapper]").css({ left: leftPos + "px" });
                })
            }
        }
    }
    return DDO;
}]);

app.directive("dropdownLink", ['$timeout', function ($timeout) {
    var DDO = {
        restrict: 'C',
        link: function (scope, element, attr) {
            $timeout(function () {
                var dropdown = $(element).find(".dropdown-link-btn");
                DropdownClass(dropdown, ".dropdown-section", "dropdown-show", "data-drop");
            }, 1000);
        }
    }
    return DDO;
}]);


/*-------------------------------------------------------------------------*/
/*-------------START GLOBAL FUNCTIONS AND VARIABLES------------------------*/
/*-------------------------------------------------------------------------*/

var addEvent = function (object, type, callback) {
    if (object == null || typeof (object) == 'undefined') return;
    if (object.addEventListener) {
        object.addEventListener(type, callback, false);
    } else if (object.attachEvent) {
        object.attachEvent("on" + type, callback);
    } else {
        object["on" + type] = callback;
    }
};

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toGMTString();
    document.cookie = cname + "=" + cvalue + "; " + expires;
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i].trim();
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return "";
}


addEvent(window, "resize", function (event) {
    windowWidth = window["screenWidth"] = innerWidth;
    windowHeight = window["screenHeight"] = innerHeight;

});



var windowGetSetHeight = {
    get: function (Obj) {
        var objTopPos = $(Obj).offset().top;
        var returnHeight = window["screenHeight"] - objTopPos;
        return returnHeight;
    },
    getMerge: function (Obj, Obj2) {
        var objTopPos = $(Obj).offset().top + Obj2;
        var returnHeight = window["screenHeight"] - objTopPos;
        return returnHeight;
    },
    set: function (Obj, _height) {
        $(Obj).height(_height);
    },
    setmin: function (Obj, _height) {
        $(Obj).css({ minHeight: _height });
    }
}
var RomoveDefaultPadding = {
    get: function (Obj) {
        if ($("body " + Obj).length > 0) {
            $("body #m-container").css({ paddingBottom: "0px" });
        }
    },
    getCondition: function (Obj, InnerObj) {
        if ($(Obj).has(InnerObj).length == 0) {
            $(Obj).parents("body #m-container").css({ paddingBottom: "0px" });
        }
    },
    GeneralCondition: function (Obj, InnerObj) {
        if ($(Obj).find(InnerObj).length == 0) {
            $(Obj).css({ paddingBottom: "0px" });
        }
        else if ($(Obj).find(InnerObj).length > 0 && $(Obj).find(InnerObj).is(":visible") == true) {
            $(Obj).css({ paddingBottom: "50px" });
        }
        else {
            $(Obj).css({ paddingBottom: "0px" });
        }
    }
}

// Window resize funtion
function windowResize() {
    resetLists();
}

function bindLeftNavScroll(sObj) {
    // console.log("nicesecroll bind");
    setTimeout(function () {
        sObj.niceScroll({ touchbehavior: false, cursoropacitymax: 1, autohidemode: true, hidecursordelay: 2000, horizrailenabled: false });
    }, 1000)
}
function resetLeftNavScroll(scrollObj) {
    // console.log("nicesecroll resize");
    scrollObj.getNiceScroll().resize();
}

function unBindLeftNavScroll(scrollObj) {
    //  console.log("nicesecroll remove");
    $(scrollObj).getNiceScroll().remove();
}


/*-------- CHECK IS IPAD RESOLUTION OR NOT ------------------------*/
function isIPADResolution() {
    if (window["screenWidth"] < 769) {
        return true;
    } else {
        return false;
    }
}
function isIPADPortait() {
    if (window["screenWidth"] < 1025) {
        return true;
    } else {
        return false;
    }
}


// start list control script for iPad design
function resetLists(containerId) {
    if (window["screenWidth"] <= 768) {
        if (typeof containerId != 'undefined') {
            bindLabelOnPad(containerId);
        } else {
            bindLabelOnPad("");
        }

    } else {
        unBindLabelOnPad();
    }
}

function bindLabelOnPad(CID) {
    $(CID + " .list-1 .lst-item > div[data-colmn-title]").each(function () {
        var obj = $(this);
        if ($(this).find(".list-1-header").length <= 0) {
            obj.prepend("<div class='list-1-header'>" + obj.attr("data-colmn-title") + "</div>");
        }
    });
    $(CID + " .list-2 .lst-item > div[data-colmn-title]").each(function () {
        var obj = $(this);
        if ($(this).find(".list-1-header").length <= 0) {
            obj.prepend("<div class='list-1-header'>" + obj.attr("data-colmn-title") + "</div>");
        }
    });
    $(CID + " .multilevel-list-1 .lst-item > div[data-colmn-title]").each(function () {
        var obj = $(this);
        if ($(this).find(".list-1-header").length <= 0) {
            obj.prepend("<div class='list-1-header'>" + obj.attr("data-colmn-title") + "</div>");
        }
    });

}
function unBindLabelOnPad() {
    $(".list-1 .list-1-header").remove();
    $(".list-2 .list-1-header").remove();
    $(".multilevel-list-1 .list-1-header").remove();
}

