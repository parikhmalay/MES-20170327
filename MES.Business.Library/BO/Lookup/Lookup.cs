using NPE.Business.Common.Base;
using NPE.Core;
using NPE.Core.Extended;
using NPE.Core.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Core.Constants;
using System.Runtime.Serialization;
using MES.Business.Repositories.Lookup;
using MES.Business.Library;
using MES.Business.Library.Enums;
using MES.Identity.Data.Library;
using NPE.Core.Extensions;
using MES.DTO.Library.Identity;

namespace MES.Business.Library.BO.Lookup
{
    public delegate void LookupCollectionDelegate(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder);
    public class Lookup : ContextBusinessBase, ILookupRepository
    {
        //Lookup Delegate
        private Dictionary<string, LookupCollectionDelegate> lookupDelegates = new Dictionary<string, LookupCollectionDelegate>();

        public Lookup()
            : base("Lookup")
        {
            initLookups();
        }

        //Fetch all lookup names
        public List<string> GetLookupNames()
        {
            var fields = typeof(LookupTypes).GetFields();
            List<string> namesList = new List<string>();
            foreach (var field in fields)
            {
                DescriptionAttribute[] attribs = field.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                if (attribs != null && attribs.Length > 0)
                {
                    namesList.Add(attribs[0].Description);
                }
            }
            return namesList;
        }

        public NPE.Core.LookupCollections Query(List<NPE.Core.Extended.Query> lookups)
        {
            return QueryLookups(lookups);
        }

        private LookupCollections QueryLookups(List<NPE.Core.Extended.Query> lookups)
        {
            LookupCollections collections = new LookupCollections();
            foreach (var lookup in lookups)
            {
                short? orderBy = null;
                if (lookup.Parameters != null)
                {
                    if (lookup.Parameters.ContainsKey("sort"))
                    {
                        orderBy = Convert.ToInt16(lookup.Parameters["sort"]);
                    }
                }

                LookupCollection col = QueryLookupData(lookup.Name, lookup.Parameters, orderBy);
                collections.Add(col);
            }

            return collections;

        }

        private LookupCollection QueryLookupData(string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            LookupCollection defaultCollection = LookupCollection.Empty(lookupName);

            if (lookupDelegates.ContainsKey(lookupName))
                lookupDelegates[lookupName](defaultCollection, lookupName, parameters, sortOrder);

            return defaultCollection;
        }

