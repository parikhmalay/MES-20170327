app.controller('CustomersCtrl', ['$rootScope', '$scope', 'common', 'CustomersSvc', '$modal', '$filter', 'LookupSvc', function ($rootScope, $scope, common, CustomersSvc, $modal, $filter, LookupSvc) {
    //Start implement security role wise
    $scope.setRoleWisePrivilege = function () {
        $scope.currentSecurityObject = currentSecurityObject;
        if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
            angular.forEach($scope.currentSecurityObject, function (obj, index) {
                switch (obj.ObjectId) {
                    case 12:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                RedirectToAccessDenied();
                                break;
                            case 2:                          //read only
                                $scope.IsReadOnlyDeleteButton = true;
                                $scope.IsReadOnlyAddButton = true;
                                break;
                            case 3:                         //write
                                break;
                        }
                        break;
                    case 13:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                $scope.IsReadOnlyAddButton = true;
                                break;
                            case 2:                          //read only
                                $scope.IsReadOnlyAddButton = true;
                                break;
                            case 3:                         //write
                                break;
                        }
                        break;
                    default:
                        break;
                }
            });
        }
        else {
            RedirectToAccessDenied();
        }
    }
    //End implement security role wise
    $scope.setRoleWisePrivilege();

    $scope.Customers = {};
    $scope.CustomersList = {};
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $scope.SelectAll = false;
    $rootScope.PageHeader = ($filter('translate')('_Customers_'));

    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;

        ///Start set search criteria when return from add edit page
        if (localStorage.getItem("CustomerListPaging") && localStorage.getItem("CustomerListPageSearchCriteria")) {
            $scope.Paging = JSON.parse(localStorage.getItem("CustomerListPaging"));
            localStorage.removeItem("CustomerListPaging");
            $scope.SearchCriteria = JSON.parse(localStorage.getItem("CustomerListPageSearchCriteria"));
            localStorage.removeItem("CustomerListPageSearchCriteria");
        }
        ///End set search criteria when return from add edit page

        $scope.GetCustomersList();
    };
    $scope.GetCustomersList = function () {
        common.usSpinnerService.spin('spnCustomers');
        CustomersSvc.GetCustomersList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnCustomers');
                 if (response.data.StatusCode == 200) {
                     $scope.CustomersList = response.data.Result;
                     $scope.Paging = response.data.PageInfo;
                     $scope.Paging.Criteria = $scope.SearchCriteria;
                     if ($scope.CustomersList.length > 0)
                         advanceSearch.close();
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnCustomers');
                 //common.aaNotify.error(error);
             });
    };

    $scope.disableClick = function ($event, objVal) {
        if (!IsUndefinedNullOrEmpty(objVal))
            $event.stopPropagation();

    };

    $scope.Edit = function (id) {
        localStorage.setItem("CustomerListPaging", JSON.stringify($scope.Paging));
        localStorage.setItem("CustomerListPageSearchCriteria", JSON.stringify($scope.SearchCriteria));
        common.$location.path("/AddEdit/" + id);
    };

    $scope.Delete = function (customersId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            CustomersSvc.Delete(customersId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetCustomersList();
               }
           },
           function (error) {
               //common.aaNotify.error(error);
           });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetCustomersList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetCustomersList();
    };

    $scope.Search = function () {
        $scope.Init();
    };

    $scope.ResetSearch = function () {
        $scope.SearchCriteria = {};
        $scope.Init();
    }

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'Customers';
        $scope.SchemaName = 'Customer';
        $scope.SpinnerKeyName = $("[id^='spn']").first().attr('id');
        var modalTemplatePreviewOptions = $modal.open({
            templateUrl: '/App_Client/common/directives/AuditLog/AuditLogsPopup.html?v=' + Version,
            controller: ViewChangeLogPageInstance,
            keyboard: true,
            backdrop: true,
            scope: $scope,
            sizeclass: 'modal-fitScreen'
        });
        modalTemplatePreviewOptions.result.then(function () {
        }, function () {
        });
    };
    // audit logs popup end here

    $scope.RedirectToAddEditPage = function () {
        common.$location.path("/AddEdit/");
    }

    //Start logic for checkbox select deselect here and send email
    $scope.SelectDeselectAll = function () {
        angular.forEach($scope.CustomersList, function (item) {
            item.chkSelect = $scope.SelectAll;
        });
    };
    $scope.select = function () {
        $scope.SelectAll = true;
        angular.forEach($scope.CustomersList, function (item) {
            if (!item.chkSelect)
                $scope.SelectAll = false;
        });
    };
    //end logic for checkbox select deselect here and send email

    //Start code for Multiple customer delete.
    $scope.DeleteMultipleCustomers = function () {
        $scope.CustomerIDs = [];
        angular.forEach($scope.CustomersList, function (item) {
            if (item.chkSelect && !IsUndefinedNullOrEmpty(item.Id))
                $scope.CustomerIDs.push(item.Id);
        });
        if ($scope.CustomerIDs.length > 0)
            $scope.DeleteCustomers();
        else
            common.aaNotify.error($filter('translate')('_AtLeastOneCustomer_'));
    };
    $scope.DeleteCustomers = function () {
        var customerIDs = $scope.CustomerIDs.join(",");
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            CustomersSvc.DeleteMultiple(customerIDs).then(
              function (response) {
                  if (ShowMessage(common, response.data)) {
                      $scope.Init();
                  }
              },
              function (error) {
                  //common.aaNotify.error(error);
              });
        }
    }
    //End code for Multiple customer delete.

    $scope.Init();
}]);

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
}

