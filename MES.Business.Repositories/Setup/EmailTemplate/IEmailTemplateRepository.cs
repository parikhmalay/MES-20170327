using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.Setup.EmailTemplate
{
    public interface IEmailTemplateRepository : ICrudMethods<MES.DTO.Library.Setup.EmailTemplate.EmailTemplate, int?, string,
          MES.DTO.Library.Setup.EmailTemplate.EmailTemplate, int, bool?, int, MES.DTO.Library.Setup.EmailTemplate.EmailTemplate>
    {
        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.EmailTemplate.EmailTemplate>> GetEmailTemplateList(NPE.Core.IPage<MES.DTO.Library.Setup.EmailTemplate.SearchCriteria> paging);
        ITypedResponse<bool?> SendEmail(MES.DTO.Library.Setup.EmailTemplate.EmailTemplate emailTemplate);
        ITypedResponse<MES.DTO.Library.Setup.EmailTemplate.EmailTemplate> FindByShortCode(string sc);
    }
}
