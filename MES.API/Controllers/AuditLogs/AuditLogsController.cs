using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.AuditLogs;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.AuditLogs
{
    [AdminPrefix("AuditLogsApi")]
    public class AuditLogsController : SecuredApiControllerBase
    {
        [Inject]
        public IAuditLogsRepository AuditLogsRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get audit log details.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetAuditLogs")]
        public ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAuditLogs(GenericPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging)
        {
            var type = this.Resolve<IAuditLogsRepository>(AuditLogsRepository).GetAuditLogDetails(paging);
            return type;
        }

        #region Defect Tracking
        [HttpPostRoute("GetAuditLogDefectTracking")]
        public ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAuditLogDefectTracking(GenericPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging)
        {
            var type = this.Resolve<IAuditLogsRepository>(AuditLogsRepository).GetAuditLogDefectTracking(paging);
            return type;
        }
        [HttpPostRoute("GetAuditLogDefectTrackingDetails")]
        public ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAuditLogDefectTrackingDetails(GenericPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging)
        {
            var type = this.Resolve<IAuditLogsRepository>(AuditLogsRepository).GetAuditLogDefectTrackingDetails(paging);
            return type;
        }

        [HttpPostRoute("GetAPQPChangeLogs")]
        public ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAPQPChangeLogs(GenericPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging)
        {
            var type = this.Resolve<IAuditLogsRepository>(AuditLogsRepository).GetAPQPChangeLogs(paging);
            return type;
        }
        [HttpPostRoute("GetChangeRequestLogs")]
        public ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetChangeRequestLogs(GenericPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging)
        {
            var type = this.Resolve<IAuditLogsRepository>(AuditLogsRepository).GetChangeRequestLogs(paging);
            return type;
        }
        #endregion

        #region CAPA
        [HttpPostRoute("GetAuditLogCAPA")]
        public ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAuditLogCAPA(GenericPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging)
        {
            var type = this.Resolve<IAuditLogsRepository>(AuditLogsRepository).GetAuditLogCAPA(paging);
            return type;
        }
        [HttpPostRoute("GetAuditLogCAPAAffectedPartDetails")]
        public ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAuditLogCAPAAffectedPartDetails(GenericPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging)
        {
            var type = this.Resolve<IAuditLogsRepository>(AuditLogsRepository).GetAuditLogCAPAAffectedPartDetails(paging);
            return type;
        }
        #endregion

        #endregion
    }
}