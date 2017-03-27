if (typeof getRoutes == 'function') {
    var app = angular.module('app');

    // Collect the routes
    app.constant('routes', getRoutes());
    // Configure the routes and route resolvers
    app.config(['$routeProvider', 'routes', '$locationProvider', routeConfigurator]);
}
else if (typeof getUIRoutes == 'function') {
    var app = angular.module('app');
    app.constant('routes', getUIRoutes())
    app.config(['$stateProvider', '$urlRouterProvider', 'routes', '$locationProvider', routeUIConfigurator]);

    app.config(function ($urlRouterProvider) {
        // Here's an example of how you might allow case insensitive urls
        $urlRouterProvider.rule(function ($injector, $location, $stateParams) {
            //what this function returns will be set as the $location.url
            var path = $location.path(), normalized = path.toLowerCase();
            if (path != normalized) {
                //$location.path(normalized);
                return normalized;
            }
        });
    });
}
function routeConfigurator($routeProvider, routes, $locationProvider) {
    routes.forEach(function (r) {
        //For Case Insensitive URL
        r.config.caseInsensitiveMatch = true;

        if (!r.config.resolve)
            r.config.resolve = {};
        ///For Multiligual Translation
        if (r.config.areaName) {
            var resolveFunction = function ($route, LocaleService) {
                return LocaleService.useLocale(Culture, r.config.areaName);
            }
            //Injecting dependency to make it work during minification
            resolveFunction.$inject = ['$route', 'LocaleService'];
            r.config.resolve["translation"] = resolveFunction;
        }
        r.config.templateUrl = Version === "" ? r.config.templateUrl : r.config.templateUrl + "?v=" + Version;

        $routeProvider.when(r.url, r.config);
    });
    $routeProvider.otherwise({ redirectTo: '/' });
}

function routeUIConfigurator($stateProvider, $urlRouterProvider, routes, $httpProvider, $locationProvider) {
    $urlRouterProvider.otherwise(routes.defaultstate);
    if (routes.states.length > 0) {
        routes.states.forEach(function (state) {
            //For Case Insensitive URL
            state.config.caseInsensitiveMatch = true;
            if (!state.config.resolve)
                state.config.resolve = {};

            //For Multiligual Translation
            if (state.config.areaName) {
                state.config.resolve["translation"] = function ($route, LocaleService) {
                    return LocaleService.useLocale(Culture, state.config.areaName);
                }
            }

            if (state.config.templateUrl)
                state.config.templateUrl = Version === "" ? state.config.templateUrl : state.config.templateUrl + "?v=" + Version;

            if (state.config.views) {
                for (var v in state.config.views) {
                    if (state.config.views[v].templateUrl)
                        state.config.views[v].templateUrl = Version === "" ? state.config.views[v].templateUrl : state.config.views[v].templateUrl + "?v=" + Version;
                }
            }

            $stateProvider.state(state.state, state.config);
        });
    }
    //$routeProvider.otherwise({ redirectTo: '/' });
}