namespace MES.Identity.Data.Library
{
    /*
     * Copyright 2014 Dominick Baier, Brock Allen
     *
     * Licensed under the Apache License, Version 2.0 (the "License");
     * you may not use this file except in compliance with the License.
     * You may obtain a copy of the License at
     *
     *   http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Configuration;
    using System.Linq;
    using System.Security.Claims;
    using System.Web;

    public class User : IdentityUser
    {
        public string DisplayName { get; set; }

        [MaxLength(10)]
        [Column(TypeName = "nvarchar")]
        public string Culture { get; set; }

        [Column(TypeName = "smallint")]
        public short? TitleId { get; set; }

        [Column(TypeName = "smallint")]
        public short GenderId { get; set; }

        [StringLength(50)]
        [Required]
        [Column(TypeName = "nvarchar")]
        public string FirstName { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(20)]
        public string MiddleName { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [Required]
        public string LastName { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(255)]
        [Required]
        public string Email { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string AddressLine1 { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string AddressLine2 { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string City { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string State { get; set; }

        [Column(TypeName = "smallint")]
        public short CountryId { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(15)]
        public string ZipCode { get; set; }

        [Column(TypeName = "int")]
        [Required]
        public int? RoleId { get; set; }

        [Column(TypeName = "bit")]
        [DefaultValue(true)]
        public bool Active { get; set; }

        [Column(TypeName = "int")]
        public int? SupplierId { get; set; }

        [Column(TypeName = "bit")]
        [DefaultValue(false)]
        public bool IsRFQCoordinator { get; set; }

        [Column(TypeName = "date")]
        public DateTime NextSystemMessageDisplayDate { get; set; }

        [Column(TypeName = "nvarchar")]
        [Required]
        [StringLength(128)]
        public string CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        [Required]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }

        [Column(TypeName = "bit")]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }

    public class Role : IdentityRole { }

    public class IdentityContext : IdentityDbContext<User, Role, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public IdentityContext()
            : base(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString)
        {
        }

        public IdentityContext(string connString)
            : base(connString)
        {
        }
    }

    public class UserStore : UserStore<User, Role, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public UserStore(IdentityContext ctx)
            : base(ctx)
        {
        }
    }

    public class UserManager : UserManager<User, string>
    {
        public UserManager(UserStore store)
            : base(store)
        {
            this.ClaimsIdentityFactory = new ClaimsFactory();
        }

    }

    public class ClaimsFactory : ClaimsIdentityFactory<User, string>
    {
        public ClaimsFactory() { }
    }

    public class RoleStore : RoleStore<Role>
    {
        public RoleStore(IdentityContext ctx)
            : base(ctx)
        {
        }
    }

    public class RoleManager : RoleManager<Role>
    {
        public RoleManager(RoleStore store)
            : base(store)
        {
        }
    }


    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }

}