        private void initLookups()
        {
            #region Setup Lookups
            lookupDelegates.Add(LookupTypes.Destinations.ToString(), GetDestinations);
            lookupDelegates.Add(LookupTypes.Origins.ToString(), GetOrigins);
            lookupDelegates.Add(LookupTypes.Forwarders.ToString(), GetForwarders);
            lookupDelegates.Add(LookupTypes.RFQTypes.ToString(), GetRFQTypes);
            lookupDelegates.Add(LookupTypes.RFQSources.ToString(), GetRFQSource);
            lookupDelegates.Add(LookupTypes.NonAwardReasons.ToString(), GetNonAwardReason);
            lookupDelegates.Add(LookupTypes.Process.ToString(), GetProcess);
            lookupDelegates.Add(LookupTypes.Commodity.ToString(), GetCommodity);
            lookupDelegates.Add(LookupTypes.RFQSuppliers.ToString(), GetRFQSuppliers);
            lookupDelegates.Add(LookupTypes.EmailTemplates.ToString(), GetEmailTemplates);
            lookupDelegates.Add(LookupTypes.EmailTemplateBody.ToString(), GetEmailTemplateBody);
            lookupDelegates.Add(LookupTypes.MachineDesc.ToString(), GetMachineDesc);
            lookupDelegates.Add(LookupTypes.MachiningDesc.ToString(), GetMachiningDesc);
            lookupDelegates.Add(LookupTypes.MachiningSecOperation.ToString(), GetMachiningSecOperation);
            lookupDelegates.Add(LookupTypes.CoatingTypes.ToString(), GetCoatingTypes);
            lookupDelegates.Add(LookupTypes.ProjectCategories.ToString(), GetProjectCategories);
            lookupDelegates.Add(LookupTypes.Categories.ToString(), GetCategories);
            lookupDelegates.Add(LookupTypes.Roles.ToString(), GetRoles);
            lookupDelegates.Add(LookupTypes.DefectType.ToString(), GetDefectType);
            lookupDelegates.Add(LookupTypes.RFQPriority.ToString(), GetRFQPriority);
            lookupDelegates.Add(LookupTypes.IndustryTypes.ToString(), GetIndustryTypes);
            #endregion Setup Lookups

            lookupDelegates.Add(LookupTypes.AssociatedToItems.ToString(), GetAssociatedToItems);
            lookupDelegates.Add(LookupTypes.Suppliers.ToString(), GetSuppliers);
            lookupDelegates.Add(LookupTypes.Customers.ToString(), GetCustomers);
            lookupDelegates.Add(LookupTypes.CustomerContacts.ToString(), GetCustomerContacts);
            lookupDelegates.Add(LookupTypes.Users.ToString(), GetUsers);
            lookupDelegates.Add(LookupTypes.Countries.ToString(), GetCountries);
            lookupDelegates.Add(LookupTypes.Status.ToString(), GetAssociatedToItems);
            lookupDelegates.Add(LookupTypes.CommodityTypes.ToString(), GetCommodityTypes);
            lookupDelegates.Add(LookupTypes.Prefixes.ToString(), GetPrefixes);
            lookupDelegates.Add(LookupTypes.DocumentTypes.ToString(), GetDocumentTypes);
            lookupDelegates.Add(LookupTypes.Genders.ToString(), GetGenders);
            lookupDelegates.Add(LookupTypes.Designations.ToString(), GetDesignations);
            lookupDelegates.Add(LookupTypes.EmailByUser.ToString(), GetUserEmail);

            #region Get Users by Designation Id
            lookupDelegates.Add(LookupTypes.SupplierQuality.ToString(), GetUsers);
            lookupDelegates.Add(LookupTypes.RFQCoordinators.ToString(), GetRFQCoordinators);
            lookupDelegates.Add(LookupTypes.SAM.ToString(), GetSAMUsers);
            lookupDelegates.Add(LookupTypes.SCUsers.ToString(), GetSCCs);//Get Supplier Chain Coordinators
            lookupDelegates.Add(LookupTypes.APQPEngineers.ToString(), GetAPQPEngineersUsers);
            #endregion Get Users by Designation Id


            lookupDelegates.Add(LookupTypes.CustomerForSupplierQuote.ToString(), GetCustomerForSupplierQuote);
            lookupDelegates.Add(LookupTypes.RFQForSupplierQuote.ToString(), GetRFQForSupplierQuote);
            lookupDelegates.Add(LookupTypes.SupplierForSupplierQuote.ToString(), GetSupplierForSupplierQuote);

            lookupDelegates.Add(LookupTypes.QuoteStatus.ToString(), GetQuoteStatus);
            lookupDelegates.Add(LookupTypes.QuoteAssumptions.ToString(), GetQuoteAssumptions);
            lookupDelegates.Add(LookupTypes.QuoteMESComments.ToString(), GetQuoteMESComments);
            lookupDelegates.Add(LookupTypes.AddressType.ToString(), GetAddressType);

            lookupDelegates.Add(LookupTypes.Pagesizes.ToString(), GetPagesizes);
            lookupDelegates.Add(LookupTypes.DefaultLandingPages.ToString(), GetDefaultLandingPages);
            lookupDelegates.Add(LookupTypes.AssignmentUsers.ToString(), GetAssignmentUsers);
            lookupDelegates.Add(LookupTypes.RMAInitiatedBy.ToString(), GetRMAInitiatedBy);

            lookupDelegates.Add(LookupTypes.CurrentUser.ToString(), GetCurrentUserId);

            lookupDelegates.Add(LookupTypes.UserWithDesignation.ToString(), GetUserWithDesigntion);

            lookupDelegates.Add(LookupTypes.SupplierItems.ToString(), GetSupplierList);
            lookupDelegates.Add(LookupTypes.SQItems.ToString(), GetSQList);
            lookupDelegates.Add(LookupTypes.CountryItems.ToString(), GetCountryList);

            lookupDelegates.Add(LookupTypes.CommodityTypesSALR.ToString(), GetCommodityTypesSALR);

            #region APQP
            lookupDelegates.Add(LookupTypes.APQPStatus.ToString(), GetAPQPStatus);
            lookupDelegates.Add(LookupTypes.SupplierWithCode.ToString(), GetSuppliersWithSupplierCode);
            lookupDelegates.Add(LookupTypes.PPAPSubmissionLevel.ToString(), GetAPQPStatus);
            lookupDelegates.Add(LookupTypes.MESWarehouses.ToString(), GetMESWarehouses);
            lookupDelegates.Add(LookupTypes.ProjectStages.ToString(), GetProjectStages);
            lookupDelegates.Add(LookupTypes.ProjectStagesWithoutCategoryId.ToString(), GetProjectStagesWithoutCategoryId);
            lookupDelegates.Add(LookupTypes.APQPDocumentType.ToString(), GetDocumentTypeForAPQP);
            lookupDelegates.Add(LookupTypes.Parts.ToString(), GetAPQPPartNoWithDesc);
            lookupDelegates.Add(LookupTypes.crDocumentTypes.ToString(), GetCRDocumentTypes);
            lookupDelegates.Add(LookupTypes.SAPItemByCustomer.ToString(), GetSAPItemByCustomer);
            lookupDelegates.Add(LookupTypes.SAPCustomers.ToString(), GetSAPCustomers);
            lookupDelegates.Add(LookupTypes.SAPSuppliers.ToString(), GetSAPSuppliers);
            lookupDelegates.Add(LookupTypes.SupplierContacts.ToString(), GetSupplierContacts);

            lookupDelegates.Add(LookupTypes.QuoteNumbers.ToString(), GetQuoteNumberList);
            lookupDelegates.Add(LookupTypes.SAPCustomersName.ToString(), GetSAPCustomersName);
            lookupDelegates.Add(LookupTypes.SAPSuppliersName.ToString(), GetSAPSuppliersName);
            lookupDelegates.Add(LookupTypes.DTDocumentType.ToString(), GetDocumentTypeForDefectTracking);
            lookupDelegates.Add(LookupTypes.SAPSuppliersByCustomer.ToString(), GetSAPSuppliersByCustomer);

            lookupDelegates.Add(LookupTypes.NPIFDesignations.ToString(), GetNPIFRecipientsDesignations);
            lookupDelegates.Add(LookupTypes.UserByDesignation.ToString(), GetUserByDesignation);
            #region CAPA
            lookupDelegates.Add(LookupTypes.CAPAQuery.ToString(), GetCAPAQueries);
            lookupDelegates.Add(LookupTypes.CAPAApproverTitle.ToString(), GetCAPAApproverTitles);
            lookupDelegates.Add(LookupTypes.SAPCustomersBySupplier.ToString(), GetSAPCustomersBySupplier);
            lookupDelegates.Add(LookupTypes.CAPADocumentType.ToString(), GetDocumentTypeForCAPA);
            lookupDelegates.Add(LookupTypes.SAPSuppliersByPartCode.ToString(), GetSAPSupplierByItemCode);
            #endregion

            #endregion APQP

        }

