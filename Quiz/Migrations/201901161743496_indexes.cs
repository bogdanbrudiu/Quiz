namespace QuizWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class indexes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.QuestionCategoryQuestions", new[] { "QuestionsID" });
            DropIndex("dbo.QuestionCategoryQuestions", new[] { "QuestionCategoryID" });
            DropIndex("dbo.TestDetails", new[] { "TestID" });
            DropIndex("dbo.Tests", new[] { "QuizID" });
            DropIndex("dbo.Quizs", new[] { "UserID" });
            DropIndex("dbo.QuizDetails", new[] { "QuestionID" });
            DropIndex("dbo.QuizDetails", new[] { "QuestionCategoryID" });
            DropIndex("dbo.QuizDetails", new[] { "QuizID" });
            DropIndex("dbo.QuestionCategories", new[] { "UserID" });
            DropIndex("dbo.QuestionCategories", new[] { "ParentCategoryID" });
            DropIndex("dbo.Questions", new[] { "UserID" });
            DropIndex("dbo.Answers", new[] { "QuestionID" });

            CreateIndex("dbo.Answers", "QuestionID");
            CreateIndex("dbo.Questions", "UserID");
            CreateIndex("dbo.QuestionCategories", "ParentCategoryID");
            CreateIndex("dbo.QuestionCategories", "UserID");
            CreateIndex("dbo.QuizDetails", "QuizID");
            CreateIndex("dbo.QuizDetails", "QuestionCategoryID");
            CreateIndex("dbo.QuizDetails", "QuestionID");
            CreateIndex("dbo.Quizs", "UserID");
            CreateIndex("dbo.Tests", "QuizID");
            CreateIndex("dbo.TestDetails", "TestID");
            CreateIndex("dbo.QuestionCategoryQuestions", "QuestionCategoryID");
            CreateIndex("dbo.QuestionCategoryQuestions", "QuestionsID");
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Answers", "QuestionID");
            CreateIndex("dbo.Questions", "UserID");
            CreateIndex("dbo.QuestionCategories", "ParentCategoryID");
            CreateIndex("dbo.QuestionCategories", "UserID");
            CreateIndex("dbo.QuizDetails", "QuizID");
            CreateIndex("dbo.QuizDetails", "QuestionCategoryID");
            CreateIndex("dbo.QuizDetails", "QuestionID");
            CreateIndex("dbo.Quizs", "UserID");
            CreateIndex("dbo.Tests", "QuizID");
            CreateIndex("dbo.TestDetails", "TestID");
            CreateIndex("dbo.QuestionCategoryQuestions", "QuestionCategoryID");
            CreateIndex("dbo.QuestionCategoryQuestions", "QuestionsID");

            DropIndex("dbo.QuestionCategoryQuestions", new[] { "QuestionsID" });
            DropIndex("dbo.QuestionCategoryQuestions", new[] { "QuestionCategoryID" });
            DropIndex("dbo.TestDetails", new[] { "TestID" });
            DropIndex("dbo.Tests", new[] { "QuizID" });
            DropIndex("dbo.Quizs", new[] { "UserID" });
            DropIndex("dbo.QuizDetails", new[] { "QuestionID" });
            DropIndex("dbo.QuizDetails", new[] { "QuestionCategoryID" });
            DropIndex("dbo.QuizDetails", new[] { "QuizID" });
            DropIndex("dbo.QuestionCategories", new[] { "UserID" });
            DropIndex("dbo.QuestionCategories", new[] { "ParentCategoryID" });
            DropIndex("dbo.Questions", new[] { "UserID" });
            DropIndex("dbo.Answers", new[] { "QuestionID" });
        }
    }
}