app.controller('AddEditCustomersCtrl', ['$rootScope', '$scope', 'common', 'CustomersSvc', '$modal', 'LookupSvc', '$filter', '$routeParams', '$timeout', 'ContactsSvc', 'DivisionsSvc', 'AddressSvc',
    function ($rootScope, $scope, common, CustomersSvc, $modal, LookupSvc, $filter, $routeParams, $timeout, ContactsSvc, DivisionsSvc, AddressSvc) {
        //Start implement security role wise
        $scope.setRoleWisePrivilege = function () {
            $scope.currentSecurityObject = currentSecurityObject;
            if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
                angular.forEach($scope.currentSecurityObject, function (obj, index) {
                    switch (obj.ObjectId) {
                        case 12:
                            switch (obj.PrivilegeId) {
                                case 1:                           //none
                                    RedirectToAccessDenied();
                                    break;
                                case 2:                           //read
                                    $scope.IsReadOnlyPage = true;
                                    break;
                            }
                            break;
                        case 13:
                            if ($scope.TransactionMode == "Create") {
                                $scope.PagePrivilegeCase(obj);
                            }
                            break;
                        case 14:
                            if ($scope.TransactionMode == "Edit") {
                                $scope.PagePrivilegeCase(obj);
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
            else {
                RedirectToAccessDenied();
            }
        }
        $scope.PagePrivilegeCase = function (obj) {
            switch (obj.PrivilegeId) {
                case 1:                           //none
                    RedirectToAccessDenied();
                    break;
                case 2:                          //read only
                    $scope.IsReadOnlyPage = true;
                    break;
                case 3:                         //write                    
                    break;
            }
        }
        //End implement security role wise

        $scope.SelectAllObj = { SelectAllContact: false, SelectAllDivision: false, SelectAllAddress: false };
        $('body').removeClass('paginationFixedToBottom haveAdvanceSearch');
        $rootScope.PageHeader = ($filter('translate')('_PageHeading_'));

        $scope.Init = function () {
            $scope.Customer = {
                lstContact: [],
                lstDivision: [],
                lstAddress: []
            };
            $scope.SetLooksupData();
            $scope.Contact = { PrefixId: 0 };
            $scope.Division = {};
            $scope.Address = {};
            $scope.PaymentRating = 0;
            $scope.DivisionPaymentRating = 0;
            $timeout(function () {
                if (!IsUndefinedNullOrEmpty($routeParams.Id) && $routeParams.Id != '0') {
                    $scope.AddEditContact = false;
                    $scope.AddEditDivision = false;
                    $scope.AddEditAddress = false;
                    $scope.TransactionMode = 'Edit';
                    $scope.Customer.Id = $routeParams.Id;
                    $scope.getData($routeParams.Id);
                }
                else {
                    $scope.AddEditContact = true;
                    $scope.AddEditDivision = true;
                    $scope.AddEditAddress = true;
                    $scope.TransactionMode = 'Create';
                    $scope.IsMandatory = false;
                }
                $scope.setRoleWisePrivilege();
            }, 1000);
        }

        $scope.SetRequired = function () {
            if (!IsUndefinedNullOrEmpty($scope.Division.CompanyName)
                || !IsUndefinedNullOrEmpty($scope.Division.Address1)
                || !IsUndefinedNullOrEmpty($scope.Division.City)
                || !IsUndefinedNullOrEmpty($scope.Division.State)
                || !IsUndefinedNullOrEmpty($scope.Division.Zip)
                || !IsUndefinedNullOrEmpty($scope.Division.CountryId)
                || !IsUndefinedNullOrEmpty($scope.Division.CompanyPhone1)
                ) {
                $scope.IsMandatory = true;
                return false;
            }
            else {
                $scope.IsMandatory = false;
                return true;
            }

        }
        //$scope.$watch($scope.Division, function (newValue, oldValue) {
        //    if ($scope.TransactionMode == 'Create')
        //        $scope.SetRequired();
        //});

        $scope.SetLooksupData = function () {
            $scope.LookUps = [
                {
                    "Name": "Countries", "Parameters": {}
                },
                {
                    "Name": "SAM", "Parameters": { "source": "SAM" }
                },
                {
                    "Name": "Prefixes", "Parameters": {}
                },
                {
                    "Name": "AddressType", "Parameters": {}
                }
            ];
            $scope.getLookupData();
        };

        $scope.getData = function (customerId) {
            common.usSpinnerService.spin('spnAddEditCustomers');
            CustomersSvc.getData(customerId).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       common.usSpinnerService.stop('spnAddEditCustomers');
                       $scope.Customer = response.data.Result;
                       $scope.PaymentRating = $scope.Customer.PaymentRating;
                   }
                   else {
                       common.usSpinnerService.stop('spnAddEditCustomers');
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnAddEditCustomers');
                   console.log(error);
               });
        };

        $scope.getLookupData = function () {
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                angular.forEach(data.data, function (o) {
                    if (o.Name === "SAM") {
                        $scope.SAMList = o.Data;
                    }
                    else if (o.Name === "Countries") {
                        $scope.CountryList = o.Data;
                    }
                    else if (o.Name === "Prefixes") {
                        $scope.PrefixList = o.Data;
                    }
                    else if (o.Name === "AddressType") {
                        $scope.AddressTypeList = o.Data;
                    }
                });
            });
        };

        $scope.SaveCustomers = function (closeForm) {
            if (IsUndefinedNullOrEmpty($scope.Customer.Id) || $scope.Customer.Id == 0) {
                $scope.setListObjects();
            }
            if (!$scope.checkForAddressType()) {
                common.aaNotify.error($filter('translate')('_AtleastPlantAddress_'));
                return false;
            }
            if ($scope.Customer.lstContact.length <= 0) {
                common.aaNotify.error($filter('translate')('_AddAtleastOneContact_'));
                return false;
            }
            common.usSpinnerService.spin('spnAddEditCustomers');
            CustomersSvc.Save($scope.Customer).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       common.usSpinnerService.stop('spnAddEditCustomers');
                       $scope.Id = response.data.Result // Id of latest created record
                       $scope.Customer.Id = response.data.Result;
                       if (closeForm) {
                           $scope.RedirectToList();
                       }
                       else {
                           if ($scope.TransactionMode == 'Create')
                               $scope.reloadWithId();
                           else
                               $scope.ResetForm();
                       }
                   }
                   else {
                       common.usSpinnerService.stop('spnAddEditCustomers');
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnAddEditCustomers');
                   console.log(error);
               });
        };

        $scope.reloadWithId = function () {
            common.$location.path('/AddEdit/' + $scope.Customer.Id);
        };

        $scope.setListObjects = function () {
            $scope.Customer.lstContact = [];
            //$scope.Contact.PrefixId = $scope.Contact.PrefixItem.Id;
            //$scope.Contact.Prefix = $scope.Contact.PrefixItem.Name;
            $scope.Contact.IsDefault = true;
            $scope.Customer.lstContact.push($scope.Contact);
            if ($scope.IsMandatory) {
                $scope.Customer.lstDivision = [];
                if ($scope.Division.SAM != undefined && $scope.Division.SAM != null)
                    $scope.Division.SAMId = $scope.Division.SAM.Id;
                $scope.Customer.lstDivision.push($scope.Division);
            }
            $scope.Customer.lstAddress = [];
            $scope.Address.AddressTypeId = $scope.Address.AddressTypeItem.Id;
            $scope.Address.IsDefault = true;
            $scope.Customer.lstAddress.push($scope.Address);
        }

        ////***Start code for contact***/////
        $scope.GetContactsList = function () {
            $scope.Customer.lstContact = [];
            common.usSpinnerService.spin('spnAddEditCustomers');
            ContactsSvc.GetContactsList($scope.Customer.Id).then(
                 function (response) {
                     common.usSpinnerService.stop('spnAddEditCustomers');
                     $scope.Customer.lstContact = response.data;
                     $scope.ResetContacts();
                 },
                 function (error) {
                     common.usSpinnerService.stop('spnAddEditCustomers');
                     //common.aaNotify.error(error);
                     console.log(error)
                 });
        };
        $scope.EditContact = function (contact, index) {
            $scope.AddNewContact();
            $scope.Contact = contact;
            $scope.TempContact = contact;
            $scope.IsDefault = $scope.Contact.IsDefault;
        };
        $scope.DeleteContact = function (contactId, index) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                ContactsSvc.Delete(contactId).then(
                  function (response) {
                      if (ShowMessage(common, response.data)) {
                          $scope.GetContactsList();
                      }
                  },
                  function (error) {
                      //common.aaNotify.error(error);
                  });
            }
        };
        $scope.DeleteMultipleContact = function () {
            $scope.ContactIds = [];
            angular.forEach($scope.Customer.lstContact, function (item) {
                if (item.chkSelect && !Isundefinedornull(item.Id) && item.Id > 0)
                    $scope.ContactIds.push(item.Id);
            });
            if ($scope.ContactIds.length > 0) {
                var contactIds = $scope.ContactIds.join(",");
                if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                    ContactsSvc.DeleteMultiple(contactIds).then(
                      function (response) {
                          if (ShowMessage(common, response.data)) {
                              $scope.GetContactsList();
                          }
                      },
                      function (error) {
                          //common.aaNotify.error(error);
                      });
                }
            }
            else
                common.aaNotify.error($filter('translate')('_AtLeastOneContact_'));
        };
        $scope.ResetContacts = function () {
            $scope.Contact = {
                FirstName: '',
                LastName: '',
                PrefixItem: null,
                Suffix: '',
                Designation: '',
                DirectPhone: null,
                Extension: null,
                DirectFax: null,
                Email: '',
                Comments: '',
                IsDefault: false
            };
            $scope.AddEditContact = false;
            $scope.IsDefault = false;
            $scope.TempContact = {};
        };
        $scope.AddNewContact = function () {
            $scope.AddEditContact = true;
            $scope.IsDefault = false;
            $scope.Contact.PrefixId = 0;
            $scope.ShowContactPopup();
        };
        $scope.ShowContactPopup = function () {
            var modalTemplatePreviewOptions = $modal.open({
                templateUrl: '/App_Client/views/RFQ/Customer/CustomerContact.html?v=' + Version,
                controller: ModalContactCtrl,
                keyboard: true,
                backdrop: true,
                scope: $scope,
                sizeclass: 'modal-md'
            });
            modalTemplatePreviewOptions.result.then(function () {
            }, function () {
            });
        };
        ////***End code for contact***/////

        ////***Start code for Division***/////
        $scope.GetDivisionsList = function () {
            $scope.Customer.lstDivision = [];
            common.usSpinnerService.spin('spnAddEditCustomers');
            DivisionsSvc.GetDivisionsList($scope.Customer.Id).then(
                 function (response) {
                     common.usSpinnerService.stop('spnAddEditCustomers');
                     $scope.Customer.lstDivision = response.data;
                     $scope.ResetDivisions();
                 },
                 function (error) {
                     common.usSpinnerService.stop('spnAddEditCustomers');
                     //common.aaNotify.error(error);
                 });
        };
        $scope.EditDivision = function (division, index) {
            $scope.AddNewDivision();
            $scope.Division = division;
            $scope.TempDivision = division;
            $scope.DivisionPaymentRating = division.PaymentRating;
        };
        $scope.DeleteDivision = function (divisionId, index) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                DivisionsSvc.Delete(divisionId).then(
                  function (response) {
                      if (ShowMessage(common, response.data)) {
                          $scope.GetDivisionsList();
                      }
                  },
                  function (error) {
                      //common.aaNotify.error(error);
                  });
            }
        };
        $scope.DeleteMultipleDivision = function () {
            $scope.DivisionIds = [];
            angular.forEach($scope.Customer.lstDivision, function (item) {
                if (item.chkSelect && !Isundefinedornull(item.Id) && item.Id > 0)
                    $scope.DivisionIds.push(item.Id);
            });
            if ($scope.DivisionIds.length > 0) {
                var divisionIds = $scope.DivisionIds.join(",");
                if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                    DivisionsSvc.DeleteMultiple(divisionIds).then(
                      function (response) {
                          if (ShowMessage(common, response.data)) {
                              $scope.GetDivisionsList();
                          }
                      },
                      function (error) {
                          //common.aaNotify.error(error);
                      });
                }
            }
            else
                common.aaNotify.error($filter('translate')('_AtLeastOneDivision_'));
        };
        $scope.ResetDivisions = function () {
            $scope.Division = {
                CustomerCode: '',
                CompanyName: '',
                Address1: '',
                Address2: '',
                City: '',
                State: '',
                CountryId: 0,
                Zip: '',
                Website: '',
                CompanyPhone1: '',
                CompanyPhone2: '',
                CompanyFax: '',
                Comments: '',
                PaymentRating: 0,
                SAM: null
            };
            $scope.DivisionPaymentRating = 0;
            $scope.AddEditDivision = false;
            $scope.TempDivision = {};
        };
        $scope.AddNewDivision = function () {
            $scope.AddEditDivision = true;
            $scope.ShowDivisionPopup();
        };
        $scope.ShowDivisionPopup = function () {
            var modalTemplatePreviewOptions = $modal.open({
                templateUrl: '/App_Client/views/RFQ/Customer/CustomerDivision.html?v=' + Version,
                controller: ModalDivisionCtrl,
                keyboard: true,
                backdrop: true,
                scope: $scope,
                sizeclass: 'modal-md'
            });
            modalTemplatePreviewOptions.result.then(function () {
            }, function () {
            });
        };
        ////***End code for Division***/////

        ////***Start code for Address***/////
        $scope.GetAddressList = function () {
            $scope.Customer.lstAddress = [];
            common.usSpinnerService.spin('spnAddEditCustomers');
            AddressSvc.GetAddressList($scope.Customer.Id).then(
                 function (response) {
                     common.usSpinnerService.stop('spnAddEditCustomers');
                     $scope.Customer.lstAddress = response.data;
                     $scope.ResetAddress();
                 },
                 function (error) {
                     common.usSpinnerService.stop('spnAddEditCustomers');
                     //common.aaNotify.error(error);
                     console.log(error)
                 });
        };
        $scope.EditAddress = function (address, index) {
            $scope.AddNewAddress();
            $scope.Address = address;
            $scope.TempAddress = address;
            $scope.AddressIsDefault = $scope.Address.IsDefault;
        };
        $scope.DeleteAddress = function (addressId, index) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                AddressSvc.Delete(addressId).then(
                  function (response) {
                      if (ShowMessage(common, response.data)) {
                          $scope.GetAddressList();
                      }
                  },
                  function (error) {
                      //common.aaNotify.error(error);
                  });
            }
        };
        $scope.DeleteMultipleAddress = function () {
            $scope.AddressIds = [];
            angular.forEach($scope.Customer.lstAddress, function (item) {
                if (item.chkSelect && !Isundefinedornull(item.Id) && item.Id > 0)
                    $scope.AddressIds.push(item.Id);
            });
            if ($scope.AddressIds.length > 0) {
                var addressIds = $scope.AddressIds.join(",");
                if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                    AddressSvc.DeleteMultipleAddress(addressIds).then(
                      function (response) {
                          if (ShowMessage(common, response.data)) {
                              $scope.GetAddressList();
                          }
                      },
                      function (error) {
                          //common.aaNotify.error(error);
                      });
                }
            }
            else
                common.aaNotify.error($filter('translate')('_AtLeastOneAddress_'));
        };
        $scope.ResetAddress = function () {
            $scope.Address = {
                AddressTypeId: 0,
                AddressTypeItem: null,
                Address1: '',
                Address2: '',
                City: '',
                State: '',
                CountryId: 0,
                Zip: '',
                IsDefault: false
            };
            $scope.AddEditAddress = false;
            $scope.AddressIsDefault = false;
            $scope.TempAddress = {};
        };
        $scope.AddNewAddress = function () {
            $scope.AddEditAddress = true;
            $scope.AddressIsDefault = false;
            $scope.ShowAddressPopup();
        };
        $scope.ShowAddressPopup = function () {
            var modalTemplatePreviewOptions = $modal.open({
                templateUrl: '/App_Client/views/RFQ/Customer/CustomerAddress.html?v=' + Version,
                controller: ModalAddressCtrl,
                keyboard: true,
                backdrop: true,
                scope: $scope,
                sizeclass: 'modal-md'
            });
            modalTemplatePreviewOptions.result.then(function () {
            }, function () {
            });
        };
        $scope.checkForAddressType = function () {
            var IsPlantAddressSelected = false;
            angular.forEach($scope.Customer.lstAddress, function (objAddress, index) {
                if (!IsPlantAddressSelected && !IsUndefinedNullOrEmpty(objAddress.AddressTypeItem) && (objAddress.AddressTypeItem.Id == "2" || objAddress.AddressTypeItem.Id == 2)) {
                    IsPlantAddressSelected = true;
                }
            });
            return IsPlantAddressSelected;
        };
        ////***End code for Address***/////

        //Start logic for checkbox select deselect here
        $scope.SelectDeselectAllContact = function () {
            angular.forEach($scope.Customer.lstContact, function (item) {
                if (!item.IsDefault)
                    item.chkSelect = $scope.SelectAllObj.SelectAllContact;
            });
        };
        $scope.selectContact = function () {
            $scope.SelectAllObj.SelectAllContact = true;
            angular.forEach($scope.Customer.lstContact, function (item) {
                if (!item.IsDefault && !item.chkSelect)
                    $scope.SelectAllObj.SelectAllContact = false;
            });
        };
        $scope.SelectDeselectAllDivision = function () {
            angular.forEach($scope.Customer.lstDivision, function (item) {
                item.chkSelect = $scope.SelectAllObj.SelectAllDivision;
            });
        };
        $scope.selectDivision = function () {
            $scope.SelectAllObj.SelectAllDivision = true;
            angular.forEach($scope.Customer.lstDivision, function (item) {
                if (!item.chkSelect)
                    $scope.SelectAllObj.SelectAllDivision = false;
            });
        };
        $scope.SelectDeselectAllAddress = function () {
            angular.forEach($scope.Customer.lstAddress, function (item) {
                if (!item.IsDefault)
                    item.chkSelect = $scope.SelectAllObj.SelectAllAddress;
            });
        };
        $scope.selectAddress = function () {
            $scope.SelectAllObj.SelectAllAddress = true;
            angular.forEach($scope.Customer.lstAddress, function (item) {
                if (!item.IsDefault && !item.chkSelect)
                    $scope.SelectAllObj.SelectAllAddress = false;
            });
        };
        //end logic for checkbox select deselect here

        $scope.RedirectToList = function () {
            common.$location.path("/");
        };
        $scope.ResetForm = function () {
            common.$route.reload();
        };
        $scope.PaymentRatingRate = function (rating) {
            $scope.Customer.PaymentRating = rating;
        };
        $scope.DivisionPaymentRatingRate = function (rating) {
            $scope.Division.PaymentRating = rating;
        };

        $scope.Init();
    }]);

