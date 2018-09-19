namespace OfflineExamSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            //DropColumn("dbo.AspNetUsers", "EmpId");
            //DropColumn("dbo.AspNetUsers", "FullName_EN");
            //DropColumn("dbo.AspNetUsers", "FullName_Ar");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "FullName_Ar", c => c.String());
            AddColumn("dbo.AspNetUsers", "FullName_EN", c => c.String());
            AddColumn("dbo.AspNetUsers", "EmpId", c => c.Int(nullable: false));
        }
    }
}
