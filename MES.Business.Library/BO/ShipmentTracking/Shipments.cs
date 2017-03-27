using Account.DTO.Library;
using MES.Business.Repositories.ShipmentTracking;
using NPE.Core;
using NPE.Core.Helpers;
using NPE.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using System.Web;
using System.IO;
using Winnovative.ExcelLib;
using MES.DTO.Library.Common;
using MES.Business.Library.Extensions;

namespace MES.Business.Library.BO.ShipmentTracking
{
    class Shipments : ContextBusinessBase, IShipmentsRepository
    {
        public Shipments()
            : base("Shipments")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.ShipmentTracking.Shipments shipments)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.Shipment();
            string filePath = string.Empty;

            if (shipments.Id > 0)
            {
                recordToBeUpdated = this.DataContext.Shipments.Where(a => a.Id == shipments.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("ShipmentsNotExists");
                else
                {

                    #region "Delete Shipments from Shipment.Suppliers for this shipment"
                    var deleteShipmentSuppliersList = this.DataContext.ShipmentSuppliers.Where(a => a.ShipmentId == shipments.Id).ToList();
                    foreach (var item in deleteShipmentSuppliersList)
                    {
                        this.DataContext.ShipmentSuppliers.Remove(item);
                    }
                    #endregion
                    #region "Delete Documents from Shipment.Documents for this shipment"
                    var deleteDocumentsList = this.DataContext.Document1.Where(a => a.ShipmentId == shipments.Id).ToList();
                    foreach (var item in deleteDocumentsList)
                    {
                        this.DataContext.Document1.Remove(item);
                    }
                    #endregion
                    #region "Delete POParts from Shipment.POPart for this shipment"
                    var deletePOPartsList = this.DataContext.POParts.Where(a => a.ShipmentId == shipments.Id).ToList();
                    foreach (var item in deletePOPartsList)
                    {
                        this.DataContext.POParts.Remove(item);
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
                this.DataContext.Shipments.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                #region Save general details
                recordToBeUpdated.Id = shipments.Id;
                recordToBeUpdated.ShipmentMode = shipments.ShipmentModeId;
                recordToBeUpdated.OriginId = shipments.OriginId;
                recordToBeUpdated.EstShpmntDate = shipments.EstShpmntDate;
                recordToBeUpdated.ActShpmntDate = shipments.ActShpmntDate;
                recordToBeUpdated.ETAAtWarehouseAtDest = shipments.ETAAtWarehouseAtDest;
                recordToBeUpdated.ActArrDateAtWarehouseAtDest = shipments.ActArrDateAtWarehouseAtDest;
                recordToBeUpdated.DestinationId = shipments.DestinationId;
                recordToBeUpdated.Status = shipments.Status;
                recordToBeUpdated.InvoiceNumbers = shipments.InvoiceNumbers;
                recordToBeUpdated.ShipmentReviewed = shipments.IsShipmentReviewed;

                recordToBeUpdated.ForwarderId = shipments.ForwarderId;
                recordToBeUpdated.EstForwarderPickupDate = shipments.EstForwarderPickupDate;
                recordToBeUpdated.ActForwarderPickupDate = shipments.ActForwarderPickupDate;
                recordToBeUpdated.ContainerNumber = shipments.ContainerNumber;
                recordToBeUpdated.LateDeliveryToForwarder = shipments.IsLateDeliveryToForwarder;

                recordToBeUpdated.Inspector = shipments.Inspector;
                recordToBeUpdated.InspectionDate = shipments.InspectionDate;

                if (!string.IsNullOrEmpty(shipments.InspectionReportFilePath))
                {
                    filePath = MES.Business.Library.Helper.BlobHelper.GetPhysicalPath(shipments.InspectionReportFilePath);
                }
                recordToBeUpdated.InspectionReportFileName = shipments.InspectionReportFileName;
                recordToBeUpdated.InspectionReportFilePath = filePath;

                recordToBeUpdated.QualityReviewStatus = shipments.QualityReviewStatusId;
                recordToBeUpdated.QualityReviewComments = shipments.QualityReviewComments;
                this.DataContext.SaveChanges();
                shipments.Id = recordToBeUpdated.Id;
                #endregion

                #region "Save ShipmentSuppliers Detail"
                MES.Data.Library.ShipmentSupplier dboShipmentSupplier = null;
                if (shipments.ShipmentSuppliers != null && shipments.ShipmentSuppliers.Count > 0)
                {
                    bool AnyShipmentSuppliers = false;
                    foreach (var shipmentSupplier in shipments.ShipmentSuppliers)
                    {
                        if (Convert.ToInt32(shipmentSupplier.Id) != 0)
                        {
                            AnyShipmentSuppliers = true;
                            dboShipmentSupplier = new MES.Data.Library.ShipmentSupplier();
                            dboShipmentSupplier.ShipmentId = shipments.Id;
                            dboShipmentSupplier.SupplierId = Convert.ToInt16(shipmentSupplier.Id);
                            dboShipmentSupplier.CreatedBy = CurrentUser;
                            dboShipmentSupplier.CreatedDate = AuditUtils.GetCurrentDateTime();
                            this.DataContext.ShipmentSuppliers.Add(dboShipmentSupplier);
                        }
                    }
                    if (AnyShipmentSuppliers)
                        this.DataContext.SaveChanges();
                }
                #endregion

                #region "Save shipment Documents Detail"
                if (shipments.lstDocument != null && shipments.lstDocument.Count > 0)
                {
                    MES.Business.Repositories.ShipmentTracking.IDocumentsRepository objIDocumentsRepository = null;
                    foreach (var objDocument in shipments.lstDocument)
                    {
                        if (objDocument.DocumentTypeId != 0)
                        {
                            objIDocumentsRepository = new MES.Business.Library.BO.ShipmentTracking.Documents();
                            objDocument.ShipmentId = shipments.Id;
                            objIDocumentsRepository.Save(objDocument);
                        }
                    }
                }
                #endregion

                #region "Save shipment POPart Detail"
                if (shipments.lstPOPart != null && shipments.lstPOPart.Count > 0)
                {
                    MES.Business.Repositories.ShipmentTracking.IPOPartsRepository objIPOPartsRepository = null;
                    foreach (var objPOPart in shipments.lstPOPart)
                    {

                        objIPOPartsRepository = new MES.Business.Library.BO.ShipmentTracking.POParts();
                        objPOPart.ShipmentId = shipments.Id;
                        objIPOPartsRepository.Save(objPOPart);

                    }
                }
                #endregion
                successMsg = Languages.GetResourceText("ShipmentsSavedSuccess");
            }

            return SuccessOrFailedResponse<int?>(errMSg, shipments.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.ShipmentTracking.Shipments> FindById(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.ShipmentTracking.Shipments shipments = new DTO.Library.ShipmentTracking.Shipments();
            this.RunOnDB(context =>
            {
                var Shipments = context.Shipments.Where(s => s.Id == id).SingleOrDefault();
                if (Shipments == null)
                    errMSg = Languages.GetResourceText("ShipmentsNotExists");
                else
                {
                    #region Bind general details
                    shipments.Id = Shipments.Id;
                    shipments.ShipmentModeId = Shipments.ShipmentMode;
                    shipments.OriginId = Shipments.OriginId;
                    shipments.EstShpmntDate = Shipments.EstShpmntDate;
                    shipments.ActShpmntDate = Shipments.ActShpmntDate;
                    shipments.ETAAtWarehouseAtDest = Shipments.ETAAtWarehouseAtDest;
                    shipments.ActArrDateAtWarehouseAtDest = Shipments.ActArrDateAtWarehouseAtDest;
                    shipments.DestinationId = Shipments.DestinationId;
                    shipments.Status = Shipments.Status;
                    shipments.InvoiceNumbers = Shipments.InvoiceNumbers;
                    shipments.IsShipmentReviewed = Shipments.ShipmentReviewed;

                    shipments.ForwarderId = Shipments.ForwarderId;
                    shipments.EstForwarderPickupDate = Shipments.EstForwarderPickupDate;
                    shipments.ActForwarderPickupDate = Shipments.ActForwarderPickupDate;
                    shipments.ContainerNumber = Shipments.ContainerNumber;
                    shipments.IsLateDeliveryToForwarder = Shipments.LateDeliveryToForwarder;

                    shipments.Inspector = Shipments.Inspector;
                    shipments.InspectionDate = Shipments.InspectionDate;
                    shipments.InspectionReportFileName = Shipments.InspectionReportFileName;
                    shipments.InspectionReportFilePath = !string.IsNullOrEmpty(Shipments.InspectionReportFilePath) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + Shipments.InspectionReportFilePath : string.Empty;
                    shipments.QualityReviewStatusId = Shipments.QualityReviewStatus;
                    shipments.QualityReviewComments = Shipments.QualityReviewComments;
                    #endregion

                    #region Bind Commodity types
                    shipments.ShipmentSuppliers = new List<MES.DTO.Library.Common.LookupFields>();
                    context.ShipmentSuppliers.Where(a => a.ShipmentId == id).ToList().ForEach(tl => shipments.ShipmentSuppliers.Add(
                        new MES.DTO.Library.Common.LookupFields()
                        {
                            Id = Convert.ToInt32(tl.SupplierId),
                            Name = tl.Supplier.CompanyName
                        }));
                    #endregion

                    #region Bind document details
                    MES.Business.Repositories.ShipmentTracking.IDocumentsRepository objIDocumentsRepository = new MES.Business.Library.BO.ShipmentTracking.Documents();
                    shipments.lstDocument = objIDocumentsRepository.GetDocumentsList(id);
                    #endregion

                    #region Bind PO Part details
                    MES.Business.Repositories.ShipmentTracking.IPOPartsRepository objIPOPartsRepository = new MES.Business.Library.BO.ShipmentTracking.POParts();
                    shipments.lstPOPart = objIPOPartsRepository.GetPOPartsList(id);
                    #endregion
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.ShipmentTracking.Shipments>(errMSg, shipments);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int shipmentsId)
        {
            var ShipmentsToBeDeleted = this.DataContext.Shipments.Where(a => a.Id == shipmentsId).SingleOrDefault();
            if (ShipmentsToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("ShipmentsNotExists"));
            }
            else
            {
                ShipmentsToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                ShipmentsToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(ShipmentsToBeDeleted).State = EntityState.Modified;
                ShipmentsToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("ShipmentsDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.ShipmentTracking.Shipments>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.ShipmentTracking.Shipments>> GetShipmentList(NPE.Core.IPage<DTO.Library.ShipmentTracking.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.ShipmentTracking.Shipments> lstShipments = new List<DTO.Library.ShipmentTracking.Shipments>();
            DTO.Library.ShipmentTracking.Shipments shipments;
            this.RunOnDB(context =>
            {
                var ShipmentsList = context.SearchShipments(paging.Criteria.Id
                    , paging.Criteria.IsLateDeliveryToForwarder
                    , paging.Criteria.ShipmentMode
                    , paging.Criteria.SupplierId
                    , paging.Criteria.OriginId
                    , paging.Criteria.ForwarderId
                    , paging.Criteria.QualityReviewStatusId
                    , paging.Criteria.Status
                    , paging.Criteria.PONumber
                    , paging.Criteria.PartNumber
                    , paging.Criteria.Inspector
                    , paging.Criteria.EstShpmntDateFrom
                    , paging.Criteria.EstShpmntDateTo
                    , paging.Criteria.ETAAtWarehouseAtDestFrom
                    , paging.Criteria.ETAAtWarehouseAtDestTo
                    , paging.Criteria.ActArrDateAtWarehouseAtDestFrom
                    , paging.Criteria.ActArrDateAtWarehouseAtDestTo
                    , paging.Criteria.InspectionDateFrom
                    , paging.Criteria.InspectionDateTo
                    , paging.PageNo, paging.PageSize
                    , totalRecords, "").ToList();
                if (ShipmentsList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in ShipmentsList)
                    {
                        shipments = new DTO.Library.ShipmentTracking.Shipments();
                        shipments.Id = item.Id;
                        shipments.ShipmentMode = item.ShipmentMode;
                        shipments.Origin = item.Origin;
                        shipments.Destination = item.Destination;
                        shipments.SupplierNames = (!string.IsNullOrEmpty(item.Supplier_Name) ? item.Supplier_Name.TrimEnd(',') : string.Empty);
                        shipments.PONumbers = string.IsNullOrEmpty(item.PONumber) ? "-" : item.PONumber.TrimEnd(',');
                        shipments.InvoiceNumbers = string.IsNullOrEmpty(item.InvoiceNumbers) ? "-" : item.InvoiceNumbers;
                        shipments.ContainerNumber = string.IsNullOrEmpty(item.ContainerNumber) ? "-" : item.ContainerNumber;
                        shipments.QualityReviewStatus = string.IsNullOrEmpty(item.QualityReviewStatus) ? "-" : item.QualityReviewStatus;
                        shipments.ShipmentReviewed = string.IsNullOrEmpty(item.ShipmentReviewed) ? "-" : item.ShipmentReviewed;
                        shipments.EstShpmntDate = item.EstShpmntDate;
                        shipments.ETAAtWarehouseAtDest = item.ETAAtWarehouseAtDest;

                        shipments.LateDeliveryToForwarder = item.LateDeliveryToForwarder;
                        shipments.InspectionFileUploaded = item.InspectionFileUploaded;
                        shipments.Documents = string.IsNullOrEmpty(item.Documents) ? "-" : item.Documents.TrimEnd(',');
                        shipments.PartNumber = string.IsNullOrEmpty(item.PartNumber) ? "-" : item.PartNumber.TrimEnd(',');
                        shipments.ActShpmntDate = item.ActShpmntDate;
                        shipments.ActArrDateAtWarehouseAtDest = item.ActArrDateAtWarehouseAtDest;
                        shipments.ForwarderName = item.ForwarderName;
                        shipments.EstForwarderPickupDate = item.EstForwarderPickupDate;
                        shipments.ActForwarderPickupDate = item.ActForwarderPickupDate;
                        shipments.Inspector = item.Inspector;
                        shipments.InspectionDate = item.InspectionDate;
                        shipments.QualityReviewComments = item.QualityReviewComments;

                        lstShipments.Add(shipments);
                    }
                }
            });

            var response = SuccessOrFailedResponse<List<MES.DTO.Library.ShipmentTracking.Shipments>>(errMSg, lstShipments);
            response.PageInfo = paging.ToPage();
            return response;
        }

        public ITypedResponse<bool?> DownloadShipment()
        {
            var context = HttpContext.Current;
            string directoryPath = context.Server.MapPath(Constants.SHIPMENTTEMPLATE),
                   filePath = directoryPath + "ShipmentTemplate.xls";

            if (System.IO.Directory.Exists(directoryPath))
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        System.IO.FileStream sourceXlsDataStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                        ExcelWorkbook ew = new ExcelWorkbook(sourceXlsDataStream);
                        ew.LicenseKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["wnvkey"]);
                        ExcelWorksheet ws = ew.Worksheets[1];

                        if (ws.UsedRangeCells.Count > 0)
                            ws.UsedRange.Clear();

                        //1. Shipment Mode
                        ws[1, 1].Value = "SHIPMENT MODE";
                        ws[3, 1].Value = "Air";
                        ws[4, 1].Value = "Land";
                        ws[5, 1].Value = "Sea - Container";
                        ws[6, 1].Value = "Sea - LCL";
                        ws[7, 1].Value = "Sea - LCL - Express";
                        ExcelRange er = ws[3, 1, 7, 1];
                        ew.NamedRanges["ShipmentMode"].Range = er;

                        int counter = 3, count = 0;
                        //2. Origin

                        var origins = this.DataContext.Origins.ToList();
                        if (origins != null && origins.Count > 0)
                        {
                            ws[1, 3].Value = "ORIGIN";
                            count = origins.Count + 1;
                            foreach (Data.Library.Origin item in origins)
                            {
                                ws[counter, 3].Value = item.Origin1; counter++;
                            }
                            er = ws[3, 3, count, 3];
                            ew.NamedRanges["Origin"].Range = er;
                        }

                        counter = 3;
                        //3. Destination
                        var destinations = this.DataContext.Destinations.ToList();
                        if (destinations != null && destinations.Count > 0)
                        {
                            ws[1, 5].Value = "DESTINATION";
                            count = destinations.Count + 1;
                            foreach (Data.Library.Destination item in destinations)
                            {
                                ws[counter, 5].Value = item.Destination1; counter++;
                            }
                            er = ws[3, 5, count, 5];
                            ew.NamedRanges["Destination"].Range = er;
                        }

                        //4. Status
                        ws[1, 7].Value = "STATUS";
                        ws[3, 7].Value = "Open";
                        ws[4, 7].Value = "Completed";
                        er = ws[3, 7, 4, 7];
                        ew.NamedRanges["Status"].Range = er;

                        counter = 3;
                        //5. Forwarder
                        var forwarders = this.DataContext.Forwarders.ToList();
                        if (forwarders.Count > 0)
                        {
                            ws[1, 9].Value = "FORWARDER NAME";
                            count = forwarders.Count + 1;
                            foreach (var item in forwarders)
                            {
                                ws[counter, 9].Value = item.ForwarderName; counter++;
                            }
                            er = ws[3, 9, count, 9];
                            ew.NamedRanges["ForwarderName"].Range = er;
                        }

                        counter = 3;
                        //6. Supplier Name

                        var suppliers = this.DataContext.Suppliers.OrderByDescending(item => item.IsCurrentSupplier).OrderBy(item => item.CompanyName).ToList();
                        if (suppliers.Count > 0)
                        {
                            ws[1, 11].Value = "SUPPLIER NAME";
                            count = suppliers.Count + 1;
                            foreach (var item in suppliers)
                            {
                                ws[counter, 11].Value = item.CompanyName; counter++;
                            }
                            er = ws[3, 11, count, 11];
                            ew.NamedRanges["SupplierName"].Range = er;
                        }

                        //7. Quality Review Status
                        ws[1, 13].Value = "QUALITY REVIEW STATUS";
                        ws[3, 13].Value = "Not Reviewed";
                        ws[4, 13].Value = "Approved";
                        ws[5, 13].Value = "Conditionally Approved";
                        ws[6, 13].Value = "Rejected";
                        er = ws[3, 13, 6, 13];
                        ew.NamedRanges["QualityReviewStatus"].Range = er;

                        //8. Shipment Review
                        ws[1, 15].Value = "SHIPMENT REVIEWED";
                        ws[3, 15].Value = "Yes";
                        ws[4, 15].Value = "No";
                        er = ws[3, 15, 4, 15];
                        ew.NamedRanges["ShipmentReviewed"].Range = er;

                        //9. Late Delivery to forwarder
                        ws[1, 17].Value = "LATE DELIVERY TO FORWARDER";
                        ws[3, 17].Value = "Yes";
                        ws[4, 17].Value = "No";
                        er = ws[3, 17, 4, 17];
                        ew.NamedRanges["LateDelivery"].Range = er;

                        ws.WorksheetSecurity.ProtectWorksheet(System.Configuration.ConfigurationManager.AppSettings["ShipmentTemplatePswd"]);

                        ws = ew.Worksheets[0];
                        ws[19, 2].Value = "No";
                        ws[13, 5].Value = "No";
                        ws[17, 2].Value = "Open";
                        ws[27, 2].Value = "Not Reviewed";
                        ws.WorksheetSecurity.ProtectWorksheet(System.Configuration.ConfigurationManager.AppSettings["ShipmentTemplatePswd"]);

                        try
                        {
                            string generatedFileName = "ShipmentTracking.xls";
                            string filepath = context.Server.MapPath(Constants.SHIPMENTPhyFOLDER) + generatedFileName;

                            if (!System.IO.Directory.Exists(context.Server.MapPath(Constants.SHIPMENTPhyFOLDER)))
                                System.IO.Directory.CreateDirectory(context.Server.MapPath(Constants.SHIPMENTPhyFOLDER));
                            else
                            {
                                if (File.Exists(filepath))
                                {
                                    File.Delete(filepath);
                                }
                            }
                            ew.Save(filepath);
                            filePath = System.Configuration.ConfigurationManager.AppSettings["ApiURL"] + (Constants.SHIPMENTFOLDER) + generatedFileName;
                        }
                        catch (Exception ex) //Error
                        {
                            throw ex;
                        }
                        finally
                        {
                            sourceXlsDataStream.Close();
                            ew.Close();

                        }
                    }
                    catch (Exception ex) //Error
                    {
                        throw ex;
                    }
                }
                else
                    return FailedBoolResponse("Shipment Template does not exists.");
            }
            return SuccessBoolResponse(filePath);
        }

        public NPE.Core.ITypedResponse<int?> UploadShipment(string uploadShipmentFilePath)
        {
            string errMSg = null;
            string successMsg = null;
            int shipmentId = 0;
            var context = HttpContext.Current;

            string directoryPath = context.Server.MapPath(Constants.SHIPMENTTEMPLATE);
            bool isValid = true, isDateValid = true;
            if (System.IO.Directory.Exists(directoryPath))
            {
                string filePath = uploadShipmentFilePath;
                Stream memoryStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(filePath);

                ExcelWorkbook ew = new ExcelWorkbook(memoryStream);
                ew.LicenseKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["wnvkey"]);
                ExcelWorksheet ws = ew.Worksheets[0];
                try
                {
                    DTO.Library.ShipmentTracking.Shipments shipmentObject = new DTO.Library.ShipmentTracking.Shipments();
                    if (ws.UsedRangeCells.Count == 0)
                    {
                        errMSg = "The uploaded file is blank. Please download the Shipment file again and fill in the required values and upload it.";
                    }
                    else if (ws[10, 1].Value.ToString() != "SHIPMENT MODE")
                    {
                        errMSg = "It seems that in this uploaded file, the information, other than the required ones have been modified.<br />Please download the Shipment file again and fill in the required values and upload it.";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(ws[10, 2].Value.ToString())//Shipment Mode
                           || string.IsNullOrEmpty(ws[11, 2].Value.ToString())//Origin
                           || string.IsNullOrEmpty(ws[16, 2].Value.ToString())//Destination  
                           || string.IsNullOrEmpty(ws[20, 2].Value.ToString())//Supplier Name
                           || string.IsNullOrEmpty(ws[10, 5].Value.ToString()))//Forwarder Name
                        {
                            errMSg = "Please fill in the required values and upload the file again.";
                        }
                        else
                        {
                            //Shipment Mode
                            if (!string.IsNullOrEmpty(ws[10, 2].Value.ToString()))
                            {
                                short shmptMode = 0;
                                switch (ws[10, 2].Value.ToString())
                                {
                                    case "Air":
                                        shmptMode = 1;
                                        break;
                                    case "Land":
                                        shmptMode = 2;
                                        break;
                                    case "Sea - Container":
                                        shmptMode = 3;
                                        break;
                                    case "Sea - LCL":
                                        shmptMode = 4;
                                        break;
                                    case "Sea - LCL - Express":
                                        shmptMode = 5;
                                        break;
                                    default:
                                        break;
                                }
                                shipmentObject.ShipmentModeId = shmptMode;
                                shipmentObject.ShipmentMode = ws[10, 2].Value.ToString();
                            }
                            //Origin
                            if (!string.IsNullOrEmpty(ws[11, 2].Value.ToString()))
                            {
                                string originXl = ws[11, 2].Value.ToString();
                                this.RunOnDB(dcontext =>
                                    {
                                        var origin = dcontext.Origins.Where(item => item.Origin1 == originXl).SingleOrDefault();
                                        if (origin != null && origin.Id > 0)
                                        {
                                            shipmentObject.OriginId = origin.Id;
                                            shipmentObject.Origin = origin.Origin1;
                                        }
                                        else
                                            isValid = false;
                                    });
                            }
                            DateTime datetime;
                            if (!string.IsNullOrEmpty(ws[12, 2].Value.ToString()))
                            {
                                if (DateTime.TryParse(ws[12, 2].Value.ToString(), out datetime))
                                    shipmentObject.EstShpmntDate = !string.IsNullOrEmpty(ws[12, 2].Value.ToString()) ? Convert.ToDateTime(ws[12, 2].Value.ToString()) : (DateTime?)null;
                                else
                                    isDateValid = false;
                            }
                            if (!string.IsNullOrEmpty(ws[13, 2].Value.ToString()))
                            {
                                if (DateTime.TryParse(ws[13, 2].Value.ToString(), out datetime))
                                    shipmentObject.ActShpmntDate = !string.IsNullOrEmpty(ws[13, 2].Value.ToString()) ? Convert.ToDateTime(ws[13, 2].Value.ToString()) : (DateTime?)null;
                                else
                                    isDateValid = false;
                            }
                            if (!string.IsNullOrEmpty(ws[14, 2].Value.ToString()))
                            {
                                if (DateTime.TryParse(ws[14, 2].Value.ToString(), out datetime))
                                    shipmentObject.ETAAtWarehouseAtDest = !string.IsNullOrEmpty(ws[14, 2].Value.ToString()) ? Convert.ToDateTime(ws[14, 2].Value.ToString()) : (DateTime?)null;
                                else
                                    isDateValid = false;
                            }
                            if (!string.IsNullOrEmpty(ws[15, 2].Value.ToString()))
                            {
                                if (DateTime.TryParse(ws[15, 2].Value.ToString(), out datetime))
                                    shipmentObject.ActArrDateAtWarehouseAtDest = !string.IsNullOrEmpty(ws[15, 2].Value.ToString()) ? Convert.ToDateTime(ws[15, 2].Value.ToString()) : (DateTime?)null;
                                else
                                    isDateValid = false;
                            }

                            //Destination
                            if (!string.IsNullOrEmpty(ws[16, 2].Value.ToString()))
                            {
                                string destinationXl = ws[16, 2].Value.ToString();
                                var destination = this.DataContext.Destinations.Where(item => item.Destination1 == destinationXl).SingleOrDefault();
                                if (destination != null && destination.Id > 0)
                                {
                                    shipmentObject.DestinationId = (short)destination.Id;
                                    shipmentObject.Destination = destination.Destination1;
                                }
                                else
                                    isValid = false;
                            }
                            //Status
                            if (!string.IsNullOrEmpty(ws[17, 2].Value.ToString()))
                            {
                                bool status = false;
                                switch (ws[17, 2].Value.ToString())
                                {
                                    case "Open":
                                        status = false;
                                        break;
                                    case "Completed":
                                        status = true;
                                        break;
                                    default:
                                        break;
                                }
                                shipmentObject.Status = status;
                            }
                            //Invoice Numbers
                            if (!string.IsNullOrEmpty(ws[18, 2].Value.ToString()))
                                shipmentObject.InvoiceNumbers = ws[18, 2].Value.ToString();

                            //Shipment Review Status
                            if (!string.IsNullOrEmpty(ws[19, 2].Value.ToString()))
                            {
                                bool shpmtRev = false;
                                switch (ws[19, 2].Value.ToString())
                                {
                                    case "Yes":
                                        shpmtRev = true;
                                        break;
                                    case "No":
                                        shpmtRev = false;
                                        break;
                                    default:
                                        break;
                                }
                                shipmentObject.IsShipmentReviewed = shpmtRev;
                            }
                            //Suppliers
                            if (!string.IsNullOrEmpty(ws[20, 2].Value.ToString()))
                            {
                                string supplierList = ws[20, 2].Value.ToString();
                                List<LookupFields> lstShpmtSupplier = new List<LookupFields>();
                                foreach (string supplieritem in supplierList.Split('\n'))
                                {
                                    LookupFields sItem = new LookupFields();

                                    var supplier = this.DataContext.Suppliers.Where(item => item.CompanyName == supplieritem.Trim()).SingleOrDefault();
                                    if (supplier != null && supplier.Id > 0)
                                    {
                                        sItem.Id = supplier.Id;
                                        lstShpmtSupplier.Add(sItem);
                                    }
                                    else
                                        isValid = false;
                                }
                                shipmentObject.ShipmentSuppliers = lstShpmtSupplier;
                            }
                            //Forwarder
                            if (!string.IsNullOrEmpty(ws[10, 5].Value.ToString()))
                            {
                                string forwarderXl = ws[10, 5].Value.ToString();
                                var forwarder = this.DataContext.Forwarders.Where(item => item.ForwarderName == forwarderXl).SingleOrDefault();
                                if (forwarder != null && forwarder.Id > 0)
                                {
                                    shipmentObject.ForwarderId = forwarder.Id;
                                    shipmentObject.ForwarderName = forwarder.ForwarderName;
                                }
                                else
                                    isValid = false;
                            }

                            if (!string.IsNullOrEmpty(ws[11, 5].Value.ToString()))
                            {
                                if (DateTime.TryParse(ws[11, 5].Value.ToString(), out datetime))
                                    shipmentObject.EstForwarderPickupDate = Convert.ToDateTime(ws[11, 5].Value.ToString());
                                else
                                    isDateValid = false;
                            }
                            if (!string.IsNullOrEmpty(ws[12, 5].Value.ToString()))
                            {
                                if (DateTime.TryParse(ws[12, 5].Value.ToString(), out datetime))
                                    shipmentObject.ActForwarderPickupDate = Convert.ToDateTime(ws[12, 5].Value.ToString());
                                else
                                    isDateValid = false;
                            }
                            //Late Delivery to Forwarder
                            if (!string.IsNullOrEmpty(ws[13, 5].Value.ToString()))
                            {
                                bool latedel = false;
                                switch (ws[13, 5].Value.ToString())
                                {
                                    case "Yes":
                                        latedel = true;
                                        break;
                                    case "No":
                                        latedel = false;
                                        break;
                                    default:
                                        break;
                                }
                                shipmentObject.IsLateDeliveryToForwarder = latedel;
                            }

                            if (!string.IsNullOrEmpty(ws[14, 5].Value.ToString()))
                                shipmentObject.ContainerNumber = ws[14, 5].Value.ToString();

                            if (!string.IsNullOrEmpty(ws[25, 2].Value.ToString()))
                                shipmentObject.Inspector = ws[25, 2].Value.ToString();

                            if (!string.IsNullOrEmpty(ws[26, 2].Value.ToString()))
                            {
                                if (DateTime.TryParse(ws[26, 2].Value.ToString(), out datetime))
                                    shipmentObject.InspectionDate = Convert.ToDateTime(ws[26, 2].Value.ToString());
                                else
                                    isDateValid = false;
                            }
                            //Quality Review Status
                            if (!string.IsNullOrEmpty(ws[27, 2].Value.ToString()))
                            {
                                short qrs = 0;
                                switch (ws[27, 2].Value.ToString())
                                {
                                    case "Not Reviewed":
                                        qrs = 1;
                                        break;
                                    case "Approved":
                                        qrs = 2;
                                        break;
                                    case "Conditionally Approved":
                                        qrs = 3;
                                        break;
                                    case "Rejected":
                                        qrs = 4;
                                        break;
                                    default:
                                        break;
                                }
                                shipmentObject.QualityReviewStatusId = qrs;
                            }
                            //Quality Review Comments
                            if (!string.IsNullOrEmpty(ws[28, 2].Value.ToString()))
                                shipmentObject.QualityReviewComments = ws[28, 2].Value.ToString();
                            shipmentObject.CreatedBy = CurrentUser;
                            bool isPartValid = true;
                            if (isValid && isDateValid)
                            {
                                shipmentObject.Id = shipmentId = Save(shipmentObject).Result.Value;
                                if (shipmentId > 0)
                                {
                                    //PO Parts List
                                    int lastRow = ws.UsedRangeRows.Count;

                                    List<MES.DTO.Library.ShipmentTracking.POParts> lstPOParts = new List<DTO.Library.ShipmentTracking.POParts>();
                                    MES.DTO.Library.ShipmentTracking.POParts popartItem;
                                    for (int i = 33; i <= lastRow; i++)
                                    {
                                        if (!string.IsNullOrEmpty(ws[i, 1].Value.ToString()))
                                        {
                                            popartItem = new DTO.Library.ShipmentTracking.POParts();
                                            popartItem.ShipmentId = shipmentId;
                                            popartItem.PONumber = ws[i, 1].Value.ToString();
                                            popartItem.PartNumber = ws[i, 2].Value.ToString();
                                            int partQuantity;
                                            if (int.TryParse(ws[i, 3].Value.ToString(), out partQuantity))
                                                popartItem.PartQuantity = int.Parse(ws[i, 3].Value.ToString());
                                            else
                                            {
                                                isPartValid = false;
                                                continue;
                                            }
                                            popartItem.PartDescription = ws[i, 4].Value.ToString();
                                            popartItem.CreatedBy = CurrentUser;
                                            lstPOParts.Add(popartItem);
                                        }
                                        else
                                            break;
                                    }
                                    shipmentObject.lstPOPart = lstPOParts;

                                    if (!isPartValid)
                                        errMSg += "<br />PO Parts Quantity should be numeric. The PO part(s) couldn't be saved.";
                                    else
                                        if (shipmentObject.lstPOPart != null && shipmentObject.lstPOPart.Count > 0)
                                        {
                                            if (Save(shipmentObject).StatusCode == 200)
                                                successMsg = "Shipment Tracking record has been saved successfully.";
                                        }

                                    //bool ret = SendIntimationEmail(shipmentObject, shipmentId);
                                }
                            }
                            else
                            {
                                if (!isDateValid)
                                    errMSg = "It seems that the uploaded file doesnot contain valid Date values.<br />Please enter a Valid Date.";
                                else
                                    errMSg = "It seems that the uploaded file doesnot contain valid data. Please enter valid data.";

                            }
                        }
                    }
                }
                catch (Exception ex) //Error
                {
                    throw ex;
                }
                finally
                {
                    ew.Close();
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
            }

            return SuccessOrFailedResponse<int?>(errMSg, shipmentId, successMsg);
        }

        private bool SendIntimationEmail(DTO.Library.ShipmentTracking.Shipments shipmentObject, int shipmentId)
        {
            bool IsSuccess = false;
            try
            {
                MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
                MES.DTO.Library.Common.EmailData emailData = new DTO.Library.Common.EmailData();

                List<string> lstToAddress = new List<string>();


                string POListstr = string.Empty;

                int counter = 0;
                if (shipmentObject.lstPOPart != null)
                {
                    foreach (DTO.Library.ShipmentTracking.POParts item in shipmentObject.lstPOPart)
                    {
                        if (counter % 2 == 0)
                        {
                            POListstr += string.Format(@"<tr>
                                                <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{0}</font></td>
                                                <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{1}</font></td>
                                                <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{2}</font></td>
                                                <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{3}</font></td>
                                                </tr>"
                                                    , item.PONumber
                                                    , item.PartNumber
                                                    , item.PartQuantity
                                                    , item.PartDescription);
                        }
                        else
                        {
                            POListstr += string.Format(@"<tr>
                                                <td bgcolor='#f6f4f2'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{0}</font></td>
                                                <td bgcolor='#f6f4f2'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{1}</font></td>
                                                <td bgcolor='#f6f4f2'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{2}</font></td>
                                                <td bgcolor='#f6f4f2'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{3}</font></td>
                                                </tr>"
                                                    , item.PONumber
                                                    , item.PartNumber
                                                    , item.PartQuantity
                                                    , item.PartDescription);
                        }
                    }
                }
                string shipmentMode = shipmentObject.ShipmentMode;

                string supplier = string.Empty;
                var lstSupplier = this.DataContext.ShipmentSuppliers.Where(item => item.ShipmentId == shipmentId).ToList();

                foreach (Data.Library.ShipmentSupplier supplierItem in lstSupplier)
                {
                    supplier += supplierItem.Supplier.CompanyName + ",<br />";
                }
                supplier = supplier.Substring(0, supplier.Length - 7);
                string originVar = string.Empty;
                originVar = shipmentObject.Origin;

                string destinationVar = string.Empty;
                destinationVar = shipmentObject.Destination;

                string forwarderVar = string.Empty;
                forwarderVar = shipmentObject.ForwarderName;

                emailData.EmailBody = GetRfqEmailBody("ShipmentIntimation.htm")
                    .Replace("<%LoggedinUser%>", CurrentUserInfo.GetName())
                    .Replace("<%ShipmentId%>", shipmentId.ToString())
                    .Replace("<%ShipmentMode%>", shipmentMode)
                    .Replace("<%Origin%>", originVar)
                    .Replace("<%EstShpmtDate%>", shipmentObject.EstShpmntDate.HasValue ? shipmentObject.EstShpmntDate.Value.ToString("dd-MMM-yy") : string.Empty)
                    .Replace("<%ActShpmtDate%>", shipmentObject.ActShpmntDate.HasValue ? shipmentObject.ActShpmntDate.Value.ToString("dd-MMM-yy") : string.Empty)
                    .Replace("<%ETADest%>", shipmentObject.ETAAtWarehouseAtDest.HasValue ? shipmentObject.ETAAtWarehouseAtDest.Value.ToString("dd-MMM-yy") : string.Empty)
                    .Replace("<%ActArrDateDest%>", shipmentObject.ActArrDateAtWarehouseAtDest.HasValue ? shipmentObject.ActArrDateAtWarehouseAtDest.Value.ToString("dd-MMM-yy") : string.Empty)
                    .Replace("<%Supplier%>", supplier)
                    .Replace("<%Destination%>", destinationVar)
                    .Replace("<%Status%>", (shipmentObject.Status) ? "Completed" : "Open")
                    .Replace("<%ShipmentReviewed%>", shipmentObject.ShipmentReviewed == "true" ? "YES" : "NO")
                    .Replace("<%InvoiceNumbers%>", shipmentObject.InvoiceNumbers)
                    .Replace("<%Forwarder%>", forwarderVar)
                    .Replace("<%EstFrwdPickupDate%>", shipmentObject.EstForwarderPickupDate.HasValue ? shipmentObject.EstForwarderPickupDate.Value.ToString("dd-MMM-yy") : string.Empty)
                    .Replace("<%ActFrwdPickupDate%>", shipmentObject.ActForwarderPickupDate.HasValue ? shipmentObject.ActForwarderPickupDate.Value.ToString("dd-MMM-yy") : string.Empty)
                    .Replace("<%LateDel%>", shipmentObject.LateDeliveryToForwarder == "true" ? "YES" : "NO")
                    .Replace("<%ContainerNumber%>", shipmentObject.ContainerNumber)
                    .Replace("<%POListstr%>", POListstr)
                    .Replace("<%addStyle%>", !string.IsNullOrEmpty(POListstr) ? "style='display:block'" : "style='display:none'");

                emailData.EmailSubject = "Shipment Intimation E-mail";
                lstToAddress.Add(System.Configuration.ConfigurationManager.AppSettings["ShipmentIntimationEmail"]);

                emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailData.EmailBody, out IsSuccess, null);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return IsSuccess;
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

        public NPE.Core.ITypedResponse<bool?> exportToExcelShipmentReport(NPE.Core.IPage<MES.DTO.Library.ShipmentTracking.SearchCriteria> searchCriteria)
        {
            string filePath = string.Empty;
            try
            {
                filePath = CreateExcelForShipmentReport(searchCriteria);
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private string CreateExcelForShipmentReport(NPE.Core.IPage<MES.DTO.Library.ShipmentTracking.SearchCriteria> searchCriteria)
        {
            //export all report
            searchCriteria.PageNo = 1;
            searchCriteria.PageSize = int.MaxValue;

            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();
            List<DTO.Library.ShipmentTracking.Shipments> lstShipments = new List<DTO.Library.ShipmentTracking.Shipments>();
            lstShipments = GetShipmentList(searchCriteria).Result;

            strBodyContent.AppendLine("<table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("<thead>");
            strBodyContent.AppendLine("    <tr>");
            strBodyContent.AppendLine("        <td  align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='21'>");
            strBodyContent.AppendLine("            <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("            <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("            </font>");
            strBodyContent.AppendLine("        </td>");
            strBodyContent.AppendLine("    </tr>");
            strBodyContent.AppendLine("    <tr style=''>");

            strBodyContent.AppendLine("<th width='150' height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Shipment Number</th> ");
            strBodyContent.AppendLine("<th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Shipment Mode</th>");
            strBodyContent.AppendLine("<th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Origin</th>");
            strBodyContent.AppendLine("<th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Estimated Sail Date</th>");
            strBodyContent.AppendLine("<th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Actual Sail Date</th>");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>ETA at Warehouse at Destination</th>");
            strBodyContent.AppendLine("<th width='350' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Actual Arrival Date at Warehouse at Destination</th>");
            strBodyContent.AppendLine("<th width='350' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Destination</th>");
            strBodyContent.AppendLine("<th width='350' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Supplier Names</th>");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Forwarder Name</th>");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Est Forwarder Pick-up Date</th>");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Actual Forwarder Pick-up Date</th>");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Late Delivery to Forwarder?</th>");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Documents Attached</th>");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Inspector</th>");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Inspection Date</th>");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Inspection File Uploaded?</th>");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Quality Review Status</th>");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Quality Review Comments</th>");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>PO Numbers</th>");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Part Numbers</th>");

            strBodyContent.AppendLine("    </tr>");
            strBodyContent.AppendLine("</thead>");
            #endregion
            strBodyContent.AppendLine("<tbody>");
            #region Main body loop
            foreach (var item in lstShipments)
            {
                strBodyContent.AppendLine("<tr>");

                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>   ");
                strBodyContent.AppendLine(Convert.ToString(item.Id));
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.ShipmentMode);
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.Origin);
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.EstShpmntDate.HasValue ? item.EstShpmntDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.ActShpmntDate.HasValue ? item.ActShpmntDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.ETAAtWarehouseAtDest.HasValue ? item.ETAAtWarehouseAtDest.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.ActArrDateAtWarehouseAtDest.HasValue ? item.ActArrDateAtWarehouseAtDest.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.Destination);
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.SupplierNames);
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.ForwarderName);
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.EstForwarderPickupDate.HasValue ? item.EstForwarderPickupDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.ActForwarderPickupDate.HasValue ? item.ActForwarderPickupDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.LateDeliveryToForwarder);
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.Documents);
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.Inspector);
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.InspectionDate.HasValue ? item.InspectionDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.InspectionFileUploaded);
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.QualityReviewStatus);
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.QualityReviewComments);
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.PONumbers);
                strBodyContent.AppendLine("</td>                                                                                   ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;' > ");
                strBodyContent.AppendLine(item.PartNumber);
                strBodyContent.AppendLine("</td>                                                                                   ");

                strBodyContent.AppendLine("</tr>");
            }
            #endregion
            strBodyContent.AppendLine("</tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "Shipment Report.xls", stringStream);
            }
            return filepath;
        }
    }
}
