app.controller('TestCallCtrl', ['$scope', 'common', '$location', 'TestCallSvc', function ($scope, common, $location, TestCallSvc) {
    $scope.TestObject = { id: 1, Name: "Raj" };
    $scope.Init = function () {
        $scope.InitValue = "this list page is called.";
        TestCallSvc.Search($scope.TestObject).then(
          function (response) {
              console.log('success');
          },
            function (error) {
                console.log(error);
                console.log('fail');
            }
            );
    }

    $scope.RedirectPage = function (pageName) {
        var url = pageName;
        window.location.href = url;
    };
    $scope.callback = function (id) {
        common.$location.path("/TestPage/" + id);
    };

    $scope.Init();
}]);