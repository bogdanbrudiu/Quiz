using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using QuizWeb.Models;

namespace QuizWeb.Areas._Public.ViewModels
{
    public class ConfirmContinueTestModel
    {
        [Required(ErrorMessage = "User name is mandatory!")]
        public string UserName { get; set; }
        public string Password { get; set; }
        public Test Test { get; set; }
    }
}