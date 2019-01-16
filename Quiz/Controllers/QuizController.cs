using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using QuizWeb.Data;
using QuizWeb.Misc;
using QuizWeb.Models;
using Resources;
using PagedList;

namespace QuizWeb.Controllers
{
    [Authorize]
    public class QuizController : BaseController
    {


        public QuizController(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }


        //
        // GET: /Quiz/
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
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

            var quizs = from s in unitOfWork.QuizzesRepository.Get()
                                    where s.UserID == CurrentUser.ID
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                quizs = quizs.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()));
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
            return View(quizs.ToPagedList(pageNumber, pageSize));
        }


        //
        // GET: /Quiz/Details/5
        public ActionResult Details(int id, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            Quiz quiz = unitOfWork.QuizzesRepository.GetByID(id);
            if (quiz.UserID != CurrentUser.ID)
            {
                return RedirectToAction("Index");
            }
            return View(quiz);
        }

        //
        // GET: /Quiz/Create
        public ActionResult Create()
        {
            Quiz quiz = new Quiz();
            ViewBag.QuestionCategories = unitOfWork.QuestionCategoriesRepository.Get().Where(qc => qc.UserID == CurrentUser.ID).ToList();
            return View(quiz);
        }

        //
        // POST: /Quiz/Create
        [HttpPost]
        public ActionResult Create(Quiz quiz, FormCollection formCollection, string[] selectedqItems)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (selectedqItems != null)
                    {
                        foreach (var item in selectedqItems)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                int itemid = 0;
                                int.TryParse(item, out itemid);
                                if (itemid != 0)
                                {
                                    Question q = unitOfWork.QuestionsRepository.Get().Where(i => i.ID == itemid && i.UserID == CurrentUser.ID).FirstOrDefault();
                                    if (q != null)
                                    {
                                        quiz.QuizDetails.Add(new QuizDetail() { Quiz = quiz, QuestionID = q.ID });
                                    }
                                }
                            }
                        }
                    }

                    foreach (string key in formCollection.Keys)
                    {
                        if (key.StartsWith("qc"))
                        {
                            int itemid = 0;
                            int.TryParse(key.Substring(2), out itemid);

                            QuestionCategory q = unitOfWork.QuestionCategoriesRepository.Get().Where(i => i.ID == itemid && i.UserID == CurrentUser.ID).FirstOrDefault();
                            if (q != null)
                            {
                                int number = 0;
                                int.TryParse(formCollection[key], out number);
                                if (number != 0)
                                {
                                    quiz.QuizDetails.Add(new QuizDetail() { Quiz = quiz, QuestionCategoryID = q.ID, Number = number });
                                }
                            }
                        }
                    }
                    
                    quiz.User = unitOfWork.UsersRepository.Get().Where(u => u.Login == HttpContext.User.Identity.Name).FirstOrDefault();
                    unitOfWork.QuizzesRepository.Add(quiz);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                //Log the error (add a variable name after DataException)
                ModelState.AddModelError("", Resource.UnableToSave);
            }
            ViewBag.QuestionCategories = unitOfWork.QuestionCategoriesRepository.Get().Where(qc => qc.UserID == CurrentUser.ID).ToList();
            return View(quiz);
        }


        //
        // GET: /Quiz/Edit/5

        public ActionResult Edit(int id, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            ViewBag.QuestionCategories = unitOfWork.QuestionCategoriesRepository.Get().Where(qc => qc.UserID == CurrentUser.ID).ToList();

            Quiz quiz = unitOfWork.QuizzesRepository.GetByID(id);
            if (quiz.UserID != CurrentUser.ID)
            {
                return RedirectToAction("Index");
            }
            return View(quiz);
        }

        //
        // POST: /Quiz/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection formCollection, string sortOrder, string currentFilter, int? page, string[] selectedqItems)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
          
            var quizToUpdate = unitOfWork.QuizzesRepository.GetByID(id);
            if (quizToUpdate.UserID != CurrentUser.ID)
            {
                return RedirectToAction("Index");
            }
            if (TryUpdateModel(quizToUpdate, ""))
            {
                try
                {
                    quizToUpdate.QuizDetails.ToList().ForEach(qd => unitOfWork.QuizDetailsRepository.Delete(qd));
                    quizToUpdate.QuizDetails.Clear();
                    if (selectedqItems !=null)
                    {
                        foreach (var item in selectedqItems)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                int itemid = 0;
                                int.TryParse(item, out itemid);
                                if (itemid != 0)
                                {
                                    Question q = unitOfWork.QuestionsRepository.Get().Where(i => i.ID == itemid && i.UserID == CurrentUser.ID).FirstOrDefault();
                                    if (q != null)
                                    {
                                        quizToUpdate.QuizDetails.Add(new QuizDetail() { Quiz = quizToUpdate, QuestionID = q.ID });
                                    }
                                }
                            }
                        }
                    }

                    foreach (string key in formCollection.Keys) 
                    {
                        if (key.StartsWith("qc")) 
                        {
                            int itemid = 0;
                            int.TryParse(key.Substring(2), out itemid);

                            QuestionCategory q = unitOfWork.QuestionCategoriesRepository.Get().Where(i => i.ID == itemid && i.UserID == CurrentUser.ID).FirstOrDefault();
                            if (q != null)
                            {
                                int number = 0;
                                int.TryParse(formCollection[key], out number);
                                if (number != 0)
                                {
                                    quizToUpdate.QuizDetails.Add(new QuizDetail() { Quiz = quizToUpdate, QuestionCategoryID = q.ID, Number = number });
                                }
                            }
                        }
                    }

                    unitOfWork.QuizzesRepository.Update(quizToUpdate);
                    unitOfWork.Save();

                    return RedirectToAction("Index", new { sortOrder = sortOrder, currentFilter = currentFilter, page = page });
                }
                catch (DataException)
                {
                    //Log the error (add a variable name after DataException)
                    ModelState.AddModelError("", Resource.UnableToSave);
                }
            }
            ViewBag.QuestionCategories = unitOfWork.QuestionCategoriesRepository.Get().Where(qc => qc.UserID == CurrentUser.ID).ToList();
            return View(quizToUpdate);
        }

        //
        // GET: /Quiz/Delete/5
        public ActionResult Delete(int id, bool? deleteError, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            if (deleteError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = Resource.UnableToSave;
            }
            Quiz quiz = unitOfWork.QuizzesRepository.GetByID(id);
            if (quiz.UserID != CurrentUser.ID)
            {
                return RedirectToAction("Index");
            }
            return View(quiz);
        }


        //
        // POST: /Quiz/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id, string sortOrder, string currentFilter, int? page)
        {

            try
            {
                Quiz quiz = unitOfWork.QuizzesRepository.GetByID(id);
                if (quiz.UserID != CurrentUser.ID)
                {
                    return RedirectToAction("Index");
                }
                unitOfWork.QuizzesRepository.Delete(id);

                unitOfWork.Save();

                return RedirectToAction("Index", new { sortOrder = sortOrder, currentFilter = currentFilter, page = page });
            }
            catch (DataException)
            {
                //Log the error (add a variable name after DataException)
                return RedirectToAction("Delete",
                    new System.Web.Routing.RouteValueDictionary { 
                { "id", id }, 
                { "deleteError", true } });
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}