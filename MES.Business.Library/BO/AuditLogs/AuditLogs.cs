using Account.DTO.Library;
using MES.Business.Repositories.AuditLogs;
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
using MES.Business.Library.Extensions;
using MES.DTO.Library.Identity;
using MES.Business.Library.Enums;

namespace MES.Business.Library.BO.AuditLogs
{
    class AuditLogs : ContextBusinessBase, IAuditLogsRepository
    {
        public AuditLogs()
            : base("AuditLogs")
        {

        }
        #region common get logs function

        public NPE.Core.ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAuditLogDetails(NPE.Core.IPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.AuditLogs.AuditLogs> lstAuditLogs = new List<DTO.Library.AuditLogs.AuditLogs>();
            DTO.Library.AuditLogs.AuditLogs auditLogs;
            this.RunOnDB(context =>
            {
                var AuditLogsList = context.GetLogDetails(paging.Criteria.ItemId, paging.Criteria.tableName, paging.Criteria.schemaName, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (AuditLogsList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in AuditLogsList)
                    {
                        auditLogs = new DTO.Library.AuditLogs.AuditLogs();
                        auditLogs.AuditLogId = item.AuditLogId;
                        auditLogs.ReferenceId = item.ReferenceId;
                        auditLogs.TablePrimaryKey = item.TablePrimaryKey;
                        auditLogs.Source = item.Source;
                        auditLogs.Description = item.Description;
                        auditLogs.UpdatedOn = item.UpdatedOn;
                        auditLogs.UpdatedOnString = item.UpdatedOn.Value.FormatDateInMediumDate();
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
                        lstAuditLogs.Add(auditLogs);
                    }
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>>(errMSg, lstAuditLogs);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

        #endregion

        #region get log for defect tracking

        public NPE.Core.ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAuditLogDefectTracking(NPE.Core.IPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.AuditLogs.AuditLogs> lstAuditLogs = new List<DTO.Library.AuditLogs.AuditLogs>();
            DTO.Library.AuditLogs.AuditLogs auditLogs;
            this.RunOnDB(context =>
            {
                var AuditLogsList = context.dtGetDefectTrackingChangeLogByItemId(paging.Criteria.ItemId, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (AuditLogsList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in AuditLogsList)
                    {
                        auditLogs = new DTO.Library.AuditLogs.AuditLogs();
                        auditLogs.AuditLogId = item.AuditLogId;
                        auditLogs.ReferenceId = item.ReferenceId;
                        auditLogs.Source = item.Source;
                        auditLogs.Description = item.Description;
                        auditLogs.UpdatedOn = item.UpdateOn;
                        auditLogs.UpdatedOnString = item.UpdateOn.Value.FormatDateInMediumDate();
                        auditLogs.TimeStamp = item.TimeStamp;
                        auditLogs.TimeStampString = item.TimeStamp.Value.FormatDateInMediumDateWithTime();
                        auditLogs.UpdatedBy = item.UserBy;
                        auditLogs.DetailId = item.DetailId;
                        auditLogs.TableName = item.TableName;
                        auditLogs.FieldName = item.FieldName;
                        auditLogs.FieldAlias = item.FieldAlias;
                        auditLogs.IsPricingField = item.IsPricingField;
                        auditLogs.OperationType = item.OperationType;
                        auditLogs.OldValue = item.OldValue;
                        auditLogs.NewValue = item.NewValue;
                        auditLogs.SourceFrom = item.SourceFrom;
                        lstAuditLogs.Add(auditLogs);
                    }
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>>(errMSg, lstAuditLogs);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAuditLogDefectTrackingDetails(NPE.Core.IPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.AuditLogs.AuditLogs> lstAuditLogs = new List<DTO.Library.AuditLogs.AuditLogs>();
            DTO.Library.AuditLogs.AuditLogs auditLogs;
            this.RunOnDB(context =>
            {
                var AuditLogsList = context.dtGetDefectTrackingDetailLogByDTId(paging.Criteria.ItemId, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (AuditLogsList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in AuditLogsList)
                    {
                        auditLogs = new DTO.Library.AuditLogs.AuditLogs();
                        auditLogs.AuditLogId = item.AuditLogId;
                        auditLogs.ReferenceId = item.ReferenceId;
                        auditLogs.Source = item.Source;
                        auditLogs.Description = item.Description;
                        auditLogs.UpdatedOn = item.UpdateOn;
                        auditLogs.UpdatedOnString = item.UpdateOn.Value.FormatDateInMediumDate();
                        auditLogs.TimeStamp = item.TimeStamp;
                        auditLogs.TimeStampString = item.TimeStamp.Value.FormatDateInMediumDateWithTime();
                        auditLogs.UpdatedBy = item.UserBy;
                        auditLogs.DetailId = item.DetailId;
                        auditLogs.TableName = item.TableName;
                        auditLogs.FieldName = item.FieldName;
                        auditLogs.FieldAlias = item.FieldAlias;
                        auditLogs.IsPricingField = item.IsPricingField;
                        auditLogs.OperationType = item.OperationType;
                        auditLogs.OldValue = item.OldValue;
                        auditLogs.NewValue = item.NewValue;
                        auditLogs.SourceFrom = item.SourceFrom;
                        auditLogs.PartNumber = item.PartNumber;
                        lstAuditLogs.Add(auditLogs);
                    }
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>>(errMSg, lstAuditLogs);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

        #endregion

        #region get APQP change log
        public NPE.Core.ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAPQPChangeLogs(NPE.Core.IPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            bool hasPricingFieldsAccess = true;
            LoginUser currentUser = GetCurrentUser;
            var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQP)).ToList();
            if (currentObjects.Count > 0)
                hasPricingFieldsAccess = currentObjects[0].HasPricingFieldsAccess;

            List<DTO.Library.AuditLogs.AuditLogs> lstAuditLogs = new List<DTO.Library.AuditLogs.AuditLogs>();
            DTO.Library.AuditLogs.AuditLogs auditLogs;
            this.RunOnDB(context =>
            {
                var AuditLogsList = context.apqpGetChangeLogByItemId(paging.Criteria.ItemId, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (AuditLogsList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in AuditLogsList)
                    {
                        auditLogs = new DTO.Library.AuditLogs.AuditLogs();
                        auditLogs.AuditLogId = item.AuditLogId;
                        auditLogs.ReferenceId = item.ReferenceId;
                        auditLogs.Source = item.Source;
                        auditLogs.Description = item.Description;
                        auditLogs.UpdatedOn = item.UpdateOn;
                        auditLogs.UpdatedOnString = item.UpdateOn.Value.FormatDateInMediumDate();
                        auditLogs.TimeStamp = item.TimeStamp;
                        auditLogs.TimeStampString = item.TimeStamp.Value.FormatDateInMediumDateWithTime();
                        auditLogs.UpdatedBy = item.UserBy;
                        auditLogs.DetailId = item.DetailId;
                        auditLogs.TableName = item.TableName;
                        auditLogs.FieldName = item.FieldName;
                        auditLogs.FieldAlias = item.FieldAlias;
                        auditLogs.IsPricingField = item.IsPricingField;
                        auditLogs.OperationType = item.OperationType;
                        auditLogs.OldValue = item.OldValue;
                        auditLogs.NewValue = item.NewValue;
                        auditLogs.SourceFrom = item.SourceFrom;
                        lstAuditLogs.Add(auditLogs);
                    }
                }
            });
            if (!hasPricingFieldsAccess)
                lstAuditLogs = lstAuditLogs.Where(item => item.IsPricingField == false).ToList();

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>>(errMSg, lstAuditLogs);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetChangeRequestLogs(NPE.Core.IPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging)
        {
            bool isFirst = true;
            string errMSg = null;
            bool hasPricingFieldsAccess = true, allowConfidentialDocumentType = true;
            LoginUser currentUser = GetCurrentUser;
            var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.ChangeRequest)).ToList();
            if (currentObjects.Count > 0)
                hasPricingFieldsAccess = currentObjects[0].HasPricingFieldsAccess;

            currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQPDashboard)).ToList();
            if (currentObjects.Count > 0)
                allowConfidentialDocumentType = currentObjects[0].AllowConfidentialDocumentType;

            List<DTO.Library.AuditLogs.AuditLogs> lstAuditLogs = new List<DTO.Library.AuditLogs.AuditLogs>();
            DTO.Library.AuditLogs.AuditLogs auditLogs;
            this.RunOnDB(context =>
            {
                var AuditLogsList = context.crGetChangeLogByAPQPItemId(paging.Criteria.ItemId).ToList();
                if (AuditLogsList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    var groups = AuditLogsList.GroupBy(item => item.TimeStamp);
                    foreach (var group in groups)
                    {
                        isFirst = true;
                        foreach (var item in group)
                        {
                            auditLogs = new DTO.Library.AuditLogs.AuditLogs();
                            auditLogs.AuditLogId = item.AuditLogId;
                            auditLogs.ReferenceId = item.ReferenceId;
                            auditLogs.Source = item.Source;
                            auditLogs.Description = item.Description;
                            auditLogs.UpdatedOn = item.UpdateOn;
                            auditLogs.UpdatedOnString = item.UpdateOn.Value.FormatDateInMediumDate();
                            auditLogs.TimeStamp = item.TimeStamp;
                            auditLogs.TimeStampString = item.TimeStamp.Value.FormatDateInMediumDateWithTime();
                            auditLogs.UpdatedBy = item.UserBy;
                            auditLogs.DetailId = item.DetailId;
                            auditLogs.TableName = item.TableName;
                            auditLogs.FieldName = item.FieldName;
                            auditLogs.FieldAlias = item.FieldAlias;
                            auditLogs.IsPricingField = item.IsPricingField;
                            auditLogs.OperationType = item.OperationType;
                            auditLogs.OldValue = item.OldValue;
                            auditLogs.NewValue = item.NewValue;
                            auditLogs.SourceFrom = item.SourceFrom;

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
            return response;
        }

        #endregion

        #region get CAPA change log
        public NPE.Core.ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAuditLogCAPA(NPE.Core.IPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.AuditLogs.AuditLogs> lstAuditLogs = new List<DTO.Library.AuditLogs.AuditLogs>();
            DTO.Library.AuditLogs.AuditLogs auditLogs;
            this.RunOnDB(context =>
            {
                var AuditLogsList = context.capaGetcapaItemMasterChangeLogCAPAId(paging.Criteria.ItemId, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (AuditLogsList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in AuditLogsList)
                    {
                        auditLogs = new DTO.Library.AuditLogs.AuditLogs();
                        auditLogs.AuditLogId = item.AuditLogId;
                        auditLogs.ReferenceId = item.ReferenceId;
                        auditLogs.Source = item.Source;
                        auditLogs.Description = item.Description;
                        auditLogs.UpdatedOn = item.UpdateOn;
                        auditLogs.UpdatedOnString = item.UpdateOn.Value.FormatDateInMediumDate();
                        auditLogs.TimeStamp = item.TimeStamp;
                        auditLogs.TimeStampString = item.TimeStamp.Value.FormatDateInMediumDateWithTime();
                        auditLogs.UpdatedBy = item.UserBy;
                        auditLogs.DetailId = item.DetailId;
                        auditLogs.TableName = item.TableName;
                        auditLogs.FieldName = item.FieldName;
                        auditLogs.FieldAlias = item.FieldAlias;
                        auditLogs.IsPricingField = item.IsPricingField;
                        auditLogs.OperationType = item.OperationType;
                        auditLogs.OldValue = item.OldValue;
                        auditLogs.NewValue = item.NewValue;
                        auditLogs.SourceFrom = item.SourceFrom;
                        lstAuditLogs.Add(auditLogs);
                    }
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>>(errMSg, lstAuditLogs);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAuditLogCAPAAffectedPartDetails(NPE.Core.IPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.AuditLogs.AuditLogs> lstAuditLogs = new List<DTO.Library.AuditLogs.AuditLogs>();
            DTO.Library.AuditLogs.AuditLogs auditLogs;
            this.RunOnDB(context =>
            {
                var AuditLogsList = context.capaGetcapaPartAffectedDetailsChangeLogByCAPAId(paging.Criteria.ItemId, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (AuditLogsList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in AuditLogsList)
                    {
                        auditLogs = new DTO.Library.AuditLogs.AuditLogs();
                        auditLogs.AuditLogId = item.AuditLogId;
                        auditLogs.ReferenceId = item.ReferenceId;
                        auditLogs.Source = item.Source;
                        auditLogs.Description = item.Description;
                        auditLogs.UpdatedOn = item.UpdateOn;
                        auditLogs.UpdatedOnString = item.UpdateOn.Value.FormatDateInMediumDate();
                        auditLogs.TimeStamp = item.TimeStamp;
                        auditLogs.TimeStampString = item.TimeStamp.Value.FormatDateInMediumDateWithTime();
                        auditLogs.UpdatedBy = item.UserBy;
                        auditLogs.DetailId = item.DetailId;
                        auditLogs.TableName = item.TableName;
                        auditLogs.FieldName = item.FieldName;
                        auditLogs.FieldAlias = item.FieldAlias;
                        auditLogs.IsPricingField = item.IsPricingField;
                        auditLogs.OperationType = item.OperationType;
                        auditLogs.OldValue = item.OldValue;
                        auditLogs.NewValue = item.NewValue;
                        auditLogs.SourceFrom = item.SourceFrom;
                        auditLogs.PartNumber = item.PartNumber;
                        lstAuditLogs.Add(auditLogs);
                    }
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>>(errMSg, lstAuditLogs);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }
        #endregion

    }
}
