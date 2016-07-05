using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Reflection;

namespace QuizWeb.Misc
{

    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
       


        public LocalizedDisplayNameAttribute(string propertyName)
            : base(propertyName)
        {

        }

        public override string DisplayName
        {
            get
            {
                return LanguageService.Instance.Translate(base.DisplayName) ??
            "**" + base.DisplayName + "**";
            }
        }
    }
}