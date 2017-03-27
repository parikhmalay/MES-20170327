app.controller('SuppliersCtrl', ['$rootScope', '$scope', 'common', 'SuppliersSvc', '$modal', '$filter', '$timeout', 'LookupSvc', function ($rootScope, $scope, common, SuppliersSvc, $modal, $filter, $timeout, LookupSvc) {
    //Start implement security role wise
    $scope.setRoleWisePrivilege = function () {
        $scope.currentSecurityObject = currentSecurityObject;
        if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
            angular.forEach($scope.currentSecurityObject, function (obj, index) {
                switch (obj.ObjectId) {
                    case 17:
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
                    case 18:
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

    $scope.Suppliers = {};
    $scope.SuppliersList = {};
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $scope.SelectAll = false;
    $scope.EmailData = {
        EmailIdsList: [],
        EmailSubject: '',
        EmailBody: '',
        EmailAttachment: ''
    };
    $rootScope.PageHeader = ($filter('translate')('_Suppliers_'));
    $scope.SetLooksupData = function () {
        $scope.LookUps = [
          {
              "Name": "Status", "Parameters": { "source": "SS" }
          },       
          {
              "Name": "Commodity", "Parameters": {}
          }
        ];
        $scope.getLookupData();
    };
    $scope.getLookupData = function () {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "Status") {
                    $scope.StatusList = o.Data;
                }
                else if (o.Name === "Commodity") {
                    $scope.CommodityList = o.Data;
                }
            });
        });
    }

    $scope.disableClick = function ($event, objVal) {
        if (!IsUndefinedNullOrEmpty(objVal))
            $event.stopPropagation();
    };

    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.SetLooksupData();
        $timeout(function () {
            ///Start set search criteria when return from add edit page
            if (localStorage.getItem("SupplierListPaging") && localStorage.getItem("SupplierListPageSearchCriteria")) {
                $scope.Paging = JSON.parse(localStorage.getItem("SupplierListPaging"));
                localStorage.removeItem("SupplierListPaging");
                $scope.SearchCriteria = JSON.parse(localStorage.getItem("SupplierListPageSearchCriteria"));
                localStorage.removeItem("SupplierListPageSearchCriteria");
            }
            ///End set search criteria when return from add edit page
            $scope.GetSuppliersList();
        }, 500);

    };
    $scope.GetSuppliersList = function () {
        common.usSpinnerService.spin('spnSuppliers');
        SuppliersSvc.GetSuppliersList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnSuppliers');
                 if (response.data.StatusCode == 200) {
                     $scope.SuppliersList = response.data.Result;
                     $scope.Paging = response.data.PageInfo;
                     $scope.Paging.Criteria = $scope.SearchCriteria;
                     if ($scope.SuppliersList.length > 0)
                         advanceSearch.close();
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnSuppliers');
                 ////common.aaNotify.error(error);
             });
    };

    $scope.Edit = function (id) {
        localStorage.setItem("SupplierListPaging", JSON.stringify($scope.Paging));
        localStorage.setItem("SupplierListPageSearchCriteria", JSON.stringify($scope.SearchCriteria));
        common.$location.path("/AddEdit/" + id);
    };

    $scope.Delete = function (suppliersId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            SuppliersSvc.Delete(suppliersId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetSuppliersList();
               }
           },
           function (error) {
               ////common.aaNotify.error(error);
           });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetSuppliersList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetSuppliersList();
    };

    $scope.Search = function () {
        if (!IsObjectEmpty($scope.SearchCriteria.CommoditySuppliers)) {
            $scope.CommodityIds = [];
            angular.forEach($scope.SearchCriteria.CommoditySuppliers, function (item) {
                if (!Isundefinedornull(item.Id) && item.Id > 0)
                    $scope.CommodityIds.push(item.Id);
            });
            $scope.SearchCriteria.CommodityIds = $scope.CommodityIds.join(",");
        }
        $scope.Init();
    };

    $scope.ResetSearch = function () {
        $scope.SearchCriteria = { CommoditySuppliers: {} };
        $scope.Init();
    }

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'Suppliers';
        $scope.SchemaName = 'Supplier';
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
        angular.forEach($scope.SuppliersList, function (item) {
            item.chkSelect = $scope.SelectAll;
        });
    };
    $scope.select = function () {
        $scope.SelectAll = true;
        angular.forEach($scope.SuppliersList, function (item) {
            if (!item.chkSelect)
                $scope.SelectAll = false;
        });
    };
    //end logic for checkbox select deselect here and send email

    //popup for send email
    $scope.SendEmails = function () {
        $scope.EmailData = { EmailIdsList: [], EmailSubject: '', EmailBody: '', EmailAttachment: '', EmailFileName: '' };
        angular.forEach($scope.SuppliersList, function (item) {
            if (item.chkSelect && !IsUndefinedNullOrEmpty(item.Email))
                $scope.EmailData.EmailIdsList.push(item.Email);
        });
        if ($scope.EmailData.EmailIdsList.length > 0)
            $scope.ShowSendEmailPopup();
        else
            common.aaNotify.error($filter('translate')('_AtLeastOneSupplier_'));
    };
    $scope.ShowSendEmailPopup = function () {
        var modalTemplatePreviewOptions = $modal.open({
            templateUrl: '/App_Client/views/RFQ/Supplier/SupplierSendEmail.html?v=' + Version,
            controller: ModalSendEmailCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            sizeclass: 'modal-md'
        });
        modalTemplatePreviewOptions.result.then(function () {
        }, function () {
        });
    };
    $scope.SetObjectvaluesSendEmailFile = function (file, name) {
        $scope.EmailData.EmailAttachment = file;
        $scope.EmailData.EmailFileName = name;
    }
    $scope.deleteFileSendEmailFile = function (filePath) {
        $scope.EmailData.EmailAttachment = '';
        $scope.EmailData.EmailFileName = '';
    }
    // send email end here

    //Start code for Multiple supplier delete.
    $scope.DeleteMultipleSuppliers = function () {
        $scope.SupplierIDs = [];
        angular.forEach($scope.SuppliersList, function (item) {
            if (item.chkSelect && !IsUndefinedNullOrEmpty(item.Id))
                $scope.SupplierIDs.push(item.Id);
        });
        if ($scope.SupplierIDs.length > 0)
            $scope.DeleteSuppliers();
        else
            common.aaNotify.error($filter('translate')('_AtLeastOneSupplier_'));
    };
    $scope.DeleteSuppliers = function () {
        var supplierIDs = $scope.SupplierIDs.join(",");
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            SuppliersSvc.DeleteMultiple(supplierIDs).then(
              function (response) {
                  if (ShowMessage(common, response.data)) {
                      $scope.Init();
                  }
              },
              function (error) {
                  ////common.aaNotify.error(error);
              });
        }
    }
    //End code for Multiple supplier delete.

    $scope.Init();
}]);

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
}

