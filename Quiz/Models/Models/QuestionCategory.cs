using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Resources;
using QuizWeb.Misc;
using System.ComponentModel.DataAnnotations.Schema;





namespace QuizWeb.Models
{
    public class QuestionCategory
    {
        public QuestionCategory()
        {
            ChildCategories = new HashSet<QuestionCategory>();
            Questions = new HashSet<Question>();

        }

        [Key]
        public int ID { get; set; }

        [LocalizedRequired("MandatoryQuestionCategoryName")]
        [LocalizedDisplayName("QuestionCategoryName")]
        [DataType(DataType.Text)]
        public string Name { get; set; }


        public int? ParentCategoryID { get; set; }
        [LocalizedDisplayName("ParentQuestionCategory")]
        [ForeignKey("ParentCategoryID")]
        public virtual QuestionCategory ParentCategory { get; set; }
        public virtual ICollection<QuestionCategory> ChildCategories { get; set; }


        public int HierarchyLevel { get; set; }
        [LocalizedDisplayName("QuestionCategoryHierarchyPath")]
        public string HierarchyPath { get; set; }

        public string FullHierarchyPath
        {
            get
            {
                string fullPath = "";
                QuestionCategory currentCategory = this;
                do
                {
                    fullPath = "/" + currentCategory.Name + fullPath;
                    currentCategory =  currentCategory.ParentCategory;
                } while (currentCategory != null);
                return fullPath;
            }
        }

        public virtual ICollection<Question> Questions { get; set; }

       
        public int UserID { get; set; }
        [LocalizedDisplayName("User")]
        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        public int QuestionCount 
        {
            get 
            {
                return this.Questions.Count + this.ChildCategories.Sum(cqc => cqc.QuestionCount);
            }
        }

    }

}
