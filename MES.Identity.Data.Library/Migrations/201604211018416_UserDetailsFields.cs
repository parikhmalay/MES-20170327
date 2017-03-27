namespace MES.Identity.Data.Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserDetailsFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "TitleId", c => c.Short(nullable: false));
            AddColumn("dbo.AspNetUsers", "GenderId", c => c.Short(nullable: false));
            AddColumn("dbo.AspNetUsers", "MiddleName", c => c.String(maxLength: 20));
            AddColumn("dbo.AspNetUsers", "AddressLine1", c => c.String(maxLength: 50));
            AddColumn("dbo.AspNetUsers", "AddressLine2", c => c.String(maxLength: 50));
            AddColumn("dbo.AspNetUsers", "City", c => c.String(maxLength: 50));
            AddColumn("dbo.AspNetUsers", "State", c => c.String(maxLength: 50));
            AddColumn("dbo.AspNetUsers", "CountryId", c => c.Short(nullable: false));
            AddColumn("dbo.AspNetUsers", "ZipCode", c => c.String(maxLength: 15));
            AddColumn("dbo.AspNetUsers", "RoleId", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "SupplierId", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "IsRFQCoordinator", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "NextSystemMessageDisplayDate", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.AspNetUsers", "CreatedBy", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.AspNetUsers", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "UpdatedBy", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "UpdatedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "UpdatedDate");
            DropColumn("dbo.AspNetUsers", "UpdatedBy");
            DropColumn("dbo.AspNetUsers", "CreatedDate");
            DropColumn("dbo.AspNetUsers", "CreatedBy");
            DropColumn("dbo.AspNetUsers", "NextSystemMessageDisplayDate");
            DropColumn("dbo.AspNetUsers", "IsRFQCoordinator");
            DropColumn("dbo.AspNetUsers", "SupplierId");
            DropColumn("dbo.AspNetUsers", "Active");
            DropColumn("dbo.AspNetUsers", "RoleId");
            DropColumn("dbo.AspNetUsers", "ZipCode");
            DropColumn("dbo.AspNetUsers", "CountryId");
            DropColumn("dbo.AspNetUsers", "State");
            DropColumn("dbo.AspNetUsers", "City");
            DropColumn("dbo.AspNetUsers", "AddressLine2");
            DropColumn("dbo.AspNetUsers", "AddressLine1");
            DropColumn("dbo.AspNetUsers", "MiddleName");
            DropColumn("dbo.AspNetUsers", "GenderId");
            DropColumn("dbo.AspNetUsers", "TitleId");
        }
    }
}
