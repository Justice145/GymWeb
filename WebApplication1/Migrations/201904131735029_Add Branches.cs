namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBranches : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Branches", "PhoneNumber", c => c.String());
            AddColumn("dbo.Branches", "WeekDayOpen", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.Branches", "WeekDayClose", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.Branches", "FridayOpen", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.Branches", "FridayClose", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.Branches", "SaturdayOpen", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.Branches", "SaturdayClose", c => c.Time(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Branches", "SaturdayClose");
            DropColumn("dbo.Branches", "SaturdayOpen");
            DropColumn("dbo.Branches", "FridayClose");
            DropColumn("dbo.Branches", "FridayOpen");
            DropColumn("dbo.Branches", "WeekDayClose");
            DropColumn("dbo.Branches", "WeekDayOpen");
            DropColumn("dbo.Branches", "PhoneNumber");
        }
    }
}