var ModalSendEmailCtrl = function ($scope, $modalInstance, common, SuppliersSvc) {
    $scope.SendEmail = function () {
        common.usSpinnerService.spin('spnSuppliers');
        SuppliersSvc.SendEmail($scope.EmailData).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   common.usSpinnerService.stop('spnSuppliers');
                   $scope.Cancel();
               }
               else {
                   common.usSpinnerService.stop('spnSuppliers');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnSuppliers');
               ////common.aaNotify.error(error);
               console.log(error);
           });
    };

    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
}

app.controller('AddEditSuppliersCtrl', ['$rootScope', '$scope', 'common', 'SuppliersSvc', '$modal', 'LookupSvc', '$filter', '$routeParams', '$timeout', '$confirm', 'ContactsSvc', 'FileUploaderSvc', 'SupplierAssessmentSvc',
    function ($rootScope, $scope, common, SuppliersSvc, $modal, LookupSvc, $filter, $routeParams, $timeout, $confirm, ContactsSvc, FileUploaderSvc, SupplierAssessmentSvc) {
        //Start implement security role wise
        $scope.setRoleWisePrivilege = function () {
            $scope.currentSecurityObject = currentSecurityObject;
            if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
                angular.forEach($scope.currentSecurityObject, function (obj, index) {
                    switch (obj.ObjectId) {
                        case 17:
                            switch (obj.PrivilegeId) {
                                case 1:                           //none
                                    RedirectToAccessDenied();
                                    break;
                                case 2:                           //read
                                    $scope.IsReadOnlyPage = true;
                                    break;
                            }
                            break;
                        case 18:
                            if ($scope.TransactionMode == "Create") {
                                $scope.PagePrivilegeCase(obj);
                            }
                            break;
                        case 19:
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

        $scope.SelectAllObj = { SelectAll: false };
        $('body').removeClass('paginationFixedToBottom haveAdvanceSearch');
        $rootScope.PageHeader = ($filter('translate')('_PageHeading_'));

        $scope.Init = function () {
            $scope.Supplier = {
                lstContact: [],
                lstDocument: []
            };
            $scope.SetLooksupData();
            $scope.Contact = { PrefixId: 0 };
            $scope.Document = {};



            $scope.Document.DocumentFilePath = ''
            $scope.Assessment = [];
            $scope.WorkQualityRating = 0;
            $scope.TimelineRating = 0;
            $scope.CostingRating = 0;

            $scope.QualityClass = 'green-notification';
            $scope.ScoreEHS = 90;
            $timeout(function () {
                if (!IsUndefinedNullOrEmpty($routeParams.Id) && $routeParams.Id != '0') {
                    $scope.AddEditContact = false;
                    $scope.AddEditAssessment = false;
                    $scope.TransactionMode = 'Edit';
                    $scope.Supplier.Id = $routeParams.Id;
                    $scope.getData($routeParams.Id);
                }
                else {
                    $scope.AddEditContact = true;
                    $scope.AddEditAssessment = true;
                    $scope.TransactionMode = 'Create';
                    //set MES SQ default for supplier quality
                    angular.forEach($scope.SupplierQualityList, function (item) {
                        if (item.Name == 'MES SQ')
                            $scope.Supplier.SQId = item.Id;
                    });
                    //set default status Active
                    $scope.Supplier.Status = 17;
                }
                $scope.setRoleWisePrivilege();
            }, 2000);
        }

        $scope.SetLooksupData = function () {
            $scope.LookUps = [
              { "Name": "Countries", "Parameters": {} },
              {
                  "Name": "SupplierQuality", "Parameters": {}
              },
              {
                  "Name": "Status", "Parameters": { "source": "SS" }
              },
              {
                  "Name": "CommodityTypes", "Parameters": {}
              },
               {
                   "Name": "Commodity", "Parameters": {}
               },
              {
                  "Name": "Prefixes", "Parameters": {}
              },
              {
                  "Name": "DocumentTypes", "Parameters": { "associatedTo_id": "SP" }
              }
            ];
            $scope.getLookupData();
        };

        $scope.getData = function (supplierId) {
            common.usSpinnerService.spin('spnAddEditSuppliers');
            SuppliersSvc.getData(supplierId).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       common.usSpinnerService.stop('spnAddEditSuppliers');
                       $scope.Supplier = response.data.Result;
                       angular.forEach($scope.Supplier.lstDocument, function (o) {
                           o.ExpirationDate = convertUTCDateToLocalDate(o.ExpirationDate);
                       });
                       angular.forEach($scope.Supplier.lstAssessment, function (o) {
                           o.CreatedDate = convertUTCDateToLocalDate(o.CreatedDate);
                           o.AuditDate = convertUTCDateToLocalDate(o.AuditDate);
                       });
                       $scope.WorkQualityRating = $scope.Supplier.WorkQualityRating;
                       $scope.TimelineRating = $scope.Supplier.TimelineRating;
                       $scope.CostingRating = $scope.Supplier.CostingRating;
                       $scope.Supplier.AssessmentDate = convertUTCDateToLocalDate($scope.Supplier.AssessmentDate);
                   }
                   else {
                       common.usSpinnerService.stop('spnAddEditSuppliers');
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnAddEditSuppliers');
                   console.log(error);
               });
        };

        $scope.getLookupData = function () {
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                angular.forEach(data.data, function (o) {
                    if (o.Name === "Countries") {
                        $scope.CountryList = o.Data;
                    }
                    else if (o.Name === "SupplierQuality") {
                        $scope.SupplierQualityList = o.Data;
                    }
                    else if (o.Name === "Status") {
                        $scope.StatusList = o.Data;
                    }
                    else if (o.Name === "CommodityTypes") {
                        $scope.CommodityList = o.Data;
                    }
                    else if (o.Name === "Commodity") {
                        $scope.CommodityList = o.Data;
                    }
                    else if (o.Name === "Prefixes") {
                        $scope.PrefixList = o.Data;
                    }
                    else if (o.Name === "DocumentTypes") {
                        $scope.DocumentList = o.Data;
                    }
                });
            });
        }

        $scope.SaveSuppliers = function (closeForm) {
            if (IsUndefinedNullOrEmpty($scope.Supplier.Id) || $scope.Supplier.Id == 0) {
                if (!$scope.ContactValidation($scope.Contact))
                    return false;
                $scope.setListObjects();
            }
            common.usSpinnerService.spin('spnAddEditSuppliers');
            SuppliersSvc.Save($scope.Supplier).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       common.usSpinnerService.stop('spnAddEditSuppliers');
                       $scope.Id = response.data.Result; // Id of latest created record
                       $scope.Supplier.Id = response.data.Result;
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
                       common.usSpinnerService.stop('spnAddEditSuppliers');
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnAddEditSuppliers');
                   console.log(error);
               });
        };

        $scope.setListObjects = function () {
            $scope.Supplier.lstContact = [];
            $scope.Supplier.AssessmentDate = new Date();
            //$scope.Contact.PrefixId = $scope.Contact.PrefixId;
            //$scope.Contact.Prefix = $scope.Contact.PrefixItem.Name;
            $scope.Contact.IsDefault = true;
            $scope.Supplier.lstContact.push($scope.Contact);
        }

        $scope.reloadWithId = function () {
            common.$location.path('/AddEdit/' + $scope.Supplier.Id);
        };
        ////***Start code for contact***/////
        $scope.EditContact = function (contact, index) {
            $scope.AddNewContact();
            $scope.Contact = contact;
            $scope.TempContact = contact;
            $scope.ContactIndex = index;
            $scope.IsDefault = $scope.Contact.IsDefault;
        };
        $scope.DeleteContact = function (contactId, index) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                $scope.Supplier.lstContact.splice(index, 1);
                common.aaNotify.success(($filter('translate')('_SupplierContact_')) + ' is deleted successfully.');

                //ContactsSvc.Delete(contactId).then(
                //  function (response) {
                //      if (ShowMessage(common, response.data)) {
                //          $scope.GetContactsList();
                //      }
                //  },
                //  function (error) {
                //      ////common.aaNotify.error(error);
                //  });
            }
        };
        $scope.DeleteMultipleContact = function () {
            $scope.ContactIds = [];
            angular.forEach($scope.Supplier.lstContact, function (item, index) {
                item.Id = index + 1;
                if (item.chkSelect)
                    $scope.ContactIds.push(item.Id);
            });
            if ($scope.ContactIds.length > 0) {
                if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                    angular.forEach($scope.ContactIds, function (item, index) {
                        angular.forEach($scope.Supplier.lstContact, function (nestItem, nestIndex) {
                            if (item == nestItem.Id)
                                $scope.Supplier.lstContact.splice(nestIndex, 1);
                        });
                    });
                    common.aaNotify.success(($filter('translate')('_SupplierContact_')) + ' is deleted successfully.');
                }
            }
            else
                common.aaNotify.error($filter('translate')('_AtLeastOneContact_'));
        };
        $scope.ResetContacts = function () {
            $scope.Contact = {
                Id: 0,
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
            $scope.ContactIndex = null;
            $scope.TempContact = {};
        };
        $scope.AddNewContact = function () {
            $scope.AddEditContact = true;
            $scope.IsDefault = false;
            $scope.Contact.PrefixId = 0;

            $scope.ShowContactPopup();
        };
        $scope.ContactValidation = function (objContact) {
            if (IsUndefinedNullOrEmpty(objContact.FirstName)) {
                common.aaNotify.error(($filter('translate')('_FirstName_')) + ' is required.');
                return false;
            }
            else if (IsUndefinedNullOrEmpty(objContact.LastName)) {
                common.aaNotify.error(($filter('translate')('_LastName_')) + ' is required.');
                return false;
            }
            else if (IsUndefinedNullOrEmpty(objContact.DirectPhone)) {
                common.aaNotify.error(($filter('translate')('_Phone_')) + ' is required.');
                return false;
            }
            else if (!IsUndefinedNullOrEmpty(objContact.Suffix) && getLength(objContact.Suffix.trim()) > 5) {
                common.aaNotify.error(($filter('translate')('_Suffix_')) + ' must be less than or equal to 5 characters.');
                return false;
            }
            else if (!Isundefinedornull(objContact.Extension) && getLength(objContact.Extension) > 5) {
                common.aaNotify.error(($filter('translate')('_Ext_')) + ' must be less than or equal to 5 characters.');
                return false;
            }
            else if (!IsUndefinedNullOrEmpty(objContact.Email)) {
                if (!IsValidEmail(objContact.Email)) {
                    common.aaNotify.error(($filter('translate')('_Email_')) + ' is invalid.');
                    return false;
                }
            }
            return true;
        };
        ////***End code for contact***/////

        ////***Start code for document****/////
        $scope.EditDocument = function (document, index) {
            $scope.AddNewDocument();
            $scope.Document = document;
            $scope.DocumentIndex = index;
            $scope.SetObjectvalues(document.DocumentFilePath, document.DocumentFileName);
        };
        $scope.DeleteDocument = function (index) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                $scope.Supplier.lstDocument.splice(index, 1);
                $scope.ResetDocuments();
                common.aaNotify.success(($filter('translate')('_DocumentType_')) + ' is deleted successfully.');
            }
        };
        $scope.CancelDocument = function () {
            $scope.ResetDocuments();
        };
        $scope.ResetDocuments = function () {
            $scope.Document = { DocumentFilePath: '', DocumentFileName: '' };
            $scope.DocumentIndex = null;
            $scope.AddEditDocument = false;
        };
        $scope.AddNewDocument = function () {
            $scope.selectedFiles = [];
            $scope.AddEditDocument = true;
            $scope.ShowDocumentPopup();
        };
        $scope.DocumentValidation = function (objDocument) {
            if (Isundefinedornull(objDocument.DocumentTypeItem) || Isundefinedornull(objDocument.DocumentTypeItem.Id)) {
                common.aaNotify.error(($filter('translate')('_DocumentType_')) + ' is required.');
                return false;
            }
            else if (IsUndefinedNullOrEmpty(objDocument.DocumentFilePath)) {
                common.aaNotify.error(($filter('translate')('_Document_')) + ' is required.');
                return false;
            }
            else if (Isundefinedornull(objDocument.ExpirationDate)) {
                common.aaNotify.error(($filter('translate')('_ExpirationDate_')) + ' is required.');
                return false;
            }
            return true;
        };

        $scope.SetObjectvalues = function (file, name) {
            $scope.selectedFiles = [];
            $scope.TempFile = { 'UploadedFilePath': file, 'UploadedFileName': name };
            $scope.selectedFiles.push($scope.TempFile);
            $scope.Document.DocumentFilePath = file;
            $scope.Document.DocumentFileName = name;
        }
        $scope.deleteFile = function (filePath, index) {
            $scope.selectedFiles.splice(index, 1);
            $scope.Document.DocumentFilePath = '';
            $scope.Document.DocumentFileName = '';

            //FileUploaderSvc.Deletedocument(filePath).then(
            //       function (response) {
            //       },
            //       function (error) {
            //       });
        }
        ////***End code for document****/////

        ////***Start code for assessment****/////
        $scope.GetAssessmentList = function () {
            $scope.Supplier.lstAssessment = [];
            common.usSpinnerService.spin('spnAddEditAssessment');
            SupplierAssessmentSvc.GetAssessmentList($scope.Supplier.Id).then(
                 function (response) {
                     common.usSpinnerService.stop('spnAddEditAssessment');
                     $scope.Supplier.lstAssessment = response.data;
                     angular.forEach($scope.Supplier.lstAssessment, function (o) {
                         o.CreatedDate = convertUTCDateToLocalDate(o.CreatedDate);
                         o.AuditDate = convertUTCDateToLocalDate(o.AuditDate);
                     });
                     //$scope.ResetAssessment();
                 },
                 function (error) {
                     common.usSpinnerService.stop('spnAddEditAssessment');
                     ////common.aaNotify.error(error);
                 });
        };
        $scope.EditAssessment = function (assessment, index) {
            //$scope.AddNewAssessment();
            //$scope.Assessment = document;
            //$scope.AssessmentIndex = index;
            // $scope.SetObjectvalues(document.DocumentFilePath, document.DocumentFileName);
            if (assessment.IsNew) {
                $confirm({ title: ($filter('translate')('_ConfirmText_')), ok: ($filter('translate')('_Yes_')), cancel: ($filter('translate')('_No_')), close: 'C', text1: 'Supplier Code: ' + $scope.Supplier.SupplierCode, text2: 'Supplier Name: ' + $scope.Supplier.CompanyName })
                .then(function (ret) {
                    if (ret == 1) {
                        return;
                    }
                    else {
                        $scope.CreateRevision(assessment);
                        $scope.ShowAssessmentPopup();
                    }
                },

            function () {
                $scope.GetAssessment(assessment.AssessmentId);
                $scope.ShowAssessmentPopup();
            }
            );
            }
            else {
                $scope.GetAssessment(assessment.AssessmentId);
                $scope.ShowAssessmentPopup();
            }
        };
        $scope.DeleteAssessment = function (index) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                //$scope.Supplier.lstDocument.splice(index, 1);
                //$scope.ResetDocuments();
                //common.aaNotify.success(($filter('translate')('_DocumentType_')) + ' is deleted successfully.');
            }
        };
        //$scope.CancelAssessment = function () {
        //    $scope.ResetAssessment();
        //};
        $scope.ResetAssessment = function () {
            $scope.Assessment = [];
            $scope.AssessmentIndex = null;
            $scope.AddEditAssessment = false;
        };
        $scope.AddNewAssessment = function () {
            //$scope.selectedFiles = [];
            //$scope.AddEditAssessment = true;
            $scope.GetAssessmentDetail();
            $scope.ShowAssessmentPopup();
        };

        $scope.AssessmentValidation = function (objAssessment) {
            var flag = true;
            angular.forEach($scope.Assessment.QuestionType, function (item) {
                angular.forEach(item.AssessmentElement, function (elementItem) {
                    angular.forEach(elementItem.ElementDetails, function (detailItem) {
                        if (Isundefinedornull(detailItem.AssessmentElementValue.Score)) {
                            //common.aaNotify.error(($filter('translate')('_ScoreRequiredMessage_')));

                            //common.aaNotify.error(($filter('translate')('_Score_')) + ' is required.');
                            flag = false;
                        }
                    });
                });
            });
            if (flag == false) {
                common.aaNotify.error(($filter('translate')('_ScoreRequiredMessage_')));
                return false;
            }
            return true;
        };

        $scope.SetAssessmentObjectvalues = function (file, name) {
            $scope.selectedAssessmentFiles = [];
            $scope.TempFile = { 'UploadedFilePath': file, 'UploadedFileName': name };
            $scope.selectedAssessmentFiles.push($scope.TempFile);
            $scope.Assessment.EquipmentCapacityList_FilePath = file;
            $scope.Assessment.EquipmentCapacityList_FileName = name;
        }
        $scope.DeleteAssessmentFile = function (filePath, index) {
            $scope.selectedAssessmentFiles = [];
            $scope.Assessment.EquipmentCapacityList_FilePath = '';
            $scope.Assessment.EquipmentCapacityList_FileName = '';
        }

        $scope.GetAssessmentDetail = function () {
            common.usSpinnerService.spin('spnAddEditAssessment');
            SupplierAssessmentSvc.GetAssessmentDetail($scope.Supplier.Id).then(
              function (response) {
                  if (ShowMessage(common, response.data)) {
                      $scope.Assessment = response.data.Result;
                      $scope.Assessment.AuditDate = convertUTCDateToLocalDate($scope.Assessment.AuditDate);

                      angular.forEach($scope.Assessment.QuestionType, function (item) {
                          angular.forEach(item.AssessmentElement, function (elementItem) {
                              angular.forEach(elementItem.ElementDetails, function (detailItem) {
                                  // detailItem.SubElementDetail.SubElementValue = '';
                                  //angular.forEach(detailItem.AssessmentElementValue, function (valueItem) {
                                  detailItem.AssessmentElementValue.Score = null;
                                  //detailItem.AssessmentElementValue.Weight = null;
                                  detailItem.AssessmentElementValue.SubTotal = 'n/r';
                                  detailItem.AssessmentElementValue.selectedAssessmentValueFiles = [];
                                  detailItem.AssessmentElementValue.TempFile = {};
                                  //});
                              });
                          });
                      });
                      $timeout(function () {
                          $scope.$broadcast("loadScrollerPanel");
                      }, 1000);
                      common.usSpinnerService.stop('spnAddEditAssessment');
                  }
                  else {
                      common.usSpinnerService.stop('spnAddEditAssessment');
                  }
                  $scope.$broadcast("loadScrollerPanel");
              },
              function (error) {
                  common.usSpinnerService.stop('spnAddEditAssessment');
                  console.log(error);
              });
        };

        //Craete Assessment Revision..
        $scope.CreateRevision = function (assessment) {
            common.usSpinnerService.spin('spnAddEditAssessment');
            SupplierAssessmentSvc.CreateRevision(assessment.AssessmentId).then(
              function (response) {
                  if (ShowMessage(common, response.data)) {
                      var Id = response.data.Result;
                      $scope.GetAssessment(Id);
                      common.usSpinnerService.stop('spnAddEditAssessment');
                  }
                  else {
                      common.usSpinnerService.stop('spnAddEditAssessment');
                  }
              },
              function (error) {
                  common.usSpinnerService.stop('spnAddEditAssessment');
                  console.log(error);
              });
        };
        //Get assessment for Edit Assessment..
        $scope.GetAssessment = function (Id) {
            common.usSpinnerService.spin('spnAddEditAssessment');
            SupplierAssessmentSvc.getData(Id).then(

              function (response) {

                  if (ShowMessage(common, response.data)) {
                      var assessmentObj = response.data.Result;
                      $scope.Assessment = assessmentObj;
                      $scope.Assessment.AuditDate = convertUTCDateToLocalDate($scope.Assessment.AuditDate);
                      $scope.SetAssessmentObjectvalues($scope.Assessment.EquipmentCapacityList_FilePath, $scope.Assessment.EquipmentCapacityList_FileName);


                      angular.forEach($scope.Assessment.QuestionType, function (item) {
                          angular.forEach(item.AssessmentElement, function (elementItem) {
                              angular.forEach(elementItem.ElementDetails, function (detailItem) {

                                  if (detailItem.AssessmentElementValue.FileName != null && detailItem.AssessmentElementValue.FileName != '')
                                      $scope.SetAssessmentValueObjectvalues(detailItem.AssessmentElementValue.FilePath, detailItem.AssessmentElementValue.FileName, detailItem);

                                  if (detailItem.Weight != null) {
                                      $scope.Assessment.WeightScore = parseFloat($scope.Assessment.WeightScore) + parseFloat(detailItem.Weight);
                                  }
                              });
                          });
                      });
                      if ($scope.Assessment.TotalScore == 0 && $scope.Assessment.WeightScore == 0)
                      { $scope.Assessment.QualityScore = 0; }
                      else {
                          $scope.Assessment.QualityScore = ($scope.Assessment.TotalScore / $scope.Assessment.WeightScore) * 5;
                          $scope.Assessment.QualityScore = $scope.Assessment.QualityScore.toFixed(2);
                      }
                      if ($scope.Assessment.QualityScore >= 70) {
                          $scope.QualityClass = 'green-notification';
                      }
                      else if ($scope.Assessment.QualityScore >= 60 && $scope.Assessment.QualityScore < 70) {
                          $scope.QualityClass = 'yellow-notification';
                      }
                      else {
                          $scope.QualityClass = 'red-notification';
                      }

                      if ($scope.Assessment.EHSScore > 0)
                          $scope.Assessment.FinalEHSScore = ($scope.Assessment.EHSScore / $scope.ScoreEHS) * 100;
                      else
                          $scope.Assessment.FinalEHSScore = 0;

                      $scope.Assessment.EHSScore = $scope.Assessment.EHSScore.toFixed(2);

                      $scope.Assessment.FinalEHSScore = $scope.Assessment.FinalEHSScore.toFixed(2);
                      common.usSpinnerService.stop('spnAddEditAssessment');
                  }
                  else {
                      common.usSpinnerService.stop('spnAddEditAssessment');
                  }

              },
              function (error) {
                  common.usSpinnerService.stop('spnAddEditAssessment');
                  console.log(error);
              });
        };


        $scope.CalculateScore = function (detailItem, flag) {
            var weight = detailItem.Weight;
            var score = detailItem.AssessmentElementValue.Score;
            var result = '';

            if (weight == null && score == null) {
                result = 'n/r';
            }
            else if (weight != null && score == null) {
                result = 'n/r';
            }
            else if (weight == null && score != null) {
                result = '0';
            }
            else {
                result = parseInt(weight, 0) * parseInt(score, 0);
            }

            if (flag && detailItem.IsOptionAvailable == true)
            { detailItem.AssessmentElementValue.OptionValue = null; }


            detailItem.AssessmentElementValue.SubTotal = result;
            $scope.FinalTotal(detailItem);

        };

        $scope.FinalTotal = function () {
            $scope.Assessment.WeightScore = 0;
            $scope.Assessment.TotalScore = 0;
            $scope.Assessment.QualityScore = 0;

            $scope.Assessment.EHSScore = 0;
            var cnt = 0;

            angular.forEach($scope.Assessment.QuestionType, function (item) {
                angular.forEach(item.AssessmentElement, function (elementItem) {
                    angular.forEach(elementItem.ElementDetails, function (detailItem) {
                        if (elementItem.Id != 12 && detailItem.AssessmentElementValue.SubTotal != 'n/r') {
                            cnt++;
                            $scope.Assessment.TotalScore = parseFloat($scope.Assessment.TotalScore, 0) + parseFloat(detailItem.AssessmentElementValue.SubTotal, 0);
                        }
                        if (elementItem.Id != 12 && detailItem.Weight != null) {
                            $scope.Assessment.WeightScore = parseFloat($scope.Assessment.WeightScore) + parseFloat(detailItem.Weight);
                        }
                        if (elementItem.Id == 12) {
                            if (detailItem.AssessmentElementValue.SubTotal != 'n/r') {
                                $scope.Assessment.EHSScore = parseFloat($scope.Assessment.EHSScore, 0) + parseFloat(detailItem.AssessmentElementValue.SubTotal, 0);
                            }
                        }
                    });
                });
            });

            if (cnt < 12) {
                $scope.Assessment.QualityScore = 'Invalid';
                $scope.QualityClass = 'green-notification';
            }
            else {
                if ($scope.Assessment.TotalScore == 0 && $scope.Assessment.WeightScore == 0)
                { $scope.Assessment.QualityScore = 0; }
                else {
                    $scope.Assessment.QualityScore = ($scope.Assessment.TotalScore / $scope.Assessment.WeightScore) * 5;

                    $scope.Assessment.QualityScore = $scope.Assessment.QualityScore.toFixed(2);
                }
                if ($scope.Assessment.QualityScore >= 70) {
                    $scope.QualityClass = 'green-notification';
                }
                else if ($scope.Assessment.QualityScore >= 60 && $scope.Assessment.QualityScore < 70) {
                    $scope.QualityClass = 'yellow-notification';
                }
                else {
                    $scope.QualityClass = 'red-notification';
                }
            }

            if ($scope.Assessment.EHSScore > 0)
                $scope.Assessment.FinalEHSScore = ($scope.Assessment.EHSScore / $scope.ScoreEHS) * 100;
            else
                $scope.Assessment.FinalEHSScore = 0;

            $scope.Assessment.EHSScore = $scope.Assessment.EHSScore.toFixed(2);

            $scope.Assessment.FinalEHSScore = $scope.Assessment.FinalEHSScore.toFixed(2);
        }

        $scope.SetAssessmentValueObjectvalues = function (file, name, detailItem) {
            detailItem.AssessmentElementValue.selectedAssessmentValueFiles = [];
            detailItem.AssessmentElementValue.TempFile = { 'UploadedFilePath': file, 'UploadedFileName': name };
            detailItem.AssessmentElementValue.selectedAssessmentValueFiles.push(detailItem.AssessmentElementValue.TempFile);
            detailItem.AssessmentElementValue.FilePath = file;
            detailItem.AssessmentElementValue.FileName = name;
        }

        $scope.DeleteAssessmentValueFiles = function (filePath, index, detailItem) {
            detailItem.AssessmentElementValue.selectedAssessmentValueFiles = [];
            detailItem.AssessmentElementValue.FielPath = '';
            detailItem.AssessmentElementValue.FileName = '';
        }

        $scope.AssessmentScore = function (rating, detailItem) {
            // $scope.AssessmentElementValue.WorkQualityRating = rating;
            detailItem.AssessmentElementValue.Score = rating;
            $scope.CalculateScore(detailItem, true);
        };

        $scope.SetRating = function (detailItem) {
            if (detailItem.AssessmentElementValue.OptionValue == 'Yes') {
                detailItem.AssessmentElementValue.Score = 5;
            }
            else if (detailItem.AssessmentElementValue.OptionValue == 'No') {
                detailItem.AssessmentElementValue.Score = 1;
            }
            else {
                detailItem.AssessmentElementValue.Score = null;
            }
            $scope.CalculateScore(detailItem, false);
        };
        ////***End code for assessment****/////

        $scope.RedirectToList = function () {
            common.$location.path("/");
        }

        $scope.ResetForm = function () {
            common.$route.reload();
        }

        $scope.WorkQualityRate = function (rating) {
            $scope.Supplier.WorkQualityRating = rating;
        };
        $scope.TimeLineRate = function (rating) {
            $scope.Supplier.TimelineRating = rating;
        };
        $scope.CostingRate = function (rating) {
            $scope.Supplier.CostingRating = rating;
        };

        //popup for show contact form
        $scope.ShowContactPopup = function () {
            var modalTemplatePreviewOptions = $modal.open({
                templateUrl: '/App_Client/views/RFQ/Supplier/SupplierContact.html?v=' + Version,
                controller: ModalContactCtrl,
                keyboard: false,
                backdrop: false,
                scope: $scope,
                sizeclass: 'modal-md'
            });
            modalTemplatePreviewOptions.result.then(function () {
            }, function () {
            });
        };
        // contact popup end here

        //popup for show document form
        $scope.ShowDocumentPopup = function () {
            var modalTemplatePreviewOptions = $modal.open({
                templateUrl: '/App_Client/views/RFQ/Supplier/SupplierDocument.html?v=' + Version,
                controller: ModalDocumentCtrl,
                keyboard: false,
                backdrop: false,
                scope: $scope,
                sizeclass: 'modal-md'
            });
            modalTemplatePreviewOptions.result.then(function () {
            }, function () {
            });
        };
        // document popup end here

        //popup for show assessment form
        $scope.ShowAssessmentPopup = function () {
            var modalTemplatePreviewOptions = $modal.open({
                templateUrl: '/App_Client/views/RFQ/Supplier/AddEditSupplierAssessment.html?v=' + Version,
                controller: ModalAssessmentCtrl,
                keyboard: false,
                backdrop: false,
                scope: $scope,
                sizeclass: 'modal-extra-full modal-fitToScreen'
            });
            modalTemplatePreviewOptions.result.then(function () {
            }, function () {
            });
        };
        // assessment popup end here

        //Start logic for checkbox select deselect here
        $scope.SelectDeselectAll = function () {
            angular.forEach($scope.Supplier.lstContact, function (item) {
                if (!item.IsDefault)
                    item.chkSelect = $scope.SelectAllObj.SelectAll;
            });
        };
        $scope.select = function () {
            $scope.SelectAllObj.SelectAll = true;
            angular.forEach($scope.Supplier.lstContact, function (item) {
                if (!item.IsDefault && !item.chkSelect)
                    $scope.SelectAllObj.SelectAll = false;
            });
        };
        //end logic for checkbox select deselect here

        $scope.Init();
    }]);

