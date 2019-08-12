namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Branches", "Address", c => c.String(nullable: false));
            AlterColumn("dbo.Branches", "PhoneNumber", c => c.String(nullable: false));
            AlterColumn("dbo.Classes", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Classes", "Name", c => c.String());
            AlterColumn("dbo.Branches", "PhoneNumber", c => c.String());
            AlterColumn("dbo.Branches", "Address", c => c.String());
        }
    }
}
