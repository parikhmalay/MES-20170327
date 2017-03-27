using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using DocuSign.eSign.Client;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using MES.Business.Library;
using System.Web;
using System;
using MES.Business.Repositories.UserManagement;
namespace MES.Business.Library.BO.DocuSign
{
    class DocuSign : ContextBusinessBase
    {
        public DocuSign()
            : base(
                "DocuSign") { }
        public NPE.Core.ITypedResponse<DTO.Library.APQP.APQP.apqpNPIFDocuSign> SendNPIF(MES.DTO.Library.APQP.APQP.apqpNPIFDocuSign npifDocuSign, string fileName)
        {
            string errMSg = null, successMsg = null;
            try
            {

                Configuration.Default.DefaultHeader.Clear();
                string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DocuSignUsername"]);// "roma.patel@nonprofiteasy.net";
                string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DocuSignPassword"]); //"test123";
                string integratorKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DocuSignIntegratorKey"]);// "7a30aada-3576-4777-9a0b-9cb3e73c329d";

                //signFile = filePath;
                string basePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DocuSignBasePath"]); //"https://demo.docusign.net/restapi"; // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)

                // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)
                // instantiate a new api client
                ApiClient apiClient = new ApiClient(basePath);

                // set client in global config so we don't need to pass it to each API object.
                Configuration.Default.ApiClient = apiClient;

                //===========================================================
                // Step 1: Login()
                //===========================================================
                // we set the api client in global config when we configured the client 
                apiClient = Configuration.Default.ApiClient;
                string authHeader = "{\"Username\":\"" + username + "\", \"Password\":\"" + password + "\", \"IntegratorKey\":\"" + integratorKey + "\"}";
                Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", authHeader);

                // we will retrieve this from the login() results
                string accountId = null;

                // the authentication api uses the apiClient (and X-DocuSign-Authentication header) that are set in Configuration object
                AuthenticationApi authApi = new AuthenticationApi();
                LoginInformation loginInfo = authApi.Login();

                // find the default account for this user
                foreach (LoginAccount loginAcct in loginInfo.LoginAccounts)
                {
                    if (loginAcct.IsDefault == "true")
                    {
                        accountId = loginAcct.AccountId;
                        break;
                    }
                }
                if (accountId == null)
                { // if no default found set to first account
                    accountId = loginInfo.LoginAccounts[0].AccountId;
                }

                //===========================================================
                // Step 2: Signature Request (AKA create & send Envelope)
                //===========================================================
                var httpcontext = HttpContext.Current;
                string signFile = httpcontext.Server.MapPath("~\\") + Constants.NPIFDocumentFolder + fileName; // specify the document we want signed

                // Read a file from disk to use as a document
                byte[] fileBytes = File.ReadAllBytes(signFile);

                EnvelopeDefinition envDef = new EnvelopeDefinition();
                envDef.AllowMarkup = "true";
                envDef.EmailSubject = "Please DocuSign this document: " + Path.GetFileName(signFile);
                envDef.EmailBlurb = "Hi All,"
                                    + @"Please review your sections for the attached NPIF(s). If there is an item with which you disagree or for which you would like to provide additional commentary, go to “Other Actions” in the top right hand corner and Markup."
                                    + @" If additional commentary is added, all signers must initial next to the comments even if they have previously signed off on the document.";
                string documentId = string.IsNullOrEmpty(npifDocuSign.DocumentId) ? "1" : npifDocuSign.DocumentId;
                // Add a document to the envelope
                Document doc = new Document();
                doc.DocumentBase64 = System.Convert.ToBase64String(fileBytes);
                doc.Name = fileName;
                doc.DocumentId = documentId;//npif-1
                npifDocuSign.DocumentId = doc.DocumentId;

                doc.FileExtension = "xls";

                envDef.Documents = new List<Document>();
                envDef.Documents.Add(doc);

                envDef.Recipients = new Recipients();
                envDef.Recipients.Signers = new List<Signer>();

                SignHere signHere = null;
                Signer signer = null;
                IUserManagementRepository userObj = new UserManagement.UserManagement();
                int counter = 1;
                foreach (var item in npifDocuSign.lstNPIFDocuSignApprovers)
                {
                    if (item.UserId == null) { break; }
                    DTO.Library.Identity.LoginUser user = userObj.FindById(item.UserId).Result;
                    signer = new Signer();

                    // Add a recipient to sign the documeent
                    signer.Name = user.FullName;
                    signer.Email = user.Email;
                    signer.RecipientId = counter.ToString();
                    counter++;
                    // Create a |SignHere| tab somewhere on the document for the recipient to sign
                    signer.Tabs = new Tabs();
                    signer.Tabs.SignHereTabs = new List<SignHere>();

                    signHere = new SignHere();
                    signHere.DocumentId = doc.DocumentId;
                    signHere.PageNumber = "1";
                    signHere.RecipientId = signer.RecipientId;

                    //in db
                    item.RecipientId = signer.RecipientId;
                    signer.RoutingOrder = item.RoutingOrder.ToString();
                    switch (item.DesignationId)
                    {
                        case (int)MES.Business.Library.Enums.DesignationFixedId.APQPQualityEngineer:
                            signHere.AnchorString = "_QE_";
                            break;
                        case (int)MES.Business.Library.Enums.DesignationFixedId.QualityManager:
                            signHere.AnchorString = "_QM_";
                            break;
                        case (int)MES.Business.Library.Enums.DesignationFixedId.SupplyChainManager:
                            signHere.AnchorString = "_SCM_";
                            break;                      
                        case (int)MES.Business.Library.Enums.DesignationFixedId.SourcingManager:
                            signHere.AnchorString = "_CSM_";
                            break;
                        case (int)MES.Business.Library.Enums.DesignationFixedId.SalesManager:
                            signHere.AnchorString = "_DOS_";
                            break;
                        case (int)MES.Business.Library.Enums.DesignationFixedId.AccountManager:
                            signHere.AnchorString = "_SAM_";
                            break;
                        default:
                            break;
                    }

                    signHere.AnchorXOffset = "0.01";
                    signHere.AnchorYOffset = "0.35";
                    signHere.AnchorIgnoreIfNotPresent = "false";
                    signHere.AnchorUnits = "inches";
                    signHere.ScaleValue = .50m;

                    signer.Tabs.SignHereTabs.Add(signHere);
                    envDef.Recipients.Signers.Add(signer);
                }
                
                // set envelope status to "sent" to immediately send the signature request
                envDef.Status = "sent";

                // Use the EnvelopesApi to create and send the signature request
                EnvelopesApi envelopesApi = new EnvelopesApi();
                EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);

                //Get Document
                MemoryStream docStream = (MemoryStream)envelopesApi.GetDocument(accountId, envelopeSummary.EnvelopeId, documentId);
                string blobFilePath = Constants.NPIFDOCUSIGNFILEPATH;
                string blobFolderName = Constants.NPIFDOCUSIGNFOLDERNAME;

                string generatedFileName = "APQP-NPIF-" + npifDocuSign.PartNumber + "-" + DateTime.Now.ToString("yyyyMMdd") + "-Sent.pdf";//APQP-NPIF-PN-DateGenerated: APQP-NPIF-PT24304-20150519

                string filePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                          + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                          + blobFilePath
                          + generatedFileName;
                Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), filePath);

