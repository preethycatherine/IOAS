using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using IOAS.GenericServies;
using IOAS.GenericServices;
using IOAS.Models;
using DataAccessLayer;
using IOAS.Filter;

namespace IOAS.Controllers
{
    [Authorized]
    public class ReportsController : Controller
    {
        private AdminService adminService = new AdminService();

        // GET: DynamicReport
        public ActionResult Index()
        {
            ListDatabaseObjects db = new ListDatabaseObjects();
            var dbView = db.getAllViews();
            DataSet dsReport = db.getReportDetails(-1);
            ViewBag.Tables = dbView;
            DataTable dtReports = dsReport.Tables[0];
            var reports = Converter.GetEntities<SqlReportModel>(dtReports);


            //WebGridClass wg = new WebGridClass();
            //var table = wg.GetDetailsFromDataTableForGrid<SqlReportModel>(dtReport, "View List", "ID");
            return View();
            //return View("DynamicReports", dbView);
            //var data = new WebGridClass();
            //data = WebGridClass.HoldWebGridDetails;
            //return View(data);
        }

        public ActionResult List()
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataSet dsReport = db.getReportDetails(-1);
                DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;   

                var result = Converter.DataTableToDict(dtReport);
                var model = Converter.GetEntities<SqlReportModel>(dtReport);
                var reports = new PagedData<SqlReportModel>();
                reports.Data = model;
                return View(reports);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult List(int page, int size)
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataSet dsReport = db.getReportDetails(-1);
                DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;
                var result = Converter.DataTableToDict(dtReport);
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        
        public ActionResult ReportBuilder()
        {
            ListDatabaseObjects db = new ListDatabaseObjects();
            DataTable dt = db.getAllViews();
            SqlReportModel model = new SqlReportModel();
            List<SqlViewsPropertyModel> vwFields = new List<SqlViewsPropertyModel>();
            List<SqlViewsModel> vwList = new List<SqlViewsModel>();

            vwList = Converter.GetEntityList<SqlViewsModel>(dt);

            var roles = AdminService.GetRoles();
            var modules = AdminService.GetModules();

            //ViewBag.Tables = vwList;
            ViewBag.TableProperties = vwFields;
            ViewBag.Modules = modules;
            ViewBag.Tables = Converter.GetEntityList<SqlViewsModel>(dt);
            var selectedRoles = new List<RolesModel>();
            model.AvailableRoles = roles;
            model.SelectedRoles = new List<RolesModel>();
            //model.AvailableFields = vwFields;
            //model.SelectedFields = new List<SqlViewsPropertyModel>();

            ViewBag.Roles = new MultiSelectList(roles, "RoleId", "RoleName");
            ViewBag.SelectedRoles = new MultiSelectList(selectedRoles, "RoleId", "RoleName");
            ViewBag.fields = new MultiSelectList(vwFields, "ID", "fieldName");

            return View("ReportBuilder", model);
        }


        [Authorize]
        public ActionResult EditReport(int ReportID)
        {
            SqlReportModel model = new SqlReportModel();

            ListDatabaseObjects db = new ListDatabaseObjects();
            DataTable dt = db.getAllViews();
            List<SqlViewsPropertyModel> vwFields = new List<SqlViewsPropertyModel>();
            List<SqlViewsModel> vwList = new List<SqlViewsModel>();
            vwList = Converter.GetEntityList<SqlViewsModel>(dt);

            var roles = AdminService.GetRoles();
            var modules = AdminService.GetModules();

            ViewBag.TableProperties = vwFields;
            ViewBag.Modules = modules;
            ViewBag.Tables = vwList;
            var selectedRoles = new List<RolesModel>();
            model.AvailableRoles = roles;
            model.SelectedRoles = new List<RolesModel>();

            ViewBag.Roles = new MultiSelectList(roles, "RoleId", "RoleName");
            ViewBag.SelectedRoles = new MultiSelectList(selectedRoles, "RoleId", "RoleName");
            ViewBag.fields = new MultiSelectList(vwFields, "ID", "fieldName");

            if (ReportID > 0)
            {
                DataSet dsReport = db.getReportDetails(ReportID);
                var result = new { };
                if (dsReport != null && dsReport.Tables.Count > 0)
                {
                    var dtReport = Converter.GetEntities<SqlViewsModel>(dsReport.Tables[0]);
                    var dtFields = Converter.GetEntityList<ReportFieldModel>(dsReport.Tables[1]);
                    var dtFilter = Converter.GetEntityList<FilterFieldModel>(dsReport.Tables[2]);
                    var dtSelectedRoles = Converter.GetEntityList<RolesModel>(dsReport.Tables[3]);
                    var dtRoles = Converter.GetEntities<RolesModel>(dsReport.Tables[4]);
                    DataRow report = (dsReport.Tables[0].Rows.Count > 0) ? dsReport.Tables[0].Rows[0] : null;
                    model = Converter.DataRowTonEntity<SqlReportModel>(report);
                    model.dtReportFields = dtFields;
                    model.dtFilterFields = dtFilter;
                    ViewBag.SelectedRoles = dtSelectedRoles;
                    ViewBag.Roles = new MultiSelectList(dtRoles, "RoleID", "RoleName"); ;
                }
            }

            return View(model);
        }

        
        //[HttpPost]
        //public ActionResult ReportBuilder(SqlReportModel model)
        //{
        //    var button = Request["button"];

