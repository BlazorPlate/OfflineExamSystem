namespace OfflineExamSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "EmpId", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "FullName_EN", c => c.String());
            AddColumn("dbo.AspNetUsers", "FullName_Ar", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "FullName_Ar");
            DropColumn("dbo.AspNetUsers", "FullName_EN");
            DropColumn("dbo.AspNetUsers", "EmpId");
        }
    }
}
