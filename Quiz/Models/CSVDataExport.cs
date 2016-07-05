using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CsvHelper.Configuration;

namespace QuizWeb.Models
{
    public class CSVDataExport
    {

        public string ID { get; set; }

        public string Categories { get; set; }

        public string RightAnswer { get; set; }

        public string Description { get; set; }

        public string Answers { get; set; }
        
    }
}