using MES.DTO.Library.Base.Messaging.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Core;
using MES.DTO.Library.UserManagement;
using MES.DTO.Library.RoleManagement;
using MES.DTO.Library.Common;
namespace MES.DTO.Library.Identity
{
    public class LoginUser 
    {
        public string UserId { get; set; }
        [MaxLength(10)]
        public string Culture { get; set; }
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string FullName { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string UserRole { get; set; }
        public bool? Active { get; set; }
        public bool? IsRFQCoordinator { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public short? CountryId { get; set; }
        public string ZipCode { get; set; }
        public short? PrefixId { get; set; }
        public short? GenderId { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public int? SupplierId { get; set; }
        public bool IsDeleted { get; set; }
        public string DefaultController { get; set; }
        public string DefaultAction { get; set; }
        public int? DefaultObjectId { get; set; }
        public List<UserDesignationMappings> lstUserDesignation { get; set; }
        public List<LoginUser> lstAssignedTo { get; set; }
        public Preferences Preferences { get; set; }
        public List<RoleObjectPrivilege> SecurityObjects { get; set; }
    }

    public class SearchCriteria
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? Active { get; set; }
        public int? RoleId { get; set; }

        //public bool isFirstTimeLoad { get; set; }
        public string rfqCoordinator { get; set; }
        public List<ItemList> SAMItems { get; set; }
        public string SAMUserId { get; set; }
        public string APQPQualityEngineerId { get; set; }
        public string SupplyChainCoordinatorId { get; set; }

        public List<ItemList> AssignedToItems { get; set; }
    }
}

