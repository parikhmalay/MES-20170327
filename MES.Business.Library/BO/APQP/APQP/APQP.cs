using Account.DTO.Library;
using MES.Business.Repositories.APQP.APQP;
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
using System.Reflection.Emit;
using MES.Business.Mapping.Extensions;
using System.Web;
using GemBox.Spreadsheet;
using EvoPdf.HtmlToPdf;
using Ionic.Zip;
using System.IO;
using System.Net;
using System.Xml;
using MES.Business.Library.Extensions;
using MES.DTO.Library.Identity;
using MES.Business.Library.Enums;
using MES.Business.Library.BO.DocuSign;
using Newtonsoft.Json.Linq;
using System.Xml.Serialization;
using DocuSign.eSign.Model;
using MES.Business.Repositories.UserManagement;

namespace MES.Business.Library.BO.APQP.APQP
{
    public class APQP : ContextBusinessBase, IAPQPRepository
    {
        public APQP()
            : base("APQP")
        { }

        #region Save apqp items list
        public NPE.Core.ITypedResponse<bool?> SaveAPQPItemList(List<DTO.Library.APQP.APQP.APQP> lstAPQP)
        {
            if (lstAPQP != null && lstAPQP.Count > 0)
            {
                foreach (var aPQP in lstAPQP)
                {
                    var obj = Save(aPQP);
                    if (obj == null || obj.StatusCode != 200)
                    {
                        return FailedBoolResponse(Languages.GetResourceText("APQPSaveFail"));
                    }
                }
            }
            return SuccessBoolResponse(Languages.GetResourceText("APQPSavedSuccess"));
        }
        #endregion

        #region APQP Tab Wise Data Export
        public NPE.Core.ITypedResponse<bool?> APQPTabWiseDataExport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.SearchCriteria> searchCriteria)
        {
            #region don't consider page size and page number, export all on the basis of search criteria
            searchCriteria.PageNo = 1;
            searchCriteria.PageSize = int.MaxValue;
            #endregion
            string filePath = string.Empty;
            try
            {
                List<DTO.Library.APQP.APQP.APQP> lstAPQP = new List<DTO.Library.APQP.APQP.APQP>();
                lstAPQP = GetAPQPList(searchCriteria).Result;
                switch (searchCriteria.Criteria.SectionName)
                {
                    case "APQPSTEP1":
                        filePath = CreateExcelForAPQPKickOff(lstAPQP);
                        break;
                    case "APQPSTEP2":
                        filePath = CreateExcelForAPQPToolingLaunch(lstAPQP);
                        break;
                    case "APQPSTEP3":
                        filePath = CreateExcelForAPQPProjectTracking(lstAPQP);
                        break;
                    case "APQPSTEP4":
                        filePath = CreateExcelForAPQPPPAPSubmission(lstAPQP);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private string CreateExcelForAPQPKickOff(List<DTO.Library.APQP.APQP.APQP> lstAPQP)
        {
            string filepath = string.Empty;
            bool HasPricingFieldsAccess = true;
            StringBuilder strBodyContent = new StringBuilder();
            int noOfColumns = 42;
            if (lstAPQP.Count > 0)
            {
                if (!lstAPQP[0].HasPricingFieldsAccess)
                {
                    noOfColumns = 38;
                    HasPricingFieldsAccess = false;
                }
            }

            strBodyContent.AppendLine("<table id='tblId' style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("    <thead>");
            strBodyContent.AppendLine("        <tr>");
            strBodyContent.AppendLine("            <td align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='" + Convert.ToString(noOfColumns) + "'>");
            strBodyContent.AppendLine("                <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("                <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("                </font>");
            strBodyContent.AppendLine("            </td>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("        <tr style=''>");
            strBodyContent.AppendLine("           <th  height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Part #</th>");
            strBodyContent.AppendLine("           <th width='250' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Rev Level</th>");
            strBodyContent.AppendLine("           <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Part Description</th>");
            strBodyContent.AppendLine("           <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Revision Date</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Drawing Number</th>");
            strBodyContent.AppendLine("           <th width='200' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>RFQ #</th>");
            strBodyContent.AppendLine("           <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Commodity</th>");
            strBodyContent.AppendLine("           <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Process</th>");
            strBodyContent.AppendLine("           <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Other / Special Requirements</th>");
            strBodyContent.AppendLine("           <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Quote #</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Project Name</th>  ");
            strBodyContent.AppendLine("           <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Project Kickoff Date (dd-MMM-yy)</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>MES SAM</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Customer Tooling P.O. Rec'd Date (dd-MMM-yy)</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Customer Tooling PO Number</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Customer</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Customer Manufacturing Location</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Customer Purchasing Contact</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Customer Purchasing Contact Email</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Customer Purchasing Contact Phone</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Customer Engineer</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Customer Engineer Email</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Customer Engineer Phone</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>PPAP Submission Level</th>   ");
            //strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Supplier</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Manufacturer</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Manufacturer City</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Manufacturer State</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Manufacturer Country</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>EAU (Usage)</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Material Type</th>          ");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Part Weight(KG)</th>            ");
            if (HasPricingFieldsAccess)
            {
                strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Purchase Piece Cost(USD)</th>     ");
                strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Purchase Tooling Cost(USD)</th>   ");
                strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Selling Piece Price(USD)</th>     ");
                strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Selling Tooling Price(USD)</th>   ");
            }
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Tooling Leadtime Days</th>   ");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>MES Project Lead / APQP Engineer</th>        ");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Supply Chain Coordinator</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>MES Warehouse</th>          ");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Part Classification</th>    ");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Number Of Samples Required</th>");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Project Notes/Comments</th>          ");
            strBodyContent.AppendLine("           <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Samples Ship-To Location</th> ");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("    </thead>");
            #endregion
            strBodyContent.AppendLine("    <tbody>");
            #region main body loop
            foreach (var item in lstAPQP)
            {
                strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.PartNumber);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objToolingLaunch.RevLevel);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.PartDesc);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objToolingLaunch.RevisionDate.HasValue ? item.objToolingLaunch.RevisionDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.DrawingNumber);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.RFQNumber);
                strBodyContent.AppendLine("</td>                                                                                               ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.Commodity);
                strBodyContent.AppendLine("</td>                                                                                               ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.Process);
                strBodyContent.AppendLine("</td>                                                                                                 ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.AdditionalPartDescription);
                strBodyContent.AppendLine("</td>                                                                                          ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.QuoteNumber);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.ProjectName);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.ProjectKickoffDate.HasValue ? item.objKickOff.ProjectKickoffDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.SAMUser);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.CustomerToolingPOAuthRcvdDate.HasValue ? item.objKickOff.CustomerToolingPOAuthRcvdDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.CustomerToolingPONumber);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.CustomerName);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.CustomerManufacturingLocation);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.CustomerProjectLead);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.CustomerProjectLeadEmail);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.CustomerProjectLeadPhone);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.CustomerEngineer);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.CustomerEngineerEmail);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.CustomerEngineerPhone);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.PPAPSubmissionLevel);
                strBodyContent.AppendLine("</td>                                                                                             ");
                //strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                //strBodyContent.AppendLine(item.objKickOff.SupplierName);
                //strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.ManufacturerName);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.ManufacturerCity);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.ManufacturerState);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.ManufacturerCountry);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(!string.IsNullOrEmpty(item.objKickOff.EAUUsage) ? item.objKickOff.EAUUsage : "0");
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.MaterialType);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\#\\,\\#\\#0\\.000;'>");
                strBodyContent.AppendLine(string.Format("{0:0,0.000}", item.objKickOff.PartWeight));
                strBodyContent.AppendLine("</td>                                                                                             ");
                if (HasPricingFieldsAccess)
                {
                    strBodyContent.AppendLine("<td  valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                    strBodyContent.AppendLine(string.Format("${0:0,0.000}", item.objKickOff.PurchasePieceCost));
                    strBodyContent.AppendLine("</td>                                                                                             ");
                    strBodyContent.AppendLine("<td  valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                    strBodyContent.AppendLine(string.Format("${0:0,0.000}", item.objKickOff.PurchaseToolingCost));
                    strBodyContent.AppendLine("</td>                                                                                             ");
                    strBodyContent.AppendLine("<td  valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                    strBodyContent.AppendLine(string.Format("${0:0,0.000}", item.objKickOff.SellingPiecePrice));
                    strBodyContent.AppendLine("</td>                                                                                             ");
                    strBodyContent.AppendLine("<td  valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                    strBodyContent.AppendLine(string.Format("${0:0,0.000}", item.objKickOff.SellingToolingPrice));
                    strBodyContent.AppendLine("</td>                                                                                             ");
                }
                strBodyContent.AppendLine("<td  valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(!string.IsNullOrEmpty(item.objKickOff.ToolingLeadtimeDays) ? item.objKickOff.ToolingLeadtimeDays : "0");
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.APQPEngineer);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.SupplyChainCoordinator);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.Destination);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.PartClassification);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(!string.IsNullOrEmpty(item.objKickOff.NumberOfSampleRequired) ? item.objKickOff.NumberOfSampleRequired : "0");
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.ProjectNotes);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td  valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.ShipToLocation);
                strBodyContent.AppendLine("</td>");
                strBodyContent.AppendLine("</tr>");
            }
            #endregion
            strBodyContent.AppendLine("    </tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "Kick Off List.xls", stringStream);
            }

            return filepath;
        }
        private string CreateExcelForAPQPToolingLaunch(List<DTO.Library.APQP.APQP.APQP> lstAPQP)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();