var ModalContactCtrl = function ($scope, $modalInstance, common, ContactsSvc) {

    $scope.AddContact = function () {
        if (!Isundefinedornull($scope.Customer.Id)) {
            $scope.Contact.CustomerId = $scope.Customer.Id;
            //if (!Isundefinedornull($scope.Contact.PrefixItem))
            //    $scope.Contact.PrefixId = $scope.Contact.PrefixItem.Id;
            //else
            //    $scope.Contact.PrefixId = null;
        }
        else {
            common.aaNotify.error('Customer is undefined.');
            return false;
        }
        if (!$scope.Contact.IsDefault) {
            $scope.Contact.IsDefault = false;
        }
        $scope.SaveContact($scope.Contact);
    };
    $scope.SaveContact = function (contact) {
        common.usSpinnerService.spin('spnAddEditCustomers');
        ContactsSvc.Save(contact).then(
          function (response) {
              if (ShowMessage(common, response.data)) {
                  common.usSpinnerService.stop('spnAddEditCustomers');
                  $scope.Cancel();
              }
              else {
                  common.usSpinnerService.stop('spnAddEditCustomers');
              }
          },
          function (error) {
              common.usSpinnerService.stop('spnAddEditCustomers');
              console.log(error);
          });
    };
    $scope.Cancel = function () {
        $scope.GetContactsList();
        $modalInstance.dismiss('cancel');
        $scope.destroyScope();
    };
    $scope.destroyScope = function () {
        var lclScope = $scope;
        lclScope.$destroy();
    };

    $scope.ResetContactPopup = function () {
        $scope.Contact = angular.copy($scope.TempContact);
    };
}


