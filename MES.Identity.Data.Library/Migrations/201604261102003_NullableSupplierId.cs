namespace MES.Identity.Data.Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableSupplierId : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "SupplierId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "SupplierId", c => c.Int(nullable: false));
        }
    }
}
