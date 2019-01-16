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
    public class QuestionCategoryController : BaseController
    {


        public QuestionCategoryController(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }


        //
        // GET: /QuestionCategory/
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

            var questionCategorys = from s in unitOfWork.QuestionCategoriesRepository.Get()
                                    where s.UserID == CurrentUser.ID
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                questionCategorys = questionCategorys.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Name desc":
                    questionCategorys = questionCategorys.OrderByDescending(s => s.Name);
                    break;
                default:
                    questionCategorys = questionCategorys.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = Constants.ITEMS_PER_PAGE;
            int pageNumber = (page ?? 1);
            return View(questionCategorys.ToPagedList(pageNumber, pageSize));
        }


        //
        // GET: /QuestionCategory/Details/5
        public ActionResult Details(int id, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            QuestionCategory questionCategory = unitOfWork.QuestionCategoriesRepository.GetByID(id);
            if (questionCategory.UserID != CurrentUser.ID)
            {
                return RedirectToAction("Index");
            }
            return View(questionCategory);
        }

        //
        // GET: /QuestionCategory/Create
        public ActionResult Create()
        {
            PopulateQuestionCategoriesDropDownList();
            QuestionCategory questionCategory = new QuestionCategory();
            return View(questionCategory);
        }

        //
        // POST: /QuestionCategory/Create
        [HttpPost]
        public ActionResult Create(QuestionCategory questionCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    questionCategory.User = unitOfWork.UsersRepository.Get().Where(u => u.Login == HttpContext.User.Identity.Name).FirstOrDefault();
                    unitOfWork.QuestionCategoriesRepository.Add(questionCategory);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                //Log the error (add a variable name after DataException)
                ModelState.AddModelError("", Resource.UnableToSave);
            }
            PopulateQuestionCategoriesDropDownList(questionCategory.ParentCategoryID);
            return View(questionCategory);
        }


        //
        // GET: /QuestionCategory/Edit/5

        public ActionResult Edit(int id, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            
            QuestionCategory questionCategory = unitOfWork.QuestionCategoriesRepository.GetByID(id);
            if (questionCategory.UserID != CurrentUser.ID)
            {
                return RedirectToAction("Index");
            }
            PopulateQuestionCategoriesDropDownList(questionCategory.ParentCategoryID);
            return View(questionCategory);
        }

        //
        // POST: /QuestionCategory/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection formCollection, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            var questionCategoryToUpdate = unitOfWork.QuestionCategoriesRepository.GetByID(id);
            if (questionCategoryToUpdate.UserID != CurrentUser.ID)
            {
                return RedirectToAction("Index");
            }
            if (TryUpdateModel(questionCategoryToUpdate, ""))
            {
                try
                {

                    unitOfWork.QuestionCategoriesRepository.Update(questionCategoryToUpdate);
                    unitOfWork.Save();

                    return RedirectToAction("Index", new { sortOrder = sortOrder, currentFilter = currentFilter, page = page });
                }
                catch (DataException)
                {
                    //Log the error (add a variable name after DataException)
                    ModelState.AddModelError("", Resource.UnableToSave);
                }
            }
            PopulateQuestionCategoriesDropDownList(questionCategoryToUpdate.ParentCategoryID);
            return View(questionCategoryToUpdate);
        }

        //
        // GET: /QuestionCategory/Delete/5
        public ActionResult Delete(int id, bool? deleteError, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            if (deleteError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = Resource.UnableToSave;
            }
            QuestionCategory questionCategory = unitOfWork.QuestionCategoriesRepository.GetByID(id);
            if (questionCategory.UserID != CurrentUser.ID)
            {
                return RedirectToAction("Index");
            }
            return View(questionCategory);
        }


        //
        // POST: /QuestionCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id, string sortOrder, string currentFilter, int? page)
        {

            try
            {
                QuestionCategory questionCategory = unitOfWork.QuestionCategoriesRepository.GetByID(id);
                if (questionCategory.UserID != CurrentUser.ID)
                {
                    return RedirectToAction("Index");
                }
                unitOfWork.QuestionCategoriesRepository.Delete(id);

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
        private void PopulateQuestionCategoriesDropDownList(object selectedQuestionCategory = null)
        {
            var questionCategories = unitOfWork.QuestionCategoriesRepository.Get().Where(qc=>qc.UserID==CurrentUser.ID);
            ViewBag.ParentCategoryID = new SelectList(questionCategories, "ID", "Name", selectedQuestionCategory);
        }
       
    }
}