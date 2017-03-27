using Account.DTO.Library;
using MES.Business.Repositories.RFQ.RFQ;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Core.Extensions;
using NPE.Core;
using System.Data.Entity;
using System.IO;
using GemBox.Spreadsheet;
using System.Text.RegularExpressions;
using System.Web;
using System.Data.Entity.Validation;

namespace MES.Business.Library.BO.RFQ.RFQ
{
    public class RFQParts : ContextBusinessBase, IRFQPartsRepository
    {
        public RFQParts()
            : base("RFQParts")
        { }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.RFQ.RFQParts rFQParts)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.Part();
            if (!string.IsNullOrEmpty(rFQParts.RfqId))
            {
                if (rFQParts.Id > 0)
                {
                    recordToBeUpdated = this.DataContext.Parts.Where(a => a.Id == rFQParts.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("RFQPartNotExists");
                    else
                    {
                        #region "Delete attachments of rfq parts"
                        var deletePartAttachmentList = this.DataContext.PartAttachments.Where(a => a.RFQPartId == rFQParts.Id).ToList();
                        foreach (var item in deletePartAttachmentList)
                        {
                            this.DataContext.PartAttachments.Remove(item);
                        }
                        #endregion
                        recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                        recordToBeUpdated.UpdatedBy = CurrentUser;
                        this.DataContext.Entry(recordToBeUpdated).State = EntityState.Modified;
                    }
                }
                else
                {
                    recordToBeUpdated.CreatedBy = recordToBeUpdated.UpdatedBy = CurrentUser;
                    recordToBeUpdated.CreatedDate = AuditUtils.GetCurrentDateTime();
                    recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    this.DataContext.Parts.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    #region Save general details
                    recordToBeUpdated.RFQId = rFQParts.RfqId;
                    recordToBeUpdated.CustomerPartNo = rFQParts.CustomerPartNo;
                    recordToBeUpdated.RevLevel = rFQParts.RevLevel;
                    recordToBeUpdated.PartDescription = rFQParts.PartDescription;
                    recordToBeUpdated.AdditionalPartDescription = rFQParts.AdditionalPartDesc;
                    recordToBeUpdated.MaterialType = rFQParts.MaterialType;
                    recordToBeUpdated.EstimatedQty = rFQParts.EstimatedQty;
                    recordToBeUpdated.PartWeightKG = rFQParts.PartWeightKG;
                    this.DataContext.SaveChanges();
                    rFQParts.Id = recordToBeUpdated.Id;
                    #endregion

                    #region "Save attachments of rfq parts"
                    MES.Data.Library.PartAttachment dboPartAttachment = null;
                    if (rFQParts.RfqPartAttachmentList != null && rFQParts.RfqPartAttachmentList.Count > 0)
                    {
                        bool AnyPartAttachment = false;
                        string fileName = string.Empty;
                        foreach (var partAttachment in rFQParts.RfqPartAttachmentList)
                        {
                            if (!string.IsNullOrEmpty(partAttachment.AttachmentPathOnServer))
                            {
                                AnyPartAttachment = true;
                                dboPartAttachment = new MES.Data.Library.PartAttachment();
                                dboPartAttachment.RFQPartId = Convert.ToInt32(rFQParts.Id);
                                dboPartAttachment.AttachmentName = partAttachment.AttachmentName;
                                dboPartAttachment.AttachmentDetail = partAttachment.AttachmentDetail ?? string.Empty;
                                fileName = string.Empty;
                                if (!string.IsNullOrEmpty(partAttachment.AttachmentPathOnServer))
                                    fileName = MES.Business.Library.Helper.BlobHelper.GetPhysicalPath(partAttachment.AttachmentPathOnServer);
                                dboPartAttachment.AttachmentPathOnServer = fileName;
                                dboPartAttachment.CreatedBy = dboPartAttachment.UpdatedBy = CurrentUser;
                                dboPartAttachment.CreatedDate = AuditUtils.GetCurrentDateTime();
                                dboPartAttachment.UpdatedDate = AuditUtils.GetCurrentDateTime();
                                this.DataContext.PartAttachments.Add(dboPartAttachment);
                            }
                        }
                        try
                        {

                            if (AnyPartAttachment)
                                this.DataContext.SaveChanges();

                        }
                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {
                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {
                                    throw ex;// ("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                                }
                            }
                        }

                    }
                    #endregion
                    successMsg = Languages.GetResourceText("RFQPartSavedSuccess");
                }
            }
            else
            {
                errMSg = Languages.GetResourceText("RFQNotCreated");
            }
            return SuccessOrFailedResponse<int?>(errMSg, rFQParts.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.RFQ.RFQParts> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int RFQPartId)
        {
            var rfqPartsToBeDeleted = this.DataContext.Parts.Where(a => a.Id == RFQPartId).SingleOrDefault();
            if (rfqPartsToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("RFQPartNotExists"));
            }
            else
            {
                ICollection<MES.Data.Library.PartAttachment> rfqpartattachmentCollection = rfqPartsToBeDeleted.PartAttachments;
                if (rfqpartattachmentCollection.Count > 0)
                {
                    string partfilePath = string.Empty;
                    foreach (MES.Data.Library.PartAttachment rfqPartAttachment in rfqpartattachmentCollection)
                    {
                        partfilePath = rfqPartAttachment.AttachmentPathOnServer;
                        if (File.Exists(partfilePath))
                            File.Delete(partfilePath);
                    }
                }

                rfqPartsToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                rfqPartsToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(rfqPartsToBeDeleted).State = EntityState.Modified;
                rfqPartsToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("RFQPartDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<bool?> DeleteByRFQId(string rfqId)
        {
            var rfqPartsToBeDeleted = this.DataContext.Parts.Where(a => a.RFQId == rfqId).ToList();
            if (rfqPartsToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("RFQPartNotExists"));
            }
            else
            {
                foreach (var item in rfqPartsToBeDeleted)
                {
                    ICollection<MES.Data.Library.PartAttachment> rfqpartattachmentCollection = item.PartAttachments;
                    if (rfqpartattachmentCollection.Count > 0)
                    {
                        string partfilePath = string.Empty;
                        foreach (MES.Data.Library.PartAttachment rfqPartAttachment in rfqpartattachmentCollection)
                        {
                            partfilePath = rfqPartAttachment.AttachmentPathOnServer;
                            if (File.Exists(partfilePath))
                                File.Delete(partfilePath);
                        }
                    }
                    item.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    item.UpdatedBy = CurrentUser;
                    this.DataContext.Entry(item).State = EntityState.Modified;
                    item.IsDeleted = true;
                    this.DataContext.SaveChanges();
                }

                return SuccessBoolResponse(Languages.GetResourceText("RFQPartDeletedSuccess"));
            }
        }


        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQParts>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public List<DTO.Library.RFQ.RFQ.RFQParts> GetRFQPartsList(string rfqId)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQParts> lstRFQParts = new List<DTO.Library.RFQ.RFQ.RFQParts>();
            DTO.Library.RFQ.RFQ.RFQParts rFQParts;
            this.RunOnDB(context =>
            {
                var rfqPartList = context.Parts.Where(c => c.RFQId == rfqId && c.IsDeleted == false).OrderBy(a => a.CreatedDate).ToList();
                if (rfqPartList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    foreach (var item in rfqPartList)
                    {
                        rFQParts = new DTO.Library.RFQ.RFQ.RFQParts();
                        rFQParts.Id = item.Id;
                        rFQParts.RfqId = item.RFQId;
                        rFQParts.CustomerPartNo = item.CustomerPartNo;
                        rFQParts.RevLevel = item.RevLevel;
                        rFQParts.PartDescription = item.PartDescription;
                        rFQParts.AdditionalPartDesc = item.AdditionalPartDescription;
                        rFQParts.MaterialType = item.MaterialType;
                        rFQParts.EstimatedQty = item.EstimatedQty;
                        rFQParts.PartWeightKG = item.PartWeightKG;

                        //RFQ Part Attachments
                        rFQParts.RfqPartAttachmentList = new List<DTO.Library.RFQ.RFQ.RFQPartAttachment>();
                        DTO.Library.RFQ.RFQ.RFQPartAttachment rFQPartAttachments;
                        var rfqPartAttachmentList = context.PartAttachments.Where(c => c.RFQPartId == rFQParts.Id).OrderByDescending(a => a.CreatedDate).ToList();
                        if (rfqPartAttachmentList == null)
                            errMSg = Languages.GetResourceText("RecordNotExist");
                        else
                        {
                            //setup total records
                            foreach (var partAttachmentitem in rfqPartAttachmentList)
                            {
                                rFQPartAttachments = new DTO.Library.RFQ.RFQ.RFQPartAttachment();
                                rFQPartAttachments.Id = partAttachmentitem.Id;
                                rFQPartAttachments.RfqPartId = Convert.ToInt32(rFQParts.Id);
                                rFQPartAttachments.AttachmentName = partAttachmentitem.AttachmentName;
                                rFQPartAttachments.AttachmentDetail = partAttachmentitem.AttachmentDetail;
                                if (!string.IsNullOrEmpty(partAttachmentitem.AttachmentPathOnServer))
                                    rFQPartAttachments.AttachmentPathOnServer = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + partAttachmentitem.AttachmentPathOnServer;
                                else
                                    rFQPartAttachments.AttachmentPathOnServer = partAttachmentitem.AttachmentPathOnServer;
                                rFQParts.RfqPartAttachmentList.Add(rFQPartAttachments);
                            }
                        }
                        lstRFQParts.Add(rFQParts);
                    }
                }
            });
            return lstRFQParts;
        }

        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQParts>> UploadRFQParts(MES.DTO.Library.RFQ.RFQ.RFQ rfq)
        {
            string successMsg = null;

            var context = HttpContext.Current;
            string errMsg = null, action = "Create";
            try
            {
                string ext = Path.GetExtension(rfq.UploadPartFilePath);
                ExcelFile ef = new ExcelFile();
                Stream memoryStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(rfq.UploadPartFilePath);

                if (ext == ".xls")
                {
                    ef.LoadXls(memoryStream);
                }
                else
                {
                    ef.LoadXlsx(memoryStream, XlsxOptions.None);
                }
                ExcelWorksheet ws = ef.Worksheets[0];
                try
                {
                    if (!string.IsNullOrEmpty(rfq.Id))
                    {
                        if (rfq.isRevision)
                            action = "Create";
                        else
                            action = "Update";
                    }
                    // Find the last real row
                    int nLastRow = 0;
                    bool isvalid = true;
                    bool hasError = false;
                    string rfqnos = string.Empty;
                    int i = 1;

                    if (ws.GetUsedCellRange(true) != null && (nLastRow = ws.GetUsedCellRange(true).LastRowIndex) > 0)
                    {
                        for (i = 1; i <= nLastRow; i++)
                        {
                            if (ws.Cells[i, 0].Value == null
                                || ws.Cells[i, 2].Value == null
                                || ws.Cells[i, 5].Value == null)
                            {
                                isvalid = false; break;
                            }
                            else if ((ws.Cells[i, 0].Value != null && ws.Cells[i, 0].Value.ToString().Trim() == "")                              
                                || (ws.Cells[i, 2].Value != null && ws.Cells[i, 2].Value.ToString().Trim() == "")
                                || (ws.Cells[i, 5].Value != null && !Regex.IsMatch(ws.Cells[i, 5].Value.ToString(), @"\d")))
                            {
                                isvalid = false; break;
                            }
                            else if (Regex.IsMatch(ws.Cells[i, 5].Value.ToString(), @"\d") && Convert.ToInt32(ws.Cells[i, 5].Value) <= 0)
                            {
                                isvalid = false; break;
                            }
                        }

                        if (isvalid)
                        {
                            for (i = 1; i <= nLastRow; i++)
                            {
                                Business.Library.BO.RFQ.RFQ.RFQParts rfqPartObj = new Business.Library.BO.RFQ.RFQ.RFQParts();
                                DTO.Library.RFQ.RFQ.RFQParts rfqPartsItem = new DTO.Library.RFQ.RFQ.RFQParts();
                                try
                                {
                                    rfqPartsItem.RfqId = rfq.Id;
                                    if (ws.Cells[i, 0].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 0].Value.ToString().Trim()))
                                        rfqPartsItem.CustomerPartNo = ws.Cells[i, 0].Value.ToString();

                                    if (ws.Cells[i, 1].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 1].Value.ToString().Trim()))
                                        rfqPartsItem.RevLevel = ws.Cells[i, 1].Value.ToString();

                                    if (ws.Cells[i, 2].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 2].Value.ToString().Trim()))
                                        rfqPartsItem.PartDescription = ws.Cells[i, 2].Value.ToString();

                                    if (ws.Cells[i, 3].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 3].Value.ToString().Trim()))
                                        rfqPartsItem.AdditionalPartDesc = ws.Cells[i, 3].Value.ToString();

