using MES.Business.Repositories.Email;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Configuration;
using System.Configuration;
using MES.Identity.Data.Library;

namespace MES.Business.Library.BO.Email
{
    public class Email : ContextBusinessBase, IEmailRepository
    {
        public Email()
            : base("Email")
        {

        }
        public void SendEmail(List<string> ToAddress, string FromAddress, string Subject, string Body, out bool isSuccess, List<Attachment> Attachments = null, List<string> CCEmail = null, List<string> BCCEmail = null)
        {
            SmtpSection smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string username = smtpSection.Network.UserName;
            string from = smtpSection.From;
            string host = smtpSection.Network.Host;
            int port = smtpSection.Network.Port;
            bool enableSsl = smtpSection.Network.EnableSsl;
            string user = smtpSection.Network.UserName;
            string password = smtpSection.Network.Password;

            MailMessage mail = new MailMessage();
            ToAddress.ForEach(addToaddress => mail.To.Add(addToaddress));
            if (CCEmail != null)
            {
                 CCEmail.ForEach(cc => mail.CC.Add(cc));
            }
            if (BCCEmail != null)
            {
                BCCEmail.ForEach(bcc => mail.Bcc.Add(bcc));
            }
            //if (string.IsNullOrEmpty(CurrentUser) || CurrentUser == "anonymous")
            //{
            //    mail.From = new MailAddress(from);
            //}
            //else
            //{
            //    if (string.IsNullOrEmpty(FromAddress))
            //    {
            //        if (GetCurrentUser.Email.Contains("mesinc.net"))
            //            mail.From = new MailAddress(GetCurrentUser.Email);
            //        else
            //            mail.From = new MailAddress(from);
            //    }
            //    else
            //        mail.From = new MailAddress(FromAddress);
            //}
            mail.From = new MailAddress(from);
            if (Attachments != null && Attachments.Count > 0)
            {
                Attachments.ForEach(attachment => mail.Attachments.Add((attachment)));
            }

            mail.Subject = Subject;
            mail.Body = Body;
            mail.IsBodyHtml = true;

            SmtpClient SmtpServer = new SmtpClient();
            SmtpServer.Host = host;
            SmtpServer.Port = port;
            SmtpServer.EnableSsl = enableSsl;
            SmtpServer.Credentials = new System.Net.NetworkCredential(user, password);
            try
            {
                SmtpServer.Send(mail);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }
            SmtpServer.Dispose();
            mail.Attachments.Clear();
            mail.Dispose();
        }

    }
}
