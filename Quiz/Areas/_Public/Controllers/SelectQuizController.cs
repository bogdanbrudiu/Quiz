using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuizWeb.Controllers;
using QuizWeb.Data;
using QuizWeb.Misc;
using PagedList;
using QuizWeb.Models;

namespace QuizWeb.Areas._Public.Controllers
{
    public class SelectQuizController : BaseController
    {


        public SelectQuizController(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }


        //
        // GET: /_Public/SelectQuiz/TakeQuiz/
        public ViewResult SelectQuiz(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name desc" : "";
            ViewBag.List = "SelectQuiz";

            if (Request.HttpMethod == "GET")
            {
                searchString = currentFilter;
            }
            else
            {
                page = 1;
            }
            ViewBag.CurrentFilter = searchString;

            var quizs = from s in unitOfWork.QuizzesRepository.Get()
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                quizs = quizs.Where(s => s.Code.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Name desc":
                    quizs = quizs.OrderByDescending(s => s.Name);
                    break;
                default:
                    quizs = quizs.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = Constants.ITEMS_PER_PAGE;
            int pageNumber = (page ?? 1);
            return View("SelectQuiz", quizs.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /_Public/SelectQuiz/TakeQuizByUser/
        public ViewResult SelectQuizByUser(string sortOrder, string currentFilter, string searchString, int? page, int userID)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name desc" : "";
            ViewBag.List = "SelectQuizByUser";
            ViewBag.UserID = userID;

            if (Request.HttpMethod == "GET")
            {
                searchString = currentFilter;
            }
            else
            {
                page = 1;
            }
            ViewBag.CurrentFilter = searchString;

            var quizs = from s in unitOfWork.QuizzesRepository.Get()
                        where s.UserID == userID
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                quizs = quizs.Where(s => s.Code.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Name desc":
                    quizs = quizs.OrderByDescending(s => s.Name);
                    break;
                default:
                    quizs = quizs.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = Constants.ITEMS_PER_PAGE;
            int pageNumber = (page ?? 1);
            return View("SelectQuiz", quizs.ToPagedList(pageNumber, pageSize));
        }


        //
        // GET: /_Public/SelectQuiz/TakeQuizByCode/
        public ActionResult SelectQuizByCode(string searchString)
        {

            Quiz quiz = unitOfWork.QuizzesRepository.Get().Where(q => q.Code == searchString).FirstOrDefault();
            if (quiz != null) 
            {
                return RedirectToAction("Details", new { id = quiz.ID });
            }
          
            return View();
        }


        //
        // GET: /_Public/SelectQuiz/Users/
        public ViewResult Users(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name desc" : "";


            if (Request.HttpMethod == "GET")
            {
                searchString = currentFilter;
            }
            else
            {
                page = 1;
            }
            ViewBag.CurrentFilter = searchString;

            var users = from s in unitOfWork.UsersRepository.Get()
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.Login.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Name desc":
                    users = users.OrderByDescending(s => s.Login);
                    break;
                default:
                    users = users.OrderBy(s => s.Login);
                    break;
            }

            int pageSize = Constants.ITEMS_PER_PAGE;
            int pageNumber = (page ?? 1);
            return View(users.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /_Public/SelectQuiz/Details/5
        public ActionResult Details(int id, string sortOrder, string currentFilter, int? page, string list, int? userID)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            ViewBag.List = list;
            ViewBag.UserID = userID;
            Quiz quiz = unitOfWork.QuizzesRepository.GetByID(id);
            return View(quiz);
        }

      

       
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