        //    var selRole = Request["SelectedRoles"] != null ? Request["SelectedRoles"].ToArray() : ("").ToArray();
        //    var availableRole = Request["AvailableRoles"] != null ? Request["AvailableRoles"].ToArray() : ("").ToArray();

        //    List<SqlViewsModel> vwList = new List<SqlViewsModel>();
        //    ListDatabaseObjects db = new ListDatabaseObjects();
        //    List<SqlViewsPropertyModel> vwFields = new List<SqlViewsPropertyModel>();
        //    DataTable dt = db.getAllViews();
        //    DataTable dtFields = db.getAllProperties(model.TableName, "view");

        //    vwFields = Converter.GetEntityList<SqlViewsPropertyModel>(dtFields);
        //    vwList = Converter.GetEntityList<SqlViewsModel>(dt);

        //    var roles = AdminService.GetRoles();
        //    var modules = AdminService.GetModules();
        //    var selectedRoles = new List<RoleModel>();

        //    //model.Tables = vwList;
        //    model.AvailableRoles = roles;
        //    model.SelectedRoles = new List<RolesModel>();
        //    //model.AvailableFields = vwFields;
        //    //model.SelectedFields = new List<SqlViewsPropertyModel>();

        //    ViewBag.Modules = modules;
        //    ViewBag.Tables = vwList;
        //    ViewBag.TableProperties = vwFields;
        //    ViewBag.Roles = new MultiSelectList(roles, "RoleId", "RoleName");
        //    ViewBag.SelectedRoles = new MultiSelectList(selectedRoles, "RoleId", "RoleName");
        //    ViewBag.fields = new MultiSelectList(vwFields, "ID", "fieldName");

        //    //if (model.fields == null || model.fields.Count != vwFields.Count)
        //    //{
        //    //    model.fields = vwFields;
        //    //}
        //    //if (model.orderByFields == null || model.orderByFields.Count != vwFields.Count)
        //    //{
        //    //    model.orderByFields = vwFields;
        //    //}
        //    //if (model.groupByFields == null || model.groupByFields.Count != vwFields.Count)
        //    //{
        //    //    model.groupByFields = vwFields;
        //    //}

           
        //    //if (button == "Save")
        //    //{
        //    //    string strFields = "";
        //    //    string groupByFields = "";
        //    //    string orderByFields = "";
        //    //    for (int item = 0; item < model.fields.Count; item++)
        //    //    {
        //    //        if (model.fields[item].IsSelected)
        //    //        {
        //    //            strFields = (strFields != "") ? strFields + ", " + model.fields[item].fieldName : model.fields[item].fieldName;
        //    //        }

        //    //    }
        //    //    for (int item = 0; item < model.groupByFields.Count; item++)
        //    //    {
        //    //        if (model.groupByFields[item].IsSelected)
        //    //        {
        //    //            groupByFields = (groupByFields != "") ? strFields + ", " + model.groupByFields[item].fieldName : model.groupByFields[item].fieldName;
        //    //        }

