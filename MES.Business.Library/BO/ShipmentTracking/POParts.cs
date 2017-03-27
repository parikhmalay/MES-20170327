using Account.DTO.Library;
using MES.Business.Repositories.ShipmentTracking;
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

namespace MES.Business.Library.BO.ShipmentTracking
{
    class POParts : ContextBusinessBase, IPOPartsRepository
    {
        public POParts()
            : base("POParts")
        { }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.ShipmentTracking.POParts poparts)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.POPart();
            recordToBeUpdated.CreatedBy = recordToBeUpdated.UpdatedBy = CurrentUser;
            recordToBeUpdated.CreatedDate = AuditUtils.GetCurrentDateTime();
            recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
            this.DataContext.POParts.Add(recordToBeUpdated);

            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.ShipmentId = poparts.ShipmentId;
                recordToBeUpdated.PONumber = poparts.PONumber;
                recordToBeUpdated.PartNumber = poparts.PartNumber;
                recordToBeUpdated.PartQuantity = poparts.PartQuantity;
                recordToBeUpdated.PartDescription = poparts.PartDescription;
                this.DataContext.SaveChanges();
                poparts.Id = recordToBeUpdated.Id;
                successMsg = Languages.GetResourceText("POPartsSavedSuccess");
            }

            return SuccessOrFailedResponse<int?>(errMSg, poparts.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.ShipmentTracking.POParts> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int popartsId)
        {
            var POPartsToBeDeleted = this.DataContext.POParts.Where(a => a.Id == popartsId).SingleOrDefault();
            if (POPartsToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("POPartsNotExists"));
            }
            else
            {
                POPartsToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                POPartsToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(POPartsToBeDeleted).State = EntityState.Modified;
                //POPartsToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("POPartsDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.ShipmentTracking.POParts>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public List<DTO.Library.ShipmentTracking.POParts> GetPOPartsList(int shipmentId)
        {
            List<DTO.Library.ShipmentTracking.POParts> lstPOPart = new List<DTO.Library.ShipmentTracking.POParts>();
            DTO.Library.ShipmentTracking.POParts poparts;
            this.RunOnDB(context =>
            {
                var POPartsList = context.POParts.Where(c => c.ShipmentId == shipmentId).OrderByDescending(a => a.CreatedDate).ToList();
                if (POPartsList != null)
                {
                    foreach (var item in POPartsList)
                    {
                        poparts = new DTO.Library.ShipmentTracking.POParts();
                        poparts.Id = item.Id;
                        poparts.ShipmentId = item.ShipmentId;
                        poparts.PONumber = item.PONumber;
                        poparts.PartNumber = item.PartNumber;
                        poparts.PartQuantity = item.PartQuantity;
                        poparts.PartDescription = item.PartDescription;
                        lstPOPart.Add(poparts);
                    }
                }
            });
            return lstPOPart;
        }
    }
}