var ModalContactCtrl = function ($scope, $modalInstance, common, $filter, ContactsSvc) {
    console.log($scope.Contact);

    $scope.AddContact = function () {
        if (!$scope.ContactValidation($scope.Contact))
            return false;

        if (!$scope.Contact.IsDefault) {
            $scope.Contact.IsDefault = false;
        }
        if (Isundefinedornull($scope.Supplier.lstContact) || $scope.Supplier.lstContact.length == 0)
            $scope.Contact.IsDefault = true;

        if (!Isundefinedornull($scope.ContactIndex)) {
            $scope.Supplier.lstContact.splice($scope.ContactIndex, 1);
        }
        if ($scope.Contact.IsDefault) {
            angular.forEach($scope.Supplier.lstContact, function (item, index) {
                item.IsDefault = false;
            });
        }
        $scope.Supplier.lstContact.push($scope.Contact);
        $scope.Cancel();
        common.aaNotify.success(($filter('translate')('_SupplierContact_')) + ' is added successfully.');
        // $scope.SaveContact($scope.Contact);
    };
    $scope.Cancel = function () {
        $scope.ResetContacts();
        $modalInstance.dismiss('cancel');
        $scope.destroyScope();
    }
    $scope.destroyScope = function () {
        var lclScope = $scope;
        lclScope.$destroy();
    };

    $scope.ResetContactPopup = function () {
        $scope.Contact = angular.copy($scope.TempContact);
    };

    $scope.ViewInstruction = function () {

    };
}

