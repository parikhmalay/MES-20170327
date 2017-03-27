namespace MES.Identity.Data.Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserCodeColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "UserCode", c => c.String(maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "TitleId", c => c.Short());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "TitleId", c => c.Short(nullable: false));
            DropColumn("dbo.AspNetUsers", "UserCode");
        }
    }
}
