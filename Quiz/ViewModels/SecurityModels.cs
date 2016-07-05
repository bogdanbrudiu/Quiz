using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using Ninject;
using Ninject.Web.Mvc;
using System.Web.Mvc;
using QuizWeb.Data;

namespace QuizWeb.Models
{
    public class CustomPrincipal : IPrincipal
    {
        private UnitOfWork UnitOfWork;


        public CustomPrincipal(CustomIdentity identity)
        {
            this.UnitOfWork = DependencyResolver.Current.GetServices<UnitOfWork>().FirstOrDefault();
           
            this.Identity = identity;
        }

        #region IPrincipal Members

        public IIdentity Identity { get; private set; }

        public bool IsInRole(string role)
        {
            if (Identity != null && Identity.IsAuthenticated)
            {
                User myUser = UnitOfWork.UsersRepository.Get().Where(u => u.Login == Identity.Name).FirstOrDefault();
                if (myUser != null)
                {
                    return role == "Administrator" && myUser.IsAdmin;
                }
            }
            return false;
        }

        #endregion
    }

    public class CustomIdentity : IIdentity
    {

        public CustomIdentity(string name)
        {
            this.Name = name;
        }

        #region IIdentity Members

        public string AuthenticationType
        {
            get { return "Custom"; }
        }

        public override string ToString()
        {
            return Name;
        }


        public bool IsAuthenticated
        {
            get { return !string.IsNullOrEmpty(this.Name); }
        }

        public string Name { get; private set; }

        #endregion
    }


}