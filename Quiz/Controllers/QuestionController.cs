using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using QuizWeb.Data;
using QuizWeb.Misc;
using QuizWeb.Models;
using Resources;
using PagedList;
using CsvHelper;
using LinqToExcel;

namespace QuizWeb.Controllers
{
    [Authorize]
    public class QuestionController : BaseController
    {


        public QuestionController(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }


        //
        // GET: /Question/
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DescriptionSortParm = String.IsNullOrEmpty(sortOrder) ? "Description desc" : "";


            if (Request.HttpMethod == "GET")
            {
                searchString = currentFilter;
            }
            else
            {
                page = 1;
            }
            ViewBag.CurrentFilter = searchString;

            var questions = from s in unitOfWork.QuestionsRepository.Get() where s.UserID==CurrentUser.ID
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                questions = questions.Where(s => s.Description.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Name desc":
                    questions = questions.OrderByDescending(s => s.Description);
                    break;
                default:
                    questions = questions.OrderBy(s => s.Description);
                    break;
            }

            int pageSize = Constants.ITEMS_PER_PAGE;
            int pageNumber = (page ?? 1);
            return View(questions.ToPagedList(pageNumber, pageSize));
        }


        //
        // GET: /Question/Details/5
        public ActionResult Details(int id, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            Question question = unitOfWork.QuestionsRepository.GetByID(id);
            if (question.UserID != CurrentUser.ID)
            {
                return RedirectToAction("Index");
            }
            return View(question);
        }

        //
        // GET: /Question/Create
        public ActionResult Create()
        {

            Question question = new Question();
            ViewBag.QuestionCategories = unitOfWork.QuestionCategoriesRepository.Get().Where(qc => qc.UserID == CurrentUser.ID).ToList().OrderBy(qc => qc.HierarchyPath);
            return View(question);
        }

