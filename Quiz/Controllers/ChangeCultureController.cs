using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuizWeb.Data;

namespace QuizWeb.Controllers
{

    public class ChangeCultureController : BaseController
    {
        public ChangeCultureController(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        public ActionResult ChangeCulture(string lang, string returnUrl)
        {
            var langCookie = new HttpCookie("lang", lang)
            {
                HttpOnly = true
            };
            Response.AppendCookie(langCookie);
            return Redirect(returnUrl);
        }
    }
}