        //    //    }
        //    //    for (int item = 0; item < model.orderByFields.Count; item++)
        //    //    {
        //    //        if (model.orderByFields[item].IsSelected)
        //    //        {
        //    //            orderByFields = (orderByFields != "") ? strFields + ", " + model.orderByFields[item].fieldName : model.orderByFields[item].fieldName;
        //    //        }

        //    //    }
        //    //    if (strFields == "")
        //    //    {
        //    //        ViewBag.Msg = "Please select anyone of the fields for report";
        //    //        return View(model);
        //    //    }
        //    //    ListDatabaseObjects db = new ListDatabaseObjects();
        //    //    ReportsProfileHandler prop = new ReportsProfileHandler();
        //    //    prop.ReportID = model.ReportID;
        //    //    prop.ReportName = null; //(model.reportName == ""  || model.reportName == null) ? "test" : model.reportName;
        //    //    prop.ReportDescription = "";
        //    //    prop.TableName = model.tableName;
        //    //    prop.Fields = strFields;
        //    //    prop.GroupByFields = groupByFields;
        //    //    prop.OrderByFields = orderByFields;
        //    //    prop.IsActive = true;
        //    //    prop.RoleId = 0;
        //    //    prop.UserId = 0;
        //    //    var result = db.AddReportDetails(prop, dtFields);

