namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m2m : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Classes", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "Class_Id", "dbo.Classes");
            DropIndex("dbo.Classes", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "Class_Id" });
            CreateTable(
                "dbo.TraineeClass",
                c => new
                    {
                        TraineeID = c.String(nullable: false, maxLength: 128),
                        ClassID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TraineeID, t.ClassID })
                .ForeignKey("dbo.AspNetUsers", t => t.TraineeID, cascadeDelete: true)
                .ForeignKey("dbo.Classes", t => t.ClassID, cascadeDelete: true)
                .Index(t => t.TraineeID)
                .Index(t => t.ClassID);
            
            DropColumn("dbo.Classes", "ApplicationUser_Id");
            DropColumn("dbo.AspNetUsers", "Class_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Class_Id", c => c.Int());
            AddColumn("dbo.Classes", "ApplicationUser_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.TraineeClass", "ClassID", "dbo.Classes");
            DropForeignKey("dbo.TraineeClass", "TraineeID", "dbo.AspNetUsers");
            DropIndex("dbo.TraineeClass", new[] { "ClassID" });
            DropIndex("dbo.TraineeClass", new[] { "TraineeID" });
            DropTable("dbo.TraineeClass");
            CreateIndex("dbo.AspNetUsers", "Class_Id");
            CreateIndex("dbo.Classes", "ApplicationUser_Id");
            AddForeignKey("dbo.AspNetUsers", "Class_Id", "dbo.Classes", "Id");
            AddForeignKey("dbo.Classes", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
