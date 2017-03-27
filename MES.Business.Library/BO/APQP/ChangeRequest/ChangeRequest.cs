using Account.DTO.Library;
using MES.Business.Repositories.APQP.ChangeRequest;
using NPE.Core;
using NPE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Core.Extensions;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using MES.Business.Mapping.Extensions;
using System.Net;
using System.Xml;
using MES.Business.Library.Enums;

using MES.Business.Library.Extensions;
using System.IO;
using MES.Business.Repositories.RoleManagement;
using MES.DTO.Library.Identity;
namespace MES.Business.Library.BO.APQP.ChangeRequest
{
    class ChangeRequest : ContextBusinessBase, IChangeRequestRepository
    {
        public ChangeRequest()
            : base("ChangeRequest")
        { }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.APQP.ChangeRequest.ChangeRequest changeRequest)
        {
            string errMSg = null;
            string successMsg = null;

            #region "get Watchers Ids"
            changeRequest.WatcherIds = string.Empty;
            if (changeRequest.WatcherItems != null && changeRequest.WatcherItems.Count > 0)
            {
                foreach (var watcherItem in changeRequest.WatcherItems)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(watcherItem.Id)))
                    {
                        if (string.IsNullOrEmpty(changeRequest.WatcherIds))
                            changeRequest.WatcherIds = Convert.ToString(watcherItem.Id);
                        else
                            changeRequest.WatcherIds = changeRequest.WatcherIds + "," + Convert.ToString(watcherItem.Id);
                    }
                }
            }
            #endregion

            ObjectParameter ChangeRequestId = new ObjectParameter("ChangeRequestId", changeRequest.Id);
            ObjectParameter ErrorKey = new ObjectParameter("ErrorKey", "");
            this.RunOnDB(context =>
            {
                int result = context.crSaveChangeRequests(changeRequest.APQPItemId, changeRequest.SourceOfChange, System.Web.HttpContext.Current.Server.HtmlEncode(changeRequest.DescriptionOfChange), changeRequest.RevLevel, changeRequest.DrawingRevDate,
                    changeRequest.MfgStartDateForNewRev, changeRequest.Subject, changeRequest.StatusId, changeRequest.AssignedToId, changeRequest.IsChangeApproved, changeRequest.IsChangeImplemented,
                    changeRequest.apqpPurchasePieceCost, changeRequest.apqpSellingPiecePrice, changeRequest.apqpPurchaseToolingCost, changeRequest.apqpSellingToolingPrice, changeRequest.PurchasePieceCost,
                    changeRequest.SellingPiecePrice, changeRequest.PurchaseToolingCost, changeRequest.SellingToolingPrice, changeRequest.WatcherIds, changeRequest.DrawingNumber, CurrentUser, ChangeRequestId, ErrorKey);
                if (Convert.ToInt32(ChangeRequestId.Value) <= 0 || !string.IsNullOrEmpty(Convert.ToString(ErrorKey.Value)))
                {
                    errMSg = Languages.GetResourceText("CRSaveFail");
                }
                else
                {
                    changeRequest.Id = Convert.ToInt32(ChangeRequestId.Value);
                    SendEmailToWatchers(changeRequest.Id);
                    #region "Save document Details"
                    if (changeRequest.lstDocument != null && changeRequest.lstDocument.Count > 0)
                    {
                        MES.Business.Repositories.APQP.ChangeRequest.IDocumentRepository objIDocumentRepository = null;
                        foreach (var objDocument in changeRequest.lstDocument)
                        {
                            objIDocumentRepository = new MES.Business.Library.BO.APQP.ChangeRequest.Document();
                            objDocument.ChangeRequestId = changeRequest.Id;
                            var objResult = objIDocumentRepository.Save(objDocument);
                            if (objResult == null || objResult.StatusCode != 200)
                            {
                                errMSg = Languages.GetResourceText("CRDocSaveFail");
                            }
                        }
                    }

                    #endregion
                }
            });

            if (string.IsNullOrEmpty(errMSg))
            {
                successMsg = Languages.GetResourceText("CRSavedSuccess");
            }
            return SuccessOrFailedResponse<int?>(errMSg, changeRequest.Id, successMsg);
        }
        public NPE.Core.ITypedResponse<int?> AddToAPQP(DTO.Library.APQP.ChangeRequest.ChangeRequest changeRequest)
        {
            string errMSg = null;
            string successMsg = null;

            if (Save(changeRequest).StatusCode == 200)
            {
                decimal TotalPurchasePieceCost = Math.Round(changeRequest.apqpPurchasePieceCost + changeRequest.PurchasePieceCost, 3);
                decimal TotalSellingPiecePrice = Math.Round(changeRequest.apqpSellingPiecePrice + changeRequest.SellingPiecePrice, 3);
                decimal TotalPurchaseToolingCost = Math.Round(changeRequest.apqpPurchaseToolingCost + changeRequest.PurchaseToolingCost, 3);
                decimal TotalSellingToolingPrice = Math.Round(changeRequest.apqpSellingToolingPrice + changeRequest.SellingToolingPrice, 3);

                ObjectParameter ChangeRequestId = new ObjectParameter("ChangeRequestId", changeRequest.Id);
                ObjectParameter ErrorKey = new ObjectParameter("ErrorKey", "");
                this.RunOnDB(context =>
                {
                    int result = context.crUpdateAPQPItem(changeRequest.Id
                                                        , changeRequest.APQPItemId
                                                        , TotalPurchasePieceCost, TotalPurchaseToolingCost, TotalSellingPiecePrice, TotalSellingToolingPrice
                                                        , changeRequest.DrawingRevDate
                                                        , changeRequest.RevLevel
                                                        , changeRequest.DrawingNumber
                                                        , CurrentUser
                                                        , ErrorKey);
                    if (!string.IsNullOrEmpty(Convert.ToString(ErrorKey.Value)))
                    {
                        errMSg = Languages.GetResourceText("AddToAPQPSaveFail");
                    }
                    else
                    {
                        DTO.Library.APQP.ChangeRequest.ChangeRequest crItem = FindById(changeRequest.Id).Result;
                        foreach (DTO.Library.APQP.ChangeRequest.Document crDocItem in crItem.lstDocument)
                        {
                            if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), crDocItem.FilePath))
                            {
                                crDocItem.FilePath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                       , Constants.APQPDocFolder
                                       , Path.GetFileName(crDocItem.FilePath)
                                       , crDocItem.FilePath);
                            }
                        }

                        successMsg = Languages.GetResourceText("AddToAPQPSuccess");
                    }
                });
            }

            return SuccessOrFailedResponse<int?>(errMSg, changeRequest.Id, successMsg);
        }
        public NPE.Core.ITypedResponse<DTO.Library.APQP.ChangeRequest.ChangeRequest> AddToCRFromAPQP(int apqpItemId)
        {
            string errMSg = string.Empty;
            DTO.Library.APQP.ChangeRequest.ChangeRequest changeRequest = new DTO.Library.APQP.ChangeRequest.ChangeRequest();

            if (GetChangeRequestStatusByAPQPItemId(apqpItemId)) //CR is not closed then open in edit mode
            {
                changeRequest = FindByAPQPId(apqpItemId).Result;
            }
            else // CR is closed then create new CR
            {
                int newCRId = ChangeRequestFirstTimeSave(apqpItemId);
                if (newCRId > 0)
                {
                    changeRequest = FindById(newCRId).Result;
                }
            }
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.APQP.ChangeRequest.ChangeRequest>(errMSg, changeRequest);
            return response;
        }
        public NPE.Core.ITypedResponse<DTO.Library.APQP.ChangeRequest.ChangeRequest> OnChangeOfPartNumber(int apqpItemId)
        {
            string errMSg = string.Empty;
            DTO.Library.APQP.ChangeRequest.ChangeRequest changeRequest = new DTO.Library.APQP.ChangeRequest.ChangeRequest();

            if (GetChangeRequestStatusByAPQPItemId(apqpItemId))
            {
                changeRequest = FindByAPQPId(apqpItemId).Result;
            }
            else
            {
                APQP.KickOff objKickOff = new APQP.KickOff();
                DTO.Library.APQP.APQP.KickOff kickOffItem = objKickOff.FindById(apqpItemId).Result;

                changeRequest.Subject = "Change Request for Part # - " + kickOffItem.PartNumber;
                changeRequest.APQPItemId = kickOffItem.Id;
                changeRequest.apqpPurchasePieceCost = kickOffItem.PurchasePieceCost;
                changeRequest.apqpPurchaseToolingCost = kickOffItem.PurchaseToolingCost;
                changeRequest.apqpSellingPiecePrice = kickOffItem.SellingPiecePrice;
                changeRequest.apqpSellingToolingPrice = kickOffItem.SellingToolingPrice;

            }
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.APQP.ChangeRequest.ChangeRequest>(errMSg, changeRequest);
            return response;
        }

        private bool GetChangeRequestStatusByAPQPItemId(int apqpItemId)
        {
            bool status = false;
            this.RunOnDB(context =>
           {
               var statuss = context.crGetChangeRequestStatusByItemId(apqpItemId).SingleOrDefault();
               status = statuss.HasValue ? Convert.ToBoolean(statuss.Value) : false;
           });

            return status;
        }

        public NPE.Core.ITypedResponse<DTO.Library.APQP.ChangeRequest.ChangeRequest> FindById(int id)
        {
            string errMSg = string.Empty;

            DTO.Library.APQP.ChangeRequest.ChangeRequest changeRequest = new DTO.Library.APQP.ChangeRequest.ChangeRequest();
            this.RunOnDB(context =>
            {
                var ChangeRequest = context.crGetChangeRequestById(id).SingleOrDefault();  // ????id (is it APQPItemid or changeRequestId) 
                if (ChangeRequest == null)
                    errMSg = Languages.GetResourceText("ChangeRequestNotExists");
                else
                {
                    #region general details
                    changeRequest = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.ChangeRequest.ChangeRequest>(ChangeRequest);
                    #endregion
                    #region Bind Watchers
                    changeRequest.WatcherItems = new List<DTO.Library.Common.LookupFields>();
                    context.crChangeRequestWatchers.Where(a => a.ChangeRequestId == id).ToList().ForEach(tl => changeRequest.WatcherItems.Add(
                        new DTO.Library.Common.LookupFields()
                        {
                            Id = Convert.ToString(tl.WatcherId),
                            Name = tl.AspNetUser.FirstName + " " + tl.AspNetUser.LastName
                        }));
                    #endregion
                    #region Bind document details
                    //MES.Business.Repositories.APQP.ChangeRequest.IDocumentRepository objIDocumentRepository = new MES.Business.Library.BO.APQP.ChangeRequest.Document();
                    //changeRequest.lstDocument = objIDocumentRepository.GetDocumentList(id); // this should be changeRequestId
                    changeRequest.lstDocument = new List<DTO.Library.APQP.ChangeRequest.Document>();
                    #endregion
                }
            });
            LoginUser currentUser = GetCurrentUser;
            var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.ChangeRequest)).ToList();
            if (currentObjects.Count > 0)
                changeRequest.HasPricingFieldsAccess = currentObjects[0].HasPricingFieldsAccess;

            currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQPDashboard)).ToList();
            if (currentObjects.Count > 0)
                changeRequest.AllowConfidentialDocumentType = currentObjects[0].AllowConfidentialDocumentType;

            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.APQP.ChangeRequest.ChangeRequest>(errMSg, changeRequest);
            return response;
        }

        public NPE.Core.ITypedResponse<DTO.Library.APQP.ChangeRequest.ChangeRequest> FindByAPQPId(int apqpItemId)
        {
            string errMSg = string.Empty;
            DTO.Library.APQP.ChangeRequest.ChangeRequest changeRequest = new DTO.Library.APQP.ChangeRequest.ChangeRequest();
            this.RunOnDB(context =>
            {
                var ChangeRequest = context.crGetChangeRequestByItemId(apqpItemId).SingleOrDefault();
                if (ChangeRequest == null)
                    errMSg = Languages.GetResourceText("ChangeRequestNotExists");
                else
                {
                    #region general details
                    changeRequest = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.ChangeRequest.ChangeRequest>(ChangeRequest);
                    #endregion
                    LoginUser currentUser = GetCurrentUser;
                    var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.ChangeRequest)).ToList();
                    if (currentObjects.Count > 0)
                        changeRequest.HasPricingFieldsAccess = currentObjects[0].HasPricingFieldsAccess;

                    currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQPDashboard)).ToList();
                    if (currentObjects.Count > 0)
                        changeRequest.AllowConfidentialDocumentType = currentObjects[0].AllowConfidentialDocumentType;

                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.APQP.ChangeRequest.ChangeRequest>(errMSg, changeRequest);
            return response;
        }

        private int ChangeRequestFirstTimeSave(int apqpItemId)
        {
            DTO.Library.APQP.ChangeRequest.ChangeRequest changeRequest = new DTO.Library.APQP.ChangeRequest.ChangeRequest();
            APQP.KickOff objKickOff = new APQP.KickOff();
            DTO.Library.APQP.APQP.KickOff kickOffItem = objKickOff.FindById(apqpItemId).Result;

            changeRequest.APQPItemId = apqpItemId;
            changeRequest.AssignedToId = GetCurrentUser.UserId;
            changeRequest.StatusId = Convert.ToInt32(StatusFixedId.ChangeRequestInReview);
            changeRequest.Subject = "Change Request for Part # - " + kickOffItem.PartNumber;

            changeRequest.apqpPurchasePieceCost = kickOffItem.PurchasePieceCost;
            changeRequest.apqpPurchaseToolingCost = kickOffItem.PurchaseToolingCost;
            changeRequest.apqpSellingPiecePrice = kickOffItem.SellingPiecePrice;
            changeRequest.apqpSellingToolingPrice = kickOffItem.SellingToolingPrice;
            changeRequest.PurchasePieceCost = 0;
            changeRequest.PurchaseToolingCost = 0;
            changeRequest.SellingPiecePrice = 0;
            changeRequest.SellingToolingPrice = 0;

            changeRequest.CreatedBy = CurrentUser;
            changeRequest.Id = Save(changeRequest).Result.Value;
            return changeRequest.Id;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int changeRequestId)
        {
            try
            {
                var apqpItemMasterToBeDeleted = this.DataContext.crItemMasters.Where(a => a.Id == changeRequestId).SingleOrDefault();
                if (apqpItemMasterToBeDeleted == null)
                {
                    return FailedBoolResponse(Languages.GetResourceText("CRNotExists"));
                }
                else
                {
                    apqpItemMasterToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    apqpItemMasterToBeDeleted.UpdatedBy = CurrentUser;
                    this.DataContext.Entry(apqpItemMasterToBeDeleted).State = EntityState.Modified;
                    apqpItemMasterToBeDeleted.IsDeleted = true;
                    this.DataContext.SaveChanges();
                    return SuccessBoolResponse(Languages.GetResourceText("CRDeletedSuccess"));
                }
            }
            catch (Exception ex)
            {
                return FailedBoolResponse(Languages.GetResourceText("CRDeleteFailed"));
            }            
       
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.ChangeRequest.ChangeRequest>> GetChangeRequestList(NPE.Core.IPage<DTO.Library.APQP.ChangeRequest.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            LoginUser currentUser = GetCurrentUser;
            var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQP)).ToList();

            List<DTO.Library.APQP.ChangeRequest.ChangeRequest> lstChangeRequest = new List<DTO.Library.APQP.ChangeRequest.ChangeRequest>();
            DTO.Library.APQP.ChangeRequest.ChangeRequest changeRequest;
            this.RunOnDB(context =>
             {
                 var ChangeRequestList = context.crGetChangeRequests(paging.Criteria.ChangeRequestId, paging.Criteria.PartNumber, paging.Criteria.StatusIds, paging.Criteria.AssignedToIds,
                     paging.Criteria.Updated, paging.Criteria.RevLevel, paging.PageNo, paging.PageSize, totalRecords, "").ToList();

                 if (ChangeRequestList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in ChangeRequestList)
                     {
                         changeRequest = new DTO.Library.APQP.ChangeRequest.ChangeRequest();
                         changeRequest.Id = item.Id;
                         changeRequest = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.ChangeRequest.ChangeRequest>(item);
                         changeRequest.AllowDeleteRecord = currentObjects[0].AllowDeleteRecord;
                         lstChangeRequest.Add(changeRequest);
                     }
                 }
             });


            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.ChangeRequest.ChangeRequest>>(errMSg, lstChangeRequest);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.SAPItem>> SearchFromSAPRecords(NPE.Core.IPage<DTO.Library.APQP.ChangeRequest.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.APQP.APQP.SAPItem> lstSAPItem = new List<DTO.Library.APQP.APQP.SAPItem>();
            DTO.Library.APQP.APQP.SAPItem sAPItem;
            this.RunOnDB(context =>
            {
                var SAPItemList = context.GetSAPItem(paging.Criteria.PartNumber, paging.Criteria.RFQNumber, paging.Criteria.QuoteNumber, paging.Criteria.CustomerName).ToList();
                if (SAPItemList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var item in SAPItemList)
                    {
                        sAPItem = new DTO.Library.APQP.APQP.SAPItem();
                        sAPItem.ItemCode = item.ItemCode;
                        sAPItem.RevLevel = item.RevLevel;
                        sAPItem.RFQNumber = item.RFQNumber;
                        sAPItem.RFQDate = string.IsNullOrEmpty(item.RFQDate) ? "" : string.Format("{0:dd-MMM-yy}", Convert.ToDateTime(item.RFQDate));
                        sAPItem.QuoteNumber = item.QuoteNumber;
                        sAPItem.CustomerName = item.CustomerName;
                        sAPItem.SupplierName = item.SupplierName;
                        sAPItem.APQPSAPItemId = item.Id;
                        lstSAPItem.Add(sAPItem);
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.APQP.SAPItem>>(errMSg, lstSAPItem);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<int?> InsertFromSAPRecords(string ItemIds)
        {
            string errMSg = null;
            string successMsg = null;
            int? ChangeRequestId = 0;
            ObjectParameter outputParam = new ObjectParameter("ChangeRequestId", 0);

            string assignedDesignation = Convert.ToInt32(MES.Business.Library.Enums.DesignationFixedId.APQPQualityEngineer).ToString() + "," +
                Convert.ToInt32(MES.Business.Library.Enums.DesignationFixedId.QualityManager).ToString() + "," +
                Convert.ToInt32(MES.Business.Library.Enums.DesignationFixedId.SupplierQualityEngineer).ToString();

            this.RunOnDB(context =>
            {
                int result = context.AddToCRFromSAPItem(Convert.ToInt32(ItemIds), Convert.ToInt32(MES.Business.Library.Enums.StatusFixedId.ChangeRequestInReview), assignedDesignation, CurrentUser, outputParam);
                ChangeRequestId = Convert.ToInt32(outputParam.Value);

                if (ChangeRequestId > 0)
                {
                    successMsg = Languages.GetResourceText("SAPItemSavedSuccess");
                }
                else
                    errMSg = Languages.GetResourceText("SAPItemSavedFail");
            });
            return SuccessOrFailedResponse<int?>(errMSg, ChangeRequestId, successMsg);
        }

        public NPE.Core.ITypedResponse<int?> GetFromSAPAndInsertInLocalSAPTable()
        {
            string errMSg = null;
            string successMsg = null;
            int? ReturnVal = 0;
            try
            {
                this.RunOnDB(context =>
                {
                    string startDate = DateTime.Now.ToString("yyyyMMddHHmm"), endDate = DateTime.Now.ToString("yyyyMMddHHmm");
                    var sapObject = context.SAPItemFlatTables.Max(a => a.CreatedDate);
                    if (sapObject != null)
                        startDate = Convert.ToDateTime(sapObject).ToString("yyyyMMddHHmm");

                    if (startDate != endDate)
                    {
                        string requestUrl = System.Configuration.ConfigurationManager.AppSettings["SAPAPIURL_GET"] + startDate + "/" + endDate;

                        HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                        request.KeepAlive = false;
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(response.GetResponseStream());
                        response.Close();
                        XmlReader xmlReader = new XmlNodeReader(xmlDoc);
                        System.Data.DataSet ds = new System.Data.DataSet();
                        ds.ReadXml(xmlReader);
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            MES.Data.Library.SAPItemFlatTable recordToBeUpdated;
                            bool isAnySaved = false;
                            foreach (System.Data.DataRow drItem in ds.Tables[0].Rows)
                            {
                                recordToBeUpdated = new Data.Library.SAPItemFlatTable();
                                recordToBeUpdated.CreatedDate = AuditUtils.GetCurrentDateTime();
                                this.DataContext.SAPItemFlatTables.Add(recordToBeUpdated);
                                recordToBeUpdated.Cost = drItem["Cost"].ToString().Trim();
                                recordToBeUpdated.CustomerName = drItem["Customer"].ToString().Trim();
                                recordToBeUpdated.CustomerCode = drItem["CustomerCode"].ToString().Trim();
                                recordToBeUpdated.ItemCode = drItem["ItemCode"].ToString().Trim();
                                recordToBeUpdated.ItemName = drItem["ItemName"].ToString().Trim();
                                recordToBeUpdated.ItemWeight = drItem["ItemWeight"].ToString().Trim();
                                recordToBeUpdated.MaterialType = drItem["MaterialType"].ToString().Trim();
                                recordToBeUpdated.ProjectName = drItem["ProjectName"].ToString().Trim();
                                recordToBeUpdated.RevLevel = drItem["Revision"].ToString().Trim();
                                recordToBeUpdated.SalesPrice = drItem["Sales_Price"].ToString().Trim();
                                recordToBeUpdated.SupplierName = drItem["Supplier"].ToString().Trim();
                                recordToBeUpdated.SupplierCode = drItem["SupplierCode"].ToString().Trim();
                                isAnySaved = true;
                            }
                            if (isAnySaved)
                                this.DataContext.SaveChanges();
                        }
                    }
                    ReturnVal = 1;
                });
            }
            catch (Exception ex)
            {
                return SuccessOrFailedResponse<int?>(Convert.ToString(ex.Message), ReturnVal, successMsg);
            }
            return SuccessOrFailedResponse<int?>(errMSg, ReturnVal, successMsg);
        }

        public NPE.Core.ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> crHistoryChangeLog(int crItemId)
        {
            string errMSg = null;
            bool hasPricingFieldsAccess = true, allowConfidentialDocumentType = true;
            LoginUser currentUser = GetCurrentUser;
            var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.ChangeRequest)).ToList();
            if (currentObjects.Count > 0)
                hasPricingFieldsAccess = currentObjects[0].HasPricingFieldsAccess;

            currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQPDashboard)).ToList();
            if (currentObjects.Count > 0)
                allowConfidentialDocumentType = currentObjects[0].AllowConfidentialDocumentType;
            bool isFirst = true;
            List<DTO.Library.AuditLogs.AuditLogs> lstAuditLogs = new List<DTO.Library.AuditLogs.AuditLogs>();
            DTO.Library.AuditLogs.AuditLogs auditLogs;
            this.RunOnDB(context =>
            {
                var crItemList = context.crGetChangeLogByItemId(crItemId).ToList();
                if (crItemList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    var groups = crItemList.GroupBy(item => item.TimeStamp);
                    foreach (var group in groups)
                    {
                        isFirst = true;
                        foreach (var item in group)
                        {
                            auditLogs = new DTO.Library.AuditLogs.AuditLogs();
                            auditLogs.AuditLogId = item.AuditLogId;
                            auditLogs.ReferenceId = item.ReferenceId;
                            auditLogs.TableName = item.TableName;
                            auditLogs.Source = item.Source;
                            auditLogs.Description = item.Description;
                            auditLogs.UpdatedOn = Convert.ToDateTime(item.UpdatedOn);
                            auditLogs.UpdatedOnString = item.UpdatedOn;
                            auditLogs.TimeStamp = item.TimeStamp;
                            auditLogs.TimeStampString = item.TimeStamp.Value.FormatDateInMediumDateWithTime();
                            auditLogs.UpdatedBy = item.UpdatedBy;
                            auditLogs.DetailId = item.DetailId;
                            auditLogs.TableName = item.TableName;
                            auditLogs.FieldName = item.FieldName;
                            auditLogs.FieldAlias = item.FieldAlias;
                            auditLogs.IsPricingField = item.IsPricingField;
                            auditLogs.OperationType = item.OperationType;
                            auditLogs.OldValue = item.OldValue;
                            auditLogs.NewValue = item.NewValue;

                            if (isFirst)
                            {
                                List<DTO.Library.APQP.ChangeRequest.Document> lstDocument = new List<DTO.Library.APQP.ChangeRequest.Document>();
                                DTO.Library.APQP.ChangeRequest.Document document;

                                var crItemDocList = context.crGetDocumentByLogId(group.First().AuditLogId.ToString()).ToList();
                                if (crItemDocList != null)
                                {
                                    foreach (var dItem in crItemDocList)
                                    {
                                        if (!allowConfidentialDocumentType && dItem.IsConfidential)
                                        {
                                            continue;
                                        }
                                        document = new DTO.Library.APQP.ChangeRequest.Document();
                                        document.Id = dItem.Id;
                                        document.ChangeRequestId = dItem.ChangeRequestId;
                                        document.DocumentTypeId = dItem.DocumentTypeId;
                                        document.DocumentType = dItem.DocumentType;
                                        document.IsConfidential = dItem.IsConfidential;
                                        document.Comments = dItem.Comments;
                                        document.FileTitle = dItem.FileTitle;
                                        document.FilePath = !string.IsNullOrEmpty(dItem.FilePath) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) +
                                                Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + dItem.FilePath : string.Empty;
                                        lstDocument.Add(document);
                                    }
                                }
                                isFirst = false;
                                auditLogs.lstDocument = lstDocument;
                                auditLogs.IsContainsDocument = lstDocument.Count > 0 ? true : false;
                            }
                            lstAuditLogs.Add(auditLogs);
                        }


                    }
                }
            });
            if (!hasPricingFieldsAccess)
                lstAuditLogs = lstAuditLogs.Where(item => item.IsPricingField == false).ToList();
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>>(errMSg, lstAuditLogs);
            //populate page property
            return response;
        }

        private void SendEmailToWatchers(int crItemId)
        {
            List<MES.DTO.Library.AuditLogs.AuditLogs> CRItem = crHistoryChangeLog(crItemId).Result;
            CRItem = CRItem.FindAll(rec => rec.FieldName != "UpdatedDate");

            if (CRItem.Count > 0)
            {
                int AuditLogId = CRItem.Max(rec => rec.AuditLogId);

                string StrHTML = "";
                foreach (var item in CRItem.FindAll(rec => rec.AuditLogId == AuditLogId))
                {
                    if (!string.IsNullOrEmpty(item.OldValue))
                        StrHTML += @"<tr><td valign='top' style='font-family: Tahoma, Geneva, sans-serif;font-size: 11px;'><ul><li><i style='font-weight: normal'>Changed&nbsp;</i><label style='font-weight: bold;text-transform: uppercase!important'>"
                            + item.FieldName + "</label>&nbsp;<i style='font-weight: normal'>from&nbsp;&nbsp;</i><label style='font-weight: bold; text-transform: uppercase!important'>"
                            + item.OldValue + " &nbsp;</label><i style='font-weight: normal'>To</i>&nbsp;"
                            + item.NewValue + "</li></ul></td></tr>";
                    else
                        StrHTML += @"<tr><td valign='top'  style='font-family: Tahoma, Geneva, sans-serif;font-size: 11px;'><ul><li><i style='font-weight: normal'>Added&nbsp;</i><label style='font-weight: bold;text-transform: uppercase!important'>"
                                    + item.FieldName + ":&nbsp;" + item.NewValue + "</li></ul></td></tr>";

                }

                bool IsSuccess = false;
                try
                {
                    MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
                    MES.DTO.Library.Common.EmailData emailData = new DTO.Library.Common.EmailData();
                    UserManagement.UserManagement userObj = new UserManagement.UserManagement();

                    DTO.Library.APQP.ChangeRequest.ChangeRequest ChangeRequestObject = FindById(crItemId).Result;
                    List<string> lstToAddress = new List<string>();
                    bool isNew = true;
                    emailData.EmailBody = GetRfqEmailBody("CRChangeIntimation.htm")
                                          .Replace("<%FirstLineText%>", Languages.GetResourceText("rqStatusCompleteEmailBodyFirstLine"))
                                          .Replace("<%CreatedOrUpdatedText%>", isNew ? "Created by" : "Updated by")
                            .Replace("<%Subject%>", ChangeRequestObject.Subject)
                            .Replace("<%CreatedBy%>", isNew ? ChangeRequestObject.CreatedByUserName : ChangeRequestObject.UpdatedByUserName)
                            .Replace("<%CreatedDate%>",
                            isNew ? (ChangeRequestObject.CreatedDate.HasValue ? Convert.ToDateTime(ChangeRequestObject.CreatedDate.Value).ToString("dd-MMM-yy") : string.Empty)
                            : (ChangeRequestObject.UpdatedDate.HasValue ? Convert.ToDateTime(ChangeRequestObject.UpdatedDate).ToString("dd-MMM-yy") : string.Empty))
                            .Replace("<%crId%>", crItemId.ToString());
                    emailData.EmailBody = emailData.EmailBody.Replace("<%changeLog%>", StrHTML);

                    emailData.EmailSubject = "Change Request # " + crItemId + " - " + ChangeRequestObject.Subject;
                    foreach (var watcherItem in ChangeRequestObject.WatcherItems)
                    {
                        lstToAddress.Add(userObj.GetUserInfoById(watcherItem.Id.ToString()).Email);
                    }

                    emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailData.EmailBody, out IsSuccess, null);
                }
                catch (Exception ex)
                {
                }

            }
        }
        /// <summary>
        /// Gets the RFQ email body.
        /// </summary>
        /// <returns></returns>
        private string GetRfqEmailBody(string fileName)
        {
            string templatePath = System.Web.HttpContext.Current.Server.MapPath("~/") + Constants.EmailTemplateFolder + fileName;
            string emailBody = string.Empty;
            using (StreamReader reader = new StreamReader(templatePath))
            {
                emailBody = reader.ReadToEnd();
                reader.Close();
            }
            return emailBody;

        }

        public NPE.Core.ITypedResponse<bool?> DeleteDocument(int Id)
        {
            //set the out put param
            ObjectParameter ErrorKey = new ObjectParameter("ErrorKey", "");
            try
            {
                var crDocToBeDeleted = this.DataContext.crDocuments.Where(item => item.Id == Id).SingleOrDefault();
                if (crDocToBeDeleted == null)
                {
                    return FailedBoolResponse(Languages.GetResourceText("APQPDocumentNotExists"));
                }
                else
                {
                    bool isSuccess = true;
                    if (!string.IsNullOrEmpty(crDocToBeDeleted.FilePath))
                        isSuccess = Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                          , Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) +
                                        Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + crDocToBeDeleted.FilePath);

                    if (isSuccess)
                    {
                        this.DataContext.Entry(crDocToBeDeleted).State = EntityState.Deleted;
                        this.DataContext.SaveChanges();

                        return SuccessBoolResponse(Languages.GetResourceText("APQPDocumentDeletedSuccess"));
                    }
                    else
                        return FailedBoolResponse(Languages.GetResourceText("APQPDocumentFailedSuccess"));
                }
            }
            catch (Exception ex) { return FailedBoolResponse(ex.ToString()); }
        }

        private bool DeleteFiles(int crId)
        {
            bool isSuccess = false;
            try
            {
                IChangeRequestRepository crRep = new ChangeRequest();
                DTO.Library.APQP.ChangeRequest.ChangeRequest crItem = crRep.FindById(crId).Result;
                foreach (MES.DTO.Library.APQP.ChangeRequest.Document item in crItem.lstDocument)
                {
                    isSuccess = Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                       , item.FilePath);
                }
            }
            catch (Exception ex)
            { throw ex; }
            return isSuccess;
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.ChangeRequest.ChangeRequest>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

    }
}
