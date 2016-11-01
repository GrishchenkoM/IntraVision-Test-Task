using System.IO;
using System.Web.Mvc;
using Web.Filters;

namespace Web.Controllers
{
    
    public class AdminController : Controller
    {
        [SimpleAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CoinView(int? index)
        {
            if (index != null)
                return View("_ManageCoin", index);

            return View("_ManageCoin", -1);
        }

        public ActionResult DrinkView(int? index)
        {
            if (index != null)
                return View("_ManageDrink", index);

            return View("_ManageDrink", -1);
        }

        [HttpPost]
        public JsonResult Upload(string value)
        {
            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                if (upload != null)
                {
                    var fileName = Path.GetFileName(upload.FileName);
                    upload.SaveAs(Server.MapPath("~/Files/" + fileName));
                }
            }
            return Json("Done!");
        }
    }
}