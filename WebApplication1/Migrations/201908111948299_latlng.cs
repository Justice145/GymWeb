namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class latlng : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Branches", "Lat", c => c.Single(nullable: false));
            AddColumn("dbo.Branches", "Lng", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Branches", "Lng");
            DropColumn("dbo.Branches", "Lat");
        }
    }
}
