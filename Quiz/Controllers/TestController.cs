using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Collections;
using QuizWeb.Data;
using QuizWeb.Misc;
using QuizWeb.Models;
using Resources;
using PagedList;

namespace QuizWeb.Controllers
{
    [Authorize]
    public class TestController : BaseController
    {


        public TestController(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        //
        // GET: /Test/
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "LastName desc" : "";


            if (Request.HttpMethod == "GET")
            {
                searchString = currentFilter;
            }
            else
            {
                page = 1;
            }
            ViewBag.CurrentFilter = searchString;

            var tests = from s in unitOfWork.TestsRepository.Get()
                        where s.Quiz.UserID == CurrentUser.ID
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                tests = tests.Where(s => s.LastName.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "LastName desc":
                    tests = tests.OrderByDescending(s => s.LastName);
                    break;
                default:
                    tests = tests.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = Constants.ITEMS_PER_PAGE;
            int pageNumber = (page ?? 1);
            return View(tests.ToPagedList(pageNumber, pageSize));
        }


        //
        // GET: /Test/Details/5
        public ActionResult Details(int id, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            Test test = unitOfWork.TestsRepository.GetByID(id);
            Hashtable question = new Hashtable();
            foreach (TestDetail testDetail in test.TestDetails)
            {
                question[testDetail.ID] = unitOfWork.QuestionsRepository.GetByID(testDetail.QuestionID);
            }
            ViewBag.Question = question;
            if (test.Quiz.UserID != CurrentUser.ID)
            {
                return RedirectToAction("Index");
            }
            return View(test);
        }

        //
        // GET: /Test/Create
        public ActionResult Create()
        {
            Test test = new Test();
            PopulateQuizzesDropDownList();
            return View(test);
        }

        //
        // POST: /Test/Create
        [HttpPost]
        public ActionResult Create(Test test)
        {
            try
            {
                if (ModelState.IsValid)
                {
                   
                    unitOfWork.TestsRepository.Add(test);
                    unitOfWork.Save();
                    //Generate Testdetails
                    Quiz quiz = unitOfWork.QuizzesRepository.GetByID(test.QuizID);
                    //search other test with the same quiz that already has testdetails
                    Test testtocopy = unitOfWork.TestsRepository.Get().Where(t => t.QuizID == quiz.ID && t.TestDetails.Count > 0).FirstOrDefault();

                    QuizWeb.Misc.Misc.GenerateTest(test, quiz, testtocopy, unitOfWork);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                //Log the error (add a variable name after DataException)
                ModelState.AddModelError("", Resource.UnableToSave);
            }
            PopulateQuizzesDropDownList(test.QuizID);
            return View(test);
        }

      

       
        //
        // GET: /Test/Edit/5

        public ActionResult Edit(int id, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            Test test = unitOfWork.TestsRepository.GetByID(id);
            if (test.Quiz.UserID != CurrentUser.ID)
            {
                return RedirectToAction("Index");
            }
            PopulateQuizzesDropDownList(test.QuizID);
            return View(test);
        }

        //
        // POST: /Test/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection formCollection, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            var testToUpdate = unitOfWork.TestsRepository.GetByID(id);
            if (testToUpdate.Quiz.UserID != CurrentUser.ID)
            {
                return RedirectToAction("Index");
            }
            if (TryUpdateModel(testToUpdate, ""))
            {
                try
                {
                  
                    unitOfWork.TestsRepository.Update(testToUpdate);
                    unitOfWork.Save();

                    return RedirectToAction("Index", new { sortOrder = sortOrder, currentFilter = currentFilter, page = page });
                }
                catch (DataException)
                {
                    //Log the error (add a variable name after DataException)
                    ModelState.AddModelError("", Resource.UnableToSave);
                }
            }
            PopulateQuizzesDropDownList(testToUpdate.QuizID);
            return View(testToUpdate);
        }

        //
        // GET: /Test/Delete/5
        public ActionResult Delete(int id, bool? deleteError, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            if (deleteError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = Resource.UnableToSave;
            }
            Test test = unitOfWork.TestsRepository.GetByID(id);
            if (test.Quiz.UserID != CurrentUser.ID)
            {
                return RedirectToAction("Index");
            }
            return View(test);
        }


        //
        // POST: /Test/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id, string sortOrder, string currentFilter, int? page)
        {

            try
            {
                Test test = unitOfWork.TestsRepository.GetByID(id);
         
                unitOfWork.TestsRepository.Delete(id);

                unitOfWork.Save();

                return RedirectToAction("Index", new { sortOrder = sortOrder, currentFilter = currentFilter, page=page });
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
        private void PopulateQuizzesDropDownList(object selectedQuiz = null)
        {
            var quizzes = unitOfWork.QuizzesRepository.Get().Where(qc => qc.UserID == CurrentUser.ID);
            ViewBag.QuizID = new SelectList(quizzes, "ID", "Name", selectedQuiz);
        }
    }
}