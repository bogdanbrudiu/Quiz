namespace QuizWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tests", "AnStudiu", c => c.String(nullable: false));
            AddColumn("dbo.Tests", "Grupa", c => c.String(nullable: false));
            AddColumn("dbo.Tests", "NrMatricol", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tests", "NrMatricol");
            DropColumn("dbo.Tests", "Grupa");
            DropColumn("dbo.Tests", "AnStudiu");
        }
    }
}
