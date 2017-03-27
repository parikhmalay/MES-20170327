using System;
using System.Collections.Generic;
using System.Net.Mail;


namespace MES.Business.Repositories.Email
{
    public interface IEmailRepository
    {
        void SendEmail(List<string> ToAddress, string FromAddress, string Subject, string Body, out bool IsSuccess, List<Attachment> Attachments = null, List<string> CCEmail = null, List<string> BCCEmail = null);
    }
}
