using System.Data;
using System.Web.Mvc;
using QuizWeb.Data;
using QuizWeb.Models;
using Resources;

namespace QuizWeb.Controllers
{
    [Authorize]
    public class AnswerController : BaseController
    {


        public AnswerController(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {           
        }


       

        //
        // GET: /Answer/Details/5
        public ViewResult Details(int id, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            Answer answer = unitOfWork.AnswersRepository.GetByID(id);
            return View(answer);
        }

        //
        // GET: /Answer/ToggleNotAnswer
        public ActionResult ToggleNotAnswer(int id, string sortOrder, string currentFilter, int? page)
        {
            Answer answer = unitOfWork.AnswersRepository.GetByID(id);
            answer.IsAnswer = false;
            unitOfWork.AnswersRepository.Update(answer);
            unitOfWork.Save();
            return RedirectToAction("Index", "Question", new { sortOrder = sortOrder, currentFilter = currentFilter, page = page });
        }
        //
        // GET: /Answer/ToggleIsAnswer
        public ActionResult ToggleIsAnswer(int id, string sortOrder, string currentFilter, int? page)
        {
            Answer answer = unitOfWork.AnswersRepository.GetByID(id);
            answer.IsAnswer = true;
            unitOfWork.AnswersRepository.Update(answer);
            unitOfWork.Save();
            return RedirectToAction("Index", "Question", new { sortOrder = sortOrder, currentFilter = currentFilter, page = page });
        }
        //
        // GET: /Answer/Create
        public ActionResult Create(int questionID, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            Answer answer = new Answer();
            answer.QuestionID = questionID;
            return View(answer);
        }

        //
        // POST: /Answer/Create
        [HttpPost]
        public ActionResult Create(Answer answer, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            try
            {
                if (ModelState.IsValid)
                {
                   
                    unitOfWork.AnswersRepository.Add(answer);
                    unitOfWork.Save();
                    return RedirectToAction("Index", "Question", new { sortOrder=sortOrder, currentFilter =currentFilter, page=page });
                }
            }
            catch (DataException)
            {
                //Log the error (add a variable name after DataException)
                ModelState.AddModelError("", Resource.UnableToSave);
            }

            return View(answer);
        }


        //
        // GET: /Answer/Edit/5

        public ActionResult Edit(int id, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            Answer answer = unitOfWork.AnswersRepository.GetByID(id);
            return View(answer);
        }

        //
        // POST: /Answer/Edit/5
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, FormCollection formCollection, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            var answerToUpdate = unitOfWork.AnswersRepository.GetByID(id);
            if (TryUpdateModel(answerToUpdate, ""))
            {
                try
                {
                    

                    unitOfWork.AnswersRepository.Update(answerToUpdate);
                    unitOfWork.Save();

                    return RedirectToAction("Index", "Question", new { sortOrder = sortOrder, currentFilter = currentFilter, page = page });
                }
                catch (DataException)
                {
                    //Log the error (add a variable name after DataException)
                    ModelState.AddModelError("", Resource.UnableToSave);
                }
            }

            return View(answerToUpdate);
        }

        //
        // GET: /Answer/Delete/5
        public ActionResult Delete(int id, bool? deleteError, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            if (deleteError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = Resource.UnableToSave;
            }
            Answer answer = unitOfWork.AnswersRepository.GetByID(id);
            return View(answer);
        }


        //
        // POST: /Answer/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id, string sortOrder, string currentFilter, int? page)
        {

            try
            {
                Answer answer = unitOfWork.AnswersRepository.GetByID(id);

                unitOfWork.AnswersRepository.Delete(id);

                unitOfWork.Save();

                return RedirectToAction("Index","Question", new { sortOrder = sortOrder, currentFilter = currentFilter, page = page });
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