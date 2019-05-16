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
using IOAS.Infrastructure;
using System.IO;
using IOAS.Filter;

namespace IOAS.Controllers
{
    //[Authorized]
    public class ProjectController : Controller
    {
        // Creation of ProjectInvoice 

        //[HttpGet]
        //public JsonResult Invoice()
        //{
        //    object output = ProjectService.GetInvoiceList();
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Invoice()
        {
            try
            {
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                //if (user_role == 8)
                //{
                    int page = 1;
                    int pageSize = 5;
                    ViewBag.PIName = Common.GetPIWithDetails();
                    var ptypeList = Common.getprojecttype();
                    int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                    ViewBag.ProjectTypeList = ptypeList;
                    var invoicetype = Common.getinvoicetype();
                    var Invoice = Common.GetInvoicedetails();
                    var emptyList = new List<MasterlistviewModel>();
                    ViewBag.ProjectNumberList = emptyList;                    
                    ViewBag.TypeofInvoice = invoicetype;
                    ViewBag.Invoice = Invoice;
                    var data = new PagedData<InvoiceSearchResultModel>();
                    InvoiceListModel model = new InvoiceListModel();
                    ProjectService _ps = new ProjectService();
                    InvoiceSearchFieldModel srchModel = new InvoiceSearchFieldModel();
                    data = _ps.GetInvoiceList(srchModel, page, pageSize);
                    model.Userrole = user_role;
                    model.SearchResult = data;
                    return View(model);
                //}
                //if (user_role == 7)
                //{
                //    int page = 1;
                //    int pageSize = 5;
                //    ViewBag.PIName = Common.GetPIWithDetails();
                //    var Projecttitle = Common.GetPIProjectdetails(logged_in_userid);
                //    var projecttype = Common.getprojecttype();
                //    var invoicetype = Common.getinvoicetype();
                //    var Invoice = Common.GetInvoicedetails();
                //    ViewBag.Project = Projecttitle;
                //    ViewBag.projecttype = projecttype;
                //    ViewBag.TypeofInvoice = invoicetype;
                //    ViewBag.Invoice = Invoice;
                //    var data = new PagedData<InvoiceSearchResultModel>();
                //    InvoiceListModel model = new InvoiceListModel();
                //    ProjectService _ps = new ProjectService();
                //    InvoiceSearchFieldModel srchModel = new InvoiceSearchFieldModel();
                //    srchModel.PIName = logged_in_userid;
                //    data = _ps.GetPIInvoiceList(srchModel, page, pageSize);
                //    model.Userrole = user_role;
                //    model.SearchResult = data;
                //    return View(model);
                //}
                //return RedirectToAction("DashBoard", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DashBoard", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchInvoiceList(InvoiceSearchFieldModel srchModel, int page)
        {
            try
            {
                int pageSize = 5;
                var data = new PagedData<InvoiceSearchResultModel>();
                InvoiceListModel model = new InvoiceListModel();
                ProjectService _ps = new ProjectService();
                if (srchModel.ToDate != null)
                {
                    DateTime todate = (DateTime)srchModel.ToDate;
                    srchModel.ToDate = todate.Date.AddDays(1).AddTicks(-1);
                }
                //else if (srchModel.ToCreateDate != null)
                //{
                //    DateTime todate = (DateTime)srchModel.ToCreateDate;
                //    srchModel.ToCreateDate = todate.Date.AddDays(1).AddTicks(-1);
                //}

                data = _ps.GetInvoiceList(srchModel, page, pageSize);

                model.SearchResult = data;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("DashBoard", "Home");
            }
        }

        public ActionResult ProjectInvoice(int pId = 0)
        {
            try
            {
                if (pId == 0)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();                
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                //if (user_role != 7 && user_role != 8)
                //{
                //    return RedirectToAction("Dashboard", "Home");
                //}                    
                CreateInvoiceModel model = new CreateInvoiceModel();
                ProjectService _ps = new ProjectService();
                model = _ps.GetProjectDetails(pId);   
                if(model.AvailableBalance <= 0)
                {
                    ViewBag.errMsg = "No balance available for raising Invoice";
                }
                if (model.TaxableValue <= 0)
                {
                    ViewBag.errMsg = "No balance available for raising Invoice for this financial year";
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
       
        [HttpPost]
        public ActionResult ProjectInvoice(CreateInvoiceModel model)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                if (model.TaxableValue <= 0)
                {
                    ViewBag.errMsg = "Invoice Cannot be generated. No balance available for raising Invoice";
                    return View(model);
                }                
                //if (roleId != 1 && roleId != 2)
                //    return RedirectToAction("Index", "Home");              
                ProjectService _ps = new ProjectService();
                var InvoiceID = _ps.CreateInvoice(model, loggedinuserid);
                if (InvoiceID == -4)
                {                    
                    ViewBag.errMsg = "Invoice Cannot be generated as the Taxable value has exceeded the balance available for raising Invoice. Please enter correct value and try again.";
                    return View(model);
                }
                if (InvoiceID > 0)
                {
                    var InvoiceNumber = Common.getinvoicenumber(InvoiceID);
                    ViewBag.succMsg = "Invoice has been created successfully with Invoice Number - " + InvoiceNumber + ".";
                }                   
                else if (InvoiceID == -2)
                {
                    var InvoiceId = Convert.ToInt32(model.InvoiceId);
                    var InvoiceNumber = Common.getinvoicenumber(InvoiceId);
                    ViewBag.succMsg = "Invoice with Invoice number - " + InvoiceNumber + " has been updated successfully.";
                }                                   
                else
                {
                    ViewBag.errMsg = "Something went wrong please contact administrator";
                }                   
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult PickDraftInvoice(int DraftId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                //if (roleId != 1 && roleId != 2)
                //    return RedirectToAction("Index", "Home");
                CreateInvoiceModel model = new CreateInvoiceModel();
                ProjectService _ps = new ProjectService();
                model = _ps.GetInvoiceDraftDetails(DraftId);

                return View("ProjectInvoice", model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult DraftProjectInvoice(CreateInvoiceModel model)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                //if (roleId != 1)
                //    return RedirectToAction("Index", "Home");
                ProjectService _ps = new ProjectService();
                var InvoiceDraftID = _ps.DraftInvoice(model, loggedinuserid);
                if (InvoiceDraftID > 0)
                {
                    ViewBag.succMsg = "Invoice has been saved as draft";
                }                   
                else
                {
                    ViewBag.errMsg = "Something went wrong please contact administrator";
                }
                return View("ProjectInvoice", model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult EditProjectInvoice(int InvoiceId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;                
                CreateInvoiceModel model = new CreateInvoiceModel();
                ProjectService _ps = new ProjectService();
                model = _ps.GetInvoiceDetails(InvoiceId);
                if(model.InvoiceId == null)
                {
                    ViewBag.errMsg = "Invoice approved or not available for edit.";
                }
                return View("ProjectInvoice", model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadConsProjectList(string PIId)
        {
            PIId = PIId == "" ? "0" : PIId;
            var locationdata = ProjectService.LoadConsProjecttitledetails(Convert.ToInt32(PIId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadInvoiceList(string ProjectId)
        {
            ProjectId = ProjectId == "" ? "0" : ProjectId;
            var locationdata = ProjectService.LoadInvoiceList(Convert.ToInt32(ProjectId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult LoadTaxpercentage(string servicetype)
        {
            servicetype = servicetype == "" ? "0" : servicetype;
            object output = ProjectService.gettaxpercentage(Convert.ToInt32(servicetype));
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadCancelableInvList(string PIId)
        {
            PIId = PIId == "" ? "0" : PIId;
            var locationdata = ProjectService.LoadCancelableInvList(Convert.ToInt32(PIId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadCancelInvoiceList(string ProjectId)
        {
            ProjectId = ProjectId == "" ? "0" : ProjectId;
            var locationdata = ProjectService.LoadInvoiceList(Convert.ToInt32(ProjectId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        #region Proforma Invoice
        public ActionResult ProformaInvoice()
        {
            try
            {
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                //if (user_role == 8)
                //{
                int page = 1;
                int pageSize = 5;
                ViewBag.PIName = Common.GetPIWithDetails();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                var invoicetype = Common.getinvoicetype();
                var Invoice = Common.GetProformaInvoicedetails();
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.ProjectNumberList = emptyList;
                ViewBag.TypeofInvoice = invoicetype;
                ViewBag.Invoice = Invoice;
                var data = new PagedData<InvoiceSearchResultModel>();
                InvoiceListModel model = new InvoiceListModel();
                ProjectService _ps = new ProjectService();
                InvoiceSearchFieldModel srchModel = new InvoiceSearchFieldModel();
                data = _ps.GetProformaInvoiceList(srchModel, page, pageSize);
                model.Userrole = user_role;
                model.SearchResult = data;
                return View(model);
                //}
                //if (user_role == 7)
                //{
                //    int page = 1;
                //    int pageSize = 5;
                //    ViewBag.PIName = Common.GetPIWithDetails();
                //    var Projecttitle = Common.GetPIProjectdetails(logged_in_userid);
                //    var projecttype = Common.getprojecttype();
                //    var invoicetype = Common.getinvoicetype();
                //    var Invoice = Common.GetInvoicedetails();
                //    ViewBag.Project = Projecttitle;
                //    ViewBag.projecttype = projecttype;
                //    ViewBag.TypeofInvoice = invoicetype;
                //    ViewBag.Invoice = Invoice;
                //    var data = new PagedData<InvoiceSearchResultModel>();
                //    InvoiceListModel model = new InvoiceListModel();
                //    ProjectService _ps = new ProjectService();
                //    InvoiceSearchFieldModel srchModel = new InvoiceSearchFieldModel();
                //    srchModel.PIName = logged_in_userid;
                //    data = _ps.GetPIInvoiceList(srchModel, page, pageSize);
                //    model.Userrole = user_role;
                //    model.SearchResult = data;
                //    return View(model);
                //}
                //return RedirectToAction("DashBoard", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DashBoard", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchProformaInvoiceList(InvoiceSearchFieldModel srchModel, int page)
        {
            try
            {
                int pageSize = 5;
                var data = new PagedData<InvoiceSearchResultModel>();
                InvoiceListModel model = new InvoiceListModel();
                ProjectService _ps = new ProjectService();
                if (srchModel.ToDate != null)
                {
                    DateTime todate = (DateTime)srchModel.ToDate;
                    srchModel.ToDate = todate.Date.AddDays(1).AddTicks(-1);
                }
                //else if (srchModel.ToCreateDate != null)
                //{
                //    DateTime todate = (DateTime)srchModel.ToCreateDate;
                //    srchModel.ToCreateDate = todate.Date.AddDays(1).AddTicks(-1);
                //}

                data = _ps.GetProformaInvoiceList(srchModel, page, pageSize);

                model.SearchResult = data;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("DashBoard", "Home");
            }
        }

        public ActionResult CreateProformaInvoice(int pId = 0)
        {
            try
            {
                if (pId == 0)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                //if (user_role != 7 && user_role != 8)
                //{
                //    return RedirectToAction("Dashboard", "Home");
                //}                    
                CreateInvoiceModel model = new CreateInvoiceModel();
                ProjectService _ps = new ProjectService();
                model = _ps.GetDetailsforProformaInvoice(pId);
                if (model.AvailableBalance <= 0)
                {
                    ViewBag.errMsg = "No balance available for raising Invoice";
                }
                if (model.TaxableValue <= 0)
                {
                    ViewBag.errMsg = "No balance available for raising Invoice for this financial year";
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult CreateProformaInvoice(CreateInvoiceModel model)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                if (model.TaxableValue <= 0)
                {
                    ViewBag.errMsg = "Proforma Invoice Cannot be generated. No balance available for raising Invoice";
                    return View(model);
                }
                //if (roleId != 1 && roleId != 2)
                //    return RedirectToAction("Index", "Home");              
                ProjectService _ps = new ProjectService();
                var InvoiceID = _ps.CreateProformaInvoice(model, loggedinuserid);
                if (InvoiceID == -4)
                {
                    ViewBag.errMsg = "Proforma Invoice Cannot be generated as the Taxable value has exceeded the balance available for raising Invoice. Please enter correct value and try again.";
                    return View(model);
                }
                if (InvoiceID > 0)
                {
                    var InvoiceNumber = Common.getproformainvoicenumber(InvoiceID);
                    ViewBag.succMsg = "Proforma Invoice has been created successfully with Invoice Number - " + InvoiceNumber + ".";
                }
                else if (InvoiceID == -2)
                {
                    var InvoiceId = Convert.ToInt32(model.InvoiceId);
                    var InvoiceNumber = Common.getproformainvoicenumber(InvoiceId);
                    ViewBag.succMsg = "Proforma Invoice with Invoice number - " + InvoiceNumber + " has been updated successfully.";
                }
                else
                {
                    ViewBag.errMsg = "Something went wrong please contact administrator";
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult PickDraftProformaInvoice(int DraftId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                //if (roleId != 1 && roleId != 2)
                //    return RedirectToAction("Index", "Home");
                CreateInvoiceModel model = new CreateInvoiceModel();
                ProjectService _ps = new ProjectService();
                model = _ps.GetProformaInvoiceDraftDetails(DraftId);

                return View("CreateProformaInvoice", model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult DraftProformaInvoice(CreateInvoiceModel model)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                //if (roleId != 1)
                //    return RedirectToAction("Index", "Home");
                ProjectService _ps = new ProjectService();
                var InvoiceDraftID = _ps.DraftInvoice(model, loggedinuserid);
                if (InvoiceDraftID > 0)
                {
                    ViewBag.succMsg = "Proforma Invoice has been saved as draft";
                }
                else
                {
                    ViewBag.errMsg = "Something went wrong please contact administrator";
                }
                return View("CreateProformaInvoice", model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult EditProformaInvoice(int InvoiceId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                CreateInvoiceModel model = new CreateInvoiceModel();
                ProjectService _ps = new ProjectService();
                model = _ps.GetProformaInvoiceDetails(InvoiceId);
                if (model.InvoiceId == null)
                {
                    ViewBag.errMsg = "Proforma Invoice approved or not available for edit.";
                }
                return View("CreateProformaInvoice", model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadProformaInvoiceList(string ProjectId)
        {
            ProjectId = ProjectId == "" ? "0" : ProjectId;
            var locationdata = ProjectService.LoadProformaInvoiceList(Convert.ToInt32(ProjectId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}