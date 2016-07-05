using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizWeb.ViewModels
{
    public class JavascriptRedirectModel
    {
        public JavascriptRedirectModel(string location)
        {
            Location = location;
        }

        public string Location { get; set; }
    }
}