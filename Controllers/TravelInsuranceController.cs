using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IOAS.Reports;
using IOAS.Models.Others;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Globalization;
using System.Net.Mail;
using System.Net;

namespace IOAS.Controllers
{
    public class TravelInsuranceController : Controller
    {
        private TravelInsuranceModel travel = new TravelInsuranceModel();
        public ActionResult TravInsurance()
        {    
            return View("TravInsurance");
        }

    
        public ActionResult TravelIndex()
        {
            
            return View("TravelIndex");
        }     
        [HttpPost]
        public ActionResult TravelIndex(IOAS.Models.Others.insuranceInput tim)
        {

            if (ModelState.IsValid)
            {
                Models.Others.TravelInsuranceModel records = new Models.Others.TravelInsuranceModel();
                try
                {
                    string travel = string.Empty;
                    using (TravelInsuranceModel traveldb = new TravelInsuranceModel())
                    {
                        string drpdwnvalue = tim.Project_no;
                        string prj_value;
                        if (drpdwnvalue == "Others")
                        { prj_value = tim.Project_otr; }
                        else
                        { prj_value = tim.Project_no; }

                        travel = traveldb.Database.SqlQuery<string>(string.Format(@"select dbo.[GenerateInsurancePDFID]()")).SingleOrDefault();
                        traveldb.tblTravelInsurances.Add(new tblTravelInsurance()
                        {
                           
                            Employee_Code = tim.Employee_Code,
                            Start_Date = tim.Start_Date,
                            Return_Date = tim.Return_Date,
                            DOB = tim.DOB,
                            First_Name = tim.First_Name,
                            Surname = tim.Surname,
                            Gender = tim.Gender,
                            Nominee_Name = tim.Nominee_Name,
                            Passport_Number = tim.Passport_Number,
                            Mobile = tim.Mobile,
                            mail = tim.mail,
                            adhar_card_name = tim.adhar_card_name,
                            disease = tim.disease,

                            Project_no = prj_value,
                            Creation_date = tim.Creation_date,
                            InsuranceCode = travel,
                        });



                        ReportDocument rd = new ReportDocument();
                        rd.Load(Server.MapPath("~/Reports/CrystalReportTravelInsurance.rpt"));
                        rd.SetDataSource(new List<Models.Others.crystalReport.pdfmodel> { new Models.Others.crystalReport.pdfmodel() {

                        Employee_Code = tim.Employee_Code,
                        Start_Date = tim.Start_Date,
                        Return_Date = tim.Return_Date,
                        DOB = tim.DOB,
                        First_Name = tim.First_Name,
                        Surname = tim.Surname,
                        Gender = tim.Gender,
                        Nominee_Name = tim.Nominee_Name,
                        Passport_Number = tim.Passport_Number,
                        Mobile = tim.Mobile,
                        mail = tim.mail,
                        adhar_card_name = tim.adhar_card_name,
                        disease = tim.disease,
                        disease_details=tim.disease_details,
                        project_no=prj_value,
                        Creation_date = tim.Creation_date,
                        InuranceCode=travel,
                    } });


                        var Path = Server.MapPath("~/Reports/PDF/TravelInsurance.pdf");
                        if (System.IO.File.Exists(Path))
                        {
                            System.IO.File.Delete(Path);
                        }
                        rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, Server.MapPath("~/Reports/PDF/TravelInsurance.pdf"));
                        SmtpClient smtp = new SmtpClient();
                        smtp.UseDefaultCredentials = false;
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential("noreply@ioas.iitm.ac.in", "welcomehbs");
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        MailMessage mail = new MailMessage("noreply@ioas.iitm.ac.in", "preethycatherine@gmail.com");
                        //mail.From.Address
                        mail.Subject = "Group Travel Insurance - Reg.";
                        mail.Body = @"Dear Sir / Madam,

The travel insurance details has been submitted successfully.
Mr.Chidambaram, Chief Manager (Admin)[Extn - 9793] or Ms.Hema Sruthi Dama, Secretary to Associate Dean(ICSR)[Extn - 8066] will be processing the travel insurance shortly.
Please contact them for any assistance.


Thanks and Regards,
Chief Manager, Admin";
                        mail.To.Add("preethycatherine@gmail.com".Replace(';', ','));
                        mail.Attachments.Add(new Attachment(Server.MapPath("~/Reports/PDF/TravelInsurance.pdf")));
                        mail.IsBodyHtml = false;

                        try
                        {
                            smtp.Send(mail);
                        }
                        catch (Exception ex)
                        {
                        }
                        bool Uploadsuccess = FTP.Ftpservice.UploadFilen("", Server.MapPath("~/Reports/PDF/TravelInsurance.pdf").ToString(), travel, "Insurance.pdf");

                        if (Uploadsuccess)
                        {
                            traveldb.tblTravelinsurancepdfs.Add(new tblTravelinsurancepdf()
                            {
                                InsuranceCode = travel,
                                PdfPath = string.Format(@"/TravelInsurance /{0}", travel),

                            });
                            traveldb.SaveChanges();
                            return RedirectToAction("DisplayInsurancepdf");
                        }
                        else
                        {

                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error : " + e);
                }
            }
            else {

                return View(tim);
            }
          
            return View();
        }


        public ActionResult DisplayInsurancepdf()
        {
            return View("DisplayInsurancepdf");
        }
    }
}