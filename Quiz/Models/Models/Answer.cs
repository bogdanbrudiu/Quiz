using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Resources;
using System.ComponentModel.DataAnnotations.Schema;
using QuizWeb.Misc;
using System.Web.Mvc;


namespace QuizWeb.Models
{
   
    public class Answer
    {
        public Answer()
        {
            //TestResponses = new HashSet<TestDetail>();

        }


        [Key]
        public int ID { get; set; }


        public int QuestionID { get; set; }
        [LocalizedDisplayName("Question")]
        [ForeignKey("QuestionID")]
        public virtual Question Question { get; set; }

        [LocalizedRequired("MandatoryAnswerDescription")]
        [LocalizedDisplayName("AnswerDescription")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        [UIHint("tinymce_jquery_full")]
        public string Description { get; set; }


         [LocalizedDisplayName("IsAnswer")]
        public bool IsAnswer { get; set; }

         //public virtual ICollection<TestDetail> TestResponses { get; set; }

    }

}
