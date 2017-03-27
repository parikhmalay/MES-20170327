using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.APQP.ChangeRequest
{
    public interface IChangeRequestRepository : ICrudMethods<MES.DTO.Library.APQP.ChangeRequest.ChangeRequest, int?, string,
          MES.DTO.Library.APQP.ChangeRequest.ChangeRequest, int, bool?, int, MES.DTO.Library.APQP.ChangeRequest.ChangeRequest>
    {
        ITypedResponse<List<MES.DTO.Library.APQP.ChangeRequest.ChangeRequest>> GetChangeRequestList(NPE.Core.IPage<MES.DTO.Library.APQP.ChangeRequest.SearchCriteria> searchCriteria);
        #region SAP Records
        ITypedResponse<List<MES.DTO.Library.APQP.APQP.SAPItem>> SearchFromSAPRecords(NPE.Core.IPage<MES.DTO.Library.APQP.ChangeRequest.SearchCriteria> searchCriteria);
        ITypedResponse<int?> InsertFromSAPRecords(string ItemIds);
        ITypedResponse<int?> GetFromSAPAndInsertInLocalSAPTable();
        #endregion
        #region
        ITypedResponse<DTO.Library.APQP.ChangeRequest.ChangeRequest> AddToCRFromAPQP(int apqpItemId);
        ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> crHistoryChangeLog(int crItemId);
        ITypedResponse<DTO.Library.APQP.ChangeRequest.ChangeRequest> OnChangeOfPartNumber(int apqpItemId);
        ITypedResponse<int?> AddToAPQP(MES.DTO.Library.APQP.ChangeRequest.ChangeRequest changerequest);
        ITypedResponse<bool?> DeleteDocument(int documentId);
        #endregion
    }
}
