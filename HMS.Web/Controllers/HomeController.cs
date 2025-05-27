using HMS.Web.API.Common;
using System.Web.Mvc;

namespace HMS.Web.API.Controllers
{
    public class HomeController : Controller
    {
     
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        [HttpGet]
        public ActionResult Invoice(string id, string Date)
        {
            
            string EncryIidText = Cryptography.Decrypt(id);
            string EncryIDateText = Cryptography.Decrypt(Date);

            return View();
        }
        [HttpGet]
        public ActionResult EasyPaisaPayment(string type,string id,string CompID)
        {
            string auth_token = Request.QueryString["auth_token"];
            var apiUrl = System.Configuration.ConfigurationManager.AppSettings["ApiUrl"];
            string postBackURL = apiUrl + "/api/Admin/adm_payment/EasyPaisaHandler?type=" + type + "&id=" + id + "&CompID=" + CompID;
            ViewBag.auth_token = auth_token;
            ViewBag.confirmUrl = System.Configuration.ConfigurationManager.AppSettings["easypayUrl"];
            Session["test"] = "type";
            var storageItem = Request.Form["PaymentMethodType"];
            ViewBag.postBackURL = postBackURL;
            return View();
        }
        public ActionResult Home()
        {
            ViewBag.Title = "Home Page";

            return PartialView();
        }
    }
}