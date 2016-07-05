using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Resources;
using QuizWeb.Misc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;





namespace QuizWeb.Models
{
    public class Question
    {
        public Question()
        {
            QuestionCategories = new HashSet<QuestionCategory>();
            Answers = new HashSet<Answer>();

        }

        [Key]
        public int ID { get; set; }



        [LocalizedRequired("MandatoryQuestionDescription")]
        [LocalizedDisplayName("QuestionDescription")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        [UIHint("tinymce_jquery_full")]
        public string Description { get; set; }

        [LocalizedDisplayName("QuestionDetails")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        [UIHint("tinymce_jquery_full")]
        public string Details { get; set; }

        public virtual ICollection<QuestionCategory> QuestionCategories { get; set; }

        [LocalizedDisplayName("QuestionAnswers")]
        public virtual ICollection<Answer> Answers { get; set; }

         
         public int UserID { get; set; }
         [LocalizedDisplayName("User")]
         [ForeignKey("UserID")]
         public virtual User User { get; set; }
    }

}
