using IOAS.DataModel;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.IO;
namespace IOAS.GenericServices
{
    public class AccountService
    {
        /// <summary>
        /// This method when user login check user name and password 
        /// </summary>
        /// <param name="logon"></param>
        /// <returns></returns>
        public static int Logon(LogOnModel logon)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {

                    String Encpassword = Cryptography.Encrypt(logon.Password, "LFPassW0rd");
                    var userquery = context.tblUser.SingleOrDefault(dup => dup.UserName == logon.UserName && dup.Password == Encpassword && dup.Status == "Active");

                    if (userquery != null)
                    {
                        tblLoginDetails log = new tblLoginDetails();
                        log.UserId = userquery.UserId;
                        log.LoginTime = DateTime.Now;
                        context.tblLoginDetails.Add(log);
                        context.SaveChanges();
                        return userquery.UserId;

                    }

                    else
                    {
                        return 0;
                    }

                }
            }
            catch (Exception ex)
            {

                return -1;
            }
        }

        public static bool ChangePasswordforuser(ChangePasswordModel model, String username)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var oldpassword = Cryptography.Encrypt(model.OldPassword, "LFPassW0rd");
                    var userquery = context.tblUser.SingleOrDefault(dup => dup.UserName == username && dup.Password == oldpassword);

                    if (userquery != null)
                    {
                        userquery.Password = Cryptography.Encrypt(model.NewPassword, "LFPassW0rd"); ;
                        context.SaveChanges();
                        context.Dispose();
                        return true;

                    }

                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        /// <summary>
        /// This method used for user forgetpassword
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UserForgotPassword(ForgotPasswordModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {

                    var userquery = context.tblUser.SingleOrDefault(dup => dup.Email == model.Email);

                    //if (userquery != null)
                    //{
                    //    string temppass = userquery.Password;
                    //    userquery.Password = Cryptography.Decrypt(temppass, "LFPassW0rd");


                    //    if (userquery.Password != null)
                    //    {

                    //        model.password = userquery.Password;

                    //    }


                    //    //context.SaveChanges();
                    //    //context.Dispose();


                    //}
                    if (userquery != null)
                    {
                        string temppass = Guid.NewGuid().ToString().Substring(0, 8);
                        userquery.Password = Cryptography.Encrypt(temppass, "LFPassW0rd");


                        var Disclaimer = EmailTemplate.disclaimer;
                        using (MailMessage mm = new MailMessage(EmailTemplate.mailid, model.Email))
                        {
                            mm.Subject = "IOAS Website Account Password";
                            string body = "Hello " + userquery.UserName + ",";
                            body += "<br /><br />Your account password has been reset successfully. Please use the below password to log into the system";
                            //body += "<br /><a href = '" + Request.Url.AbsoluteUri.Replace("Jobseekers.aspx", "CS_Activation.aspx?ActivationCode=" + activationCode) + "'>Click here to activate your account.</a>";
                            body += "<br />Your new password is " + temppass;
                            body += "<br /><br />Thanks";
                            body += "<br /><br />________________________________________________________________________________________________________________";
                            body += "<br /><br />*** This is an automatically generated email, please do not reply ***";
                            body += "<br /><br />" + Disclaimer;
                            mm.Body = body;
                            mm.IsBodyHtml = true;

                            using (SmtpClient smtp = new SmtpClient(EmailTemplate.smtpAddress, EmailTemplate.portNumber))
                            {
                                smtp.Credentials = new NetworkCredential(EmailTemplate.mailid, EmailTemplate.password);
                                //smtp.Credentials = new NetworkCredential("info@crescentglobal.com", "ofni963");
                                //smtp.EnableSsl = EmailTemplate.enableSSL;
                                smtp.Send(mm);
                            }
                        }




                        context.SaveChanges();
                        context.Dispose();
                        return true;

                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {

                return true;
            }

        }
        /// <summary>

        public static int UserRegistration(RegisterModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())

                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        tblUserRole objuserrole = new tblUserRole();

                        if (model.UserId == 0)
                        {
                            try
                            {
                                tblUser reg = new tblUser();
                                var chkuser = context.tblUser.FirstOrDefault(dup => dup.UserName == model.Username && dup.Status == "Active");
                                if (chkuser != null)
                                    return 2;
                                reg.FirstName = model.Firstname;
                                reg.LastName = model.Lastname;
                                reg.RoleId = model.RoleId;
                                reg.UserName = model.Username;
                                reg.Password = Cryptography.Encrypt(model.Password, "LFPassW0rd");
                                reg.Dateofbirth = model.Dateofbirth;
                                reg.DepartmentId = model.Department;
                                reg.Gender = model.Gender;
                                reg.CRTDDateTS = DateTime.Now;
                                reg.UPDTDateTS = DateTime.Now;
                                string Username = model.Createuser;
                                reg.CreatedUserId = Common.GetUserid(Username);
                                reg.Email = model.Username;
                                reg.Status = "Active";
                                reg.UserImage = model.Image;
                                context.tblUser.Add(reg);
                                context.SaveChanges();
                                if (model.SelectedRoles != null)
                                {
                                    var userid = (from U in context.tblUser
                                                  where (U.UserName == model.Username)
                                                  select U.UserId).FirstOrDefault();
                                    model.UserId = userid;
                                    for (int i = 0; i < model.SelectedRoles.Length; i++)
                                    {
                                        objuserrole.UserId = model.UserId;
                                        objuserrole.RoleId = model.SelectedRoles[i];
                                        objuserrole.Delegated_f = false;
                                        context.tblUserRole.Add(objuserrole);
                                        context.SaveChanges();
                                    }
                                }
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return -1;
                            }
                            return 1;
                        }
                        else
                        {
                            var objupdateuser = context.tblUser.Where(U => U.UserId == model.UserId).FirstOrDefault();
                            if (objupdateuser != null)
                            {
                                try
                                {
                                    objupdateuser.UserId = model.UserId;
                                    objupdateuser.FirstName = model.Firstname;
                                    objupdateuser.LastName = model.Lastname;
                                    objupdateuser.RoleId = model.RoleId;
                                    //bjupdateuser.UserName = model.Username;
                                    //reg.Password = Cryptography.Encrypt(model.Password, "LFPassW0rd");
                                    objupdateuser.Dateofbirth = model.Dateofbirth;
                                    objupdateuser.DepartmentId = model.Department;
                                    objupdateuser.Gender = model.Gender;
                                    objupdateuser.UPDTDateTS = DateTime.Now;
                                    string Username = model.Createuser;
                                    objupdateuser.LastUpdateUserId = Common.GetUserid(Username);
                                    //objupdateuser.Email = model.Username;
                                    if (model.Image != null)
                                    {
                                        objupdateuser.UserImage = model.Image;
                                    }

                                    context.SaveChanges();

                                    var username = (from U in context.tblUser
                                                    where (U.UserId == model.UserId)
                                                    select U.UserName).FirstOrDefault();
                                    model.Username = username;
                                    var query = (from R in context.tblUserRole
                                                 where (R.UserId == model.UserId)
                                                 select R).ToList();
                                    if (query.Count > 0)
                                    {
                                        context.tblUserRole.RemoveRange(query);
                                        context.SaveChanges();

                                    }
                                    if (model.SelectedRoles != null)
                                    {
                                        for (int i = 0; i < model.SelectedRoles.Length; i++)
                                        {
                                            objuserrole.UserId = model.UserId;
                                            objuserrole.RoleId = model.SelectedRoles[i];
                                            objuserrole.Delegated_f = false;
                                            context.tblUserRole.Add(objuserrole);
                                            context.SaveChanges();
                                        }
                                    }
                                    transaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return -1;
                                }
                            }
                            return 3;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public static int Deleteuserlist(int UserId, string Username)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from D in context.tblUser
                                 where (D.UserId == UserId)
                                 select D.UserId).FirstOrDefault();

                    var user = context.tblUser.Where(U => U.UserId == UserId).FirstOrDefault();
                    if (user != null)
                    {
                        user.LastUpdateUserId = Common.GetUserid(Username);
                        user.UPDTDateTS = DateTime.Now;
                        user.Status = "InActive";
                        context.SaveChanges();
                    }
                    var userrole = (from R in context.tblUserRole
                                    where (R.UserId == UserId)
                                    select R).ToList();
                    if (userrole.Count > 0)
                    {
                        context.tblUserRole.RemoveRange(userrole);
                        context.SaveChanges();

                    }
                }
                return 4;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public static int PIRegistration(RegisterModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (model.UserId == 0)
                    {
                        tblUser PIreg = new tblUser();
                        var chkPI = context.tblUser.FirstOrDefault(dup => dup.Email == model.Email && dup.Status == "Active");
                        if (chkPI != null)
                            return 2;
                        string Username = model.Createuser;
                        PIreg.FirstName = model.Firstname;
                        PIreg.LastName = model.Lastname;
                        PIreg.RoleId = 7;
                        PIreg.Dateofbirth = model.Dateofbirth;
                        PIreg.DepartmentId = model.Department;
                        PIreg.Gender = model.Gender;
                        PIreg.CRTDDateTS = DateTime.Now;
                        PIreg.UPDTDateTS = DateTime.Now;
                        PIreg.CreatedUserId = Common.GetUserid(Username);
                        PIreg.Email = model.Email;
                        PIreg.Status = "Active";
                        PIreg.InstituteId = model.InstituteId;
                        PIreg.UserImage = model.Image;
                        PIreg.EMPCode = model.EMPCode;
                        PIreg.Designation = model.Designation;
                        context.tblUser.Add(PIreg);
                        context.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        var objupdatePI = context.tblUser.Where(M => M.UserId == model.UserId).FirstOrDefault();
                        if (objupdatePI != null)
                        {
                            string Username = model.Createuser;
                            objupdatePI.FirstName = model.Firstname;
                            objupdatePI.LastName = model.Lastname;
                            objupdatePI.RoleId = 7;
                            objupdatePI.Dateofbirth = model.Dateofbirth;
                            objupdatePI.DepartmentId = model.Department;
                            objupdatePI.Gender = model.Gender;
                            objupdatePI.UPDTDateTS = DateTime.Now;
                            objupdatePI.LastUpdateUserId = Common.GetUserid(Username);
                            objupdatePI.Email = model.Email;
                            objupdatePI.Status = "Active";
                            objupdatePI.InstituteId = model.InstituteId;
                            if (model.Image != null)
                            {
                                objupdatePI.UserImage = model.Image;
                            }
                            objupdatePI.EMPCode = model.EMPCode;
                            objupdatePI.Designation = model.Designation;
                            context.SaveChanges();

                        }
                        return 3;
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        //delete PI List in grid
        public static int DeletePIlist(int UserId, string Username)
        {
            try
            {


                using (var context = new IOASDBEntities())
                {
                    var query = (from D in context.tblUser
                                 where (D.UserId == UserId)
                                 select D.UserId).FirstOrDefault();

                    var user = context.tblUser.Where(U => U.UserId == UserId).FirstOrDefault();

                    if (user != null)
                    {
                        user.Status = "InActive";
                        user.UPDTDateTS = DateTime.Now;
                        user.LastUpdateUserId = Common.GetUserid(Username);
                        context.SaveChanges();
                    }
                }
                return 4;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public static List<MasterlistviewModel> getPIList(int Departmentid, int Instituteid)
        {
            try
            {

                List<MasterlistviewModel> PIList = new List<MasterlistviewModel>();
                PIList.Add(new MasterlistviewModel()
                {
                    id = 0,
                    name = "Select any"

                });
                using (var context = new IOASDBEntities())
                {
                    if (Departmentid > 0)
                    {
                        var query = (from C in context.tblUser
                                     join ins in context.tblInstituteMaster on C.InstituteId equals ins.InstituteId
                                     where (C.RoleId == 2 && C.DepartmentId == Departmentid && C.InstituteId == Instituteid)
                                     orderby C.UserId
                                     select new { C.UserId, C.FirstName, C.LastName, C.EMPCode, ins.Institutecode }).ToList();
                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                PIList.Add(new MasterlistviewModel()
                                {
                                    id = query[i].UserId,
                                    name = query[i].EMPCode + "-" + query[i].FirstName + " " + query[i].LastName + "-" + query[i].Institutecode,

                                });
                            }
                        }
                    }


                }

                return PIList;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static int Resetpassword(ResetPassword model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var user = context.tblUser.Where(U => U.UserId == model.Userid).FirstOrDefault();
                    if (user != null)
                    {
                        string Username = model.Username;
                        user.Password = Cryptography.Encrypt(model.NewPassword, "LFPassW0rd");
                        user.UPDTDateTS = DateTime.Now;
                        user.LastUpdateUserId = Common.GetUserid(Username);
                        context.SaveChanges();
                        context.Dispose();
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        /// <summary>
        /// Get function list in table
        /// </summary>
        /// <returns>To Fill in dropdown list</returns>
        public static List<Functionlistmodel> GetFunction()
        {
            try
            {
                List<Functionlistmodel> Function = new List<Functionlistmodel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from F in context.tblFunction
                                 orderby F.FunctionName
                                 select new { F.FunctionId, F.FunctionName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Function.Add(new Functionlistmodel
                            {
                                Functionid = query[i].FunctionId,
                                Functionname = query[i].FunctionName
                            });
                        }
                    }
                }
                return Function;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// To load department in dropdownlist
        /// </summary>
        /// <returns></returns>
        public static List<RoleModel> Getdepartment()
        {
            try
            {
                List<RoleModel> dept = new List<RoleModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from d in context.tblDepartment
                                 orderby d.DepartmentName
                                 where (d.Status == "Active")
                                 select new { d.DepartmentId, d.DepartmentName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            dept.Add(new RoleModel
                            {

                                Departmentid = query[i].DepartmentId,
                                Departmentname = query[i].DepartmentName
                            });
                        }
                    }
                }
                return dept;
            }
            catch (Exception ex)
            {
                List<RoleModel> dept = new List<RoleModel>();
                return dept;
            }
        }

        /// <summary>
        /// Get Department list 
        /// </summary>
        /// <param name="Departmentid">To pass DepartmentId in this parameter</param>
        /// <returns></returns>
        public static List<Functionviewmodel> GetFunctionlist(int Departmentid)
        {
            try
            {
                List<Functionviewmodel> Funmodel = new List<Functionviewmodel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from D in context.tblRole
                                 where (D.DepartmentId == Departmentid && D.Status == "Active")
                                 select new { D.RoleId, D.RoleName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Funmodel.Add(new Functionviewmodel()
                            {
                                sno = i + 1,
                                Roleid = query[i].RoleId,
                                Rolename = query[i].RoleName
                            });
                        }
                    }
                }
                return Funmodel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Add department wise role authorization   
        /// </summary>
        /// <param name="model">Role access form data</param>
        /// <returns></returns>
        public static List<Functionviewmodel> AddDepartmentrole(List<Functionviewmodel> model)
        {
            try
            {

                List<Functionviewmodel> listfunction = new List<Functionviewmodel>();
                tblRoleaccess objrole = new tblRoleaccess();
                using (var dbctx = new IOASDBEntities())
                {

                    var funid = model[0].Functionid;
                    var deptid = model[0].Departmentid;
                    var query = (from f in dbctx.tblRoleaccess
                                 where (f.FunctionId == funid && f.DepartmentId == deptid)
                                 select f).ToList();
                    if (query.Count > 0)
                    {
                        dbctx.tblRoleaccess.RemoveRange(query);
                        dbctx.SaveChanges();

                    }

                }
                using (var context = new IOASDBEntities())
                {

                    if (model.Count > 0)
                    {

                        for (int i = 0; i < model.Count; i++)
                        {

                            if (model[i].Read == true)
                            {
                                objrole.RoleId = model[i].Roleid;
                                objrole.FunctionId = model[i].Functionid;
                                objrole.Read_f = model[i].Read;
                                objrole.Add_f = model[i].Add;
                                objrole.Delete_f = model[i].Delete;
                                objrole.Approve_f = model[i].Approve;
                                objrole.Update_f = model[i].Update;
                                objrole.DepartmentId = model[i].Departmentid;
                                objrole.Status = "Active";
                                context.tblRoleaccess.Add(objrole);
                                context.SaveChanges();
                            }
                        }
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                var msg = ex;
                return model;
            }
        }
        public static List<Functionviewmodel> AccessRightsfunction(int functionid, int Departmentid)
        {
            try
            {
                List<Functionviewmodel> funaccess = new List<Functionviewmodel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from R in context.tblRoleaccess
                                 where (R.FunctionId == functionid && R.DepartmentId == Departmentid)
                                 select new { R.FunctionId, R.DepartmentId, R.RoleId, R.Read_f, R.Add_f, R.Approve_f, R.Delete_f, R.Update_f }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            funaccess.Add(new Functionviewmodel()
                            {
                                Roleid = (Int32)query[i].RoleId,
                                Functionid = (Int32)query[i].FunctionId,
                                Departmentid = (Int32)query[i].DepartmentId,
                                Read = (bool)query[i].Read_f,
                                Add = (bool)query[i].Add_f,
                                Approve = (bool)query[i].Approve_f,
                                Delete = (bool)query[i].Delete_f,
                                Update = (bool)query[i].Update_f
                            });
                        }
                    }
                    return funaccess;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}