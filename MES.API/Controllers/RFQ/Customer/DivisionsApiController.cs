using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.RFQ.Customer;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.RFQ.Customer
{
    [AdminPrefix("DivisionsApi")]
    public class DivisionsApiController : SecuredApiControllerBase
    {
        [Inject]
        public IDivisionsRepository DivisionsRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.RFQ.Customer.Divisions> Get(int Id)
        {
            var type = this.Resolve<IDivisionsRepository>(DivisionsRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the division data.
        /// </summary>
        /// <param name="Divisions"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.RFQ.Customer.Divisions divisions)
        {
            var type = this.Resolve<IDivisionsRepository>(DivisionsRepository).Save(divisions);
            return type;
        }

        /// <summary>
        /// delete division.
        /// </summary>
        /// <param name="DivisionsId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int divisionId)
        {
            var type = this.Resolve<IDivisionsRepository>(DivisionsRepository).Delete(divisionId);
            return type;
        }
        /// <summary>
        /// delete multiple division.
        /// </summary>
        /// <param name="DivisionsId"></param>
        /// <returns></returns>
        [HttpPostRoute("DeleteMultiple")]
        public ITypedResponse<bool?> DeleteMultiple(string DivisionIds)
        {
            var type = this.Resolve<IDivisionsRepository>(DivisionsRepository).DeleteMultiple(DivisionIds);
            return type;
        }
        /// <summary>
        /// Get Divisions list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetDivisionsList")]
        public List<MES.DTO.Library.RFQ.Customer.Divisions> GetDivisionsList(int supplierId)
        {
            var type = this.Resolve<IDivisionsRepository>(DivisionsRepository).GetDivisionsList(supplierId);
            return type;
        }
        #endregion Methods
    }
}