namespace MES.Identity.Data.Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserCodeCol : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "UserCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "UserCode", c => c.String(maxLength: 50));
        }
    }
}
