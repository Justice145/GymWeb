namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeLatLngToDouble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Branches", "Lat", c => c.Double(nullable: false));
            AlterColumn("dbo.Branches", "Lng", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Branches", "Lng", c => c.Single(nullable: false));
            AlterColumn("dbo.Branches", "Lat", c => c.Single(nullable: false));
        }
    }
}
