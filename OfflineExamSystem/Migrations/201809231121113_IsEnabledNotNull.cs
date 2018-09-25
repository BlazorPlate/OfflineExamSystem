namespace OfflineExamSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsEnabledNotNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "IsEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "IsEnabled", c => c.Boolean());
        }
    }
}
