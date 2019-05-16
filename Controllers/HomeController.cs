using IOAS.Filter;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IOAS.Controllers
{
    public class HomeController : Controller
    {

      //  [Authorized]
        public ActionResult Index()
        {
            return View();
        }

       // [Authorized]
        public ActionResult Dashboard()

        {
            try
            {
                string logged_in_user = User.Identity.Name;
                int logged_in_user_id = Common.GetUserid(logged_in_user);
                var user = Common.getUserIdAndRole(logged_in_user);
                int user_role = user.Item2;
                ViewBag.FirstName = Common.GetUserFirstName(logged_in_user);
                ViewBag.LoginTS = Common.GetLoginTS(logged_in_user_id);
                List<NotificationModel> nofity = new List<NotificationModel>();
                nofity = Common.GetNotification(logged_in_user_id, user_role);
                return View(nofity);
            }
            catch (FileNotFoundException ex)
            {
                return View();
            }
        }
       // [Authorized]
        public ActionResult ShowDocument(string file, string filepath)
        {
            try
            {
                string fileType = Common.GetMimeType(Path.GetExtension(file));
                byte[] fileData = file.GetFileData(Server.MapPath(filepath));
                Response.AddHeader("Content-Disposition", "inline; filename=\"" + file + "\"");
                return File(fileData, fileType);
            }
            catch (FileNotFoundException ex)
            {
                throw new HttpException(404, "File not found.");
            }
        }

    }
}