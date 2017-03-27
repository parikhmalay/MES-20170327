using System;
using System.Collections.Generic;

namespace MES.Business.Repositories.AuditLogs
{
    public interface IAuditLogsRepository
    {
        NPE.Core.ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAuditLogDetails(NPE.Core.IPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging);

        #region Defect Tracking
        NPE.Core.ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAuditLogDefectTracking(NPE.Core.IPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging);
        NPE.Core.ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAuditLogDefectTrackingDetails(NPE.Core.IPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging);
        #endregion

        #region APQP change logs
        NPE.Core.ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAPQPChangeLogs(NPE.Core.IPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging);
        NPE.Core.ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetChangeRequestLogs(NPE.Core.IPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging);
        #endregion
        #region CAPA Change log
        NPE.Core.ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAuditLogCAPA(NPE.Core.IPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging);
        NPE.Core.ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetAuditLogCAPAAffectedPartDetails(NPE.Core.IPage<MES.DTO.Library.AuditLogs.SearchCriteria> paging); 
        #endregion
    }
}

