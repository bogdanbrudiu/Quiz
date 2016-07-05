using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using QuizWeb.Misc;

namespace QuizWeb.ViewModels
{
    public class LogOnModel 
    {
        [Required(ErrorMessage="User name is mandatory!")]
        [LocalizedDisplayName("UserName")]
        public string UserName { get; set; }
         [LocalizedDisplayName("Password")]
        public string Password { get; set; }
         [LocalizedDisplayName("RememberMe")]
        public bool RememberMe { get; set; }
    }
  
}