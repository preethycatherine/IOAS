using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using IOAS.Models;
using IOAS.GenericServices;
using System.Collections.Generic;
using System.Web.Security;
using System.Drawing;
using IOAS.Infrastructure;
using System.IO;
using IOAS.Filter;

namespace IOAS.Controllers
{
   
    public class AccountController : Controller
    {
        //[Authorized]
        //[HttpGet]
        //public ActionResult Role()
        //{
        //    ViewBag.dept = AccountService.Getdepartment();
        //    return View();
        //}

        //[Authorized]
        //[HttpPost]
        //public ActionResult Role(RoleModel model)
        //{
        //    try
        //    {
        //        ViewBag.dept = AccountService.Getdepartment();
        //        model.Createduser = User.Identity.Name;
        //        int Rolestatus = AccountService.Addrole(model);
        //        if (Rolestatus == 1)
        //        {
        //            ViewBag.message = "Role name created successfully.";
        //        }
        //        else if (Rolestatus == 2)
        //            ViewBag.Msg = "Role already exists.";
        //        else if (Rolestatus == 3)
        //            ViewBag.update = "Role name Updated successfully.";
        //        else
        //            ViewBag.error = "This smothing went to Error Please Contact your Admin.";

        //        return View();
        //    }
        //    catch(Exception ex)
        //    {
        //        ViewBag.error = "This smothing went to Error Please Contact your Admin.";
        //        return View();
        //    }
        //}
        //[Authorized]
        //[HttpGet]
        //public ActionResult Department()
        //{
        //    ViewBag.message = null;
        //    return View();
        //}

        //[Authorized]
        //[HttpPost]
        //public ActionResult Department(DepartmentModel model)
        //{
        //    try
        //    {
        //        model.Createduser = User.Identity.Name;
        //        int status = AccountService.AddDepartment(model);
        //        if (status == 1)
        //        {
        //            ViewBag.add = "Department name created successfully.";
        //        }
        //        else if (status == 2)
        //            ViewBag.update = "Department name Updated successfully.";
        //        else if (status == 3)
        //            ViewBag.message = "Department name already exists.";
        //        else
        //            ViewBag.error = "This smothing went to Error Please Contact your Admin.";

        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.error = "This smothing went to Error Please Contact your Admin.";
        //        return View();
        //    }
        //}
       // [Authorized]
        [HttpGet]
        public ActionResult AccessRights()
        {
            ViewBag.Function = AccountService.GetFunction();
            ViewBag.dept = AccountService.Getdepartment();
            return View();
        }

        //[Authorized]
        [HttpPost]
        public ActionResult AccessRights(int Depertmentid)
        {

            ViewBag.Function = AccountService.GetFunction();
            ViewBag.dept = AccountService.Getdepartment();
            object output = AccountService.GetFunctionlist(Depertmentid);
            return Json(new { result = output });

        }

        //[Authorized]
        //[HttpPost]
        //public ActionResult Rolelist(int Depertmentid)
        //{
        //    object result = AccountService.GetFunctionlist(Depertmentid);
        //    return Json(result, JsonRequestBehavior.AllowGet);

        //}


        //[Authorized]
        [HttpPost]
        public ActionResult AccessRightsadd(List<Functionviewmodel> model)

