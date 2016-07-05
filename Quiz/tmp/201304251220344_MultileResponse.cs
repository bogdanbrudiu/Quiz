namespace QuizWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MultileResponse : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TestDetails", "ResponseID", "dbo.Answers");
            DropIndex("dbo.TestDetails", new[] { "ResponseID" });
            CreateTable(
                "dbo.AnswerTestDetails",
                c => new
                    {
                        AnswerID = c.Int(nullable: false),
                        TestDetailID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AnswerID, t.TestDetailID })
                .ForeignKey("dbo.Answers", t => t.AnswerID, cascadeDelete: true)
                .ForeignKey("dbo.TestDetails", t => t.TestDetailID, cascadeDelete: false)
                .Index(t => t.AnswerID)
                .Index(t => t.TestDetailID);
            
            DropColumn("dbo.TestDetails", "ResponseID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TestDetails", "ResponseID", c => c.Int());
            DropIndex("dbo.AnswerTestDetails", new[] { "TestDetailID" });
            DropIndex("dbo.AnswerTestDetails", new[] { "AnswerID" });
            DropForeignKey("dbo.AnswerTestDetails", "TestDetailID", "dbo.TestDetails");
            DropForeignKey("dbo.AnswerTestDetails", "AnswerID", "dbo.Answers");
            DropTable("dbo.AnswerTestDetails");
            CreateIndex("dbo.TestDetails", "ResponseID");
            AddForeignKey("dbo.TestDetails", "ResponseID", "dbo.Answers", "ID");
        }
    }
}
