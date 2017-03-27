var app = angular.module('app', [
       'ngAnimate',        // animations
       'ngRoute',           // routing
       'common',            //Common Configuration for logger
       'ui.bootstrap',
       'gettext',           //Multilingual
       'ngLocale',
       'aa.formExtensions',
       'aa.notify',
       'ui.multiselect',
       'bootstrap-tagsinput', //Tag input Control
       'aa.select2',
       'ui.sortable',       //Drag-Drop List
       'angularSpinner', //, //Spinner/Loader
       'ngCookies',
        'ui.router',//,//,// UI routing.
        'angularFileUpload',
        'datePicker',         //DateTime Picker Control
       //'ngTagsInput'
       'angular-confirm',
       'highcharts-ng',
       'angularTreeview',
       'ngSanitize'
]);


//App configuration
app.config(['$httpProvider', '$locationProvider', '$provide', function ($httpProvider, $locationProvider, $provide) {
    $httpProvider.interceptors.push('httpInterceptor');
    $httpProvider.interceptors.push('httpRequestInterceptor');
    $provide.decorator('$sniffer', ['$delegate', function ($sniffer) {
        var msie = parseInt((/msie (\d+)/.exec(angular.lowercase(navigator.userAgent)) || [])[1], 10);
        var _hasEvent = $sniffer.hasEvent;
        $sniffer.hasEvent = function (event) {
            if (event === 'input' && msie > 10) {
                return false;
            }
            _hasEvent.call(this, event);
            //20170109 if doesnt work in Ipad, uncomment the below code
            //return _hasEvent.call(this, event);
        };
        return $sniffer;
    }])
}]);

////Local Service for language translation
app.service('LocaleService', ['$q', 'gettextCatalog', 'common', function ($q, gettextCatalog, common) {
    var self = {
        useLocale: function (language, area) {
            var path = '/Resources/' + area + '_' + language + '.json';// + "?v=" + Version;
            var ssid = area + '_' + language;

            var defer = $q.defer();

            //var tempPath = '/Resources/testpage/testpage.json'
            //$resource(tempPath).get(function (translation) {
            //    console.log(translation);
            //});

            //var tempPath = '/Resources/TestCall/TestCall_en-US.json'
            //common.$http.get(tempPath).then(function (translation) {
            //    console.log(translation);
            //});

            common.$resource(path).get(function (translation) {
                gettextCatalog.currentLanguage = language;
                gettextCatalog.setStrings(language, translation);
                defer.resolve(translation);
            }, function () {
                language = 'en-US';
                var path = '/Resources/' + area + '_' + language + '.json';//+ "?v=" + Version;

                common.$resource(path).get(function (translation) {
                    gettextCatalog.currentLanguage = language;
                    gettextCatalog.setStrings(language, translation);
                    defer.resolve(translation);
                })
            });

            return defer.promise;
        },
    };

    return self;
}]);

app.run(function ($rootScope, gettextCatalog, LocaleService) {
    $rootScope.pageId = { Add: 'A', View: 'V', Other: 'O' };
    $rootScope.version = Version;
    $rootScope.baseUrl = ApiURL;
    $rootScope.baseAdminUrl = $rootScope.baseUrl + 'admin/';
    gettextCatalog.currentLanguage = Culture;
    LocaleService.useLocale(Culture, 'Common');
});
app.run(
  ['$rootScope', '$state', '$stateParams',
    function ($rootScope, $state, $stateParams) {
        // It's very handy to add references to $state and $stateParams to the $rootScope
        // so that you can access them from any scope within your applications.For example,
        // <li ui-sref-active="active }"> will set the <li> // to active whenever
        // 'contacts.list' or one of its decendents is active.
        $rootScope.$state = $state;
        $rootScope.$stateParams = $stateParams;
    }
  ]
)

app.directive('bindHtmlUnsafeapp', function ($compile) {
    return function ($scope, $element, $attrs) {

        var compile = function (newHTML) { // Create re-useable compile function
            newHTML = $compile(newHTML)($scope); // Compile html
            $element.html('').append(newHTML); // Clear and append it
        };

        var htmlName = $attrs.bindHtmlUnsafeapp; // Get the name of the variable 
        // Where the HTML is stored

        $scope.$watch(htmlName, function (newHTML) { // Watch for changes to 
            // the HTML
            if (!newHTML) return;
            compile(newHTML);   // Compile it
        });

    };
});

app.directive('ngFocus', ['$parse', function ($parse) {
    return function (scope, element, attr) {
        var fn = $parse(attr['ngFocus']);
        element.on('focus', function (event) {
            scope.$apply(function () {
                fn(scope, { $event: event });
            });
        });
    };
}])

app.filter('unique', function () {
    return function (input, key) {
        var unique = {};
        var uniqueList = [];
        for (var i = 0; i < input.length; i++) {
            if (typeof unique[input[i][key]] == "undefined") {
                unique[input[i][key]] = "";
                uniqueList.push(input[i]);
            }
        }
        return uniqueList;
    };
});

app.service('httpRequestInterceptor', ['UserAuthService', '$rootScope', function (UserAuthService, $rootScope) {
    var service = this;
    service.request = function (config) {
        //debugger;
        var access_token = UserAuthService.getAccessToken();

        if (access_token) {
            config.headers.Authorization = 'Bearer ' + access_token;
        }

        return config;
    };
}]);

//app.filter('toArray', function () {
//    return function (obj, addKey) {
//        if (!angular.isObject(obj)) return obj;
//        if (addKey === false) {
//            return Object.keys(obj).map(function (key) {
//                return obj[key];
//            });
//        } else {
//            return Object.keys(obj).map(function (key) {
//                var value = obj[key];
//                return angular.isObject(value) ?
//                  Object.defineProperty(value, '$key', { enumerable: false, value: key }) :
//                { $key: key, $value: value };
//            });
//        }
//    };
//});