        {
            List<Functionviewmodel> value = new List<Functionviewmodel>();
            value = AccountService.AddDepartmentrole(model);
            return Json(value);

        }
        [HttpGet]
        public ActionResult Login()
        {
            string test = User.Identity.Name;
            if (!string.IsNullOrWhiteSpace(test))
                return RedirectToAction("Dashboard", "Home");

            return View();
        }
        [HttpPost]
        public ActionResult Login(LogOnModel model , string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int UserId = AccountService.Logon(model);
                    if(UserId > 0)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, false);
                        string roles = Common.GetRoles(UserId);
                        var authTicket = new FormsAuthenticationTicket(1, model.UserName, DateTime.Now, DateTime.Now.AddHours(4), false, roles);
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        HttpContext.Response.Cookies.Add(authCookie);
                        if (!String.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);
                        return RedirectToAction("Dashboard", "Home");
                    }
                    else
                    {
                        //ModelState.AddModelError("", "The username or password provided is incorrect.");
                        ViewBag.Msg = string.Format("The username or password provided is incorrect.");
                    }
                }
                catch(Exception ex)
                {
                    return null;
                }
            }
            return View();
        }

        public ActionResult LogOff()
        {
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetExpires(DateTime.Now.AddDays(-1d));
            //Response.Cache.SetNoStore();
            //Response.Cookies.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
        [Authorized]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorized]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    String currentusername = User.Identity.Name;
                    changePasswordSucceeded = AccountService.ChangePasswordforuser(model, currentusername);
                }
                catch (Exception ex)
                {
                    //Infrastructure.UAYException.Instance.HandleMe(this, ex);
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        
        [HttpGet]
        public ActionResult ForgotPassword()
        {
           
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            try
            {
                bool sendresetpassword;
                sendresetpassword = AccountService.UserForgotPassword(model);
                if (sendresetpassword)
                {
                    ViewBag.Msg = "We have emailed a new password to your email address. Please check and use it in your next login";
                    return View();
                }
                {
                    ViewBag.Msg = "The username and email address combination does not exist in our database";
                    return View();

                }
                
            }
            catch (Exception ex)
            {
                return null;
            }
            }
        //
        // GET: /Account/ChangePasswordSuccess
        [Authorized]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }


        //[Authorized]
        //[HttpGet]
        //public ActionResult Createuser()
        //{
        //    RegisterModel model = new RegisterModel();
        //    ViewBag.gender = AccountService.GetGender();
        //    ViewBag.dept = AccountService.Getdepartment();
        //    return View(model);
        //}

        //[Authorized]
        //[HttpPost]
        //public ActionResult Createuser(RegisterModel model, HttpPostedFileBase UserImage)
        //{
        //    try
        //    {
        //        if (UserImage != null)
        //        {
        //            Bitmap img = new Bitmap(UserImage.InputStream, false);
        //            int height = img.Height;
        //            int width = img.Width;
        //            string ratio = Common.GetRatio(width, height);
        //            var imageextensions = new[] { ".png", ".jpg", ".jpeg", ".PNG", ".JPG", ".JPEG" };
        //            var ext = Path.GetExtension(UserImage.FileName);
        //            if (!imageextensions.Contains(ext))
        //            {
        //                ModelState.AddModelError("", "Please upload any one of these type image [png, jpg, jpeg]");
        //                return View();
        //            }
        //            else if (UserImage.ContentLength > 10485760)
        //            {
        //                ModelState.AddModelError("", "You can upload image up to 10 MB");
        //                return View();
        //            }
        //            string logoName = System.IO.Path.GetFileName(UserImage.FileName);
        //            var fileId = Guid.NewGuid().ToString();
        //            logoName = fileId + "_" + logoName;
        //            /*Saving the file in server folder*/
        //            UserImage.SaveAs(Server.MapPath("~/Content/UserImage/" + logoName));
        //            model.Image = logoName;
        //        }
        //        ViewBag.dept = AccountService.Getdepartment();
        //        ViewBag.gender = AccountService.GetGender();
        //        model.Createuser = User.Identity.Name;
        //        //if (ModelState.IsValid)
        //        //{
        //        var status = AccountService.UserRegistration(model);
        //        if (status == 1)
        //        {
        //            ViewBag.message = "You have registered Successfully";
        //        }
        //        else if (status == 2)
        //            ViewBag.Msg = "Username " + model.Username + " Already Exists";
        //        else if (status == 3)
        //            ViewBag.update = "User updated successfully.";

        //        else
        //            ViewBag.error = "Something went wrong please contact administrator";

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.message = "Something went wrong please contact administrator";
        //        return View();
        //    }
        //}
        //[Authorized]
        //[HttpGet]
        //public JsonResult GetCreateUserlist()
        //{
        //    object output = AccountService.GetUserList();
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}

        //[Authorized]
        //[HttpPost]
        //public JsonResult EditUserlist(int UserId)
        //{
        //    object output = AccountService.EditUserList(UserId);
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}

        //[Authorized]
        //[HttpPost]
        //public JsonResult DeletUserlist(int UserId)
        //{
        //    string Username = User.Identity.Name;
        //    object output = AccountService.Deleteuserlist(UserId,Username);
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}
        //[Authorized]
        //[HttpGet]
        //public ActionResult CreatePI()
        //{
        //    ViewBag.gender = AccountService.GetGender();
        //    ViewBag.dept = Common.getPIdepartment();
        //    ViewBag.instute = Common.Getinstitute();
        //    ViewBag.designation = Common.Getdesignation();
        //    return View();
        //}

        //[Authorized]
        //[HttpPost]
        //public ActionResult CreatePI(RegisterModel model, HttpPostedFileBase UserImage)
        //{
        //    try
        //    {
        //        if (UserImage != null)
        //        {
        //            Bitmap img = new Bitmap(UserImage.InputStream, false);
        //            int height = img.Height;
        //            int width = img.Width;
        //            string ratio = Common.GetRatio(width, height);
        //            var imageextensions = new[] { ".png", ".jpg", ".jpeg", ".PNG", ".JPG", ".JPEG" };
        //            var ext = Path.GetExtension(UserImage.FileName);
        //            if (!imageextensions.Contains(ext))
        //            {
        //                ModelState.AddModelError("", "Please upload any one of these type image [png, jpg, jpeg]");
        //                return View();
        //            }
        //            else if (UserImage.ContentLength > 10485760)
        //            {
        //                ModelState.AddModelError("", "You can upload image up to 10 MB");
        //                return View();
        //            }
        //            string logoName = System.IO.Path.GetFileName(UserImage.FileName);
        //            var fileId = Guid.NewGuid().ToString();
        //            logoName = fileId + "_" + logoName;
        //            /*Saving the file in server folder*/
        //            UserImage.SaveAs(Server.MapPath("~/Content/UserImage/" + logoName));
        //            model.Image = logoName;
        //        }
        //        ViewBag.gender = AccountService.GetGender();
        //        ViewBag.dept = Common.getPIdepartment();
        //        ViewBag.instute = Common.Getinstitute();
        //        ViewBag.designation = Common.Getdesignation();
        //        model.Createuser = User.Identity.Name;
        //        var PIstatus = AccountService.PIRegistration(model);
        //        if (PIstatus == 1)
        //        {
        //            ViewBag.message = "You have registered Successfully";
        //        }
        //        else if (PIstatus == 2)
        //            ViewBag.Msg = "PI " + model.Email + " Already Exists";
        //        else if (PIstatus == 3)
        //            ViewBag.update = "PI updated successfully.";
        //        else
        //            ViewBag.error = "Something went wrong please contact administrator";
        //        return View();
        //    }
        //    catch(Exception ex)
        //    {
        //        ViewBag.message = "Something went wrong please contact administrator";
        //        return View();
        //    }
            
        //}

        //[Authorized]
        //[HttpGet]
        //public JsonResult GetPIlist()
        //{
        //    object output = AccountService.GetPIList();
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}
        //[Authorized]
        //[HttpPost]
        //public JsonResult EditPIlist(int UserId)
        //{
        //    object output = AccountService.EditPIList(UserId);
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}
        //[Authorized]
        //[HttpPost]
        //public JsonResult DeletePIlist(int UserId)
        //{
        //    string Username = User.Identity.Name;
        //    object output = AccountService.DeletePIlist(UserId, Username);
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}
        //[Authorized]
        //[HttpGet]
        //public JsonResult GetDepartmentlist()
        //{

            
        //    object output = AccountService.GetDepartmentlist();
        //   return Json(output, JsonRequestBehavior.AllowGet);

        //}

        //[Authorized]
        //[HttpPost]
        //public ActionResult GetEditDepartmentlist(int DepartmentId)
        //{

        //    object output = AccountService.GetEditDepartmentlist(DepartmentId);
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}

        //[Authorized]
        //[HttpPost]
        //public ActionResult DeleteDepartment(int DepartmentId)
        //{
        //    string Username = User.Identity.Name;
        //    int deletestatus = AccountService.Deletedepartment(DepartmentId,Username);
        //    if(deletestatus==1)
        //    {
        //        object output = 1;
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
        //    else if(deletestatus==2)
        //    {
        //        object output = 2;
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        object output = -1;
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
            
        //}

        //[Authorized]
        //[HttpGet]
        //public ActionResult GetDepartmentrole(int page=1)
        //{
            
        //    object output = AccountService.GetDepartmentRolelist();
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}

        //[Authorized]
        //[HttpPost]
        //public ActionResult GetEditRolelist(int RoleId)
        //{

        //    object output = AccountService.GetEditRolelist(RoleId);
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}

        //[Authorized]
        //[HttpPost]
        //public ActionResult Deleterolelist(int RoleId)
        //{
        //    string Username = User.Identity.Name;
        //    int deleterolestatus = AccountService.Deleterole(RoleId, Username);
        //    if(deleterolestatus==1)
        //    {
        //        object output = 1;
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
        //    else if(deleterolestatus==2)
        //    {
        //        object output = 2;
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        object output = -1;
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
            
        //}

        //[Authorized]
        //[HttpPost]
        //public ActionResult GetaddtionalRolelist(int Roleid)
        //{
           
        //    object result = AccountService.GetaddtionalRole(Roleid);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPIByDepartment(string Departmentid, string Instituteid)
        {
            Departmentid = Departmentid == "" ? "0" : Departmentid;
            Instituteid = Instituteid == "" ? "0" : Instituteid;
            var locationdata = AccountService.getPIList(Convert.ToInt32(Departmentid), Convert.ToInt32(Instituteid));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        [Authorized]
        [HttpPost]
        public ActionResult AccessRightsFill(int functionid, int Departmentid)
        {
            object output = AccountService.AccessRightsfunction(functionid, Departmentid);
            return Json(new { list = output });

        }

        // [HttpGet]
        //[Authorize]
        //public ActionResult AccountGroup()
        //{
        //    ViewBag.account = Common.GetAccounttype();
        //    return View();
        //}
        //[HttpPost]
        //[Authorized]
        //public ActionResult AccountGroup(Accountgroupmodel model)
        //{
        //    try
        //    {
        //        ViewBag.account = Common.GetAccounttype();
        //        var Username = User.Identity.Name;
        //        model.userid = Common.GetUserid(Username);
        //        var status = AccountService.Accountgroup(model);
        //        if (status == 1)
        //        {
        //            ViewBag.success = "Saved successfully";
        //            return Json(status, JsonRequestBehavior.AllowGet);
        //        }
        //        else if (status == 2)
        //        {
        //            ViewBag.Msg = "This Account group already exits";
        //            return Json(status, JsonRequestBehavior.AllowGet);
        //        }
        //        else if (status == 3)
        //        {
        //            ViewBag.update = "Account group updated successfully";
        //            return Json(status, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            ViewBag.error = "Somthing went to worng please contact Admin!.";
        //            return Json(status, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        var Msg = ex;
        //        return Json(Msg, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[Authorized]
        //[HttpGet]
        //public JsonResult GetAccountgrouplist()
        //{
        //    object output = AccountService.Getaccountgrouplist();
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}
        //[Authorized]
        //[HttpGet]
        //public ActionResult Editaccontgroup(int acccountgrpId)
        //{

        //    object output = AccountService.Editaccountgroup(acccountgrpId);
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}
        //    [Authorized]
        //    [HttpGet]
        //    public ActionResult SRBItemcategory()
        //    {

        //        return View();
        //    }
        //    [Authorized]
        //    [HttpPost]
        //    public ActionResult SRBItemcategory(SRBItemcategory model)
        //    {
        //        try
        //        {

        //            var Username = User.Identity.Name;
        //            model.userid = Common.GetUserid(Username);
        //            int srbitemstatus = AccountService.AddSRBitemcategory(model);
        //            if (srbitemstatus == 1)
        //            {
        //                ViewBag.success = "Saved successfully";
        //            }
        //            else if (srbitemstatus == 2)
        //                ViewBag.Msg = "This SRB Item name already exits";
        //            else if (srbitemstatus == 3)
        //                ViewBag.update = "SRB item name updated successfully";
        //            else
        //                ViewBag.error = "Somthing went to worng please contact Admin!.";
        //            return View();

        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.error = ex;
        //            return View();
        //        }
        //    }
        //    [Authorized]
        //    [HttpGet]
        //    public JsonResult SRBItemcategorylist()
        //    {
        //        object output = AccountService.SRBcategorylist();
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
        //    [Authorized]
        //    [HttpPost]
        //    public JsonResult editsrbitemcategory(int srbitmcateid)
        //    {
        //        object output = AccountService.Editsrbitemcategory(srbitmcateid);
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
        //    [Authorized]
        //    [HttpPost]
        //    public ActionResult deletesrbcategory(int srbitmcateid)
        //    {
        //        string Username = User.Identity.Name;
        //        object output = AccountService.deletesrbitemcategory(srbitmcateid, Username);
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
    }
}