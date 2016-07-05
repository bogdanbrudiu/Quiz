using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

using System.Collections;
using System.Diagnostics;
using System.Data;
using QuizWeb.Models;


namespace QuizWeb.Data
{

    public class QuizContextInitializer : CreateDatabaseIfNotExists<QuizContext>
    {
        
        protected override void Seed(QuizContext context)
        {
            base.Seed(context);
            DBInit.Init(context);
          
        }

    }
}
