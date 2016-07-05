namespace QuizWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TakeQuiz : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quizs", "AllowNavigate", c => c.Boolean(nullable: false));
            AddColumn("dbo.Quizs", "QuestionsPerPage", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Quizs", "QuestionsPerPage");
            DropColumn("dbo.Quizs", "AllowNavigate");
        }
    }
}
