using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Resources;
using QuizWeb.Misc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;
using System.Text.RegularExpressions;

namespace QuizWeb.Models
{
    public class TestDetail
    {
        public TestDetail()
        {
          

        }

        [Key]
        public int ID { get; set; }

        public int TestID { get; set; }
        [ForeignKey("TestID")]
        public virtual Test Test { get; set; }

        //public int QuestionID { get; set; }
        //[ForeignKey("QuestionID")]
        //public virtual Question Question { get; set; }

        //[NotMapped]
        //public int[] ResponseID 
        //{
        //    get { return Responses.Select(r => r.ID).ToArray(); } 
        //}

        //public virtual ICollection<Answer> Responses { get; set; }
        public int QuestionID { get; set; }
     
        public string QuestionDescription { get; set; }


        public string ResponsesArray { get; set; }

        [NotMapped]
        public IEnumerable<string> Responses { 
            get 
            {
                return string.IsNullOrEmpty(ResponsesArray)?new List<string>(): Regex.Split(ResponsesArray, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)").ToArray<string>().Select(x => x.Substring(1, x.Length - 2));
            } 
            set 
            {
                ResponsesArray = string.Join(",", value.Select(x => "\"" + x + "\""));
            } 
        }

        public bool IsAnswer { get; set; }

        public bool Answered { get; set; }

        public DateTime? AnsweredOn { get; set; }
    }

}
