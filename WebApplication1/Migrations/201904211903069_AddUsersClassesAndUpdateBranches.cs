namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUsersClassesAndUpdateBranches : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Classes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Time = c.DateTime(nullable: false),
                        TrainerID = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .ForeignKey("dbo.Users", t => t.TrainerID, cascadeDelete: true)
                .Index(t => t.TrainerID)
                .Index(t => t.BranchId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Password = c.String(),
                        Address = c.String(),
                        PhoneNumber = c.String(),
                        UserType = c.Int(nullable: false),
                        Class_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Classes", t => t.Class_Id)
                .Index(t => t.Class_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Classes", "TrainerID", "dbo.Users");
            DropForeignKey("dbo.Users", "Class_Id", "dbo.Classes");
            DropForeignKey("dbo.Classes", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Classes", "BranchId", "dbo.Branches");
            DropIndex("dbo.Users", new[] { "Class_Id" });
            DropIndex("dbo.Classes", new[] { "User_Id" });
            DropIndex("dbo.Classes", new[] { "BranchId" });
            DropIndex("dbo.Classes", new[] { "TrainerID" });
            DropTable("dbo.Users");
            DropTable("dbo.Classes");
        }
    }
}
