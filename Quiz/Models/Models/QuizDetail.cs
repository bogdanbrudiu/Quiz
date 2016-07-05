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
    public class QuizDetail
    {
        public QuizDetail()
        {
          

        }

        [Key]
        public int ID { get; set; }

        public int QuizID { get; set; }
        [ForeignKey("QuizID")]
        public virtual Quiz Quiz { get; set; }

        public int? QuestionCategoryID { get; set; }
        [ForeignKey("QuestionCategoryID")]
        public virtual QuestionCategory QuestionCategory { get; set; }

        public int? QuestionID { get; set; }
        [ForeignKey("QuestionID")]
        public virtual Question Question { get; set; }

        public int Number { get; set; }
    }

}
