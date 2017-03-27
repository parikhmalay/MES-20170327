using Account.DTO.Library;
using MES.Business.Repositories.APQP.DefectTracking;
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
using Winnovative.ExcelLib;
using System.Data;
using MES.Business.Repositories.RFQ.Supplier;
using System.Web;
using System.Net.Mail;
using EvoPdf.HtmlToPdf.PdfDocument;
using System.Drawing;

namespace MES.Business.Library.BO.APQP.DefectTracking
{
    class DefectTracking : ContextBusinessBase, IDefectTrackingRepository
    {
        public DefectTracking()
            : base("DefectTracking")
        { }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.APQP.DefectTracking.DefectTracking defectTracking)
        {
            string errMSg = null;
            string successMsg = null;

            ObjectParameter DefectTrackingId = new ObjectParameter("DefectTrackingId", defectTracking.Id);
            ObjectParameter ErrorKey = new ObjectParameter("ErrorKey", "");
            this.RunOnDB(context =>
            {
                int result = context.dtSaveDefectTracking(DefectTrackingId, defectTracking.IncludeInPPM, defectTracking.Finding, defectTracking.QualityOrDeliveryIssue,
                    defectTracking.CustomerCode, defectTracking.RMANumber, defectTracking.RMADate, defectTracking.RMAInitiatedBy, defectTracking.PartNumber, CurrentUser, ErrorKey);
                if (Convert.ToInt32(DefectTrackingId.Value) <= 0 || !string.IsNullOrEmpty(Convert.ToString(ErrorKey.Value)))
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(ErrorKey.Value)))
                        errMSg = Languages.GetResourceText(Convert.ToString(ErrorKey.Value));
                    else
                        errMSg = Languages.GetResourceText("DTSaveFail");
                }
                else
                {
                    defectTracking.Id = Convert.ToInt32(DefectTrackingId.Value);

                    #region "Save defect tracking Details"
                    if (defectTracking.lstDefectTrackingDetail.Count > 0 && defectTracking.Mode != "DropdownButton")
                    {
                        MES.Business.Repositories.APQP.DefectTracking.IDefectTrackingDetailRepository objIDefectTrackingDetailRepository = null;
                        foreach (var objDefectTrackingDetail in defectTracking.lstDefectTrackingDetail)
                        {
                            objIDefectTrackingDetailRepository = new MES.Business.Library.BO.APQP.DefectTracking.DefectTrackingDetail();
                            objDefectTrackingDetail.DefectTrackingId = defectTracking.Id;
                            var objResult = objIDefectTrackingDetailRepository.Save(objDefectTrackingDetail);
                            if (objResult == null || objResult.StatusCode != 200)
                            {
                                errMSg = Languages.GetResourceText("DTSaveFail");
                            }
                        }
                    }

                    //TODO
                    bool ret = GenerateRMAForm(defectTracking.Id);

                    #endregion
                }
            });

            if (string.IsNullOrEmpty(errMSg))
            {
                successMsg = Languages.GetResourceText("DTSavedSuccess");

                #region Create CAPA record
                if (defectTracking.AddToCAPA == 1)
                {
                    try
                    {
                        var objDTDetails = defectTracking.lstDefectTrackingDetail.Where(b => b.chkSelect == true).First();
                        var objCAPA = this.DataContext.capaItemMasters.Where(a => a.DefectTrackingId == defectTracking.Id && a.SupplierCode == objDTDetails.SupplierCode).SingleOrDefault();
                        if (objCAPA != null && objCAPA.Id > 0)  // if CAPA is already created for this DT then just redirect to CAPA in Edit mode. so returning only CAPA Id.
                        {
                            defectTracking.Id = objCAPA.Id; //return CAPA Id 
                        }
                        else
                        {
                            int CAPAId = CreateCAPAFromDT(defectTracking.Id, defectTracking);
                            defectTracking.Id = CAPAId; // return CAPA Id
                        }
                    }
                    catch (Exception ex)
                    {
                        successMsg = null;
                        errMSg = ex.Message;
                    }
                }
                #endregion
            }
            return SuccessOrFailedResponse<int?>(errMSg, defectTracking.Id, successMsg);
        }
        public int CreateCAPAFromDT(int dtId, DTO.Library.APQP.DefectTracking.DefectTracking objDefectTracking)
        {
            #region "Create new CAPA record. Note : followed the step of CAPA Save function"
            #region "Save CAPA master table data"
            int CAPAId = 0;
            var objDTDetails = objDefectTracking.lstDefectTrackingDetail.Where(b => b.chkSelect == true).First();
            var objCAPAMaster = new MES.Data.Library.capaItemMaster();
            objCAPAMaster.DefectTrackingId = dtId;
            objCAPAMaster.IncludeInPPM = objDefectTracking.IncludeInPPM;
            objCAPAMaster.CorrectiveActionType = objDefectTracking.Finding;
            objCAPAMaster.CorrectiveActionInitiatedBy = CurrentUser;
            objCAPAMaster.SupplierName = objDTDetails.SupplierName;
            objCAPAMaster.SupplierCode = objDTDetails.SupplierCode;
            objCAPAMaster.SupplierContactName = objDTDetails.SupplierContactName;
            objCAPAMaster.CustomerCode = objDefectTracking.CustomerCode;
            objCAPAMaster.CustomerName = objDefectTracking.CustomerName;
            objCAPAMaster.CorrectiveActionInitiatedDate = objCAPAMaster.CreatedDate = AuditUtils.GetCurrentDateTime();
            objCAPAMaster.CreatedBy = objCAPAMaster.UpdatedBy = CurrentUser;
            this.DataContext.capaItemMasters.Add(objCAPAMaster);
            this.DataContext.SaveChanges();
            CAPAId = objCAPAMaster.Id;
            #endregion
            #region "Save capa parts affected Details"
            MES.Data.Library.capaPartAffectedDetail objcapaPartAffectedDetail = null;
            ObjectParameter ErrorKey = new ObjectParameter("ErrorKey", "");
            foreach (var objDefectTrackingDetail in objDefectTracking.lstDefectTrackingDetail.Where(a => a.chkSelect == true).ToList())
            {
                #region "Save in capa parts affected detail table"
                objcapaPartAffectedDetail = new MES.Data.Library.capaPartAffectedDetail();
                objcapaPartAffectedDetail.CorrectiveActionId = CAPAId;
                if (!string.IsNullOrEmpty(objDefectTrackingDetail.DefectDescription) && this.DataContext.DefectTypes.Any(a => a.DefectType1 == objDefectTrackingDetail.DefectDescription.Trim()))
                    objcapaPartAffectedDetail.DefectTypeId = this.DataContext.DefectTypes.Where(a => a.DefectType1 == objDefectTrackingDetail.DefectDescription.Trim()).SingleOrDefault().Id;
                objcapaPartAffectedDetail.APQPItemId = objDefectTrackingDetail.APQPItemId;
                objcapaPartAffectedDetail.PartName = objDefectTrackingDetail.PartName;
                objcapaPartAffectedDetail.CustomerRejectedPartQty = objDefectTrackingDetail.CustomerInitialRejectQty;
                objcapaPartAffectedDetail.SupplierRejectedPartQty = objDefectTrackingDetail.TotalNumberOfPartsRejected;
                objcapaPartAffectedDetail.CreatedDate = objCAPAMaster.CreatedDate;
                objcapaPartAffectedDetail.CreatedBy = objcapaPartAffectedDetail.UpdatedBy = CurrentUser;
                this.DataContext.capaPartAffectedDetails.Add(objcapaPartAffectedDetail);
                this.DataContext.SaveChanges();
                #endregion

                #region "Insert DT docs into CAPA document table"
                this.DataContext.dtSaveDTDocumentInToCAPADocument(objDefectTrackingDetail.Id, objcapaPartAffectedDetail.Id, CurrentUser, ErrorKey);
                #endregion
            }
            #endregion
            #endregion
            return CAPAId;
        }
        public List<MES.DTO.Library.APQP.DefectTracking.PartDocument> dtGetPartDocumentByDTId(int defectTrackingId)
        {
            List<DTO.Library.APQP.DefectTracking.PartDocument> lstPartDocument = new List<DTO.Library.APQP.DefectTracking.PartDocument>();
            DTO.Library.APQP.DefectTracking.PartDocument partDocument;
            this.RunOnDB(context =>
            {
                var PartDocumentList = context.dtGetPartDocumentByDTId(defectTrackingId).ToList();
                if (PartDocumentList != null)
                {
                    foreach (var item in PartDocumentList)
                    {
                        partDocument = new DTO.Library.APQP.DefectTracking.PartDocument();
                        partDocument.Id = item.Id;
                        partDocument.DefectTrackingDetailId = item.DefectTrackingDetailId;
                        partDocument.DocumentTypeId = item.DocumentTypeId;
                        partDocument.DocumentType = item.DocumentType;
                        partDocument.IsConfidential = item.IsConfidential;
                        partDocument.FileTitle = item.FileTitle;
                        partDocument.FilePath = !string.IsNullOrEmpty(item.FilePath) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) +
                                Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + item.FilePath : string.Empty;
                        partDocument.Comments = item.Comments;
                        partDocument.RevLevel = item.RevLevel;
                        partDocument.AssociatedToId = item.AssociatedToId;
                        lstPartDocument.Add(partDocument);
                    }
                }
            });
            return lstPartDocument;
        }

        public NPE.Core.ITypedResponse<bool?> GenerateCAPAForm(DTO.Library.APQP.DefectTracking.DefectTracking defectTracking)
        {
            string filepath = string.Empty;
            var context = HttpContext.Current;
            string directoryPath = context.Server.MapPath(Constants.APQPTemplateFolder)
                       , filePath = context.Server.MapPath(Constants.APQPTemplateFolder) + "DTCAPAFormTemplate.xls";

            try
            {
                bool isValid = true;
                //A CAPA form can be associated with only one supplier. Please select the same supplier for all parts.
                string supplierName = string.Empty, supplierTemp = string.Empty, supplierCode = string.Empty;

                if (isValid)
                {

                    if (System.IO.Directory.Exists(directoryPath))
                    {
                        if (File.Exists(filePath))
                        {
                            System.IO.FileStream sourceXlsDataStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                            ExcelWorkbook ew = new ExcelWorkbook(sourceXlsDataStream);
                            ew.LicenseKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["wnvkey"]);

                            ExcelWorksheet ws = ew.Worksheets[0]; ws.Name = "Summary Sheet";
                            ExcelWorksheet ws1 = ew.Worksheets[1]; ws1.Name = "Defective Parts Pictures";
                            ExcelWorksheet ws2 = ew.Worksheets[2]; ws2.Name = "CA Verification Pictures";

                            #region Section - One Elements

                            /*Corrective Actions Issuer - A4
                                 *Dan Promen:Issuer Name should populate from RMA Initiated by field in portal*/
                            ws[4, 1].Value = defectTracking.RMAInitiatedByName.Trim();
                            ws[4, 2].Value = "001-740-201-8112";//Phone - B4

                            /*E-Mail - C4
                             *Dan Promen:email address should populate from email listed in User list.*/
                            if (!string.IsNullOrEmpty(defectTracking.RMAInitiatedBy))
                            {
                                MES.Business.Repositories.UserManagement.IUserManagementRepository userObj = new UserManagement.UserManagement();
                                MES.DTO.Library.Identity.LoginUser info = userObj.FindById(defectTracking.RMAInitiatedBy).Result;

                                ws[4, 3].Value = info.Email;
                            }

                            /*Supplier Name - D3
                             *Should be linked to Defect Portal 4. Other page Supplier Contact field. Can supplier contact be linked to contacts in SAP?
                             */
                            supplierCode = defectTracking.lstDefectTrackingDetail[0].SupplierCode;
                            supplierName = supplierCode + "-" + defectTracking.lstDefectTrackingDetail[0].SupplierName;

                            ws[3, 4].Value = supplierName;
                            ISuppliersRepository SuppliersRepository = new MES.Business.Library.BO.RFQ.Supplier.Suppliers();
                            MES.DTO.Library.RFQ.Supplier.Suppliers sItem = SuppliersRepository.FindByCode(supplierCode).Result;

                            if (sItem != null)
                            {
                                /*Supplier Contact - A8
                                 *editable field to add name of China Supplier Quality Engineer 
                                 */
                                ws[8, 1].Value = sItem.SQName;

                                /*Phone - B8
                                 *make field editable  
                                 */
                                ws[8, 2].Value = "";

                                /*Email - C8
                                 *editable field
                                 */
                                ws[8, 3].Value = sItem.SQEmail;

                                /*Supplier Contact - A10
                                 *Should be linked to Defect Portal 4. Other page Supplier Contact field. Can supplier contact be linked to contacts in SAP?
                                 */
                                ws[10, 1].Value = sItem.SCName;

                                /*Phone - B10
                                 *Can this field read from contact information in SAP? If not, Manual Entry 
                                 */
                                ws[10, 2].Value = sItem.SCPhone;

                                /*Email - C10
                                 *Can this field read from SAP contact list? If not Manual Entry.
                                 */
                                ws[10, 3].Value = sItem.SCEmail;
                            }
                            else { ws[10, 1].Value = defectTracking.lstDefectTrackingDetail[0].SupplierContactName; }


                            /*Corrective Actions Issuer - F2
                             *Dan Promen:date field should automatically populate as the date form is created. This should link to the RMA Date in defect tracking portal.*/
                            ws[2, 6].Value = defectTracking.RMADate;

                            /*Due Date: - H2
                             *Dan Promen: Field should auto populate from Open Date + 30 calendar days as default. Example:Open Date 14-JAN-16 Due Date - 13-FEB-16*/
                            if (defectTracking.RMADate.HasValue)
                                ws[2, 8].Value = defectTracking.RMADate.Value.AddDays(30);

                            /*CAPA# - E3
                             *Dan Promen:CAPA # field needs to auto populate with year and sequential number. It should be the RMA # from Defect Portal.
                             *Example: 2016-0001, 2016-0002 etc.
                             *In case of Multiple line items in Defect Tracking Record - add and L# (For line item on Defect tracking record) onto the CAPA.  For example, CAPA # 2016-0002-L1.  
                             *If this is too complicated, just keep the same CAPA number for all line items that generate a CAPA.*/
                            ws[3, 6].Value = defectTracking.RMANumber;

                            /*Quality - D4 | Delivery - E4 | Other - F4
                             *Dan Promen:Quality, Delivery, Other need to have active check boxes included in the field.  Default should be based on what is elected in Defect tracking header (Defect Type field).*/
                            //TODO

                            //
                            //
                            //
                            #endregion

                            #region Section - One - Part Details
                            /*Parts details*/
                            int rowIndex = 13;
                            List<MES.DTO.Library.APQP.DefectTracking.DefectTrackingDetail> lstDTD = defectTracking.lstDefectTrackingDetail.Where(item => item.chkSelect == true).ToList();

                            if (lstDTD.Count > 0)
                                foreach (MES.DTO.Library.APQP.DefectTracking.DefectTrackingDetail item in lstDTD)
                                {

                                    /*Part Name and Part #'s Pull in part number listed for that Line item and pull in Part Description from SAP.*/
                                    ws[rowIndex, 1].Value = item.PartNumber;
                                    ws[rowIndex, 2].Value = item.PartName;

                                    /*pull from Defect Description field in page 1 for the line item CAPA is being generated for.*/
                                    ws[rowIndex, 3].Value = item.DefectDescription;
                                    /*Initial Parts quantity NO GOOD @ Customer? - M8
                                     *Page 1 customer sort / rework page link to Initial Rejects at Customer field. Auto Populate
                                     */
                                    ws[rowIndex, 4].Value = item.CustomerInitialRejectQty.HasValue ? item.CustomerInitialRejectQty.Value : 0;

                                    rowIndex++;
                                    ws.InsertRow(rowIndex, true, true);

                                }

                            ws.DeleteRow(rowIndex);
                            #endregion

                            #region Add Defective Parts Pictures & CA Verification Pictures

                            if (!System.IO.Directory.Exists(context.Server.MapPath("~") + Constants.DTRMAFormDocumentPhyFolder))
                                System.IO.Directory.CreateDirectory(context.Server.MapPath("~") + Constants.DTRMAFormDocumentPhyFolder);
                            else
                            {
                                try
                                {
                                    System.IO.DirectoryInfo di = new DirectoryInfo(context.Server.MapPath("~") + Constants.DTRMAFormDocumentPhyFolder);
                                    foreach (FileInfo file in di.GetFiles())
                                    {
                                        file.Delete();
                                    }
                                }
                                catch { }
                            }
                            bool isFirst = true;
                            int leftColumnIndex = 0, topRowIndex = 0, rightColumnIndex = 0, bottomRowIndex = 0;
                            ExcelPicture ePicture = null;

                            string fileTemp = string.Empty;
                            HashSet<string> imageExtensions; string extension = string.Empty; bool isImage;
                            string sectionName = "DTSTEP1";
                            List<MES.DTO.Library.APQP.DefectTracking.PartDocument> lstDocuments = null;
                            foreach (MES.DTO.Library.APQP.DefectTracking.DefectTrackingDetail DTDitem in lstDTD)
                            {
                                lstDocuments = GetPartDocumentList(DTDitem.Id, sectionName).Result;

                                if (lstDocuments.Count > 0)
                                    foreach (MES.DTO.Library.APQP.DefectTracking.PartDocument partDocument in lstDocuments)
                                    {
                                        if (partDocument.AssociatedToId.HasValue && partDocument.AssociatedToId.Value == (int)MES.Business.Library.Enums.DocTypeAssociatedTo.DefectTrackingPartDocument)
                                        {
                                            imageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                                            imageExtensions.Add(".jpg"); imageExtensions.Add(".jpeg");
                                            imageExtensions.Add(".bmp"); imageExtensions.Add(".png");
                                            imageExtensions.Add(".tiff"); imageExtensions.Add(".gif");

                                            extension = Path.GetExtension(partDocument.FilePath);
                                            isImage = imageExtensions.Contains(extension);
                                            if (isImage)
                                            {
                                                if (isFirst)
                                                    topRowIndex = ws1.UsedRangeRows.Count + 2;
                                                else
                                                    topRowIndex += 2;
                                                byte[] fileBytes = Helper.BlobHelper.GetBlobStreamByUrl(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                                    , partDocument.FilePath);
                                                fileTemp = context.Server.MapPath("~") + Constants.DTRMAFormDocumentPhyFolder + Guid.NewGuid();// Path.GetFileName(partDocument.FilePath);
                                                using (var fileStream = new MemoryStream(fileBytes))
                                                {
                                                    fileStream.Seek(0, SeekOrigin.Begin);
                                                    File.WriteAllBytes(fileTemp, fileBytes);

                                                    ePicture = ws1.Pictures.AddPicture(1, topRowIndex, fileTemp);
                                                }

                                                leftColumnIndex = ePicture.LeftColumnIndex;
                                                topRowIndex = ePicture.TopRowIndex;
                                                rightColumnIndex = ePicture.RightColumnIndex;
                                                bottomRowIndex = ePicture.BottomRowIndex;

                                                ws1[topRowIndex, 1].RowHeightInPoints = 12.75 * (bottomRowIndex - topRowIndex);
                                                isFirst = false;
                                            }
                                        }
                                    }
                            }

                            isFirst = true;
                            lstDocuments = null;
                            sectionName = "DTSTEP2";
                            foreach (MES.DTO.Library.APQP.DefectTracking.DefectTrackingDetail DTDitem in lstDTD)
                            {
                                lstDocuments = GetPartDocumentList(DTDitem.Id, sectionName).Result;
                                if (lstDocuments.Count > 0)
                                    foreach (MES.DTO.Library.APQP.DefectTracking.PartDocument partDocument in lstDocuments)
                                    {
                                        if (partDocument.AssociatedToId.HasValue && partDocument.AssociatedToId.Value == (int)MES.Business.Library.Enums.DocTypeAssociatedTo.DefectTrackingCorrectiveAction)
                                        {
                                            imageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                                            imageExtensions.Add(".jpg"); imageExtensions.Add(".jpeg");
                                            imageExtensions.Add(".bmp"); imageExtensions.Add(".png");
                                            imageExtensions.Add(".tiff"); imageExtensions.Add(".gif");

                                            extension = Path.GetExtension(partDocument.FilePath);
                                            isImage = imageExtensions.Contains(extension);
                                            if (isImage)
                                            {
                                                if (isFirst)
                                                    topRowIndex = ws2.UsedRangeRows.Count + 2;
                                                else
                                                    topRowIndex += 2;

                                                byte[] fileBytes = Helper.BlobHelper.GetBlobStreamByUrl(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                                 , partDocument.FilePath);
                                                fileTemp = context.Server.MapPath("~") + Constants.DTRMAFormDocumentPhyFolder + Guid.NewGuid();// Path.GetFileName(partDocument.FilePath);
                                                using (var fileStream = new MemoryStream(fileBytes))
                                                {
                                                    fileStream.Seek(0, SeekOrigin.Begin);
                                                    File.WriteAllBytes(fileTemp, fileBytes);

                                                    ePicture = ws2.Pictures.AddPicture(1, topRowIndex, fileTemp);
                                                }
                                                leftColumnIndex = ePicture.LeftColumnIndex;
                                                topRowIndex = ePicture.TopRowIndex;
                                                rightColumnIndex = ePicture.RightColumnIndex;
                                                bottomRowIndex = ePicture.BottomRowIndex;

                                                ws2[topRowIndex, 1].RowHeightInPoints = 12.75 * (bottomRowIndex - topRowIndex);
                                                isFirst = false;
                                            }
                                        }
                                    }
                            }

                            #endregion

                            string generatedFileName = "CAPA-" + defectTracking.RMANumber + ".xls";
                            try
                            {

                                byte[] fileBytes = ew.Save();
                                string dtFilePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                                    + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + Constants.TEMPFILESEMAILATTACHMENTSFOLDER + generatedFileName;

                                Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), dtFilePath);

                                filePath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                              , Constants.TEMPFILESEMAILATTACHMENTSFOLDERName
                                              , generatedFileName
                                              , fileBytes);
                                if (sItem != null && !string.IsNullOrEmpty(sItem.SQEmail))
                                {
                                    SendEmailToSQ(sItem, filePath, defectTracking.RMANumber);
                                }


                            }
                            catch (Exception ex) //Error
                            {
                                return FailedBoolResponse(ex.Message);
                            }
                            finally
                            {
                                sourceXlsDataStream.Flush();
                                sourceXlsDataStream.Close();

                                ew.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return SuccessBoolResponse(filePath);
        }

        private void SendEmailToSQ(DTO.Library.RFQ.Supplier.Suppliers sqUser, string CAPAPath, string RMANumber)
        {
            MES.DTO.Library.Common.EmailData emailData = null;
            string strBodyText = string.Empty;
            bool IsSuccess = false;
            try
            {
                emailData = new DTO.Library.Common.EmailData();
                MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
                emailData.EmailBody = "Hello " + sqUser.SQName + ",<br />" + "CAPA form has been generated by " + CurrentUser + " for RMA - " + RMANumber;

                string footer = @"<br /><br /><br /><hr />
                                    <span style='color: #FF0000; font-size: 10px; font-family: Tahoma, Geneva, sans-serif;' align='left'> 
                                        Electronic Privacy Notice & Email Disclaimer:
                                    </span><br />
                                    <span style='color: #626465; font-size: 10px; font-family: Tahoma, Geneva, sans-serif;' align='left'>  
                                        This email, and any attachments, contains information that is, or may be, covered by electronic communications privacy laws,
                                        and is also confidential and proprietary in nature. If you are not the intended recipient, please be advised that you are 
                                        legally prohibited from retaining, using, copying, distributing, or otherwise disclosing this information in any manner.
                                        Instead, please reply to the sender that you have received this communication in error and then immediately delete it. 
                                        MES Inc. is not liable for any damage caused by any virus transmitted by this email. Thank you for your co operation.
                                    </span>";

                //create a list for To address
                List<string> lstToAddress = new List<string>();
                lstToAddress.Add(sqUser.SQEmail);
                //TODO: Attachment-Ext Quote PDF
                List<Attachment> attachments = new List<Attachment>();

                if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                  , CAPAPath))
                {
                    Stream memoryStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(CAPAPath);
                    attachments.Add(new System.Net.Mail.Attachment(memoryStream, Path.GetFileName(CAPAPath)));
                }
                emailData.EmailSubject = "CAPA form for RMA - " + RMANumber;
                emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailData.EmailBody + footer, out IsSuccess, attachments, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool GenerateRMAForm(int defectTrackingId)
        {
            string errMSg = null, filepath = string.Empty;
            var context = System.Web.HttpContext.Current;
            try
            {
                DTO.Library.APQP.DefectTracking.DefectTracking defectTracking = FindById(defectTrackingId).Result;

                PdfConverter pdfConverter = new PdfConverter();
                pdfConverter.LicenseKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["evopdfkey"]);
                pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.Letter;
                pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
                pdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
                pdfConverter.AvoidImageBreak = true;
                pdfConverter.AvoidTextBreak = true;

                string contents = File.ReadAllText(context.Server.MapPath("~/EmailTemplates/dtRMAForm.htm"));

                // Replace the placeholders with the user-specified text
                contents = contents.Replace("<%RMADate%>", defectTracking.RMADate.HasValue ? Convert.ToDateTime(defectTracking.RMADate).ToString("dd-MMM-yy") : string.Empty);
                contents = contents.Replace("<%RMANumber%>", defectTracking.RMANumber);
                contents = contents.Replace("<%CustomerDetails%>", defectTracking.CustomerName);
                contents = contents.Replace("<%RMAInitiatedBy%>", defectTracking.RMAInitiatedByName);

                List<MES.DTO.Library.APQP.DefectTracking.DefectTrackingDetail> defectTrackingDetailList = defectTracking.lstDefectTrackingDetail.ToList();
                if (defectTrackingDetailList == null || defectTrackingDetailList.Count == 0)
                    return false;

                MES.DTO.Library.APQP.DefectTracking.DefectTrackingDetail firstPartDefectTrackingDetail = new DTO.Library.APQP.DefectTracking.DefectTrackingDetail();
                firstPartDefectTrackingDetail = defectTrackingDetailList.First();
                string warehouseLoc = string.Empty;

                MES.Business.Repositories.Setup.Destination.IDestinationRepository destinationObj = new Setup.Destination.Destination();
                if (firstPartDefectTrackingDetail != null && firstPartDefectTrackingDetail.MESWarehouseLocationId.HasValue)
                {
                    DTO.Library.Setup.Destination.Destination destinationInfo = destinationObj.FindById(firstPartDefectTrackingDetail.MESWarehouseLocationId.Value).Result;
                    warehouseLoc = destinationInfo.Location;
                }
                contents = contents.Replace("<%WarehouseDetails%>", warehouseLoc);

                StringBuilder PartRejectionDetailsHTML = new StringBuilder();
                int totalCustomerRejectedPartQty = 0;
                foreach (MES.DTO.Library.APQP.DefectTracking.DefectTrackingDetail defectTrackingDetailInfo in defectTrackingDetailList)
                {
                    if (defectTrackingDetailInfo.CustomerRejectedPartQty.HasValue)
                        totalCustomerRejectedPartQty += defectTrackingDetailInfo.CustomerRejectedPartQty.Value;

                    PartRejectionDetailsHTML.AppendLine("<tr>");
                    PartRejectionDetailsHTML.AppendLine("<td align='left' valign='top' width='15%' ><b>PART #</b></td>");
                    PartRejectionDetailsHTML.AppendLine("<td align='left' valign='top' width='10%' ><b>Suplier Code</b></td>");
                    PartRejectionDetailsHTML.AppendLine("<td align='center' valign='top' width='20%' ><b>QTY REJECTED (CUST PARTS) - PPM</b></td>");
                    PartRejectionDetailsHTML.AppendLine("<td align='left' valign='top' width='20%' ><b>Defect Description</b></td>");
                    PartRejectionDetailsHTML.AppendLine("<td align='center' valign='top' width='10%' ><b>Weight per Piece</b></td>");
                    PartRejectionDetailsHTML.AppendLine("<td align='center' valign='top' width='5%' ><b>Disposition</b></td>");
                    PartRejectionDetailsHTML.AppendLine("<td align='left' valign='top' width='20%' ><b>Comments</b></td>");
                    PartRejectionDetailsHTML.AppendLine("</tr>");

                    PartRejectionDetailsHTML.AppendLine("<tr>");
                    PartRejectionDetailsHTML.AppendLine("<td align='left' valign='top' >" + defectTrackingDetailInfo.PartNumber + "</td>");
                    PartRejectionDetailsHTML.AppendLine("<td align='left' valign='top' >" + defectTrackingDetailInfo.SupplierCode + "</td>");
                    PartRejectionDetailsHTML.AppendLine("<td align='center' valign='top' >" + defectTrackingDetailInfo.CustomerRejectedPartQty + "</td>");
                    PartRejectionDetailsHTML.AppendLine("<td align='left' valign='top' >" + defectTrackingDetailInfo.DefectDescription + "</td>");
                    PartRejectionDetailsHTML.AppendLine("<td align='right' valign='top' >" + defectTrackingDetailInfo.WeightPerPiece + "</td>");
                    PartRejectionDetailsHTML.AppendLine("<td align='left' valign='top' >" + defectTrackingDetailInfo.DispositionOfParts + "</td>");
                    PartRejectionDetailsHTML.AppendLine("<td align='left' valign='top' >" + defectTrackingDetailInfo.Comment + "</td>");
                    PartRejectionDetailsHTML.AppendLine("</tr>");

                    PartRejectionDetailsHTML.AppendLine("<tr><td colspan='7' > ");

                    StringBuilder PartImageHTML = new StringBuilder();
                    PartImageHTML.AppendLine("<table border='0' class='tablebreak' >");

                    string sectionName = "DTSTEP1";
                    List<MES.DTO.Library.APQP.DefectTracking.PartDocument> lstDocuments = GetPartDocumentList(defectTrackingDetailInfo.Id, sectionName).Result;
                    string fileTemp = string.Empty;
                    string extension = string.Empty;
                    if (lstDocuments.Count > 0)
                    {
                        if (!System.IO.Directory.Exists(context.Server.MapPath("~") + Constants.DTPartMediumSizeImagePhyFolder))
                            System.IO.Directory.CreateDirectory(context.Server.MapPath("~") + Constants.DTPartMediumSizeImagePhyFolder);

                        foreach (MES.DTO.Library.APQP.DefectTracking.PartDocument partDocument in lstDocuments)
                        {
                            if ((".jpg,.jpeg,.gif,.bmp,.png,.tiff").ToLower().Contains(System.IO.Path.GetExtension(partDocument.FilePath).ToLower()))
                            {

                                if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                    , partDocument.FilePath))
                                {
                                    byte[] fileBytes = Helper.BlobHelper.GetBlobStreamByUrl(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                              , partDocument.FilePath);
                                    fileTemp = context.Server.MapPath("~") + Constants.DTPartMediumSizeImagePhyFolder + Path.GetFileName(partDocument.FilePath);
                                    try
                                    {
                                        using (var fileStream = new MemoryStream(fileBytes))
                                        {
                                            fileStream.Seek(0, SeekOrigin.Begin);
                                            File.WriteAllBytes(fileTemp, fileBytes);

                                            PartImageHTML.AppendLine("<tr><td>");
                                            PartImageHTML.AppendLine("<div style='position:relative; display:inline-block;'>");
                                            PartImageHTML.AppendLine("<img alt='' src='" + fileTemp + "' />");
                                            PartImageHTML.AppendLine("<div style='position:absolute; top:0; left:0; width:100%; height:100%;'><table border='0' cellpadding='0' cellspacing='0' style='width:100%; height:100%;'><tr><td valign='middle' style='vertical-align:middle; text-align:center;color:red;font-size:20px;font-weight:bold;'>" + (string.IsNullOrEmpty(partDocument.Comments) ? partDocument.Comments : partDocument.Comments.Trim()) + "</td></tr></table></div>");
                                            PartImageHTML.AppendLine("</div>");
                                            PartImageHTML.AppendLine("</td></tr>");
                                        }


                                    }
                                    catch (Exception ex)
                                    {
                                        continue;
                                    }

                                }
                            }
                            else
                            {
                                if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                      , partDocument.FilePath))
                                {
                                    fileTemp = context.Server.MapPath(Constants.IMAGEFOLDER);

                                    extension = Path.GetExtension(partDocument.FilePath);
                                    string fileAttachmentWithIconPath = string.Empty;
                                    switch (extension)
                                    {
                                        case ".doc":
                                        case ".docx":
                                            fileAttachmentWithIconPath = fileTemp + "doc.png";

                                            break;
                                        case ".xls":
                                        case ".xlsx":
                                            fileAttachmentWithIconPath = fileTemp + "xl.png";

                                            break;
                                        case ".pdf":
                                            fileAttachmentWithIconPath = fileTemp + "pdf.png";

                                            break;
                                        default:
                                            fileAttachmentWithIconPath = fileTemp + "file.png";

                                            break;
                                    }

                                    PartImageHTML.AppendLine("<tr><td>");
                                    PartImageHTML.AppendLine("<a href='" + partDocument.FilePath + "'><img alt='' src='" + fileAttachmentWithIconPath + "' /></a></td><td>");
                                    PartImageHTML.AppendLine("<div valign='middle' style='vertical-align:middle; text-align:center;color:red;font-size:20px;font-weight:bold;'>" + (string.IsNullOrEmpty(partDocument.Comments) ? partDocument.Comments : partDocument.Comments.Trim()) + "</div>");
                                    PartImageHTML.AppendLine("</td></tr>");
                                }
                            }
                        }
                    }
                    PartImageHTML.AppendLine("</table>");

                    if (!string.IsNullOrEmpty(PartImageHTML.ToString()))
                        PartRejectionDetailsHTML.AppendLine(PartImageHTML.ToString());

                    PartRejectionDetailsHTML.AppendLine("</td></tr>");
                }

                StringBuilder PartDetailsHTML = new StringBuilder();
                if (defectTrackingDetailList.Count > 1)
                {
                    PartDetailsHTML.AppendLine("<tr>");
                    PartDetailsHTML.AppendLine("<td valign='top' >Multiple</td>");
                    PartDetailsHTML.AppendLine("<td valign='top' >Multiple</td>");
                    PartDetailsHTML.AppendLine("<td align='center' valign='top' >" + totalCustomerRejectedPartQty + "</td>"); //CustomerInitialRejectQty - // IT should be pulled from QTY REJECTED (CUST PARTS) - PPM - [18-SEP Defect & RMA Tracking - Initial Testing-Review feedback.xlsx]
                    PartDetailsHTML.AppendLine("</tr>");
                }

                else
                {
                    if (firstPartDefectTrackingDetail != null)
                    {
                        PartDetailsHTML.AppendLine("<tr>");
                        PartDetailsHTML.AppendLine("<td valign='top' >" + firstPartDefectTrackingDetail.PartNumber + "</td>");
                        PartDetailsHTML.AppendLine("<td valign='top' >" + firstPartDefectTrackingDetail.PartName + "</td>");
                        PartDetailsHTML.AppendLine("<td align='center' valign='top' >" + firstPartDefectTrackingDetail.CustomerRejectedPartQty + "</td>"); //CustomerInitialRejectQty - // IT should be pulled from QTY REJECTED (CUST PARTS) - PPM - [18-SEP Defect & RMA Tracking - Initial Testing-Review feedback.xlsx]
                        PartDetailsHTML.AppendLine("</tr>");
                    }
                }
                contents = contents.Replace("<%PartDetails%>", PartDetailsHTML.ToString());
                contents = contents.Replace("<%PartRejectionDetails%>", PartRejectionDetailsHTML.ToString());

                // get the pdf bytes from html string
                byte[] pdfBytes = pdfConverter.GetPdfBytesFromHtmlString(contents);

                pdfConverter.PdfDocumentOptions.ShowHeader = true;
                pdfConverter.PdfHeaderOptions.HeaderHeight = 65;

                string headerAndFooterHtmlUrl = context.Server.MapPath("~/EmailTemplates/Header.htm");
                pdfConverter.PdfHeaderOptions.HtmlToPdfArea = new HtmlToPdfArea(0, 0, 0, pdfConverter.PdfHeaderOptions.HeaderHeight, headerAndFooterHtmlUrl, 900, 100);
                pdfConverter.PdfHeaderOptions.DrawHeaderLine = false;

                // Set the footer height in points
                pdfConverter.PdfFooterOptions.FooterHeight = 20;
                pdfConverter.PdfDocumentOptions.ShowFooter = true;
                pdfConverter.PdfFooterOptions.DrawFooterLine = true;
                TextArea footerText = new TextArea(0, 5, "Page &p; of &P;  ", new System.Drawing.Font(new System.Drawing.FontFamily("Times New Roman"), 10, System.Drawing.GraphicsUnit.Point));
                // Align the text at the right of the footer
                footerText.TextAlign = HorizontalTextAlign.Right;
                // Add the text element to footer
                pdfConverter.PdfFooterOptions.AddTextArea(footerText);

                if (!System.IO.Directory.Exists(context.Server.MapPath("~") + Constants.DTRMAFormDocumentPhyFolder))
                    System.IO.Directory.CreateDirectory(context.Server.MapPath("~") + Constants.DTRMAFormDocumentPhyFolder);

                string generatedFileName = "RMA-" + defectTracking.RMANumber + ".pdf";
                string tempFilepath = context.Server.MapPath("~") + Constants.DTRMAFormDocumentPhyFolder + generatedFileName;
                pdfConverter.SavePdfFromHtmlStringToFile(contents, tempFilepath);

                filepath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                         + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                         + Constants.DTRMAFormDocumentFolder
                         + generatedFileName;

                Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), filepath);

                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.DTRMAFormDocumentFolder
                              , generatedFileName
                              , tempFilepath);
                File.Delete(tempFilepath);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }
        public NPE.Core.ITypedResponse<bool?> GenerateRMAFormFromCAPA(int defectTrackingId)
        {
            GenerateRMAForm(defectTrackingId);
            return SuccessBoolResponse(Languages.GetResourceText("RMAFormSuccess"));
        }

        public NPE.Core.ITypedResponse<DTO.Library.APQP.DefectTracking.DefectTracking> FindById(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.APQP.DefectTracking.DefectTracking defectTracking = new DTO.Library.APQP.DefectTracking.DefectTracking();
            this.RunOnDB(context =>
            {
                var DefectTracking = context.dtGetDefectTrackingById(id).SingleOrDefault();
                if (DefectTracking == null)
                    errMSg = Languages.GetResourceText("DefectTrackingNotExists");
                else
                {
                    #region general details
                    defectTracking = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.DefectTracking.DefectTracking>(DefectTracking);
                    #endregion
                    #region Bind defect traking list details
                    MES.Business.Repositories.APQP.DefectTracking.IDefectTrackingDetailRepository objIDefectTrackingDetailRepository = new MES.Business.Library.BO.APQP.DefectTracking.DefectTrackingDetail();
                    defectTracking.lstDefectTrackingDetail = objIDefectTrackingDetailRepository.GetDefectTrackingDetailList(id).Result;
                    #endregion
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.APQP.DefectTracking.DefectTracking>(errMSg, defectTracking);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int defectTrackingId)
        {
            try
            {
                var capaItemMasterToBeDeleted = this.DataContext.dtDefectTrackings.Where(a => a.Id == defectTrackingId).SingleOrDefault();
                if (capaItemMasterToBeDeleted == null)
                {
                    return FailedBoolResponse(Languages.GetResourceText("DTPNotExists"));
                }
                else
                {
                    capaItemMasterToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    capaItemMasterToBeDeleted.UpdatedBy = CurrentUser;
                    this.DataContext.Entry(capaItemMasterToBeDeleted).State = EntityState.Modified;
                    capaItemMasterToBeDeleted.IsDeleted = true;
                    this.DataContext.SaveChanges();
                    return SuccessBoolResponse(Languages.GetResourceText("DTDeletedSuccess"));
                }
            }
            catch (Exception ex)
            {
                return FailedBoolResponse(Languages.GetResourceText("DTDeleteFailed"));
            }
        }
        public NPE.Core.ITypedResponse<bool?> DeleteDefectTrackingDetail(int DefectTrackingDetailId)
        {
            #region Delete DefectTrackingDetail
            IDefectTrackingDetailRepository objIDefectTrackingDetailRepository = new MES.Business.Library.BO.APQP.DefectTracking.DefectTrackingDetail();
            return objIDefectTrackingDetailRepository.Delete(DefectTrackingDetailId);
            #endregion
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.DefectTracking.DefectTracking>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.DefectTracking.DefectTracking>> GetDefectTrackingList(NPE.Core.IPage<DTO.Library.APQP.DefectTracking.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            var httpContext = System.Web.HttpContext.Current;
            List<DTO.Library.APQP.DefectTracking.DefectTracking> lstDefectTracking = new List<DTO.Library.APQP.DefectTracking.DefectTracking>();
            DTO.Library.APQP.DefectTracking.DefectTracking defectTracking;
            this.RunOnDB(context =>
             {
                 var DefectTrackingList = context.dtGetDefectTrackings(paging.Criteria.RMANumber, paging.Criteria.CustomerName, paging.Criteria.SupplierName, paging.Criteria.PartNumberId,
                     paging.Criteria.RMAInitiatedBy, paging.Criteria.CorrectiveActionNumber, paging.Criteria.MESWarehouseLocationId, paging.Criteria.SupplierCode, paging.PageNo,
                     paging.PageSize, totalRecords, "").ToList();

                 if (DefectTrackingList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in DefectTrackingList)
                     {
                         defectTracking = new DTO.Library.APQP.DefectTracking.DefectTracking();
                         defectTracking = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.DefectTracking.DefectTracking>(item);

                         string generatedFileName = "RMA-" + defectTracking.RMANumber + ".pdf";
                         string filepath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                                   + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                   + "/" + Constants.DTRMAFormDocumentFolder
                                   + generatedFileName;
                         if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                         , filepath))
                         {
                             defectTracking.RMAFormFilePath = filepath;
                         }
                         else
                             defectTracking.RMAFormFilePath = string.Empty;
                         lstDefectTracking.Add(defectTracking);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.DefectTracking.DefectTracking>>(errMSg, lstDefectTracking);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<string> GetNewRMANumber()
        {
            //set the out put param
            ObjectParameter RMANumber = new ObjectParameter("RMANumber", "");
            ObjectParameter ErrorKey = new ObjectParameter("ErrorKey", "");
            string errMSg = null, outRMANumber = string.Empty;

            this.RunOnDB(context =>
            {
                context.dtGetGenerateNewRMANumber(RMANumber, ErrorKey);
                if (!string.IsNullOrEmpty(Convert.ToString(ErrorKey.Value)))
                    errMSg = Languages.GetResourceText(Convert.ToString(ErrorKey.Value));
                else
                {
                    outRMANumber = Convert.ToString(RMANumber.Value);
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<string>(errMSg, outRMANumber);
            return response;
        }

        public NPE.Core.ITypedResponse<List<MES.DTO.Library.APQP.DefectTracking.PartDocument>> GetPartDocumentList(int defectTrackingDetailId, string SectionName)
        {
            string errMSg = string.Empty;
            int AssociatedToId = 0;
            switch (SectionName)
            {
                case "DTSTEP1":
                    AssociatedToId = (int)MES.Business.Library.Enums.DocTypeAssociatedTo.DefectTrackingPartDocument;
                    break;
                case "DTSTEP2":
                    AssociatedToId = (int)MES.Business.Library.Enums.DocTypeAssociatedTo.DefectTrackingCorrectiveAction;
                    break;
                default:
                    break;
            }

            List<DTO.Library.APQP.DefectTracking.PartDocument> lstDocument = new List<DTO.Library.APQP.DefectTracking.PartDocument>();
            IPartDocumentRepository objIPartDocumentRepository = new MES.Business.Library.BO.APQP.DefectTracking.PartDocument();
            lstDocument = objIPartDocumentRepository.GetPartDocumentList(defectTrackingDetailId, AssociatedToId);
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.DefectTracking.PartDocument>>(errMSg, lstDocument);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> DeletePartDocument(int documentId)
        {
            #region Delete Document
            IPartDocumentRepository objIPartDocumentRepository = new MES.Business.Library.BO.APQP.DefectTracking.PartDocument();
            return objIPartDocumentRepository.Delete(documentId);
            #endregion
        }

        public NPE.Core.ITypedResponse<int?> SavePartDocument(DTO.Library.APQP.DefectTracking.PartDocument document)
        {
            #region Save Document
            IPartDocumentRepository objIPartDocumentRepository = new MES.Business.Library.BO.APQP.DefectTracking.PartDocument();
            return objIPartDocumentRepository.Save(document);
            #endregion
        }
    }
}
