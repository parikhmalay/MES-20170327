using Account.DTO.Library;
using MES.Business.Repositories.APQP.DocumentManagement;
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
using EvoPdf.HtmlToPdf;
using System.IO;
using System.Data;
using MES.Business.Library.Extensions;
using Ionic.Zip;
using System.Web;
using MES.DTO.Library.Identity;

namespace MES.Business.Library.BO.APQP.DocumentManagement
{
    public class DocumentManagement : ContextBusinessBase, IDocumentManagementRepository
    {
        public DocumentManagement()
            : base("DocumentManagement")
        { }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.APQP.DocumentManagement.DocumentManagement documentManagement)
        {
            throw new NotImplementedException();
        }
        public NPE.Core.ITypedResponse<DTO.Library.APQP.DocumentManagement.DocumentManagement> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int documentManagementId)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.DocumentManagement.DocumentManagement>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.DocumentManagement.DocumentManagement>> GetDocumentManagementList(NPE.Core.IPage<DTO.Library.APQP.DocumentManagement.SearchCriteria> searchCriteria)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            bool allowConfidentialDocumentType = true;
            LoginUser currentUser = GetCurrentUser;
            var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQPDashboard)).ToList();
            if (currentObjects.Count > 0)
                allowConfidentialDocumentType = currentObjects[0].AllowConfidentialDocumentType;

            var httpContext = System.Web.HttpContext.Current;
            List<DTO.Library.APQP.DocumentManagement.DocumentManagement> lstDocumentManagement = new List<DTO.Library.APQP.DocumentManagement.DocumentManagement>();
            DTO.Library.APQP.DocumentManagement.DocumentManagement documentManagement;
            this.RunOnDB(context =>
             {
                 var DocumentManagementList = context.GetDocumentManagements(searchCriteria.Criteria.RFQNumber, searchCriteria.Criteria.QuoteNumber, searchCriteria.Criteria.CustomerName, searchCriteria.Criteria.PartNo,
                      searchCriteria.Criteria.PartDescription, searchCriteria.Criteria.APQPStatusIds, searchCriteria.Criteria.SAMUserId, searchCriteria.Criteria.APQPQualityEngineerId,
                      searchCriteria.Criteria.SupplyChainCoordinatorId, allowConfidentialDocumentType, searchCriteria.PageNo, searchCriteria.PageSize, totalRecords, "").ToList();

                 if (DocumentManagementList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     searchCriteria.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in DocumentManagementList)
                     {
                         documentManagement = new DTO.Library.APQP.DocumentManagement.DocumentManagement();
                         documentManagement = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.DocumentManagement.DocumentManagement>(item);
                         lstDocumentManagement.Add(documentManagement);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.DocumentManagement.DocumentManagement>>(errMSg, lstDocumentManagement);
            //populate page property
            response.PageInfo = searchCriteria.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<List<MES.DTO.Library.APQP.DocumentManagement.Document>> GetDocumentList(int documentManagementId)
        {
            string errMSg = string.Empty;
            bool allowConfidentialDocumentType = true;
            LoginUser currentUser = GetCurrentUser;
            var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQPDashboard)).ToList();
            if (currentObjects.Count > 0)
                allowConfidentialDocumentType = currentObjects[0].AllowConfidentialDocumentType;

            List<DTO.Library.APQP.DocumentManagement.Document> lstPartDocument = new List<DTO.Library.APQP.DocumentManagement.Document>();
            DTO.Library.APQP.DocumentManagement.Document partDocument;

            this.RunOnDB(context =>
            {
                var PartDocumentList = context.GetDocumentByAPQPItemById(documentManagementId).ToList();
                if (PartDocumentList != null)
                {
                    foreach (var item in PartDocumentList)
                    {
                        if (item.IsConfidential && !allowConfidentialDocumentType)
                        { continue; }
                        partDocument = new DTO.Library.APQP.DocumentManagement.Document();
                        partDocument.Id = item.Id;
                        partDocument.APQPItemId = item.APQPItemId;
                        partDocument.DocumentTypeId = item.DocumentTypeId;
                        partDocument.DocumentType = item.DocumentType;
                        partDocument.IsConfidential = item.IsConfidential;
                        partDocument.ReceivedDate = item.ReceivedDate;
                        partDocument.PreparedDate = item.PreparedDate;
                        partDocument.FileTitle = item.FileTitle;
                        partDocument.FilePath = !string.IsNullOrEmpty(item.FilePath) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) +
                                Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + item.FilePath : string.Empty;
                        partDocument.Comments = item.Comments;
                        partDocument.RevLevel = item.RevLevel;
                        partDocument.RowNumber = item.RowNumber;
                        partDocument.DocumentCount = item.DocumentCount;
                        partDocument.CreatedDate = item.CreatedDate;
                        partDocument.CreatedDateString = item.CreatedDate.Value.FormatDateInMediumDateWithTime();
                        lstPartDocument.Add(partDocument);
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.DocumentManagement.Document>>(errMSg, lstPartDocument);
            return response;
        }

        public NPE.Core.ITypedResponse<string> DownloadDocuments(List<string> DocumentFilePathList)
        {
            string errMSg = null, successMsg = null, zipFileNavigationURL = string.Empty;

            try
            {
                if (DocumentFilePathList.Count > 0)
                {
                    if (DocumentFilePathList.Count == 1)
                    {
                        if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), DocumentFilePathList[0].Trim()))
                        {
                            successMsg = DocumentFilePathList[0].Trim();
                        }
                    }
                    else
                    {
                        using (ZipFile zip = new ZipFile())
                        {
                            HttpContext context = HttpContext.Current;
                            for (var i = 0; i < DocumentFilePathList.Count; i++)
                            {
                                if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), DocumentFilePathList[i]))
                                {
                                    byte[] fileBytes = Helper.BlobHelper.GetBlobStreamByUrl(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), DocumentFilePathList[i]);
                                    using (var fileStream = new MemoryStream(fileBytes))
                                    {
                                        fileStream.Seek(0, SeekOrigin.Begin);
                                        zip.AddEntry(Convert.ToString(i) + Path.GetFileName(DocumentFilePathList[i].Trim()), fileBytes);
                                    }
                                }
                            }
                            string zipName = String.Format("document_{0}.zip", DateTime.Now.ToString("MM_dd_yy_HH_mm_ss"));
                            using (var zipStream = new MemoryStream())
                            {
                                zip.Save(zipStream);
                                zipStream.Seek(0, SeekOrigin.Begin);
                                zipFileNavigationURL = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), "", zipName, zipStream);
                            }
                        }
                        successMsg = zipFileNavigationURL;
                    }
                }
            }
            catch (Exception ex)
            {
                errMSg = ex.ToString();
            }

            var response = SuccessOrFailedResponse<string>(errMSg, successMsg, successMsg);
            return response;
        }

    }
}
