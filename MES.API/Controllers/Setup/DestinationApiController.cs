using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.Destination;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("DestinationApi")]
    public class DestinationApiController : SecuredApiControllerBase
    {
        [Inject]
        public IDestinationRepository DestinationRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.Destination.Destination> Get(int Id)
        {
            var type = this.Resolve<IDestinationRepository>(DestinationRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the destination data.
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.Destination.Destination destination)
        {
            var type = this.Resolve<IDestinationRepository>(DestinationRepository).Save(destination);
            return type;
        }

        /// <summary>
        /// delete the destination data.
        /// </summary>
        /// <param name="destinationId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int destinationId)
        {
            var type = this.Resolve<IDestinationRepository>(DestinationRepository).Delete(destinationId);
            return type;
        }
        /// <summary>
        /// Get Destinations list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetDestinations")]
        public ITypedResponse<List<MES.DTO.Library.Setup.Destination.Destination>> GetDestinations(GenericPage<string> page)
        {
            var type = this.Resolve<IDestinationRepository>(DestinationRepository).Search(page);
            return type;
        }

        /// <summary>
        /// Get Destinations list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetDestinationsList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.Destination.Destination>> GetDestinationsList(GenericPage<MES.DTO.Library.Setup.Destination.SearchCriteria> paging)
        {
            var type = this.Resolve<IDestinationRepository>(DestinationRepository).GetDestinationsList(paging);
            return type;
        }

        #endregion Methods
    }
}