using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using QuizWeb.Models;
using System.Text;
using System.Data.Entity.Validation;


namespace QuizWeb.Data
{

    public class QuizContext : DbContext
    {
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<QuestionCategory> QuestionCategories { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizDetail> QuizDetails { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestDetail> TestDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuestionCategory>()
            .HasMany(x => x.ChildCategories)
            .WithOptional(x => x.ParentCategory).WillCascadeOnDelete(false);

            modelBuilder.Entity<QuestionCategory>()
            .HasMany(x => x.Questions)
            .WithMany(x => x.QuestionCategories)
            .Map(
            x =>
            {
                x.ToTable("QuestionCategoryQuestions");
                x.MapLeftKey("QuestionCategoryID");
                x.MapRightKey("QuestionsID");
            }
            );
         

            modelBuilder.Entity<Question>()
           .HasMany(x => x.Answers)
           .WithRequired(x => x.Question).WillCascadeOnDelete(true);

            modelBuilder.Entity<Quiz>()
            .HasMany(x => x.QuizDetails)
            .WithRequired(x => x.Quiz).WillCascadeOnDelete(true);

            modelBuilder.Entity<Quiz>()
            .HasMany(x => x.Tests)
            .WithRequired(x => x.Quiz).WillCascadeOnDelete(true);

            modelBuilder.Entity<Test>()
            .HasMany(x => x.TestDetails)
            .WithRequired(x => x.Test).WillCascadeOnDelete(true);

            //modelBuilder.Entity<Answer>()
            //.HasMany(x => x.TestResponses)
            //.WithMany(x => x.Responses)
            //.Map(
            //x =>
            //{
            //    x.ToTable("AnswerTestDetails");
            //    x.MapLeftKey("AnswerID");
            //    x.MapRightKey("TestDetailID");
            //}
            //);
          

            modelBuilder.Entity<User>()
            .HasMany(x => x.Questions)
            .WithRequired(x => x.User).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
           .HasMany(x => x.QuestionCategories)
           .WithRequired(x => x.User).WillCascadeOnDelete(false);

        }
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }
  	 

    }
}