var ModalDocumentCtrl = function ($scope, $modalInstance, common, $filter, FileUploaderSvc) {
    $scope.AddDocument = function () {
        //$scope.Document.ExpirationDate = new Date();
        if (!$scope.DocumentValidation($scope.Document))
            return false;
        if (!Isundefinedornull($scope.DocumentIndex)) {
            $scope.Supplier.lstDocument.splice($scope.DocumentIndex, 1);
        }
        $scope.Document.DocumentTypeId = $scope.Document.DocumentTypeItem.Id;
        $scope.Supplier.lstDocument.push($scope.Document);
        $scope.Cancel();
        common.aaNotify.success(($filter('translate')('_DocumentType_')) + ' is added successfully.');
    };
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
        $scope.ResetDocuments();
        $scope.destroyScope();
    };
    $scope.destroyScope = function () {
        var lclScope = $scope;
        lclScope.$destroy();
    };
    ////this is for single file upload

}

//Assessment Modal..
var ModalAssessmentCtrl = function ($scope, $modalInstance, common, SupplierAssessmentSvc) {
    $scope.AddAssessment = function () {
        if (!$scope.AssessmentValidation($scope.Assessment))
            return false;
        if (!Isundefinedornull($scope.Supplier.Id)) {
            $scope.Assessment.SupplierId = $scope.Supplier.Id;
        }
        else {
            common.aaNotify.error('Supplier is undefined.');
            return false;
        }
        $scope.SaveAssessment($scope.Assessment);
    };
    $scope.SaveAssessment = function (assessment) {
        common.usSpinnerService.spin('spnAddEditSuppliers');
        //if (!$scope.AssessmentValidation($scope.Assessment))
        //    return false;
        SupplierAssessmentSvc.Save(assessment).then(
          function (response) {
              console.log(response.data);
              if (ShowMessage(common, response.data)) {
                  common.usSpinnerService.stop('spnAddEditSuppliers');
                  $scope.CancelAssessment();
              }
              else {
                  common.usSpinnerService.stop('spnAddEditSuppliers');
              }
          },
          function (error) {
              common.usSpinnerService.stop('spnAddEditSuppliers');
              console.log(error);
          });
    };
    $scope.CancelAssessment = function () {
        $scope.GetAssessmentList();
        $modalInstance.dismiss('cancel');
        $scope.destroyScope();
    }
    $scope.destroyScope = function () {
        var lclScope = $scope;
        lclScope.$destroy();
    };

    $scope.ResetAssessmentPopup = function () {
        $scope.Assessment = [];
        $scope.AssessmentIndex = null;
        $scope.AddEditAssessment = false;
    };
}