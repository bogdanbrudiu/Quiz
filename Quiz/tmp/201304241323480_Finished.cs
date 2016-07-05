namespace QuizWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Finished : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tests", "Finished", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tests", "Finished");
        }
    }
}