                                    if (ws.Cells[i, 4].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 4].Value.ToString().Trim()))
                                        rfqPartsItem.MaterialType = ws.Cells[i, 4].Value.ToString();
                                    if (ws.Cells[i, 5].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 5].Value.ToString().Trim()) && Convert.ToInt32(ws.Cells[i, 5].Value) > 0)
                                        rfqPartsItem.EstimatedQty = Convert.ToInt32(ws.Cells[i, 5].Value);
                                    if (ws.Cells[i, 6].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 6].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 6].Value) > 0)
                                        rfqPartsItem.PartWeightKG = Convert.ToDecimal(ws.Cells[i, 6].Value);
                                    else
                                        rfqPartsItem.PartWeightKG = 0;
                                    if (action == "Update")
                                    {
                                        int? ret = rfqPartObj.Save(rfqPartsItem).Result;

                                        if (ret > 0)
                                        {


                                        }
                                        else
                                        {
                                            hasError = true;
                                            if (rfqnos != string.Empty)
                                                rfqnos += ", " + rfqPartsItem.CustomerPartNo;
                                            else rfqnos = rfqPartsItem.CustomerPartNo;
                                        }
                                    }
                                    rfq.lstRFQPart.Add(rfqPartsItem);

                                }
                                catch (Exception ex)
                                {
                                    hasError = true;
                                    if (rfqnos != string.Empty)
                                        rfqnos += ", " + rfqPartsItem.CustomerPartNo;
                                    else rfqnos = rfqPartsItem.CustomerPartNo;
                                }

                            }
                            if (!hasError)
                            {
                                if (action == "Update")
                                {
                                    Business.Library.BO.RFQ.RFQ.RFQ rfqObj = new Business.Library.BO.RFQ.RFQ.RFQ();
                                    DTO.Library.RFQ.RFQ.RFQ rfqItem = rfqObj.FindById(rfq.Id).Result;
                                    rfqObj.CreatePDF(rfqItem);
                                }
                                successMsg = Languages.GetResourceText("RFQPartsSavedSuccess");
                            }
                            else
                            {
                                errMsg = rfqnos + " : " + Languages.GetResourceText("FillReqData");
                            }
                        }
                        else
                        {
                            //if (File.Exists(filepath))
                            //    File.Delete(filepath);
                            errMsg = Languages.GetResourceText("FillReqData");
                        }

                    }
                    else
                    {
                        //if (File.Exists(filepath))
                        //    File.Delete(filepath);

                        errMsg = Languages.GetResourceText("UploadedFileIsBlank");
                    }
                }
                catch (Exception ex) //Error
                {
                    errMsg = ex.ToString();//"Error - Read xls",				
                }
                finally
                {
                    ws = null;
                    ef.ClosePreservedXlsx();
                }
            }
            catch (Exception ex) //Error
            {
                errMsg = ex.ToString();//"Error - Load xls",				
            }


            return SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQParts>>(errMsg, rfq.lstRFQPart, successMsg);
        }

    }
}
