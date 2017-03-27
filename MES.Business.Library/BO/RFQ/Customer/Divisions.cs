using Account.DTO.Library;
using MES.Business.Repositories.RFQ.Customer;
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

namespace MES.Business.Library.BO.RFQ.Customer
{
    class Divisions : ContextBusinessBase, IDivisionsRepository
    {
        public Divisions()
            : base("Divisions")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.Customer.Divisions divisions)
        {
            string errMSg = null;
            string successMsg = null;

            var recordToBeUpdated = new MES.Data.Library.Division();
            if (divisions.Id > 0)
            {
                recordToBeUpdated = this.DataContext.Divisions.Where(a => a.Id == divisions.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("DivisionsNotExists");
                else
                {
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
                this.DataContext.Divisions.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.CustomerId = divisions.CustomerId;
                recordToBeUpdated.CustomerCode = divisions.CustomerCode;
                recordToBeUpdated.CompanyName = divisions.CompanyName;
                recordToBeUpdated.Address1 = divisions.Address1;
                recordToBeUpdated.Address2 = divisions.Address2;
                recordToBeUpdated.City = divisions.City;
                recordToBeUpdated.State = divisions.State;
                recordToBeUpdated.CountryId = divisions.CountryId;
                recordToBeUpdated.Zip = divisions.Zip;
                recordToBeUpdated.Website = divisions.Website;
                recordToBeUpdated.CompanyPhone1 = divisions.CompanyPhone1;
                recordToBeUpdated.CompanyPhone2 = divisions.CompanyPhone2;
                recordToBeUpdated.CompanyFax = divisions.CompanyFax;
                recordToBeUpdated.Comments = divisions.Comments;
                recordToBeUpdated.PaymentRating = divisions.PaymentRating;
                recordToBeUpdated.SAMId = divisions.SAMId;
                this.DataContext.SaveChanges();
                divisions.Id = recordToBeUpdated.Id;
                successMsg = Languages.GetResourceText("DivisionsSavedSuccess");
            }

            return SuccessOrFailedResponse<int?>(errMSg, divisions.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Customer.Divisions> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int divisionsId)
        {
            var DivisionsToBeDeleted = this.DataContext.Divisions.Where(a => a.Id == divisionsId).SingleOrDefault();
            if (DivisionsToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("DivisionsNotExists"));
            }
            else
            {
                DivisionsToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                DivisionsToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(DivisionsToBeDeleted).State = EntityState.Modified;
                DivisionsToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("DivisionsDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<bool?> DeleteMultiple(string DivisionIds)
        {
            //set the out put param
            ObjectParameter Result = new ObjectParameter("Result", 0);
            this.RunOnDB(context =>
            {
                context.DeleteMultipleCustomerDivision(DivisionIds, CurrentUser, Result);
            });
            if (Convert.ToInt32(Result.Value) > 0)
                return SuccessBoolResponse(Languages.GetResourceText("DivisionsDeletedSuccess"));
            else
                return FailedBoolResponse(Languages.GetResourceText("DivisionDeleteFail"));
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.Customer.Divisions>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public List<DTO.Library.RFQ.Customer.Divisions> GetDivisionsList(int customerId)
        {
            List<DTO.Library.RFQ.Customer.Divisions> lstDivision = new List<DTO.Library.RFQ.Customer.Divisions>();
            DTO.Library.RFQ.Customer.Divisions divisions;
            this.RunOnDB(context =>
            {
                var DivisionsList = context.Divisions.Where(c => c.CustomerId == customerId && c.IsDeleted == false).OrderByDescending(a => a.CreatedDate).ToList();
                if (DivisionsList != null)
                {
                    foreach (var item in DivisionsList)
                    {
                        divisions = new DTO.Library.RFQ.Customer.Divisions();
                        divisions.Id = item.Id;
                        divisions.CustomerCode = item.CustomerCode;
                        divisions.CompanyName = item.CompanyName;
                        divisions.Address1 = item.Address1;
                        divisions.Address2 = item.Address2;
                        divisions.City = item.City;
                        divisions.State = item.State;
                        divisions.CountryId = item.CountryId;
                        divisions.Zip = item.Zip;
                        divisions.Website = item.Website;
                        divisions.CompanyPhone1 = item.CompanyPhone1;
                        divisions.CompanyPhone2 = item.CompanyPhone2;
                        divisions.CompanyFax = item.CompanyFax;
                        divisions.Comments = item.Comments;
                        divisions.PaymentRating = item.PaymentRating;
                        divisions.SAMId = item.SAMId;
                        divisions.SAM = new DTO.Library.Common.LookupFields
                        {
                            Id = item.SAMId,
                            Name = "" //item.Prefix.Value
                        };
                        lstDivision.Add(divisions);
                    }
                }
            });
            return lstDivision;
        }
    }
}
