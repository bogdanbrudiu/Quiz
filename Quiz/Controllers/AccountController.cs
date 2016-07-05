using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using QuizWeb.ViewModels;
using QuizWeb.Data;

namespace QuizWeb.Controllers
{

    public interface IAuth
    {
        void SetAuthCookie(string name, bool persistent);
        void SignOut();
    }

    public class FormsAuthWrapper : IAuth
    {
        public void SetAuthCookie(string name, bool persistent)
        {
            FormsAuthentication.SetAuthCookie(name, persistent);
        }
        public virtual void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }

    public class AccountController : BaseController
    {

        private readonly IAuth formsAuth;
        public AccountController(IAuth formsAuth, UnitOfWork unitOfWork) : base(unitOfWork)
        {
            this.formsAuth = formsAuth;
        }

        public ActionResult LogOn()
        {
            return View(new LogOnModel());	
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid && unitOfWork.UsersRepository.Get().Where(u=>u.Login==model.UserName && u.Password==model.Password).Count()==1)
            {
                formsAuth.SetAuthCookie(model.UserName, model.RememberMe);
            }
            else
            {
                return View(model);
            }
            return RedirectToReturnUrl(returnUrl);
        }

        private ActionResult RedirectToReturnUrl(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                       && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult LogOff()
        {
            formsAuth.SignOut();
            return RedirectToAction("Index", "Home");
        }
	}


}