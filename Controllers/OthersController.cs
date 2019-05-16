using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IOAS.Controllers
{
    public class OthersController : Controller
    {
        // GET: Others
        public ActionResult GroupTravelInsurance()
        {
            return View("GroupTravelInsurance");
        }
    }
}