                string uploadedpath = MES.Business.Library.Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                      , blobFolderName, generatedFileName, docStream);

                npifDocuSign.EnvelopeId = envelopeSummary.EnvelopeId;
                npifDocuSign.InitialDocumentPath = uploadedpath;
                npifDocuSign.Status = envelopeSummary.Status;

                envDef = null;
                Configuration.Default.ApiClient = null;
                Configuration.Default.DefaultHeader.Clear();
                httpcontext.Response.Headers.Remove("X-DocuSign-Authentication");
                httpcontext = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return SuccessOrFailedResponse<DTO.Library.APQP.APQP.apqpNPIFDocuSign>(errMSg, npifDocuSign, successMsg);
        }

        public string GetDocuSignDocument(string envelopeId, string documentId, string partNumber, string type)
        {
            string blobFilePath = string.Empty, blobFolderName = string.Empty;
            switch (type)
            {
                case Constants.NPIFDocusign:
                    blobFilePath = Constants.NPIFDOCUSIGNFILEPATH;
                    blobFolderName = Constants.NPIFDOCUSIGNFOLDERNAME;

                    break;
                case Constants.PPAPDocusign:
                    blobFilePath = Constants.PPAPDOCUSIGNFILEPATH;
                    blobFolderName = Constants.PPAPDOCUSIGNFOLDERNAME;

                    break;
                default:
                    blobFilePath = Constants.NPIFDOCUSIGNFILEPATH;
                    blobFolderName = Constants.NPIFDOCUSIGNFOLDERNAME;

                    break;
            }

            string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DocuSignUsername"]);// "roma.patel@nonprofiteasy.net";
            string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DocuSignPassword"]); //"test123";
            string integratorKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DocuSignIntegratorKey"]);// "7a30aada-3576-4777-9a0b-9cb3e73c329d";

            //signFile = filePath;
            string basePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DocuSignBasePath"]); //"https://demo.docusign.net/restapi"; // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)

            // instantiate a new api client
            ApiClient apiClient = new ApiClient(basePath);

            // set client in global config so we don't need to pass it to each API object.
            Configuration.Default.ApiClient = apiClient;

            //===========================================================
            // Step 1: Login()
            //===========================================================
            // we set the api client in global config when we configured the client 
            apiClient = Configuration.Default.ApiClient;
            string authHeader = "{\"Username\":\"" + username + "\", \"Password\":\"" + password + "\", \"IntegratorKey\":\"" + integratorKey + "\"}";
            Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", authHeader);

            // we will retrieve this from the login() results
            string accountId = null;

            // the authentication api uses the apiClient (and X-DocuSign-Authentication header) that are set in Configuration object
            AuthenticationApi authApi = new AuthenticationApi();
            LoginInformation loginInfo = authApi.Login();

            // find the default account for this user
            foreach (LoginAccount loginAcct in loginInfo.LoginAccounts)
            {
                if (loginAcct.IsDefault == "true")
                {
                    accountId = loginAcct.AccountId;
                    break;
                }
            }
            if (accountId == null)
            { // if no default found set to first account
                accountId = loginInfo.LoginAccounts[0].AccountId;
            }

            //===========================================================
            // Step 2: List Envelope Document(s)
            //===========================================================
            EnvelopesApi envelopesApi = new EnvelopesApi();
            //Get Document
            MemoryStream docStream = (MemoryStream)envelopesApi.GetDocument(accountId, envelopeId, documentId);

            string generatedFileName = "APQP-NPIF-" + partNumber + "-" + DateTime.Now.ToString("yyyyMMdd") + "-Approved.pdf";//APQP-NPIF-PN-DateGenerated: APQP-NPIF-PT24304-20150519

            string filePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                      + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                      + blobFilePath
                      + generatedFileName;
            Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), filePath);

            string uploadedpath = MES.Business.Library.Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                  , blobFolderName, generatedFileName, docStream);

            Configuration.Default.ApiClient = null;
            Configuration.Default.DefaultHeader.Clear();
            return uploadedpath;
        }

        public Envelope CheckStatusOfDocuSign(MES.Data.Library.NPIFDocusign npifDocuSign)
        {
            try
            {
                Configuration.Default.DefaultHeader.Clear();
                string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DocuSignUsername"]);// "roma.patel@nonprofiteasy.net";
                string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DocuSignPassword"]); //"test123";
                string integratorKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DocuSignIntegratorKey"]);// "7a30aada-3576-4777-9a0b-9cb3e73c329d";

                //signFile = filePath;
                string basePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DocuSignBasePath"]); //"https://demo.docusign.net/restapi"; // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)

                // instantiate a new api client
                ApiClient apiClient = new ApiClient(basePath);

                // set client in global config so we don't need to pass it to each API object.
                Configuration.Default.ApiClient = apiClient;

                //===========================================================
                // Step 1: Login()
                //===========================================================
                // we set the api client in global config when we configured the client 
                apiClient = Configuration.Default.ApiClient;
                string authHeader = "{\"Username\":\"" + username + "\", \"Password\":\"" + password + "\", \"IntegratorKey\":\"" + integratorKey + "\"}";
                Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", authHeader);

                // we will retrieve this from the login() results
                string accountId = null;

                // the authentication api uses the apiClient (and X-DocuSign-Authentication header) that are set in Configuration object
                AuthenticationApi authApi = new AuthenticationApi();
                LoginInformation loginInfo = authApi.Login();

                // find the default account for this user
                foreach (LoginAccount loginAcct in loginInfo.LoginAccounts)
                {
                    if (loginAcct.IsDefault == "true")
                    {
                        accountId = loginAcct.AccountId;
                        break;
                    }
                }
                if (accountId == null)
                { // if no default found set to first account
                    accountId = loginInfo.LoginAccounts[0].AccountId;
                }


                //===========================================================
                // Step 2: List Envelope Document(s)
                //===========================================================
                EnvelopesApi envelopesApi = new EnvelopesApi();
                Envelope envInfo = envelopesApi.GetEnvelope(accountId, npifDocuSign.EnvelopeId);

                Configuration.Default.ApiClient = null;
                Configuration.Default.DefaultHeader.Clear();

                return envInfo;
            }
            catch (Exception ex)
            { throw ex; }
        }
    }
}