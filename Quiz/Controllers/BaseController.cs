using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using QuizWeb.ViewModels;
using QuizWeb.Data;
using QuizWeb.Models;

namespace QuizWeb.Controllers
{

    public class BaseController : Controller
    {
         protected UnitOfWork unitOfWork;

         public BaseController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public User CurrentUser
        {
            get
            {
                return unitOfWork.UsersRepository.Get().Where(u => u.Login == ((CustomPrincipal)this.HttpContext.User).Identity.Name).FirstOrDefault();
            }
        }
	}


}