namespace QuizWeb.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using QuizWeb.Data;

    internal sealed class Configuration : DbMigrationsConfiguration<QuizWeb.Data.QuizContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(QuizWeb.Data.QuizContext context)
        {
            DBInit.Init(context);
        }
    }
}
