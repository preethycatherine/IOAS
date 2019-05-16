using System;
using System.Collections.Generic;
using System.Linq;
using IOAS.Models;
using IOAS.DataModel;

namespace IOAS.GenericServies
{
    public class AdminService
    {

        //public List<ReportModulesModel> GetModules()
        //{
        //    List<ReportModulesModel> ModulesList = new List<ReportModulesModel>();
        //    ModulesList.Add(new ReportModulesModel { ModuleId = 1, ModuleName = "Admin" });
        //    ModulesList.Add(new ReportModulesModel { ModuleId = 2, ModuleName = "Sales" });
        //    ModulesList.Add(new ReportModulesModel { ModuleId = 3, ModuleName = "Finance" });
        //    ModulesList.Add(new ReportModulesModel { ModuleId = 4, ModuleName = "Accounts" });
        //    ModulesList.Add(new ReportModulesModel { ModuleId = 5, ModuleName = "IT" });
        //    ModulesList.Add(new ReportModulesModel { ModuleId = 6, ModuleName = "Pay Roll" });
        //    ModulesList.Add(new ReportModulesModel { ModuleId = 7, ModuleName = "Tester" });


        //    return ModulesList;
        //}

        public static List<RolesModel> GetRoles()
        {
            try
            {
                List<RolesModel> roles = new List<RolesModel>();
                using (var context = new IOASDBEntities())
                {
                    roles = (from R in context.tblRole
                               orderby R.RoleName
                               select new RolesModel { RoleID = R.RoleId, RoleName = R.RoleName }).ToList();

                }
                return roles;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<ReportModulesModel> GetModules()
        {
            try
            {
                List<ReportModulesModel> modules = new List<ReportModulesModel>();
                using (var context = new IOASDBEntities())
                {
                    modules = (from M in context.tblModules
                               orderby M.ModuleName
                               select new ReportModulesModel { ModuleID = M.ModuleID, ModuleName = M.ModuleName }).ToList();

                }
                return modules;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }


}