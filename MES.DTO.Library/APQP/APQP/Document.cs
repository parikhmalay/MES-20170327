using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.APQP
{
    public class Document : CreateEditPropBase
    {
        public int Id { get; set; }
        public int APQPItemId { get; set; }
        public Nullable<int> DocumentTypeId { get; set; }
        public string DocumentType { get; set; }
        public bool IsConfidential { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public Nullable<System.DateTime> PreparedDate { get; set; }
        public string FileTitle { get; set; }
        public string FilePath { get; set; }
        public string Comments { get; set; }
        public string RevLevel { get; set; }
        public Nullable<int> crId { get; set; }
        public Nullable<bool> IsAddPSW { get; set; }
        public Nullable<int> UploadedFromStepId { get; set; }
        public string SectionName { get; set; }

        public string APQPItemIds { get; set; }
        public List<APQPSupplierDetails> lstAPQPSupplierDetails { get; set; }
    }
    public class APQPSupplierDetails
    {
        public Nullable<int> SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SupplierLocation { get; set; }
        public int APQPItemId { get; set; }
        public bool chkSelect { get; set; }
    }
    public class SearchDocument
    {
        public PSWItem objPSWItem { get; set; }
        public List<Document> lstDocument { get; set; }
    }
}
