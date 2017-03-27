/*
 * angular-confirm
 * https://github.com/Schlogen/angular-confirm
 * @version v1.1.2 - 2015-09-26
 * @license Apache
 */
angular.module('angular-confirm', ['ui.bootstrap.modal'])
  .controller('ConfirmModalController', function ($scope, $modalInstance, data) {

      $scope.data = angular.copy(data);
      $scope.ok = function (obj) {
          $modalInstance.close(obj);
      };

      $scope.cancel = function () {
          $modalInstance.dismiss('cancel');
      };

  })
  .value('$confirmModalDefaults', {
      template: '<div class="modal-header"><h3 class="modal-title float-l">{{data.title}}</h3><button class="btn float-r" ng-click="ok(1)"><i class="fa fa-close"></i></button></div>' +
                '<div class="modal-body body-text-c"><div>{{data.text1}}</div>' +
                '<div>{{data.text2}}</div></div>' +
                '<div class="modal-footer">' +
                '<button class="orange-btn" ng-click="ok(0)">{{data.ok}}</button>' +
                '<button class="orange-btn" ng-click="cancel()" focus-me>{{data.cancel}}</button>' +
                '</div>',
      controller: 'ConfirmModalController',
      backdrop: 'static',
      keyboard: false,
      defaultLabels: {
          title: 'Confirm',
          ok: 'OK',
          cancel: 'Cancel',
          close: 'Close',
      }
  })
  .factory('$confirm', function ($modal, $confirmModalDefaults) {
      return function (data, settings) {
          var defaults = angular.copy($confirmModalDefaults);
          settings = angular.extend(defaults, (settings || {}));

          data = angular.extend({}, settings.defaultLabels, data || {});

          if ('templateUrl' in settings && 'template' in settings) {
              delete settings.template;
          }

          settings.resolve = {
              data: function () {
                  return data;
              }
          };

          return $modal.open(settings).result;
      };
  })
  .directive('confirm', function ($confirm) {
      return {
          priority: 1,
          restrict: 'A',
          scope: {
              confirmIf: "=",
              ngClick: '&',
              confirm: '@',
              confirmSettings: "=",
              confirmTitle: '@',
              confirmOk: '@',
              confirmCancel: '@',
              confirmClose: '@'
          },
          link: function (scope, element, attrs) {

              element.unbind("click").bind("click", function ($event) {

                  $event.preventDefault();
                  if (angular.isUndefined(scope.confirmIf) || scope.confirmIf) {

                      var data = { text: scope.confirm };
                      if (scope.confirmTitle) {
                          data.title = scope.confirmTitle;
                      }
                      if (scope.confirmOk) {
                          data.ok = scope.confirmOk;
                      }
                      if (scope.confirmCancel) {
                          data.cancel = scope.confirmCancel;
                      }
                      if (scope.confirmClose) {
                          data.close = scope.confirmClose;
                      }
                      $confirm(data, scope.confirmSettings || {}).then(scope.ngClick);
                  } else {

                      scope.$apply(scope.ngClick);
                  }
              });

          }
      }
  });
