using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using QuizWeb.Models;
using QuizWeb.Data;

namespace QuizWeb.Misc
{
    public static class Misc
    {
        public static string Multiply(this string source, int multiplier)
        {
            StringBuilder sb = new StringBuilder(multiplier * source.Length);
            for (int i = 0; i < multiplier; i++)
            {
                sb.Append(source);
            }

            return sb.ToString();
        }
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void GenerateTest(Test test, Quiz quiz, Test testtocopy, UnitOfWork unitOfWork)
        {
            if (quiz.AlwaysGenerate && testtocopy != null)
            {
                IList<TestDetail> questions=testtocopy.TestDetails.ToList();
                if (quiz.RandomiseQuestionsOrder) 
                {
                    questions.Shuffle();
                }
                foreach (TestDetail testDetail in questions)
                {
                    TestDetail newTestDetail = new TestDetail() { TestID = test.ID, QuestionDescription = testDetail.QuestionDescription, QuestionID = testDetail.QuestionID };
                    test.TestDetails.Add(newTestDetail);
                }
            }
            else
            {
                List<TestDetail> questions = new List<TestDetail>();
                //first add explicit questions
                foreach (var item in quiz.QuizDetails.Where(qd => qd.QuestionID.HasValue))
                {
                    //TestDetail newTestDetail = new TestDetail() { TestID = test.ID, QuestionID = item.QuestionID.Value, Question = item.Question };
                    TestDetail newTestDetail = new TestDetail() { TestID = test.ID, QuestionDescription = item.Question.Description, QuestionID = item.QuestionID.Value };
                    questions.Add(newTestDetail);
                }
                //second add questions to match number fo required questions
                foreach (var item in quiz.QuizDetails.Where(qd => qd.QuestionCategoryID.HasValue))
                {
                    string questionCategoryID = item.QuestionCategoryID.ToString();
                    int requirednumber = item.Number - questions.Where(q => unitOfWork.QuestionsRepository.GetByID(q.QuestionID) != null && unitOfWork.QuestionsRepository.GetByID(q.QuestionID).QuestionCategories.Where(qc => qc.HierarchyPath.Contains(questionCategoryID)).Count() > 0).Count();
                    int[] ids = questions.Select(q => q.QuestionID).ToArray();
                    if (requirednumber > 0)
                    {
                        IList<Question> mylist = unitOfWork.QuestionsRepository.Get().Where(q => q.QuestionCategories.Where(qc => qc.HierarchyPath.Contains(questionCategoryID)).Count() > 0 && !ids.Contains(q.ID)).ToList();
                        mylist.Shuffle();
                        foreach (Question question in mylist.Take(requirednumber))
                        {
                            //TestDetail newTestDetail = new TestDetail() { TestID = test.ID, QuestionID = question.ID, Question = question };
                            TestDetail newTestDetail = new TestDetail() { TestID = test.ID, QuestionDescription = question.Description, QuestionID = question.ID };
                            questions.Add(newTestDetail);
                        }
                    }
                }
                questions.Shuffle();
                foreach (TestDetail testDetail in questions)
                {
                    test.TestDetails.Add(testDetail);
                }

            }
        }
    }
}