using System.Web.Mvc;

namespace HMS.Web.API.Areas.FileServer
{
    public class FileServerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "FileServer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "FileServer_default",
                "FileServer/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}