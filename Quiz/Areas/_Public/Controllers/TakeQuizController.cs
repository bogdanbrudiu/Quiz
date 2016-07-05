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
using Resources;
using System.Data;
using QuizWeb.Areas._Public.ViewModels;

namespace QuizWeb.Areas._Public.Controllers
{
    public class TakeQuizController : BaseController
    {


        public TakeQuizController(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }


        //
        // GET: /_Public/TakeQuiz/5
        public ActionResult Index(int id)
        {
            Test test = new Test();
            test.QuizID = id;
            Quiz quiz = unitOfWork.QuizzesRepository.GetByID(id);
            return View(test);
        }

        //
        // POST: /_Public/TakeQuiz/Index
        [HttpPost]
        public ActionResult Index(Test test)
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
                    return RedirectToAction("AnswerQuestion", new { id=test.ID});
                }
            }
            catch (DataException)
            {
                //Log the error (add a variable name after DataException)
                ModelState.AddModelError("", Resource.UnableToSave);
            }
    
            return View(test);
        }

        //
        // GET: /_Public/TakeQuiz/ContinueTest
        public ActionResult ContinueTest()
        {
            return View();
        }

        //
        // POST: /_Public/TakeQuiz/ContinueTest
        [HttpPost]
        public ActionResult ContinueTest(int id)
        {
            Test test = unitOfWork.TestsRepository.GetByID(id);
            if (test != null) 
            {
                return RedirectToAction("ConfirmContinueTest", new { id = id });
            }
           ModelState.AddModelError("ID",Resources.Resource.TestIDNotFound);

            return View();
        }

        //
        // GET: /_Public/TakeQuiz/ContinueTest
        public ActionResult ConfirmContinueTest(int id)
        {
            Test test = unitOfWork.TestsRepository.GetByID(id);
            if (test == null)
            {
                return RedirectToAction("ContinueTest");
            }
            return View(new ConfirmContinueTestModel() { Test=test});
        }

        //
        // POST: /_Public/TakeQuiz/ContinueTest
        [HttpPost]
        public ActionResult ConfirmContinueTest(ConfirmContinueTestModel confirmContinueTestModel)
        {
            if (confirmContinueTestModel.Test == null)
            {
                return RedirectToAction("ContinueTest");
            }
            confirmContinueTestModel.Test = unitOfWork.TestsRepository.GetByID(confirmContinueTestModel.Test.ID);
            if (unitOfWork.UsersRepository.Get().Where(u => u.Login == confirmContinueTestModel.UserName && u.Password == confirmContinueTestModel.Password).Count() == 1 && confirmContinueTestModel.Test.Quiz.User.Login==confirmContinueTestModel.UserName)
            {
                return RedirectToAction("AnswerQuestion", new { id=confirmContinueTestModel.Test.ID});
            }
            ModelState.AddModelError("UserName", Resources.Resource.BadUserNameOrPassword);

            return View(confirmContinueTestModel);
        }
        
        //
        // GET: /_Public/TakeQuiz/AnswerQuestion/5
        public ActionResult AnswerQuestion(int id, int? position)
        {
            Test test = unitOfWork.TestsRepository.GetByID(id);
            if (test.Finished)
            {
                TempData["ID"] = test.ID;
                return RedirectToAction("Results");
            }
            TestDetail testDetail;
            if (position.HasValue && position.Value >= 0 && position.Value < test.TestDetails.Count && test.Quiz.AllowNavigate)
            {
                testDetail = test.TestDetails.ToList()[position.Value]; 
            }
            else
            {
                testDetail = test.TestDetails.Where(t => t.Answered == false).OrderBy(t => t.ID).FirstOrDefault();
                if (testDetail == null)
                {
                    return RedirectToAction("TestDone", new { id = test.ID });
                }
            }
            int myposition = test.TestDetails.ToList().IndexOf(testDetail);
            ViewBag.Position = myposition;
            ViewBag.NotFirst = myposition != 0;
            ViewBag.NotLast = myposition != test.TestDetails.Count - 1;
            ViewBag.Test = test;
            ViewBag.Done = test.TestDetails.Where(t => t.Answered == false).Count() == 0;

            ViewBag.Question = unitOfWork.QuestionsRepository.GetByID(testDetail.QuestionID);
            return View(testDetail);
        }

        //
        // POST: /_Public/TakeQuiz/AnswerQuestion/Index
        [HttpPost]
        public ActionResult AnswerQuestion(int ID, FormCollection formCollection)
        {
             var testDetailToUpdate = unitOfWork.TestDetailsRepository.GetByID(ID);

             if (TryUpdateModel(testDetailToUpdate, ""))
             {
                 try
                 {
                     if (ModelState.IsValid)
                     {
                         testDetailToUpdate.Answered = true;
                         testDetailToUpdate.AnsweredOn = DateTime.Now;
                         testDetailToUpdate.ResponsesArray = "";
                         if (!string.IsNullOrEmpty(formCollection["ResponseID[]"]))
                         {
                             foreach (string item in formCollection["ResponseID[]"].Split(','))
                             {
                                 int id = 0;
                                 int.TryParse(item, out id);
                                 if (id != 0)
                                 {
                                     Answer answer = unitOfWork.AnswersRepository.GetByID(id);
                                     if (answer != null)
                                     {
                                         var newlist = testDetailToUpdate.Responses.ToList<string>();
                                         newlist.Add(answer.Description);
                                         testDetailToUpdate.Responses = newlist;
                                     }
                                 }
                             }
                         }
                         int count = 0;
                         bool containsAllAnswers = true;
                         foreach (Answer answer in unitOfWork.QuestionsRepository.GetByID(testDetailToUpdate.QuestionID).Answers.Where(a=>a.IsAnswer==true)) 
                         {
                             count++;
                             if (!testDetailToUpdate.Responses.Contains(answer.Description)) 
                             {
                                 containsAllAnswers = false;
                             }
 
                         }
                         testDetailToUpdate.IsAnswer = (count == testDetailToUpdate.Responses.Count()) && containsAllAnswers;
                         unitOfWork.TestDetailsRepository.Update(testDetailToUpdate);
                         unitOfWork.Save();
                         return RedirectToAction("AnswerQuestion", new { id = testDetailToUpdate.TestID });
                     }
                 }
                 catch (DataException)
                 {
                     //Log the error (add a variable name after DataException)
                     ModelState.AddModelError("", Resource.UnableToSave);
                 }
             }
             int myposition = testDetailToUpdate.Test.TestDetails.ToList().IndexOf(testDetailToUpdate);
            ViewBag.Position = myposition;
            ViewBag.NotFirst = myposition != 0;
            ViewBag.NotLast = myposition != testDetailToUpdate.Test.TestDetails.Count - 1;
            ViewBag.Test = testDetailToUpdate.Test;

            ViewBag.Question = unitOfWork.QuestionsRepository.GetByID(testDetailToUpdate.QuestionID);
            return View(testDetailToUpdate);
        }

        //
        // GET: /_Public/TakeQuiz/TestDone/5
        public ActionResult TestDone(int id)
        {
            Test test = unitOfWork.TestsRepository.GetByID(id);
            ViewBag.Position = test.TestDetails.Count - 1;
            ViewBag.Test = test;
            return View(test);
        }

        //
        // POST: /_Public/TakeQuiz/TestDone/5
        [HttpPost]
        public ActionResult TestDone(Test testdone)
        {
            Test test = unitOfWork.TestsRepository.GetByID(testdone.ID);
            test.Finished = true;
            unitOfWork.TestsRepository.Update(test);
            unitOfWork.Save();
            ViewBag.Test = test;
            TempData["ID"] = test.ID;
            return RedirectToAction("Results");
        }


        public ActionResult Results()
        {
            if (TempData["ID"] != null)
            {
                Test test = unitOfWork.TestsRepository.GetByID(TempData["ID"]);
                ViewBag.Test = test;
                return View(test);
            }
            else 
            {
                return RedirectToAction("Index","Home",null);
            }

        }
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
