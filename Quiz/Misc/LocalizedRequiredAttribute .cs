using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace QuizWeb.Misc
{
   
    public class LocalizedRequiredAttribute : System.ComponentModel.DataAnnotations.RequiredAttribute
    {
        private string message;

        public LocalizedRequiredAttribute(string message)
        {
            ErrorMessageResourceName = message;
        }

        protected override ValidationResult IsValid
        (object value, ValidationContext validationContext)
        {
            message = validationContext.DisplayName;
            return base.IsValid(value, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            var msg = LanguageService.Instance.Translate(ErrorMessageResourceName);
            return string.Format(msg, message);
        }
    }
}