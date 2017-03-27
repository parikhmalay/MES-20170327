using Account.DTO.Library;
using MES.Business.Repositories.Setup.Destination;
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
using MES.Business.Mapping.Extensions;

namespace MES.Business.Library.BO.Setup.Destination
{
    class Destination : ContextBusinessBase, IDestinationRepository
    {
        public Destination()
            : base("Destination")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.Setup.Destination.Destination destination)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.Destinations.AsNoTracking().Any(a => a.Destination1 == destination.destination && a.IsDeleted == false && a.Id != destination.Id))
            {
                errMSg = Languages.GetResourceText("DestinationExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.Destination();

                if (destination.Id > 0)
                {
                    recordToBeUpdated = this.DataContext.Destinations.Where(a => a.Id == destination.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("DestinationNotExists");
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
                    this.DataContext.Destinations.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.Destination1 = destination.destination;
                    recordToBeUpdated.Location = destination.Location;
                    recordToBeUpdated.IsWarehouse = destination.IsWarehouse;
                    this.DataContext.SaveChanges();
                    destination.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("DestinationSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, destination.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.Setup.Destination.Destination> FindById(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.Setup.Destination.Destination destination = new DTO.Library.Setup.Destination.Destination();
           
            var destinationItem = this.DataContext.Destinations.Where(a => a.Id == id).SingleOrDefault();
            if (destinationItem == null)
            {
                errMSg = Languages.GetResourceText("RFQNotExists");
            }
            else
            {
                #region general details
                destination = ObjectLibExtensions.AutoConvert<DTO.Library.Setup.Destination.Destination>(destinationItem);
                #endregion
            }  
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.Setup.Destination.Destination>(errMSg, destination);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int destinationId)
        {
            var destinationToBeDeleted = this.DataContext.Destinations.Where(a => a.Id == destinationId).SingleOrDefault();
            if (destinationToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("DestinationNotExists"));
            }
            else
            {
                destinationToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                destinationToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(destinationToBeDeleted).State = EntityState.Modified;
                destinationToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
               return SuccessBoolResponse(Languages.GetResourceText("DestinationDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.Destination.Destination>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.Destination.Destination>> GetDestinationsList(NPE.Core.IPage<DTO.Library.Setup.Destination.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);

            string errMSg = null;

            //declare paging variables
            //int PageNumber = paging.PageNo > 0 ? paging.PageNo - 1 : 0;
            //int PageSize = paging.PageSize > 0 ? paging.PageSize : 10;
            //int RecordStart = PageNumber * PageSize;

            List<DTO.Library.Setup.Destination.Destination> lstDestination = new List<DTO.Library.Setup.Destination.Destination>();
            DTO.Library.Setup.Destination.Destination destination;
            this.RunOnDB(context =>
             {
                 var destinationList = context.SearchDestinations(paging.Criteria.destination, paging.Criteria.Location, paging.Criteria.IsWarehouse, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (destinationList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in destinationList)
                     {
                         destination = new DTO.Library.Setup.Destination.Destination();
                         destination.Id = item.Id;
                         destination.destination = item.Destination;
                         destination.Location = item.Location;
                         destination.IsWarehouse = item.IsWarehouse;
                         lstDestination.Add(destination);
                     }
                 }
             });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.Destination.Destination>>(errMSg, lstDestination);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }
    }
}
