namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeToStringID : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Classes", new[] { "Trainer_Id" });
            DropColumn("dbo.Classes", "TrainerID");
            RenameColumn(table: "dbo.Classes", name: "Trainer_Id", newName: "TrainerID");
            AlterColumn("dbo.Classes", "TrainerID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Classes", "TrainerID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Classes", new[] { "TrainerID" });
            AlterColumn("dbo.Classes", "TrainerID", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Classes", name: "TrainerID", newName: "Trainer_Id");
            AddColumn("dbo.Classes", "TrainerID", c => c.Int(nullable: false));
            CreateIndex("dbo.Classes", "Trainer_Id");
        }
    }
}