var ModalDivisionCtrl = function ($scope, $modalInstance, common, DivisionsSvc) {
    $scope.AddDivision = function () {
        if (!Isundefinedornull($scope.Customer.Id)) {
            $scope.Division.CustomerId = $scope.Customer.Id;
            if ($scope.Division.SAM != undefined && $scope.Division.SAM != null)
                $scope.Division.SAMId = $scope.Division.SAM.Id;
        }
        else {
            common.aaNotify.error('Customer is undefined.');
            return false;
        }
        $scope.SaveDivision($scope.Division);
    };
    $scope.SaveDivision = function (division) {
        common.usSpinnerService.spin('spnAddEditCustomers');
        DivisionsSvc.Save(division).then(
          function (response) {
              if (ShowMessage(common, response.data)) {
                  common.usSpinnerService.stop('spnAddEditCustomers');
                  $scope.Cancel();
              }
              else {
                  common.usSpinnerService.stop('spnAddEditCustomers');
              }
          },
          function (error) {
              common.usSpinnerService.stop('spnAddEditCustomers');
              console.log(error);
          });
    };
    $scope.Cancel = function () {
        $scope.GetDivisionsList();
        $modalInstance.dismiss('cancel');
        $scope.destroyScope();
    };
    $scope.destroyScope = function () {
        var lclScope = $scope;
        lclScope.$destroy();
    };

    $scope.ResetDivisionPopup = function () {
        $scope.Division = angular.copy($scope.TempDivision);
    };
}

