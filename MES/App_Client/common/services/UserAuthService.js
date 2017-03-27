//http://onehungrymind.com/winning-http-interceptors-angularjs/

//var app = angular.module('app');

app.service('UserAuthService', ['$cookies', function ($cookies) {
    var service = this;
    
    service.getAccessToken = function () {
        var token = $cookies.get('token');
        return token;
    }
    service.getClientId = function () {
        alert($cookies.cid);
        alert($cookies.get('cid'));
        return $cookies.cid;
    }

}]);