        //    //}
        //    return View("ReportBuilder", model);
        //}


        
        public ActionResult ReportViewer()
        {
            ListDatabaseObjects db = new ListDatabaseObjects();
            SqlReportModel model = new SqlReportModel();
            DataSet dsReport = db.getReportDetails(-1);
            DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;

            List<SqlReportModel> vwList = new List<SqlReportModel>();
            foreach (DataRow row in dtReport.Rows)
            {
                vwList.Add(new SqlReportModel
                {
                    ReportID = Convert.ToInt32(row["ReportID"].ToString()),
                    ReportName = row["ReportName"].ToString()
                });

            }

            ViewBag.Reports = vwList;
            //ViewBag.Reports = db.getReportView("vwUserDetails");
            //DataTable dtUser = db.getReportView("vwUserDetails");
            return View("ReportViewer", model);

            //WebGridClass obj = new WebGridClass();
            //var table = obj.GetTableDetailsForGrid(vwList, "View List", "ID");
            //return View("Index", table);
            //WebGridClass.GetDetailsForGrid(vwList, "View List", "ID");
            //return RedirectToAction("Index");
        }

        
        [HttpPost]
        public ActionResult ReportViewer(SqlReportModel model)
        {
            ListDatabaseObjects db = new ListDatabaseObjects();
            var dbView = db.getAllViews();
            DataTable dt = db.getAllViews();
            SqlViewsModel vwModel = new SqlViewsModel();
            List<SqlViewsPropertyModel> vwPropertyList = new List<SqlViewsPropertyModel>();

            List<SqlViewsModel> vwList = new List<SqlViewsModel>();
            foreach (DataRow row in dt.Rows)
            {
                vwList.Add(new SqlViewsModel
                {
                    ID = row["ID"].ToString(),
                    name = row["name"].ToString()
                });

            }
            ViewBag.Tables = vwList;
            ViewBag.Reports = dt;
            //WebGridClass.GetDetailsForGrid(vwList, "View List", "ID");
            return View("ReportViewer", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult getReportDetails(int ReportID)
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                SqlReportModel model = new SqlReportModel();
                DataSet dsReport = db.getReportDetails(ReportID);
                DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;
                DataTable dtResult = new DataTable();
                dynamic filters = new Dictionary<string, Object>();
                if (dtReport.Rows.Count > 0)
                {
                    dtResult = db.getReportView(ReportID, "");
                    
                    if (dsReport != null && dsReport.Tables.Count > 0)
                    {
                        var report = Converter.GetEntities<SqlViewsModel>(dsReport.Tables[0]);
                        var dtFields = Converter.GetEntityList<ReportFieldModel>(dsReport.Tables[1]);
                        var dtFilter = Converter.GetEntityList<FilterFieldModel>(dsReport.Tables[2]);
                        var dtRoles = Converter.GetEntityList<RolesModel>(dsReport.Tables[3]);
                        DataRow drReport = (dsReport.Tables[0].Rows.Count > 0) ? dsReport.Tables[0].Rows[0] : null;
                        model = Converter.DataRowTonEntity<SqlReportModel>(drReport);
                        model.dtReportFields = dtFields;
                        model.dtFilterFields = dtFilter;
                        model.SelectedRoles = dtRoles;

                        DataTable dtFilters = dsReport.Tables[2];
                        
                        for (int i=0;i<dtFilters.Rows.Count;i++)
                        {
                            if(dtFilters.Rows[i]["FieldType"].ToString() == "Dropdown")
                            {
                                var key = dtFilters.Rows[i]["ReportField"].ToString();
                                var dtResultField = db.getFilterDetails(ReportID, key);
                                if(dtResultField != null)
                                {
                                    filters[key] = Converter.DataTableToDict(dtResultField);
                                }
                                
                            }
                        }
                    }
                }

                var result = dtResult != null ? Converter.DataTableToDict(dtResult) : new List<Dictionary<string, object>>();
                var resultJson = new { result = result, schema = model, filters = filters }; 
                return Json(resultJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult searchReportDetails(int ReportID, string condition)
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtResult = new DataTable();
                dtResult = db.getReportView(ReportID, condition);
                var result = dtResult != null ? Converter.DataTableToDict(dtResult) : new List<Dictionary<string, object>>();
                var resultJson = new { result = result };
                return Json(resultJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult getTables()
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtFields = db.getAllTables();
                var result = Converter.DataTableToDict(dtFields);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult getFields(string tableName)
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtFields = db.getFieldDetails(tableName);
                var result = Converter.DataTableToDict(dtFields);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult getRefFields(string tableName)
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtFields = db.getAllProperties(tableName, "");
                var result = Converter.DataTableToDict(dtFields);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult getFilterDetails(int ReportID, string ReportField)
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtFields = db.getFilterDetails(ReportID, ReportField);
                var result = Converter.DataTableToDict(dtFields);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [ValidateAntiForgeryToken]
        public JsonResult getRoles()
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                var roles = AdminService.GetRoles();
                return Json(roles, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveReportData(SqlReportModel model)
        {

            var msg = "";
            if (model.ReportName == null || model.ReportName == "")
            {
                msg = "Report Name is missing";
            }
            else if (model.ModuleID == 0)
            {
                msg = "Module Name is missing";
            }
            else if (model.TableName == null || model.TableName == "")
            {
                msg = "Table Name is missing";
            }
            else if (model.SelectedRoles == null || model.SelectedRoles.Count == 0)
            {
                msg = "Please select roles";
            }
            else if (model.dtReportFields == null || model.dtReportFields.Count == 0)
            {
                msg = "Please select report fields(atleast one)";
            }
            if(msg != "")
            {
                var error = new { msg = msg };
                return Json(error, JsonRequestBehavior.AllowGet);
            }
            ListDatabaseObjects db = new ListDatabaseObjects();
            ReportsProfileHandler prop = new ReportsProfileHandler();
            prop.ReportID = model.ReportID;
            prop.ReportName = model.ReportName;
            prop.ReportDescription = "";
            prop.TableName = model.TableName;

            prop.dtReportFields = Converter.ToDataTable<ReportFieldModel>(model.dtReportFields);
            prop.dtFilterFields = Converter.ToDataTable<FilterFieldModel>(model.dtFilterFields);
            prop.dtRoles = Converter.ToDataTable<RolesModel>(model.SelectedRoles);

            prop.IsActive = true;
            prop.RoleId = 0;
            prop.ModuleId = model.ModuleID;
            //    prop.UserId = 0;
            var reportId = db.AddReportDetails(prop);
            //var reportId = 1;
            DataSet dsReport = db.getReportDetails(reportId);
            var result = new { };
            if (dsReport != null && dsReport.Tables.Count > 0)
            {
                var dtReport = Converter.DataTableToDict(dsReport.Tables[0]);
                var dtFields = Converter.DataTableToDict(dsReport.Tables[1]);
                var dtFilter = Converter.DataTableToDict(dsReport.Tables[2]);
                var dtRoles = Converter.DataTableToDict(dsReport.Tables[3]);
                var report = (dtReport != null && dtReport.Count > 0) ? dtReport[0] : null;
                var resultJson = new { Report = report, Fields = dtFields, Filter = dtFilter, Roles = dtRoles };
                return Json(resultJson, JsonRequestBehavior.AllowGet);
            }
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