var ModalAddressCtrl = function ($scope, $modalInstance, $filter, common, AddressSvc) {
    $scope.AddAddress = function () {
        if (!Isundefinedornull($scope.Customer.Id)) {
            $scope.Address.CustomerId = $scope.Customer.Id;
            $scope.Address.AddressTypeId = $scope.Address.AddressTypeItem.Id;
        }
        else {
            common.aaNotify.error('Customer is undefined.');
            return false;
        }
        if (!$scope.checkForAddressType() && $scope.Address.AddressTypeId != 2 && $scope.Address.AddressTypeId != "2") {
            common.aaNotify.error($filter('translate')('_AtleastPlantAddress_'));
            return false;
        }
        if (!$scope.Address.IsDefault) {
            $scope.Address.IsDefault = false;
        }
        $scope.SaveAddress($scope.Address);
    };
    $scope.SaveAddress = function (address) {
        common.usSpinnerService.spin('spnAddEditCustomers');
        AddressSvc.Save(address).then(
          function (response) {
              if (ShowMessage(common, response.data)) {
                  common.usSpinnerService.stop('spnAddEditCustomers');
                  $scope.Cancel();
              }
              else {
                  common.usSpinnerService.stop('spnAddEditCustomers');
              }
          },
          function (error) {
              common.usSpinnerService.stop('spnAddEditCustomers');
              console.log(error);
          });
    };
    $scope.Cancel = function () {
        $scope.GetAddressList();
        $modalInstance.dismiss('cancel');
        $scope.destroyScope();
    };
    $scope.destroyScope = function () {
        var lclScope = $scope;
        lclScope.$destroy();
    };
    $scope.ResetAddressPopup = function () {
        $scope.Address = angular.copy($scope.TempAddress);
    };
}
