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
    public class Test
    {
        public Test()
        {
            TestDetails = new HashSet<TestDetail>();

        }

        [Key]
        public int ID { get; set; }

        [LocalizedRequired("MandatoryTestFirstName")]
        [LocalizedDisplayName("TestFirstName")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [LocalizedRequired("MandatoryTestLastName")]
        [LocalizedDisplayName("TestLastName")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [LocalizedRequired("MandatoryTestAnStudiu")]
        [LocalizedDisplayName("TestAnStudiu")]
        [DataType(DataType.Text)]
        public string AnStudiu { get; set; }

        [LocalizedRequired("MandatoryTestGrupa")]
        [LocalizedDisplayName("TestGrupa")]
        [DataType(DataType.Text)]
        public string Grupa { get; set; }

        [LocalizedRequired("MandatoryTestNrMatricol")]
        [LocalizedDisplayName("TestNrMatricol")]
        [DataType(DataType.Text)]
        public string NrMatricol { get; set; }

        public int QuizID { get; set; }
        [LocalizedDisplayName("Quiz")]
        [ForeignKey("QuizID")]
        public virtual Quiz Quiz { get; set; }

        public bool Finished { get; set; }

        public virtual ICollection<TestDetail> TestDetails { get; set; }
       
    }

}
