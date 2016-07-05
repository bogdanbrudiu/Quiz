using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Resources;
using QuizWeb.Misc;
namespace QuizWeb.Models {

    public class User
    {

        public User()
        {
            QuestionCategories = new HashSet<QuestionCategory>();
            Questions = new HashSet<Question>();

        }

        [Key]
        public int ID { get; set; }
        [LocalizedRequired("MandatoryLogin")]
        [LocalizedDisplayName("Login")]
        [DataType(DataType.Text)]
        public string Login { get; set; }
        [LocalizedDisplayName("FirstName")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }
        [LocalizedDisplayName("LastName")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }
        [LocalizedDisplayName("Password")]
        [DataType(DataType.Text)]
        public string Password { get; set; }
        [LocalizedDisplayName("IsAdmin")]
        public bool IsAdmin { get; set; }

        public virtual ICollection<QuestionCategory> QuestionCategories { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
   
    }

}