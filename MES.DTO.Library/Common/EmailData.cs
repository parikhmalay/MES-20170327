using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.Common
{
    public class EmailData
    {
        public string[] Ids { get; set; }
        public string[] EmailIdsList { get; set; }
        public string BCCEmailIds { get; set; }
        public string CCEmailIds { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string EmailBodyRFQCloseout { get; set; }
        public string EmailAttachment { get; set; }
        public string EmailFileName { get; set; }
        public List<EmailDocumentAttachment> lstEmailDocumentAttachment = new List<EmailDocumentAttachment>();
        public int? EmailTypeId { get; set; }
        public bool? AttachRFQPDF { get; set; }
        public bool? AttachDocument { get; set; }
        public string RFQId { get; set; }
        public string APQPItemIds { get; set; }
        public string UserIds { get; set; }
        public string ToEmailIds { get; set; }
        public MES.DTO.Library.APQP.CAPA.CAPA objCAPA { get; set; }
    }
    public class EmailDocumentAttachment
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