        private void GetRoles(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.Roles.Where(item => item.Active == true))
                {
                    outputCollection.Add(item.Id, item.RoleName);
                }
            }, true);
        }
        
        private void GetAPQPEngineersUsers(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.GetAPQPEngineersUsers())
                {
                    outputCollection.Add(item.Id, item.FullName);
                }
            }, true);
        }

        private void GetSCCs(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.GetSCCs())
                {
                    outputCollection.Add(item.Id, item.FullName);
                }
            }, true);
        }

        private void GetUserWithDesigntion(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {

                foreach (var item in context.GetUserDesignation())
                {
                    outputCollection.Add(item.Id, item.User);
                }


            }, true);
        }

        private void GetQuoteStatus(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.QuoteStatus.OrderBy(item => item.Status))
                {
                    outputCollection.Add(item.Id, item.Status);
                }
            }, true);
        }

        private void GetQuoteAssumptions(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            int associatedToId = Convert.ToInt32(MES.Business.Library.Enums.RemarksAssociatedTo.QuoteAssumptions);

            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.GetRemarksLookup(associatedToId))
                {
                    outputCollection.Add(item.Id, item.Remarks);
                }
            }, true);
        }


        private void GetQuoteMESComments(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            int associatedToId = Convert.ToInt32(MES.Business.Library.Enums.RemarksAssociatedTo.QuoteMESComments);

            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.GetRemarksLookup(associatedToId))
                {
                    outputCollection.Add(item.Id, item.Remarks);
                }
            }, true);
        }
        //Get Customer For SupplierQuote
        private void GetCustomerForSupplierQuote(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            string partNumber = string.Empty;
            if (parameters.ContainsKey("PartNumber"))
                partNumber = string.IsNullOrEmpty(parameters["PartNumber"]) ? "" : Convert.ToString(parameters["PartNumber"]);
            this.RunOnDB(context =>
            {
                foreach (var item in context.GetCustomerForSupplierQuote(partNumber).OrderBy(item => item.CompanyName))
                {
                    outputCollection.Add(item.Id, item.CompanyName);
                }
            }, true);
        }

        //Get RFQs For SupplierQuote
        private void GetRFQForSupplierQuote(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            string partNumber = string.Empty;
            int customerId = string.IsNullOrEmpty(parameters["CustomerId"]) ? 0 : Convert.ToInt32(parameters["CustomerId"]);

            if (parameters.ContainsKey("PartNumber"))
                partNumber = string.IsNullOrEmpty(parameters["PartNumber"]) ? "" : Convert.ToString(parameters["PartNumber"]);
            this.RunOnDB(context =>
            {
                foreach (var item in context.GetRFQForSupplierQuote(partNumber, customerId).OrderByDescending(item => item.Id))
                {
                    outputCollection.Add(item.Id, item.Id + " - " + item.CompanyName, "", item.CustomerId, null);
                }
            }, true);
        }

        //get Supplier For Supplier Quote
        private void GetSupplierForSupplierQuote(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            string rfqId = string.IsNullOrEmpty(parameters["RFQId"]) ? string.Empty : parameters["RFQId"].ToString();

            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                if (!string.IsNullOrEmpty(rfqId))
                {
                    foreach (var item in context.SearchSuppliersQuotedNotQuoted(rfqId).OrderByDescending(item => item.RFQId))
                    {
                        outputCollection.Add(item.SupplierId, item.CompanyName, "", item.GroupName, null);
                    }
                }
            }, true);
        }

        private void GetEmailTemplateBody(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            int emailTypeId = string.IsNullOrEmpty(parameters["emailTypeId"]) ? 0 : Convert.ToInt32(parameters["emailTypeId"]);

            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.EmailTemplates.Where(item => item.IsDeleted == false && item.Id == emailTypeId))
                {
                    outputCollection.Add(item.Id, item.EmailBody);
                }
            }, true);
        }

        private void GetRFQSuppliers(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            string rfqId = string.IsNullOrEmpty(parameters["rfqId"]) ? string.Empty : parameters["rfqId"].ToString();

            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                if (!string.IsNullOrEmpty(rfqId))
                {
                    foreach (var item in context.SearchRFQSuppliers(rfqId))
                    {
                        outputCollection.Add(item.Id, item.CompanyName);
                    }

                }
            }, true);
        }

        private void GetEmailTemplates(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.EmailTemplates.Where(item => item.IsDeleted == false).OrderBy(item => item.Title))
                {
                    outputCollection.Add(item.Id, item.Title);
                }
            }, true);
        }
        //Get Customer Contacts by CustomerId
        private void GetCustomerContacts(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            int customerId = 0;
            if (parameters.ContainsKey("customerId"))
                customerId = string.IsNullOrEmpty(parameters["customerId"]) ? 0 : Convert.ToInt32(parameters["customerId"]);

            this.RunOnDB(context =>
            {
                //if (customerId > 0)
                {
                    foreach (var item in context.Contacts.Where(item => item.IsDeleted == false).OrderBy(item => item.FirstName + " " + item.LastName)
                        .Where(item => ((customerId == 0) || (item.CustomerId == customerId))))
                    {
                        outputCollection.Add(item.Id, item.FirstName + " " + item.LastName, "", item.CustomerId, null);
                    }
                }
            }, true);
        }
        //Get IndustryTypes
        private void GetIndustryTypes(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.IndustryTypes.Where(item => item.IsDeleted == false).OrderBy(item => item.IndustryType1))
                {
                    outputCollection.Add(item.Id, item.IndustryType1);
                }
            }, true);
        }
       
        //Get RFQTypes
        private void GetRFQTypes(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.RFQTypes.Where(item => item.IsDeleted == false).OrderBy(item => item.RFQTypeName))
                {
                    outputCollection.Add(item.Id, item.RFQTypeName);
                }
            }, true);
        }
        //Get Commodity
        private void GetCommodity(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.GetCommodity())
                {
                    outputCollection.Add(item.Id, item.CommodityName, "", item.CategoryName, null);
                }
            }, true);
        }
        //Get Process
        private void GetProcess(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.Processes.Where(item => item.IsDeleted == false).OrderBy(item => item.ProcessName))
                {
                    outputCollection.Add(item.Id, item.ProcessName);
                }
            }, true);
        }
        //Ger NonAwardReason
        private void GetNonAwardReason(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.NonAwardFeedbacks.Where(item => item.IsDeleted == false).OrderBy(item => item.NonAwardFeedback1))
                {
                    outputCollection.Add(item.Id, item.NonAwardFeedback1);
                }
            }, true);
        }
        //Get RFQ Source
        private void GetRFQSource(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.RFQSources.Where(item => item.IsDeleted == false).OrderBy(item => item.RFQSource1))
                {
                    outputCollection.Add(item.Id, item.RFQSource1);
                }
            }, true);
        }
        //Get RFQ coordinators
        private void GetRFQCoordinators(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;

            this.RunOnDB(context =>
             {
                 foreach (var item in context.GetRfqCoordinators())
                 {
                     outputCollection.Add(item.Id, item.FullName);
                 }
             }, true);
        }
        //Get Forwarders
        private void GetForwarders(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.Forwarders.Where(item => item.IsDeleted == false).OrderBy(item => item.ForwarderName))
                {
                    outputCollection.Add(item.Id, item.ForwarderName);
                }
            }, true);
        }
        //Get Origins
        private void GetOrigins(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.Origins.Where(item => item.IsDeleted == false).OrderBy(item => item.Origin1))
                {
                    outputCollection.Add(item.Id, item.Origin1);
                }
            }, true);
        }
        //Get Destinations
        private void GetDestinations(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.Destinations.Where(item => item.IsDeleted == false).OrderBy(item => item.Destination1))
                {
                    outputCollection.Add(item.Id, item.Destination1);
                }
            }, true);
        }
       
        //Get RFQ Priority
        private void GetRFQPriority(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
              outputCollection.KeyType = KeyTypes.String;
              this.RunOnDB(context =>
              {
                  foreach (var item in context.RFQPriorities.Where(item => item.IsDeleted == false).OrderBy(item => item.rfqPriority1))
                  {
                      outputCollection.Add(item.Id, item.rfqPriority1);
                  }
              }, true);
        }

        //Get Project categories
        private void GetProjectCategories(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
           {
               foreach (var item in context.ProjectCategories.Where(item => item.IsDeleted == false).OrderBy(item => item.ProjectCategory1))
               {
                   outputCollection.Add(item.Id, item.ProjectCategory1);
               }
           }, true);
        }

        //Get commodity categories
        private void GetCategories(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.CommodityCategories.Where(item => item.IsDeleted == false).OrderBy(item => item.Category))
                {
                    outputCollection.Add(item.Id, item.Category);
                }
            }, true);
        }

        //Get Associated To Items
        private void GetAssociatedToItems(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            string source = string.Empty;
            switch (parameters["source"])
            {
                case "DT"://Document Type
                    source = Constants.DOCUMENTTYPE.ToString();

                    break;
                case "RM"://Remarks
                    source = Constants.REMARKS.ToString();

                    break;
                case "ST"://Status
                    source = Constants.STATUS.ToString();

                    break;
                case "SS"://Supplier status
                    source = Constants.SUPPLIERSTATUS.ToString();
                    break;
                default:
                    break;

            }

            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.AssociatedToes.Where(a => a.IsDeleted == false && a.Source.ToUpper() == source).OrderBy(item => item.AssociatedTo1))
                {
                    outputCollection.Add(item.Id, item.AssociatedTo1);
                }
            }, true);
        }

        //Get Suppliers
        private void GetSuppliers(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.Suppliers.Where(item => item.IsDeleted == false).OrderBy(item => item.CompanyName))
                {
                    outputCollection.Add(item.Id, item.CompanyName);
                }
            }, true);
        }

        //Get Customers
        private void GetCustomers(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.Customers.Where(item => item.IsDeleted == false).OrderBy(item => item.CompanyName))
                {
                    outputCollection.Add(item.Id, item.CompanyName);
                }
            }, true);
        }

        //Get Users
        private void GetUsers(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;

            using (var db = new ApplicationDbContext())
            {
                var userlist = db.Users.Where(item => item.IsDeleted == false && item.Active == true).OrderBy(item => item.FirstName);
                foreach (var item in userlist)
                {
                    outputCollection.Add(item.Id, item.FirstName + " " + item.LastName);
                }
            }
        }
        //Get Users having Designation = SAM
        private void GetSAMUsers(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.GetSAMUsers())
                {
                    outputCollection.Add(item.Id, item.FullName);
                }
            }, true);

        }
        //Get Users
        private void GetUserEmail(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            string userId = Convert.ToString(parameters["userId"]);
            using (var db = new ApplicationDbContext())
            {
                var userEmail = db.Users.Where(item => item.Id == userId);
                foreach (var item in userEmail)
                {                 
                    outputCollection.Add("UserEmail", item.Email);
                }
            }
        }
        //Get Countries
        private void GetCountries(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.Countries.OrderBy(item => item.Value).OrderBy(item => item.Value))
                {
                    outputCollection.Add(item.Id, item.Value);
                }
            }, true);
        }

        //Get CommodityTypes
        private void GetCommodityTypes(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.CommodityTypes.Where(cm => cm.IsDeleted == false).ToList().OrderBy(item => item.CommodityType1))
                {
                    outputCollection.Add(item.Id, item.CommodityType1);
                }
            }, true);
        }

        //Get Prefix
        private void GetPrefixes(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.Prefixes.OrderBy(item => item.Value).OrderBy(item => item.Value))
                {
                    outputCollection.Add(item.Id, item.Value);
                }
            }, true);
        }

        //Get DocumentType
        private void GetDocumentTypes(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            int associatedTo_id = 0;
            switch (parameters["associatedTo_id"])
            {
                case "SH"://SHIPMENT
                    associatedTo_id = (int)MES.Business.Library.Enums.AssociatedTo.Shipment;

                    break;
                case "SP"://SUPPLIER
                    associatedTo_id = (int)MES.Business.Library.Enums.AssociatedTo.Supplier;

                    break;
                case "APQPSTEP1":
                    associatedTo_id = (int)MES.Business.Library.Enums.AssociatedTo.APQPStep1;

                    break;
                case "APQPSTEP2":
                    associatedTo_id = (int)MES.Business.Library.Enums.AssociatedTo.APQPStep2;
                    break;
                case "APQPSTEP3":
                    associatedTo_id = (int)MES.Business.Library.Enums.AssociatedTo.APQPStep3;

                    break;
                case "APQPSTEP4":
                    associatedTo_id = (int)MES.Business.Library.Enums.AssociatedTo.APQPStep4;
                    break;
                case "CR"://CHANGE REQUEST
                    associatedTo_id = (int)MES.Business.Library.Enums.AssociatedTo.ChangeRequest;

                    break;
                case "DTPD":
                    associatedTo_id = (int)MES.Business.Library.Enums.AssociatedTo.DefectTrackingPartDocument;

                    break;
                case "DTCA":
                    associatedTo_id = (int)MES.Business.Library.Enums.AssociatedTo.DefectTrackingCorrectiveAction;

                    break;
                default:
                    break;

            }
            outputCollection.KeyType = KeyTypes.String;

            this.RunOnDB(context =>
            {
                #region select data after joining the tables
                var query = from DocumentType in context.DocumentTypes
                            join DocumentTypeAssociatedTo in context.DocumentTypeAssociatedToes
                             on DocumentType.Id equals DocumentTypeAssociatedTo.DocumentTypeId
                            join AssociatedTo in context.AssociatedToes
                              on DocumentTypeAssociatedTo.AssociatedToId equals AssociatedTo.Id
                            where AssociatedTo.Source.ToUpper() == "Document Type".ToUpper() && AssociatedTo.Id == associatedTo_id
                            select new
                            {
                                DocumentType.Id,
                                DocumentType.DocumentType1,
                                DocumentType.IsDeleted
                            };
                #endregion

                foreach (var item in query.Where(dt => dt.IsDeleted == false).OrderBy(item => item.DocumentType1).Distinct().ToList())
                {
                    outputCollection.Add(item.Id, item.DocumentType1);
                }
            }, true);
        }

        //Get Gender
        private void GetGenders(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.Genders.OrderBy(item => item.Value).OrderBy(item => item.Value))
                {
                    outputCollection.Add(item.Id, item.Value);
                }
            }, true);
        }

        //Get Designation
        private void GetDesignations(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.Designations.OrderBy(item => item.Designation1).Where(item => item.IsDeleted == false).OrderBy(item => item.Designation1))
                {
                    outputCollection.Add(item.Id, item.Designation1);
                }
            }, true);
        }

        //Get NPIF Recipients Designation
        private void GetNPIFRecipientsDesignations(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.GetNPIFRecipientsDesignation())
                {
                    outputCollection.Add(item.Id, item.Designation);
                }
            }, true);
        }

        //Get MachineDesc
        private void GetMachineDesc(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.MachineDescs.Where(cm => cm.IsDeleted == false).ToList().OrderBy(item => item.MachineDescription))
                {
                    outputCollection.Add(item.Id, item.MachineDescription);
                }
            }, true);
        }

        //Get MachiningDesc
        private void GetMachiningDesc(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.MachiningDescs.Where(cm => cm.IsDeleted == false).ToList().OrderBy(item => item.MachiningDescription))
                {
                    outputCollection.Add(item.Id, item.MachiningDescription);
                }
            }, true);
        }

        //Get MachiningSecOperation
        private void GetMachiningSecOperation(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.SecondaryOperationDescs.Where(cm => cm.IsDeleted == false).ToList().OrderBy(item => item.SecondaryOperationDescription))
                {
                    outputCollection.Add(item.Id, item.SecondaryOperationDescription);
                }
            }, true);
        }

        //Get CoatingTypes
        private void GetCoatingTypes(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.CoatingTypes.Where(cm => cm.IsDeleted == false).ToList().OrderBy(item => item.CoatingType1))
                {
                    outputCollection.Add(item.Id, item.CoatingType1);
                }
            }, true);
        }
        //Get AddressType
        private void GetAddressType(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.AddressTypes.OrderBy(item => item.Id))
                {
                    outputCollection.Add(item.Id, item.Name);
                }
            }, true);
        }

        //get pagesize list
        private void GetPagesizes(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.PageSizes.OrderBy(item => item.OrderBy))
                {
                    outputCollection.Add(item.Size, item.SizeText, "", item.Id, null);
                }
            }, true);
        }

        //get DefaultLandingPages
        private void GetDefaultLandingPages(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.Objects.Where(item => item.CanBeDefault == true && item.IsActive == true).OrderBy(item => item.ObjectName))
                {
                    outputCollection.Add(item.ObjectId, item.ObjectName);
                }
            }, true);
        }
        //Get AssignmentUsers
        private void GetAssignmentUsers(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            string assignedDesignation = Convert.ToInt32(DesignationFixedId.APQPQualityEngineer).ToString() + "," +
                                                Convert.ToInt32(DesignationFixedId.QualityManager).ToString() + "," +
                                                Convert.ToInt32(DesignationFixedId.SupplierQualityEngineer).ToString();
            this.RunOnDB(context =>
            {
                foreach (var item in context.GetAssignToUserByDesignation(assignedDesignation, true))
                {
                    outputCollection.Add(item.Id, item.FullName);
                }
            }, true);
        }
        //Get RMAInitiatedBy
        private void GetRMAInitiatedBy(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            string assignedDesignation = Convert.ToInt32(DesignationFixedId.APQPQualityEngineer).ToString();
            this.RunOnDB(context =>
            {
                foreach (var item in context.GetAssignToUserByDesignation(assignedDesignation, true))
                {
                    outputCollection.Add(item.Id, item.FullName);
                }
            }, true);
        }
        //Get Users By DesignationId
        private void GetUserByDesignation(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            string DesignationId = string.IsNullOrEmpty(parameters["DesignationId"]) ? string.Empty : Convert.ToString(parameters["DesignationId"]);
            this.RunOnDB(context =>
            {
                foreach (var item in context.GetAssignToUserByDesignation(DesignationId, true))
                {
                    outputCollection.Add(item.Id, item.FullName);
                }
            }, true);
        }

        //Get Current UserId
        private void GetCurrentUserId(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            if (!(System.Threading.Thread.CurrentPrincipal).GetType().Equals(typeof(System.Security.Claims.ClaimsPrincipal)))
                System.Threading.Thread.CurrentPrincipal = System.Web.HttpContext.Current.User;
            string name = "";
            try
            {
                name = System.Security.Claims.ClaimsPrincipal.Current.GetSubjectId();
            }
            catch
            {
                name = "anonymous";
            }
            outputCollection.KeyType = KeyTypes.String;
            outputCollection.Add("UserId", name);
        }

        //Get Supplier List for report
        private void GetSupplierList(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {

                foreach (var item in context.GetSupplierList(null, null))
                {
                    outputCollection.Add(item.Id, item.CompanyName);
                }
            }, true);
        }

        //Get SQ List for report
        private void GetSQList(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            string SupplierIds = Convert.ToString(parameters["SupplierIds"]);
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {

                foreach (var item in context.GetSQList(SupplierIds))
                {
                    outputCollection.Add(item.UserId, item.UserName);
                }
            }, true);
        }

        //Get Country List for report
        private void GetCountryList(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            string SupplierIds = Convert.ToString(parameters["SupplierIds"]);
            string SQIds = Convert.ToString(parameters["SQIds"]);
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {

                foreach (var item in context.GetCountryList(SupplierIds, SQIds))
                {
                    outputCollection.Add(item.Id, item.Country);
                }
            }, true);
        }
        //Get CommodityTypes for supplier activity level report
        private void GetCommodityTypesSALR(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            string SupplierIds = Convert.ToString(parameters["SupplierIds"]);
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {

                foreach (var item in context.GetCommodityTypeBySupplierIds(SupplierIds))
                {
                    outputCollection.Add(item.Id, item.CommodityType);
                }
            }, true);
        }
        #region APQP
        //Get Associated To Items
        private void GetAPQPStatus(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            int? associatedTo_id = 0;
            switch (parameters["source"])
            {
                case "CR"://Change Request
                    associatedTo_id = Convert.ToInt32(MES.Business.Library.Enums.StatusAssociatedTo.ChangeRequest);

                    break;
                case "APQP"://APQP
                    associatedTo_id = Convert.ToInt32(MES.Business.Library.Enums.StatusAssociatedTo.APQP);

                    break;
                case "PPAP"://PPAP
                    associatedTo_id = Convert.ToInt32(MES.Business.Library.Enums.StatusAssociatedTo.PPAP);

                    break;
                default:
                    break;
            }

            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.apqpSearchStatus(associatedTo_id.Value))
                {
                    outputCollection.Add(item.Id, item.Status);
                }
            }, true);
        }

        //Get Suppliers with supplier Code
        private void GetSuppliersWithSupplierCode(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                string CompanyNameWithCode = string.Empty;
                foreach (var item in context.Suppliers.Where(item => item.IsDeleted == false).OrderBy(item => item.CompanyName))
                {
                    CompanyNameWithCode = !string.IsNullOrEmpty(item.SupplierCode) ? item.CompanyName + " - " + item.SupplierCode : item.CompanyName;
                    outputCollection.Add(item.Id, CompanyNameWithCode);
                }
            }, true);
        }
        //Get MESWarehouses
        private void GetMESWarehouses(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.Destinations.Where(item => item.IsDeleted == false && item.IsWarehouse == true).OrderBy(item => item.Destination1))
                {
                    outputCollection.Add(item.Id, item.Destination1);
                }
            }, true);
        }

        //get project stages by category id
        private void GetProjectStages(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            int ProjectCategoryId = string.IsNullOrEmpty(parameters["ProjectCategoryId"]) ? 0 : Convert.ToInt32(parameters["ProjectCategoryId"]);
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.ProjectStages.Where(item => item.IsDeleted == false && item.ProjectCategoryId == ProjectCategoryId).OrderBy(item => item.ProjectStage1))
                {
                    outputCollection.Add(item.Id, item.ProjectStage1);
                }
            }, true);
        }

        //get project stages
        private void GetProjectStagesWithoutCategoryId(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.ProjectStages.Where(item => item.IsDeleted == false).OrderBy(item => item.ProjectStage1))
                {
                    outputCollection.Add(item.Id, item.ProjectStage1, "", item.ProjectCategoryId, null);
                }
            }, true);
        }

        //Get DocumentType
        private void GetDocumentTypeForAPQP(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            int AssociatedToId = 0, apqpId = 0, DocumentTypeId = 0;
            string IsEditMode = "No";
            apqpId = string.IsNullOrEmpty(parameters["APQPItemId"]) ? 0 : Convert.ToInt32(parameters["APQPItemId"]);
            DocumentTypeId = string.IsNullOrEmpty(parameters["DocumentTypeId"]) ? 0 : Convert.ToInt32(parameters["DocumentTypeId"]);
            IsEditMode = string.IsNullOrEmpty(parameters["IsEditMode"]) ? IsEditMode : Convert.ToString(parameters["IsEditMode"]);
            bool allowConfidential = true;

            try
            {
                if (!(System.Threading.Thread.CurrentPrincipal).GetType().Equals(typeof(System.Security.Claims.ClaimsPrincipal)))
                    System.Threading.Thread.CurrentPrincipal = System.Web.HttpContext.Current.User;

                UserManagement.UserManagement userObj = new UserManagement.UserManagement();

                var currentUser = userObj.GetUserInfoById(System.Security.Claims.ClaimsPrincipal.Current.GetSubjectId());

                var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQPDashboard)).ToList();
                if (currentObjects.Count > 0)
                    allowConfidential = currentObjects[0].AllowConfidentialDocumentType;
            }
            catch (Exception ex)
            {
            }
            switch (parameters["AssociatedToId"])
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
                case "DTSTEP1":
                    AssociatedToId = (int)MES.Business.Library.Enums.DocTypeAssociatedTo.DefectTrackingPartDocument;

                    break;
                case "DTSTEP2":
                    AssociatedToId = (int)MES.Business.Library.Enums.DocTypeAssociatedTo.DefectTrackingCorrectiveAction;

                    break;
                default:
                    break;
            }
            outputCollection.KeyType = KeyTypes.String;

            this.RunOnDB(context =>
            {
                foreach (var item in context.GetDocumentTypeItemIdLookup(AssociatedToId, apqpId, DocumentTypeId, IsEditMode).ToList())
                {
                    if (allowConfidential)
                    {
                        outputCollection.Add(item.Id, item.DocumentType);
                    }
                    else
                    {
                        if (item.IsConfidential.HasValue && item.IsConfidential.Value)
                        { }
                        else
                            outputCollection.Add(item.Id, item.DocumentType);
                    }
                }
            }, true);
        }

        #region CR
        private void GetAPQPPartNoWithDesc(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.GetAPQPPartNoWithDesc(0).ToList())
                {
                    outputCollection.Add(item.Id, item.PartNumber, "", item.PartName, null);
                }
            }, true);
        }
        //Get CR DocumentTypes
        private void GetCRDocumentTypes(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            int? associatedTo_id = 0;
            bool allowConfidential = true;

            try
            {
                if (!(System.Threading.Thread.CurrentPrincipal).GetType().Equals(typeof(System.Security.Claims.ClaimsPrincipal)))
                    System.Threading.Thread.CurrentPrincipal = System.Web.HttpContext.Current.User;

                UserManagement.UserManagement userObj = new UserManagement.UserManagement();

                var currentUser = userObj.GetUserInfoById(System.Security.Claims.ClaimsPrincipal.Current.GetSubjectId());

                var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQPDashboard)).ToList();
                if (currentObjects.Count > 0)
                    allowConfidential = currentObjects[0].AllowConfidentialDocumentType;
            }
            catch (Exception ex)
            {
            }

            switch (parameters["source"])
            {
                case "CR":
                    associatedTo_id = Convert.ToInt32(DocTypeAssociatedTo.ChangeRequest);

                    break;
                default:
                    break;
            }
            outputCollection.KeyType = KeyTypes.String;

            this.RunOnDB(context =>
            {
                foreach (var item in context.GetDocumentTypeConfidentialLookup(associatedTo_id))
                {
                    if (allowConfidential)
                    {
                        outputCollection.Add(item.Id, item.DocumentType);
                    }
                    else
                    {
                        if (item.IsConfidential.HasValue && item.IsConfidential.Value)
                        { }
                        else
                            outputCollection.Add(item.Id, item.DocumentType);
                    }
                }
            }, true);
        }
        #endregion

        #region Defect Tracking
        //Get SAPItemByCustomer
        private void GetSAPItemByCustomer(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            string CustomerCode = string.IsNullOrEmpty(parameters["CustomerCode"]) ? string.Empty : Convert.ToString(parameters["CustomerCode"]);

            this.RunOnDB(context =>
            {
                foreach (var item in context.GetSAPItemByCustomer(CustomerCode, "", null))
                {
                    outputCollection.Add(item.Id, item.PartNumber, item.ItemCode, "", null);
                }
            }, true);
        }
        //Get SAPCustomers
        private void GetSAPCustomers(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;

            this.RunOnDB(context =>
            {
                foreach (var item in context.GetSAPCustomerLookUp())
                {
                    outputCollection.Add(item.CustomerCode, item.CustomerNameWithCode, "", item.CustomerName, null);
                }
            }, true);
        }
        //Get SAPCustomers only name 
        private void GetSAPCustomersName(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;

            this.RunOnDB(context =>
            {
                foreach (var item in context.GetSAPCustomerLookUp())
                {
                    outputCollection.Add(item.CustomerCode, item.CustomerName);
                }
            }, true);
        }
        //Get SAPSuppliers
        private void GetSAPSuppliers(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;

            this.RunOnDB(context =>
            {
                foreach (var item in context.GetSAPSupplierLookUp())
                {
                    outputCollection.Add(item.SupplierCode, item.SupplierNameWithCode, "", item.SupplierName, null);
                }
            }, true);
        }
        //Get SAPSuppliers Name
        private void GetSAPSuppliersName(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;

            this.RunOnDB(context =>
            {
                foreach (var item in context.GetSAPSupplierLookUp())
                {
                    outputCollection.Add(item.SupplierCode, item.SupplierName);
                }
            }, true);
        }
        //Get SupplierContacts
        private void GetSupplierContacts(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            string SupplierCode = string.IsNullOrEmpty(parameters["SupplierCode"]) ? string.Empty : Convert.ToString(parameters["SupplierCode"]);
            this.RunOnDB(context =>
            {
                foreach (var item in context.GetSCListByCode(SupplierCode))
                {
                    outputCollection.Add(item.SCName, item.SCName);
                }
            }, true);
        }
        //Get DocumentType For Defect Tracking
        private void GetDocumentTypeForDefectTracking(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            int apqpId = 0, DocumentTypeId = 0, AssociatedToId = 0;
            string IsEditMode = "No";
            apqpId = string.IsNullOrEmpty(parameters["APQPItemId"]) ? 0 : Convert.ToInt32(parameters["APQPItemId"]);
            DocumentTypeId = string.IsNullOrEmpty(parameters["DocumentTypeId"]) ? 0 : Convert.ToInt32(parameters["DocumentTypeId"]);
            IsEditMode = string.IsNullOrEmpty(parameters["IsEditMode"]) ? IsEditMode : Convert.ToString(parameters["IsEditMode"]);
            AssociatedToId = string.IsNullOrEmpty(parameters["AssociatedToId"]) ? 0 : Convert.ToInt32(parameters["AssociatedToId"]);
            bool allowConfidential = true;

            try
            {
                if (!(System.Threading.Thread.CurrentPrincipal).GetType().Equals(typeof(System.Security.Claims.ClaimsPrincipal)))
                    System.Threading.Thread.CurrentPrincipal = System.Web.HttpContext.Current.User;

                UserManagement.UserManagement userObj = new UserManagement.UserManagement();

                var currentUser = userObj.GetUserInfoById(System.Security.Claims.ClaimsPrincipal.Current.GetSubjectId());

                var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQPDashboard)).ToList();
                if (currentObjects.Count > 0)
                    allowConfidential = currentObjects[0].AllowConfidentialDocumentType;
            }
            catch (Exception ex)
            {
            }
            outputCollection.KeyType = KeyTypes.String;

            this.RunOnDB(context =>
            {
                foreach (var item in context.dtGetDocumentTypeItemForDTLookup(apqpId, DocumentTypeId, IsEditMode, AssociatedToId).ToList())
                {
                    if (allowConfidential)
                    {
                        outputCollection.Add(item.Id, item.DocumentType + " - " + item.AssociatedTo, "", item.AssociatedToId, null);
                    }
                    else
                    {
                        if (item.IsConfidential.HasValue && item.IsConfidential.Value)
                        { }
                        else
                            outputCollection.Add(item.Id, item.DocumentType + " - " + item.AssociatedTo, "", item.AssociatedToId, null);
                    }
                }
            }, true);
        }
        //Get DocumentType For Defect Tracking
        private void GetSAPSuppliersByCustomer(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            string CustomerNameWithCode = string.IsNullOrEmpty(parameters["CustomerNameWithCode"]) ? string.Empty : Convert.ToString(parameters["CustomerNameWithCode"]);

            this.RunOnDB(context =>
            {
                foreach (var item in context.capaGetSAPSupplierByCustomer(CustomerNameWithCode))
                {
                    outputCollection.Add(item.SupplierCode, item.SupplierNameWithCode, "", item.SupplierName, null);
                }
            }, true);
        }
        #endregion

        //Get Quote numbers list
        private void GetQuoteNumberList(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.Quotes.Where(item => item.IsDeleted == false && !string.IsNullOrEmpty(item.QuoteNumber)).OrderByDescending(item => item.QuoteNumber))
                {
                    outputCollection.Add(item.QuoteNumber, item.QuoteNumber);
                }
            }, true);
        }

        #region CAPA
        //Get CAPA Queries
        private void GetCAPAQueries(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.capaQueries)
                {
                    outputCollection.Add(item.Id, item.Value);
                }
            }, true);
        }

        //Get CAPA Approver Titles
        private void GetCAPAApproverTitles(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.capaApproverTitles)
                {
                    outputCollection.Add(item.Id, item.Value);
                }
            }, true);
        }

        private void GetSAPCustomersBySupplier(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            string SupplierNameWithCode = string.IsNullOrEmpty(parameters["SupplierNameWithCode"]) ? string.Empty : Convert.ToString(parameters["SupplierNameWithCode"]);

            this.RunOnDB(context =>
            {
                foreach (var item in context.capaGetSAPCustomerBySupplier(SupplierNameWithCode))
                {
                    outputCollection.Add(item.CustomerCode, item.CustomerNameWithCode, "", item.CustomerName, null);
                }
            }, true);
        }

        private void GetDefectType(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            this.RunOnDB(context =>
            {
                foreach (var item in context.DefectTypes.Where(item => item.IsDeleted == false).OrderBy(a => a.DefectType1))
                {
                    outputCollection.Add(item.Id, item.DefectType1);
                }
            }, true);
        }

        private void GetDocumentTypeForCAPA(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            int apqpId = 0, DocumentTypeId = 0, AssociatedToId = 0;
            string IsEditMode = "No";
            apqpId = string.IsNullOrEmpty(parameters["APQPItemId"]) ? 0 : Convert.ToInt32(parameters["APQPItemId"]);
            DocumentTypeId = string.IsNullOrEmpty(parameters["DocumentTypeId"]) ? 0 : Convert.ToInt32(parameters["DocumentTypeId"]);
            IsEditMode = string.IsNullOrEmpty(parameters["IsEditMode"]) ? IsEditMode : Convert.ToString(parameters["IsEditMode"]);
            AssociatedToId = string.IsNullOrEmpty(parameters["AssociatedToId"]) ? 0 : Convert.ToInt32(parameters["AssociatedToId"]);
            bool allowConfidential = true;

            try
            {
                if (!(System.Threading.Thread.CurrentPrincipal).GetType().Equals(typeof(System.Security.Claims.ClaimsPrincipal)))
                    System.Threading.Thread.CurrentPrincipal = System.Web.HttpContext.Current.User;

                UserManagement.UserManagement userObj = new UserManagement.UserManagement();

                var currentUser = userObj.GetUserInfoById(System.Security.Claims.ClaimsPrincipal.Current.GetSubjectId());

                var currentObjects = currentUser.SecurityObjects.Where(securityObject => securityObject.ObjectId == Convert.ToInt32(Pages.APQPDashboard)).ToList();
                if (currentObjects.Count > 0)
                    allowConfidential = currentObjects[0].AllowConfidentialDocumentType;
            }
            catch (Exception ex)
            {
            }
            outputCollection.KeyType = KeyTypes.String;

            this.RunOnDB(context =>
            {
                foreach (var item in context.capaGetDocumentTypeItemForCAPALookup(apqpId, DocumentTypeId, IsEditMode, AssociatedToId).ToList())
                {
                    if (allowConfidential)
                    {
                        outputCollection.Add(item.Id, item.DocumentType + " - " + item.AssociatedTo, "", item.AssociatedToId, null);
                    }
                    else
                    {
                        if (item.IsConfidential.HasValue && item.IsConfidential.Value)
                        { }
                        else
                            outputCollection.Add(item.Id, item.DocumentType + " - " + item.AssociatedTo, "", item.AssociatedToId, null);
                    }
                }
            }, true);
        }

        private void GetSAPSupplierByItemCode(LookupCollection outputCollection, string lookupName, Dictionary<string, string> parameters, short? sortOrder)
        {
            outputCollection.KeyType = KeyTypes.String;
            string PartCode = string.IsNullOrEmpty(parameters["PartCode"]) ? string.Empty : Convert.ToString(parameters["PartCode"]);

            this.RunOnDB(context =>
            {
                foreach (var item in context.capaGetSAPSupplierByItemCode(PartCode))
                {
                    outputCollection.Add(item.SupplierCode, item.SupplierNameWithCode, "", item.SupplierName, null);
                }
            }, true);
        }

        #endregion

        #endregion APQP
    }
}
