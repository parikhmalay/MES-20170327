using Account.DTO.Library;
using MES.Business.Repositories.Setup.EmailTemplate;
using NPE.Core;
using NPE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Core.Extensions;
using System.Data.Entity.Core.Objects;
using System.Net.Mail;
using System.Net.Configuration;
using System.Configuration;
using System.Data.Entity.Validation;

namespace MES.Business.Library.BO.Setup.EmailTemplate
{
    class EmailTemplate : ContextBusinessBase, IEmailTemplateRepository
    {
        public EmailTemplate()
            : base("EmailTemplate")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.Setup.EmailTemplate.EmailTemplate emailTemplate)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.EmailTemplates.AsNoTracking().Any(a => a.Title == emailTemplate.Title && a.IsDeleted == false && a.Id != emailTemplate.Id))
            {
                errMSg = Languages.GetResourceText("TitleExists");
            }
            else
            {
                try
                {
                    var recordToBeUpdated = new MES.Data.Library.EmailTemplate();

                    if (emailTemplate.Id > 0)
                    {
                        recordToBeUpdated = this.DataContext.EmailTemplates.Where(a => a.Id == emailTemplate.Id).SingleOrDefault();

                        if (recordToBeUpdated == null)
                            errMSg = Languages.GetResourceText("EmailTemplateNotExists");
                        else
                        {
                            recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                            recordToBeUpdated.UpdatedBy = CurrentUser;
                            this.DataContext.Entry(recordToBeUpdated).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        recordToBeUpdated.CreatedBy = recordToBeUpdated.UpdatedBy = CurrentUser;
                        recordToBeUpdated.CreatedDate = AuditUtils.GetCurrentDateTime();
                        recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                        this.DataContext.EmailTemplates.Add(recordToBeUpdated);
                    }
                    if (string.IsNullOrEmpty(errMSg))
                    {
                        recordToBeUpdated.Title = emailTemplate.Title;
                        recordToBeUpdated.EmailSubject = emailTemplate.EmailSubject;
                        recordToBeUpdated.EmailBody = emailTemplate.EmailBody;
                        recordToBeUpdated.ShortCode = emailTemplate.Title.Replace(" ", "");
                        recordToBeUpdated.Description = emailTemplate.Description;
                        this.DataContext.SaveChanges();
                        emailTemplate.Id = recordToBeUpdated.Id;
                        successMsg = Languages.GetResourceText("EmailTemplateSavedSuccess");
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            throw ex;// ("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }

            }
            return SuccessOrFailedResponse<int?>(errMSg, emailTemplate.Id, successMsg);
        }
        public NPE.Core.ITypedResponse<DTO.Library.Setup.EmailTemplate.EmailTemplate> FindByShortCode(string sc)
        {

            string errMSg = string.Empty;
            DTO.Library.Setup.EmailTemplate.EmailTemplate et = new DTO.Library.Setup.EmailTemplate.EmailTemplate();
            this.RunOnDB(context =>
            {
                var etItem = context.EmailTemplates.Where(r => r.ShortCode == sc && r.IsDeleted == false).SingleOrDefault();
                if (etItem == null)
                    errMSg = Languages.GetResourceText("EmailTemplateNotExists");
                else
                {
                    #region general details
                    et.Id = etItem.Id;
                    et.ShortCode = etItem.ShortCode;
                    et.EmailBody = etItem.EmailBody;
                    et.EmailSubject = etItem.EmailSubject;
                    et.Description = etItem.Description;
                    #endregion
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.Setup.EmailTemplate.EmailTemplate>(errMSg, et);
            return response;
        }


        public NPE.Core.ITypedResponse<DTO.Library.Setup.EmailTemplate.EmailTemplate> FindById(int id)
        {

            string errMSg = string.Empty;
            DTO.Library.Setup.EmailTemplate.EmailTemplate et = new DTO.Library.Setup.EmailTemplate.EmailTemplate();
            this.RunOnDB(context =>
            {
                var etItem = context.EmailTemplates.Where(r => r.Id == id && r.IsDeleted == false).SingleOrDefault();
                if (etItem == null)
                    errMSg = Languages.GetResourceText("EmailTemplateNotExists");
                else
                {
                    #region general details
                    et.Id = etItem.Id;
                    et.ShortCode = etItem.ShortCode;
                    et.EmailBody = etItem.EmailBody;
                    et.EmailSubject = etItem.EmailSubject;
                    et.Description = etItem.Description;
                    #endregion
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.Setup.EmailTemplate.EmailTemplate>(errMSg, et);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int emailTemplateId)
        {
            var EmailTemplateToBeDeleted = this.DataContext.EmailTemplates.Where(a => a.Id == emailTemplateId).SingleOrDefault();
            if (EmailTemplateToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("EmailTemplateNotExists"));
            }
            else
            {
                EmailTemplateToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                EmailTemplateToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(EmailTemplateToBeDeleted).State = EntityState.Modified;
                EmailTemplateToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("EmailTemplateDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.EmailTemplate.EmailTemplate>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.EmailTemplate.EmailTemplate>> GetEmailTemplateList(NPE.Core.IPage<DTO.Library.Setup.EmailTemplate.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.Setup.EmailTemplate.EmailTemplate> lstEmailTemplate = new List<DTO.Library.Setup.EmailTemplate.EmailTemplate>();
            DTO.Library.Setup.EmailTemplate.EmailTemplate emailTemplate;
            this.RunOnDB(context =>
             {
                 var EmailTemplateList = context.SearchEmailTemplate(paging.Criteria.Title, paging.Criteria.EmailSubject, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (EmailTemplateList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in EmailTemplateList)
                     {
                         emailTemplate = new DTO.Library.Setup.EmailTemplate.EmailTemplate();
                         emailTemplate.Id = item.Id;
                         emailTemplate.Title = item.Title;
                         emailTemplate.EmailSubject = item.EmailSubject;
                         emailTemplate.EmailBody = item.EmailBody;
                         emailTemplate.ShortCode = item.ShortCode;
                         emailTemplate.Description = item.Description;
                         lstEmailTemplate.Add(emailTemplate);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.EmailTemplate.EmailTemplate>>(errMSg, lstEmailTemplate);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> SendEmail(DTO.Library.Setup.EmailTemplate.EmailTemplate emailTemplate)
        {
            bool IsSuccess = false;
            try
            {
                MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
                List<string> lstToAddress = new List<string>();
                lstToAddress.Add(emailTemplate.TestEmailAddress);
                emailRepository.SendEmail(lstToAddress, "", emailTemplate.EmailSubject, emailTemplate.EmailBody, out IsSuccess, null);
            }
            catch (Exception ex)
            {
            }
            if (IsSuccess)
                return SuccessBoolResponse(Languages.GetResourceText("SendEmailSuccess"));
            else
                return SuccessBoolResponse(Languages.GetResourceText("SendEmailFail"));
        }
    }
}
