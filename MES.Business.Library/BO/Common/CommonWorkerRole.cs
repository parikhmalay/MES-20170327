using DocuSign.eSign.Model;
using MES.Business.Library.Enums;
using MES.Business.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Core;
using NPE.Core.Helpers;
namespace MES.Business.Library.BO.Common
{
    public class CommonWorkerRole : ContextBusinessBase, ICommonWorkerRoleRepository
    {
        public CommonWorkerRole()
            : base("CommonWorkerRole")
        { }
        public void ProcessDaily()
        {
            try
            {
                DocuSignStatusUpdate();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DocuSignStatusUpdate()
        {
            DocuSign.DocuSign prg = new DocuSign.DocuSign();

            try
            {
                this.RunOnDB(context =>
                {
                    var recordToBeUpdated = context.NPIFDocusigns.Where(item => item.Status.ToLower() == DocuSignEnvelopeStatus.sent.ToString()).ToList();
                    if (recordToBeUpdated != null)
                    {
                        foreach (MES.Data.Library.NPIFDocusign item in recordToBeUpdated)
                        {
                            if (!string.IsNullOrEmpty(item.EnvelopeId))
                            {
                                Envelope envDetails = prg.CheckStatusOfDocuSign(item);

                                string partNumber = string.Empty;
                                APQP.APQP.KickOff apqpObj = new APQP.APQP.KickOff();
                                var kickOffItem = apqpObj.FindById(item.APQPItemId.Value).Result;
                                if (kickOffItem != null && !string.IsNullOrEmpty(kickOffItem.PartNumber))
                                    partNumber = kickOffItem.PartNumber;

                                item.Status = envDetails.Status;
                                item.DateOfStatus = (envDetails.SentDateTime != null) ? Convert.ToDateTime(envDetails.SentDateTime) : (DateTime?)null;
                                item.SignedDocumentPath = string.Empty;

                                if (envDetails.Status.ToLower() == DocuSignEnvelopeStatus.completed.ToString())
                                {
                                    item.SignedDocumentPath = prg.GetDocuSignDocument(item.EnvelopeId, item.DocumentId, partNumber, Constants.NPIFDocusign);
                                    item.DateOfStatus = envDetails.CompletedDateTime != null ? Convert.ToDateTime(envDetails.CompletedDateTime) : (DateTime?)null;
                                }
                                else if (envDetails.Status.ToLower() == DocuSignEnvelopeStatus.voided.ToString())
                                {
                                    item.VoidedReason = envDetails.VoidedReason;
                                    item.DateOfStatus = envDetails.VoidedDateTime != null ? Convert.ToDateTime(envDetails.VoidedDateTime) : (DateTime?)null;
                                }
                                else if (envDetails.Status.ToLower() == DocuSignEnvelopeStatus.declined.ToString())
                                {
                                    item.DateOfStatus = envDetails.DeclinedDateTime != null ? Convert.ToDateTime(envDetails.DeclinedDateTime) : (DateTime?)null;
                                }

                                if (!string.IsNullOrEmpty(envDetails.DeletedDateTime))
                                {
                                    item.DateOfStatus = envDetails.DeletedDateTime != null ? Convert.ToDateTime(envDetails.DeletedDateTime) : (DateTime?)null;
                                    envDetails.Status = DocuSignEnvelopeStatus.deleted.ToString();
                                }

                                context.Entry(item).State = EntityState.Modified;
                                context.SaveChanges();
                            }
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        private void SendEmailTest()
        {
            //this.LogDebug<CommonWorkerRole>("Start Scheduler -  Processing Email Send");

            /*MES.DTO.Library.Common.EmailData emailData = null;
            bool IsSuccess = true;
            try
            {
                emailData = new DTO.Library.Common.EmailData();
                MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
                emailData.EmailBody = "this is test email from service";

                List<string> lstToAddress = new List<string>();
                lstToAddress.Add("roma.patel@almikatech.com");

                emailData.EmailSubject = "TEST EMAIL FROM SERVICE";
                emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailData.EmailBody, out IsSuccess, null, null, null);
            }
            catch (Exception)
            {
                throw;
            }*/
        }

    }
}
