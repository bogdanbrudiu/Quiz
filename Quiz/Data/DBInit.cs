using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Data.Entity.Validation;
using QuizWeb.Models;

namespace QuizWeb.Data
{
    public class DBInit
    {

//           Sql(@"IF OBJECT_ID ('[dbo].[trg_QuestionCategoriesInsert]', 'TR') IS NOT NULL
//   DROP TRIGGER [dbo].[trg_QuestionCategoriesInsert];");
//            Sql(@"CREATE TRIGGER [dbo].[trg_QuestionCategoriesInsert] ON [dbo].[QuestionCategories] FOR INSERT
//AS 
//BEGIN
//    DECLARE @numrows int
//    SET @numrows = @@ROWCOUNT
//    
//    if @numrows > 1 
//    BEGIN
//        RAISERROR('Only single row insertion is supported', 16, 1)
//        ROLLBACK TRAN
//    END
//    ELSE    
//    BEGIN
//        UPDATE 
//            M
//        SET
//            HierarchyLevel    = 
//            CASE 
//                WHEN M.ParentCategoryID IS NULL THEN 0
//                ELSE ParentCategory.HierarchyLevel + 1
//            END,
//            HierarchyPath = 
//            CASE
//                WHEN M.ParentCategoryID IS NULL THEN '.'
//                ELSE ParentCategory.HierarchyPath 
//            END + CAST((M.ID) AS varchar(10)) + '.'
//            FROM
//                QuestionCategories AS M
//            INNER JOIN
//                inserted AS I ON I.ID = M.ID
//            LEFT OUTER JOIN
//                QuestionCategories AS ParentCategory ON ParentCategory.ID = M.ParentCategoryID
//    END
//END");
//            Sql(@"
//IF OBJECT_ID ('[dbo].[trg_QuestionCategoriesUpdate]', 'TR') IS NOT NULL
//   DROP TRIGGER [dbo].[trg_QuestionCategoriesUpdate];
//");
//            Sql(@"
// CREATE TRIGGER [dbo].[trg_QuestionCategoriesUpdate] ON [dbo].[QuestionCategories] FOR UPDATE
//AS 
//BEGIN
//  IF @@ROWCOUNT = 0 
//        RETURN
//    
//    if UPDATE(ParentCategoryID) 
//    BEGIN
//        UPDATE
//            M
//        SET
//            HierarchyLevel    = 
//                M.HierarchyLevel - I.HierarchyLevel + 
//                    CASE 
//                        WHEN I.ParentCategoryID IS NULL THEN 0
//                        ELSE ParentCategory.HierarchyLevel + 1
//                    END,
//            HierarchyPath = 
//                ISNULL(ParentCategory.HierarchyPath, '.') +
//                CAST((I.ID) as varchar(10)) + '.' +
//                RIGHT(M.HierarchyPath, len(M.HierarchyPath) - len(I.HierarchyPath))
//            FROM
//                QuestionCategories AS M
//            INNER JOIN
//                inserted AS I ON M.HierarchyPath LIKE I.HierarchyPath + '%'
//            LEFT OUTER JOIN
//                QuestionCategories AS ParentCategory ON I.ParentCategoryID = ParentCategory.ID
//    END
//
//END"); 

        public static void Init(QuizContext context)
        {

            var users = new List<User>
            {
                new User { ID=1, Login = "administrator",  FirstName="", LastName="", Password="administrator",IsAdmin = true },
                new User { ID=2, Login = "user1",  FirstName="", LastName="", Password="user1",IsAdmin=false }
            };

            users.ForEach(u => context.Users.AddOrUpdate(u));
            context.SaveChanges();

            //var questionCategories = new List<QuestionCategory>
            //{
            //    new QuestionCategory { ID=1, Name="Matematica", UserID=1, ParentCategoryID=null },
            //    new QuestionCategory { ID=2, Name="Algebra", UserID=1, ParentCategoryID=1 },
            //    new QuestionCategory { ID=3, Name="Adunare", UserID=1, ParentCategoryID=2 },
            //    new QuestionCategory { ID=4, Name="Scadere", UserID=1, ParentCategoryID=2 },
            //    new QuestionCategory { ID=5, Name="Inmultire", UserID=1, ParentCategoryID=2 },
            //    new QuestionCategory { ID=6, Name="Impartire", UserID=1, ParentCategoryID=2 },
            //    new QuestionCategory { ID=7, Name="Geometrie", UserID=1, ParentCategoryID=1 },
            //    new QuestionCategory { ID=8, Name="Arie", UserID=1, ParentCategoryID=7 },
            //    new QuestionCategory { ID=9, Name="Volum", UserID=1, ParentCategoryID=7 },
            //    new QuestionCategory { ID=10, Name="PC", UserID=1, ParentCategoryID=null },
            //    new QuestionCategory { ID=11, Name="Hard", UserID=1, ParentCategoryID=10 },
            //    new QuestionCategory { ID=12, Name="Soft", UserID=1, ParentCategoryID=10 },

            //    new QuestionCategory { ID=13, Name="Drept", UserID=2, ParentCategoryID=null },
            //    new QuestionCategory { ID=14, Name="Penal", UserID=2, ParentCategoryID=13 },
            //    new QuestionCategory { ID=15, Name="Civil", UserID=2, ParentCategoryID=13 },
            //};

            //questionCategories.ForEach(qc => context.QuestionCategories.AddOrUpdate(qc));

            //var questions = new List<Question>
            //{
            //    new Question { ID=1, Description="1+1", UserID=1 },
            //    new Question { ID=2, Description="1*2", UserID=1 },
            //    new Question { ID=3, Description="4/2", UserID=1 },
            //    new Question { ID=4, Description="3m/3m", UserID=1 },
            //    new Question { ID=5, Description="2m/2m/2m", UserID=1 }
            //};
            //questions[0].QuestionCategories.Add(questionCategories[2]);
            //questions[1].QuestionCategories.Add(questionCategories[4]);
            //questions[2].QuestionCategories.Add(questionCategories[5]);
            //questions[3].QuestionCategories.Add(questionCategories[7]);
            //questions[4].QuestionCategories.Add(questionCategories[8]);

            //questions.ForEach(q => context.Questions.AddOrUpdate(q));

            //var answers = new List<Answer>
            //{
            //    new Answer { ID=1, Description="2", QuestionID=1, IsAnswer=true },
            //    new Answer { ID=2, Description="3", QuestionID=1, IsAnswer=true },
            //    new Answer { ID=3, Description="0", QuestionID=1, IsAnswer=true },
            //    new Answer { ID=4, Description="5", QuestionID=1, IsAnswer=false },
               
            //    new Answer { ID=5, Description="2", QuestionID=2, IsAnswer=true },
            //    new Answer { ID=6, Description="1", QuestionID=2, IsAnswer=false },

            //    new Answer { ID=7, Description="6", QuestionID=3, IsAnswer=false },
            //    new Answer { ID=8, Description="9", QuestionID=3, IsAnswer=true },

            //    new Answer { ID=9, Description="8", QuestionID=4, IsAnswer=true },
            //    new Answer { ID=10, Description="6", QuestionID=4, IsAnswer=false },
            //};

            //answers.ForEach(a => context.Answers.AddOrUpdate(a));

        }
    }
}