            strBodyContent.AppendLine("<table id='tblId' style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("    <thead>");
            strBodyContent.AppendLine("        <tr>");
            strBodyContent.AppendLine("            <td align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='16'>");
            strBodyContent.AppendLine("                <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("                <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("                </font>");
            strBodyContent.AppendLine("            </td>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("        <tr style=''>");

            strBodyContent.AppendLine("<th  height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>PART #</th>                     ");
            strBodyContent.AppendLine("<th width='250' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>REV LEVEL</th>                   ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>REVISION DATE(DD-MMM-YY)</th>                ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>DRAWING NUMBER</th>                          ");
            strBodyContent.AppendLine("<th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>RFQ #</th>                       ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>QUOTE #</th>                                 ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>PROJECT NAME</th>                            ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>CUSTOMER</th>                                ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>MANUFACTURER</th>                            ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>APQP STATUS</th>                             ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>MES TOOLING PO NUMBER</th>                   ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>TOOLING KICKOFF DATE(DD-MMM-YY)</th>         ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>PLAN TOOLING COMPLETION DATE(DD-MMM-YY)</th> ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>APQP DRAWING STATUS</th>                     ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>DAYS REQUIRED TO PREPARE PPAP SUBMISSION</th>");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>DESIGN REVIEW FEEDBACK</th>                  ");

            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("    </thead>");
            #endregion
            strBodyContent.AppendLine("    <tbody>");
            #region main body loop
            foreach (var item in lstAPQP)
            {
                strBodyContent.AppendLine("<tr>");

                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objToolingLaunch.PartNumber);
                strBodyContent.AppendLine("</td>");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'> ");
                strBodyContent.AppendLine(item.objToolingLaunch.RevLevel);
                strBodyContent.AppendLine("</td>                                                                                              ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'> ");
                strBodyContent.AppendLine(item.objToolingLaunch.RevisionDate.HasValue ? item.objToolingLaunch.RevisionDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                              ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'> ");
                strBodyContent.AppendLine(item.objKickOff.DrawingNumber);
                strBodyContent.AppendLine("</td>                                                                                              ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'> ");
                strBodyContent.AppendLine(item.objToolingLaunch.RFQNumber);
                strBodyContent.AppendLine("</td>                                                                                              ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'> ");
                strBodyContent.AppendLine(item.objToolingLaunch.QuoteNumber);
                strBodyContent.AppendLine("</td>                                                                                              ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'> ");
                strBodyContent.AppendLine(item.objToolingLaunch.ProjectName);
                strBodyContent.AppendLine("</td>                                                                                              ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'> ");
                strBodyContent.AppendLine(item.objToolingLaunch.CustomerName);
                strBodyContent.AppendLine("</td>                                                                                              ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'> ");
                strBodyContent.AppendLine(item.objToolingLaunch.ManufacturerName);
                strBodyContent.AppendLine("</td>                                                                                              ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'> ");
                strBodyContent.AppendLine(item.objKickOff.Status);
                strBodyContent.AppendLine("</td>                                                                                              ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'> ");
                strBodyContent.AppendLine(item.objToolingLaunch.MESToolingPONumber);
                strBodyContent.AppendLine("</td>                                                                                              ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'> ");
                strBodyContent.AppendLine(item.objToolingLaunch.ToolingKickoffDate.HasValue ? item.objToolingLaunch.ToolingKickoffDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                              ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'> ");
                strBodyContent.AppendLine(item.objToolingLaunch.PlanToolingCompletionDate.HasValue ? item.objToolingLaunch.PlanToolingCompletionDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                              ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'> ");
                strBodyContent.AppendLine(item.objToolingLaunch.APQPDrawingStatus);
                strBodyContent.AppendLine("</td>                                                                                              ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;'> ");
                strBodyContent.AppendLine(item.objToolingLaunch.PPAPSubmissonPreparationDays.HasValue ? Convert.ToString(item.objToolingLaunch.PPAPSubmissonPreparationDays.Value) : "0");
                strBodyContent.AppendLine("</td>                                                                                              ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'> ");
                strBodyContent.AppendLine(item.objToolingLaunch.Comments);
                strBodyContent.AppendLine("</td>");

                strBodyContent.AppendLine("</tr>");
            }
            #endregion
            strBodyContent.AppendLine("    </tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "Tooling Launch List.xls", stringStream);
            }

            return filepath;
        }
        private string CreateExcelForAPQPProjectTracking(List<DTO.Library.APQP.APQP.APQP> lstAPQP)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();

            strBodyContent.AppendLine("<table id='tblId' style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("    <thead>");
            strBodyContent.AppendLine("        <tr>");
            strBodyContent.AppendLine("            <td align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='21'>");
            strBodyContent.AppendLine("                <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("                <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("                </font>");
            strBodyContent.AppendLine("            </td>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("        <tr style=''>");
            strBodyContent.AppendLine("<th  height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>PART #</th>                           ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>REV LEVEL</th>                                     ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>DRAWING NUMBER</th>                                ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>QUOTE #</th>                                       ");
            strBodyContent.AppendLine("<th  style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>PROJECT NAME</th>                                 ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>CUSTOMER</th>                                      ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>MANUFACTURER</th>                                  ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>TOOLING KICKOFF DATE(DD-MMM-YY)</th>               ");
            strBodyContent.AppendLine("<th  style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>TOOLING LEADTIME</th>                             ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>PLAN TOOLING COMPLETION DATE(DD-MMM-YY)</th>       ");
            strBodyContent.AppendLine("<th  style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>APQP STATUS</th>                                  ");
            strBodyContent.AppendLine("<th  style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>CATEGORY</th>                                     ");
            strBodyContent.AppendLine("<th  style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>STAGE</th>                                        ");
            strBodyContent.AppendLine("<th  style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>CHANGE TO TOOLING COMPLETION DATE(DD-MMM-YY)</th> ");
            strBodyContent.AppendLine("<th  style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>TOOL CHANGE DETAILS</th>                          ");
            strBodyContent.AppendLine("<th  style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>ACTUAL TOOLING COMPLETION DATE(DD-MMM-YY)</th>    ");
            strBodyContent.AppendLine("<th  style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>SHIPMENT TRACKING #</th>                          ");
            strBodyContent.AppendLine("<th  style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>ESTIMATED SAMPLE SHIPMENT DATE(DD-MMM-YY)</th>    ");
            strBodyContent.AppendLine("<th  style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>ACTUAL SAMPLE SHIPMENT DATE(DD-MMM-YY)</th>       ");
            strBodyContent.AppendLine("<th  style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>QUALITY FEEDBACK / COMMENTS</th>                  ");
            strBodyContent.AppendLine("<th  style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>ADDITIONAL COMMENTS / REMARKS</th>                ");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("    </thead>");
            #endregion
            strBodyContent.AppendLine("    <tbody>");
            #region main body loop
            foreach (var item in lstAPQP)
            {
                strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("<td valign='top' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objProjectTracking.PartNumber);
                strBodyContent.AppendLine("</td>                                 ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>             ");
                strBodyContent.AppendLine(item.objToolingLaunch.RevLevel);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>             ");
                strBodyContent.AppendLine(item.objKickOff.DrawingNumber);
                strBodyContent.AppendLine("</td>                                   ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>             ");
                strBodyContent.AppendLine(item.objProjectTracking.QuoteNumber);
                strBodyContent.AppendLine("</td>");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>             ");
                strBodyContent.AppendLine(item.objProjectTracking.ProjectName);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>             ");
                strBodyContent.AppendLine(item.objProjectTracking.CustomerName);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>             ");
                strBodyContent.AppendLine(item.objProjectTracking.ManufacturerName);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>             ");
                strBodyContent.AppendLine(item.objProjectTracking.ToolingKickoffDate.HasValue ? item.objProjectTracking.ToolingKickoffDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>             ");
                strBodyContent.AppendLine(item.objProjectTracking.ToolingLeadtimeDays);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>             ");
                strBodyContent.AppendLine(item.objProjectTracking.PlanToolingCompletionDate.HasValue ? item.objProjectTracking.PlanToolingCompletionDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>             ");
                strBodyContent.AppendLine(item.objKickOff.Status);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>             ");
                strBodyContent.AppendLine(item.objProjectTracking.ProjectCategory);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>             ");
                strBodyContent.AppendLine(item.objProjectTracking.ProjectStage);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>             ");
                strBodyContent.AppendLine(item.objProjectTracking.CurrentEstimatedToolingCompletionDate.HasValue ? item.objProjectTracking.CurrentEstimatedToolingCompletionDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>             ");
                strBodyContent.AppendLine(item.objProjectTracking.ToolChangeDetails);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine();
                strBodyContent.AppendLine(" <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>            ");
                strBodyContent.AppendLine(item.objProjectTracking.ActualToolingCompletionDate.HasValue ? item.objProjectTracking.ActualToolingCompletionDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine(" <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>            ");
                strBodyContent.AppendLine(item.objProjectTracking.ShipmentTrackingNumber);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine(" <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>            ");
                strBodyContent.AppendLine(item.objProjectTracking.EstimatedSampleShipmentDate.HasValue ? item.objProjectTracking.EstimatedSampleShipmentDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine(" <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>            ");
                strBodyContent.AppendLine(item.objProjectTracking.ActualSampleShipmentDate.HasValue ? item.objProjectTracking.ActualSampleShipmentDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine(" <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>            ");
                strBodyContent.AppendLine(item.objProjectTracking.QualityFeedbackInformation);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine(" <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>            ");
                strBodyContent.AppendLine(item.objProjectTracking.Remarks);
                strBodyContent.AppendLine("</td>                                                                                             ");
                strBodyContent.AppendLine("</tr>");
            }
            #endregion
            strBodyContent.AppendLine("    </tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "Project Tracking List.xls", stringStream);
            }

            return filepath;
        }
        private string CreateExcelForAPQPPPAPSubmission(List<DTO.Library.APQP.APQP.APQP> lstAPQP)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();

            strBodyContent.AppendLine("<table id='tblId' style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("    <thead>");
            strBodyContent.AppendLine("        <tr>");
            strBodyContent.AppendLine("            <td align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='12'>");
            strBodyContent.AppendLine("                <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("                <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("                </font>");
            strBodyContent.AppendLine("            </td>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("        <tr style=''>");

            strBodyContent.AppendLine("<th  height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>PART #</th>                    ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>REV LEVEL</th>                  ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>DRAWING NUMBER</th>                         ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>QUOTE #</th>                                ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>PROJECT NAME</th>                           ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>APQP STATUS</th>                ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>ACTUAL SAMPLE SHIPMENT DATE(DD-MMM-YY)</th> ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>PLANNED PSW DATE(DD-MMM-YY)</th>            ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>ACTUAL PSW DATE(DD-MMM-YY)</th>             ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>PPAP STATUS</th>                            ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>PPAP PARTS APPROVED(DD-MMM-YY)</th>         ");
            strBodyContent.AppendLine("<th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>COMMENTS</th>                               ");

            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("    </thead>");
            #endregion
            strBodyContent.AppendLine("    <tbody>");
            #region main body loop
            foreach (var item in lstAPQP)
            {
                strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objPPAPSubmission.PartNumber);
                strBodyContent.AppendLine("</td>                                                                               ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objToolingLaunch.RevLevel);
                strBodyContent.AppendLine("</td>                                                                                ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.DrawingNumber);
                strBodyContent.AppendLine("</td>                                                                                ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objPPAPSubmission.QuoteNumber);
                strBodyContent.AppendLine("</td>                                                                                ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objPPAPSubmission.ProjectName);
                strBodyContent.AppendLine("</td>                                                                                ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objKickOff.Status);
                strBodyContent.AppendLine("</td>                                                                                ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objPPAPSubmission.ActualSampleShipmentDate.HasValue ? item.objPPAPSubmission.ActualSampleShipmentDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objPPAPSubmission.PSWDate.HasValue ? item.objPPAPSubmission.PSWDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objPPAPSubmission.ActualPSWDate.HasValue ? item.objPPAPSubmission.ActualPSWDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objPPAPSubmission.PPAPStatus);
                strBodyContent.AppendLine("</td>                                                                                ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objPPAPSubmission.PPAPPartsApprovedDate.HasValue ? item.objPPAPSubmission.PPAPPartsApprovedDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("</td>                                                                                ");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.objPPAPSubmission.Comments);
                strBodyContent.AppendLine("</td>                                                                                ");
                strBodyContent.AppendLine("</tr>");
            }
            #endregion
            strBodyContent.AppendLine("    </tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "PPAP Submission List.xls", stringStream);
            }

            return filepath;
        }
        #endregion

        #region General CRUD operation
        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.APQP.APQP.APQP aPQP)
        {
            string errMSg = null, successMsg = null;

            #region Save KickOff details
            IKickOffRepository objIKickOffRepository = new MES.Business.Library.BO.APQP.APQP.KickOff();
            if (objIKickOffRepository.Save(aPQP.objKickOff).StatusCode != 200)
            {
                errMSg = Languages.GetResourceText("APQPSaveFail");
                return SuccessOrFailedResponse<int?>(errMSg, aPQP.Id, successMsg);
            }
            #endregion
            #region Save Tooling Launch details
            IToolingLaunchRepository objToolingLaunchRepository = new MES.Business.Library.BO.APQP.APQP.ToolingLaunch();
            if (objToolingLaunchRepository.Save(aPQP.objToolingLaunch).StatusCode != 200)
            {
                errMSg = Languages.GetResourceText("APQPSaveFail");
                return SuccessOrFailedResponse<int?>(errMSg, aPQP.Id, successMsg);
            }
            #endregion
            #region Save Project Tracking details
            IProjectTrackingRepository objProjectTrackingRepository = new MES.Business.Library.BO.APQP.APQP.ProjectTracking();
            if (objProjectTrackingRepository.Save(aPQP.objProjectTracking).StatusCode != 200)
            {
                errMSg = Languages.GetResourceText("APQPSaveFail");
                return SuccessOrFailedResponse<int?>(errMSg, aPQP.Id, successMsg);
            }
            #endregion
            #region Save PPAP Submission details
            IPPAPSubmissionRepository objPPAPSubmissionRepository = new MES.Business.Library.BO.APQP.APQP.PPAPSubmission();
            if (objPPAPSubmissionRepository.Save(aPQP.objPPAPSubmission).StatusCode != 200)
            {
                errMSg = Languages.GetResourceText("APQPSaveFail");
                return SuccessOrFailedResponse<int?>(errMSg, aPQP.Id, successMsg);
            }
            #endregion

            successMsg = Languages.GetResourceText("APQPSavedSuccess");
            return SuccessOrFailedResponse<int?>(errMSg, aPQP.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.APQP.APQP.APQP> FindById(int id)
        {
            string errMSg = string.Empty;

            LoginUser currentUser = GetCurrentUser;

            bool allowDeleteRecord = true, allowExportToSAP = true, allowSendDataToSAP = true, allowCheckNPIFStatus = true, hasPricingFieldsAccess = true;
            var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQP)).ToList();
            if (currentObjects.Count > 0)
                allowDeleteRecord = currentObjects[0].AllowDeleteRecord;

            currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQP)).ToList();
            if (currentObjects.Count > 0)
                hasPricingFieldsAccess = currentObjects[0].HasPricingFieldsAccess;

            currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.PPAPSubmission)).ToList();
            if (currentObjects.Count > 0)
                allowExportToSAP = currentObjects[0].AllowExportToSAP;

            currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.KickOff)).ToList();
            if (currentObjects.Count > 0)
                allowSendDataToSAP = currentObjects[0].AllowSendDataToSAP;

            currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.KickOff)).ToList();
            if (currentObjects.Count > 0)
                allowCheckNPIFStatus = currentObjects[0].AllowCheckNPIFStatus;

            DTO.Library.APQP.APQP.APQP aPQP = new DTO.Library.APQP.APQP.APQP();
            this.RunOnDB(context =>
            {
                #region General details
                #endregion
                #region KickOff details
                IKickOffRepository objIKickOffRepository = new MES.Business.Library.BO.APQP.APQP.KickOff();
                aPQP.objKickOff = objIKickOffRepository.FindById(id).Result;
                #endregion
                #region Tooling Launch details
                IToolingLaunchRepository objToolingLaunchRepository = new MES.Business.Library.BO.APQP.APQP.ToolingLaunch();
                aPQP.objToolingLaunch = objToolingLaunchRepository.FindById(id).Result;
                #endregion
                #region Project Tracking details
                IProjectTrackingRepository objProjectTrackingRepository = new MES.Business.Library.BO.APQP.APQP.ProjectTracking();
                aPQP.objProjectTracking = objProjectTrackingRepository.FindById(id).Result;
                #endregion
                #region PPAP Submission details
                IPPAPSubmissionRepository objPPAPSubmissionRepository = new MES.Business.Library.BO.APQP.APQP.PPAPSubmission();
                aPQP.objPPAPSubmission = objPPAPSubmissionRepository.FindById(id).Result;
                #endregion
                #region get controls tooltip
                var controlTooptip = context.ControlToolTips.ToList();
                var objExpandoObject = new System.Dynamic.ExpandoObject() as IDictionary<string, Object>;
                foreach (var item in controlTooptip)
                {
                    objExpandoObject.Add(item.ToolTipKey, item.ToolTipDescription);
                }
                aPQP.objTooltip = objExpandoObject;

                aPQP.AllowDeleteRecord = allowDeleteRecord;
                aPQP.AllowExportToSAP = allowExportToSAP;
                aPQP.AllowSendDataToSAP = allowSendDataToSAP;
                aPQP.AllowCheckNPIFStatus = allowCheckNPIFStatus;
                aPQP.HasPricingFieldsAccess = hasPricingFieldsAccess;
                #endregion
            });

            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.APQP.APQP.APQP>(errMSg, aPQP);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int aPQPId)
        {
            //set the out put param
            ObjectParameter Result = new ObjectParameter("Result", 0);
            this.RunOnDB(context =>
            {
                //context.DeleteMultipleCustomer(CustomerIds, CurrentUser, Result);
            });
            if (Convert.ToInt32(Result.Value) > 0)
                return SuccessBoolResponse(Languages.GetResourceText("APQPDeletedSuccess"));
            else
                return FailedBoolResponse(Languages.GetResourceText("APQPDeleteFail"));
        }

        public NPE.Core.ITypedResponse<bool?> DeleteAPQPItem(int APQPItemId)
        {
            try
            {
                var apqpItemMasterToBeDeleted = this.DataContext.ItemMasters.Where(a => a.Id == APQPItemId).SingleOrDefault();
                if (apqpItemMasterToBeDeleted == null)
                {
                    return FailedBoolResponse(Languages.GetResourceText("APQPNotExists"));
                }
                else
                {
                    apqpItemMasterToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    apqpItemMasterToBeDeleted.UpdatedBy = CurrentUser;
                    this.DataContext.Entry(apqpItemMasterToBeDeleted).State = EntityState.Modified;
                    apqpItemMasterToBeDeleted.IsDeleted = true;
                    this.DataContext.SaveChanges();
                    return SuccessBoolResponse(Languages.GetResourceText("APQPDeletedSuccess"));
                }
            }
            catch (Exception ex)
            {
                return FailedBoolResponse(Languages.GetResourceText("APQPDeleteFail"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.APQP>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.APQP>> GetAPQPList(NPE.Core.IPage<DTO.Library.APQP.APQP.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            LoginUser currentUser = GetCurrentUser;
            List<DTO.Library.APQP.APQP.APQP> lstAPQP = new List<DTO.Library.APQP.APQP.APQP>();
            DTO.Library.APQP.APQP.APQP aPQP;

            #region GEt controls tooltip
            var objExpandoObject = new System.Dynamic.ExpandoObject() as IDictionary<string, Object>;
            this.RunOnDB(context =>
             {
                 var controlTooptip = context.ControlToolTips.ToList();
                 foreach (var item in controlTooptip)
                 {
                     objExpandoObject.Add(item.ToolTipKey, item.ToolTipDescription);
                 }
             });
            #endregion
            #region KickOff details
            IKickOffRepository objIKickOffRepository = new MES.Business.Library.BO.APQP.APQP.KickOff();
            var KickOffData = objIKickOffRepository.GetKickOffList(paging).Result.OrderBy(a => a.Id).ToList();
            #endregion
            #region Tooling Launch details
            IToolingLaunchRepository objToolingLaunchRepository = new MES.Business.Library.BO.APQP.APQP.ToolingLaunch();
            var ToolingLaunchData = objToolingLaunchRepository.GetToolingLaunchList(paging).Result.OrderBy(a => a.APQPItemId).ToList();
            //aPQP.objToolingLaunch
            #endregion
            #region Project Tracking details
            IProjectTrackingRepository objProjectTrackingRepository = new MES.Business.Library.BO.APQP.APQP.ProjectTracking();
            var ProjectTrackingData = objProjectTrackingRepository.GetProjectTrackingList(paging).Result.OrderBy(a => a.APQPItemId).ToList();
            //aPQP.objProjectTracking
            #endregion
            #region PPAP Submission details
            IPPAPSubmissionRepository objPPAPSubmissionRepository = new MES.Business.Library.BO.APQP.APQP.PPAPSubmission();
            var PPAPSubmissionData = objPPAPSubmissionRepository.GetPPAPSubmissionList(paging).Result.OrderBy(a => a.APQPItemId).ToList();
            //aPQP.objPPAPSubmission
            #endregion
            #region prepare list object
            int TotalCount = KickOffData.Count;

            bool allowDeleteRecord = true, allowExportToSAP = true, allowSendDataToSAP = true, hasPricingFieldsAccess = true;
            var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQP)).ToList();
            if (currentObjects.Count > 0)
                allowDeleteRecord = currentObjects[0].AllowDeleteRecord;

            currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQP)).ToList();
            if (currentObjects.Count > 0)
                hasPricingFieldsAccess = currentObjects[0].HasPricingFieldsAccess;

            currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.PPAPSubmission)).ToList();
            if (currentObjects.Count > 0)
                allowExportToSAP = currentObjects[0].AllowExportToSAP;

            currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.KickOff)).ToList();
            if (currentObjects.Count > 0)
                allowSendDataToSAP = currentObjects[0].AllowSendDataToSAP;

            for (int i = 0; i < TotalCount; i++)
            {
                aPQP = new DTO.Library.APQP.APQP.APQP();
                aPQP.Id = KickOffData[i].Id;
                aPQP.AllowDeleteRecord = allowDeleteRecord;
                aPQP.AllowExportToSAP = allowExportToSAP;
                aPQP.AllowSendDataToSAP = allowSendDataToSAP;
                aPQP.HasPricingFieldsAccess = hasPricingFieldsAccess;
                aPQP.chkSelect = false;
                aPQP.SelectedClassId = 0;

                aPQP.objKickOff = KickOffData[i];
                aPQP.objToolingLaunch = ToolingLaunchData[i];
                aPQP.objProjectTracking = ProjectTrackingData[i];
                aPQP.objPPAPSubmission = PPAPSubmissionData[i];
                aPQP.objTooltip = objExpandoObject;
                lstAPQP.Add(aPQP);
            }
            lstAPQP = lstAPQP.OrderByDescending(a => a.Id).ToList();
            #endregion

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.APQP.APQP>>(errMSg, lstAPQP);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.SAPItem>> SearchFromSAPRecords(NPE.Core.IPage<DTO.Library.APQP.APQP.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.APQP.APQP.SAPItem> lstSAPItem = new List<DTO.Library.APQP.APQP.SAPItem>();
            DTO.Library.APQP.APQP.SAPItem sAPItem;
            this.RunOnDB(context =>
             {
                 var SAPItemList = context.GetSAPItem(paging.Criteria.PartNo, paging.Criteria.RFQNumber, paging.Criteria.QuoteNumber, paging.Criteria.CustomerName).ToList();
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

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.SAPItem>> SearchFromAPQPRecords(NPE.Core.IPage<DTO.Library.APQP.APQP.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.APQP.APQP.SAPItem> lstSAPItem = new List<DTO.Library.APQP.APQP.SAPItem>();
            DTO.Library.APQP.APQP.SAPItem sAPItem;
            this.RunOnDB(context =>
            {
                var SAPItemList = context.GetSAPItem(paging.Criteria.PartNo, paging.Criteria.RFQNumber, paging.Criteria.QuoteNumber, paging.Criteria.CustomerName).ToList();
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

        public NPE.Core.ITypedResponse<DTO.Library.APQP.APQP.PSWItem> getGeneratePSWData(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.APQP.APQP.PSWItem pSWItem = new DTO.Library.APQP.APQP.PSWItem();
            this.RunOnDB(context =>
            {
                var PSWData = context.GetPSWByItemId(id).SingleOrDefault();
                if (PSWData == null)
                    errMSg = Languages.GetResourceText("PSWDataNotExists");
                else
                {
                    #region General details
                    pSWItem = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.APQP.PSWItem>(PSWData);
                    pSWItem.DerivedSupplierAddress = pSWItem.SupplierCity + (!string.IsNullOrEmpty(pSWItem.SupplierCountry) ? ", " + pSWItem.SupplierCountry : string.Empty);
                    pSWItem.DerivedSupplierName = pSWItem.SupplierName;
                    if (!string.IsNullOrEmpty(pSWItem.ManufacturerName) && (pSWItem.SupplierName.Trim().ToLower() != pSWItem.ManufacturerName.Trim().ToLower()))
                        pSWItem.DerivedSupplierName = pSWItem.ManufacturerName + " (" + pSWItem.SupplierName + ")";
                    #endregion
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.APQP.APQP.PSWItem>(errMSg, pSWItem);
            return response;
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.PSWItem>> getGeneratePSWDataByItems(string ids)
        {
            string errMSg = string.Empty;
            List<DTO.Library.APQP.APQP.PSWItem> lstpSWItem = new List<DTO.Library.APQP.APQP.PSWItem>();
            DTO.Library.APQP.APQP.PSWItem pSWItem;
            this.RunOnDB(context =>
            {
                var PSWData = context.GetAPQPPSWByIds(ids).ToList();
                if (PSWData == null)
                    errMSg = Languages.GetResourceText("PSWDataNotExists");
                else
                {
                    foreach (var item in PSWData)
                    {
                        pSWItem = new DTO.Library.APQP.APQP.PSWItem();
                        #region General details
                        pSWItem = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.APQP.PSWItem>(item);
                        pSWItem.DerivedSupplierAddress = pSWItem.SupplierCity + (!string.IsNullOrEmpty(pSWItem.SupplierCountry) ? ", " + pSWItem.SupplierCountry : string.Empty);
                        pSWItem.DerivedSupplierName = pSWItem.SupplierName;
                        if (!string.IsNullOrEmpty(pSWItem.ManufacturerName) && (pSWItem.SupplierName.Trim().ToLower() != pSWItem.ManufacturerName.Trim().ToLower()))
                            pSWItem.DerivedSupplierName = pSWItem.ManufacturerName + " (" + pSWItem.SupplierName + ")";
                        #endregion

                        lstpSWItem.Add(pSWItem);
                    }
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.APQP.PSWItem>>(errMSg, lstpSWItem);
            return response;
        }
        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.PredefinedDocumentTypes>> getAPQPPredefinedDocumentTypes(string APQPItemIds)
        {
            string errMSg = string.Empty;
            List<DTO.Library.APQP.APQP.PredefinedDocumentTypes> lstPredefinedDocumentTypes = new List<DTO.Library.APQP.APQP.PredefinedDocumentTypes>();
            DTO.Library.APQP.APQP.PredefinedDocumentTypes predefinedDocumentTypes = new DTO.Library.APQP.APQP.PredefinedDocumentTypes();
            this.RunOnDB(context =>
            {
                //if (allowConfidentialDocumentType == true)
                var PredefinedDocumentTypesData = context.GetDocumentType(null, null, null).Where(item => item.AssociatedToIds.Contains(Convert.ToInt16(MES.Business.Library.Enums.DocTypeAssociatedTo.APQP_Step_2).ToString()) || item.AssociatedToIds.Contains(Convert.ToInt16(MES.Business.Library.Enums.DocTypeAssociatedTo.APQP_Step_3).ToString())).OrderBy(rec => rec.AssociatedToName).ToList();
                //else
                //var PredefinedDocumentTypesData = context.GetDocumentType(null, null, null).Where(item =>item.IsConfidential != true &&  item.AssociatedToIds.Contains(Convert.ToInt16(MES.Business.Library.Enums.DocTypeAssociatedTo.APQP_Step_2).ToString()) || item.AssociatedToIds.Contains(Convert.ToInt16(MES.Business.Library.Enums.DocTypeAssociatedTo.APQP_Step_3).ToString())).OrderBy(rec => rec.AssociatedToName).ToList();
                if (PredefinedDocumentTypesData == null)
                    errMSg = Languages.GetResourceText("PSWDataNotExists");
                else
                {
                    foreach (var item in PredefinedDocumentTypesData)
                    {
                        predefinedDocumentTypes = new DTO.Library.APQP.APQP.PredefinedDocumentTypes();
                        predefinedDocumentTypes.Id = item.Id;
                        predefinedDocumentTypes.DocumentType = item.DocumentType;
                        predefinedDocumentTypes.IsConfidential = item.IsConfidential;
                        predefinedDocumentTypes.AssociatedToIds = item.AssociatedToIds;
                        predefinedDocumentTypes.AssociatedToName = item.AssociatedToName;
                        predefinedDocumentTypes.DocumentTypeUsedAssociatedIds = item.DocumentTypeUsedAssociatedIds;
                        lstPredefinedDocumentTypes.Add(predefinedDocumentTypes);
                    }
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.APQP.PredefinedDocumentTypes>>(errMSg, lstPredefinedDocumentTypes);
            return response;

        }

        public NPE.Core.ITypedResponse<bool?> SavePredefinedDocumentType(string DocumentTypeIds, string APQPItemIds)
        {
            string errMSg = null;
            string successMsg = null;
            bool? IsSuccess = true;
            ObjectParameter outputParam = new ObjectParameter("ErrorKey", "");
            this.RunOnDB(context =>
            {
                int result = context.SavePredefinedDocumentTypes(DocumentTypeIds, APQPItemIds, CurrentUser, outputParam);

                if (string.IsNullOrEmpty(Convert.ToString(outputParam.Value)))
                {
                    successMsg = Languages.GetResourceText("PredefinedDocumentTypeSavedSuccess");
                }
                else
                {
                    errMSg = Languages.GetResourceText("PredefinedDocumentTypeSavedSuccess") + " Document Type(s) - " + Convert.ToString(outputParam.Value).Trim(',') + " could not be added; since they are already added.";
                    IsSuccess = false;
                }
            });
            return SuccessOrFailedResponse<bool?>(errMSg, IsSuccess, successMsg);
        }

        public NPE.Core.ITypedResponse<MES.DTO.Library.APQP.APQP.SearchDocument> getDocumentsData(int APQPItemId, string SectionName)
        {
            string errMSg = string.Empty;
            bool allowConfidentialDocumentType = true;
            LoginUser currentUser = GetCurrentUser;
            var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQPDashboard)).ToList();
            if (currentObjects.Count > 0)
                allowConfidentialDocumentType = currentObjects[0].AllowConfidentialDocumentType;

            int? AssociatedToId = null;
            switch (SectionName)
            {
                case "APQPSTEP1":
                    AssociatedToId = (int)MES.Business.Library.Enums.DocTypeAssociatedTo.APQP_Step_1;
                    break;
                case "APQPSTEP2":
                    AssociatedToId = (int)MES.Business.Library.Enums.DocTypeAssociatedTo.APQP_Step_2;
                    break;
                case "APQPSTEP3":
                    AssociatedToId = (int)MES.Business.Library.Enums.DocTypeAssociatedTo.APQP_Step_3;
                    break;
                case "APQPSTEP4":
                    AssociatedToId = (int)MES.Business.Library.Enums.DocTypeAssociatedTo.APQP_Step_4;
                    break;
                default:
                    break;
            }

            List<DTO.Library.APQP.APQP.Document> lstDocument = new List<DTO.Library.APQP.APQP.Document>();
            DTO.Library.APQP.APQP.SearchDocument searchDocument = new DTO.Library.APQP.APQP.SearchDocument();
            DTO.Library.APQP.APQP.Document document = null;
            this.RunOnDB(context =>
            {
                searchDocument.objPSWItem = getGeneratePSWData(APQPItemId).Result;
                var lstAPQPSupplierDetails = getAPQPSupplierDetailsFromShareDocuments(APQPItemId).Result;

                var DocumentData = context.GetDocuments(APQPItemId, AssociatedToId).ToList();
                if (DocumentData == null)
                    errMSg = Languages.GetResourceText("DataNotExists");
                else
                {
                    foreach (var item in DocumentData)
                    {
                        if (!allowConfidentialDocumentType && item.IsConfidential)
                        { continue; }

                        document = new DTO.Library.APQP.APQP.Document();
                        document = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.APQP.Document>(item);
                        document.FilePath = !string.IsNullOrEmpty(item.FilePath) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) +
                            Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + item.FilePath : string.Empty;

                        document.lstAPQPSupplierDetails = lstAPQPSupplierDetails;
                        lstDocument.Add(document);
                    }
                    searchDocument.lstDocument = lstDocument;
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.APQP.APQP.SearchDocument>(errMSg, searchDocument);
            return response;

        }
        public NPE.Core.ITypedResponse<List<MES.DTO.Library.APQP.APQP.APQPSupplierDetails>> getAPQPSupplierDetailsFromShareDocuments(int APQPItemId)
        {
            string errMSg = string.Empty;
            List<DTO.Library.APQP.APQP.APQPSupplierDetails> lstAPQPSupplierDetails = new List<DTO.Library.APQP.APQP.APQPSupplierDetails>();
            DTO.Library.APQP.APQP.APQPSupplierDetails aPQPSupplierDetails = null;
            this.RunOnDB(context =>
            {
                var APQPSupplierDetailsData = context.GetSupplierByPartNumber(APQPItemId).ToList();
                if (APQPSupplierDetailsData == null)
                    errMSg = Languages.GetResourceText("DataNotExists");
                else
                {
                    foreach (var item in APQPSupplierDetailsData)
                    {
                        aPQPSupplierDetails = new DTO.Library.APQP.APQP.APQPSupplierDetails();
                        aPQPSupplierDetails = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.APQP.APQPSupplierDetails>(item);
                        lstAPQPSupplierDetails.Add(aPQPSupplierDetails);
                    }
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.APQP.APQPSupplierDetails>>(errMSg, lstAPQPSupplierDetails);
            return response;
        }

        public NPE.Core.ITypedResponse<int?> SaveDocument(DTO.Library.APQP.APQP.Document document)
        {
            #region Save Document
            switch (document.SectionName)
            {
                case "APQPSTEP1":
                    document.UploadedFromStepId = (int)MES.Business.Library.Enums.DocTypeAssociatedTo.APQP_Step_1;
                    break;
                case "APQPSTEP2":
                    document.UploadedFromStepId = (int)MES.Business.Library.Enums.DocTypeAssociatedTo.APQP_Step_2;
                    break;
                case "APQPSTEP3":
                    document.UploadedFromStepId = (int)MES.Business.Library.Enums.DocTypeAssociatedTo.APQP_Step_3;

                    break;
                case "APQPSTEP4":
                    document.UploadedFromStepId = (int)MES.Business.Library.Enums.DocTypeAssociatedTo.APQP_Step_4;
                    break;
                default:
                    break;
            }

            IDocumentRepository objIDocumentRepository = new MES.Business.Library.BO.APQP.APQP.Document();
            return objIDocumentRepository.Save(document);
            #endregion
        }
        public NPE.Core.ITypedResponse<int?> SaveShareDocumentFiles(string APQPItemIds, int documentId)
        {
            #region Share Document Files
            string successMsg = null;
            int resultVal = 0;
            this.RunOnDB(context =>
            {
                resultVal = context.ShareDocument(APQPItemIds, documentId, CurrentUser);
                successMsg = Languages.GetResourceText("SAPItemSavedSuccess");
            });
            return SuccessOrFailedResponse<int?>(null, resultVal, successMsg);
            #endregion
        }
        public NPE.Core.ITypedResponse<bool?> DeleteDocument(int documentId)
        {
            #region Delete Document
            IDocumentRepository objIDocumentRepository = new MES.Business.Library.BO.APQP.APQP.Document();
            return objIDocumentRepository.Delete(documentId);
            #endregion
        }
        public NPE.Core.ITypedResponse<bool?> GetDocumentsAvailabilityByAPQPItemId(int APQPItemId)
        {
            string errMSg = string.Empty;
            string successMsg = string.Empty;
            bool result = false;
            this.RunOnDB(context =>
            {

                var data = context.GetDocumentsAvailabilityByAPQPItemId(APQPItemId, false).SingleOrDefault();
                if (data == null)
                    errMSg = Languages.GetResourceText("DataNotExists");
                else
                {
                    if (data.HasValue && Convert.ToInt32(data.Value) > 0)
                        result = true;
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<bool?>(errMSg, result, successMsg);
            return response;
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.QuotePartItem>> GetAPQPQuotePartByQuoteIds(string QuoteIds)
        {
            string errMSg = string.Empty;
            List<DTO.Library.APQP.APQP.QuotePartItem> lstQuotePartItem = new List<DTO.Library.APQP.APQP.QuotePartItem>();
            DTO.Library.APQP.APQP.QuotePartItem quotePartItem;
            this.RunOnDB(context =>
            {
                var QuotePartItemData = context.apqpGetQuotePart(QuoteIds).ToList();
                if (QuotePartItemData == null)
                    errMSg = Languages.GetResourceText("QuotePartDataNotExists");
                else
                {
                    foreach (var item in QuotePartItemData)
                    {
                        quotePartItem = new DTO.Library.APQP.APQP.QuotePartItem();
                        quotePartItem = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.APQP.QuotePartItem>(item);
                        lstQuotePartItem.Add(quotePartItem);
                    }
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.APQP.QuotePartItem>>(errMSg, lstQuotePartItem);
            return response;
        }
        public NPE.Core.ITypedResponse<int?> InsertAPQPItemQuotePart(string ItemIds)
        {
            string errMSg = null;
            string successMsg = null;
            int? APQPItemId = 1;
            this.RunOnDB(context =>
            {
                int result = context.InsertAPQPItemQuotePart(ItemIds, CurrentUser);
                //APQPItemId = Convert.ToInt32(result);

                if (APQPItemId > 0)
                {
                    successMsg = Languages.GetResourceText("QuoteItemSavedSuccess");
                }
                else
                    errMSg = Languages.GetResourceText("QuoteItemSavedFail");
            });
            return SuccessOrFailedResponse<int?>(errMSg, APQPItemId, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Supplier.Suppliers> getManufacturerDetails(int manufacturerId)
        {
            string errMSg = string.Empty, countryName = string.Empty;
            DTO.Library.RFQ.Supplier.Suppliers suppliers = new DTO.Library.RFQ.Supplier.Suppliers();
            this.RunOnDB(context =>
            {
                var Suppliers = context.Suppliers.Where(s => s.Id == manufacturerId).SingleOrDefault();
                if (Suppliers == null)
                    errMSg = Languages.GetResourceText("SuppliersNotExists");
                else
                {
                    if (Suppliers.CountryId.HasValue)
                    {
                        var country = context.Countries.Where(s => s.Id == Suppliers.CountryId.Value).SingleOrDefault();
                        countryName = country.Value;
                    }
                    suppliers.SupplierCode = Suppliers.SupplierCode;
                    suppliers.CompanyName = Suppliers.CompanyName;
                    suppliers.Address1 = Suppliers.Address1;
                    suppliers.Address2 = Suppliers.Address2;
                    suppliers.City = Suppliers.City;
                    suppliers.State = Suppliers.State;
                    suppliers.CountryId = Suppliers.CountryId;
                    suppliers.Country = countryName;
                    suppliers.Zip = Suppliers.Zip;
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<DTO.Library.RFQ.Supplier.Suppliers>(errMSg, suppliers);
            return response;
        }
        #endregion

        #region SAP Operations
        public NPE.Core.ITypedResponse<bool?> SendDataToSAP(string APQPItemIds)
        {
            bool IsSuccess = true;
            string errMSg = null, successMsg = null, requestSAPUrl = System.Configuration.ConfigurationManager.AppSettings["SAPAPIURL_PUSH"];

            List<DTO.Library.APQP.APQP.PSWItem> apqpPSWData = getGeneratePSWDataByItems(APQPItemIds).Result;
            List<MES.DTO.Library.APQP.APQP.SAPDataList> objSAPDataList = new List<MES.DTO.Library.APQP.APQP.SAPDataList>();
            try
            {
                foreach (var item in apqpPSWData)
                {
                    objSAPDataList.Add(new MES.DTO.Library.APQP.APQP.SAPDataList()
                    {
                        ItemCode = string.IsNullOrEmpty(item.PartNumber) ? string.Empty : item.PartNumber,
                        RevLevel = string.IsNullOrEmpty(item.RevLevel) ? string.Empty : item.RevLevel,
                        PartDec = string.IsNullOrEmpty(item.PartDesc) ? string.Empty : item.PartDesc,
                        RevDate = item.RevisionDate.HasValue ? item.RevisionDate.Value.ToShortDateString() : string.Empty,
                        Customer = string.IsNullOrEmpty(item.CustomerCode) ? string.Empty : item.CustomerCode,
                        Supplier = string.IsNullOrEmpty(item.SupplierCode) ? string.Empty : item.SupplierCode,
                        MaterialType = string.IsNullOrEmpty(item.MaterialType) ? string.Empty : item.MaterialType,
                        PartWeight = string.IsNullOrEmpty(item.PartWeight) ? string.Empty : item.PartWeight,
                        PurchasePieceCost = item.PurchasePieceCost,
                        SellingPiecePrice = item.SellingPiecePrice,
                        SupplyChainCoordinator = string.IsNullOrEmpty(item.SupplyChainCoordinator) ? string.Empty : item.SupplyChainCoordinator,
                        MESToolingPONumber = string.IsNullOrEmpty(item.MESToolingPONumber) ? string.Empty : item.MESToolingPONumber,
                        ToolingKickoffDate = item.ToolingKickoffDate.HasValue ? item.ToolingKickoffDate.Value.ToShortDateString() : string.Empty,
                        MESWarehouse = string.IsNullOrEmpty(item.MESWarehouse) ? string.Empty : item.MESWarehouse,
                        APQPEngineer = string.IsNullOrEmpty(item.APQPEngineer) ? string.Empty : item.APQPEngineer,
                        MESAccountManager = string.IsNullOrEmpty(item.MESAccountManager) ? string.Empty : item.MESAccountManager
                    });
                }

                StringWriter stringWriter = new StringWriter();
                XmlDocument xmlDoc = new XmlDocument();
                XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
                XmlSerializer serializer = new XmlSerializer(typeof(List<MES.DTO.Library.APQP.APQP.SAPDataList>));
                serializer.Serialize(xmlWriter, objSAPDataList);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestSAPUrl);
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(stringWriter.ToString());
                request.ContentType = "text/xml; encoding='utf-8'";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    JToken token = JObject.Parse(new StreamReader(responseStream).ReadToEnd());
                    successMsg = (string)token.SelectToken("content");
                    successMsg = "Data has been sent successfully to SAP - " + successMsg.Replace("\r\n", "<br/>");
                }
                else
                {
                    IsSuccess = false;
                    errMSg = ((HttpWebResponse)response).StatusCode + " - " + ((HttpWebResponse)response).StatusDescription;
                }
                response.Close();
                response = null;
                requestStream.Close();
                requestStream.Dispose();
                requestStream.Flush();
                requestStream = null;
                bytes = null;
                request = null;
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                errMSg = ex.Message;
            }
            return SuccessOrFailedResponse<bool?>(errMSg, IsSuccess, successMsg);
        }

        public NPE.Core.ITypedResponse<int?> InsertFromSAPRecords(string ItemIds)
        {
            string errMSg = null;
            string successMsg = null;
            int? APQPItemId = 0;
            ObjectParameter outputParam = new ObjectParameter("APQPItemId", 0);
            this.RunOnDB(context =>
           {
               int result = context.InsertAPQPSAPItem(ItemIds, CurrentUser, outputParam);
               APQPItemId = Convert.ToInt32(outputParam.Value);

               if (APQPItemId > 0)
               {
                   successMsg = Languages.GetResourceText("SAPItemSavedSuccess");
               }
               else
                   errMSg = Languages.GetResourceText("SAPItemSavedFail");
           });
            return SuccessOrFailedResponse<int?>(errMSg, APQPItemId, successMsg);
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
        #endregion

        #region Get project status report
        public ITypedResponse<List<MES.DTO.Library.APQP.APQP.APQPProjectStatusReport>> GetAPQPProjectStatusReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.ReportSearchCriteria> searchCriteria)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.APQP.APQP.APQPProjectStatusReport> lstAPQPProjectStatusReport = new List<DTO.Library.APQP.APQP.APQPProjectStatusReport>();
            DTO.Library.APQP.APQP.APQPProjectStatusReport aPQPProjectStatusReport;
            this.RunOnDB(context =>
            {
                var APQPProjectStatusReportList = context.GetProjectStatus(searchCriteria.Criteria.CustomerName, searchCriteria.Criteria.APQPQualityEngineerId, searchCriteria.Criteria.PartNo, searchCriteria.Criteria.ProjectName,
                    searchCriteria.Criteria.SupplyChainCoordinatorId, searchCriteria.Criteria.SAMUserId, searchCriteria.Criteria.ManufacturerName, searchCriteria.Criteria.ManufacturerCode,
                    searchCriteria.Criteria.RFQNumber, searchCriteria.Criteria.QuoteNumber, searchCriteria.Criteria.APQPStatusIds, searchCriteria.Criteria.DateFrom, searchCriteria.Criteria.DateTo,
                    searchCriteria.PageNo, searchCriteria.PageSize, totalRecords, "").ToList();
                if (APQPProjectStatusReportList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    searchCriteria.TotalRecords = Convert.ToInt32(totalRecords.Value);

                    foreach (var apqpData in APQPProjectStatusReportList)
                    {
                        #region assign data
                        aPQPProjectStatusReport = new DTO.Library.APQP.APQP.APQPProjectStatusReport();
                        aPQPProjectStatusReport = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.APQP.APQPProjectStatusReport>(apqpData);
                        lstAPQPProjectStatusReport.Add(aPQPProjectStatusReport);
                        #endregion
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.APQP.APQPProjectStatusReport>>(errMSg, lstAPQPProjectStatusReport);
            response.PageInfo = searchCriteria.ToPage();
            return response;
        }
        #endregion

        #region project status report Export to Excel
        public NPE.Core.ITypedResponse<bool?> ExportAPQPProjectStatusReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.ReportSearchCriteria> searchCriteria)
        {
            #region don't consider page size and page number, export all on the basis of search criteria
            searchCriteria.PageNo = 1;
            searchCriteria.PageSize = int.MaxValue;
            #endregion
            string filePath = string.Empty;
            try
            {
                filePath = CreateExcelForProjectStatusReport(searchCriteria);
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private string CreateExcelForProjectStatusReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.ReportSearchCriteria> searchCriteria)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();
            List<MES.DTO.Library.APQP.APQP.APQPProjectStatusReport> lstAPQPProjectStatusReport = new List<DTO.Library.APQP.APQP.APQPProjectStatusReport>();
            lstAPQPProjectStatusReport = GetAPQPProjectStatusReport(searchCriteria).Result;

            strBodyContent.AppendLine("<table id='tblId' style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("    <thead>");
            strBodyContent.AppendLine("        <tr>");
            strBodyContent.AppendLine("            <td align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='17'>");
            strBodyContent.AppendLine("                <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("                <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("                </font>");
            strBodyContent.AppendLine("            </td>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("        <tr style=''>");
            strBodyContent.AppendLine("            <th height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Part Number</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Rev Level</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Part Description</th>");
            strBodyContent.AppendLine("            <th width='170' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Project Kickoff Date</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Customer</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Project Name</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Manufacturer</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Engineer</th>");
            strBodyContent.AppendLine("            <th width='220' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Actual Tooling Kickoff Date</th>");
            strBodyContent.AppendLine("            <th width='150' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Tooling Leadtime</th>");
            strBodyContent.AppendLine("            <th width='250' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Planned Tooling Completion Date</th>");
            strBodyContent.AppendLine("            <th width='250' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Actual Tooling Completion Date</th>");
            strBodyContent.AppendLine("            <th width='240' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Estimated Sample Shipment Date</th>");
            strBodyContent.AppendLine("            <th width='220' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Actual Sample Shipment Date</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>APQP Status</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Category</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Stage</th>");
            strBodyContent.AppendLine("            <th width='220' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Quality Feedback/Comments</th>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("    </thead>");
            #endregion
            strBodyContent.AppendLine("    <tbody>");
            #region main body loop
            foreach (var item in lstAPQPProjectStatusReport)
            {
                strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("  <td align='left' valign='top' align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>" + item.PartNumber ?? "" + "</td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>" + item.RevLevel ?? "" + "</td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>" + item.Description + "</td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.ProjectKickoffDate.HasValue ? item.ProjectKickoffDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Customer ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.ProjectName ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Manufacturer ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.APQPEngineer ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.ActualToolingKickoffDate.HasValue ? item.ActualToolingKickoffDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.ToolingLeadtime ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.PlannedToolingCompletionDate.HasValue ? item.PlannedToolingCompletionDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.ActualToolingCompletionDate.HasValue ? item.ActualToolingCompletionDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.EstimatedSampleShipmentDate.HasValue ? item.EstimatedSampleShipmentDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine("");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.APQPStatus ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Category ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Stage ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.QualityFeedback ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("</tr>");
            }
            #endregion
            strBodyContent.AppendLine("    </tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "Project Status Report.xls", stringStream);
            }

            return filepath;
        }
        #endregion

        #region project status report Export to Excel
        public NPE.Core.ITypedResponse<bool?> ExportAPQPNewBusinessAwardedReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.APQPNewBusinessAwardedReportSearchCriteria> searchCriteria)
        {
            #region don't consider page size and page number, export all on the basis of search criteria
            searchCriteria.PageNo = 1;
            searchCriteria.PageSize = int.MaxValue;
            #endregion
            string filePath = string.Empty;
            try
            {
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        #endregion

        #region Get New Business Awarded Report
        public ITypedResponse<List<MES.DTO.Library.APQP.APQP.APQPNewBusinessAwardedReport>> GetAPQPNewBusinessAwardedReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.APQPNewBusinessAwardedReportSearchCriteria> searchCriteria)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.APQP.APQP.APQPNewBusinessAwardedReport> lstAPQPNewBusinessAwardedReport = new List<DTO.Library.APQP.APQP.APQPNewBusinessAwardedReport>();
            DTO.Library.APQP.APQP.APQPNewBusinessAwardedReport aPQPNewBusinessAwardedReport;
            this.RunOnDB(context =>
            {
                var APQPNewBusinessAwardedReportList = context.GetNewBusinessAwardReport(searchCriteria.Criteria.WeeklyDateFrom, searchCriteria.Criteria.WeeklyDateTo, searchCriteria.Criteria.ShippingCost).ToList();
                if (APQPNewBusinessAwardedReportList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    searchCriteria.TotalRecords = Convert.ToInt32(totalRecords.Value);

                    foreach (var apqpData in APQPNewBusinessAwardedReportList)
                    {
                        #region assign data
                        aPQPNewBusinessAwardedReport = new DTO.Library.APQP.APQP.APQPNewBusinessAwardedReport();
                        aPQPNewBusinessAwardedReport = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.APQP.APQPNewBusinessAwardedReport>(apqpData);
                        lstAPQPNewBusinessAwardedReport.Add(aPQPNewBusinessAwardedReport);
                        #endregion
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.APQP.APQPNewBusinessAwardedReport>>(errMSg, lstAPQPNewBusinessAwardedReport);
            response.PageInfo = searchCriteria.ToPage();
            return response;
        }
        #endregion

        #region Get Defect Tracking report
        public ITypedResponse<List<MES.DTO.Library.APQP.DefectTracking.DefectTrackingReport>> GetDefectTrackingReport(NPE.Core.IPage<MES.DTO.Library.APQP.DefectTracking.ReportSearchCriteria> searchCriteria)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.APQP.DefectTracking.DefectTrackingReport> lstDefectTrackingReport = new List<DTO.Library.APQP.DefectTracking.DefectTrackingReport>();
            DTO.Library.APQP.DefectTracking.DefectTrackingReport defectTrackingReport;
            DTO.Library.APQP.DefectTracking.DefectTrackingReportDetails defectTrackingReportDetails;
            this.RunOnDB(context =>
            {
                var firstPartData = context.dtGetDefectTrackingReportFirstPart(searchCriteria.Criteria.RMANumber, searchCriteria.Criteria.CustomerName, searchCriteria.Criteria.SupplierName,
                    searchCriteria.Criteria.PartNo, searchCriteria.Criteria.RMAInitiatedBy, searchCriteria.Criteria.CAPANumber, searchCriteria.Criteria.MESWarehouseLocation, searchCriteria.Criteria.SupplierCode,
                    searchCriteria.Criteria.DateFrom, searchCriteria.Criteria.DateTo).ToList();

                var secondPartData = context.dtGetDefectTrackingReportSecondPart(searchCriteria.Criteria.RMANumber, searchCriteria.Criteria.CustomerName, searchCriteria.Criteria.SupplierName,
                    searchCriteria.Criteria.PartNo, searchCriteria.Criteria.RMAInitiatedBy, searchCriteria.Criteria.CAPANumber, searchCriteria.Criteria.MESWarehouseLocation, searchCriteria.Criteria.SupplierCode,
                    searchCriteria.Criteria.DateFrom, searchCriteria.Criteria.DateTo).ToList();

                if (firstPartData == null || secondPartData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var firstPart in firstPartData)
                    {
                        #region assign data
                        defectTrackingReport = new DTO.Library.APQP.DefectTracking.DefectTrackingReport();
                        defectTrackingReport = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.DefectTracking.DefectTrackingReport>(firstPart);
                        defectTrackingReport.lstDefectTrackingReportDetails = new List<DTO.Library.APQP.DefectTracking.DefectTrackingReportDetails>();
                        foreach (var secondPart in secondPartData.Where(a => a.DefectTrackingId == firstPart.Id).ToList())
                        {
                            defectTrackingReportDetails = new DTO.Library.APQP.DefectTracking.DefectTrackingReportDetails();
                            defectTrackingReportDetails = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.DefectTracking.DefectTrackingReportDetails>(secondPart);
                            defectTrackingReport.lstDefectTrackingReportDetails.Add(defectTrackingReportDetails);
                        }
                        lstDefectTrackingReport.Add(defectTrackingReport);
                        #endregion
                    }
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.DefectTracking.DefectTrackingReport>>(errMSg, lstDefectTrackingReport);
            return response;
        }
        #endregion

        #region  Defect Tracking report Export to Excel
        public NPE.Core.ITypedResponse<bool?> ExportDefectTrackingReport(NPE.Core.IPage<MES.DTO.Library.APQP.DefectTracking.ReportSearchCriteria> searchCriteria)
        {
            string filePath = string.Empty;
            try
            {
                filePath = CreateExcelForDefectTrackingReport(searchCriteria);
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private string CreateExcelForDefectTrackingReport(NPE.Core.IPage<MES.DTO.Library.APQP.DefectTracking.ReportSearchCriteria> searchCriteria)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();
            List<MES.DTO.Library.APQP.DefectTracking.DefectTrackingReport> lstDefectTrackingReport = new List<DTO.Library.APQP.DefectTracking.DefectTrackingReport>();
            lstDefectTrackingReport = GetDefectTrackingReport(searchCriteria).Result;

            strBodyContent.AppendLine("<table id='tblId' style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("<thead>");
            strBodyContent.AppendLine("  <tr>");
            strBodyContent.AppendLine("   <td align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='10'>");
            strBodyContent.AppendLine("    <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("     <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("     </font>");
            strBodyContent.AppendLine("    </td>");
            strBodyContent.AppendLine("  </tr>");
            strBodyContent.AppendLine("  <tr style=''>");
            strBodyContent.AppendLine("        <th height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>RMA #</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>RMA Date</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Include in PPM?</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Int. or Ext. Finding?</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Quality or Delivery Issue?</th>");
            strBodyContent.AppendLine("        <th width='200' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Customer</th>");
            strBodyContent.AppendLine("        <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>RMA Initiated By</th>");
            strBodyContent.AppendLine("        <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Part #</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>QTY Rejected (Cust Parts) - PPM</th>");
            strBodyContent.AppendLine("        <th width='200' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Supplier</th>");
            strBodyContent.AppendLine("    </tr>");
            strBodyContent.AppendLine("</thead>");
            #endregion
            strBodyContent.AppendLine("<tbody>");
            #region Main body loop
            foreach (var item in lstDefectTrackingReport)
            {
                strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("    <td align='left' valign='top' align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>" + item.RMANumber ?? "" + "</td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>" + (item.RMADate.HasValue ? item.RMADate.Value.FormatDateInMediumDate() : "") + "</td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.IncludeInPPM ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Finding ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.QualityOrDeliveryIssue ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.CustomerName ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.RMAInitiatedByName ?? "");
                strBodyContent.AppendLine("    </td>");

                #region sub list items data
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                strBodyContent.AppendLine("        <table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
                foreach (var subItem in item.lstDefectTrackingReportDetails)
                {
                    strBodyContent.AppendLine("            <tr>");
                    strBodyContent.AppendLine("                <td style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                    strBodyContent.AppendLine(subItem.PartNumber ?? "");
                    strBodyContent.AppendLine("                </td>");
                    strBodyContent.AppendLine("            </tr>");
                }
                strBodyContent.AppendLine("        </table>");
                strBodyContent.AppendLine("    </td>");
                #endregion

                #region sub list items data
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                strBodyContent.AppendLine("        <table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
                foreach (var subItem in item.lstDefectTrackingReportDetails)
                {
                    strBodyContent.AppendLine("            <tr>");
                    strBodyContent.AppendLine("                <td style='text-align:right;color:#454545;font-size:13px; padding:10px;'>");
                    strBodyContent.AppendLine(Convert.ToString(subItem.CustomerRejectedPartQty));
                    strBodyContent.AppendLine("                </td>");
                    strBodyContent.AppendLine("            </tr>");
                }
                strBodyContent.AppendLine("        </table>");
                strBodyContent.AppendLine("    </td>");
                #endregion

                #region sub list items data
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                strBodyContent.AppendLine("        <table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
                foreach (var subItem in item.lstDefectTrackingReportDetails)
                {
                    strBodyContent.AppendLine("            <tr>");
                    strBodyContent.AppendLine("                <td style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                    strBodyContent.AppendLine(subItem.SupplierName ?? "");
                    strBodyContent.AppendLine("                </td>");
                    strBodyContent.AppendLine("            </tr>");
                }
                strBodyContent.AppendLine("        </table>");
                strBodyContent.AppendLine("    </td>");
                #endregion

                strBodyContent.AppendLine("</tr>");
            }
            #endregion
            strBodyContent.AppendLine("</tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "Defect Tracking Report.xls", stringStream);
            }

            return filepath;
        }
        #endregion

        #region get APQP PPPAP Approval Report
        public ITypedResponse<List<MES.DTO.Library.APQP.APQP.PPAPApprovalReport>> GetAPQPPPAPApprovalReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.PPAPApprovalReportSearchCriteria> searchCriteria)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.APQP.APQP.PPAPApprovalReport> lstPPAPApprovalReport = new List<DTO.Library.APQP.APQP.PPAPApprovalReport>();
            DTO.Library.APQP.APQP.PPAPApprovalReport pPAPApprovalReport;
            this.RunOnDB(context =>
            {
                var PPAPApprovalReportList = context.GetAPQPPPAPApprovedData(searchCriteria.Criteria.DateFrom, searchCriteria.Criteria.DateTo, searchCriteria.Criteria.CustomerName,
                    searchCriteria.Criteria.APQPQualityEngineerIds, searchCriteria.Criteria.SupplyChainCoordinatorIds, searchCriteria.Criteria.SAMUserIds,
                    searchCriteria.Criteria.ProjectStageIds, searchCriteria.Criteria.PPAPStatusText, searchCriteria.PageNo, searchCriteria.PageSize, totalRecords, "").ToList();
                if (PPAPApprovalReportList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    searchCriteria.TotalRecords = Convert.ToInt32(totalRecords.Value);

                    foreach (var apqpData in PPAPApprovalReportList)
                    {
                        #region assign data
                        pPAPApprovalReport = new DTO.Library.APQP.APQP.PPAPApprovalReport();
                        pPAPApprovalReport = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.APQP.PPAPApprovalReport>(apqpData);
                        lstPPAPApprovalReport.Add(pPAPApprovalReport);
                        #endregion
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.APQP.PPAPApprovalReport>>(errMSg, lstPPAPApprovalReport);
            response.PageInfo = searchCriteria.ToPage();
            return response;
        }
        #endregion

        #region APQP PPPAP Approval Report Export to Excel
        public NPE.Core.ITypedResponse<bool?> ExportAPQPPPAPApprovalReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.PPAPApprovalReportSearchCriteria> searchCriteria)
        {
            #region don't consider page size and page number, export all on the basis of search criteria
            searchCriteria.PageNo = 1;
            searchCriteria.PageSize = int.MaxValue;
            #endregion
            string filePath = string.Empty;
            try
            {
                filePath = CreateExcelForAPQPPPAPApprovalReport(searchCriteria);
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private string CreateExcelForAPQPPPAPApprovalReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.PPAPApprovalReportSearchCriteria> searchCriteria)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();
            List<MES.DTO.Library.APQP.APQP.PPAPApprovalReport> lstPPAPApprovalReport = new List<DTO.Library.APQP.APQP.PPAPApprovalReport>();
            lstPPAPApprovalReport = GetAPQPPPAPApprovalReport(searchCriteria).Result;

            strBodyContent.AppendLine("<table id='tblId' style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("    <thead>");
            strBodyContent.AppendLine("        <tr>");
            strBodyContent.AppendLine("            <td align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='15'>");
            strBodyContent.AppendLine("                <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("                <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("                </font>");
            strBodyContent.AppendLine("            </td>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("        <tr style=''>");
            strBodyContent.AppendLine("            <th height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Part Number</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Project Kickoff Date</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Actual Tooling Kickoff Date</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Tooling Leadtime</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>APQP Engineer</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Supply Chain Coordinator</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>MES SAM</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Customer Name</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Planned Tooling Completion Date</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Actual Tooling Completion Date</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Planned PSW Date</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Actual PSW Date</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Actual PPAP Parts Approved Date</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>PPAP Status</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>EAU (usage)</th>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("    </thead>");
            #endregion
            strBodyContent.AppendLine("    <tbody>");
            #region main body loop
            foreach (var item in lstPPAPApprovalReport)
            {
                strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("  <td align='left' valign='top' align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>" + item.PartNumber ?? "" + "</td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.ProjectKickoffDate.HasValue ? item.ProjectKickoffDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.ToolingKickoffDate.HasValue ? item.ToolingKickoffDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.ToolingLeadtimeDays ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.APQPEngineer ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.SupplyChainCoordinator ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.SalesAccountManager ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.CustomerName ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.PlanToolingCompletionDate.HasValue ? item.PlanToolingCompletionDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.ActualToolingCompletionDate.HasValue ? item.ActualToolingCompletionDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.PSWDate.HasValue ? item.PSWDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.ActualPSWDate.HasValue ? item.ActualPSWDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.PPAPPartsApprovedDate.HasValue ? item.PPAPPartsApprovedDate.Value.FormatDateInMediumDate() : "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.PPAPStatus ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.EAUUsage ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("</tr>");
            }
            #endregion
            strBodyContent.AppendLine("    </tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "PPAP Approval Report.xls", stringStream);
            }

            return filepath;
        }
        #endregion

        #region get APQP Weekly meeting Report and Export
        public ITypedResponse<MES.DTO.Library.APQP.APQP.APQPWeeklyMeetingReportViewModel> GetAPQPWeeklyMeetingReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.APQPWeeklyMeetingReportSearchCriteria> searchCriteria)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            DTO.Library.APQP.APQP.APQPWeeklyMeetingReport aPQPWeeklyMeetingReport;
            //List<DTO.Library.APQP.APQP.APQPWeeklyMeetingReportViewModel> lstAPQPWeeklyMeetingReportViewModel = new List<DTO.Library.APQP.APQP.APQPWeeklyMeetingReportViewModel>();
            DTO.Library.APQP.APQP.APQPWeeklyMeetingReportViewModel objAPQPWeeklyMeetingReportViewModel = new DTO.Library.APQP.APQP.APQPWeeklyMeetingReportViewModel();
            List<DTO.Library.APQP.APQP.NameValue> APQPEngineersNameValue = new List<DTO.Library.APQP.APQP.NameValue>();
            DTO.Library.APQP.APQP.CustomerNameValue objCustomerNameValue;
            DTO.Library.APQP.APQP.NameValue objNameValue;
            string strComments = string.Empty;
            this.RunOnDB(context =>
            {
                var APQPWeeklyMeetingReportPartAList = context.GetAPQPWeeklyMeetingReportPartA(searchCriteria.Criteria.DateFrom, searchCriteria.Criteria.DateTo, searchCriteria.PageNo, searchCriteria.PageSize, totalRecords, "").ToList();
                var APQPWeeklyMeetingReportPartBList = context.GetAPQPWeeklyMeetingReportPartB(searchCriteria.Criteria.DateFrom, searchCriteria.Criteria.DateTo, searchCriteria.PageNo, searchCriteria.PageSize, totalRecords, "").ToList();
                var APQPWeeklyMeetingReportPartCList = context.GetAPQPWeeklyMeetingReportPartC(searchCriteria.Criteria.DateFrom, searchCriteria.Criteria.DateTo, searchCriteria.PageNo, searchCriteria.PageSize, totalRecords, "").ToList();
                var APQPWeeklyMeetingReportPartDList = context.GetAPQPWeeklyMeetingReportPartD(searchCriteria.Criteria.DateFrom, searchCriteria.Criteria.DateTo, searchCriteria.PageNo, searchCriteria.PageSize, totalRecords, "").ToList();

                objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartA = new List<DTO.Library.APQP.APQP.APQPWeeklyMeetingReport>();
                objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartB = new List<DTO.Library.APQP.APQP.APQPWeeklyMeetingReport>();
                objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartC = new List<DTO.Library.APQP.APQP.APQPWeeklyMeetingReport>();
                objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartD = new List<DTO.Library.APQP.APQP.APQPWeeklyMeetingReport>();
                foreach (var partA in APQPWeeklyMeetingReportPartAList.GroupBy(a => a.APQPEngineer))
                {
                    aPQPWeeklyMeetingReport = new DTO.Library.APQP.APQP.APQPWeeklyMeetingReport();
                    aPQPWeeklyMeetingReport.APQPEngineer = partA.Key;
                    aPQPWeeklyMeetingReport.APQPEngineerId = partA.First().APQPEngineerId;
                    aPQPWeeklyMeetingReport.PartNumberCount = partA.ToList().Count;
                    aPQPWeeklyMeetingReport.PartNumber = string.Join(", ", partA.ToList().Select(x => x.PartNumber));
                    objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartA.Add(aPQPWeeklyMeetingReport);
                    if (!APQPEngineersNameValue.Any(a => a.Name == aPQPWeeklyMeetingReport.APQPEngineer))
                        APQPEngineersNameValue.Add(new DTO.Library.APQP.APQP.NameValue { Name = aPQPWeeklyMeetingReport.APQPEngineer, Value = aPQPWeeklyMeetingReport.APQPEngineerId });
                }
                foreach (var partB in APQPWeeklyMeetingReportPartBList.GroupBy(a => a.APQPEngineer))
                {
                    aPQPWeeklyMeetingReport = new DTO.Library.APQP.APQP.APQPWeeklyMeetingReport();
                    aPQPWeeklyMeetingReport.APQPEngineer = partB.Key;
                    aPQPWeeklyMeetingReport.APQPEngineerId = partB.First().APQPEngineerId;
                    aPQPWeeklyMeetingReport.PartNumberCount = partB.ToList().Count;
                    aPQPWeeklyMeetingReport.PartNumber = string.Join(", ", partB.ToList().Select(x => x.PartNumber));
                    objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartB.Add(aPQPWeeklyMeetingReport);
                    if (!APQPEngineersNameValue.Any(a => a.Name == aPQPWeeklyMeetingReport.APQPEngineer))
                        APQPEngineersNameValue.Add(new DTO.Library.APQP.APQP.NameValue { Name = aPQPWeeklyMeetingReport.APQPEngineer, Value = aPQPWeeklyMeetingReport.APQPEngineerId });
                }
                foreach (var partC in APQPWeeklyMeetingReportPartCList.GroupBy(a => a.APQPEngineer))
                {
                    aPQPWeeklyMeetingReport = new DTO.Library.APQP.APQP.APQPWeeklyMeetingReport();
                    aPQPWeeklyMeetingReport.APQPEngineer = partC.Key;
                    aPQPWeeklyMeetingReport.APQPEngineerId = partC.First().APQPEngineerId;
                    aPQPWeeklyMeetingReport.PartNumberCount = partC.ToList().Count;
                    aPQPWeeklyMeetingReport.PartNumber = string.Join(", ", partC.ToList().Select(x => x.PartNumber));
                    objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartC.Add(aPQPWeeklyMeetingReport);
                    if (!APQPEngineersNameValue.Any(a => a.Name == aPQPWeeklyMeetingReport.APQPEngineer))
                        APQPEngineersNameValue.Add(new DTO.Library.APQP.APQP.NameValue { Name = aPQPWeeklyMeetingReport.APQPEngineer, Value = aPQPWeeklyMeetingReport.APQPEngineerId });
                }
                foreach (var partD in APQPWeeklyMeetingReportPartDList.GroupBy(a => a.APQPEngineer))
                {
                    aPQPWeeklyMeetingReport = new DTO.Library.APQP.APQP.APQPWeeklyMeetingReport();
                    aPQPWeeklyMeetingReport.APQPEngineer = partD.Key;
                    aPQPWeeklyMeetingReport.APQPEngineerId = partD.First().APQPEngineerId;
                    aPQPWeeklyMeetingReport.PartNumberCount = partD.ToList().Count;
                    aPQPWeeklyMeetingReport.PartNumber = string.Join(", ", partD.ToList().Select(x => x.PartNumber));
                    aPQPWeeklyMeetingReport.objCustomerNameValue = new List<DTO.Library.APQP.APQP.CustomerNameValue>();
                    foreach (var partlist in partD.GroupBy(b => b.CustomerName))
                    {
                        objCustomerNameValue = new DTO.Library.APQP.APQP.CustomerNameValue();
                        objCustomerNameValue.CustomerName = partlist.Key;
                        objCustomerNameValue.lstNameValue = new List<DTO.Library.APQP.APQP.NameValue>();
                        strComments = string.Empty;
                        foreach (var parts in partlist)
                        {
                            objNameValue = new DTO.Library.APQP.APQP.NameValue();
                            objNameValue.Name = parts.PartNumber;

                            #region merge all comments from all APQP tabs
                            strComments = parts.KickOffComments;
                            if (string.IsNullOrEmpty(strComments))
                                strComments = parts.ToolingLaunchComments;
                            else
                                strComments = strComments + (string.IsNullOrEmpty(parts.ToolingLaunchComments) ? "" : ", " + parts.ToolingLaunchComments);
                            if (string.IsNullOrEmpty(strComments))
                                strComments = parts.ProjectTrackingComments;
                            else
                                strComments = strComments + (string.IsNullOrEmpty(parts.ProjectTrackingComments) ? "" : ", " + parts.ProjectTrackingComments);
                            if (string.IsNullOrEmpty(strComments))
                                strComments = parts.ProjectTrackingRemarks;
                            else
                                strComments = strComments + (string.IsNullOrEmpty(parts.ProjectTrackingRemarks) ? "" : ", " + parts.ProjectTrackingRemarks);
                            if (string.IsNullOrEmpty(strComments))
                                strComments = parts.PPAPComments;
                            else
                                strComments = strComments + (string.IsNullOrEmpty(parts.PPAPComments) ? "" : ", " + parts.PPAPComments);
                            #endregion

                            objNameValue.Value = strComments;
                            objCustomerNameValue.lstNameValue.Add(objNameValue);
                        }
                        aPQPWeeklyMeetingReport.objCustomerNameValue.Add(objCustomerNameValue);
                    }

                    objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartD.Add(aPQPWeeklyMeetingReport);
                    if (!APQPEngineersNameValue.Any(a => a.Name == aPQPWeeklyMeetingReport.APQPEngineer))
                        APQPEngineersNameValue.Add(new DTO.Library.APQP.APQP.NameValue { Name = aPQPWeeklyMeetingReport.APQPEngineer, Value = aPQPWeeklyMeetingReport.APQPEngineerId });
                }
                foreach (var objAPQPEngineersNameValue in APQPEngineersNameValue)
                {
                    if (!objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartA.Any(a => a.APQPEngineer == objAPQPEngineersNameValue.Name))
                    {
                        aPQPWeeklyMeetingReport = new DTO.Library.APQP.APQP.APQPWeeklyMeetingReport();
                        aPQPWeeklyMeetingReport.APQPEngineer = objAPQPEngineersNameValue.Name;
                        aPQPWeeklyMeetingReport.APQPEngineerId = objAPQPEngineersNameValue.Value;
                        aPQPWeeklyMeetingReport.PartNumberCount = 0;
                        aPQPWeeklyMeetingReport.PartNumber = "";
                        objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartA.Add(aPQPWeeklyMeetingReport);
                    }
                    if (!objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartB.Any(a => a.APQPEngineer == objAPQPEngineersNameValue.Name))
                    {
                        aPQPWeeklyMeetingReport = new DTO.Library.APQP.APQP.APQPWeeklyMeetingReport();
                        aPQPWeeklyMeetingReport.APQPEngineer = objAPQPEngineersNameValue.Name;
                        aPQPWeeklyMeetingReport.APQPEngineerId = objAPQPEngineersNameValue.Value;
                        aPQPWeeklyMeetingReport.PartNumberCount = 0;
                        aPQPWeeklyMeetingReport.PartNumber = "";
                        objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartB.Add(aPQPWeeklyMeetingReport);
                    }
                    if (!objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartC.Any(a => a.APQPEngineer == objAPQPEngineersNameValue.Name))
                    {
                        aPQPWeeklyMeetingReport = new DTO.Library.APQP.APQP.APQPWeeklyMeetingReport();
                        aPQPWeeklyMeetingReport.APQPEngineer = objAPQPEngineersNameValue.Name;
                        aPQPWeeklyMeetingReport.APQPEngineerId = objAPQPEngineersNameValue.Value;
                        aPQPWeeklyMeetingReport.PartNumberCount = 0;
                        aPQPWeeklyMeetingReport.PartNumber = "";
                        objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartC.Add(aPQPWeeklyMeetingReport);
                    }
                    if (!objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartD.Any(a => a.APQPEngineer == objAPQPEngineersNameValue.Name))
                    {
                        aPQPWeeklyMeetingReport = new DTO.Library.APQP.APQP.APQPWeeklyMeetingReport();
                        aPQPWeeklyMeetingReport.APQPEngineer = objAPQPEngineersNameValue.Name;
                        aPQPWeeklyMeetingReport.APQPEngineerId = objAPQPEngineersNameValue.Value;
                        aPQPWeeklyMeetingReport.PartNumberCount = 0;
                        aPQPWeeklyMeetingReport.PartNumber = "";
                        objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartD.Add(aPQPWeeklyMeetingReport);
                    }
                }
                objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartA = objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartA.OrderBy(a => a.APQPEngineer).ToList();
                objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartB = objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartB.OrderBy(a => a.APQPEngineer).ToList();
                objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartC = objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartC.OrderBy(a => a.APQPEngineer).ToList();
                objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartD = objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartD.OrderBy(a => a.APQPEngineer).ToList();


            });

            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.APQP.APQP.APQPWeeklyMeetingReportViewModel>(errMSg, objAPQPWeeklyMeetingReportViewModel);
            response.PageInfo = searchCriteria.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> ExportAPQPWeeklyMeetingReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.APQPWeeklyMeetingReportSearchCriteria> searchCriteria)
        {
            #region don't consider page size and page number, export all on the basis of search criteria
            searchCriteria.PageNo = 1;
            searchCriteria.PageSize = int.MaxValue;
            #endregion
            string filePath = string.Empty;
            try
            {
                filePath = CreateExcelForAPQPWeeklyMeetingReport(searchCriteria);
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private string CreateExcelForAPQPWeeklyMeetingReport(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.APQPWeeklyMeetingReportSearchCriteria> searchCriteria)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();
            MES.DTO.Library.APQP.APQP.APQPWeeklyMeetingReportViewModel objAPQPWeeklyMeetingReportViewModel = new DTO.Library.APQP.APQP.APQPWeeklyMeetingReportViewModel();
            objAPQPWeeklyMeetingReportViewModel = GetAPQPWeeklyMeetingReport(searchCriteria).Result;

            strBodyContent.AppendLine("<table id='tblId' style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("    <thead>");
            strBodyContent.AppendLine("        <tr>");
            strBodyContent.AppendLine("            <td align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='4'>");
            strBodyContent.AppendLine("                <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("                <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("                </font>");
            strBodyContent.AppendLine("            </td>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("        <tr style=''>");
            strBodyContent.AppendLine("            <th width='10%' height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'></th>");
            strBodyContent.AppendLine("            <th width='30%' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>New Parts Kicked Off</th>");
            strBodyContent.AppendLine("            <th width='30%' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Parts PPAP Approved</th>");
            strBodyContent.AppendLine("            <th width='30%' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Total Projects in Development</th>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("    </thead>");
            #endregion
            strBodyContent.AppendLine("    <tbody>");

            #region main body loop
            strBodyContent.AppendLine("<tr>");
            strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
            #region sub list items data
            strBodyContent.AppendLine("        <table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            foreach (var subItem in objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartA)
            {
                strBodyContent.AppendLine("            <tr>");
                strBodyContent.AppendLine("                <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(subItem.APQPEngineer ?? "");
                strBodyContent.AppendLine("                </td>");
                strBodyContent.AppendLine("            </tr>");
            }
            strBodyContent.AppendLine("        </table>");
            #endregion
            strBodyContent.AppendLine("  </td>");

            strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
            #region sub list items data
            strBodyContent.AppendLine("        <table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            foreach (var subItem in objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartA)
            {
                strBodyContent.AppendLine("            <tr>");
                strBodyContent.AppendLine("                <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine((Convert.ToString(subItem.PartNumberCount) + "(" + subItem.PartNumber + ")") ?? "");
                strBodyContent.AppendLine("                </td>");
                strBodyContent.AppendLine("            </tr>");
            }
            strBodyContent.AppendLine("        </table>");
            #endregion
            strBodyContent.AppendLine("  </td>");

            strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
            #region sub list items data
            strBodyContent.AppendLine("        <table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            foreach (var subItem in objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartB)
            {
                strBodyContent.AppendLine("            <tr>");
                strBodyContent.AppendLine("                <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine((Convert.ToString(subItem.PartNumberCount) + "(" + subItem.PartNumber + ")") ?? "");
                strBodyContent.AppendLine("                </td>");
                strBodyContent.AppendLine("            </tr>");
            }
            strBodyContent.AppendLine("        </table>");
            #endregion
            strBodyContent.AppendLine("  </td>");

            strBodyContent.AppendLine("  <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
            #region sub list items data
            strBodyContent.AppendLine("        <table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            foreach (var subItem in objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartC)
            {
                strBodyContent.AppendLine("            <tr>");
                strBodyContent.AppendLine("                <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine((Convert.ToString(subItem.PartNumberCount) + "(" + subItem.PartNumber + ")") ?? "");
                strBodyContent.AppendLine("                </td>");
                strBodyContent.AppendLine("            </tr>");
            }
            strBodyContent.AppendLine("        </table>");
            #endregion
            strBodyContent.AppendLine("  </td>");
            strBodyContent.AppendLine("</tr>");

            strBodyContent.AppendLine("<tr>");
            strBodyContent.AppendLine("  <td colspan='4' valign='top' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Number of Parts submitted for PPAP Approval, but not Approved Yet</td>");
            strBodyContent.AppendLine("</tr>");

            #region sub list items data
            foreach (var subItem in objAPQPWeeklyMeetingReportViewModel.lstAPQPWeeklyMeetingReportPartD)
            {
                strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("  <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(subItem.APQPEngineer ?? "");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("  <td colspan='3' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine("        <table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
                if (subItem.objCustomerNameValue != null && subItem.objCustomerNameValue.Count > 0)
                {
                    foreach (var customerNameValue in subItem.objCustomerNameValue)
                    {
                        strBodyContent.AppendLine("  <tr>");
                        strBodyContent.AppendLine("   <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                        strBodyContent.AppendLine(customerNameValue.CustomerName ?? "");
                        strBodyContent.AppendLine("   </td>");
                        strBodyContent.AppendLine("   <td colspan='2' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                        strBodyContent.AppendLine("        <table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
                        if (customerNameValue.lstNameValue != null && customerNameValue.lstNameValue.Count > 0)
                        {
                            foreach (var NameValue in customerNameValue.lstNameValue)
                            {
                                strBodyContent.AppendLine("            <tr>");
                                strBodyContent.AppendLine("                <td colspan='2' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                                strBodyContent.AppendLine(NameValue.Name + " - " + NameValue.Value);
                                strBodyContent.AppendLine("                </td>");
                                strBodyContent.AppendLine("            </tr>");
                            }
                        }
                        else
                        {
                            strBodyContent.AppendLine("            <tr>");
                            strBodyContent.AppendLine("                <td colspan='2' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                            strBodyContent.AppendLine("                </td>");
                            strBodyContent.AppendLine("            </tr>");
                        }
                        strBodyContent.AppendLine("        </table>");
                        strBodyContent.AppendLine("   </td>");
                        strBodyContent.AppendLine("</tr>");
                    }
                }
                else
                {
                    strBodyContent.AppendLine("            <tr>");
                    strBodyContent.AppendLine("                <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                    strBodyContent.AppendLine("                </td>");
                    strBodyContent.AppendLine("            </tr>");
                }
                strBodyContent.AppendLine("        </table>");
                strBodyContent.AppendLine("  </td>");
                strBodyContent.AppendLine("</tr>");
            }
            #endregion

            #endregion
            strBodyContent.AppendLine("    </tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "Weekly Meeting Report.xls", stringStream);
            }

            return filepath;
        }
        #endregion

        #region get Next Or Previous APQPItemId
        public NPE.Core.ITypedResponse<int?> getNextOrPreviousAPQPItemId(NPE.Core.IPage<DTO.Library.APQP.APQP.SearchCriteria> searchCriteria)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            int? apqpItemId = 0;
            this.RunOnDB(context =>
             {
                 var data = context.getNextOrPreviousAPQPItemId(searchCriteria.Criteria.RFQNumber, searchCriteria.Criteria.QuoteNumber, searchCriteria.Criteria.CustomerName, searchCriteria.Criteria.PartNo,
                     searchCriteria.Criteria.ProjectName, searchCriteria.Criteria.APQPStatusIds, searchCriteria.Criteria.SAMUserId, searchCriteria.Criteria.APQPQualityEngineerId,
                     searchCriteria.Criteria.SupplyChainCoordinatorId, searchCriteria.Criteria.AllowConfidentialDocumentType, searchCriteria.PageNo, searchCriteria.PageSize, totalRecords, "").SingleOrDefault();
                 if (data == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     if (data.HasValue && Convert.ToInt32(data.Value) > 0)
                         apqpItemId = Convert.ToInt32(data.Value);
                 }
             });

            //get hold of response
            var response = SuccessOrFailedResponse<int?>(errMSg, apqpItemId);
            //populate page property
            response.PageInfo = searchCriteria.ToPage();
            return response;
        }
        #endregion

        #region Update Individual APQP Fields
        public NPE.Core.ITypedResponse<bool?> UpdateIndividualFields(DTO.Library.APQP.APQP.UpdateIndividualFields updateIndividualFields)
        {
            string successMsg = null;
            this.RunOnDB(context =>
            {
                context.UpdateIndividualFields(updateIndividualFields.DrawingNumber, updateIndividualFields.RevLevel, updateIndividualFields.StatusId,
                    updateIndividualFields.FieldName, updateIndividualFields.UpdatedFromSource, updateIndividualFields.APQPItemId, CurrentUser);


                switch (updateIndividualFields.FieldName)
                {
                    case "APQPStatus": successMsg = Languages.GetResourceText("StatusUpdatedSuccess");
                        break;
                    case "RevLevel": successMsg = Languages.GetResourceText("RevLevelSuccess");
                        break;
                    case "DrawingNumber": successMsg = Languages.GetResourceText("DrawingNumberSuccess");
                        break;

                }

            });
            return SuccessOrFailedResponse<bool?>(null, true, successMsg);
        }
        #endregion

        #region Send email
        public NPE.Core.ITypedResponse<MES.DTO.Library.Common.EmailData> getTemplateAndUserIdsData(string APQPItemIds, string SectionName)
        {
            #region set variables
            List<string> APQPEngineerIdAndSupplierChainCoordinatorIds = new List<string>();
            string errMSg = string.Empty;
            string startTableStr = string.Empty, NewPartFormInfo = string.Empty, endTableStr = string.Empty, SupplierLocation = string.Empty;
            string TriggerPointName = string.Empty, ShortCode = string.Empty, PartNumbers = string.Empty;
            DTO.Library.Common.EmailData emailData = new DTO.Library.Common.EmailData();
            switch (SectionName)
            {
                case "APQPSTEP1":
                    TriggerPointName = Constants.TriggerPointAPQPStep1;
                    ShortCode = Constants.APQPEmailStep1;
                    break;
                case "APQPSTEP2":
                    TriggerPointName = Constants.TriggerPointAPQPStep2;
                    ShortCode = Constants.APQPEmailStep2;
                    break;
                case "APQPSTEP3":
                    TriggerPointName = Constants.TriggerPointAPQPStep3;
                    ShortCode = Constants.APQPEmailStep4;
                    break;
                case "APQPSTEP4":
                    TriggerPointName = Constants.TriggerPointAPQPStep4;
                    ShortCode = Constants.APQPEmailStep5;
                    break;
                default:
                    TriggerPointName = string.Empty;
                    break;
            }
            #endregion

            this.RunOnDB(context =>
            {
                #region get email template
                var emailTemplateData = context.GetEmailTemplates(ShortCode).SingleOrDefault();
                if (emailTemplateData != null)
                {
                    emailData.EmailSubject = emailTemplateData.EmailSubject + ""; // part numbers
                    emailData.EmailBody = emailTemplateData.EmailBody.Trim();

                    startTableStr = @"<table width='100%' cellspacing='0' cellpadding='1' style='padding-left: 3px;'><tbody><tr>
							<td align='left' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; white-space: nowrap; background-color: #e1dbd1;'>
							<label style='color: #635857; font-family: Tahoma,Geneva,sans-serif; font-size: 10px; text-transform: uppercase; font-weight: bold;'>Part Number</label>
							</td>
                            <td align='left' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; white-space: nowrap; background-color: #e1dbd1;'>
							<label style='color: #635857; font-family: Tahoma,Geneva,sans-serif; font-size: 10px; text-transform: uppercase; font-weight: bold;'>Revision Level</label>
							</td>
							<td align='left' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; white-space: nowrap; background-color: #e1dbd1;'>
							<label style='color: #635857; font-family: Tahoma,Geneva,sans-serif; font-size: 10px; text-transform: uppercase; font-weight: bold;'>Part Name</label>
							</td>
							<td align='left' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; white-space: nowrap; background-color: #e1dbd1;'>
							<label style='color: #635857; font-family: Tahoma,Geneva,sans-serif; font-size: 10px; text-transform: uppercase; font-weight: bold;'>Program</label>
							</td>
							<td align='left' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; white-space: nowrap; background-color: #e1dbd1;'>
							<label style='color: #635857; font-family: Tahoma,Geneva,sans-serif; font-size: 10px; text-transform: uppercase; font-weight: bold;'>RFQ Number</label>
							</td>
							<td align='left' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; white-space: nowrap; background-color: #e1dbd1;'>
							<label style='color: #635857; font-family: Tahoma,Geneva,sans-serif; font-size: 10px; text-transform: uppercase; font-weight: bold;'>Customer</label>
							</td>
							<td align='left' style='border-bottom: 1px solid #000000; border-top: 1px solid #000000; border-left: 1px solid #000000; white-space: nowrap; background-color: #e1dbd1;'>
							<label style='color: #635857; font-family: Tahoma,Geneva,sans-serif; font-size: 10px; text-transform: uppercase; font-weight: bold;'>Customer Plant(s)</label>
							</td>
							<td align='left' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; white-space: nowrap; background-color: #e1dbd1;'>
							<label style='color: #635857; font-family: Tahoma,Geneva,sans-serif; font-size: 10px; text-transform: uppercase; font-weight: bold;'>Supplier Code</label>
							</td>
							<td align='left' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; white-space: nowrap; background-color: #e1dbd1;'>
							<label style='color: #635857; font-family: Tahoma,Geneva,sans-serif; font-size: 10px; text-transform: uppercase; font-weight: bold;'>Supplier Name</label>
							</td>
							<td align='left' style='border-left: 1px solid #000000; border-top: 1px solid #000000; border-bottom: 1px solid #000000; white-space: nowrap; background-color: #e1dbd1;'>
							<label style='color: #635857; font-family: Tahoma,Geneva,sans-serif; font-size: 10px; text-transform: uppercase; font-weight: bold;'>Supplier Location</label>
							</td>
							<td align='left' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; white-space: nowrap; background-color: #e1dbd1;'>
							<label style='color: #635857; font-family: Tahoma,Geneva,sans-serif; font-size: 10px; text-transform: uppercase; font-weight: bold;'>Quote Number</label>
							</td>
							<td align='left' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: 1px solid #000000; white-space: nowrap; background-color: #e1dbd1;'>
							<label style='color: #635857; font-family: Tahoma,Geneva,sans-serif; font-size: 10px; text-transform: uppercase; font-weight: bold;'>Tooling Start Date</label>
							</td>							
							</tr>";

                    if (!string.IsNullOrEmpty(APQPItemIds))
                    {
                        DTO.Library.APQP.APQP.PSWItem apqpPSWData = null;
                        foreach (string id in APQPItemIds.Split(','))
                        {
                            apqpPSWData = new DTO.Library.APQP.APQP.PSWItem();
                            apqpPSWData = getGeneratePSWData(Convert.ToInt32(id)).Result;

                            //set part numbers in subject line                                                          
                            if (string.IsNullOrEmpty(PartNumbers))
                                PartNumbers = apqpPSWData.PartNumber;
                            else
                                PartNumbers = PartNumbers + ", " + apqpPSWData.PartNumber;

                            //set ApqpEngineerId and supplier chain coordinator id
                            if (!string.IsNullOrEmpty(apqpPSWData.APQPEngineerId))
                                APQPEngineerIdAndSupplierChainCoordinatorIds.Add(apqpPSWData.APQPEngineerId);
                            if (!string.IsNullOrEmpty(apqpPSWData.SupplyChainCoordinatorId))
                                APQPEngineerIdAndSupplierChainCoordinatorIds.Add(apqpPSWData.SupplyChainCoordinatorId);

                            SupplierLocation = "";
                            if (!string.IsNullOrEmpty(apqpPSWData.ManufacturerCity))
                                SupplierLocation = apqpPSWData.ManufacturerCity.Trim();

                            if (!string.IsNullOrEmpty(apqpPSWData.ManufacturerState))
                                SupplierLocation = !string.IsNullOrEmpty(SupplierLocation) ? SupplierLocation.Trim() + " " + apqpPSWData.ManufacturerState.Trim() : apqpPSWData.ManufacturerState.Trim();

                            if (!string.IsNullOrEmpty(apqpPSWData.ManufacturerCountry))
                                SupplierLocation = !string.IsNullOrEmpty(SupplierLocation) ? SupplierLocation.Trim() + " " + apqpPSWData.ManufacturerCountry.Trim() : apqpPSWData.ManufacturerCountry.Trim();

                            //Customer Plant(s)
                            string CustomerLocation = apqpPSWData.CustomerManufacturingLocation;//string.IsNullOrEmpty(apqpPSWData.CustomerState) ? apqpPSWData.CustomerCity : (string.IsNullOrEmpty(apqpPSWData.CustomerCity) ? apqpPSWData.CustomerState : apqpPSWData.CustomerCity + " " + apqpPSWData.CustomerState);
                            NewPartFormInfo += @"<tr>
                                <td align='left' style='border-left: 1px solid #000000; border-bottom: 1px solid #000000;'>
                                <span style='font-size: 11px; text-transform: uppercase; font-family: Tahoma,Geneva,sans-serif; color: #000000;'>"
                                     + apqpPSWData.PartNumber + "</span></td>";
                            NewPartFormInfo += @"
                                <td align='left' style='border-left: 1px solid #bbbbc3; border-bottom: 1px solid #000000;'>
                                <span style='font-size: 11px; text-transform: uppercase; font-family: Tahoma,Geneva,sans-serif; color: #000000;'>"
                                     + apqpPSWData.RevLevel + "</span></td>";
                            NewPartFormInfo += @"
                                <td align='left' style='border-left: 1px solid #bbbbc3; border-bottom: 1px solid #000000;'>
                                <span style='font-size: 11px; text-transform: uppercase; font-family: Tahoma,Geneva,sans-serif; color: #000000;'>"
                                        + apqpPSWData.PartDesc + "</span></td>";
                            NewPartFormInfo += @"
                                <td align='left' style='border-left: 1px solid #bbbbc3; border-bottom: 1px solid #000000;'>
                                <span style='font-size: 11px; text-transform: uppercase; font-family: Tahoma,Geneva,sans-serif; color: #000000;'>"
                                        + apqpPSWData.ProjectName + "</span></td>";
                            NewPartFormInfo += @"
                                <td align='left' style='border-left: 1px solid #bbbbc3; border-bottom: 1px solid #000000;'>
                                <span style='font-size: 11px; text-transform: uppercase; font-family: Tahoma,Geneva,sans-serif; color: #000000;'>"
                                        + apqpPSWData.RFQNumber + "</span></td>";
                            NewPartFormInfo += @"
                                <td align='left' style='border-left: 1px solid #bbbbc3; border-bottom: 1px solid #000000;'>
                                <span style='font-size: 11px; text-transform: uppercase; font-family: Tahoma,Geneva,sans-serif; color: #000000;'>"
                                        + apqpPSWData.CustomerName + "</span></td>";
                            NewPartFormInfo += @"
                                <td align='left' style='border-left: 1px solid #bbbbc3; border-bottom: 1px solid #000000;'>
                                <span style='font-size: 11px; text-transform: uppercase; font-family: Tahoma,Geneva,sans-serif; color: #000000;'>"
                                        + CustomerLocation + "</span></td>";
                            NewPartFormInfo += @"
                                <td align='left' style='border-left: 1px solid #bbbbc3; border-bottom: 1px solid #000000;'>
                                <span style='font-size: 11px; text-transform: uppercase; font-family: Tahoma,Geneva,sans-serif; color: #000000;'>"
                                        + apqpPSWData.SupplierCode + "</span></td>";
                            NewPartFormInfo += @"
                                <td align='left' style='border-left: 1px solid #bbbbc3; border-bottom: 1px solid #000000;'>
                                <span style='font-size: 11px; text-transform: uppercase; font-family: Tahoma,Geneva,sans-serif; color: #000000;'>"
                                        + apqpPSWData.SupplierName + "</span></td>";
                            NewPartFormInfo += @"
                                <td align='left' style='border-left: 1px solid #bbbbc3; border-bottom: 1px solid #000000;'>
                                <span style='font-size: 11px; text-transform: uppercase; font-family: Tahoma,Geneva,sans-serif; color: #000000;'>"
                                        + SupplierLocation + "</span></td>";
                            NewPartFormInfo += @"
                                <td align='left' style='border-left: 1px solid #bbbbc3; border-bottom: 1px solid #000000;'>
                                <span style='font-size: 11px; text-transform: uppercase; font-family: Tahoma,Geneva,sans-serif; color: #000000;'>"
                                        + apqpPSWData.QuoteNumber + "</span></td>";
                            NewPartFormInfo += @"
                                <td align='left' style='border-left: 1px solid #bbbbc3; border-bottom: 1px solid #000000;border-right: 1px solid #000000;'>
                                <span style='font-size: 11px; text-transform: uppercase; font-family: Tahoma,Geneva,sans-serif; color: #000000;'>"
                                        + (apqpPSWData.ToolingKickoffDate.HasValue ? apqpPSWData.ToolingKickoffDate.Value.ToString("dd-MMM-yy") : string.Empty) + "</span></td></tr>";
                        }
                    }
                    else
                        NewPartFormInfo += @"<tr><td colspan='12' bgcolor='#ffffff' style='border-right: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>No records found.</font></td></tr>";

                    endTableStr = "</tbody></table>";

                    emailData.EmailBody = emailData.EmailBody.Replace("%NewPartFormInfo%", startTableStr + NewPartFormInfo + endTableStr);

                    string footer = @"<br /><br /><hr />
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
                    emailData.EmailBody += footer;
                    emailData.EmailSubject = emailData.EmailSubject + " - " + PartNumbers;
                }
                else
                {
                    errMSg = Languages.GetResourceText("CreateEmailTemplate");
                }
                #endregion

                #region get associate user with trigger point
                if (string.IsNullOrEmpty(errMSg))
                {
                    var usersList = context.GetUsersEmailByTriggerPoint(TriggerPointName).ToList();
                    if (usersList == null)
                        errMSg = Languages.GetResourceText("DataNotExists");
                    else
                    {
                        if (usersList.Count > 0)
                        {
                            foreach (var item in usersList)
                            {
                                emailData.UserIds = emailData.UserIds + item.UserId + ",";
                            }
                            if (APQPEngineerIdAndSupplierChainCoordinatorIds.Count > 0)
                            {
                                emailData.UserIds = emailData.UserIds + string.Join(",", APQPEngineerIdAndSupplierChainCoordinatorIds);
                            }
                            //remove duplicate users
                            string[] allusers = emailData.UserIds.Split(',');
                            allusers = allusers.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
                            emailData.UserIds = string.Join(",", allusers);
                        }
                        else
                            errMSg = Languages.GetResourceText("AssociateuserwtihTriggerPoint");
                    }
                }
                #endregion
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.Common.EmailData>(errMSg, emailData);
            return response;
        }
        public NPE.Core.ITypedResponse<bool?> APQPSendEmail(MES.DTO.Library.Common.EmailData emailData)
        {
            bool IsSuccess = false;
            try
            {
                MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
                List<string> lstToAddress = new List<string>();
                List<string> lstCCEmails = new List<string>();
                List<string> lstBCCEmails = new List<string>();

                MES.Business.Library.BO.UserManagement.UserManagement objUserManagement = new MES.Business.Library.BO.UserManagement.UserManagement();
                foreach (var item in emailData.EmailIdsList)
                {
                    lstToAddress.Add(objUserManagement.GetUserInfoById(item).Email);
                }
                if (!string.IsNullOrEmpty(emailData.CCEmailIds))
                {
                    foreach (string ccEmail in emailData.CCEmailIds.Split(','))
                        lstCCEmails.Add(ccEmail);
                }

                lstBCCEmails.Add(GetCurrentUser.Email);
                List<System.Net.Mail.Attachment> Attachments = new List<System.Net.Mail.Attachment>();
                if (emailData.AttachDocument.HasValue && emailData.AttachDocument.Value)
                {
                    foreach (string APQPItemId in emailData.APQPItemIds.Split(','))
                    {
                        //TODO : 
                        try
                        {
                            string filePath = DownloadNPIF(Convert.ToInt32(APQPItemId), true);
                            if (!string.IsNullOrEmpty(filePath))
                                Attachments.Add(new System.Net.Mail.Attachment(filePath));
                        }
                        catch (Exception ex) { throw ex; }
                    }
                }
                if (!string.IsNullOrEmpty(emailData.EmailAttachment))
                {
                    Stream memoryStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(emailData.EmailAttachment);
                    Attachments.Add(new System.Net.Mail.Attachment(memoryStream, emailData.EmailFileName));
                }
                emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailData.EmailBody, out IsSuccess, Attachments, lstCCEmails, lstBCCEmails);
            }
            catch (Exception ex)
            {
            }
            if (IsSuccess)
                return SuccessBoolResponse(Languages.GetResourceText("SendEmailSuccess"));
            else
                return SuccessBoolResponse(Languages.GetResourceText("SendEmailFail"));
        }
        #endregion

        //TODO:

        #region NPIF

        public NPE.Core.ITypedResponse<bool?> GenerateNPIF(int apqpItemId)
        {

            string filePath = string.Empty;
            try
            {
                filePath = DownloadNPIF(apqpItemId, false);
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }

        private string DownloadNPIF(int apqpItemId, bool useAsAttachment)
        {
            string filePath = string.Empty;

            var httpcontext = HttpContext.Current;
            string directoryPath = httpcontext.Server.MapPath(Constants.APQPTemplateFolder)
                , templateFilePath = httpcontext.Server.MapPath(Constants.APQPTemplateFolder) + "NPIFGate1Form.xls";

            DTO.Library.APQP.APQP.PSWItem apqpPSWData = getGeneratePSWData(apqpItemId).Result;

            if (System.IO.Directory.Exists(directoryPath))
            {
                if (File.Exists(templateFilePath))
                {
                    try
                    {
                        System.IO.FileStream sourceXlsDataStream = new System.IO.FileStream(templateFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                        Winnovative.ExcelLib.ExcelWorkbook ew = new Winnovative.ExcelLib.ExcelWorkbook(sourceXlsDataStream);
                        ew.LicenseKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["wnvkey"]);
                        Winnovative.ExcelLib.ExcelWorksheet ws = ew.Worksheets[0];

                        bool hasPricingFieldsAccess = true;
                        LoginUser currentUser = GetCurrentUser;
                        if (currentUser != null)
                        {
                            //TODO: ROLE OBJECT PRIVILEGE ACCESS TO PRICING FIELDS
                            var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQP)).ToList();

                            currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQP)).ToList();
                            if (currentObjects.Count > 0)
                                hasPricingFieldsAccess = currentObjects[0].HasPricingFieldsAccess;
                        }

                        #region Part Information
                        ws[4, 2].Value = apqpPSWData.CustomerName; //Customer
                        ws[5, 2].Value = apqpPSWData.CustomerManufacturingLocation; //Customer Plant(s)
                        ws[6, 2].Value = apqpPSWData.ProjectName;//Program
                        ws[4, 4].Value = apqpPSWData.SupplierName;//Supplier
                        ws[5, 4].Value = apqpPSWData.ManufacturerName;//Manufacturer Name
                        ws[6, 4].Value = apqpPSWData.SupplierCode;//Supplier Code
                        #endregion Part Information

                        #region Part Information At RFQ / Sourcing
                        ws[8, 2].Value = apqpPSWData.PartNumber; //Part Number
                        ws[9, 2].Value = apqpPSWData.PartDesc;//Part Name
                        ws[10, 2].Value = apqpPSWData.RevLevel;//Revision Level
                        ws[8, 4].Value = apqpPSWData.RFQNumber;//RFQ Number
                        ws[9, 4].Value = apqpPSWData.QuoteNumber;//Quote Number
                        #endregion Part Information At RFQ / Sourcing

                        #region Commercial Information
                        ws[12, 3].Value = (apqpPSWData.CustomerToolingPOAuthRcvdDate.HasValue ? apqpPSWData.CustomerToolingPOAuthRcvdDate.Value.ToString("dd-MMM-yyyy") : string.Empty);//Cust. Tooling P.O. Rec'd Date
                        ws[13, 2].Value = apqpPSWData.ToolingLeadtimeDays;//Quoted Tooling Lead Time

                        //start here - show pricing fields based on Access rights
                        if (hasPricingFieldsAccess)
                        {
                            ws[14, 2].Value = apqpPSWData.PurchaseToolingCost;//Purchase Tooling Cost
                            ws[14, 4].Value = apqpPSWData.PurchasePieceCost;//Purchase Piece Cost
                            ws[15, 2].Value = apqpPSWData.SellingToolingPrice;//Tooling Selling Price
                            ws[15, 4].Value = apqpPSWData.SellingPiecePrice;//Selling Piece Price
                        }
                        //end here
                        if (apqpPSWData.MOQConfirmation.HasValue && apqpPSWData.MOQConfirmation.Value)
                            ws[16, 2].Value = "Yes";//Supplier Provided MOQ  
                        else
                            ws[16, 2].Value = "No";//Supplier Provided MOQ  

                        ws[13, 4].Value = apqpPSWData.CustomerToolingPONumber; //Customer Tooling PO Number
                        #endregion Commercial Information

                        #region Team/Contact Information
                        ws[18, 2].Value = apqpPSWData.CustomerProjectLead;//Customer Purchasing Contact
                        ws[19, 2].Value = apqpPSWData.CustomerProjectLeadEmail;//Customer Email
                        ws[20, 2].Value = apqpPSWData.CustomerEngineer;//Customer Engineering Contact
                        ws[21, 2].Value = apqpPSWData.CustomerEngineerEmail;//Customer Engineering Email
                        ws[22, 2].Value = apqpPSWData.APQPEngineer;//APQP Engineer
                        ws[23, 2].Value = apqpPSWData.MESAccountManager;//MES Acct Mgr
                        ws[24, 2].Value = apqpPSWData.SupplyChainCoordinator;//Supply Chain Coordinator

                        ws[18, 4].Value = apqpPSWData.CustomerProjectLeadPhone;//Customer Purchasing Phone Number
                        ws[20, 4].Value = apqpPSWData.CustomerEngineerPhone;//Customer Engineering Phone Number
                        #endregion Team/Contact Information

                        #region Engineering Information At RFQ / Sourcing
                        ws[26, 2].Value = apqpPSWData.Commodity;//Commodity
                        ws[27, 2].Value = apqpPSWData.Process;//Process
                        ws[28, 2].Value = apqpPSWData.Intial2DDrawingReceivedDate.HasValue ? "Yes" : "No";//Intial 2D Drawing Received Date
                        ws[29, 2].Value = apqpPSWData.Intial3DDataReceivedDate.HasValue ? "Yes" : "No";//Intial 3D Data Received Date
                        ws[30, 2].Value = (!string.IsNullOrEmpty(apqpPSWData.EAUUsage) ? Convert.ToDecimal(apqpPSWData.EAUUsage).ToString("#,##0") : string.Empty);//EAU Usage

                        if (apqpPSWData.DestinationId.HasValue)
                            ws[31, 2].Value = apqpPSWData.Destination + "\r\n" + apqpPSWData.Location;//MES Warehouse Location

                        //Right now it is Additional Part Desc
                        ws[33, 2].Value = apqpPSWData.AdditionalPartDescription;//Other/Special Requirements                   
                        ws[26, 4].Value = apqpPSWData.MaterialType;//Material Type
                        ws[27, 4].Value = (!string.IsNullOrEmpty(apqpPSWData.PartWeight) ? apqpPSWData.PartWeight + " kg" : string.Empty);//Material Weight

                        //PPAP Submission Level
                        int associatedTo_id = Convert.ToInt32(MES.Business.Library.Enums.StatusAssociatedTo.PPAP);
                        this.RunOnDB(context =>
                        {
                            foreach (var item in context.apqpSearchStatus(associatedTo_id))
                            {
                                if (item.Id.ToString() == apqpPSWData.PPAPSubmissionLevel)
                                    ws[30, 4].Value = item.Status;
                            }
                        }, true);


                        ws[31, 4].Value = apqpPSWData.ShipToLocation;//Customer Samples Ship-To Address:
                        ws[33, 4].Value = apqpPSWData.ProjectNotes;//Comments

                        #endregion Engineering Information At RFQ / Sourcing
                        string ti = "Generated on " + DateTime.Now.ToString("dd-MMM-yyyy") + ", at " + DateTime.Now.ToShortTimeString()
                                                                          + " " + TimeZone.CurrentTimeZone.StandardName + " by "
                                                                          + CurrentUserInfo.GetName();
                        ws[44, 2].Value = ti;

                        /* 20150830 bshah:
                         * Why is Excel File Locked?  Does it need to be locked?  Right now, it is only an export.  No need for it to be locked. */
                        //ws.WorksheetSecurity.ProtectWorksheet(System.Configuration.ConfigurationManager.AppSettings["ShipmentTemplatePswd"]);

                        ws.AutofitColumn(5); ws.AutofitColumn(13); ws.AutofitColumn(16);

                        if (!System.IO.Directory.Exists(httpcontext.Server.MapPath(@"~\") + Constants.NPIFDocumentFolder))
                            System.IO.Directory.CreateDirectory(httpcontext.Server.MapPath(@"~\") + Constants.NPIFDocumentFolder);

                        string generatedFileName = "APQP-NPIF-" + apqpPSWData.PartNumber + "-" + DateTime.Now.ToString("yyyyMMdd") + ".xls";//APQP-NPIF-PN-DateGenerated: APQP-NPIF-PT24304-20150519

                        filePath = httpcontext.Server.MapPath(@"~\") + Constants.NPIFDocumentFolder + @"\" + generatedFileName;
                        try
                        {
                            ew.Save(filePath);
                            if (!useAsAttachment)
                                filePath = System.Configuration.ConfigurationManager.AppSettings["ApiURL"] + Constants.NPIFDocumentFolder + generatedFileName;

                        }
                        catch (Exception ex) //Error
                        {
                            filePath = string.Empty;
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
                {
                    // FailedBoolResponse("APQP Item Template does not exists.");
                }
            }
            return filePath;
        }

        #endregion NPIF

        #region PPAP - SAP Export to Excel
        public NPE.Core.ITypedResponse<bool?> PPAPSubmissionSAPDataExport(string APQPItemIds)
        {
            string filePath = string.Empty;
            try
            {
                filePath = DownloadPPAPItemTemplate(APQPItemIds);
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private string DownloadPPAPItemTemplate(string APQPItemIds)
        {
            string filePath = string.Empty;
            int rowIndex = 2;
            if (!string.IsNullOrEmpty(APQPItemIds))
            {
                var httpcontext = HttpContext.Current;
                string directoryPath = httpcontext.Server.MapPath(Constants.APQPTemplateFolder)
                    , templateFilePath = httpcontext.Server.MapPath(Constants.APQPTemplateFolder) + "APQPPPAPItemTemplate.xls";

                if (System.IO.Directory.Exists(directoryPath))
                {
                    if (File.Exists(templateFilePath))
                    {
                        try
                        {
                            System.IO.FileStream sourceXlsDataStream = new System.IO.FileStream(templateFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                            Winnovative.ExcelLib.ExcelWorkbook ew = new Winnovative.ExcelLib.ExcelWorkbook(sourceXlsDataStream);
                            ew.LicenseKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["wnvkey"]);
                            Winnovative.ExcelLib.ExcelWorksheet ws = ew.Worksheets[0];
                            #region APQP Elements

                            List<DTO.Library.APQP.APQP.PSWItem> apqpPSWData = getGeneratePSWDataByItems(APQPItemIds).Result;
                            foreach (var item in apqpPSWData)
                            {
                                ws[rowIndex, 1].Value = item.PartNumber;//Part Number
                                ws[rowIndex, 2].Value = item.PartDesc;//Part Description
                                ws[rowIndex, 3].Value = item.RevLevel;//Rev Level
                                ws[rowIndex, 4].Value = item.DrawingNumber;//Drawing Number
                                ws[rowIndex, 5].Value = item.PartWeight;//Part Weight (KG)
                                //start here - show pricing fields based on Access rights
                                bool hasPricingFieldsAccess = true;
                                LoginUser currentUser = GetCurrentUser;
                                if (currentUser != null)
                                {
                                    //TODO: ROLE OBJECT PRIVILEGE ACCESS TO PRICING FIELDS
                                    var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQP)).ToList();

                                    currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQP)).ToList();
                                    if (currentObjects.Count > 0)
                                        hasPricingFieldsAccess = currentObjects[0].HasPricingFieldsAccess;

                                }
                                if (hasPricingFieldsAccess)
                                {
                                    ws[rowIndex, 6].Value = item.PurchasePieceCost;//Purchase Cost
                                    ws[rowIndex, 7].Value = item.SellingPiecePrice;//Selling Price
                                }
                                ws[rowIndex, 8].Value = item.PartClassification;//Part Classification
                                ws[rowIndex, 9].Value = item.MaterialType;//Material Type
                                ws[rowIndex, 10].Value = item.APQPEngineer;//MES Inc. Project Lead
                                ws[rowIndex, 11].Value = item.SupplyChainCoordinator;//Supply Chain Coordinator
                                rowIndex++;

                            }
                            #endregion

                            ws.WorksheetSecurity.ProtectWorksheet(System.Configuration.ConfigurationManager.AppSettings["ShipmentTemplatePswd"]);

                            string generatedFileName = "APQP-PPAP-" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                            filePath = httpcontext.Server.MapPath(@"~\") + Constants.QUOTEFILEPATH + @"\" + generatedFileName;
                            try
                            {
                                ew.Save(filePath);
                                filePath = System.Configuration.ConfigurationManager.AppSettings["ApiURL"] + Constants.QUOTEFILEPATH + generatedFileName;
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
                    {
                        //FailedBoolResponse("PPAP Submission Item Template does not exists.");
                    }
                }
            }

            return filePath;
        }
        #endregion PPAP - SAP Export to Excel

        #region PSW
        public NPE.Core.ITypedResponse<bool?> ExportGeneratePSW(DTO.Library.APQP.APQP.PSWItem pswItem)
        {
            string filePath = string.Empty;
            try
            {
                GeneratePSW(pswItem, ref filePath);
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private bool GeneratePSW(DTO.Library.APQP.APQP.PSWItem pswItem, ref string filePath)
        {
            bool isSuccess = false;
            var httpcontext = HttpContext.Current;

            try
            {
                ExcelFile ef = new ExcelFile();
                ExcelFile myExcelFile = new ExcelFile();
                ExcelWorksheet excWsheet = myExcelFile.Worksheets.Add("External Quote");

                try
                {
                    CellRange cr = excWsheet.Cells.GetSubrange("A1", "Z100");


                    if (!System.IO.Directory.Exists(httpcontext.Server.MapPath("~" + Constants.PSWDocumentPhyFolder)))
                        System.IO.Directory.CreateDirectory(httpcontext.Server.MapPath("~" + Constants.PSWDocumentPhyFolder));

                    string generatedFileName = "APQP-PSW-" + pswItem.PartNumber + "-" + DateTime.Now.ToString("yyyyMMdd") + ".pdf"; //APQP-PSW-PN-DateGenerated: APQP-PSW-PN-20150612.pdf
                    string pdfFilePath = httpcontext.Server.MapPath("~" + Constants.PSWDocumentPhyFolder) + generatedFileName;

                    if (File.Exists(pdfFilePath))
                        File.Delete(pdfFilePath);

                    PdfConverter pdfConverter = new PdfConverter();
                    pdfConverter.LicenseKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["evopdfkey"]);
                    pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.Letter;
                    pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
                    pdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;

                    string contents = File.ReadAllText(httpcontext.Server.MapPath("~/EmailTemplates/apqpPSWPdfTemplate.htm"));

                    // Replace the placeholders with the user-specified text

                    contents = contents.Replace("<%PartName%>", pswItem.PartDesc);
                    contents = contents.Replace("<%CustPartName%>", pswItem.PartNumber);
                    contents = contents.Replace("<%ShownOnDrawingNo%>", pswItem.DrawingNumber);
                    contents = contents.Replace("<%OrgPartNumber%>", pswItem.OrgPartNumber);
                    contents = contents.Replace("<%EngineeringChangeLevel%>", pswItem.RevLevel);
                    contents = contents.Replace("<%EngChangeLevelDate%>", pswItem.ToolingKickoffDate.HasValue ? Convert.ToDateTime(pswItem.ToolingKickoffDate.Value.ToString()).ToString("dd-MMM-yy") : string.Empty);
                    contents = contents.Replace("<%AdditionalEngineeringChanges%>", pswItem.AdditionalEngineeringChanges);
                    contents = contents.Replace("<%AdditionalEngineeringChangesDate%>", pswItem.AdditionalEngineeringChangesDate.HasValue ? Convert.ToDateTime(pswItem.AdditionalEngineeringChangesDate.ToString().Trim()).ToString("dd-MMM-yy") : string.Empty);

                    if (pswItem.SafetyGovernmentRegulation == "1")
                    {
                        contents = contents.Replace("<%YesSafetyGovernmentRegulation%>", "checked='checked'");
                        contents = contents.Replace("<%NoSafetyGovernmentRegulation%>", "");
                    }
                    else
                    {
                        contents = contents.Replace("<%YesSafetyGovernmentRegulation%>", "");
                        contents = contents.Replace("<%NoSafetyGovernmentRegulation%>", "checked='checked'");
                    }

                    contents = contents.Replace("<%PurchaseOrderNumber%>", pswItem.PurchaseOrderNo);
                    contents = contents.Replace("<%Weight%>", pswItem.PartWeight);
                    contents = contents.Replace("<%CheckingAidNo%>", pswItem.CheckingAidEngineeringChange);
                    contents = contents.Replace("<%CheckingAidEngineeringChangeLevel%>", pswItem.CheckingAidEngineeringChange);
                    contents = contents.Replace("<%CheckingAidEngChangeLevelDate%>", pswItem.CheckingAidEngChangeLevelDate.HasValue ? Convert.ToDateTime(pswItem.CheckingAidEngChangeLevelDate.Value.ToString().Trim()).ToString("dd-MMM-yy") : string.Empty);
                    contents = contents.Replace("<%SupplierCode%>", (!string.IsNullOrEmpty(pswItem.SupplierCode) ? pswItem.SupplierName.Trim() + " " + pswItem.SupplierCode.Trim()
                        : pswItem.SupplierName));
                    contents = contents.Replace("<%CustomerName%>", pswItem.CustomerName);
                    contents = contents.Replace("<%SupplierStreetAddress%>", pswItem.SupplierAddress1);
                    contents = contents.Replace("<%Buyer%>", pswItem.BuyerCode);

                    string SupplierCityStateZip = "";
                    if (!string.IsNullOrEmpty(pswItem.SupplierCity))
                        SupplierCityStateZip = pswItem.SupplierCity;

                    if (!string.IsNullOrEmpty(pswItem.SupplierState))
                        SupplierCityStateZip = string.IsNullOrEmpty(SupplierCityStateZip) ? pswItem.SupplierState.Trim() : SupplierCityStateZip + " " + pswItem.SupplierState.Trim();

                    if (!string.IsNullOrEmpty(pswItem.SupplierZip))
                        SupplierCityStateZip = string.IsNullOrEmpty(SupplierCityStateZip) ? pswItem.SupplierZip.Trim() : SupplierCityStateZip + " " + pswItem.SupplierZip.Trim();

                    contents = contents.Replace("<%SupplierCityStateZip%>", SupplierCityStateZip);
                    contents = contents.Replace("<%Application%>", pswItem.Application);

                    if (pswItem.MaterialsReporting == "1")
                    {
                        contents = contents.Replace("<%YesMaterialsReporting%>", "checked='checked'");
                        contents = contents.Replace("<%NoMaterialsReporting%>", "");
                        contents = contents.Replace("<%NAMaterialsReporting%>", "");
                    }
                    else if (pswItem.MaterialsReporting == "0")
                    {
                        contents = contents.Replace("<%YesMaterialsReporting%>", "");
                        contents = contents.Replace("<%NoMaterialsReporting%>", "checked='checked'");
                        contents = contents.Replace("<%NAMaterialsReporting%>", "");
                    }
                    else
                    {
                        contents = contents.Replace("<%YesMaterialsReporting%>", "");
                        contents = contents.Replace("<%NoMaterialsReporting%>", "");
                        contents = contents.Replace("<%NAMaterialsReporting%>", "checked='checked'");
                    }

                    contents = contents.Replace("<%MaterialsReportingIMDS%>", pswItem.SubmittedbyIMDS);

                    if (pswItem.MaterialsReporting == "1")
                    {
                        contents = contents.Replace("<%YesMaterialsReportingISO%>", "checked='checked'");
                        contents = contents.Replace("<%NoMaterialsReportingISO%>", "");
                        contents = contents.Replace("<%NAMaterialsReportingISO%>", "");
                    }
                    else if (pswItem.MaterialsReporting == "0")
                    {
                        contents = contents.Replace("<%YesMaterialsReportingISO%>", "");
                        contents = contents.Replace("<%NoMaterialsReportingISO%>", "checked='checked'");
                        contents = contents.Replace("<%NAMaterialsReportingISO%>", "");
                    }
                    else
                    {
                        contents = contents.Replace("<%YesMaterialsReportingISO%>", "");
                        contents = contents.Replace("<%NoMaterialsReportingISO%>", "");
                        contents = contents.Replace("<%NAMaterialsReportingISO%>", "checked='checked'");
                    }

                    contents = contents.Replace("<%ReasonForSubmissionInitialSubmission%>", (pswItem.InitialSubmission ? "checked='checked'" : ""));
                    contents = contents.Replace("<%ReasonForSubmissionEngineeringChange%>", (pswItem.EngineeringChanges ? "checked='checked'" : ""));
                    contents = contents.Replace("<%ReasonForSubmissionToolingTransferReplacementRefurbishmentOrAdditional%>", (pswItem.ToolingTransferReplacementRefurbishmentoradditional ? "checked='checked'" : ""));
                    contents = contents.Replace("<%ReasonForSubmissionCorrectionofDiscrepancy%>", (pswItem.CorrectionofDiscrepancy ? "checked='checked'" : ""));
                    contents = contents.Replace("<%ReasonForSubmissionToolingInactive%>", (pswItem.ToolingInactiveYear ? "checked='checked'" : ""));
                    contents = contents.Replace("<%ReasonForSubmissionChangeToOptionalConstructionOrMaterial%>", (pswItem.ChangetoOptionalConstructionorMaterial ? "checked='checked'" : ""));
                    contents = contents.Replace("<%ReasonForSubmissionSubSupplierOrMaterialSourceChange%>", (pswItem.SubSupplierorMaterialSourceChange ? "checked='checked'" : ""));
                    contents = contents.Replace("<%ReasonForSubmissionChangeInPartProcessing%>", (pswItem.ChangeinPartProcessing ? "checked='checked'" : ""));
                    contents = contents.Replace("<%ReasonForSubmissionPartProducedAtAdditionalLocation%>", (pswItem.PartProductedatAdditionalLocation ? "checked='checked'" : ""));
                    contents = contents.Replace("<%ReasonForSubmissionOtherPleaseSpecifyBelow%>", (pswItem.OtherPleasespecifybelow ? "checked='checked'" : ""));
                    contents = contents.Replace("<%ReasonForSubmissionOtherSpecifyComment%>", pswItem.ReasonForSubmissionOther);

                    contents = contents.Replace("<%ReasonSubmissionLevelCheckLevel1%>", (pswItem.RequestedForSubmission == "1" ? "checked='checked'" : "false"));
                    contents = contents.Replace("<%ReasonSubmissionLevelCheckLevel2%>", (pswItem.RequestedForSubmission == "2" ? "checked='checked'" : "false"));
                    contents = contents.Replace("<%ReasonSubmissionLevelCheckLevel3%>", (pswItem.RequestedForSubmission == "3" ? "checked='checked'" : "false"));
                    contents = contents.Replace("<%ReasonSubmissionLevelCheckLevel4%>", (pswItem.RequestedForSubmission == "4" ? "checked='checked'" : "false"));
                    contents = contents.Replace("<%ReasonSubmissionLevelCheckLevel5%>", (pswItem.RequestedForSubmission == "5" ? "checked='checked'" : "false"));

                    contents = contents.Replace("<%SubmissionResultDimensionalMeasurements%>", (pswItem.DimensionalMeasurements ? "checked='checked'" : ""));
                    contents = contents.Replace("<%SubmissionResultMaterialAndFunctionalTests%>", (pswItem.MaterialandFunctionalTests ? "checked='checked'" : ""));
                    contents = contents.Replace("<%SubmissionResultAppearanceCriteria%>", (pswItem.AppearanceCriteria ? "checked='checked'" : ""));
                    contents = contents.Replace("<%SubmissionResultStatisticalProcessPackage%>", (pswItem.StatisticalProcessPackage ? "checked='checked'" : ""));

                    if (pswItem.SubmissionResultRequirement == "1")
                    {
                        contents = contents.Replace("<%YesDrawingAndSpecificationRequirement%>", "checked='checked'");
                        contents = contents.Replace("<%NoDrawingAndSpecificationRequirement%>", "");
                    }
                    else
                    {
                        contents = contents.Replace("<%YesDrawingAndSpecificationRequirement%>", "");
                        contents = contents.Replace("<%NoDrawingAndSpecificationRequirement%>", "checked='checked'");
                    }

                    contents = contents.Replace("<%DeclarationProductionRate1%>", pswItem.Rate1);
                    contents = contents.Replace("<%DeclarationProductionRate2%>", pswItem.Rate2);
                    contents = contents.Replace("<%Explanation%>", pswItem.ExplainationComments);

                    if (pswItem.CustomerToolTagged == "1")
                    {
                        contents = contents.Replace("<%YesCustomerToolTagged%>", "checked='checked'");
                        contents = contents.Replace("<%NoCustomerToolTagged%>", "");
                        contents = contents.Replace("<%NACustomerToolTagged%>", "");
                    }
                    else if (pswItem.CustomerToolTagged == "0")
                    {
                        contents = contents.Replace("<%YesCustomerToolTagged%>", "");
                        contents = contents.Replace("<%NoCustomerToolTagged%>", "checked='checked'");
                        contents = contents.Replace("<%NACustomerToolTagged%>", "");
                    }
                    else
                    {
                        contents = contents.Replace("<%YesCustomerToolTagged%>", "");
                        contents = contents.Replace("<%NoCustomerToolTagged%>", "");
                        contents = contents.Replace("<%NACustomerToolTagged%>", "checked='checked'");
                    }

                    contents = contents.Replace("<%PrintName%>", pswItem.PrintName);
                    contents = contents.Replace("<%PhoneNo%>", pswItem.PhoneNo);
                    contents = contents.Replace("<%FaxNo%>", pswItem.FaxNo);
                    contents = contents.Replace("<%Title%>", pswItem.Title);
                    contents = contents.Replace("<%email%>", pswItem.Email);

                    // get the pdf bytes from html string
                    byte[] pdfBytes = pdfConverter.GetPdfBytesFromHtmlString(contents);

                    pdfConverter.PdfDocumentOptions.ShowFooter = true;
                    pdfConverter.PdfDocumentOptions.ShowHeader = true;
                    pdfConverter.PdfHeaderOptions.HeaderHeight = 65;

                    string headerAndFooterHtmlUrl = httpcontext.Server.MapPath("~/EmailTemplates/Header.htm");
                    pdfConverter.PdfHeaderOptions.HtmlToPdfArea = new HtmlToPdfArea(0, 0, 0, pdfConverter.PdfHeaderOptions.HeaderHeight, headerAndFooterHtmlUrl, 900, 100);
                    pdfConverter.PdfHeaderOptions.DrawHeaderLine = false;

                    pdfConverter.PdfFooterOptions.FooterHeight = 30;

                    headerAndFooterHtmlUrl = httpcontext.Server.MapPath("~/EmailTemplates/APQPFooter.htm");
                    pdfConverter.PdfFooterOptions.HtmlToPdfArea = new HtmlToPdfArea(50, 0, 0, pdfConverter.PdfHeaderOptions.HeaderHeight, headerAndFooterHtmlUrl, 200, 30);
                    pdfConverter.PdfFooterOptions.DrawFooterLine = true;

                    pdfConverter.SavePdfFromHtmlStringToFile(contents, pdfFilePath);

                    HttpContext context = HttpContext.Current;
                    string rfqTempFilePath = string.Empty;
                    ZipFile zip = new ZipFile();

                    List<string> filePathList = new List<string>();
                    string apqpDocumentIds = pswItem.DocumentIds;
                    bool allowConfidentialDocumentType = true;
                    LoginUser currentUser = GetCurrentUser;
                    var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQPDashboard)).ToList();
                    if (currentObjects.Count > 0)
                        allowConfidentialDocumentType = currentObjects[0].AllowConfidentialDocumentType;


                    if (File.Exists(pdfFilePath))
                    {
                        if (!string.IsNullOrEmpty(apqpDocumentIds))
                        {
                            foreach (string item in apqpDocumentIds.Split(','))
                            {
                                Document documentObj = new Document();
                                DTO.Library.APQP.APQP.Document docItem = documentObj.FindById(Convert.ToInt32(item)).Result;

                                if (docItem.IsConfidential && !allowConfidentialDocumentType)
                                { continue; }

                                if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), docItem.FilePath.Trim()))
                                {
                                    string newFileName = pswItem.PartNumber + (!string.IsNullOrEmpty(docItem.RevLevel) ? "-" + docItem.RevLevel.Trim() : "")
                                        + (!string.IsNullOrEmpty(docItem.DocumentType) ? "-" + docItem.DocumentType.Trim().Replace(":", string.Empty)
                                                                                    .Replace("\\", string.Empty)
                                                                                    .Replace("/", string.Empty)
                                                                                    .Replace("|", string.Empty)
                                                                                    .Replace("*", string.Empty)
                                                                                    .Replace("<", string.Empty)
                                                                                    .Replace("?", string.Empty)
                                                                                    .Replace("\"", string.Empty)
                                                                                    .Replace(">", string.Empty) : "") + "-" + Convert.ToDateTime(docItem.CreatedDate).ToString("yyyyMMddhhmmss")
                                        + System.IO.Path.GetExtension(docItem.FileTitle.Trim());

                                    byte[] fileBytes = Helper.BlobHelper.GetBlobStreamByUrl(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), docItem.FilePath);
                                    using (var fileStream = new MemoryStream(fileBytes))
                                    {
                                        fileStream.Seek(0, SeekOrigin.Begin);
                                        zip.AddEntry(Path.GetFileName(docItem.FilePath.Trim()), fileBytes).FileName = newFileName;
                                    }
                                }
                            }
                        }

                        zip.AddFile(pdfFilePath, "");

                        string zipFileNavigationURL = "APQP-PSW-" + pswItem.PartNumber + "-" + DateTime.Now.ToString("yyyyMMdd") + ".zip"; //APQP-PSW-PN-DateGenerated: APQP-PSW-PN-20150612.zip

                        string zipFileName = httpcontext.Server.MapPath("~") + Constants.PSWDocumentPhyFolder + zipFileNavigationURL;  //APQP-PSW-PN-DateGenerated: APQP-PSW-PN-20150612.zip

                        if (File.Exists(zipFileName))
                            File.Delete(zipFileName);

                        if (zip.Count > 0)
                            zip.Save(zipFileName);

                        string zipFilePath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                            , Constants.PSWDocumentFolder
                            , zipFileNavigationURL
                            , zipFileName);

                        if (File.Exists(pdfFilePath))
                            File.Delete(pdfFilePath);

                        //TODO:  Update APQP Document Add On PSW
                        if (!string.IsNullOrEmpty(apqpDocumentIds))
                            UpdateAddPSWDocument(pswItem.ItemMasterId, apqpDocumentIds);

                        PPAPSubmission apqpObj = new PPAPSubmission();
                        apqpObj.UpdatePSWDetails("/" + Constants.PSWDocumentFolder + zipFileNavigationURL, pswItem.ItemMasterId);

                        filePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                                    + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                    + "/" + Constants.PSWDocumentFolder + zipFileNavigationURL;
                        isSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    throw (ex);
                }
                finally
                {
                    excWsheet = null;
                    ef.ClosePreservedXlsx();
                }
            }
            catch (Exception ex)//Error
            {
                isSuccess = false;
                //"Error - LoadCSV",
                throw (ex);
            }
            return isSuccess;

        }
        private bool UpdateAddPSWDocument(int apqpItemId, string apqpDocumentIds)
        {
            bool ret = false;
            string successMsg = null;
            ObjectParameter outputParam = new ObjectParameter("ErrorKey", "");
            this.RunOnDB(context =>
            {
                int result = context.UpdateAddPSWDocument(apqpItemId, apqpDocumentIds, outputParam);

                if (string.IsNullOrEmpty(Convert.ToString(outputParam.Value)))
                {
                    successMsg = Languages.GetResourceText("PSWAddSuccess");
                }
            });
            return ret;

        }
        #endregion PSW

        #region Start NPIF Approvals

        public NPE.Core.ITypedResponse<DTO.Library.APQP.APQP.apqpNPIFDocuSign> GetPredefinedNPIFRecipients(string apqpItemId)
        {
            DTO.Library.APQP.APQP.apqpNPIFDocuSign npifDocuSign = new DTO.Library.APQP.APQP.apqpNPIFDocuSign();
            npifDocuSign.APQPItemId = Convert.ToInt32(apqpItemId);

            List<DTO.Library.APQP.APQP.apqpNPIFDocuSignApprovers> lstNPIFApprovers = new List<DTO.Library.APQP.APQP.apqpNPIFDocuSignApprovers>();
            DTO.Library.APQP.APQP.apqpNPIFDocuSignApprovers npifApprovers = null;
            string errMSg = string.Empty;
            int i;
            for (i = 0; i < 6; i++)
            {
                npifApprovers = new DTO.Library.APQP.APQP.apqpNPIFDocuSignApprovers();

                switch (i)
                {
                    case 0:
                        npifApprovers.DesignationId = (short)DesignationFixedId.AccountManager;
                        npifApprovers.RoutingOrder = 1;
                        break;
                    case 1:
                        npifApprovers.DesignationId = (short)DesignationFixedId.SalesManager;
                        npifApprovers.RoutingOrder = 1;
                        break;
                    case 2:
                        npifApprovers.DesignationId = (short)DesignationFixedId.SourcingManager;
                        npifApprovers.RoutingOrder = 1;
                        break;
                    case 3:
                        npifApprovers.DesignationId = (short)DesignationFixedId.SupplyChainManager;
                        npifApprovers.RoutingOrder = 1;
                        break;
                    case 4:
                        npifApprovers.DesignationId = (short)DesignationFixedId.QualityManager;
                        npifApprovers.RoutingOrder = 2;
                        break;
                    case 5:
                        npifApprovers.DesignationId = (short)DesignationFixedId.APQPQualityEngineer;
                        npifApprovers.RoutingOrder = 1;
                        break;

                }

                lstNPIFApprovers.Add(npifApprovers);
            }
            npifDocuSign.lstNPIFDocuSignApprovers = lstNPIFApprovers;
            var response = SuccessOrFailedResponse<MES.DTO.Library.APQP.APQP.apqpNPIFDocuSign>(errMSg, npifDocuSign);
            return response;
        }

        public NPE.Core.ITypedResponse<int?> SendNPIF(DTO.Library.APQP.APQP.apqpNPIFDocuSign npifDocuSign)
        {
            string errMSg = null;
            string successMsg = null;
            string filePath = string.Empty;
            int? ReturnVal = 0;
            DocuSign.DocuSign prg = new DocuSign.DocuSign();
            this.RunOnDB(context =>
           {
               var NPIFDocuSignList = context.NPIFDocusigns.Where(item => item.APQPItemId == npifDocuSign.APQPItemId).OrderByDescending(item => item.CreatedDate).FirstOrDefault();
               if (NPIFDocuSignList == null)
                   npifDocuSign.DocumentId = "1";
               else
               {
                   npifDocuSign.DocumentId = (Convert.ToInt32(NPIFDocuSignList.DocumentId) + 1).ToString();
               }
           });
            try
            {
                filePath = DownloadNPIF(npifDocuSign.APQPItemId, false);
                if (!string.IsNullOrEmpty(filePath))
                {
                    npifDocuSign = prg.SendNPIF(npifDocuSign, Path.GetFileName(filePath)).Result;
                    if (npifDocuSign != null && !string.IsNullOrEmpty(npifDocuSign.EnvelopeId))
                    {
                        SaveNPIFRecipients(npifDocuSign);
                        if (npifDocuSign.Status != DocuSignEnvelopeStatus.completed.ToString())
                            successMsg = "false";
                    }
                }
            }
            catch (Exception ex)
            {
                return SuccessOrFailedResponse<int?>(Convert.ToString(ex.Message), ReturnVal, successMsg);
            }
            return SuccessOrFailedResponse<int?>(errMSg, ReturnVal, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.APQP.APQP.apqpNPIFDocuSign> SaveNPIFRecipients(DTO.Library.APQP.APQP.apqpNPIFDocuSign npifDocuSign)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToAdd = new MES.Data.Library.NPIFDocusign();
            try
            {
                recordToAdd.CreatedBy = CurrentUser;
                recordToAdd.DateOfStatus = recordToAdd.CreatedDate = AuditUtils.GetCurrentDateTime();
                recordToAdd.APQPItemId = npifDocuSign.APQPItemId;
                recordToAdd.DocumentId = npifDocuSign.DocumentId;
                recordToAdd.EnvelopeId = npifDocuSign.EnvelopeId;
                recordToAdd.Status = npifDocuSign.Status;
                recordToAdd.InitialDocumentPath = npifDocuSign.InitialDocumentPath == null ? string.Empty : npifDocuSign.InitialDocumentPath;
                recordToAdd.SignedDocumentPath = string.Empty;

                this.DataContext.NPIFDocusigns.Add(recordToAdd);

                this.DataContext.SaveChanges();
                npifDocuSign.Id = recordToAdd.Id;

                #region "Save NPIF DocuSign Approvers Detail"

                MES.Data.Library.NPIFDocuSignApprover dboApprovers = null;
                if (npifDocuSign.lstNPIFDocuSignApprovers != null && npifDocuSign.lstNPIFDocuSignApprovers.Count > 0)
                {

                    foreach (var npifApprover in npifDocuSign.lstNPIFDocuSignApprovers)
                    {
                        if (npifDocuSign.Id != 0)
                        {
                            dboApprovers = new MES.Data.Library.NPIFDocuSignApprover();
                            dboApprovers.DesignationId = npifApprover.DesignationId;
                            dboApprovers.NPIFDocusignId = npifDocuSign.Id;
                            dboApprovers.UserId = npifApprover.UserId;
                            dboApprovers.RecipientId = npifApprover.RecipientId;
                            this.DataContext.NPIFDocuSignApprovers.Add(dboApprovers);
                        }
                    }

                    this.DataContext.SaveChanges();
                }

                #endregion


                if (string.IsNullOrEmpty(errMSg))
                {
                    successMsg = Languages.GetResourceText("NPIFSavedSuccess");
                }
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
            return SuccessOrFailedResponse<DTO.Library.APQP.APQP.apqpNPIFDocuSign>(errMSg, npifDocuSign, successMsg);
        }

        public NPE.Core.ITypedResponse<bool?> CheckNPIFDocuSignStatus()
        {
            bool IsSuccess = false;
            DocuSign.DocuSign prg = new DocuSign.DocuSign();
            var recordToBeUpdated = new List<MES.Data.Library.NPIFDocusign>();
            try
            {
                this.RunOnDB(context =>
                {
                    recordToBeUpdated = context.NPIFDocusigns.Where(item => item.Status.ToLower() == DocuSignEnvelopeStatus.sent.ToString()).ToList();
                    if (recordToBeUpdated != null)
                    {
                        foreach (MES.Data.Library.NPIFDocusign item in recordToBeUpdated)
                        {
                            if (!string.IsNullOrEmpty(item.EnvelopeId))
                            {
                                Envelope envDetails = prg.CheckStatusOfDocuSign(item);

                                item.Status = envDetails.Status;
                                item.DateOfStatus = (envDetails.SentDateTime != null) ? Convert.ToDateTime(envDetails.SentDateTime) : (DateTime?)null;
                                item.SignedDocumentPath = string.Empty;

                                string partNumber = string.Empty;
                                KickOff apqpObj = new KickOff();
                                var kickOffItem = apqpObj.FindById(item.APQPItemId.Value).Result;
                                if (kickOffItem != null && !string.IsNullOrEmpty(kickOffItem.PartNumber))
                                    partNumber = kickOffItem.PartNumber;

                                if (envDetails.Status.ToLower() == DocuSignEnvelopeStatus.completed.ToString())
                                {
                                    item.SignedDocumentPath = prg.GetDocuSignDocument(item.EnvelopeId, item.DocumentId, partNumber, Constants.NPIFDocusign);
                                    item.DateOfStatus = envDetails.CompletedDateTime != null ? Convert.ToDateTime(envDetails.CompletedDateTime) : (DateTime?)null;
                                }
                                else if (envDetails.Status.ToLower() == DocuSignEnvelopeStatus.voided.ToString())
                                {
                                    item.VoidedReason = envDetails.VoidedReason;
                                    item.DateOfStatus = envDetails.VoidedDateTime != null ? Convert.ToDateTime(envDetails.VoidedDateTime) : (DateTime?)null;
                                }
                                else if (envDetails.Status.ToLower() == DocuSignEnvelopeStatus.declined.ToString())
                                {
                                    item.DateOfStatus = envDetails.DeclinedDateTime != null ? Convert.ToDateTime(envDetails.DeclinedDateTime) : (DateTime?)null;
                                }

                                if (!string.IsNullOrEmpty(envDetails.DeletedDateTime))
                                {
                                    item.DateOfStatus = envDetails.DeletedDateTime != null ? Convert.ToDateTime(envDetails.DeletedDateTime) : (DateTime?)null;
                                    envDetails.Status = DocuSignEnvelopeStatus.deleted.ToString();
                                }

                                this.DataContext.Entry(item).State = EntityState.Modified;
                                this.DataContext.SaveChanges();
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return FailedBoolResponse(ex.ToString());
            }
            if (IsSuccess)
                return SuccessBoolResponse(Languages.GetResourceText("UpdateSuccess"));
            else
                return FailedBoolResponse(Languages.GetResourceText("UpdateFail"));
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.apqpNPIFDocuSign>> GetNPIFDocuSignList(string apqpItemId)
        {
            string errMSg = null;
            List<DTO.Library.APQP.APQP.apqpNPIFDocuSign> lstNPIFDocuSign = new List<DTO.Library.APQP.APQP.apqpNPIFDocuSign>();
            DTO.Library.APQP.APQP.apqpNPIFDocuSign npifDocuSign = null;
            int apqpId = Convert.ToInt32(apqpItemId);
            this.RunOnDB(context =>
            {
                var NPIFDocuSignList = context.NPIFDocusigns.Where(item => item.APQPItemId == apqpId).OrderByDescending(item => item.CreatedDate).ToList();
                if (NPIFDocuSignList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var item in NPIFDocuSignList)
                    {
                        npifDocuSign = new DTO.Library.APQP.APQP.apqpNPIFDocuSign();
                        npifDocuSign.APQPItemId = Convert.ToInt32(apqpItemId);
                        npifDocuSign.Id = item.Id;
                        npifDocuSign.DateOfStatus = item.DateOfStatus.Value;
                        npifDocuSign.InitialDocumentPath = item.InitialDocumentPath == null ? string.Empty : item.InitialDocumentPath;
                        npifDocuSign.SignedDocumentPath = item.SignedDocumentPath == null ? string.Empty : item.SignedDocumentPath;

                        npifDocuSign.Status = item.Status;

                        if (npifDocuSign.Status == DocuSignEnvelopeStatus.completed.ToString())
                            npifDocuSign.ApprovalDate = item.DateOfStatus;

                        IUserManagementRepository userRep = new UserManagement.UserManagement();
                        MES.DTO.Library.Identity.LoginUser createdByItem = userRep.FindById(item.CreatedBy).Result;
                        if (createdByItem != null)
                            npifDocuSign.CreatedBy = createdByItem.FullName;
                        npifDocuSign.ApprovalSentDate = item.CreatedDate;

                        lstNPIFDocuSign.Add(npifDocuSign);
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.APQP.apqpNPIFDocuSign>>(errMSg, lstNPIFDocuSign);
            return response;
        }

        #endregion
    }
}
