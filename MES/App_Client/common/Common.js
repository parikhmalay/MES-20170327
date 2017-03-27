(function () {
    'use strict';
    var commonModule = angular.module('common', ['ngResource', 'aa.formExtensions', 'aa.notify']);

    commonModule.factory('common',
        ['$rootScope', '$resource', '$location', '$http', '$route', '$routeParams', 'usSpinnerService', 'aaNotify', '$state', '$stateParams', common]);

    function common($rootScope, $resource, $location, $http, $route, $routeParams, usSpinnerService, aaNotify, $state, $stateParams) {

        var service = {
            // common angular dependencies
            $rootScope: $rootScope,
            $resource: $resource,
            $location: $location,
            $http: $http,
            $route: $route,
            $routeParams: $routeParams,
            usSpinnerService: usSpinnerService,
            aaNotify: aaNotify,
            $state: $state,
            $stateParams: $stateParams
        };
        return service;
    }
})();

