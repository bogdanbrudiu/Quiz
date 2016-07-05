namespace QuizWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        Details = c.String(),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.QuestionCategories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ParentCategoryID = c.Int(),
                        HierarchyLevel = c.Int(nullable: false),
                        HierarchyPath = c.String(),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.QuestionCategories", t => t.ParentCategoryID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .Index(t => t.ParentCategoryID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Login = c.String(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Password = c.String(),
                        IsAdmin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        QuestionID = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        IsAnswer = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Questions", t => t.QuestionID, cascadeDelete: true)
                .Index(t => t.QuestionID);
            
            CreateTable(
                "dbo.TestDetails",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TestID = c.Int(nullable: false),
                        QuestionID = c.Int(nullable: false),
                        ResponseID = c.Int(),
                        Answered = c.Boolean(nullable: false),
                        AnsweredOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Tests", t => t.TestID, cascadeDelete: true)
                .ForeignKey("dbo.Questions", t => t.QuestionID, cascadeDelete: true)
                .ForeignKey("dbo.Answers", t => t.ResponseID)
                .Index(t => t.TestID)
                .Index(t => t.QuestionID)
                .Index(t => t.ResponseID);
            
            CreateTable(
                "dbo.Tests",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        QuizID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Quizs", t => t.QuizID, cascadeDelete: true)
                .Index(t => t.QuizID);
            
            CreateTable(
                "dbo.Quizs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Code = c.String(),
                        AlwaysGenerate = c.Boolean(nullable: false),
                        RandomiseQuestionsOrder = c.Boolean(nullable: false),
                        RandomiseAnswersOrder = c.Boolean(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.QuizDetails",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        QuizID = c.Int(nullable: false),
                        QuestionCategoryID = c.Int(),
                        QuestionID = c.Int(),
                        Number = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.QuestionCategories", t => t.QuestionCategoryID)
                .ForeignKey("dbo.Questions", t => t.QuestionID)
                .ForeignKey("dbo.Quizs", t => t.QuizID, cascadeDelete: true)
                .Index(t => t.QuestionCategoryID)
                .Index(t => t.QuestionID)
                .Index(t => t.QuizID);
            
            CreateTable(
                "dbo.QuestionCategoryQuestions",
                c => new
                    {
                        QuestionCategoryID = c.Int(nullable: false),
                        QuestionsID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.QuestionCategoryID, t.QuestionsID })
                .ForeignKey("dbo.QuestionCategories", t => t.QuestionCategoryID, cascadeDelete: true)
                .ForeignKey("dbo.Questions", t => t.QuestionsID, cascadeDelete: true)
                .Index(t => t.QuestionCategoryID)
                .Index(t => t.QuestionsID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.QuestionCategoryQuestions", new[] { "QuestionsID" });
            DropIndex("dbo.QuestionCategoryQuestions", new[] { "QuestionCategoryID" });
            DropIndex("dbo.QuizDetails", new[] { "QuizID" });
            DropIndex("dbo.QuizDetails", new[] { "QuestionID" });
            DropIndex("dbo.QuizDetails", new[] { "QuestionCategoryID" });
            DropIndex("dbo.Quizs", new[] { "UserID" });
            DropIndex("dbo.Tests", new[] { "QuizID" });
            DropIndex("dbo.TestDetails", new[] { "ResponseID" });
            DropIndex("dbo.TestDetails", new[] { "QuestionID" });
            DropIndex("dbo.TestDetails", new[] { "TestID" });
            DropIndex("dbo.Answers", new[] { "QuestionID" });
            DropIndex("dbo.QuestionCategories", new[] { "UserID" });
            DropIndex("dbo.QuestionCategories", new[] { "ParentCategoryID" });
            DropIndex("dbo.Questions", new[] { "UserID" });
            DropForeignKey("dbo.QuestionCategoryQuestions", "QuestionsID", "dbo.Questions");
            DropForeignKey("dbo.QuestionCategoryQuestions", "QuestionCategoryID", "dbo.QuestionCategories");
            DropForeignKey("dbo.QuizDetails", "QuizID", "dbo.Quizs");
            DropForeignKey("dbo.QuizDetails", "QuestionID", "dbo.Questions");
            DropForeignKey("dbo.QuizDetails", "QuestionCategoryID", "dbo.QuestionCategories");
            DropForeignKey("dbo.Quizs", "UserID", "dbo.Users");
            DropForeignKey("dbo.Tests", "QuizID", "dbo.Quizs");
            DropForeignKey("dbo.TestDetails", "ResponseID", "dbo.Answers");
            DropForeignKey("dbo.TestDetails", "QuestionID", "dbo.Questions");
            DropForeignKey("dbo.TestDetails", "TestID", "dbo.Tests");
            DropForeignKey("dbo.Answers", "QuestionID", "dbo.Questions");
            DropForeignKey("dbo.QuestionCategories", "UserID", "dbo.Users");
            DropForeignKey("dbo.QuestionCategories", "ParentCategoryID", "dbo.QuestionCategories");
            DropForeignKey("dbo.Questions", "UserID", "dbo.Users");
            DropTable("dbo.QuestionCategoryQuestions");
            DropTable("dbo.QuizDetails");
            DropTable("dbo.Quizs");
            DropTable("dbo.Tests");
            DropTable("dbo.TestDetails");
            DropTable("dbo.Answers");
            DropTable("dbo.Users");
            DropTable("dbo.QuestionCategories");
            DropTable("dbo.Questions");
        }
    }
}
