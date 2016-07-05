using System.Web.Mvc;

namespace QuizWeb.Areas._Public
{
    public class _PublicAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "_Public";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "_Public_default",
                "_Public/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
