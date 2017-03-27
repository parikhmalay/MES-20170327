using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.AuditLogs
{
    public class AuditLogs
    {
        public int AuditLogId { get; set; }
        public Nullable<int> ReferenceId { get; set; }
        /// <summary>
        /// this is transaction's primary key.
        /// </summary>
        public Nullable<int> TablePrimaryKey { get; set; }
        /// <summary>
        /// this is Schema name.
        /// </summary>
        public string Source { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedOnString { get; set; }
        public Nullable<System.DateTime> TimeStamp { get; set; }
        public string TimeStampString { get; set; }
        public string UpdatedBy { get; set; }
        public int DetailId { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public string FieldAlias { get; set; }
        public Nullable<bool> IsPricingField { get; set; }
        public string OperationType { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string SourceFrom { get; set; }
        public string PartNumber { get; set; }

        public List<DTO.Library.APQP.ChangeRequest.Document> lstDocument { get; set; }
        public bool IsContainsDocument { get; set; }
    }
    public class SearchCriteria
    {
        public int ItemId { get; set; }
        public string tableName { get; set; }
        public string schemaName { get; set; }
    }
}

