using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Objects;
using QuizWeb.Models;

namespace QuizWeb.Data
{
    public class UnitOfWork
    {
        private QuizContext context;
        private Repository<Question> questionsRepository;
        private Repository<Answer> answersRepository;
        private Repository<QuestionCategory> questionCategoriesRepository;
        private Repository<User> usersRepository;
        private Repository<Quiz> quizzesRepository;
        private Repository<QuizDetail> quizDetailsRepository;
        private Repository<Test> testsRepository;
        private Repository<TestDetail> testDetailsRepository;

        public UnitOfWork(QuizContext context)
        {
            this.context = context;
        }
     


        public Repository<Question> QuestionsRepository
        {
            get
            {

                if (this.questionsRepository == null)
                {
                    this.questionsRepository = new Repository<Question>(context);
                }
                return questionsRepository;
            }
        }

        public Repository<Answer> AnswersRepository
        {
            get
            {

                if (this.answersRepository == null)
                {
                    this.answersRepository = new Repository<Answer>(context);
                }
                return answersRepository;
            }
        }
        public Repository<QuestionCategory> QuestionCategoriesRepository
        {
            get
            {

                if (this.questionCategoriesRepository == null)
                {
                    this.questionCategoriesRepository = new Repository<QuestionCategory>(context);
                }
                return questionCategoriesRepository;
            }
        }
        public Repository<Quiz> QuizzesRepository
        {
            get
            {

                if (this.quizzesRepository == null)
                {
                    this.quizzesRepository = new Repository<Quiz>(context);
                }
                return quizzesRepository;
            }
        }
        public Repository<QuizDetail> QuizDetailsRepository
        {
            get
            {

                if (this.quizDetailsRepository == null)
                {
                    this.quizDetailsRepository = new Repository<QuizDetail>(context);
                }
                return quizDetailsRepository;
            }
        }
        public Repository<Test> TestsRepository
        {
            get
            {

                if (this.testsRepository == null)
                {
                    this.testsRepository = new Repository<Test>(context);
                }
                return testsRepository;
            }
        }
        public Repository<TestDetail> TestDetailsRepository
        {
            get
            {

                if (this.testDetailsRepository == null)
                {
                    this.testDetailsRepository = new Repository<TestDetail>(context);
                }
                return testDetailsRepository;
            }
        }
        public Repository<User> UsersRepository
        {
            get
            {

                if (this.usersRepository == null)
                {
                    this.usersRepository = new Repository<User>(context);
                }
                return usersRepository;
            }
        }

      


        internal int Save()
        {
            return context.SaveChanges();
        }
    }
}