        //
        // POST: /Question/Create
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Question question, string[] selectedItems)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (selectedItems != null)
                    {
                        foreach (var item in selectedItems)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                int itemid = 0;
                                int.TryParse(item, out itemid);
                                if (itemid != 0)
                                {
                                    QuestionCategory qc = unitOfWork.QuestionCategoriesRepository.Get().Where(i => i.UserID == CurrentUser.ID && i.ID == itemid).FirstOrDefault();
                                    if (qc != null)
                                    {
                                        question.QuestionCategories.Add(qc);
                                    }
                                }
                            }
                        }
                    }
                    else 
                    {
                        ModelState.AddModelError("selectedItems", Resource.MandatoryQuestionCategory);
                        ViewBag.QuestionCategories = unitOfWork.QuestionCategoriesRepository.Get().Where(qc => qc.UserID == CurrentUser.ID).ToList().OrderBy(qc => qc.HierarchyPath);
                        return View(question);
                    }
                    question.User = unitOfWork.UsersRepository.Get().Where(u => u.Login == HttpContext.User.Identity.Name).FirstOrDefault();
                    unitOfWork.QuestionsRepository.Add(question);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                //Log the error (add a variable name after DataException)
                ModelState.AddModelError("", Resource.UnableToSave);
            }
            ViewBag.QuestionCategories = unitOfWork.QuestionCategoriesRepository.Get().Where(qc => qc.UserID == CurrentUser.ID).ToList().OrderBy(qc => qc.HierarchyPath);
            return View(question);
        }


        //
        // GET: /Question/Edit/5

        public ActionResult Edit(int id, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;

            ViewBag.QuestionCategories = unitOfWork.QuestionCategoriesRepository.Get().Where(qc => qc.UserID == CurrentUser.ID).ToList().OrderBy(qc => qc.HierarchyPath);
            Question question = unitOfWork.QuestionsRepository.GetByID(id);
            if (question.UserID != CurrentUser.ID)
            {
                return RedirectToAction("Index");
            }
            return View(question);
        }

        //
        // POST: /Question/Edit/5
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, FormCollection formCollection, string sortOrder, string currentFilter, int? page, string[] selectedItems)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;

            ViewBag.QuestionCategories = unitOfWork.QuestionCategoriesRepository.Get().Where(qc => qc.UserID == CurrentUser.ID).ToList().OrderBy(qc => qc.HierarchyPath);
            var questionToUpdate = unitOfWork.QuestionsRepository.GetByID(id);
            if (questionToUpdate.UserID != CurrentUser.ID)
            {
                return RedirectToAction("Index");
            }
            if (TryUpdateModel(questionToUpdate, ""))
            {
               
                try
                {
                    questionToUpdate.QuestionCategories.Clear();
                    foreach (var item in selectedItems)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            int itemid = 0;
                            int.TryParse(item, out itemid);
                            if (itemid != 0)
                            {
                                QuestionCategory qc = unitOfWork.QuestionCategoriesRepository.Get().Where(i => i.ID == itemid && i.UserID == CurrentUser.ID).FirstOrDefault();
                                if (qc != null)
                                {
                                    questionToUpdate.QuestionCategories.Add(qc);
                                }
                            }
                        }
                    }

                    unitOfWork.QuestionsRepository.Update(questionToUpdate);
                    unitOfWork.Save();

                    return RedirectToAction("Index", new { sortOrder = sortOrder, currentFilter = currentFilter, page = page });
                }
                catch (DataException)
                {
                    //Log the error (add a variable name after DataException)
                    ModelState.AddModelError("", Resource.UnableToSave);
                }
            }

            return View(questionToUpdate);
        }

        //
        // GET: /Question/Delete/5
        public ActionResult Delete(int id, bool? deleteError, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            if (deleteError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = Resource.UnableToSave;
            }
            Question question = unitOfWork.QuestionsRepository.GetByID(id);
            if (question.UserID != CurrentUser.ID)
            {
                return RedirectToAction("Index");
            }
            return View(question);
        }


        //
        // POST: /Question/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id, string sortOrder, string currentFilter, int? page)
        {

            try
            {
                Question question = unitOfWork.QuestionsRepository.GetByID(id);
                if (question.UserID != CurrentUser.ID)
                {
                    return RedirectToAction("Index");
                }
                unitOfWork.QuestionsRepository.Delete(id);

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


        public ActionResult Export()
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            var csv = new CsvWriter(sw);
            csv.Configuration.Delimiter = ',';
            csv.Configuration.QuoteAllFields = true;
            csv.Configuration.Encoding = System.Text.Encoding.UTF8; 
            //header
            csv.WriteField("ID");
            csv.WriteField("Categories");
            csv.WriteField("RightAnswer");
            csv.WriteField("Details");
            csv.WriteField("Description");
            int maxAnswerCount = unitOfWork.QuestionsRepository.Get().Where(s=>s.UserID==CurrentUser.ID).Max(q => q.Answers.Count());
            for(int i=0;i<maxAnswerCount;i++)
            {
                 csv.WriteField( "Answer" + i );
            }
            csv.NextRecord();

            foreach (var item in unitOfWork.QuestionsRepository.Get().Where(s => s.UserID == CurrentUser.ID).ToList())
            {
                csv.WriteField(item.ID.ToString());
                csv.WriteField(item.QuestionCategories.Select(qc => qc.FullHierarchyPath).Count()>0? item.QuestionCategories.Select(qc => qc.FullHierarchyPath).Aggregate((c, n) => c + ", " + n):"");
                csv.WriteField(item.Answers.Select((s, i) => new { Index = i, IsAnswer = s.IsAnswer }).Where(a => a.IsAnswer == true).Select(a => a.Index.ToString()).Count() > 0 ? item.Answers.Select((s, i) => new { Index = i, IsAnswer = s.IsAnswer }).Where(a => a.IsAnswer == true).Select(a => a.Index.ToString()).Aggregate((c, n) => c + ", " + n) : "");
                csv.WriteField(item.Details);
                csv.WriteField(item.Description);
                for (int i = 0; i < maxAnswerCount; i++)
                {
                    csv.WriteField(i<item.Answers.Count()?item.Answers.ToList()[i].Description:"");
                }
                csv.NextRecord();
            }
            sw.Flush();
            ms.Position = 0;
            return File(ms, "text/csv", "data.csv");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        //private ArrayList GetNodes(IEnumerable<QuestionCategory> list, Question question) 
        //{
        //    ArrayList nodes = new ArrayList();
        //    foreach (var item in list)
        //    {
        //        if (item.ChildCategories.Count > 0)
        //        {
        //            nodes.Add(new
        //            {
        //                key = item.ID.ToString(),
        //                title = item.Name,
        //                isFolder = true,
        //                children = GetNodes(item.ChildCategories.ToList(), question),
        //                select = question!=null?question.QuestionCategories.Contains(item):false
        //            });
        //        }
        //        else
        //        {
        //            nodes.Add(new
        //            {
        //                key = item.ID.ToString(),
        //                title = item.Name,
        //                children = new ArrayList(),
        //                select = question != null ? question.QuestionCategories.Contains(item) : false
        //            });
        //        }
        //    }
        //    return nodes;
        //}
        //public ActionResult GetTopLevelNodesAsJson(int questionID)
        //{
        //    Question question=unitOfWork.QuestionsRepository.Get().Where(s => s.UserID == CurrentUser.ID && s.ID == questionID).FirstOrDefault();
        //    return Json(GetNodes(unitOfWork.QuestionCategoriesRepository.Get().Where(s => s.UserID == CurrentUser.ID && s.ParentCategory==null).ToList(), question).ToArray());
        //}


        //
        // GET: /Question/Import
        public ActionResult Import(string message)
        {
            ViewBag.Message=message;
            return View();
        }

        //
        // POST: /Question/Import
        [HttpPost]
        public ActionResult Import(HttpPostedFileBase importfile)
        {
            if (importfile.ContentLength > 0)
            {
                switch (Path.GetExtension(importfile.FileName))
                {
                    case ".csv":ImportFromCVS(importfile); break;
                    case ".xlsx":
                    case ".xls": ImportFromXSL(importfile); break;
                    default: return RedirectToAction("Import", new {message=Resource.InvalidExtension});
                }

            }
            return RedirectToAction("Index");
        }
        private void ImportFromXSL(HttpPostedFileBase importfile)
        {
            string fileName = System.IO.Path.GetTempFileName();
            try
            {
                CleanData();


                importfile.SaveAs(fileName);
                var excel = new ExcelQueryFactory(fileName);


                foreach (var record in from i in excel.WorksheetNoHeader(0) select i)
                 {
                     var myrecord = record.Select(c => c.Value.ToString()).ToArray();
                     ParseRow(myrecord);
                     unitOfWork.Save();
                 }

 
            }finally
            {
                System.IO.File.Delete(fileName);
            }

           
        }

        private void CleanData()
        {
            //drop all questionCategories

            foreach (var q in unitOfWork.QuizzesRepository.Get().Where(s => s.UserID == CurrentUser.ID))
            {
                unitOfWork.QuizzesRepository.Delete(q);
            }
            unitOfWork.Save();
            foreach (var q in unitOfWork.QuestionsRepository.Get().Where(s => s.UserID == CurrentUser.ID))
            {
                unitOfWork.QuestionsRepository.Delete(q);
            }
            unitOfWork.Save();
            foreach (var qc in unitOfWork.QuestionCategoriesRepository.Get().Where(s => s.UserID == CurrentUser.ID))
            {
                unitOfWork.QuestionCategoriesRepository.Delete(qc);
            }
            unitOfWork.Save();
        }
        private void ImportFromCVS(HttpPostedFileBase importfile)
        {
            CleanData();



            var csv = new CsvReader(new StreamReader(importfile.InputStream));

            csv.Read();
            while (csv.CurrentRecord != null)
            {
                string[] record = csv.CurrentRecord;

                ParseRow(record);
                csv.Read();
                unitOfWork.Save();
            }
           

        }

        private void ParseRow(string[] record)
        {
            if (!string.IsNullOrEmpty(record[0]))
            {
                Question q = new Question() { UserID = CurrentUser.ID, Details = record[3], Description = record[4] };
                unitOfWork.QuestionsRepository.Add(q);


                string[] categories = ((string)record[1]).Split(',');
                foreach (var category in categories)
                {
                    QuestionCategory qc = unitOfWork.QuestionCategoriesRepository.Get().Where(s => s.UserID == CurrentUser.ID).ToList().Where(i => i.FullHierarchyPath == category).FirstOrDefault();
                    if (qc == null)
                    {
                        string[] categoriesfrompath = category.Split('/');
                        string currentpath = "";
                        foreach (var categoryfrompath in categoriesfrompath)
                        {
                            if (!string.IsNullOrEmpty(categoryfrompath))
                            {
                                currentpath += "/" + categoryfrompath;
                                QuestionCategory qcp = unitOfWork.QuestionCategoriesRepository.Get().Where(s => s.UserID == CurrentUser.ID).ToList().Where(i => i.FullHierarchyPath == currentpath).FirstOrDefault();
                                if (qcp == null)
                                {
                                    qcp = new QuestionCategory() { ParentCategory = qc, Name = categoryfrompath, UserID = CurrentUser.ID };
                                    unitOfWork.QuestionCategoriesRepository.Add(qcp);
                                }
                                qc = qcp;
                            }
                        }
                    }
                    qc.Questions.Add(q);
                }

                for (int i = 5; i < record.Length; i++)
                {
                    if (!string.IsNullOrEmpty(record[i]))
                    {
                        Answer a = new Answer() { Description = record[i], IsAnswer = ((string)record[2]).Replace(" ", "").Split(',').Contains((i - 5).ToString()), Question = q };
                        unitOfWork.AnswersRepository.Add(a);
                    }
                }
            }
        }
    
    }
}