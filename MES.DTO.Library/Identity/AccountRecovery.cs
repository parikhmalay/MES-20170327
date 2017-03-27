using ExpressiveAnnotations.Attributes;
using MES.DTO.Library.Base.Messaging.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.Identity
{
    public class AccountRecovery
    {
        [RequiredIf("RecoveryType == AccountRecoveryType.UserName", ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "EmailRequired"), StringLength(50)]
        [Display(ResourceType = typeof(DTOMessageResources), Name = "ForgotUserName")]
        public string Email { get; set; }

        [RequiredIf("RecoveryType == AccountRecoveryType.Password", ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "UserNameRequired"), StringLength(50)]
        [Display(ResourceType = typeof(DTOMessageResources), Name = "ForgotPassword")]
        public string UserName { get; set; }

        public AccountRecoveryType RecoveryType { get; set; }

        public string SigningToken { get; set; }
    }

    public enum AccountRecoveryType
    {
        Password,
        UserName
    }
}
