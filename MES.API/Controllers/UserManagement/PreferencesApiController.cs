using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.UserManagement;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.UserManagement
{
    [AdminPrefix("PreferencesApi")]
    public class PreferencesApiController : SecuredApiControllerBase
    {
        [Inject]
        public IPreferencesRepository PreferencesRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.UserManagement.Preferences> Get(string userId)
        {
            var type = this.Resolve<IPreferencesRepository>(PreferencesRepository).FindById(userId);
            return type;
        }

        /// <summary>
        /// save the Preferences data.
        /// </summary>
        /// <param name="Preferences"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.UserManagement.Preferences preferences)
        {
            var type = this.Resolve<IPreferencesRepository>(PreferencesRepository).Save(preferences);
            return type;
        }

        /// <summary>
        /// delete the Preferences data.
        /// </summary>
        /// <param name="PreferencesId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int preferencesId)
        {
            var type = this.Resolve<IPreferencesRepository>(PreferencesRepository).Delete(preferencesId);
            return type;
        }
        #endregion Methods
    }
}