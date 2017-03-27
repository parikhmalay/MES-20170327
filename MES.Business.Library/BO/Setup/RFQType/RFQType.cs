using Account.DTO.Library;
using MES.Business.Repositories.Setup.RFQType;
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


namespace MES.Business.Library.BO.Setup.RFQType
{
    class RFQType : ContextBusinessBase, IRFQTypeRepository
    {
        public RFQType() : base("RFQType") { }


        public ITypedResponse<bool?> Delete(int id)
        {
            var rfqtypeToBeDeleted = this.DataContext.RFQTypes.Where(a => a.Id == id).SingleOrDefault();

            if (rfqtypeToBeDeleted == null)
                return FailedBoolResponse(Languages.GetResourceText("RFQTypeNotExists"));
            else
            {
                rfqtypeToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                rfqtypeToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(rfqtypeToBeDeleted).State = EntityState.Modified;

                rfqtypeToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("RFQTypeDeletedSuccess"));
            }
        }

        public ITypedResponse<DTO.Library.Setup.RFQType.RFQType> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public ITypedResponse<int?> Save(DTO.Library.Setup.RFQType.RFQType rfqType)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.RFQTypes.AsNoTracking().Any(a => a.RFQTypeName == rfqType.rfqType && a.IsDeleted == false && a.Id != rfqType.Id))
            {
                errMSg = Languages.GetResourceText("RFQTypeExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.RFQType();

                if (rfqType.HasId())
                {
                    recordToBeUpdated = this.DataContext.RFQTypes.Where(a => a.Id == rfqType.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("RFQTypeNotExists");
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
                    this.DataContext.RFQTypes.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.RFQTypeName = rfqType.rfqType;
                    this.DataContext.SaveChanges();
                    rfqType.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("RFQTypeSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, rfqType.Id, successMsg);
        }

        public ITypedResponse<List<DTO.Library.Setup.RFQType.RFQType>> GetRFQTypes(IPage<DTO.Library.Setup.RFQType.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);


            string errMSg = null;

            //declare paging variables
            /*int PageNumber = paging.PageNo > 0 ? paging.PageNo - 1 : 0;
            int PageSize = paging.PageSize > 0 ? paging.PageSize : 10;
            int RecordStart = PageNumber * PageSize;*/

            List<DTO.Library.Setup.RFQType.RFQType> rfqTypes = new List<DTO.Library.Setup.RFQType.RFQType>();
            DTO.Library.Setup.RFQType.RFQType rfqType;
            this.RunOnDB(context =>
            {
                var rfqTypeList = context.SearchRFQTypes(paging.Criteria.rfqType, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (rfqTypeList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in rfqTypeList)
                    {
                        rfqType = new DTO.Library.Setup.RFQType.RFQType();
                        rfqType.Id = item.Id;
                        rfqType.rfqType = item.RFQTypename;
                        rfqTypes.Add(rfqType);
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.RFQType.RFQType>>(errMSg, rfqTypes);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }


        public ITypedResponse<List<DTO.Library.Setup.RFQType.RFQType>> Search(IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

    }
}
