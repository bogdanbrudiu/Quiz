namespace QuizWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HierarchyPath : DbMigration
    {
        public override void Up()
        {
            Sql(@"IF OBJECT_ID ('[dbo].[trg_QuestionCategoriesInsert]', 'TR') IS NOT NULL
   DROP TRIGGER [dbo].[trg_QuestionCategoriesInsert];");
            Sql(@"CREATE TRIGGER [dbo].[trg_QuestionCategoriesInsert] ON [dbo].[QuestionCategories] FOR INSERT
            AS 
            BEGIN
                DECLARE @numrows int
                SET @numrows = @@ROWCOUNT
                
                if @numrows > 1 
                BEGIN
                    RAISERROR('Only single row insertion is supported', 16, 1)
                    ROLLBACK TRAN
                END
                ELSE    
                BEGIN
                    UPDATE 
                        M
                    SET
                        HierarchyLevel    = 
                        CASE 
                            WHEN M.ParentCategoryID IS NULL THEN 0
                            ELSE ParentCategory.HierarchyLevel + 1
                        END,
                        HierarchyPath = 
                        CASE
                            WHEN M.ParentCategoryID IS NULL THEN '.'
                            ELSE ParentCategory.HierarchyPath 
                        END + CAST((M.ID) AS varchar(10)) + '.'
                        FROM
                            QuestionCategories AS M
                        INNER JOIN
                            inserted AS I ON I.ID = M.ID
                        LEFT OUTER JOIN
                            QuestionCategories AS ParentCategory ON ParentCategory.ID = M.ParentCategoryID
                END
            END");
            Sql(@"
            IF OBJECT_ID ('[dbo].[trg_QuestionCategoriesUpdate]', 'TR') IS NOT NULL
               DROP TRIGGER [dbo].[trg_QuestionCategoriesUpdate];
            ");
            Sql(@"
             CREATE TRIGGER [dbo].[trg_QuestionCategoriesUpdate] ON [dbo].[QuestionCategories] FOR UPDATE
            AS 
            BEGIN
              IF @@ROWCOUNT = 0 
                    RETURN
                
                if UPDATE(ParentCategoryID) 
                BEGIN
                    UPDATE
                        M
                    SET
                        HierarchyLevel    = 
                            M.HierarchyLevel - I.HierarchyLevel + 
                                CASE 
                                    WHEN I.ParentCategoryID IS NULL THEN 0
                                    ELSE ParentCategory.HierarchyLevel + 1
                                END,
                        HierarchyPath = 
                            ISNULL(ParentCategory.HierarchyPath, '.') +
                            CAST((I.ID) as varchar(10)) + '.' +
                            RIGHT(M.HierarchyPath, len(M.HierarchyPath) - len(I.HierarchyPath))
                        FROM
                            QuestionCategories AS M
                        INNER JOIN
                            inserted AS I ON M.HierarchyPath LIKE I.HierarchyPath + '%'
                        LEFT OUTER JOIN
                            QuestionCategories AS ParentCategory ON I.ParentCategoryID = ParentCategory.ID
                END
            
            END");
        }

        public override void Down()
        {
            Sql(@"IF OBJECT_ID ('[dbo].[trg_QuestionCategoriesInsert]', 'TR') IS NOT NULL
   DROP TRIGGER [dbo].[trg_QuestionCategoriesInsert];");
            Sql(@"
            IF OBJECT_ID ('[dbo].[trg_QuestionCategoriesUpdate]', 'TR') IS NOT NULL
               DROP TRIGGER [dbo].[trg_QuestionCategoriesUpdate];
            ");
        }
    }
}
