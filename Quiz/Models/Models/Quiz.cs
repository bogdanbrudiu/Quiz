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
    public class Quiz
    {
        public Quiz()
        {
            QuizDetails = new HashSet<QuizDetail>();
            Tests = new HashSet<Test>();

        }

        [Key]
        public int ID { get; set; }

        [LocalizedRequired("MandatoryQuizName")]
        [LocalizedDisplayName("QuizName")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [LocalizedDisplayName("QuizCode")]
        public string Code { get; set; }

        public virtual ICollection<QuizDetail> QuizDetails { get; set; }
        public virtual ICollection<Test> Tests { get; set; }

        [LocalizedDisplayName("AlwaysGenerate")]
        public bool AlwaysGenerate { get; set; }
         [LocalizedDisplayName("RandomiseQuestionsOrder")]
        public bool RandomiseQuestionsOrder { get; set; }
         [LocalizedDisplayName("RandomiseAnswersOrder")]
        public bool RandomiseAnswersOrder { get; set; }
         [LocalizedDisplayName("AllowNavigate")]
         public bool AllowNavigate { get; set; }
            
         [LocalizedRequired("MandatoryQuestionsPerPage")]
         [LocalizedDisplayName("QuestionsPerPage")]
         public int QuestionsPerPage { get; set; }


        public int UserID { get; set; }
        [LocalizedDisplayName("User")]
        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        
    }

}
