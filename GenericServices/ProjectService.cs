using IOAS.DataModel;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace IOAS.GenericServices
{
    public class ProjectService
    {
        
        public PagedData<InvoiceSearchResultModel> GetInvoiceList(InvoiceSearchFieldModel model, int page, int pageSize)
        {
            try
            {
                List<InvoiceSearchResultModel> list = new List<InvoiceSearchResultModel>();
                var searchData = new PagedData<InvoiceSearchResultModel>();
                int skiprec = 0;
                if (page == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (page - 1) * pageSize;
                }
                using (var context = new IOASDBEntities())
                {
                    
                    var query = (from I in context.tblProjectInvoice
                                 join prj in context.tblProject on I.ProjectId equals prj.ProjectId
                                 join user in context.vwFacultyStaffDetails on I.PIId equals user.UserId
                                 where ((prj.ProjectType == 2) && (prj.PIName == model.PIName || model.PIName == null) &&
                                 ((I.InvoiceType == model.InvoiceType) || (model.InvoiceType == null))
                                 && ((I.InvoiceDate >= model.FromDate && I.InvoiceDate <= model.ToDate) || (model.FromDate == null && model.ToDate == null))
                                 && (I.InvoiceNumber.Contains(model.InvoiceNumber) || string.IsNullOrEmpty(model.InvoiceNumber))
                                 && (prj.ProjectNumber.Contains(model.ProjectNumber) || string.IsNullOrEmpty(model.ProjectNumber))
                                 && (I.Status != "InActive"))
                                 orderby I.InvoiceId descending
                                 select new { I, prj, user }).Skip(skiprec).Take(pageSize).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            var projectid = query[i].prj.ProjectId;
                            var projectquery = context.tblProject.FirstOrDefault(dup => dup.ProjectId == projectid);
                            //var doc = query[i].PODocs.Split(new char[] { '_' }, 2);
                            list.Add(new InvoiceSearchResultModel()
                            {
                                ProjectNumber = projectquery.ProjectNumber,
                                ProjectId = projectid,
                                ProjectType = projectquery.ProjectType,
                                InvoiceDate = String.Format("{0:ddd dd-MMM-yyyy}", query[i].I.InvoiceDate),
                                InvoiceType = query[i].I.InvoiceType,
                                SACNumber = query[i].I.TaxCode,
                                Service = query[i].I.DescriptionofServices,
                                InvoiceNumber = query[i].I.InvoiceNumber,
                                PIId = projectquery.PIName,
                                InvoiceId = query[i].I.InvoiceId,
                                TotalInvoiceValue = query[i].I.TotalInvoiceValue,
                                ProjectTitle = projectquery.ProjectTitle,
                                Status = query[i].I.Status,
                            });

                        }
                    }
                    var records = (from I in context.tblProjectInvoice
                                   join prj in context.tblProject on I.ProjectId equals prj.ProjectId
                                   join user in context.vwFacultyStaffDetails on I.PIId equals user.UserId
                                   where ((prj.ProjectType == 2) && (prj.PIName == model.PIName || model.PIName == null) &&
                                   ((I.InvoiceType == model.InvoiceType) || (model.InvoiceType == null))
                                   && ((I.InvoiceDate >= model.FromDate && I.InvoiceDate <= model.ToDate) || (model.FromDate == null && model.ToDate == null))
                                   && (I.InvoiceNumber.Contains(model.InvoiceNumber) || string.IsNullOrEmpty(model.InvoiceNumber))
                                   && (prj.ProjectNumber.Contains(model.ProjectNumber) || string.IsNullOrEmpty(model.ProjectNumber))
                                   && (I.Status != "InActive"))
                                   orderby I.InvoiceId descending
                                   select new { I, prj, user }).Count();
                    searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)records / pageSize));
                    searchData.Data = list;
                    searchData.pageSize = pageSize;
                    searchData.visiblePages = 10;
                    searchData.CurrentPage = page;
                }

                return searchData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public PagedData<InvoiceSearchResultModel> GetPIInvoiceList(InvoiceSearchFieldModel model, int page, int pageSize)
        {
            try
            {
                List<InvoiceSearchResultModel> list = new List<InvoiceSearchResultModel>();
                var searchData = new PagedData<InvoiceSearchResultModel>();
                int skiprec = 0;
                if (page == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (page - 1) * pageSize;
                }
                using (var context = new IOASDBEntities())
                {
                    var predicate = PredicateBuilder.BaseAnd<tblProjectInvoice>();
                    if (!string.IsNullOrEmpty(model.InvoiceNumber))
                        predicate = predicate.And(d => d.InvoiceNumber.Contains(model.InvoiceNumber));
                    if (model.InvoiceType != null)
                        predicate = predicate.And(d => d.InvoiceType == model.InvoiceType);                    
                    if (model.FromDate != null && model.ToDate != null)
                        predicate = predicate.And(d => d.InvoiceDate >= model.FromDate && d.InvoiceDate <= model.ToDate);
                    if (model.PIName != null)
                        predicate = predicate.And(d => d.PIId == model.PIName);
                    //if (model.FromSRBDate != null && model.ToSRBDate != null)
                    //    predicate = predicate.And(d => d.InwardDate >= model.FromSRBDate && d.InwardDate <= model.ToSRBDate);
                    var query = context.tblProjectInvoice.Where(predicate).OrderByDescending(m => m.InvoiceId).Skip(skiprec).Take(pageSize).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            var projectid = query[i].ProjectId;
                            var projectquery = context.tblProject.FirstOrDefault(dup => dup.ProjectId == projectid);
                            //var doc = query[i].PODocs.Split(new char[] { '_' }, 2);
                            list.Add(new InvoiceSearchResultModel()
                            {
                                ProjectNumber = projectquery.ProjectNumber,
                                ProjectId = projectid,
                                ProjectType = projectquery.ProjectType,
                                InvoiceDate = String.Format("{0:ddd dd-MMM-yyyy}", query[i].InvoiceDate),
                                InvoiceType = query[i].InvoiceType,
                                SACNumber = query[i].TaxCode,
                                Service = query[i].DescriptionofServices,
                                InvoiceNumber = query[i].InvoiceNumber,
                                PIId = projectquery.PIName,
                                InvoiceId = query[i].InvoiceId,
                                TotalInvoiceValue = query[i].TotalInvoiceValue,
                                ProjectTitle = projectquery.ProjectTitle,
                            });

                        }
                    }
                    var records = context.tblProjectInvoice.Where(predicate).OrderByDescending(m => m.InvoiceId).Count();
                    searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)records / pageSize));
                    searchData.Data = list;
                    searchData.pageSize = pageSize;
                    searchData.visiblePages = 10;
                    searchData.CurrentPage = page;
                }

                return searchData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<CreateInvoiceModel> GetInvoiceList()
        {
            List<CreateInvoiceModel> invoicelist = new List<CreateInvoiceModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from I in context.tblProjectInvoice
                             join P in context.tblProject on I.ProjectId equals P.ProjectId
                             join user in context.vwFacultyStaffDetails on I.PIId equals user.UserId                            
                             orderby I.InvoiceId descending
                             select new { P, I, user.FirstName, user.EmployeeId}).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        invoicelist.Add(new CreateInvoiceModel()
                        {
                            Sno = i + 1,
                            ProjectID = query[i].I.ProjectId,
                            Projecttitle = query[i].P.ProjectTitle,
                            ProjectNumber = query[i].P.ProjectNumber,
                            TotalInvoiceValue = query[i].I.TotalInvoiceValue,
                            InvoiceId = query[i].I.InvoiceId,
                            InvoiceNumber = query[i].I.InvoiceNumber,
                            NameofPI = query[i].FirstName,
                            Invoicedatestrng = String.Format("{0:s}", query[i].I.InvoiceDate)
                        });
                    }
                }
            }
            return invoicelist;
        }
        public static List<MasterlistviewModel> LoadConsProjecttitledetails(int PIId)
        {
            try
           
            {

                List<MasterlistviewModel> Title = new List<MasterlistviewModel>();

                Title.Add(new MasterlistviewModel()
                {
                    id = null,
                    name = "Select Any"
                });
                using (var context = new IOASDBEntities())
                {
                                          
                        var query = (from P in context.tblProject
                                     join U in context.vwFacultyStaffDetails on P.PIName equals U.UserId
                                     where (P.PIName == PIId && P.ProjectType == 2)
                                     orderby P.ProjectId
                                     select new { U.FirstName, P }).ToList();
                        
                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                Title.Add(new MasterlistviewModel()
                                {
                                    id = query[i].P.ProjectId,
                                    name = query[i].P.ProjectNumber + "-" + query[i].P.ProjectTitle + "- " + query[i].FirstName,
                                });
                            }
                        }
                    }
                
                return Title;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<MasterlistviewModel> LoadProjecttitledetails(int projecttype)
        {
            try
            {

                List<MasterlistviewModel> Title = new List<MasterlistviewModel>();

                Title.Add(new MasterlistviewModel()
                {
                    id = null,
                    name = "Select Any"
                });
                using (var context = new IOASDBEntities())
                {
                    if (projecttype == 1 || projecttype == 2)
                    {


                        var query = (from C in context.tblProject
                                     join U in context.vwFacultyStaffDetails on C.PIName equals U.UserId
                                     where (C.Status == "Active" && C.ProjectType == projecttype)
                                     orderby C.ProjectId
                                     select new { U.FirstName, C }).ToList();


                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                Title.Add(new MasterlistviewModel()
                                {
                                    id = query[i].C.ProjectId,
                                    name = query[i].C.ProjectNumber + "-" + query[i].C.ProjectTitle + "- " + query[i].FirstName,
                                });
                            }
                        }

                    }
                }



                return Title;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<MasterlistviewModel> LoadInvoiceList(int ProjectId)
        {
            try
            {
                List<MasterlistviewModel> Title = new List<MasterlistviewModel>();

                Title.Add(new MasterlistviewModel()
                {
                    id = null,
                    name = "Select Any"
                });
                using (var context = new IOASDBEntities())
                {                    
                        var query = (from I in context.tblProjectInvoiceDraft
                                     join P in context.tblProject on I.ProjectId equals P.ProjectId
                                     join U in context.vwFacultyStaffDetails on P.PIName equals U.UserId
                                     where (I.ProjectId == ProjectId && I.Status == "Draft")
                                     orderby I.InvoiceDraftId
                                     select new { U.FirstName, P, I }).ToList();

                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                Title.Add(new MasterlistviewModel()
                                {
                                    id = query[i].I.InvoiceDraftId,
                                    name = query[i].I.DescriptionofServices + "-" + query[i].FirstName,
                                });
                            }
                        }
                    }
                
                return Title;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public CreateInvoiceModel GetProjectDetails(int ProjectID)
        {
            try
            {
                CreateInvoiceModel model = new CreateInvoiceModel();

                using (var context = new IOASDBEntities())
                {

                    var query = (from P in context.tblProject
                                 join user in context.vwFacultyStaffDetails on P.PIName equals user.UserId into g
                                 join agency in context.tblAgencyMaster on P.SponsoringAgency equals agency.AgencyId into i
                                 from user in g.DefaultIfEmpty()
                                 from agency in i.DefaultIfEmpty()
                                 where P.ProjectId == ProjectID
                                 select new { P, user, agency }).FirstOrDefault();
                    var finyear = (from year in context.tblFinYear
                                   where year.CurrentYearFlag == true
                                   select year).FirstOrDefault();
                    var projectid = query.P.ProjectId;
                    var currentfinyear = finyear.Year;
                    if (query != null)
                    {                       
                        model.InvoiceDate = DateTime.Now;
                        model.Invoicedatestrng = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);
                        model.ProjectNumber = query.P.ProjectNumber;
                        model.Projecttitle = query.P.ProjectTitle;
                        model.ProjectID = projectid;                        
                        model.ProjectType = query.P.ProjectType;
                        model.PIDepartmentName = query.user.DepartmentName;
                        model.PIId = query.P.PIName;
                        model.NameofPI = query.user.FirstName;
                        model.SanctionOrderNumber = query.P.SanctionOrderNumber;
                        model.Sanctionvalue = query.P.SanctionValue;
                        model.SponsoringAgency = query.P.SponsoringAgency;
                        model.SponsoringAgencyName = query.agency.AgencyName;
                        model.Agencyregaddress = query.agency.Address;
                        model.Agencydistrict = query.agency.District;
                        model.AgencyPincode = query.agency.PinCode;
                        model.Agencystate = query.agency.State;
                        model.Agencystatecode = Convert.ToInt32(query.agency.StateCode);
                        model.GSTNumber = query.agency.GSTIN;
                        model.PAN = query.agency.PAN;
                        model.TAN = query.agency.TAN;
                        model.Agencycontactperson = query.agency.ContactPerson;
                        model.AgencycontactpersonEmail = query.agency.ContactEmail;
                        model.Agencycontactpersonmobile = query.agency.ContactNumber;
                        model.CommunicationAddress = query.agency.Address;
                        model.TaxStatus = query.P.TaxStatus;
                        model.BankName = query.agency.BankName;
                        model.BankAccountNumber = query.agency.AccountNumber;
                        var agencytype = query.agency.AgencyType;
                        var agencycategory = query.agency.AgencyCountryCategoryId;
                        var indianagencycategory = query.agency.IndianAgencyCategoryId;
                        var nonsezcategory = query.agency.NonSezCategoryId;
                        if (agencycategory == 1)
                        {
                            if (indianagencycategory == 1)
                            {
                                model.InvoiceType = 2;
                            }
                            if (indianagencycategory == 2 && nonsezcategory == 1)
                            {
                                model.InvoiceType = 3;
                            }
                            if (indianagencycategory == 2 && nonsezcategory == 2)
                            {
                                model.InvoiceType = 4;
                            }
                            if (agencytype == 1)
                            {
                                model.InvoiceType = 2;
                            }
                        }
                        if (agencycategory == 2)
                        {
                            model.InvoiceType = 1;
                        }
                       
                        model.CurrentFinancialYear = currentfinyear;
                        model.AvailableBalance = query.P.SanctionValue;
                        Nullable<Decimal> totalinvoicevalue = 0;
                        var invoicequery = (from I in context.tblProjectInvoice
                                            where I.ProjectId == ProjectID
                                            select I).ToList();
                        if (invoicequery.Count() > 0)
                        {
                            Nullable<int>[] _invoiceid = new Nullable<int>[invoicequery.Count];
                            string[] _invoicenumber = new string[invoicequery.Count];
                            Nullable<Decimal>[] _invoicevalue = new Nullable<Decimal>[invoicequery.Count];
                            string[] _invoicedate = new string[invoicequery.Count];
                            
                            for (int i = 0; i < invoicequery.Count(); i++)
                            {
                                _invoiceid[i] = invoicequery[i].InvoiceId;
                                _invoicenumber[i] = invoicequery[i].InvoiceNumber;
                                _invoicevalue[i] = Convert.ToDecimal(invoicequery[i].TotalInvoiceValue);
                                _invoicedate[i] = String.Format("{0:ddd dd-MMM-yyyy}", invoicequery[i].InvoiceDate);
                                totalinvoicevalue += _invoicevalue[i];
                            }
                            model.PreviousInvoiceDate = _invoicedate;
                            model.PreviousInvoiceId = _invoiceid;
                            model.PreviousInvoiceNumber = _invoicenumber;
                            model.PreviousInvoicevalue = _invoicevalue;
                            model.AvailableBalance = model.Sanctionvalue - totalinvoicevalue;
                        }

                        var instalmentquery = (from I in context.tblInstallment
                                            where I.ProjectId == ProjectID
                                            select I).ToList();
                        if (instalmentquery.Count() > 0)
                        {
                            Nullable<int>[] _instalmentid = new Nullable<int>[instalmentquery.Count];
                            Nullable<int>[] _instalmentnumber = new Nullable<int>[instalmentquery.Count];
                            Nullable<int>[] _instalmentyear = new Nullable<int>[instalmentquery.Count];
                            Nullable<Decimal>[] _instalmentvalue = new Nullable<Decimal>[instalmentquery.Count];
                            Nullable<Decimal> _totalinstalmentvalue = 0; 
                            string[] _isInvoiced = new string [instalmentquery.Count];
                            for (int i = 0; i < instalmentquery.Count(); i++)
                            {
                                _instalmentid[i] = instalmentquery[i].InstallmentID;
                                _instalmentnumber[i] = instalmentquery[i].InstallmentNo;
                                _instalmentyear[i] = instalmentquery[i].Year;
                                _isInvoiced[i] = "No";
                                if (query.P.IsYearWiseAllocation == true)
                                {
                                    _instalmentvalue[i] = Convert.ToDecimal(instalmentquery[i].InstallmentValue);
                                    for (int j = 0; j < invoicequery.Count(); j++)
                                    {
                                        if (invoicequery[j].InstalmentNumber == instalmentquery[i].InstallmentNo && invoicequery[j].InstalmentYear == instalmentquery[i].Year)
                                        {
                                            _isInvoiced[i] = "Yes";
                                        }                                        
                                    }
                                }
                                if (query.P.IsYearWiseAllocation != true)
                                {
                                    _instalmentvalue[i] = Convert.ToDecimal(instalmentquery[i].InstallmentValue);
                                    for (int j = 0; j < invoicequery.Count(); j++)
                                    {
                                        if (invoicequery[j].InstalmentNumber == instalmentquery[i].InstallmentNo)
                                        {
                                            _isInvoiced[i] = "Yes";
                                        }                                        
                                    }                                    
                                }
                                _totalinstalmentvalue += _instalmentvalue[i];
                            }
                            model.InstalmentId = _instalmentid;
                            model.InstlmntNumber = _instalmentnumber;
                            model.InstalValue = _instalmentvalue;
                            model.Instalmentyear = _instalmentyear;
                            model.Invoiced = _isInvoiced;
                           
                           // model.AvailableBalance = model.Sanctionvalue - _totalinstalmentvalue;
                        }

                        if (query.P.IsYearWiseAllocation == true)
                        {
                            DateTime startdate = DateTime.Now;
                            DateTime enddate = DateTime.Now;
                            DateTime today = DateTime.Now;

                            if (query.P.ActualStartDate == null)
                            {
                                startdate = (DateTime)query.P.TentativeStartDate;
                                enddate = (DateTime)query.P.TentativeCloseDate;
                                TimeSpan diff_date = today - startdate;
                                int noofdays = diff_date.Days;
                                int years = noofdays / 365;
                                int currentprojectyear = 0;
                                if (years == 0)
                                {
                                    currentprojectyear = 1;
                                }
                                if (years > 0)
                                {
                                    currentprojectyear = years;
                                }
                                if (instalmentquery.Count() > 0)
                                {
                                    var previousinstalmentinvoice = (from ins in context.tblProjectInvoice
                                                                     where ins.ProjectId == ProjectID && ins.InstalmentYear == currentprojectyear
                                                                     orderby ins.InvoiceId descending
                                                                     select ins).FirstOrDefault();
                                    int lastinvoicedinstalment = 0;
                                    decimal? currinstval = 0;
                                    decimal? balincurrinstval = 0;
                                    int currentinstalment = 0;
                                    decimal? totalinsinvvalue = 0;
                                    if (previousinstalmentinvoice != null)
                                    {
                                        lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;

                                        var instalinv = (from ins in context.tblProjectInvoice
                                                         where ins.ProjectId == ProjectID && ins.InstalmentYear == currentprojectyear && ins.InstalmentNumber == lastinvoicedinstalment
                                                         orderby ins.InvoiceId descending
                                                         select ins).ToList();

                                        if (instalinv != null)
                                        {
                                            totalinsinvvalue = instalinv.Select(m => m.TotalInvoiceValue).Sum() ?? 0;
                                        }
                                        var currinstalment = (from ins in context.tblInstallment
                                                              where (ins.ProjectId == projectid && ins.Year == currentprojectyear && ins.InstallmentNo == lastinvoicedinstalment)
                                                              select ins).FirstOrDefault();
                                        if (currinstalment != null)
                                        {
                                            currinstval = currinstalment.InstallmentValue;
                                        }
                                        balincurrinstval = currinstval - totalinsinvvalue;
                                     if (balincurrinstval <= 0)
                                    {
                                        currentinstalment = lastinvoicedinstalment + 1;
                                    }
                                    else if (balincurrinstval > 0)
                                    {
                                        currentinstalment = lastinvoicedinstalment;
                                    }
                                }
                                else if (previousinstalmentinvoice == null)
                                {
                                        currentinstalment = lastinvoicedinstalment + 1;
                                        //var currinstalment = (from ins in context.tblInstallment
                                        //                      where (ins.ProjectId == projectid && ins.Year == currentprojectyear && ins.InstallmentNo == currentinstalment)
                                        //                      select ins).FirstOrDefault();
                                        //balincurrinstval = currinstalment.InstallmentValue;
                                }   
                                    var instalment = (from ins in context.tblInstallment
                                                      where (ins.ProjectId == projectid && ins.Year == currentprojectyear && ins.InstallmentNo == currentinstalment)
                                                      select ins).FirstOrDefault();
                                    if (instalment != null)
                                    {
                                        model.Instlmntyr = instalment.Year;
                                        model.Instalmentnumber = instalment.InstallmentNo;
                                        model.Instalmentvalue = instalment.InstallmentValue;
                                        model.TaxableValue = instalment.InstallmentValue - balincurrinstval;                                        
                                    }
                                    else if (instalment == null)
                                    {
                                        model.Instlmntyr = currentprojectyear;
                                        model.TaxableValue = model.AvailableBalance;
                                    }
                                }
                                else if (instalmentquery.Count() == 0)
                                {
                                     var aloc = (from alloc in context.tblProjectAllocation
                                                where (alloc.ProjectId == projectid)
                                                select alloc).ToList();
                                    var inv = (from alloc in context.tblProjectInvoice
                                               where (alloc.ProjectId == projectid && alloc.InstalmentYear == currentprojectyear)
                                               select alloc).ToList();
                                    var yearallocamt = (from alloc in context.tblProjectAllocation
                                                        where (alloc.ProjectId == projectid && alloc.Year == currentprojectyear)
                                                        select alloc).ToList();
                                    var nextyear = currentprojectyear + 1;
                                    var nextyearalloc = (from alloc in context.tblProjectAllocation
                                                         where (alloc.ProjectId == projectid && alloc.Year == nextyear)
                                                         select alloc).ToList();
                                    decimal? totalyearalloc = 0;
                                    decimal? invtotal = 0;
                                    decimal? totalpjctalloc = 0;
                                    for (int i = 0; i < aloc.Count(); i++)
                                    {
                                        totalpjctalloc += aloc[i].AllocationValue;
                                    }
                                    for (int i = 0; i < inv.Count(); i++)
                                    {
                                        invtotal += inv[i].TotalInvoiceValue;
                                    }
                                    for (int i = 0; i < yearallocamt.Count(); i++)
                                    {
                                        totalyearalloc += yearallocamt[i].AllocationValue;
                                    }
                                    if (invtotal >= 0 && totalyearalloc > 0)
                                    {
                                        model.TaxableValue = totalyearalloc - invtotal;
                                    }
                                    model.Instlmntyr = currentprojectyear;
                                    if (totalpjctalloc != model.Sanctionvalue && model.TaxableValue == 0)
                                    {
                                        model.TaxableValue = model.AvailableBalance;
                                    }
                                    //else if (totalyearalloc <= 0)
                                    //{
                                    //    decimal? nextyrallocamt = 0;
                                    //    for (int i = 0; i < yearallocamt.Count(); i++)
                                    //    {
                                    //        nextyrallocamt += nextyearalloc[i].AllocationValue;
                                    //    }
                                    //    model.TaxableValue = nextyrallocamt - invtotal;
                                    //}

                                    // model.TaxableValue = model.AvailableBalance;
                                }

                            }
                            if (query.P.ActualStartDate != null)
                            {
                                startdate = (DateTime)query.P.ActualStartDate;
                                enddate = (DateTime)query.P.ActuaClosingDate;
                                TimeSpan diff_date = today - startdate;
                                int noofdays = Math.Abs(diff_date.Days);
                                int years = noofdays / 365;
                                int currentprojectyear = 0;
                                if (years == 0)
                                {
                                    currentprojectyear = 1;
                                }
                                if (years > 0)
                                {
                                    currentprojectyear = years;
                                }
                                if (instalmentquery.Count() > 0)
                                {
                                    var previousinstalmentinvoice = (from ins in context.tblProjectInvoice
                                                                     where ins.ProjectId == ProjectID && ins.InstalmentYear == currentprojectyear
                                                                     orderby ins.InvoiceId descending
                                                                     select ins).FirstOrDefault();
                                    int lastinvoicedinstalment = 0;
                                    decimal? currinstval = 0;
                                    decimal? balincurrinstval = 0;
                                    int currentinstalment = 0;
                                    decimal? totalinsinvvalue = 0;
                                    if (previousinstalmentinvoice != null)
                                    {
                                        lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                        var instalinv = (from ins in context.tblProjectInvoice
                                                         where ins.ProjectId == ProjectID && ins.InstalmentYear == currentprojectyear && ins.InstalmentNumber == lastinvoicedinstalment
                                                         orderby ins.InvoiceId descending
                                                         select ins).ToList();

                                        if (instalinv != null)
                                        {
                                            totalinsinvvalue = instalinv.Select(m => m.TotalInvoiceValue).Sum() ?? 0;
                                        }
                                        var currinstalment = (from ins in context.tblInstallment
                                                              where (ins.ProjectId == projectid && ins.Year == currentprojectyear && ins.InstallmentNo == lastinvoicedinstalment)
                                                              select ins).FirstOrDefault();
                                        if (currinstalment != null)
                                        {
                                            currinstval = currinstalment.InstallmentValue;
                                        }
                                        balincurrinstval = currinstval - totalinsinvvalue;
                                        if (balincurrinstval <= 0)
                                        {
                                            currentinstalment = lastinvoicedinstalment + 1;
                                        }
                                        else if (balincurrinstval > 0)
                                        {
                                            currentinstalment = lastinvoicedinstalment;
                                        }
                                    }
                                    else if (previousinstalmentinvoice == null)
                                    {
                                        currentinstalment = lastinvoicedinstalment + 1;
                                        //var currinstalment = (from ins in context.tblInstallment
                                        //                      where (ins.ProjectId == projectid && ins.Year == currentprojectyear && ins.InstallmentNo == currentinstalment)
                                        //                      select ins).FirstOrDefault();
                                        //balincurrinstval = currinstalment.InstallmentValue;
                                    }
                                    var instalment = (from ins in context.tblInstallment
                                                      where (ins.ProjectId == projectid && ins.Year == currentprojectyear && ins.InstallmentNo == currentinstalment)
                                                      select ins).FirstOrDefault();
                                    if (instalment != null)
                                    { 
                                        model.Instlmntyr = instalment.Year;
                                        model.Instalmentnumber = instalment.InstallmentNo;                                        
                                        model.Instalmentvalue = instalment.InstallmentValue;
                                        model.TaxableValue = instalment.InstallmentValue - balincurrinstval;
                                    }
                                    else if (instalment == null)
                                    {
                                        model.TaxableValue = 0;
                                    }
                                }
                                else if (instalmentquery.Count() == 0)
                                {
                                    var aloc = (from alloc in context.tblProjectAllocation
                                                where (alloc.ProjectId == projectid)
                                                select alloc).ToList();
                                    var inv = (from alloc in context.tblProjectInvoice
                                               where (alloc.ProjectId == projectid && alloc.InstalmentYear == currentprojectyear)
                                               select alloc).ToList();
                                    var yearallocamt = (from alloc in context.tblProjectAllocation
                                                        where (alloc.ProjectId == projectid && alloc.Year == currentprojectyear)
                                                        select alloc).ToList();
                                    var nextyear = currentprojectyear + 1;
                                    var nextyearalloc = (from alloc in context.tblProjectAllocation
                                                         where (alloc.ProjectId == projectid && alloc.Year == nextyear)
                                                         select alloc).ToList();
                                    decimal? totalyearalloc = 0;
                                    decimal? invtotal = 0;
                                    decimal? totalpjctalloc = 0;
                                    for (int i = 0; i < aloc.Count(); i++)
                                    {
                                        totalpjctalloc += aloc[i].AllocationValue;
                                    }
                                    for (int i = 0; i < inv.Count(); i++)
                                    {
                                        invtotal += inv[i].TotalInvoiceValue;
                                    }
                                    for (int i = 0; i < yearallocamt.Count(); i++)
                                    {
                                        totalyearalloc += yearallocamt[i].AllocationValue;
                                    }
                                    if (invtotal >= 0 && totalyearalloc > 0)
                                    {
                                        model.TaxableValue = totalyearalloc - invtotal;
                                    }
                                    model.Instlmntyr = currentprojectyear;
                                    if(totalpjctalloc != model.Sanctionvalue && model.TaxableValue == 0)
                                    {
                                        model.TaxableValue = model.AvailableBalance;
                                    }
                                }
                            }
                        }
                        if (query.P.IsYearWiseAllocation != true)
                        {
                            if (instalmentquery.Count() > 0)
                            {
                                var previousinstalmentinvoice = (from ins in context.tblProjectInvoice
                                                                 where ins.ProjectId == ProjectID
                                                                 orderby ins.InvoiceId descending
                                                                 select ins).FirstOrDefault();
                                int lastinvoicedinstalment = 0;
                                decimal? currinstval = 0;
                                decimal? balincurrinstval = 0;
                                int currentinstalment = 0;
                                decimal? totalinsinvvalue = 0;
                                if (previousinstalmentinvoice != null)
                                {
                                    lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                    var instalinv = (from ins in context.tblProjectInvoice
                                                     where ins.ProjectId == ProjectID && ins.InstalmentNumber == lastinvoicedinstalment
                                                     orderby ins.InvoiceId descending
                                                     select ins).ToList();
                                    
                                    if (instalinv != null)
                                    {                                        
                                            totalinsinvvalue = instalinv.Select(m => m.TotalInvoiceValue).Sum() ?? 0;                                       
                                    }
                                    var currinstalment = (from ins in context.tblInstallment
                                                          where (ins.ProjectId == projectid && ins.InstallmentNo == lastinvoicedinstalment)
                                                          select ins).FirstOrDefault();
                                    if (currinstalment != null)
                                    {
                                        currinstval = currinstalment.InstallmentValue;
                                    }
                                    balincurrinstval = currinstval - totalinsinvvalue;
                                    if (balincurrinstval <= 0)
                                    {
                                        currentinstalment = lastinvoicedinstalment + 1;
                                    }
                                    else if (balincurrinstval > 0)
                                    {
                                        currentinstalment = lastinvoicedinstalment;
                                    }
                                }
                                else if (previousinstalmentinvoice == null)
                                {
                                    currentinstalment = lastinvoicedinstalment + 1;
                                }                                
                                var instalment = (from ins in context.tblInstallment
                                                  where (ins.ProjectId == projectid && ins.InstallmentNo == currentinstalment)
                                                  select ins).FirstOrDefault();
                                if (instalment != null)
                                {
                                    model.Instlmntyr = instalment.Year;
                                    model.Instalmentnumber = instalment.InstallmentNo;                                  
                                    model.Instalmentvalue = instalment.InstallmentValue;
                                    model.TaxableValue = instalment.InstallmentValue - balincurrinstval;                                   
                                }
                                else if (instalment == null)
                                {
                                    model.TaxableValue = model.AvailableBalance;
                                }
                            }
                            else if (instalmentquery.Count() == 0)
                            {
                                model.TaxableValue = model.AvailableBalance;
                            }
                        }
                        //if (query.P.ProjectType == 1)
                        //{
                        //    var projectcategory = Convert.ToInt32(query.P.SponProjectCategory);                            
                        //    var bankquery = (from Bank in context.tblIITMBankMaster
                        //                      join account in context.tblBankAccountMaster on Bank.BankId equals account.BankId
                        //                      join projectgroup in context.tblBankProjectGroup on account.BankAccountId equals projectgroup.BankAccountId
                        //                      join cc in context.tblCodeControl on
                        //                      new { pjctgroup = projectcategory, codeName = "SponBankProjectGroup" } equals
                        //                      new { pjctgroup = cc.CodeValAbbr, codeName = cc.CodeName }
                        //                      where projectgroup.ProjectGroup == projectcategory
                        //                      select new { Bank, account, projectgroup, cc }).FirstOrDefault();
                           
                        //    if (bankquery != null)
                        //    {
                        //        model.Bank = bankquery.Bank.BankId;
                        //        model.BankName = bankquery.Bank.BankName;
                        //        model.BankAccountNumber = bankquery.account.AccountNumber;
                        //        model.BankAccountId = bankquery.account.BankAccountId;
                        //    }                            
                        //   // model.TaxableValue = model.AvailableBalance;
                        //}
                        //if (query.P.ProjectType == 2)
                        //{
                        //    var prjctgroup = 0;
                        //    if(query.P.ConsProjectSubType == 1)
                        //    {
                        //        prjctgroup = 3;
                        //    }
                        //    if (query.P.ConsProjectSubType == 2)
                        //    {
                        //        prjctgroup = 4;
                        //    }
                            
                            //var bankquery = (from Bank in context.tblIITMBankMaster
                            //                 join account in context.tblBankAccountMaster on Bank.BankId equals account.BankId
                            //                 join projectgroup in context.tblBankProjectGroup on account.BankAccountId equals projectgroup.BankAccountId
                            //                 join cc in context.tblCodeControl on
                            //                 new { pjctgroup = prjctgroup, codeName = "ConsBankProjectGroup" } equals
                            //                 new { pjctgroup = cc.CodeValAbbr, codeName = cc.CodeName }
                            //                 where projectgroup.ProjectGroup == prjctgroup
                            //                 select new { Bank, account, projectgroup, cc }).FirstOrDefault();
                            //if (bankquery != null)
                            //{
                            //    model.Bank = bankquery.Bank.BankId;
                            //    model.BankName = bankquery.Bank.BankName;
                            //    model.BankAccountNumber = bankquery.account.AccountNumber;
                            //    model.BankAccountId = bankquery.account.BankAccountId;
                            //}
                        //}
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public CreateInvoiceModel GetInvoiceDetails(int InvoiceId)
        {
            try
            {
                CreateInvoiceModel model = new CreateInvoiceModel();

                using (var context = new IOASDBEntities())
                {
                    var invquery = (from inv in context.tblProjectInvoice
                                        where inv.InvoiceId == InvoiceId && inv.Status == "Approval Pending" 
                                    select inv).FirstOrDefault();
                    if (invquery != null)
                    {
                        var projectid = invquery.ProjectId;
                        var query = (from P in context.tblProject
                                         //join cc in context.tblCodeControl on
                                         //new { Category = srb.ItemCategoryId ?? 0, codeName = "SRB_ItemCategory" } equals
                                         //new { Category = cc.CodeValAbbr, codeName = cc.CodeName }
                                     join user in context.vwFacultyStaffDetails on P.PIName equals user.UserId into g                                 
                                     join agency in context.tblAgencyMaster on P.SponsoringAgency equals agency.AgencyId into i
                                     from user in g.DefaultIfEmpty()
                                     from agency in i.DefaultIfEmpty()
                                     where P.ProjectId == projectid
                                     select new { P, user, agency }).FirstOrDefault();
                        var finyear = (from year in context.tblFinYear
                                       where year.CurrentYearFlag == true
                                       select year).FirstOrDefault();
                        var currentfinyear = finyear.Year;

                    
                        model.CurrentFinancialYear = currentfinyear;
                        model.InvoiceDate = invquery.InvoiceDate;
                        model.InvoiceId = InvoiceId;
                        model.InvoiceNumber = invquery.InvoiceNumber;
                        model.DescriptionofServices = invquery.DescriptionofServices;
                        model.ServiceType = invquery.ServiceType;
                        model.SACNumber = invquery.TaxCode;
                        model.TaxableValue = invquery.TaxableValue;
                        model.TotalInvoiceValue = invquery.TotalInvoiceValue;
                        model.Invoicedatestrng = String.Format("{0:ddd dd-MMM-yyyy}", invquery.InvoiceDate);
                        model.ProjectNumber = query.P.ProjectNumber;
                        model.ProjectID = query.P.ProjectId;
                        model.ProjectType = query.P.ProjectType;
                        model.Projecttitle = query.P.ProjectTitle;
                     //   model.Department = query.user.DepartmentCode;
                        model.PIDepartmentName = query.user.DepartmentName;
                        model.PIId = query.P.PIName;
                        model.NameofPI = query.user.FirstName;
                        model.SanctionOrderNumber = query.P.SanctionOrderNumber;
                        model.Sanctionvalue = query.P.SanctionValue;
                        model.SponsoringAgency = query.P.SponsoringAgency;
                        model.SponsoringAgencyName = query.agency.AgencyName;
                        model.Agencyregaddress = query.agency.Address;
                        model.Agencydistrict = query.agency.District;
                        model.AgencyPincode = query.agency.PinCode;
                        model.Agencystate = query.agency.State;
                        model.Agencystatecode = Convert.ToInt32(query.agency.StateCode);
                        model.GSTNumber = query.agency.GSTIN;
                        model.PAN = query.agency.PAN;
                        model.TAN = query.agency.TAN;
                        model.Agencycontactperson = query.agency.ContactPerson;
                        model.AgencycontactpersonEmail = query.agency.ContactEmail;
                        model.Agencycontactpersonmobile = query.agency.ContactNumber;
                        model.CommunicationAddress = invquery.CommunicationAddress;
                        model.PONumber = invquery.PONumber;
                        model.InvoiceType = invquery.InvoiceType;
                        //model.Bank = query.agency.BankId;
                        model.BankName = query.agency.BankName;
                        model.BankAccountNumber = query.agency.AccountNumber;
                        //model.BankAccountId = query.agency.BankAccountId;
                        Nullable<Decimal> totalinvoicevalue = 0;
                        var invoicequery = (from I in context.tblProjectInvoice
                                            where I.ProjectId == projectid
                                            select I).ToList();
                        if (invoicequery.Count() > 0)
                        {
                            Nullable<int>[] _invoiceid = new Nullable<int>[invoicequery.Count];
                            string[] _invoicenumber = new string[invoicequery.Count];
                            Nullable<Decimal>[] _invoicevalue = new Nullable<Decimal>[invoicequery.Count];
                            string[] _invoicedate = new string[invoicequery.Count];

                            for (int i = 0; i < invoicequery.Count(); i++)
                            {
                                _invoiceid[i] = invoicequery[i].InvoiceId;
                                _invoicenumber[i] = invoicequery[i].InvoiceNumber;
                                _invoicevalue[i] = Convert.ToDecimal(invoicequery[i].TotalInvoiceValue);
                                _invoicedate[i] = String.Format("{0:ddd dd-MMM-yyyy}", invoicequery[i].InvoiceDate);
                                totalinvoicevalue += _invoicevalue[i];
                            }
                            model.PreviousInvoiceDate = _invoicedate;
                            model.PreviousInvoiceId = _invoiceid;
                            model.PreviousInvoiceNumber = _invoicenumber;
                            model.PreviousInvoicevalue = _invoicevalue;
                            model.AvailableBalance = model.Sanctionvalue - totalinvoicevalue;
                            if(model.AvailableBalance < 0)
                            {
                                model.AvailableBalance = 0;
                            }
                        }
                        var instalmentquery = (from I in context.tblInstallment
                                               where I.ProjectId == projectid
                                               select I).ToList();
                        if (instalmentquery.Count() > 0)
                        {
                            Nullable<int>[] _instalmentid = new Nullable<int>[instalmentquery.Count];
                            Nullable<int>[] _instalmentnumber = new Nullable<int>[instalmentquery.Count];
                            Nullable<int>[] _instalmentyear = new Nullable<int>[instalmentquery.Count];
                            Nullable<Decimal>[] _instalmentvalue = new Nullable<Decimal>[instalmentquery.Count];
                            string[] _isInvoiced = new string[instalmentquery.Count];
                            for (int i = 0; i < instalmentquery.Count(); i++)
                            {
                                _instalmentid[i] = instalmentquery[i].InstallmentID;
                                _instalmentnumber[i] = instalmentquery[i].InstallmentNo;
                                _instalmentyear[i] = instalmentquery[i].Year;
                                _isInvoiced[i] = "No";
                                if (query.P.IsYearWiseAllocation == true)
                                {
                                    _instalmentvalue[i] = Convert.ToDecimal(instalmentquery[i].InstallmentValue);
                                    for (int j = 0; j < invoicequery.Count(); j++)
                                    {
                                        if (invoicequery[j].InstalmentNumber == instalmentquery[i].InstallmentNo && invoicequery[j].InstalmentYear == instalmentquery[i].Year)
                                        {
                                            _isInvoiced[i] = "Yes";
                                        }
                                    }
                                }
                                if (query.P.IsYearWiseAllocation != true)
                                {
                                    _instalmentvalue[i] = Convert.ToDecimal(instalmentquery[i].InstallmentValue);
                                    for (int j = 0; j < invoicequery.Count(); j++)
                                    {
                                        if (invoicequery[j].InstalmentNumber == instalmentquery[i].InstallmentNo)
                                        {
                                            _isInvoiced[i] = "Yes";
                                        }
                                    }
                                }
                                //  totalinvoicevalue += _invoicevalue[i];
                            }
                            model.InstalmentId = _instalmentid;
                            model.InstlmntNumber = _instalmentnumber;
                            model.InstalValue = _instalmentvalue;
                            model.Instalmentyear = _instalmentyear;
                            model.Invoiced = _isInvoiced;
                            // model.AvailableBalance = model.Sanctionvalue - totalinvoicevalue;
                        }
                        //if (query.P.ProjectType == 1)
                        //{
                        //    var projectcategory = Convert.ToInt32(query.P.SponProjectCategory);
                        //    var bankquery = (from Bank in context.tblIITMBankMaster
                        //                     join account in context.tblBankAccountMaster on Bank.BankId equals account.BankId
                        //                     join projectgroup in context.tblBankProjectGroup on account.BankAccountId equals projectgroup.BankAccountId
                        //                     join cc in context.tblCodeControl on
                        //                     new { pjctgroup = projectcategory, codeName = "SponBankProjectGroup" } equals
                        //                     new { pjctgroup = cc.CodeValAbbr, codeName = cc.CodeName }
                        //                     where projectgroup.ProjectGroup == projectcategory
                        //                     select new { Bank, account, projectgroup, cc }).FirstOrDefault();
                        //    if (bankquery != null)
                        //    {
                        //        model.Bank = bankquery.Bank.BankId;
                        //        model.BankName = bankquery.Bank.BankName;
                        //        model.BankAccountNumber = bankquery.account.AccountNumber;
                        //        model.BankAccountId = bankquery.account.BankAccountId;
                        //    }
                        //}
                        if (query.P.ProjectType == 2)
                        {
                            var projectcategory = Convert.ToInt32(query.P.ConsProjectSubType);
                            var balanceinstvalue = query.P.SanctionValue - totalinvoicevalue;
                            model.Instalmentvalue = balanceinstvalue;
                            model.Instalmentnumber = 1;
                            model.Instlmntyr = 1;
                            if (query.P.IsYearWiseAllocation == true)
                            {
                                DateTime startdate = DateTime.Now;
                                DateTime enddate = DateTime.Now;
                                DateTime today = DateTime.Now;

                                if (query.P.ActualStartDate == null)
                                {
                                    startdate = (DateTime)query.P.TentativeStartDate;
                                    enddate = (DateTime)query.P.TentativeCloseDate;
                                    TimeSpan diff_date = today - startdate;
                                    int noofdays = diff_date.Days;
                                    int years = noofdays / 365;
                                    int currentprojectyear = 0;
                                    if (years == 0)
                                    {
                                        currentprojectyear = 1;
                                    }
                                    if (years > 0)
                                    {
                                        currentprojectyear = years;
                                    }
                                    var instalment = (from ins in context.tblInstallment
                                                      where (ins.ProjectId == projectid && ins.Year == currentprojectyear)
                                                      select ins).FirstOrDefault();
                                    if (instalment != null)
                                    {
                                        model.Instalmentnumber = instalment.InstallmentNo;
                                        var balanceinstalment = instalment.InstallmentValueForYear - totalinvoicevalue;
                                        model.Instalmentvalue = balanceinstalment;
                                        model.Instlmntyr = instalment.Year;
                                    }
                                  }
                                if (query.P.ActualStartDate != null)
                                {
                                    startdate = (DateTime)query.P.ActualStartDate;
                                    enddate = (DateTime)query.P.ActuaClosingDate;
                                    TimeSpan diff_date = today - startdate;
                                    int noofdays = diff_date.Days;
                                    int years = noofdays / 365;
                                    int currentprojectyear = 0;
                                    if (years == 0)
                                    {
                                        currentprojectyear = 1;
                                    }
                                    if (years > 0)
                                    {
                                        currentprojectyear = years;
                                    }

                                    var instalment = (from ins in context.tblInstallment
                                                      where (ins.ProjectId == projectid && ins.Year == currentprojectyear)
                                                      select ins).FirstOrDefault();
                                    if (instalment != null)
                                    {
                                        model.Instalmentnumber = instalment.InstallmentNo;
                                        var balanceinstalment = instalment.InstallmentValueForYear - totalinvoicevalue;
                                        model.Instalmentvalue = balanceinstalment;
                                        model.Instlmntyr = instalment.Year;
                                    }
                                }
                            }
                            if (query.P.IsYearWiseAllocation != true)
                            {
                                var instalment = (from ins in context.tblInstallment
                                                  where (ins.ProjectId == projectid)
                                                  select ins).FirstOrDefault();
                                if (instalment != null)
                                {
                                    var balanceinstalment = instalment.InstallmentValue - totalinvoicevalue;
                                    model.Instalmentvalue = balanceinstalment;
                                    model.Instalmentnumber = 1;
                                    model.Instlmntyr = 1;
                                }
                            }
                            //var bankquery = (from Bank in context.tblIITMBankMaster
                            //                 join account in context.tblBankAccountMaster on Bank.BankId equals account.BankId
                            //                 join projectgroup in context.tblBankProjectGroup on account.BankAccountId equals projectgroup.BankAccountId
                            //                 join cc in context.tblCodeControl on
                            //                 new { pjctgroup = projectcategory, codeName = "ConsBankProjectGroup" } equals
                            //                 new { pjctgroup = cc.CodeValAbbr, codeName = cc.CodeName }
                            //                 select new { Bank, account, projectgroup, cc }).FirstOrDefault();
                            //if (bankquery != null)
                            //{
                            //    model.Bank = bankquery.Bank.BankId;
                            //    model.BankName = bankquery.Bank.BankName;
                            //    model.BankAccountNumber = bankquery.account.AccountNumber;
                            //    model.BankAccountId = bankquery.account.BankAccountId;
                            //}
                        }
                    }                    
                }

                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public CreateInvoiceModel GetInvoiceDraftDetails(int DraftId)
        {
            try
            {
                CreateInvoiceModel model = new CreateInvoiceModel();

                using (var context = new IOASDBEntities())
                {
                    var invquery = (from inv in context.tblProjectInvoiceDraft
                                    where inv.InvoiceDraftId == DraftId
                                    select inv).FirstOrDefault();
                    var projectid = invquery.ProjectId;
                    var query = (from P in context.tblProject
                                     //join cc in context.tblCodeControl on
                                     //new { Category = srb.ItemCategoryId ?? 0, codeName = "SRB_ItemCategory" } equals
                                     //new { Category = cc.CodeValAbbr, codeName = cc.CodeName }
                                 join user in context.vwFacultyStaffDetails on P.PIName equals user.UserId into g
                                 join agency in context.tblAgencyMaster on P.SponsoringAgency equals agency.AgencyId into i
                                 from user in g.DefaultIfEmpty()
                                 from agency in i.DefaultIfEmpty()
                                 where P.ProjectId == projectid
                                 select new { P, user, agency }).FirstOrDefault();
                    var finyear = (from year in context.tblFinYear
                                   where year.CurrentYearFlag == true
                                   select year).FirstOrDefault();

                    var currentfinyear = finyear.Year;
                    if (query != null)
                    {
                        model.CurrentFinancialYear = currentfinyear;
                        model.InvoiceDate = invquery.InvoiceDate;
                        // model.InvoiceId = DraftId;
                        model.InvoiceDraftId = DraftId;
                        model.DescriptionofServices = invquery.DescriptionofServices;
                        model.ServiceType = invquery.ServiceType;
                        model.TaxableValue = invquery.TaxableValue;
                        model.PONumber = invquery.PONumber;
                        model.CommunicationAddress = invquery.CommunicationAddress;
                        //  model.TotalInvoiceValue = invquery.TotalInvoiceValue;
                        model.Invoicedatestrng = String.Format("{0:ddd dd-MMM-yyyy}", invquery.InvoiceDate);
                        model.ProjectNumber = query.P.ProjectNumber;
                        model.ProjectID = query.P.ProjectId;
                        model.Projecttitle = query.P.ProjectTitle;
                        model.ProjectType = query.P.ProjectType;
                       // model.Department = query.user.DepartmentId;
                        model.PIDepartmentName = query.user.DepartmentName;
                        model.PIId = query.P.PIName;
                        model.NameofPI = query.user.FirstName;
                        model.SanctionOrderNumber = query.P.SanctionOrderNumber;
                        model.Sanctionvalue = query.P.SanctionValue;
                        model.SponsoringAgency = query.P.SponsoringAgency;
                        model.SponsoringAgencyName = query.agency.AgencyName;
                        model.Agencyregaddress = query.agency.Address;
                        model.Agencydistrict = query.agency.District;
                        model.AgencyPincode = query.agency.PinCode;
                        model.Agencystate = query.agency.State;
                        model.Agencystatecode = Convert.ToInt32(query.agency.StateCode);
                        model.GSTNumber = query.agency.GSTIN;
                        model.PAN = query.agency.PAN;
                        model.TAN = query.agency.TAN;
                        model.Instlmntyr = invquery.InstalmentYear;
                        model.Instalmentnumber = invquery.InstalmentYear;
                        model.Agencycontactperson = query.agency.ContactPerson;
                        model.AgencycontactpersonEmail = query.agency.ContactEmail;
                        model.Agencycontactpersonmobile = query.agency.ContactNumber;
                        model.CommunicationAddress = query.agency.Address;
                        model.InvoiceType = invquery.InvoiceType;
                        Nullable<Decimal> totalinvoicevalue = 0;
                        var invoicequery = (from I in context.tblProjectInvoice
                                            where I.ProjectId == projectid
                                            select I).ToList();
                        if (invoicequery.Count() > 0)
                        {
                            Nullable<int>[] _invoiceid = new Nullable<int>[invoicequery.Count];
                            string[] _invoicenumber = new string[invoicequery.Count];
                            Nullable<Decimal>[] _invoicevalue = new Nullable<Decimal>[invoicequery.Count];
                            string[] _invoicedate = new string[invoicequery.Count];
                            for (int i = 0; i < invoicequery.Count(); i++)
                            {
                                _invoiceid[i] = invoicequery[i].InvoiceId;
                                _invoicenumber[i] = invoicequery[i].InvoiceNumber;
                                _invoicevalue[i] = Convert.ToDecimal(invoicequery[i].TotalInvoiceValue);
                                _invoicedate[i] = String.Format("{0:ddd dd-MMM-yyyy}", invoicequery[i].InvoiceDate);
                                totalinvoicevalue += _invoicevalue[i];
                            }
                            model.PreviousInvoiceDate = _invoicedate;
                            model.PreviousInvoiceId = _invoiceid;
                            model.PreviousInvoiceNumber = _invoicenumber;
                            model.PreviousInvoicevalue = _invoicevalue;
                            model.AvailableBalance = model.Sanctionvalue - totalinvoicevalue;
                        }
                        var instalmentquery = (from I in context.tblInstallment
                                               where I.ProjectId == projectid
                                               select I).ToList();
                        if (instalmentquery.Count() > 0)
                        {
                            Nullable<int>[] _instalmentid = new Nullable<int>[instalmentquery.Count];
                            Nullable<int>[] _instalmentnumber = new Nullable<int>[instalmentquery.Count];
                            Nullable<int>[] _instalmentyear = new Nullable<int>[instalmentquery.Count];
                            Nullable<Decimal>[] _instalmentvalue = new Nullable<Decimal>[instalmentquery.Count];
                            string[] _isInvoiced = new string[instalmentquery.Count];
                            for (int i = 0; i < instalmentquery.Count(); i++)
                            {
                                _instalmentid[i] = instalmentquery[i].InstallmentID;
                                _instalmentnumber[i] = instalmentquery[i].InstallmentNo;
                                _instalmentyear[i] = instalmentquery[i].Year;
                                _isInvoiced[i] = "No";
                                if (query.P.IsYearWiseAllocation == true)
                                {
                                    _instalmentvalue[i] = Convert.ToDecimal(instalmentquery[i].InstallmentValue);
                                    for (int j = 0; j < invoicequery.Count(); j++)
                                    {
                                        if (invoicequery[j].InstalmentNumber == instalmentquery[i].InstallmentNo && invoicequery[j].InstalmentYear == instalmentquery[i].Year)
                                        {
                                            _isInvoiced[i] = "Yes";
                                        }
                                    }
                                }
                                if (query.P.IsYearWiseAllocation != true)
                                {
                                    _instalmentvalue[i] = Convert.ToDecimal(instalmentquery[i].InstallmentValue);
                                    for (int j = 0; j < invoicequery.Count(); j++)
                                    {
                                        if (invoicequery[j].InstalmentNumber == instalmentquery[i].InstallmentNo)
                                        {
                                            _isInvoiced[i] = "Yes";
                                        }
                                    }
                                }
                                //  totalinvoicevalue += _invoicevalue[i];
                            }
                            model.InstalmentId = _instalmentid;
                            model.InstlmntNumber = _instalmentnumber;
                            model.InstalValue = _instalmentvalue;
                            model.Instalmentyear = _instalmentyear;
                            model.Invoiced = _isInvoiced;
                            // model.AvailableBalance = model.Sanctionvalue - totalinvoicevalue;
                        }

                        if (query.P.IsYearWiseAllocation == true)
                        {
                            DateTime startdate = DateTime.Now;
                            DateTime enddate = DateTime.Now;
                            DateTime today = DateTime.Now;

                            if (query.P.ActualStartDate == null)
                            {
                                startdate = (DateTime)query.P.TentativeStartDate;
                                enddate = (DateTime)query.P.TentativeCloseDate;
                                TimeSpan diff_date = today - startdate;
                                int noofdays = diff_date.Days;
                                int years = noofdays / 365;
                                int currentprojectyear = 0;
                                if (years == 0)
                                {
                                    currentprojectyear = 1;
                                }
                                if (years > 0)
                                {
                                    currentprojectyear = years;
                                }
                                if (instalmentquery.Count() > 0)
                                {
                                    var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
                                                                     where ins.ProjectId == projectid && ins.InstalmentYear == currentprojectyear
                                                                     orderby ins.InvoiceId descending
                                                                     select ins).FirstOrDefault();
                                    int lastinvoicedinstalment = 0;
                                    if (previousinstalmentinvoice != null)
                                    {
                                        lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                    }
                                    int currentinstalment = lastinvoicedinstalment + 1;
                                    var instalment = (from ins in context.tblInstallment
                                                      where (ins.ProjectId == projectid && ins.Year == currentprojectyear && ins.InstallmentNo == currentinstalment)
                                                      select ins).FirstOrDefault();
                                    if (instalment != null)
                                    {
                                        model.Instlmntyr = instalment.Year;
                                        model.Instalmentnumber = instalment.InstallmentNo;
                                        model.Instalmentvalue = instalment.InstallmentValue;
                                        model.TaxableValue = instalment.InstallmentValue;
                                    }
                                    else if (instalment == null)
                                    {
                                        model.TaxableValue = 0;
                                    }
                                }
                                else if (instalmentquery.Count() == 0)
                                {
                                    model.TaxableValue = model.AvailableBalance;
                                }

                            }
                            if (query.P.ActualStartDate != null)
                            {
                                startdate = (DateTime)query.P.ActualStartDate;
                                enddate = (DateTime)query.P.ActuaClosingDate;
                                TimeSpan diff_date = today - startdate;
                                int noofdays = diff_date.Days;
                                int years = noofdays / 365;
                                int currentprojectyear = 0;
                                if (years == 0)
                                {
                                    currentprojectyear = 1;
                                }
                                if (years > 0)
                                {
                                    currentprojectyear = years;
                                }
                                if (instalmentquery.Count() > 0)
                                {
                                    var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
                                                                     where ins.ProjectId == projectid && ins.InstalmentYear == currentprojectyear
                                                                     orderby ins.InvoiceId descending
                                                                     select ins).FirstOrDefault();
                                    int lastinvoicedinstalment = 0;
                                    if (previousinstalmentinvoice != null)
                                    {
                                        lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                    }
                                    int currentinstalment = lastinvoicedinstalment + 1;
                                    var instalment = (from ins in context.tblInstallment
                                                      where (ins.ProjectId == projectid && ins.Year == currentprojectyear && ins.InstallmentNo == currentinstalment)
                                                      select ins).FirstOrDefault();
                                    if (instalment != null)
                                    {
                                        model.Instlmntyr = instalment.Year;
                                        model.Instalmentnumber = instalment.InstallmentNo;
                                        model.Instalmentvalue = instalment.InstallmentValue;
                                        model.TaxableValue = instalment.InstallmentValue;
                                    }
                                    else if (instalment == null)
                                    {
                                        model.TaxableValue = 0;
                                    }
                                }
                                else if (instalmentquery.Count() == 0)
                                {
                                    model.TaxableValue = model.AvailableBalance;
                                }
                            }
                        }
                        if (query.P.IsYearWiseAllocation != true)
                        {
                            if (instalmentquery.Count() > 0)
                            {
                                var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
                                                                 where ins.ProjectId == projectid
                                                                 orderby ins.InvoiceId descending
                                                                 select ins).FirstOrDefault();
                                int lastinvoicedinstalment = 0;
                                if (previousinstalmentinvoice != null)
                                {
                                    lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                }
                                int currentinstalment = lastinvoicedinstalment + 1;
                                var instalment = (from ins in context.tblInstallment
                                                  where (ins.ProjectId == projectid && ins.InstallmentNo == currentinstalment)
                                                  select ins).FirstOrDefault();
                                if (instalment != null)
                                {
                                    model.Instlmntyr = instalment.Year;
                                    model.Instalmentnumber = instalment.InstallmentNo;
                                    model.Instalmentvalue = instalment.InstallmentValue;
                                    model.TaxableValue = instalment.InstallmentValue;
                                }
                                else if (instalment == null)
                                {
                                    model.TaxableValue = 0;
                                }
                            }
                            else if (instalmentquery.Count() == 0)
                            {
                                model.TaxableValue = model.AvailableBalance;
                            }
                        }
                        if (query.P.ProjectType == 1)
                        {
                            var projectcategory = Convert.ToInt32(query.P.SponProjectCategory);
                            var bankquery = (from Bank in context.tblIITMBankMaster
                                             join account in context.tblBankAccountMaster on Bank.BankId equals account.BankId
                                             join projectgroup in context.tblBankProjectGroup on account.BankAccountId equals projectgroup.BankAccountId
                                             join cc in context.tblCodeControl on
                                             new { pjctgroup = projectcategory, codeName = "SponBankProjectGroup" } equals
                                             new { pjctgroup = cc.CodeValAbbr, codeName = cc.CodeName }
                                             where projectgroup.ProjectGroup == projectcategory
                                             select new { Bank, account, projectgroup, cc }).FirstOrDefault();
                            if (bankquery != null)
                            {
                                model.Bank = bankquery.Bank.BankId;
                                model.BankName = bankquery.Bank.BankName;
                                model.BankAccountNumber = bankquery.account.AccountNumber;
                                model.BankAccountId = bankquery.account.BankAccountId;
                            }
                        }
                        if (query.P.ProjectType == 2)
                        {
                            var projectcategory = Convert.ToInt32(query.P.ConsProjectSubType);                            
                            var bankquery = (from Bank in context.tblIITMBankMaster
                                             join account in context.tblBankAccountMaster on Bank.BankId equals account.BankId
                                             join projectgroup in context.tblBankProjectGroup on account.BankAccountId equals projectgroup.BankAccountId
                                             join cc in context.tblCodeControl on
                                             new { pjctgroup = projectcategory, codeName = "ConsBankProjectGroup" } equals
                                             new { pjctgroup = cc.CodeValAbbr, codeName = cc.CodeName }
                                             select new { Bank, account, projectgroup, cc }).FirstOrDefault();
                            if (bankquery != null)
                            {
                                model.Bank = bankquery.Bank.BankId;
                                model.BankName = bankquery.Bank.BankName;
                                model.BankAccountNumber = bankquery.account.AccountNumber;
                                model.BankAccountId = bankquery.account.BankAccountId;
                            }
                        }
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static MasterlistviewModel gettaxpercentage(int servicetype)
        {
            try
            {

                MasterlistviewModel taxpercent = new MasterlistviewModel();

                using (var context = new IOASDBEntities())
                {
                    taxpercent.id = null;
                    taxpercent.name = "";
                    if (servicetype > 0)
                    {
                        var query = (from C in context.tblTaxMaster
                                     where (C.TaxMasterId == servicetype)
                                     select C).FirstOrDefault();
                        if (query != null)
                        {
                            taxpercent.id = query.TaxMasterId;
                            taxpercent.name = Convert.ToString(query.TaxRate);
                            taxpercent.code = query.TaxCode;
                        }
                    }


                }

                return taxpercent;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public int CreateInvoice(CreateInvoiceModel model, int LoggedInUser)
        {
            
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                 {
                    try
                    {
                        int InvoiceId = 0;
                        // Edit Invoice function start
                        if (model.InvoiceId > 0)
                        {
                            var query = context.tblProjectInvoice.FirstOrDefault(m => m.InvoiceId == model.InvoiceId);
                            var taxquery = context.tblInvoiceTaxDetails.FirstOrDefault(m => m.InvoiceId == model.InvoiceId);

                            // Validation for EditInvoice details
                            if (query != null)
                            {
                                var ProjectId = model.ProjectID;
                                var Pjctquery = (from P in context.tblProject
                                                 join user in context.vwFacultyStaffDetails on P.PIName equals user.UserId into g
                                                 join agency in context.tblAgencyMaster on P.SponsoringAgency equals agency.AgencyId into i
                                                 from user in g.DefaultIfEmpty()
                                                 from agency in i.DefaultIfEmpty()
                                                 where P.ProjectId == ProjectId
                                                 select new { P, user, agency }).FirstOrDefault();

                                var invoicequery = (from I in context.tblProjectInvoice
                                                    where I.ProjectId == ProjectId && I.InvoiceId != model.InvoiceId
                                                    select I).ToList();
                                Nullable<Decimal> totalprevinvoicevalue = 0;
                                var AvailableBalance = Pjctquery.P.SanctionValue;
                                Nullable<Decimal> TaxableValue = 0;
                                if (invoicequery.Count() > 0)
                                {
                                    Nullable<int>[] _invoiceid = new Nullable<int>[invoicequery.Count];
                                    string[] _invoicenumber = new string[invoicequery.Count];
                                    Nullable<Decimal>[] _invoicevalue = new Nullable<Decimal>[invoicequery.Count];
                                    string[] _invoicedate = new string[invoicequery.Count];

                                    for (int i = 0; i < invoicequery.Count(); i++)
                                    {
                                        _invoiceid[i] = invoicequery[i].InvoiceId;
                                        _invoicenumber[i] = invoicequery[i].InvoiceNumber;
                                        _invoicevalue[i] = Convert.ToDecimal(invoicequery[i].TotalInvoiceValue);
                                        _invoicedate[i] = String.Format("{0:ddd dd-MMM-yyyy}", invoicequery[i].InvoiceDate);
                                        totalprevinvoicevalue += _invoicevalue[i];
                                    }

                                    AvailableBalance = model.Sanctionvalue - totalprevinvoicevalue;
                                }

                                var instalmentquery = (from I in context.tblInstallment
                                                       where I.ProjectId == ProjectId
                                                       select I).ToList();

                                if (Pjctquery.P.IsYearWiseAllocation == true)
                                {
                                    DateTime startdate = DateTime.Now;
                                    DateTime enddate = DateTime.Now;
                                    DateTime today = DateTime.Now;

                                    startdate = (DateTime)Pjctquery.P.TentativeStartDate;
                                    enddate = (DateTime)Pjctquery.P.TentativeCloseDate;
                                    TimeSpan diff_date = today - startdate;
                                    int noofdays = diff_date.Days;
                                    int years = noofdays / 365;
                                    int currentprojectyear = 0;
                                    if (years == 0)
                                    {
                                        currentprojectyear = 1;
                                    }
                                    if (years > 0)
                                    {
                                        currentprojectyear = years;
                                    }
                                    if (instalmentquery.Count() > 0)
                                    {
                                        var previousinstalmentinvoice = (from ins in context.tblProjectInvoice
                                                                         where ins.ProjectId == ProjectId && ins.InvoiceId == model.InvoiceId
                                                                         // orderby ins.InvoiceId descending
                                                                         select ins).SingleOrDefault();
                                        int lastinvoicedinstalment = 1;
                                        if (previousinstalmentinvoice != null)
                                        {
                                            lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                        }
                                        //  int currentinstalment = lastinvoicedinstalment + 1;
                                        var instalment = (from ins in context.tblInstallment
                                                          where (ins.ProjectId == ProjectId && ins.Year == currentprojectyear && ins.InstallmentNo == lastinvoicedinstalment)
                                                          select ins).FirstOrDefault();
                                        if (instalment != null)
                                        {
                                            TaxableValue = instalment.InstallmentValue;
                                        }
                                        else if (instalment == null)
                                        {
                                            TaxableValue = AvailableBalance;
                                        }
                                        if (model.TaxableValue > TaxableValue)
                                        {
                                            return -4;
                                        }

                                    }
                                    else if (instalmentquery.Count() == 0)
                                    {
                                        TaxableValue = AvailableBalance;
                                        if (model.TaxableValue > TaxableValue)
                                        {
                                            return -4;
                                        }
                                    }
                                }

                                if (Pjctquery.P.IsYearWiseAllocation != true)
                                {
                                    if (instalmentquery.Count() > 0)
                                    {
                                        var previousinstalmentinvoice = (from ins in context.tblProjectInvoice
                                                                         where ins.ProjectId == ProjectId && ins.InvoiceId == model.InvoiceId
                                                                         //orderby ins.InvoiceId descending
                                                                         select ins).SingleOrDefault();
                                        int lastinvoicedinstalment = 1;
                                        if (previousinstalmentinvoice != null)
                                        {
                                            lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                        }
                                        // int currentinstalment = lastinvoicedinstalment + 1;
                                        var instalment = (from ins in context.tblInstallment
                                                          where (ins.ProjectId == ProjectId && ins.InstallmentNo == lastinvoicedinstalment)
                                                          select ins).FirstOrDefault();
                                        if (instalment != null)
                                        {
                                            TaxableValue = instalment.InstallmentValue;
                                        }
                                        else if (instalment == null)
                                        {
                                            TaxableValue = AvailableBalance;
                                        }
                                        if (model.TaxableValue > TaxableValue)
                                        {
                                            return -4;
                                        }
                                    }
                                    else if (instalmentquery.Count() == 0)
                                    {
                                        TaxableValue = AvailableBalance;
                                        if (model.TaxableValue > TaxableValue)
                                        {
                                            return -4;
                                        }
                                    }
                                }
                                // Validation ends and save edit in table                
                                query.UpdtUserId = LoggedInUser;
                                query.UpdtTS = DateTime.Now;
                                query.PONumber = model.PONumber;
                                query.InvoiceNumber = model.InvoiceNumber;
                                query.InvoiceType = model.InvoiceType;
                                query.InvoiceDate = model.InvoiceDate;
                                query.ProjectId = model.ProjectID;
                                query.PIId = model.PIId;
                                query.TaxCode = model.SACNumber;
                                query.AgencyId = model.SponsoringAgency;
                                query.ServiceType = model.ServiceType;
                                query.DescriptionofServices = model.DescriptionofServices;
                                query.CommunicationAddress = model.CommunicationAddress;
                                query.TaxableValue = model.TaxableValue;
                                query.CurrencyCode = model.CurrencyCode;
                                query.InstalmentNumber = model.Instalmentnumber;
                                query.InstalmentYear = model.Instlmntyr;
                                query.TotalInvoiceValue = model.TotalInvoiceValue;
                                query.TotalInvoiceValueinWords = model.TotalInvoiceValueinwords;
                                query.Bank = model.Bank;
                                query.BankAccountNumber = model.BankAccountNumber;
                                query.Status = "Approval Pending";
                                context.SaveChanges();
                                if (taxquery != null)
                                {
                                    context.Entry(taxquery).State = System.Data.Entity.EntityState.Deleted;
                                    context.SaveChanges();
                                }
                                InvoiceId = query.InvoiceId;
                                tblInvoiceTaxDetails tax = new tblInvoiceTaxDetails();
                                if (model.IGSTPercentage == "NA")
                                {
                                    tax.InvoiceId = InvoiceId;
                                    // Regex regexObj = new Regex(@"[^\d]");
                                    //var cgstpercent = regexObj.Replace(model.CGSTPercentage, ""); 
                                    var cgstpercent = model.CGSTPercentage.Substring(0, (model.CGSTPercentage.Length - 1));
                                    tax.CGSTRate = Convert.ToDecimal(cgstpercent);
                                    tax.CGSTAmount = model.CGST;
                                    var sgstpercent = model.SGSTPercentage.Substring(0, (model.SGSTPercentage.Length - 1));
                                    tax.SGSTRate = Convert.ToDecimal(sgstpercent);
                                    tax.SGSTAmount = model.SGST;
                                    tax.IGSTRate = 0;
                                    tax.IGSTAmount = 0;
                                    tax.CrtdTS = DateTime.Now;
                                    tax.CrtdUserId = LoggedInUser;
                                    tax.TotalTaxValue = model.TotalTaxValue;
                                }
                                if (model.IGSTPercentage != "NA")
                                {
                                    //Regex regexObj = new Regex(@"[^\d]");
                                    tax.InvoiceId = InvoiceId;
                                    tax.CGSTRate = 0;
                                    tax.CGSTAmount = 0;
                                    tax.SGSTRate = 0;
                                    tax.SGSTAmount = 0;
                                    var igstrate = model.IGSTPercentage.Substring(0, (model.IGSTPercentage.Length - 1));
                                    tax.IGSTRate = Convert.ToDecimal(igstrate);
                                    tax.IGSTAmount = model.IGST;
                                    tax.CrtdTS = DateTime.Now;
                                    tax.CrtdUserId = LoggedInUser;
                                    tax.TotalTaxValue = model.TotalTaxValue;
                                }
                                context.tblInvoiceTaxDetails.Add(tax);
                                context.SaveChanges();
                                transaction.Commit();
                                return -2;
                            }
                            else
                            {
                                return -3;
                            }
                        }
                        // Edit Invoice function ends

                        // Creating new invoice function starts 
                        else
                        {
                            var ProjectId = model.ProjectID;
                            var Pjctquery = (from P in context.tblProject
                                             join user in context.vwFacultyStaffDetails on P.PIName equals user.UserId into g
                                             join agency in context.tblAgencyMaster on P.SponsoringAgency equals agency.AgencyId into i
                                             from user in g.DefaultIfEmpty()
                                             from agency in i.DefaultIfEmpty()
                                             where P.ProjectId == ProjectId
                                             select new { P, user, agency }).FirstOrDefault();

                            var invoicequery = (from I in context.tblProjectInvoice
                                                where I.ProjectId == ProjectId
                                                select I).ToList();
                            Nullable<Decimal> totalprevinvoicevalue = 0;
                            var AvailableBalance = Pjctquery.P.SanctionValue;
                            Nullable<Decimal> TaxableValue = 0;
                            if (invoicequery.Count() > 0)
                            {
                                Nullable<int>[] _invoiceid = new Nullable<int>[invoicequery.Count];
                                string[] _invoicenumber = new string[invoicequery.Count];
                                Nullable<Decimal>[] _invoicevalue = new Nullable<Decimal>[invoicequery.Count];
                                string[] _invoicedate = new string[invoicequery.Count];

                                for (int i = 0; i < invoicequery.Count(); i++)
                                {
                                    _invoiceid[i] = invoicequery[i].InvoiceId;
                                    _invoicenumber[i] = invoicequery[i].InvoiceNumber;
                                    _invoicevalue[i] = Convert.ToDecimal(invoicequery[i].TotalInvoiceValue);
                                    _invoicedate[i] = String.Format("{0:ddd dd-MMM-yyyy}", invoicequery[i].InvoiceDate);
                                    totalprevinvoicevalue += _invoicevalue[i];
                                }

                                AvailableBalance = model.Sanctionvalue - totalprevinvoicevalue;
                            }

                            var instalmentquery = (from I in context.tblInstallment
                                                   where I.ProjectId == ProjectId
                                                   select I).ToList();

                            if (Pjctquery.P.IsYearWiseAllocation == true)
                            {
                                DateTime startdate = DateTime.Now;
                                DateTime enddate = DateTime.Now;
                                DateTime today = DateTime.Now;

                                startdate = (DateTime)Pjctquery.P.TentativeStartDate;
                                enddate = (DateTime)Pjctquery.P.TentativeCloseDate;
                                TimeSpan diff_date = today - startdate;
                                int noofdays = diff_date.Days;
                                int years = noofdays / 365;
                                int currentprojectyear = 0;
                                if (years == 0)
                                {
                                    currentprojectyear = 1;
                                }
                                if (years > 0)
                                {
                                    currentprojectyear = years;
                                }
                                if (instalmentquery.Count() > 0)
                                {
                                    var previousinstalmentinvoice = (from ins in context.tblProjectInvoice
                                                                     where ins.ProjectId == ProjectId && ins.InstalmentYear == currentprojectyear
                                                                     orderby ins.InvoiceId descending
                                                                     select ins).FirstOrDefault();
                                    int lastinvoicedinstalment = 0;
                                    if (previousinstalmentinvoice != null)
                                    {
                                        lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                    }
                                    int currentinstalment = lastinvoicedinstalment + 1;
                                    var instalment = (from ins in context.tblInstallment
                                                      where (ins.ProjectId == ProjectId && ins.Year == currentprojectyear && ins.InstallmentNo == currentinstalment)
                                                      select ins).FirstOrDefault();
                                    if (instalment != null)
                                    {
                                        TaxableValue = instalment.InstallmentValueForYear;
                                    }
                                    else if (instalment == null)
                                    {
                                        TaxableValue = 0;
                                    }
                                    if (model.TaxableValue > TaxableValue)
                                    {
                                        return -4;
                                    }

                                }
                                else if (instalmentquery.Count() == 0)
                                {
                                    TaxableValue = AvailableBalance;
                                    if (model.TaxableValue > TaxableValue)
                                    {
                                        return -4;
                                    }
                                }
                            }

                            if (Pjctquery.P.IsYearWiseAllocation != true)
                            {
                                if (instalmentquery.Count() > 0)
                                {
                                    var previousinstalmentinvoice = (from ins in context.tblProjectInvoice
                                                                     where ins.ProjectId == ProjectId
                                                                     orderby ins.InvoiceId descending
                                                                     select ins).FirstOrDefault();
                                    int lastinvoicedinstalment = 0;
                                    if (previousinstalmentinvoice != null)
                                    {
                                        lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                    }
                                    int currentinstalment = lastinvoicedinstalment + 1;
                                    var instalment = (from ins in context.tblInstallment
                                                      where (ins.ProjectId == ProjectId && ins.InstallmentNo == currentinstalment)
                                                      select ins).FirstOrDefault();
                                    if (instalment != null)
                                    {
                                        TaxableValue = instalment.InstallmentValue;
                                    }
                                    else if (instalment == null)
                                    {
                                        TaxableValue = AvailableBalance;
                                    }
                                    if (model.TaxableValue > TaxableValue)
                                    {
                                        return -4;
                                    }
                                }
                                else if (instalmentquery.Count() == 0)
                                {
                                    TaxableValue = AvailableBalance;
                                    if (model.TaxableValue > TaxableValue)
                                    {
                                        return -4;
                                    }
                                }
                            }
                            tblProjectInvoice Invoice = new tblProjectInvoice();
                            Invoice.CrtdUserId = LoggedInUser;
                            Invoice.CrtdTS = DateTime.Now;
                            Invoice.InvoiceNumber = model.InvoiceNumber;
                            Invoice.InvoiceType = model.InvoiceType;
                            Invoice.InvoiceDate = model.InvoiceDate;
                            Invoice.ProjectId = model.ProjectID;
                            Invoice.PIId = model.PIId;
                            Invoice.PONumber = model.PONumber;
                            Invoice.TaxCode = model.SACNumber;
                            Invoice.AgencyId = model.SponsoringAgency;
                            Invoice.ServiceType = model.ServiceType;
                            Invoice.DescriptionofServices = model.DescriptionofServices;
                            Invoice.TaxableValue = model.TaxableValue;
                            Invoice.CurrencyCode = model.CurrencyCode;
                            Invoice.InstalmentNumber = model.Instalmentnumber;
                            Invoice.InstalmentYear = model.Instlmntyr;
                            Invoice.TotalInvoiceValue = model.TotalInvoiceValue;
                            Invoice.TotalInvoiceValueinWords = model.TotalInvoiceValueinwords;
                            Invoice.Bank = model.Bank;
                            Invoice.BankAccountNumber = model.BankAccountNumber;
                            Invoice.Status = "Approval Pending";
                            Invoice.CommunicationAddress = model.CommunicationAddress;
                            var PI = model.PIId;
                            var facultycode = Common.getfacultycode(Convert.ToInt32(PI));
                            int CurrentYear = (DateTime.Today.Year) % 100;
                            int PreviousYear = (DateTime.Today.Year - 1) % 100;
                            int NextYear = (DateTime.Today.Year + 1) % 100;
                            string PreYear = PreviousYear.ToString();
                            string NexYear = NextYear.ToString();
                            string CurYear = CurrentYear.ToString();
                            string FinYear = null;

                            if (DateTime.Today.Month > 3)
                                FinYear = CurYear + NexYear;
                            else
                                FinYear = PreYear + CurYear;
                            var Sequencenumber = Common.getInvoiceId();
                            model.InvoiceNumber = Sequencenumber;                           
                            Invoice.InvoiceNumber = model.InvoiceNumber;
                            context.tblProjectInvoice.Add(Invoice);
                            context.SaveChanges();

                            InvoiceId = Invoice.InvoiceId;
                            tblInvoiceTaxDetails tax = new tblInvoiceTaxDetails();
                            if (model.IGSTPercentage == "NA")
                            {
                                tax.InvoiceId = InvoiceId;
                                //  Regex regexObj = new Regex(@"[^\d]");
                                var cgstpercent = model.CGSTPercentage.Substring(0, (model.CGSTPercentage.Length - 1));
                                tax.CGSTRate = Convert.ToDecimal(cgstpercent);
                                tax.CGSTAmount = model.CGST;
                                var sgstpercent = model.SGSTPercentage.Substring(0, (model.SGSTPercentage.Length - 1));
                                tax.SGSTRate = Convert.ToDecimal(sgstpercent);
                                tax.SGSTAmount = model.SGST;
                                tax.IGSTRate = 0;
                                tax.IGSTAmount = 0;
                                tax.CrtdTS = DateTime.Now;
                                tax.CrtdUserId = LoggedInUser;
                                tax.TotalTaxValue = model.TotalTaxValue;
                            }
                            if (model.IGSTPercentage != "NA")
                            {
                                // Regex regexObj = new Regex(@"[^\d]");
                                tax.InvoiceId = InvoiceId;
                                tax.CGSTRate = 0;
                                tax.CGSTAmount = 0;
                                tax.SGSTRate = 0;
                                tax.SGSTAmount = 0;
                                var igstrate = model.IGSTPercentage.Substring(0, (model.IGSTPercentage.Length - 1));
                                tax.IGSTRate = Convert.ToDecimal(igstrate);
                                tax.IGSTAmount = model.IGST;
                                tax.CrtdTS = DateTime.Now;
                                tax.CrtdUserId = LoggedInUser;
                                tax.TotalTaxValue = model.TotalTaxValue;
                            }
                            context.tblInvoiceTaxDetails.Add(tax);
                            context.SaveChanges();


                            if (model.InvoiceDraftId != null && model.InvoiceDraftId != 0)
                            {
                                tblProjectInvoiceDraft InvoiceDraft = new tblProjectInvoiceDraft();
                                InvoiceDraft.Status = "Invoiced";
                            }
                            
                        }
                        transaction.Commit();
                        return InvoiceId;
                    }

                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return -1;
                    }
               }

            }
        }

        public int DraftInvoice(CreateInvoiceModel model, int LoggedInUser)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                   
                        tblProjectInvoiceDraft Draft = new tblProjectInvoiceDraft();
                        Draft.CrtdUserId = LoggedInUser;
                        Draft.CrtdTS = DateTime.Now;                       
                        Draft.InvoiceType = model.InvoiceType;
                        Draft.InvoiceDate = model.InvoiceDate;
                        Draft.ProjectId = model.ProjectID;
                        Draft.PIId = model.PIId;
                        Draft.TaxCode = model.SACNumber;
                        Draft.AgencyId = model.SponsoringAgency;
                        Draft.ServiceType = model.ServiceType;
                        Draft.PONumber = model.PONumber;
                        Draft.CommunicationAddress = model.CommunicationAddress;
                        Draft.DescriptionofServices = model.DescriptionofServices;
                        Draft.TaxableValue = model.TaxableValue;
                        Draft.InstalmentNumber = model.Instalmentnumber;
                        Draft.InstalmentYear = model.Instlmntyr;
                        Draft.CurrencyCode = model.CurrencyCode;
                        Draft.TotalInvoiceValue = model.TotalTaxValue + model.TaxableValue;
                        Draft.TotalInvoiceValueinWords = model.TotalInvoiceValueinwords;
                        Draft.Bank = model.Bank;
                        Draft.BankAccountNumber = model.BankAccountNumber;
                        Draft.Status = "Draft";
                        context.tblProjectInvoiceDraft.Add(Draft);
                        context.SaveChanges();
                        int InvoiceDraftId = Draft.InvoiceDraftId;
                        return InvoiceDraftId;
                    }                
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static List<MasterlistviewModel> LoadCancelableInvList(int PIId)
        {
            try

            {

                List<MasterlistviewModel> Title = new List<MasterlistviewModel>();

                Title.Add(new MasterlistviewModel()
                {
                    id = null,
                    name = "Select Any"
                });
                using (var context = new IOASDBEntities())
                {

                    var query = (from I in context.tblProjectInvoice
                                 join U in context.vwFacultyStaffDetails on I.PIId equals U.UserId                                 
                                 where (I.PIId == PIId && I.Status == "Active" && I.IsCancelled == false)
                                 orderby I.InvoiceId
                                 select new { I, U }).ToList();
                    if(query.Count() > 0)
                    {
                        for(int i = 0; i < query.Count(); i++ )
                        {
                            var invid = query[i].I.InvoiceId;
                            var rcvquery = (from I in context.tblReceipt
                                            where (I.InvoiceId == invid && I.IsCanceledReceipt == true)
                                            orderby I.InvoiceId
                                            select I).ToList();
                            if (rcvquery == null)
                            {                                
                                    Title.Add(new MasterlistviewModel()
                                    {
                                        id = query[i].I.InvoiceId,
                                        name = query[i].I.InvoiceNumber + "-" + query[i].U.FirstName,
                                    });                                
                            }
                        }                        
                        
                    }
                    
                    
                }

                return Title;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public PagedData<InvoiceSearchResultModel> GetProformaInvoiceList(InvoiceSearchFieldModel model, int page, int pageSize)
        {
            try
            {
                List<InvoiceSearchResultModel> list = new List<InvoiceSearchResultModel>();
                var searchData = new PagedData<InvoiceSearchResultModel>();
                int skiprec = 0;
                if (page == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (page - 1) * pageSize;
                }
                using (var context = new IOASDBEntities())
                {
                    var predicate = PredicateBuilder.BaseAnd<tblProformaInvoice>();
                    if (!string.IsNullOrEmpty(model.InvoiceNumber))
                        predicate = predicate.And(d => d.InvoiceNumber.Contains(model.InvoiceNumber));
                    if (model.InvoiceType != null)
                        predicate = predicate.And(d => d.InvoiceType == model.InvoiceType);
                    if (model.PIName != null)
                        predicate = predicate.And(d => d.PIId == model.PIName);
                    if (model.FromDate != null && model.ToDate != null)
                        predicate = predicate.And(d => d.InvoiceDate >= model.FromDate && d.InvoiceDate <= model.ToDate);
                    //if (model.FromSRBDate != null && model.ToSRBDate != null)
                    //    predicate = predicate.And(d => d.InwardDate >= model.FromSRBDate && d.InwardDate <= model.ToSRBDate);
                    var query = context.tblProformaInvoice.Where(predicate).OrderByDescending(m => m.InvoiceId).Skip(skiprec).Take(pageSize).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            var projectid = query[i].ProjectId;
                            var projectquery = context.tblProject.FirstOrDefault(dup => dup.ProjectId == projectid);
                            //var doc = query[i].PODocs.Split(new char[] { '_' }, 2);
                            list.Add(new InvoiceSearchResultModel()
                            {
                                ProjectNumber = projectquery.ProjectNumber,
                                ProjectId = projectid,
                                ProjectType = projectquery.ProjectType,
                                InvoiceDate = String.Format("{0:ddd dd-MMM-yyyy}", query[i].InvoiceDate),
                                InvoiceType = query[i].InvoiceType,
                                SACNumber = query[i].TaxCode,
                                Service = query[i].DescriptionofServices,
                                InvoiceNumber = query[i].InvoiceNumber,
                                PIId = projectquery.PIName,
                                InvoiceId = query[i].InvoiceId,
                                TotalInvoiceValue = query[i].TotalInvoiceValue,
                                ProjectTitle = projectquery.ProjectTitle,
                            });

                        }
                    }
                    var records = context.tblProformaInvoice.Where(predicate).OrderByDescending(m => m.InvoiceId).Count();
                    searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)records / pageSize));
                    searchData.Data = list;
                    searchData.pageSize = pageSize;
                    searchData.visiblePages = 10;
                    searchData.CurrentPage = page;
                }

                return searchData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int CreateProformaInvoice(CreateInvoiceModel model, int LoggedInUser)
        {

            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        int InvoiceId = 0;
                        // Edit Invoice function start
                        if (model.InvoiceId > 0)
                        {
                            var query = context.tblProformaInvoice.FirstOrDefault(m => m.InvoiceId == model.InvoiceId);
                            var taxquery = context.tblProformaInvoiceTaxDetails.FirstOrDefault(m => m.InvoiceId == model.InvoiceId);

                            // Validation for EditInvoice details
                            if (query != null)
                            {
                                var ProjectId = model.ProjectID;
                                var Pjctquery = (from P in context.tblProject
                                                 join user in context.vwFacultyStaffDetails on P.PIName equals user.UserId into g
                                                 join agency in context.tblAgencyMaster on P.SponsoringAgency equals agency.AgencyId into i
                                                 from user in g.DefaultIfEmpty()
                                                 from agency in i.DefaultIfEmpty()
                                                 where P.ProjectId == ProjectId
                                                 select new { P, user, agency }).FirstOrDefault();

                                var invoicequery = (from I in context.tblProformaInvoice
                                                    where I.ProjectId == ProjectId && I.InvoiceId != model.InvoiceId
                                                    select I).ToList();
                                Nullable<Decimal> totalprevinvoicevalue = 0;
                                var AvailableBalance = Pjctquery.P.SanctionValue;
                                Nullable<Decimal> TaxableValue = 0;
                                if (invoicequery.Count() > 0)
                                {
                                    Nullable<int>[] _invoiceid = new Nullable<int>[invoicequery.Count];
                                    string[] _invoicenumber = new string[invoicequery.Count];
                                    Nullable<Decimal>[] _invoicevalue = new Nullable<Decimal>[invoicequery.Count];
                                    string[] _invoicedate = new string[invoicequery.Count];

                                    for (int i = 0; i < invoicequery.Count(); i++)
                                    {
                                        _invoiceid[i] = invoicequery[i].InvoiceId;
                                        _invoicenumber[i] = invoicequery[i].InvoiceNumber;
                                        _invoicevalue[i] = Convert.ToDecimal(invoicequery[i].TotalInvoiceValue);
                                        _invoicedate[i] = String.Format("{0:ddd dd-MMM-yyyy}", invoicequery[i].InvoiceDate);
                                        totalprevinvoicevalue += _invoicevalue[i];
                                    }

                                    AvailableBalance = model.Sanctionvalue - totalprevinvoicevalue;
                                }

                                var instalmentquery = (from I in context.tblInstallment
                                                       where I.ProjectId == ProjectId
                                                       select I).ToList();

                                if (Pjctquery.P.IsYearWiseAllocation == true)
                                {
                                    DateTime startdate = DateTime.Now;
                                    DateTime enddate = DateTime.Now;
                                    DateTime today = DateTime.Now;

                                    startdate = (DateTime)Pjctquery.P.TentativeStartDate;
                                    enddate = (DateTime)Pjctquery.P.TentativeCloseDate;
                                    TimeSpan diff_date = today - startdate;
                                    int noofdays = diff_date.Days;
                                    int years = noofdays / 365;
                                    int currentprojectyear = 0;
                                    if (years == 0)
                                    {
                                        currentprojectyear = 1;
                                    }
                                    if (years > 0)
                                    {
                                        currentprojectyear = years;
                                    }
                                    if (instalmentquery.Count() > 0)
                                    {
                                        var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
                                                                         where ins.ProjectId == ProjectId && ins.InvoiceId == model.InvoiceId
                                                                         // orderby ins.InvoiceId descending
                                                                         select ins).SingleOrDefault();
                                        int lastinvoicedinstalment = 1;
                                        if (previousinstalmentinvoice != null)
                                        {
                                            lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                        }
                                        //  int currentinstalment = lastinvoicedinstalment + 1;
                                        var instalment = (from ins in context.tblInstallment
                                                          where (ins.ProjectId == ProjectId && ins.Year == currentprojectyear && ins.InstallmentNo == lastinvoicedinstalment)
                                                          select ins).FirstOrDefault();
                                        if (instalment != null)
                                        {
                                            TaxableValue = instalment.InstallmentValue;
                                        }
                                        else if (instalment == null)
                                        {
                                            TaxableValue = AvailableBalance;
                                        }
                                        if (model.TaxableValue > TaxableValue)
                                        {
                                            return -4;
                                        }

                                    }
                                    else if (instalmentquery.Count() == 0)
                                    {
                                        TaxableValue = AvailableBalance;
                                        if (model.TaxableValue > TaxableValue)
                                        {
                                            return -4;
                                        }
                                    }
                                }

                                if (Pjctquery.P.IsYearWiseAllocation != true)
                                {
                                    if (instalmentquery.Count() > 0)
                                    {
                                        var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
                                                                         where ins.ProjectId == ProjectId && ins.InvoiceId == model.InvoiceId
                                                                         //orderby ins.InvoiceId descending
                                                                         select ins).SingleOrDefault();
                                        int lastinvoicedinstalment = 1;
                                        if (previousinstalmentinvoice != null)
                                        {
                                            lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                        }
                                        // int currentinstalment = lastinvoicedinstalment + 1;
                                        var instalment = (from ins in context.tblInstallment
                                                          where (ins.ProjectId == ProjectId && ins.InstallmentNo == lastinvoicedinstalment)
                                                          select ins).FirstOrDefault();
                                        if (instalment != null)
                                        {
                                            TaxableValue = instalment.InstallmentValue;
                                        }
                                        else if (instalment == null)
                                        {
                                            TaxableValue = AvailableBalance;
                                        }
                                        if (model.TaxableValue > TaxableValue)
                                        {
                                            return -4;
                                        }
                                    }
                                    else if (instalmentquery.Count() == 0)
                                    {
                                        TaxableValue = AvailableBalance;
                                        if (model.TaxableValue > TaxableValue)
                                        {
                                            return -4;
                                        }
                                    }
                                }
                                // Validation ends and save edit in table                
                                query.UpdtUserId = LoggedInUser;
                                query.UpdtTS = DateTime.Now;
                                query.PONumber = model.PONumber;
                                query.InvoiceNumber = model.InvoiceNumber;
                                query.InvoiceType = model.InvoiceType;
                                query.InvoiceDate = model.InvoiceDate;
                                query.ProjectId = model.ProjectID;
                                query.PIId = model.PIId;
                                query.TaxCode = model.SACNumber;
                                query.AgencyId = model.SponsoringAgency;
                                query.ServiceType = model.ServiceType;
                                query.DescriptionofServices = model.DescriptionofServices;
                                query.CommunicationAddress = model.CommunicationAddress;
                                query.TaxableValue = model.TaxableValue;
                                query.CurrencyCode = model.CurrencyCode;
                                query.InstalmentNumber = model.Instalmentnumber;
                                query.InstalmentYear = model.Instlmntyr;
                                query.TotalInvoiceValue = model.TotalInvoiceValue;
                                query.TotalInvoiceValueinWords = model.TotalInvoiceValueinwords;
                                query.Bank = model.Bank;
                                query.BankAccountNumber = model.BankAccountNumber;
                                query.Status = "Approval Pending";
                                context.SaveChanges();
                                if (taxquery != null)
                                {
                                    context.Entry(taxquery).State = System.Data.Entity.EntityState.Deleted;
                                    context.SaveChanges();
                                }
                                InvoiceId = query.InvoiceId;
                                tblProformaInvoiceTaxDetails tax = new tblProformaInvoiceTaxDetails();
                                if (model.IGSTPercentage == "NA")
                                {
                                    tax.InvoiceId = InvoiceId;
                                    // Regex regexObj = new Regex(@"[^\d]");
                                    //var cgstpercent = regexObj.Replace(model.CGSTPercentage, ""); 
                                    var cgstpercent = model.CGSTPercentage.Substring(0, (model.CGSTPercentage.Length - 1));
                                    tax.CGSTRate = Convert.ToDecimal(cgstpercent);
                                    tax.CGSTAmount = model.CGST;
                                    var sgstpercent = model.SGSTPercentage.Substring(0, (model.SGSTPercentage.Length - 1));
                                    tax.SGSTRate = Convert.ToDecimal(sgstpercent);
                                    tax.SGSTAmount = model.SGST;
                                    tax.IGSTRate = 0;
                                    tax.IGSTAmount = 0;
                                    tax.CrtdTS = DateTime.Now;
                                    tax.CrtdUserId = LoggedInUser;
                                    tax.TotalTaxValue = model.TotalTaxValue;
                                }
                                if (model.IGSTPercentage != "NA")
                                {
                                    //Regex regexObj = new Regex(@"[^\d]");
                                    tax.InvoiceId = InvoiceId;
                                    tax.CGSTRate = 0;
                                    tax.CGSTAmount = 0;
                                    tax.SGSTRate = 0;
                                    tax.SGSTAmount = 0;
                                    var igstrate = model.IGSTPercentage.Substring(0, (model.IGSTPercentage.Length - 1));
                                    tax.IGSTRate = Convert.ToDecimal(igstrate);
                                    tax.IGSTAmount = model.IGST;
                                    tax.CrtdTS = DateTime.Now;
                                    tax.CrtdUserId = LoggedInUser;
                                    tax.TotalTaxValue = model.TotalTaxValue;
                                }
                                context.tblProformaInvoiceTaxDetails.Add(tax);
                                context.SaveChanges();
                                transaction.Commit();
                                return -2;
                            }
                            else
                            {
                                return -3;
                            }
                        }
                        // Edit Invoice function ends

                        // Creating new invoice function starts 
                        else
                        {
                            var ProjectId = model.ProjectID;
                            var Pjctquery = (from P in context.tblProject
                                             join user in context.vwFacultyStaffDetails on P.PIName equals user.UserId into g
                                             join agency in context.tblAgencyMaster on P.SponsoringAgency equals agency.AgencyId into i
                                             from user in g.DefaultIfEmpty()
                                             from agency in i.DefaultIfEmpty()
                                             where P.ProjectId == ProjectId
                                             select new { P, user, agency }).FirstOrDefault();

                            var invoicequery = (from I in context.tblProformaInvoice
                                                where I.ProjectId == ProjectId
                                                select I).ToList();
                            Nullable<Decimal> totalprevinvoicevalue = 0;
                            var AvailableBalance = Pjctquery.P.SanctionValue;
                            Nullable<Decimal> TaxableValue = 0;
                            if (invoicequery.Count() > 0)
                            {
                                Nullable<int>[] _invoiceid = new Nullable<int>[invoicequery.Count];
                                string[] _invoicenumber = new string[invoicequery.Count];
                                Nullable<Decimal>[] _invoicevalue = new Nullable<Decimal>[invoicequery.Count];
                                string[] _invoicedate = new string[invoicequery.Count];

                                for (int i = 0; i < invoicequery.Count(); i++)
                                {
                                    _invoiceid[i] = invoicequery[i].InvoiceId;
                                    _invoicenumber[i] = invoicequery[i].InvoiceNumber;
                                    _invoicevalue[i] = Convert.ToDecimal(invoicequery[i].TotalInvoiceValue);
                                    _invoicedate[i] = String.Format("{0:ddd dd-MMM-yyyy}", invoicequery[i].InvoiceDate);
                                    totalprevinvoicevalue += _invoicevalue[i];
                                }

                                AvailableBalance = model.Sanctionvalue - totalprevinvoicevalue;
                            }

                            var instalmentquery = (from I in context.tblInstallment
                                                   where I.ProjectId == ProjectId
                                                   select I).ToList();

                            if (Pjctquery.P.IsYearWiseAllocation == true)
                            {
                                DateTime startdate = DateTime.Now;
                                DateTime enddate = DateTime.Now;
                                DateTime today = DateTime.Now;

                                startdate = (DateTime)Pjctquery.P.TentativeStartDate;
                                enddate = (DateTime)Pjctquery.P.TentativeCloseDate;
                                TimeSpan diff_date = today - startdate;
                                int noofdays = diff_date.Days;
                                int years = noofdays / 365;
                                int currentprojectyear = 0;
                                if (years == 0)
                                {
                                    currentprojectyear = 1;
                                }
                                if (years > 0)
                                {
                                    currentprojectyear = years;
                                }
                                if (instalmentquery.Count() > 0)
                                {
                                    var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
                                                                     where ins.ProjectId == ProjectId && ins.InstalmentYear == currentprojectyear
                                                                     orderby ins.InvoiceId descending
                                                                     select ins).FirstOrDefault();
                                    int lastinvoicedinstalment = 0;
                                    if (previousinstalmentinvoice != null)
                                    {
                                        lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                    }
                                    int currentinstalment = lastinvoicedinstalment + 1;
                                    var instalment = (from ins in context.tblInstallment
                                                      where (ins.ProjectId == ProjectId && ins.Year == currentprojectyear && ins.InstallmentNo == currentinstalment)
                                                      select ins).FirstOrDefault();
                                    if (instalment != null)
                                    {
                                        TaxableValue = instalment.InstallmentValueForYear;
                                    }
                                    else if (instalment == null)
                                    {
                                        TaxableValue = 0;
                                    }
                                    if (model.TaxableValue > TaxableValue)
                                    {
                                        return -4;
                                    }

                                }
                                else if (instalmentquery.Count() == 0)
                                {
                                    TaxableValue = AvailableBalance;
                                    if (model.TaxableValue > TaxableValue)
                                    {
                                        return -4;
                                    }
                                }
                            }

                            if (Pjctquery.P.IsYearWiseAllocation != true)
                            {
                                if (instalmentquery.Count() > 0)
                                {
                                    var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
                                                                     where ins.ProjectId == ProjectId
                                                                     orderby ins.InvoiceId descending
                                                                     select ins).FirstOrDefault();
                                    int lastinvoicedinstalment = 0;
                                    if (previousinstalmentinvoice != null)
                                    {
                                        lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                    }
                                    int currentinstalment = lastinvoicedinstalment + 1;
                                    var instalment = (from ins in context.tblInstallment
                                                      where (ins.ProjectId == ProjectId && ins.InstallmentNo == currentinstalment)
                                                      select ins).FirstOrDefault();
                                    if (instalment != null)
                                    {
                                        TaxableValue = instalment.InstallmentValue;
                                    }
                                    else if (instalment == null)
                                    {
                                        TaxableValue = AvailableBalance;
                                    }
                                    if (model.TaxableValue > TaxableValue)
                                    {
                                        return -4;
                                    }
                                }
                                else if (instalmentquery.Count() == 0)
                                {
                                    TaxableValue = AvailableBalance;
                                    if (model.TaxableValue > TaxableValue)
                                    {
                                        return -4;
                                    }
                                }
                            }
                            tblProformaInvoice Invoice = new tblProformaInvoice();
                            Invoice.CrtdUserId = LoggedInUser;
                            Invoice.CrtdTS = DateTime.Now;
                            Invoice.InvoiceNumber = model.InvoiceNumber;
                            Invoice.InvoiceType = model.InvoiceType;
                            Invoice.InvoiceDate = model.InvoiceDate;
                            Invoice.ProjectId = model.ProjectID;
                            Invoice.PIId = model.PIId;
                            Invoice.PONumber = model.PONumber;
                            Invoice.TaxCode = model.SACNumber;
                            Invoice.AgencyId = model.SponsoringAgency;
                            Invoice.ServiceType = model.ServiceType;
                            Invoice.DescriptionofServices = model.DescriptionofServices;
                            Invoice.TaxableValue = model.TaxableValue;
                            Invoice.CurrencyCode = model.CurrencyCode;
                            Invoice.InstalmentNumber = model.Instalmentnumber;
                            Invoice.InstalmentYear = model.Instlmntyr;
                            Invoice.TotalInvoiceValue = model.TotalInvoiceValue;
                            Invoice.TotalInvoiceValueinWords = model.TotalInvoiceValueinwords;
                            Invoice.Bank = model.Bank;
                            Invoice.BankAccountNumber = model.BankAccountNumber;
                            Invoice.Status = "Approval Pending";
                            Invoice.CommunicationAddress = model.CommunicationAddress;
                            var PI = model.PIId;
                            var facultycode = Common.getfacultycode(Convert.ToInt32(PI));
                            int CurrentYear = (DateTime.Today.Year) % 100;
                            int PreviousYear = (DateTime.Today.Year - 1) % 100;
                            int NextYear = (DateTime.Today.Year + 1) % 100;
                            string PreYear = PreviousYear.ToString();
                            string NexYear = NextYear.ToString();
                            string CurYear = CurrentYear.ToString();
                            string FinYear = null;

                            if (DateTime.Today.Month > 3)
                                FinYear = CurYear + NexYear;
                            else
                                FinYear = PreYear + CurYear;
                            var Sequencenumber = Common.getProformaInvoiceId();
                            model.InvoiceNumber = Sequencenumber;                            
                            Invoice.InvoiceNumber = model.InvoiceNumber;
                            context.tblProformaInvoice.Add(Invoice);
                            context.SaveChanges();

                            InvoiceId = Invoice.InvoiceId;
                            tblProformaInvoiceTaxDetails tax = new tblProformaInvoiceTaxDetails();
                            if (model.IGSTPercentage == "NA")
                            {
                                tax.InvoiceId = InvoiceId;
                                //  Regex regexObj = new Regex(@"[^\d]");
                                var cgstpercent = model.CGSTPercentage.Substring(0, (model.CGSTPercentage.Length - 1));
                                tax.CGSTRate = Convert.ToDecimal(cgstpercent);
                                tax.CGSTAmount = model.CGST;
                                var sgstpercent = model.SGSTPercentage.Substring(0, (model.SGSTPercentage.Length - 1));
                                tax.SGSTRate = Convert.ToDecimal(sgstpercent);
                                tax.SGSTAmount = model.SGST;
                                tax.IGSTRate = 0;
                                tax.IGSTAmount = 0;
                                tax.CrtdTS = DateTime.Now;
                                tax.CrtdUserId = LoggedInUser;
                                tax.TotalTaxValue = model.TotalTaxValue;
                            }
                            if (model.IGSTPercentage != "NA")
                            {
                                // Regex regexObj = new Regex(@"[^\d]");
                                tax.InvoiceId = InvoiceId;
                                tax.CGSTRate = 0;
                                tax.CGSTAmount = 0;
                                tax.SGSTRate = 0;
                                tax.SGSTAmount = 0;
                                var igstrate = model.IGSTPercentage.Substring(0, (model.IGSTPercentage.Length - 1));
                                tax.IGSTRate = Convert.ToDecimal(igstrate);
                                tax.IGSTAmount = model.IGST;
                                tax.CrtdTS = DateTime.Now;
                                tax.CrtdUserId = LoggedInUser;
                                tax.TotalTaxValue = model.TotalTaxValue;
                            }
                            context.tblProformaInvoiceTaxDetails.Add(tax);
                            context.SaveChanges();


                            if (model.InvoiceDraftId != null && model.InvoiceDraftId != 0)
                            {
                                tblProformaInvoiceDraft InvoiceDraft = new tblProformaInvoiceDraft();
                                InvoiceDraft.Status = "Invoiced";
                            }

                        }
                        transaction.Commit();
                        return InvoiceId;
                    }

                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return -1;
                    }
                }

            }
        }

        //public int CreateProformaInvoice(CreateInvoiceModel model, int LoggedInUser)
        //{
        //    try
        //    {
        //        using (var context = new IOASDBEntities())
        //        {
        //            if (model.InvoiceId > 0)
        //            {
        //                var query = context.tblProformaInvoice.FirstOrDefault(m => m.InvoiceId == model.InvoiceId);
        //                var taxquery = context.tblProformaInvoiceTaxDetails.FirstOrDefault(m => m.InvoiceId == model.InvoiceId);

        //                // Validation for EditInvoice details
        //                if (query != null)
        //                {
        //                    var ProjectId = model.ProjectID;
        //                    var Pjctquery = (from P in context.tblProject
        //                                     join user in context.vwFacultyStaffDetails on P.PIName equals user.UserId into g
        //                                     join agency in context.tblAgencyMaster on P.SponsoringAgency equals agency.AgencyId into i
        //                                     from user in g.DefaultIfEmpty()
        //                                     from agency in i.DefaultIfEmpty()
        //                                     where P.ProjectId == ProjectId
        //                                     select new { P, user, agency }).FirstOrDefault();

        //                    var invoicequery = (from I in context.tblProformaInvoice
        //                                        where I.ProjectId == ProjectId && I.InvoiceId != model.InvoiceId
        //                                        select I).ToList();
        //                    Nullable<Decimal> totalprevinvoicevalue = 0;
        //                    var AvailableBalance = Pjctquery.P.SanctionValue;
        //                    Nullable<Decimal> TaxableValue = 0;
        //                    if (invoicequery.Count() > 0)
        //                    {
        //                        Nullable<int>[] _invoiceid = new Nullable<int>[invoicequery.Count];
        //                        string[] _invoicenumber = new string[invoicequery.Count];
        //                        Nullable<Decimal>[] _invoicevalue = new Nullable<Decimal>[invoicequery.Count];
        //                        string[] _invoicedate = new string[invoicequery.Count];

        //                        for (int i = 0; i < invoicequery.Count(); i++)
        //                        {
        //                            _invoiceid[i] = invoicequery[i].InvoiceId;
        //                            _invoicenumber[i] = invoicequery[i].InvoiceNumber;
        //                            _invoicevalue[i] = Convert.ToDecimal(invoicequery[i].TotalInvoiceValue);
        //                            _invoicedate[i] = String.Format("{0:ddd dd-MMM-yyyy}", invoicequery[i].InvoiceDate);
        //                            totalprevinvoicevalue += _invoicevalue[i];
        //                        }

        //                        AvailableBalance = model.Sanctionvalue - totalprevinvoicevalue;
        //                    }

        //                    var instalmentquery = (from I in context.tblInstallment
        //                                           where I.ProjectId == ProjectId
        //                                           select I).ToList();

        //                    if (Pjctquery.P.IsYearWiseAllocation == true)
        //                    {
        //                        DateTime startdate = DateTime.Now;
        //                        DateTime enddate = DateTime.Now;
        //                        DateTime today = DateTime.Now;

        //                        startdate = (DateTime)Pjctquery.P.TentativeStartDate;
        //                        enddate = (DateTime)Pjctquery.P.TentativeCloseDate;
        //                        TimeSpan diff_date = today - startdate;
        //                        int noofdays = diff_date.Days;
        //                        int years = noofdays / 365;
        //                        int currentprojectyear = 0;
        //                        if (years == 0)
        //                        {
        //                            currentprojectyear = 1;
        //                        }
        //                        if (years > 0)
        //                        {
        //                            currentprojectyear = years;
        //                        }
        //                        if (instalmentquery.Count() > 0)
        //                        {
        //                            var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
        //                                                             where ins.ProjectId == ProjectId && ins.InvoiceId == model.InvoiceId
        //                                                             // orderby ins.InvoiceId descending
        //                                                             select ins).SingleOrDefault();
        //                            int lastinvoicedinstalment = 1;
        //                            if (previousinstalmentinvoice != null)
        //                            {
        //                                lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
        //                            }
        //                            //  int currentinstalment = lastinvoicedinstalment + 1;
        //                            var instalment = (from ins in context.tblInstallment
        //                                              where (ins.ProjectId == ProjectId && ins.Year == currentprojectyear && ins.InstallmentNo == lastinvoicedinstalment)
        //                                              select ins).FirstOrDefault();
        //                            if (instalment != null)
        //                            {
        //                                TaxableValue = instalment.InstallmentValue;
        //                            }
        //                            else if (instalment == null)
        //                            {
        //                                TaxableValue = 0;
        //                            }
        //                            if (model.TaxableValue > TaxableValue)
        //                            {
        //                                return -4;
        //                            }

        //                        }
        //                        else if (instalmentquery.Count() == 0)
        //                        {
        //                            TaxableValue = AvailableBalance;
        //                            if (model.TaxableValue > TaxableValue)
        //                            {
        //                                return -4;
        //                            }
        //                        }
        //                    }

        //                    if (Pjctquery.P.IsYearWiseAllocation != true)
        //                    {
        //                        if (instalmentquery.Count() > 0)
        //                        {
        //                            var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
        //                                                             where ins.ProjectId == ProjectId && ins.InvoiceId == model.InvoiceId
        //                                                             //orderby ins.InvoiceId descending
        //                                                             select ins).SingleOrDefault();
        //                            int lastinvoicedinstalment = 1;
        //                            if (previousinstalmentinvoice != null)
        //                            {
        //                                lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
        //                            }
        //                            // int currentinstalment = lastinvoicedinstalment + 1;
        //                            var instalment = (from ins in context.tblInstallment
        //                                              where (ins.ProjectId == ProjectId && ins.InstallmentNo == lastinvoicedinstalment)
        //                                              select ins).FirstOrDefault();
        //                            if (instalment != null)
        //                            {
        //                                TaxableValue = instalment.InstallmentValue;
        //                            }
        //                            else if (instalment == null)
        //                            {
        //                                TaxableValue = 0;
        //                            }
        //                            if (model.TaxableValue > TaxableValue)
        //                            {
        //                                return -4;
        //                            }
        //                        }
        //                        else if (instalmentquery.Count() == 0)
        //                        {
        //                            TaxableValue = AvailableBalance;
        //                            if (model.TaxableValue > TaxableValue)
        //                            {
        //                                return -4;
        //                            }
        //                        }
        //                    }

        //                    query.UpdtUserId = LoggedInUser;
        //                    query.UpdtTS = DateTime.Now;
        //                    query.InvoiceNumber = model.InvoiceNumber;
        //                    query.InvoiceType = model.InvoiceType;
        //                    query.InvoiceDate = model.InvoiceDate;
        //                    query.ProjectId = model.ProjectID;
        //                    query.PIId = model.PIId;
        //                    query.TaxCode = model.SACNumber;
        //                    query.AgencyId = model.SponsoringAgency;
        //                    query.ServiceType = model.ServiceType;
        //                    query.DescriptionofServices = model.DescriptionofServices;
        //                    query.TaxableValue = model.TaxableValue;
        //                    query.CurrencyCode = model.CurrencyCode;
        //                    query.InstalmentNumber = model.Instalmentnumber;
        //                    query.InstalmentYear = model.Instlmntyr;
        //                    query.TotalInvoiceValue = model.TotalInvoiceValue;
        //                    query.TotalInvoiceValueinWords = model.TotalInvoiceValueinwords;
        //                    query.Bank = model.Bank;
        //                    query.BankAccountNumber = model.BankAccountNumber;
        //                    query.Status = "Approval Pending";
        //                    context.SaveChanges();
        //                    if (taxquery != null)
        //                    {
        //                        context.Entry(taxquery).State = System.Data.Entity.EntityState.Deleted;
        //                        context.SaveChanges();
        //                    }
        //                    int? InvoiceId = model.InvoiceId;
        //                    tblProformaInvoiceTaxDetails tax = new tblProformaInvoiceTaxDetails();
        //                    if (model.IGSTPercentage == "NA")
        //                    {
        //                        tax.InvoiceId = InvoiceId;
        //                      //  Regex regexObj = new Regex(@"[^\d]");
        //                        var cgstpercent = model.CGSTPercentage.Substring(0, (model.CGSTPercentage.Length - 1));
        //                        tax.CGSTRate = Convert.ToDecimal(cgstpercent);
        //                        tax.CGSTAmount = model.CGST;
        //                        var sgstpercent = model.SGSTPercentage.Substring(0, (model.SGSTPercentage.Length - 1));
        //                        tax.SGSTRate = Convert.ToDecimal(sgstpercent);
        //                        tax.SGSTAmount = model.SGST;
        //                        tax.IGSTRate = 0;
        //                        tax.IGSTAmount = 0;
        //                        tax.CrtdTS = DateTime.Now;
        //                        tax.CrtdUserId = LoggedInUser;
        //                        tax.TotalTaxValue = model.TotalTaxValue;
        //                    }
        //                    if (model.IGSTPercentage != "NA")
        //                    {
        //                       // Regex regexObj = new Regex(@"[^\d]");
        //                        tax.InvoiceId = InvoiceId;
        //                        tax.CGSTRate = 0;
        //                        tax.CGSTAmount = 0;
        //                        tax.SGSTRate = 0;
        //                        tax.SGSTAmount = 0;
        //                        var igstrate = model.IGSTPercentage.Substring(0, (model.IGSTPercentage.Length - 1));
        //                        tax.IGSTRate = Convert.ToDecimal(igstrate);
        //                        tax.IGSTAmount = model.IGST;
        //                        tax.CrtdTS = DateTime.Now;
        //                        tax.CrtdUserId = LoggedInUser;
        //                        tax.TotalTaxValue = model.TotalTaxValue;
        //                    }
        //                    context.tblProformaInvoiceTaxDetails.Add(tax);
        //                    context.SaveChanges();

        //                    return -2;
        //                }
        //                else
        //                {
        //                    return -3;
        //                }
        //            }
        //            else
        //            {
        //                var ProjectId = model.ProjectID;
        //                var Pjctquery = (from P in context.tblProject
        //                                 join user in context.vwFacultyStaffDetails on P.PIName equals user.UserId into g
        //                                 join agency in context.tblAgencyMaster on P.SponsoringAgency equals agency.AgencyId into i
        //                                 from user in g.DefaultIfEmpty()
        //                                 from agency in i.DefaultIfEmpty()
        //                                 where P.ProjectId == ProjectId
        //                                 select new { P, user, agency }).FirstOrDefault();

        //                var invoicequery = (from I in context.tblProformaInvoice
        //                                    where I.ProjectId == ProjectId
        //                                    select I).ToList();
        //                Nullable<Decimal> totalprevinvoicevalue = 0;
        //                var AvailableBalance = Pjctquery.P.SanctionValue;
        //                Nullable<Decimal> TaxableValue = 0;
        //                if (invoicequery.Count() > 0)
        //                {
        //                    Nullable<int>[] _invoiceid = new Nullable<int>[invoicequery.Count];
        //                    string[] _invoicenumber = new string[invoicequery.Count];
        //                    Nullable<Decimal>[] _invoicevalue = new Nullable<Decimal>[invoicequery.Count];
        //                    string[] _invoicedate = new string[invoicequery.Count];

        //                    for (int i = 0; i < invoicequery.Count(); i++)
        //                    {
        //                        _invoiceid[i] = invoicequery[i].InvoiceId;
        //                        _invoicenumber[i] = invoicequery[i].InvoiceNumber;
        //                        _invoicevalue[i] = Convert.ToDecimal(invoicequery[i].TotalInvoiceValue);
        //                        _invoicedate[i] = String.Format("{0:ddd dd-MMM-yyyy}", invoicequery[i].InvoiceDate);
        //                        totalprevinvoicevalue += _invoicevalue[i];
        //                    }

        //                    AvailableBalance = model.Sanctionvalue - totalprevinvoicevalue;
        //                }

        //                var instalmentquery = (from I in context.tblInstallment
        //                                       where I.ProjectId == ProjectId
        //                                       select I).ToList();

        //                if (Pjctquery.P.IsYearWiseAllocation == true)
        //                {
        //                    DateTime startdate = DateTime.Now;
        //                    DateTime enddate = DateTime.Now;
        //                    DateTime today = DateTime.Now;

        //                    startdate = (DateTime)Pjctquery.P.TentativeStartDate;
        //                    enddate = (DateTime)Pjctquery.P.TentativeCloseDate;
        //                    TimeSpan diff_date = today - startdate;
        //                    int noofdays = diff_date.Days;
        //                    int years = noofdays / 365;
        //                    int currentprojectyear = 0;
        //                    if (years == 0)
        //                    {
        //                        currentprojectyear = 1;
        //                    }
        //                    if (years > 0)
        //                    {
        //                        currentprojectyear = years;
        //                    }
        //                    if (instalmentquery.Count() > 0)
        //                    {
        //                        var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
        //                                                         where ins.ProjectId == ProjectId && ins.InstalmentYear == currentprojectyear
        //                                                         orderby ins.InvoiceId descending
        //                                                         select ins).FirstOrDefault();
        //                        int lastinvoicedinstalment = 0;
        //                        if (previousinstalmentinvoice != null)
        //                        {
        //                            lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
        //                        }
        //                        int currentinstalment = lastinvoicedinstalment + 1;
        //                        var instalment = (from ins in context.tblInstallment
        //                                          where (ins.ProjectId == ProjectId && ins.Year == currentprojectyear && ins.InstallmentNo == currentinstalment)
        //                                          select ins).FirstOrDefault();
        //                        if (instalment != null)
        //                        {
        //                            TaxableValue = instalment.InstallmentValue;
        //                        }
        //                        else if (instalment == null)
        //                        {
        //                            TaxableValue = 0;
        //                        }
        //                        if (model.TaxableValue > TaxableValue)
        //                        {
        //                            return -4;
        //                        }

        //                    }
        //                    else if (instalmentquery.Count() == 0)
        //                    {
        //                        TaxableValue = AvailableBalance;
        //                        if (model.TaxableValue > TaxableValue)
        //                        {
        //                            return -4;
        //                        }
        //                    }
        //                }

        //                if (Pjctquery.P.IsYearWiseAllocation != true)
        //                {
        //                    if (instalmentquery.Count() > 0)
        //                    {
        //                        var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
        //                                                         where ins.ProjectId == ProjectId
        //                                                         orderby ins.InvoiceId descending
        //                                                         select ins).FirstOrDefault();
        //                        int lastinvoicedinstalment = 0;
        //                        if (previousinstalmentinvoice != null)
        //                        {
        //                            lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
        //                        }
        //                        int currentinstalment = lastinvoicedinstalment + 1;
        //                        var instalment = (from ins in context.tblInstallment
        //                                          where (ins.ProjectId == ProjectId && ins.InstallmentNo == currentinstalment)
        //                                          select ins).FirstOrDefault();
        //                        if (instalment != null)
        //                        {
        //                            TaxableValue = instalment.InstallmentValue;
        //                        }
        //                        else if (instalment == null)
        //                        {
        //                            TaxableValue = 0;
        //                        }
        //                        if (model.TaxableValue > TaxableValue)
        //                        {
        //                            return -4;
        //                        }
        //                    }
        //                    else if (instalmentquery.Count() == 0)
        //                    {
        //                        TaxableValue = AvailableBalance;
        //                        if (model.TaxableValue > TaxableValue)
        //                        {
        //                            return -4;
        //                        }
        //                    }
        //                }
        //                tblProformaInvoice Invoice = new tblProformaInvoice();
        //                Invoice.CrtdUserId = LoggedInUser;
        //                Invoice.CrtdTS = DateTime.Now;
        //                Invoice.InvoiceNumber = model.InvoiceNumber;
        //                Invoice.InvoiceType = model.InvoiceType;
        //                Invoice.InvoiceDate = model.InvoiceDate;
        //                Invoice.ProjectId = model.ProjectID;
        //                Invoice.PIId = model.PIId;
        //                Invoice.TaxCode = model.SACNumber;
        //                Invoice.AgencyId = model.SponsoringAgency;
        //                Invoice.ServiceType = model.ServiceType;
        //                Invoice.DescriptionofServices = model.DescriptionofServices;
        //                Invoice.TaxableValue = model.TaxableValue;
        //                Invoice.CurrencyCode = model.CurrencyCode;
        //                Invoice.InstalmentNumber = model.Instalmentnumber;
        //                Invoice.InstalmentYear = model.Instlmntyr;
        //                Invoice.TotalInvoiceValue = model.TotalInvoiceValue;
        //                Invoice.TotalInvoiceValueinWords = model.TotalInvoiceValueinwords;
        //                Invoice.Bank = model.Bank;
        //                Invoice.BankAccountNumber = model.BankAccountNumber;
        //                Invoice.Status = "Approval Pending";
        //                var PI = model.PIId;
        //                var facultycode = Common.getfacultycode(Convert.ToInt32(PI));
        //                int CurrentYear = (DateTime.Today.Year) % 100;
        //                int PreviousYear = (DateTime.Today.Year - 1) % 100;
        //                int NextYear = (DateTime.Today.Year + 1) % 100;
        //                string PreYear = PreviousYear.ToString();
        //                string NexYear = NextYear.ToString();
        //                string CurYear = CurrentYear.ToString();
        //                string FinYear = null;

        //                if (DateTime.Today.Month > 3)
        //                    FinYear = CurYear + NexYear;
        //                else
        //                    FinYear = PreYear + CurYear;
        //                var Sequencenumber = Common.getInvoiceId();
        //                if (Sequencenumber != null && Sequencenumber != "")
        //                {
        //                    //model.InvoiceNumber = "C" + FinYear + facultycode + "P" + Sequencenumber;
        //                    model.InvoiceNumber = "C" + FinYear + facultycode + "P" + Sequencenumber;
        //                }
        //                else
        //                {
        //                    model.InvoiceNumber = "C" + FinYear + facultycode + "P00001";
        //                }
        //                Invoice.InvoiceNumber = model.InvoiceNumber;
        //                context.tblProformaInvoice.Add(Invoice);
        //                context.SaveChanges();

        //                int InvoiceId = Invoice.InvoiceId;
        //                tblProformaInvoiceTaxDetails tax = new tblProformaInvoiceTaxDetails();
        //                if (model.IGSTPercentage == "NA")
        //                {
        //                    tax.InvoiceId = InvoiceId;
        //                  //  Regex regexObj = new Regex(@"[^\d]");
        //                    var cgstpercent = model.CGSTPercentage.Substring(0, (model.CGSTPercentage.Length - 1));
        //                    tax.CGSTRate = Convert.ToDecimal(cgstpercent);
        //                    tax.CGSTAmount = model.CGST;
        //                    var sgstpercent = model.SGSTPercentage.Substring(0, (model.SGSTPercentage.Length - 1));
        //                    tax.SGSTRate = Convert.ToDecimal(sgstpercent);
        //                    tax.SGSTAmount = model.SGST;
        //                    tax.IGSTRate = 0;
        //                    tax.IGSTAmount = 0;
        //                    tax.CrtdTS = DateTime.Now;
        //                    tax.CrtdUserId = LoggedInUser;
        //                    tax.TotalTaxValue = model.TotalTaxValue;
        //                }
        //                if (model.IGSTPercentage != "NA")
        //                {
        //                    //Regex regexObj = new Regex(@"[^\d]");
        //                    tax.InvoiceId = InvoiceId;
        //                    tax.CGSTRate = 0;
        //                    tax.CGSTAmount = 0;
        //                    tax.SGSTRate = 0;
        //                    tax.SGSTAmount = 0;
        //                    var igstrate = model.IGSTPercentage.Substring(0, (model.IGSTPercentage.Length - 1));
        //                    tax.IGSTRate = Convert.ToDecimal(igstrate);
        //                    tax.IGSTAmount = model.IGST;
        //                    tax.CrtdTS = DateTime.Now;
        //                    tax.CrtdUserId = LoggedInUser;
        //                    tax.TotalTaxValue = model.TotalTaxValue;
        //                }
        //                context.tblProformaInvoiceTaxDetails.Add(tax);
        //                context.SaveChanges();


        //                if (model.InvoiceDraftId != null && model.InvoiceDraftId != 0)
        //                {
        //                    tblProformaInvoiceDraft InvoiceDraft = new tblProformaInvoiceDraft();
        //                    InvoiceDraft.Status = "Invoiced";
        //                }
        //                return InvoiceId;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        return -1;
        //    }
        //}      
        public CreateInvoiceModel GetDetailsforProformaInvoice(int ProjectID)
        {
            try
            {
                CreateInvoiceModel model = new CreateInvoiceModel();

                using (var context = new IOASDBEntities())
                {

                    var query = (from P in context.tblProject
                                 join user in context.vwFacultyStaffDetails on P.PIName equals user.UserId into g
                                 join agency in context.tblAgencyMaster on P.SponsoringAgency equals agency.AgencyId into i
                                 from user in g.DefaultIfEmpty()
                                 from agency in i.DefaultIfEmpty()
                                 where P.ProjectId == ProjectID
                                 select new { P, user, agency }).FirstOrDefault();
                    var finyear = (from year in context.tblFinYear
                                   where year.CurrentYearFlag == true
                                   select year).FirstOrDefault();
                    var projectid = query.P.ProjectId;
                    var currentfinyear = finyear.Year;
                    if (query != null)
                    {
                        model.InvoiceDate = DateTime.Now;
                        model.Invoicedatestrng = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);
                        model.ProjectNumber = query.P.ProjectNumber;
                        model.Projecttitle = query.P.ProjectTitle;
                        model.ProjectID = projectid;
                        model.ProjectType = query.P.ProjectType;
                        model.PIDepartmentName = query.user.DepartmentName;
                        model.PIId = query.P.PIName;
                        model.NameofPI = query.user.FirstName;
                        model.SanctionOrderNumber = query.P.SanctionOrderNumber;
                        model.Sanctionvalue = query.P.SanctionValue;
                        model.SponsoringAgency = query.P.SponsoringAgency;
                        model.SponsoringAgencyName = query.agency.AgencyName;
                        model.Agencyregaddress = query.agency.Address;
                        model.Agencydistrict = query.agency.District;
                        model.AgencyPincode = query.agency.PinCode;
                        model.Agencystate = query.agency.State;
                        model.Agencystatecode = Convert.ToInt32(query.agency.StateCode);
                        model.GSTNumber = query.agency.GSTIN;
                        model.PAN = query.agency.PAN;
                        model.TAN = query.agency.TAN;
                        model.Agencycontactperson = query.agency.ContactPerson;
                        model.AgencycontactpersonEmail = query.agency.ContactEmail;
                        model.Agencycontactpersonmobile = query.agency.ContactNumber;
                        model.CommunicationAddress = query.agency.Address;
                        model.TaxStatus = query.P.TaxStatus;
                        model.BankName = query.agency.BankName;
                        model.BankAccountNumber = query.agency.AccountNumber;
                        var agencytype = query.agency.AgencyType;
                        var agencycategory = query.agency.AgencyCountryCategoryId;
                        var indianagencycategory = query.agency.IndianAgencyCategoryId;
                        var nonsezcategory = query.agency.NonSezCategoryId;
                        if (agencycategory == 1)
                        {
                            if (indianagencycategory == 1)
                            {
                                model.InvoiceType = 2;
                            }
                            if (indianagencycategory == 2 && nonsezcategory == 1)
                            {
                                model.InvoiceType = 3;
                            }
                            if (indianagencycategory == 2 && nonsezcategory == 2)
                            {
                                model.InvoiceType = 4;
                            }
                            if (agencytype == 1)
                            {
                                model.InvoiceType = 2;
                            }
                        }
                        if (agencycategory == 2)
                        {
                            model.InvoiceType = 1;
                        }
                        
                        model.CurrentFinancialYear = currentfinyear;
                        model.AvailableBalance = query.P.SanctionValue;
                        Nullable<Decimal> totalinvoicevalue = 0;
                        var invoicequery = (from I in context.tblProformaInvoice
                                            where I.ProjectId == ProjectID
                                            select I).ToList();
                        var invquery = (from I in context.tblProjectInvoice
                                        where I.ProjectId == ProjectID
                                        select I).ToList();
                        decimal? totalinv = invquery.Select(m => m.TotalInvoiceValue).Sum();
                        if (invoicequery.Count() > 0)
                        {
                            Nullable<int>[] _invoiceid = new Nullable<int>[invoicequery.Count];
                            string[] _invoicenumber = new string[invoicequery.Count];
                            Nullable<Decimal>[] _invoicevalue = new Nullable<Decimal>[invoicequery.Count];
                            string[] _invoicedate = new string[invoicequery.Count];

                            for (int i = 0; i < invoicequery.Count(); i++)
                            {
                                _invoiceid[i] = invoicequery[i].InvoiceId;
                                _invoicenumber[i] = invoicequery[i].InvoiceNumber;
                                _invoicevalue[i] = Convert.ToDecimal(invoicequery[i].TotalInvoiceValue);
                                _invoicedate[i] = String.Format("{0:ddd dd-MMM-yyyy}", invoicequery[i].InvoiceDate);
                                totalinvoicevalue += _invoicevalue[i];
                            }
                            model.PreviousInvoiceDate = _invoicedate;
                            model.PreviousInvoiceId = _invoiceid;
                            model.PreviousInvoiceNumber = _invoicenumber;
                            model.PreviousInvoicevalue = _invoicevalue;
                            model.AvailableBalance = model.Sanctionvalue - (totalinvoicevalue + totalinv);
                        }

                        var instalmentquery = (from I in context.tblInstallment
                                               where I.ProjectId == ProjectID
                                               select I).ToList();
                        if (instalmentquery.Count() > 0)
                        {
                            Nullable<int>[] _instalmentid = new Nullable<int>[instalmentquery.Count];
                            Nullable<int>[] _instalmentnumber = new Nullable<int>[instalmentquery.Count];
                            Nullable<int>[] _instalmentyear = new Nullable<int>[instalmentquery.Count];
                            Nullable<Decimal>[] _instalmentvalue = new Nullable<Decimal>[instalmentquery.Count];
                            Nullable<Decimal> _totalinstalmentvalue = 0;
                            string[] _isInvoiced = new string[instalmentquery.Count];
                            for (int i = 0; i < instalmentquery.Count(); i++)
                            {
                                _instalmentid[i] = instalmentquery[i].InstallmentID;
                                _instalmentnumber[i] = instalmentquery[i].InstallmentNo;
                                _instalmentyear[i] = instalmentquery[i].Year;
                                _isInvoiced[i] = "No";
                                if (query.P.IsYearWiseAllocation == true)
                                {
                                    _instalmentvalue[i] = Convert.ToDecimal(instalmentquery[i].InstallmentValue);
                                    for (int j = 0; j < invoicequery.Count(); j++)
                                    {
                                        if (invoicequery[j].InstalmentNumber == instalmentquery[i].InstallmentNo && invoicequery[j].InstalmentYear == instalmentquery[i].Year)
                                        {
                                            _isInvoiced[i] = "Yes";
                                        }
                                    }
                                }
                                if (query.P.IsYearWiseAllocation != true)
                                {
                                    _instalmentvalue[i] = Convert.ToDecimal(instalmentquery[i].InstallmentValue);
                                    for (int j = 0; j < invoicequery.Count(); j++)
                                    {
                                        if (invoicequery[j].InstalmentNumber == instalmentquery[i].InstallmentNo)
                                        {
                                            _isInvoiced[i] = "Yes";
                                        }
                                    }
                                }
                                _totalinstalmentvalue += _instalmentvalue[i];
                                //  totalinvoicevalue += _invoicevalue[i];
                            }
                            model.InstalmentId = _instalmentid;
                            model.InstlmntNumber = _instalmentnumber;
                            model.InstalValue = _instalmentvalue;
                            model.Instalmentyear = _instalmentyear;
                            model.Invoiced = _isInvoiced;
                            // model.AvailableBalance = model.Sanctionvalue - _totalinstalmentvalue;
                        }

                        if (query.P.IsYearWiseAllocation == true)
                        {
                            DateTime startdate = DateTime.Now;
                            DateTime enddate = DateTime.Now;
                            DateTime today = DateTime.Now;

                            if (query.P.ActualStartDate == null)
                            {
                                startdate = (DateTime)query.P.TentativeStartDate;
                                enddate = (DateTime)query.P.TentativeCloseDate;
                                TimeSpan diff_date = today - startdate;
                                int noofdays = diff_date.Days;
                                int years = noofdays / 365;
                                int currentprojectyear = 0;
                                if (years == 0)
                                {
                                    currentprojectyear = 1;
                                }
                                if (years > 0)
                                {
                                    currentprojectyear = years;
                                }
                                if (instalmentquery.Count() > 0)
                                {
                                    var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
                                                                     where ins.ProjectId == ProjectID && ins.InstalmentYear == currentprojectyear
                                                                     orderby ins.InvoiceId descending
                                                                     select ins).FirstOrDefault();
                                    int lastinvoicedinstalment = 0;
                                    decimal? currinstval = 0;
                                    decimal? balincurrinstval = 0;
                                    int currentinstalment = 0;
                                    decimal? totalinsinvvalue = 0;
                                    decimal? ttlinsinvvalue = 0;
                                    if (previousinstalmentinvoice != null)
                                    {
                                        lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;

                                        var instalinv = (from ins in context.tblProformaInvoice
                                                         where ins.ProjectId == ProjectID && ins.InstalmentYear == currentprojectyear && ins.InstalmentNumber == lastinvoicedinstalment
                                                         orderby ins.InvoiceId descending
                                                         select ins).ToList();
                                        var insinv = (from ins in context.tblProjectInvoice
                                                         where ins.ProjectId == ProjectID && ins.InstalmentYear == currentprojectyear && ins.InstalmentNumber == lastinvoicedinstalment
                                                         orderby ins.InvoiceId descending
                                                         select ins).ToList();

                                        if (instalinv != null)
                                        {
                                            totalinsinvvalue = instalinv.Select(m => m.TotalInvoiceValue).Sum() ?? 0;
                                        }
                                        if (insinv != null)
                                        {
                                            ttlinsinvvalue = insinv.Select(m => m.TaxableValue).Sum() ?? 0;
                                        }
                                        var currinstalment = (from ins in context.tblInstallment
                                                              where (ins.ProjectId == projectid && ins.Year == currentprojectyear && ins.InstallmentNo == lastinvoicedinstalment)
                                                              select ins).FirstOrDefault();
                                        if (currinstalment != null)
                                        {
                                            currinstval = currinstalment.InstallmentValue;
                                        }
                                        balincurrinstval = currinstval - (totalinsinvvalue + ttlinsinvvalue);
                                        if (balincurrinstval <= 0)
                                        {
                                            currentinstalment = lastinvoicedinstalment + 1;
                                        }
                                        else if (balincurrinstval > 0)
                                        {
                                            currentinstalment = lastinvoicedinstalment;
                                        }
                                    }
                                    else if (previousinstalmentinvoice == null)
                                    {
                                        currentinstalment = lastinvoicedinstalment + 1;
                                        //var currinstalment = (from ins in context.tblInstallment
                                        //                      where (ins.ProjectId == projectid && ins.Year == currentprojectyear && ins.InstallmentNo == currentinstalment)
                                        //                      select ins).FirstOrDefault();
                                        //balincurrinstval = currinstalment.InstallmentValue;
                                    }
                                    var instalment = (from ins in context.tblInstallment
                                                      where (ins.ProjectId == projectid && ins.Year == currentprojectyear && ins.InstallmentNo == currentinstalment)
                                                      select ins).FirstOrDefault();
                                    if (instalment != null)
                                    {
                                        model.Instlmntyr = instalment.Year;
                                        model.Instalmentnumber = instalment.InstallmentNo;
                                        model.Instalmentvalue = instalment.InstallmentValue;
                                        model.TaxableValue = instalment.InstallmentValue - balincurrinstval;
                                    }
                                    else if (instalment == null)
                                    {
                                        model.Instlmntyr = currentprojectyear;
                                        model.TaxableValue = model.AvailableBalance;
                                    }
                                }
                                else if (instalmentquery.Count() == 0)
                                {
                                    var aloc = (from alloc in context.tblProjectAllocation
                                                where (alloc.ProjectId == projectid)
                                                select alloc).ToList();
                                    var inv = (from alloc in context.tblProformaInvoice
                                               where (alloc.ProjectId == projectid && alloc.InstalmentYear == currentprojectyear)
                                               select alloc).ToList();
                                    var invoice = (from alloc in context.tblProjectInvoice
                                                   where (alloc.ProjectId == projectid && alloc.InstalmentYear == currentprojectyear)
                                                   select alloc).ToList();
                                    var yearallocamt = (from alloc in context.tblProjectAllocation
                                                        where (alloc.ProjectId == projectid && alloc.Year == currentprojectyear)
                                                        select alloc).ToList();
                                    var nextyear = currentprojectyear + 1;
                                    var nextyearalloc = (from alloc in context.tblProjectAllocation
                                                         where (alloc.ProjectId == projectid && alloc.Year == nextyear)
                                                         select alloc).ToList();
                                    decimal? totalyearalloc = 0;
                                    decimal? invtotal = 0;
                                    decimal? invoicetotal = 0;
                                    decimal? totalpjctalloc = 0;
                                    for (int i = 0; i < aloc.Count(); i++)
                                    {
                                        totalpjctalloc += aloc[i].AllocationValue;
                                    }
                                    for (int i = 0; i < inv.Count(); i++)
                                    {
                                        invtotal += inv[i].TotalInvoiceValue;
                                    }
                                    for (int i = 0; i < invoice.Count(); i++)
                                    {
                                        invoicetotal += inv[i].TotalInvoiceValue;
                                    }
                                    for (int i = 0; i < yearallocamt.Count(); i++)
                                    {
                                        totalyearalloc += yearallocamt[i].AllocationValue;
                                    }
                                    if ((invtotal >= 0 || invoicetotal >= 0) && totalyearalloc > 0)
                                    {
                                        model.TaxableValue = totalyearalloc - (invtotal + invoicetotal);
                                    }
                                    model.Instlmntyr = currentprojectyear;
                                    if (totalpjctalloc != model.Sanctionvalue && model.TaxableValue == 0)
                                    {
                                        model.TaxableValue = model.AvailableBalance;
                                    }
                                    //else if (totalyearalloc <= 0)
                                    //{
                                    //    decimal? nextyrallocamt = 0;
                                    //    for (int i = 0; i < yearallocamt.Count(); i++)
                                    //    {
                                    //        nextyrallocamt += nextyearalloc[i].AllocationValue;
                                    //    }
                                    //    model.TaxableValue = nextyrallocamt - invtotal;
                                    //}

                                    // model.TaxableValue = model.AvailableBalance;
                                }

                            }
                            if (query.P.ActualStartDate != null)
                            {
                                startdate = (DateTime)query.P.ActualStartDate;
                                enddate = (DateTime)query.P.ActuaClosingDate;
                                TimeSpan diff_date = today - startdate;
                                int noofdays = Math.Abs(diff_date.Days);
                                int years = noofdays / 365;
                                int currentprojectyear = 0;
                                if (years == 0)
                                {
                                    currentprojectyear = 1;
                                }
                                if (years > 0)
                                {
                                    currentprojectyear = years;
                                }
                                if (instalmentquery.Count() > 0)
                                {
                                    var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
                                                                     where ins.ProjectId == ProjectID && ins.InstalmentYear == currentprojectyear
                                                                     orderby ins.InvoiceId descending
                                                                     select ins).FirstOrDefault();
                                    int lastinvoicedinstalment = 0;
                                    decimal? currinstval = 0;
                                    decimal? balincurrinstval = 0;
                                    int currentinstalment = 0;
                                    decimal? totalinsinvvalue = 0;
                                    decimal? ttlinsinvvalue = 0;
                                    if (previousinstalmentinvoice != null)
                                    {
                                        lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;

                                        var instalinv = (from ins in context.tblProformaInvoice
                                                         where ins.ProjectId == ProjectID && ins.InstalmentYear == currentprojectyear && ins.InstalmentNumber == lastinvoicedinstalment
                                                         orderby ins.InvoiceId descending
                                                         select ins).ToList();
                                        var insinv = (from ins in context.tblProjectInvoice
                                                      where ins.ProjectId == ProjectID && ins.InstalmentYear == currentprojectyear && ins.InstalmentNumber == lastinvoicedinstalment
                                                      orderby ins.InvoiceId descending
                                                      select ins).ToList();

                                        if (instalinv != null)
                                        {
                                            totalinsinvvalue = instalinv.Select(m => m.TotalInvoiceValue).Sum() ?? 0;
                                        }
                                        if (insinv != null)
                                        {
                                            ttlinsinvvalue = insinv.Select(m => m.TaxableValue).Sum() ?? 0;
                                        }
                                        var currinstalment = (from ins in context.tblInstallment
                                                              where (ins.ProjectId == projectid && ins.Year == currentprojectyear && ins.InstallmentNo == lastinvoicedinstalment)
                                                              select ins).FirstOrDefault();
                                        if (currinstalment != null)
                                        {
                                            currinstval = currinstalment.InstallmentValue;
                                        }
                                        balincurrinstval = currinstval - (totalinsinvvalue + ttlinsinvvalue);
                                        if (balincurrinstval <= 0)
                                        {
                                            currentinstalment = lastinvoicedinstalment + 1;
                                        }
                                        else if (balincurrinstval > 0)
                                        {
                                            currentinstalment = lastinvoicedinstalment;
                                        }
                                    }
                                    else if (previousinstalmentinvoice == null)
                                    {
                                        currentinstalment = lastinvoicedinstalment + 1;
                                        //var currinstalment = (from ins in context.tblInstallment
                                        //                      where (ins.ProjectId == projectid && ins.Year == currentprojectyear && ins.InstallmentNo == currentinstalment)
                                        //                      select ins).FirstOrDefault();
                                        //balincurrinstval = currinstalment.InstallmentValue;
                                    }
                                    var instalment = (from ins in context.tblInstallment
                                                      where (ins.ProjectId == projectid && ins.Year == currentprojectyear && ins.InstallmentNo == currentinstalment)
                                                      select ins).FirstOrDefault();
                                    if (instalment != null)
                                    {
                                        model.Instlmntyr = instalment.Year;
                                        model.Instalmentnumber = instalment.InstallmentNo;
                                        model.Instalmentvalue = instalment.InstallmentValue;
                                        model.TaxableValue = instalment.InstallmentValue - balincurrinstval;
                                    }
                                    else if (instalment == null)
                                    {
                                        model.Instlmntyr = currentprojectyear;
                                        model.TaxableValue = model.AvailableBalance;
                                    }
                                }
                                else if (instalmentquery.Count() == 0)
                                {
                                    var aloc = (from alloc in context.tblProjectAllocation
                                                where (alloc.ProjectId == projectid)
                                                select alloc).ToList();
                                    var inv = (from alloc in context.tblProformaInvoice
                                               where (alloc.ProjectId == projectid && alloc.InstalmentYear == currentprojectyear)
                                               select alloc).ToList();
                                    var invoice = (from alloc in context.tblProjectInvoice
                                                   where (alloc.ProjectId == projectid && alloc.InstalmentYear == currentprojectyear)
                                                   select alloc).ToList();
                                    var yearallocamt = (from alloc in context.tblProjectAllocation
                                                        where (alloc.ProjectId == projectid && alloc.Year == currentprojectyear)
                                                        select alloc).ToList();
                                    var nextyear = currentprojectyear + 1;
                                    var nextyearalloc = (from alloc in context.tblProjectAllocation
                                                         where (alloc.ProjectId == projectid && alloc.Year == nextyear)
                                                         select alloc).ToList();
                                    decimal? totalyearalloc = 0;
                                    decimal? invtotal = 0;
                                    decimal? invoicetotal = 0;
                                    decimal? totalpjctalloc = 0;
                                    for (int i = 0; i < aloc.Count(); i++)
                                    {
                                        totalpjctalloc += aloc[i].AllocationValue;
                                    }
                                    for (int i = 0; i < inv.Count(); i++)
                                    {
                                        invtotal += inv[i].TotalInvoiceValue;
                                    }
                                    for (int i = 0; i < invoice.Count(); i++)
                                    {
                                        invoicetotal += inv[i].TotalInvoiceValue;
                                    }
                                    for (int i = 0; i < yearallocamt.Count(); i++)
                                    {
                                        totalyearalloc += yearallocamt[i].AllocationValue;
                                    }
                                    if ((invtotal >= 0 || invoicetotal >= 0) && totalyearalloc > 0)
                                    {
                                        model.TaxableValue = totalyearalloc - (invtotal + invoicetotal);
                                    }
                                    model.Instlmntyr = currentprojectyear;
                                    if (totalpjctalloc != model.Sanctionvalue && model.TaxableValue == 0)
                                    {
                                        model.TaxableValue = model.AvailableBalance;
                                    }
                                    //else if (totalyearalloc <= 0)
                                    //{
                                    //    decimal? nextyrallocamt = 0;
                                    //    for (int i = 0; i < yearallocamt.Count(); i++)
                                    //    {
                                    //        nextyrallocamt += nextyearalloc[i].AllocationValue;
                                    //    }
                                    //    model.TaxableValue = nextyrallocamt - invtotal;
                                    //}

                                    // model.TaxableValue = model.AvailableBalance;
                                }
                            }
                        }
                        if (query.P.IsYearWiseAllocation != true)
                        {
                            if (instalmentquery.Count() > 0)
                            {
                                var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
                                                                 where ins.ProjectId == ProjectID
                                                                 orderby ins.InvoiceId descending
                                                                 select ins).FirstOrDefault();
                                int lastinvoicedinstalment = 0;
                                decimal? currinstval = 0;
                                decimal? balincurrinstval = 0;
                                int currentinstalment = 0;
                                decimal? totalinsinvvalue = 0;
                                decimal? ttlinsinvvalue = 0;
                                if (previousinstalmentinvoice != null)
                                {
                                    lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                    var instalinv = (from ins in context.tblProformaInvoice
                                                     where ins.ProjectId == ProjectID && ins.InstalmentNumber == lastinvoicedinstalment
                                                     orderby ins.InvoiceId descending
                                                     select ins).ToList();
                                    var insinv = (from ins in context.tblProformaInvoice
                                                     where ins.ProjectId == ProjectID && ins.InstalmentNumber == lastinvoicedinstalment
                                                     orderby ins.InvoiceId descending
                                                     select ins).ToList();

                                    if (instalinv != null)
                                    {
                                        totalinsinvvalue = instalinv.Select(m => m.TotalInvoiceValue).Sum() ?? 0;
                                    }
                                    if (insinv != null)
                                    {
                                        ttlinsinvvalue = instalinv.Select(m => m.TotalInvoiceValue).Sum() ?? 0;
                                    }
                                    var currinstalment = (from ins in context.tblInstallment
                                                          where (ins.ProjectId == projectid && ins.InstallmentNo == lastinvoicedinstalment)
                                                          select ins).FirstOrDefault();
                                    if (currinstalment != null)
                                    {
                                        currinstval = currinstalment.InstallmentValue;
                                    }
                                    balincurrinstval = currinstval - (totalinsinvvalue + ttlinsinvvalue);
                                    if (balincurrinstval <= 0)
                                    {
                                        currentinstalment = lastinvoicedinstalment + 1;
                                    }
                                    else if (balincurrinstval > 0)
                                    {
                                        currentinstalment = lastinvoicedinstalment;
                                    }
                                }
                                else if (previousinstalmentinvoice == null)
                                {
                                    currentinstalment = lastinvoicedinstalment + 1;
                                }
                                var instalment = (from ins in context.tblInstallment
                                                  where (ins.ProjectId == projectid && ins.InstallmentNo == currentinstalment)
                                                  select ins).FirstOrDefault();
                                if (instalment != null)
                                {
                                    model.Instlmntyr = instalment.Year;
                                    model.Instalmentnumber = instalment.InstallmentNo;
                                    model.Instalmentvalue = instalment.InstallmentValue;
                                    model.TaxableValue = instalment.InstallmentValue - balincurrinstval;
                                }
                                else if (instalment == null)
                                {
                                    model.TaxableValue = model.AvailableBalance;
                                }
                            }
                            else if (instalmentquery.Count() == 0)
                            {
                                model.TaxableValue = model.AvailableBalance;
                            }
                        }
                        //if (query.P.ProjectType == 1)
                        //{
                        //    var projectcategory = Convert.ToInt32(query.P.SponProjectCategory);                            
                        //    var bankquery = (from Bank in context.tblIITMBankMaster
                        //                      join account in context.tblBankAccountMaster on Bank.BankId equals account.BankId
                        //                      join projectgroup in context.tblBankProjectGroup on account.BankAccountId equals projectgroup.BankAccountId
                        //                      join cc in context.tblCodeControl on
                        //                      new { pjctgroup = projectcategory, codeName = "SponBankProjectGroup" } equals
                        //                      new { pjctgroup = cc.CodeValAbbr, codeName = cc.CodeName }
                        //                      where projectgroup.ProjectGroup == projectcategory
                        //                      select new { Bank, account, projectgroup, cc }).FirstOrDefault();

                        //    if (bankquery != null)
                        //    {
                        //        model.Bank = bankquery.Bank.BankId;
                        //        model.BankName = bankquery.Bank.BankName;
                        //        model.BankAccountNumber = bankquery.account.AccountNumber;
                        //        model.BankAccountId = bankquery.account.BankAccountId;
                        //    }                            
                        //   // model.TaxableValue = model.AvailableBalance;
                        //}
                        //if (query.P.ProjectType == 2)
                        //{
                        //    var prjctgroup = 0;
                        //    if(query.P.ConsProjectSubType == 1)
                        //    {
                        //        prjctgroup = 3;
                        //    }
                        //    if (query.P.ConsProjectSubType == 2)
                        //    {
                        //        prjctgroup = 4;
                        //    }

                        //var bankquery = (from Bank in context.tblIITMBankMaster
                        //                 join account in context.tblBankAccountMaster on Bank.BankId equals account.BankId
                        //                 join projectgroup in context.tblBankProjectGroup on account.BankAccountId equals projectgroup.BankAccountId
                        //                 join cc in context.tblCodeControl on
                        //                 new { pjctgroup = prjctgroup, codeName = "ConsBankProjectGroup" } equals
                        //                 new { pjctgroup = cc.CodeValAbbr, codeName = cc.CodeName }
                        //                 where projectgroup.ProjectGroup == prjctgroup
                        //                 select new { Bank, account, projectgroup, cc }).FirstOrDefault();
                        //if (bankquery != null)
                        //{
                        //    model.Bank = bankquery.Bank.BankId;
                        //    model.BankName = bankquery.Bank.BankName;
                        //    model.BankAccountNumber = bankquery.account.AccountNumber;
                        //    model.BankAccountId = bankquery.account.BankAccountId;
                        //}
                        //}
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public CreateInvoiceModel GetProformaInvoiceDraftDetails(int DraftId)
        {
            try
            {
                CreateInvoiceModel model = new CreateInvoiceModel();

                using (var context = new IOASDBEntities())
                {
                    var invquery = (from inv in context.tblProformaInvoiceDraft
                                    where inv.InvoiceDraftId == DraftId
                                    select inv).FirstOrDefault();
                    var projectid = invquery.ProjectId;
                    var query = (from P in context.tblProject
                                     //join cc in context.tblCodeControl on
                                     //new { Category = srb.ItemCategoryId ?? 0, codeName = "SRB_ItemCategory" } equals
                                     //new { Category = cc.CodeValAbbr, codeName = cc.CodeName }
                                 join user in context.vwFacultyStaffDetails on P.PIName equals user.UserId into g
                                 join agency in context.tblAgencyMaster on P.SponsoringAgency equals agency.AgencyId into i
                                 from user in g.DefaultIfEmpty()
                                 from agency in i.DefaultIfEmpty()
                                 where P.ProjectId == projectid
                                 select new { P, user, agency }).FirstOrDefault();
                    var finyear = (from year in context.tblFinYear
                                   where year.CurrentYearFlag == true
                                   select year).FirstOrDefault();

                    var currentfinyear = finyear.Year;
                    if (query != null)
                    {
                        model.CurrentFinancialYear = currentfinyear;
                        model.InvoiceDate = invquery.InvoiceDate;
                        // model.InvoiceId = DraftId;
                        model.InvoiceDraftId = DraftId;
                        model.DescriptionofServices = invquery.DescriptionofServices;
                        model.ServiceType = invquery.ServiceType;
                        model.TaxableValue = invquery.TaxableValue;
                        model.PONumber = invquery.PONumber;
                        model.CommunicationAddress = invquery.CommunicationAddress;
                        //  model.TotalInvoiceValue = invquery.TotalInvoiceValue;
                        model.Invoicedatestrng = String.Format("{0:ddd dd-MMM-yyyy}", invquery.InvoiceDate);
                        model.ProjectNumber = query.P.ProjectNumber;
                        model.ProjectID = query.P.ProjectId;
                        model.Projecttitle = query.P.ProjectTitle;
                        model.ProjectType = query.P.ProjectType;
                        // model.Department = query.user.DepartmentId;
                        model.PIDepartmentName = query.user.DepartmentName;
                        model.PIId = query.P.PIName;
                        model.NameofPI = query.user.FirstName;
                        model.SanctionOrderNumber = query.P.SanctionOrderNumber;
                        model.Sanctionvalue = query.P.SanctionValue;
                        model.SponsoringAgency = query.P.SponsoringAgency;
                        model.SponsoringAgencyName = query.agency.AgencyName;
                        model.Agencyregaddress = query.agency.Address;
                        model.Agencydistrict = query.agency.District;
                        model.AgencyPincode = query.agency.PinCode;
                        model.Agencystate = query.agency.State;
                        model.Agencystatecode = Convert.ToInt32(query.agency.StateCode);
                        model.GSTNumber = query.agency.GSTIN;
                        model.PAN = query.agency.PAN;
                        model.TAN = query.agency.TAN;
                        model.Instlmntyr = invquery.InstalmentYear;
                        model.Instalmentnumber = invquery.InstalmentYear;
                        model.Agencycontactperson = query.agency.ContactPerson;
                        model.AgencycontactpersonEmail = query.agency.ContactEmail;
                        model.Agencycontactpersonmobile = query.agency.ContactNumber;
                        model.CommunicationAddress = query.agency.Address;
                        model.InvoiceType = invquery.InvoiceType;
                        Nullable<Decimal> totalinvoicevalue = 0;
                        var invoicequery = (from I in context.tblProformaInvoice
                                            where I.ProjectId == projectid
                                            select I).ToList();
                        if (invoicequery.Count() > 0)
                        {
                            Nullable<int>[] _invoiceid = new Nullable<int>[invoicequery.Count];
                            string[] _invoicenumber = new string[invoicequery.Count];
                            Nullable<Decimal>[] _invoicevalue = new Nullable<Decimal>[invoicequery.Count];
                            string[] _invoicedate = new string[invoicequery.Count];
                            for (int i = 0; i < invoicequery.Count(); i++)
                            {
                                _invoiceid[i] = invoicequery[i].InvoiceId;
                                _invoicenumber[i] = invoicequery[i].InvoiceNumber;
                                _invoicevalue[i] = Convert.ToDecimal(invoicequery[i].TotalInvoiceValue);
                                _invoicedate[i] = String.Format("{0:ddd dd-MMM-yyyy}", invoicequery[i].InvoiceDate);
                                totalinvoicevalue += _invoicevalue[i];
                            }
                            model.PreviousInvoiceDate = _invoicedate;
                            model.PreviousInvoiceId = _invoiceid;
                            model.PreviousInvoiceNumber = _invoicenumber;
                            model.PreviousInvoicevalue = _invoicevalue;
                            model.AvailableBalance = model.Sanctionvalue - totalinvoicevalue;
                        }

                        var instalmentquery = (from I in context.tblInstallment
                                               where I.ProjectId == projectid
                                               select I).ToList();
                        if (instalmentquery.Count() > 0)
                        {
                            Nullable<int>[] _instalmentid = new Nullable<int>[instalmentquery.Count];
                            Nullable<int>[] _instalmentnumber = new Nullable<int>[instalmentquery.Count];
                            Nullable<int>[] _instalmentyear = new Nullable<int>[instalmentquery.Count];
                            Nullable<Decimal>[] _instalmentvalue = new Nullable<Decimal>[instalmentquery.Count];
                            string[] _isInvoiced = new string[instalmentquery.Count];
                            for (int i = 0; i < instalmentquery.Count(); i++)
                            {
                                _instalmentid[i] = instalmentquery[i].InstallmentID;
                                _instalmentnumber[i] = instalmentquery[i].InstallmentNo;
                                _instalmentyear[i] = instalmentquery[i].Year;
                                _isInvoiced[i] = "No";
                                if (query.P.IsYearWiseAllocation == true)
                                {
                                    _instalmentvalue[i] = Convert.ToDecimal(instalmentquery[i].InstallmentValue);
                                    for (int j = 0; j < invoicequery.Count(); j++)
                                    {
                                        if (invoicequery[j].InstalmentNumber == instalmentquery[i].InstallmentNo && invoicequery[j].InstalmentYear == instalmentquery[i].Year)
                                        {
                                            _isInvoiced[i] = "Yes";
                                        }
                                    }
                                }
                                if (query.P.IsYearWiseAllocation != true)
                                {
                                    _instalmentvalue[i] = Convert.ToDecimal(instalmentquery[i].InstallmentValue);
                                    for (int j = 0; j < invoicequery.Count(); j++)
                                    {
                                        if (invoicequery[j].InstalmentNumber == instalmentquery[i].InstallmentNo)
                                        {
                                            _isInvoiced[i] = "Yes";
                                        }
                                    }
                                }
                                //  totalinvoicevalue += _invoicevalue[i];
                            }
                            model.InstalmentId = _instalmentid;
                            model.InstlmntNumber = _instalmentnumber;
                            model.InstalValue = _instalmentvalue;
                            model.Instalmentyear = _instalmentyear;
                            model.Invoiced = _isInvoiced;
                        }
                            if (query.P.IsYearWiseAllocation == true)
                        {
                            DateTime startdate = DateTime.Now;
                            DateTime enddate = DateTime.Now;
                            DateTime today = DateTime.Now;

                            if (query.P.ActualStartDate == null)
                            {
                                startdate = (DateTime)query.P.TentativeStartDate;
                                enddate = (DateTime)query.P.TentativeCloseDate;
                                TimeSpan diff_date = today - startdate;
                                int noofdays = diff_date.Days;
                                int years = noofdays / 365;
                                int currentprojectyear = 0;
                                if (years == 0)
                                {
                                    currentprojectyear = 1;
                                }
                                if (years > 0)
                                {
                                    currentprojectyear = years;
                                }
                                if (instalmentquery.Count() > 0)
                                {
                                    var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
                                                                     where ins.ProjectId == projectid && ins.InstalmentYear == currentprojectyear
                                                                     orderby ins.InvoiceId descending
                                                                     select ins).FirstOrDefault();
                                    int lastinvoicedinstalment = 0;
                                    if (previousinstalmentinvoice != null)
                                    {
                                        lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                    }
                                    int currentinstalment = lastinvoicedinstalment + 1;
                                    var instalment = (from ins in context.tblInstallment
                                                      where (ins.ProjectId == projectid && ins.Year == currentprojectyear && ins.InstallmentNo == currentinstalment)
                                                      select ins).FirstOrDefault();
                                    if (instalment != null)
                                    {
                                        model.Instlmntyr = instalment.Year;
                                        model.Instalmentnumber = instalment.InstallmentNo;
                                        model.Instalmentvalue = instalment.InstallmentValue;
                                        model.TaxableValue = instalment.InstallmentValue;
                                    }
                                    else if (instalment == null)
                                    {
                                        model.TaxableValue = 0;
                                    }
                                }
                                else if (instalmentquery.Count() == 0)
                                {
                                    model.TaxableValue = model.AvailableBalance;
                                }

                            }
                            if (query.P.ActualStartDate != null)
                            {
                                startdate = (DateTime)query.P.ActualStartDate;
                                enddate = (DateTime)query.P.ActuaClosingDate;
                                TimeSpan diff_date = today - startdate;
                                int noofdays = diff_date.Days;
                                int years = noofdays / 365;
                                int currentprojectyear = 0;
                                if (years == 0)
                                {
                                    currentprojectyear = 1;
                                }
                                if (years > 0)
                                {
                                    currentprojectyear = years;
                                }
                                if (instalmentquery.Count() > 0)
                                {
                                    var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
                                                                     where ins.ProjectId == projectid && ins.InstalmentYear == currentprojectyear
                                                                     orderby ins.InvoiceId descending
                                                                     select ins).FirstOrDefault();
                                    int lastinvoicedinstalment = 0;
                                    if (previousinstalmentinvoice != null)
                                    {
                                        lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                    }
                                    int currentinstalment = lastinvoicedinstalment + 1;
                                    var instalment = (from ins in context.tblInstallment
                                                      where (ins.ProjectId == projectid && ins.Year == currentprojectyear && ins.InstallmentNo == currentinstalment)
                                                      select ins).FirstOrDefault();
                                    if (instalment != null)
                                    {
                                        model.Instlmntyr = instalment.Year;
                                        model.Instalmentnumber = instalment.InstallmentNo;
                                        model.Instalmentvalue = instalment.InstallmentValue;
                                        model.TaxableValue = instalment.InstallmentValue;
                                    }
                                    else if (instalment == null)
                                    {
                                        model.TaxableValue = 0;
                                    }
                                }
                                else if (instalmentquery.Count() == 0)
                                {
                                    model.TaxableValue = model.AvailableBalance;
                                }
                            }
                        }
                        if (query.P.IsYearWiseAllocation != true)
                        {
                            if (instalmentquery.Count() > 0)
                            {
                                var previousinstalmentinvoice = (from ins in context.tblProformaInvoice
                                                                 where ins.ProjectId == projectid
                                                                 orderby ins.InvoiceId descending
                                                                 select ins).FirstOrDefault();
                                int lastinvoicedinstalment = 0;
                                if (previousinstalmentinvoice != null)
                                {
                                    lastinvoicedinstalment = previousinstalmentinvoice.InstalmentNumber ?? 0;
                                }
                                int currentinstalment = lastinvoicedinstalment + 1;
                                var instalment = (from ins in context.tblInstallment
                                                  where (ins.ProjectId == projectid && ins.InstallmentNo == currentinstalment)
                                                  select ins).FirstOrDefault();
                                if (instalment != null)
                                {
                                    model.Instlmntyr = instalment.Year;
                                    model.Instalmentnumber = instalment.InstallmentNo;
                                    model.Instalmentvalue = instalment.InstallmentValue;
                                    model.TaxableValue = instalment.InstallmentValue;
                                }
                                else if (instalment == null)
                                {
                                    model.TaxableValue = 0;
                                }
                            }
                            else if (instalmentquery.Count() == 0)
                            {
                                model.TaxableValue = model.AvailableBalance;
                            }
                        }
                        if (query.P.ProjectType == 1)
                        {
                            var projectcategory = Convert.ToInt32(query.P.SponProjectCategory);
                            var bankquery = (from Bank in context.tblIITMBankMaster
                                             join account in context.tblBankAccountMaster on Bank.BankId equals account.BankId
                                             join projectgroup in context.tblBankProjectGroup on account.BankAccountId equals projectgroup.BankAccountId
                                             join cc in context.tblCodeControl on
                                             new { pjctgroup = projectcategory, codeName = "SponBankProjectGroup" } equals
                                             new { pjctgroup = cc.CodeValAbbr, codeName = cc.CodeName }
                                             where projectgroup.ProjectGroup == projectcategory
                                             select new { Bank, account, projectgroup, cc }).FirstOrDefault();
                            if (bankquery != null)
                            {
                                model.Bank = bankquery.Bank.BankId;
                                model.BankName = bankquery.Bank.BankName;
                                model.BankAccountNumber = bankquery.account.AccountNumber;
                                model.BankAccountId = bankquery.account.BankAccountId;
                            }
                        }
                        if (query.P.ProjectType == 2)
                        {
                            var projectcategory = Convert.ToInt32(query.P.ConsProjectSubType);
                            
                            var bankquery = (from Bank in context.tblIITMBankMaster
                                             join account in context.tblBankAccountMaster on Bank.BankId equals account.BankId
                                             join projectgroup in context.tblBankProjectGroup on account.BankAccountId equals projectgroup.BankAccountId
                                             join cc in context.tblCodeControl on
                                             new { pjctgroup = projectcategory, codeName = "ConsBankProjectGroup" } equals
                                             new { pjctgroup = cc.CodeValAbbr, codeName = cc.CodeName }
                                             select new { Bank, account, projectgroup, cc }).FirstOrDefault();
                            if (bankquery != null)
                            {
                                model.Bank = bankquery.Bank.BankId;
                                model.BankName = bankquery.Bank.BankName;
                                model.BankAccountNumber = bankquery.account.AccountNumber;
                                model.BankAccountId = bankquery.account.BankAccountId;
                            }
                        }
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public CreateInvoiceModel GetProformaInvoiceDetails(int InvoiceId)
        {
            try
            {
                CreateInvoiceModel model = new CreateInvoiceModel();

                using (var context = new IOASDBEntities())
                {
                    var invquery = (from inv in context.tblProformaInvoice
                                    where inv.InvoiceId == InvoiceId
                                    select inv).FirstOrDefault();
                    if (invquery != null)
                    {
                    var projectid = invquery.ProjectId;
                    var query = (from P in context.tblProject
                                     //join cc in context.tblCodeControl on
                                     //new { Category = srb.ItemCategoryId ?? 0, codeName = "SRB_ItemCategory" } equals
                                     //new { Category = cc.CodeValAbbr, codeName = cc.CodeName }
                                 join user in context.vwFacultyStaffDetails on P.PIName equals user.UserId into g
                                 join agency in context.tblAgencyMaster on P.SponsoringAgency equals agency.AgencyId into i
                                 from user in g.DefaultIfEmpty()
                                 from agency in i.DefaultIfEmpty()
                                 where P.ProjectId == projectid
                                 select new { P, user, agency }).FirstOrDefault();
                    var finyear = (from year in context.tblFinYear
                                   where year.CurrentYearFlag == true
                                   select year).FirstOrDefault();
                    var currentfinyear = finyear.Year;

                    
                        model.CurrentFinancialYear = currentfinyear;
                        model.InvoiceDate = invquery.InvoiceDate;
                        model.InvoiceId = InvoiceId;
                        model.InvoiceNumber = invquery.InvoiceNumber;
                        model.DescriptionofServices = invquery.DescriptionofServices;
                        model.ServiceType = invquery.ServiceType;
                        model.SACNumber = invquery.TaxCode;
                        model.TaxableValue = invquery.TaxableValue;
                        model.TotalInvoiceValue = invquery.TotalInvoiceValue;
                        model.Invoicedatestrng = String.Format("{0:ddd dd-MMM-yyyy}", invquery.InvoiceDate);
                        model.ProjectNumber = query.P.ProjectNumber;
                        model.ProjectID = query.P.ProjectId;
                        model.ProjectType = query.P.ProjectType;
                        model.Projecttitle = query.P.ProjectTitle;
                        //   model.Department = query.user.DepartmentCode;
                        model.PIDepartmentName = query.user.DepartmentName;
                        model.PIId = query.P.PIName;
                        model.NameofPI = query.user.FirstName;
                        model.SanctionOrderNumber = query.P.SanctionOrderNumber;
                        model.Sanctionvalue = query.P.SanctionValue;
                        model.SponsoringAgency = query.P.SponsoringAgency;
                        model.SponsoringAgencyName = query.agency.AgencyName;
                        model.Agencyregaddress = query.agency.Address;
                        model.Agencydistrict = query.agency.District;
                        model.AgencyPincode = query.agency.PinCode;
                        model.Agencystate = query.agency.State;
                        model.Agencystatecode = Convert.ToInt32(query.agency.StateCode);
                        model.GSTNumber = query.agency.GSTIN;
                        model.PAN = query.agency.PAN;
                        model.TAN = query.agency.TAN;
                        model.Agencycontactperson = query.agency.ContactPerson;
                        model.AgencycontactpersonEmail = query.agency.ContactEmail;
                        model.Agencycontactpersonmobile = query.agency.ContactNumber;
                        model.CommunicationAddress = invquery.CommunicationAddress;
                        model.PONumber = invquery.PONumber;
                        model.InvoiceType = invquery.InvoiceType;
                        Nullable<Decimal> totalinvoicevalue = 0;
                        var invoicequery = (from I in context.tblProformaInvoice
                                            where I.ProjectId == projectid
                                            select I).ToList();
                        if (invoicequery.Count() > 0)
                        {
                            Nullable<int>[] _invoiceid = new Nullable<int>[invoicequery.Count];
                            string[] _invoicenumber = new string[invoicequery.Count];
                            Nullable<Decimal>[] _invoicevalue = new Nullable<Decimal>[invoicequery.Count];
                            string[] _invoicedate = new string[invoicequery.Count];

                            for (int i = 0; i < invoicequery.Count(); i++)
                            {
                                _invoiceid[i] = invoicequery[i].InvoiceId;
                                _invoicenumber[i] = invoicequery[i].InvoiceNumber;
                                _invoicevalue[i] = Convert.ToDecimal(invoicequery[i].TotalInvoiceValue);
                                _invoicedate[i] = String.Format("{0:ddd dd-MMM-yyyy}", invoicequery[i].InvoiceDate);
                                totalinvoicevalue += _invoicevalue[i];
                            }
                            model.PreviousInvoiceDate = _invoicedate;
                            model.PreviousInvoiceId = _invoiceid;
                            model.PreviousInvoiceNumber = _invoicenumber;
                            model.PreviousInvoicevalue = _invoicevalue;
                            model.AvailableBalance = model.Sanctionvalue - totalinvoicevalue;
                            if(model.AvailableBalance < 0)
                            {
                                model.AvailableBalance = 0;
                            }
                        }

                        var instalmentquery = (from I in context.tblInstallment
                                               where I.ProjectId == projectid
                                               select I).ToList();
                        if (instalmentquery.Count() > 0)
                        {
                            Nullable<int>[] _instalmentid = new Nullable<int>[instalmentquery.Count];
                            Nullable<int>[] _instalmentnumber = new Nullable<int>[instalmentquery.Count];
                            Nullable<int>[] _instalmentyear = new Nullable<int>[instalmentquery.Count];
                            Nullable<Decimal>[] _instalmentvalue = new Nullable<Decimal>[instalmentquery.Count];
                            string[] _isInvoiced = new string[instalmentquery.Count];
                            for (int i = 0; i < instalmentquery.Count(); i++)
                            {
                                _instalmentid[i] = instalmentquery[i].InstallmentID;
                                _instalmentnumber[i] = instalmentquery[i].InstallmentNo;
                                _instalmentyear[i] = instalmentquery[i].Year;
                                _isInvoiced[i] = "No";
                                if (query.P.IsYearWiseAllocation == true)
                                {
                                    _instalmentvalue[i] = Convert.ToDecimal(instalmentquery[i].InstallmentValue);
                                    for (int j = 0; j < invoicequery.Count(); j++)
                                    {
                                        if (invoicequery[j].InstalmentNumber == instalmentquery[i].InstallmentNo && invoicequery[j].InstalmentYear == instalmentquery[i].Year)
                                        {
                                            _isInvoiced[i] = "Yes";
                                        }
                                    }
                                }
                                if (query.P.IsYearWiseAllocation != true)
                                {
                                    _instalmentvalue[i] = Convert.ToDecimal(instalmentquery[i].InstallmentValue);
                                    for (int j = 0; j < invoicequery.Count(); j++)
                                    {
                                        if (invoicequery[j].InstalmentNumber == instalmentquery[i].InstallmentNo)
                                        {
                                            _isInvoiced[i] = "Yes";
                                        }
                                    }
                                }
                                //  totalinvoicevalue += _invoicevalue[i];
                            }
                            model.InstalmentId = _instalmentid;
                            model.InstlmntNumber = _instalmentnumber;
                            model.InstalValue = _instalmentvalue;
                            model.Instalmentyear = _instalmentyear;
                            model.Invoiced = _isInvoiced;
                        }
                        //if (query.P.ProjectType == 1)
                        //{
                        //    var projectcategory = Convert.ToInt32(query.P.SponProjectCategory);
                        //    var bankquery = (from Bank in context.tblIITMBankMaster
                        //                     join account in context.tblBankAccountMaster on Bank.BankId equals account.BankId
                        //                     join projectgroup in context.tblBankProjectGroup on account.BankAccountId equals projectgroup.BankAccountId
                        //                     join cc in context.tblCodeControl on
                        //                     new { pjctgroup = projectcategory, codeName = "SponBankProjectGroup" } equals
                        //                     new { pjctgroup = cc.CodeValAbbr, codeName = cc.CodeName }
                        //                     where projectgroup.ProjectGroup == projectcategory
                        //                     select new { Bank, account, projectgroup, cc }).FirstOrDefault();
                        //    if (bankquery != null)
                        //    {
                        //        model.Bank = bankquery.Bank.BankId;
                        //        model.BankName = bankquery.Bank.BankName;
                        //        model.BankAccountNumber = bankquery.account.AccountNumber;
                        //        model.BankAccountId = bankquery.account.BankAccountId;
                        //    }
                        //}
                        if (query.P.ProjectType == 2)
                        {
                            var projectcategory = Convert.ToInt32(query.P.ConsProjectSubType);
                            var balanceinstvalue = query.P.SanctionValue - totalinvoicevalue;
                            model.Instalmentvalue = balanceinstvalue;
                            model.Instalmentnumber = 1;
                            model.Instlmntyr = 1;
                            if (query.P.IsYearWiseAllocation == true)
                            {
                                DateTime startdate = DateTime.Now;
                                DateTime enddate = DateTime.Now;
                                DateTime today = DateTime.Now;

                                if (query.P.ActualStartDate == null)
                                {
                                    startdate = (DateTime)query.P.TentativeStartDate;
                                    enddate = (DateTime)query.P.TentativeCloseDate;
                                    TimeSpan diff_date = today - startdate;
                                    int noofdays = diff_date.Days;
                                    int years = noofdays / 365;
                                    int currentprojectyear = 0;
                                    if (years == 0)
                                    {
                                        currentprojectyear = 1;
                                    }
                                    if (years > 0)
                                    {
                                        currentprojectyear = years;
                                    }
                                    var instalment = (from ins in context.tblInstallment
                                                      where (ins.ProjectId == projectid && ins.Year == currentprojectyear)
                                                      select ins).FirstOrDefault();
                                    if (instalment != null)
                                    {
                                        model.Instalmentnumber = instalment.InstallmentNo;
                                        var balanceinstalment = instalment.InstallmentValueForYear - totalinvoicevalue;
                                        model.Instalmentvalue = balanceinstalment;
                                        model.Instlmntyr = instalment.Year;
                                    }
                                }
                                if (query.P.ActualStartDate != null)
                                {
                                    startdate = (DateTime)query.P.ActualStartDate;
                                    enddate = (DateTime)query.P.ActuaClosingDate;
                                    TimeSpan diff_date = today - startdate;
                                    int noofdays = diff_date.Days;
                                    int years = noofdays / 365;
                                    int currentprojectyear = 0;
                                    if (years == 0)
                                    {
                                        currentprojectyear = 1;
                                    }
                                    if (years > 0)
                                    {
                                        currentprojectyear = years;
                                    }

                                    var instalment = (from ins in context.tblInstallment
                                                      where (ins.ProjectId == projectid && ins.Year == currentprojectyear)
                                                      select ins).FirstOrDefault();
                                    if (instalment != null)
                                    {
                                        model.Instalmentnumber = instalment.InstallmentNo;
                                        var balanceinstalment = instalment.InstallmentValueForYear - totalinvoicevalue;
                                        model.Instalmentvalue = balanceinstalment;
                                        model.Instlmntyr = instalment.Year;
                                    }
                                }
                            }
                            if (query.P.IsYearWiseAllocation != true)
                            {
                                var instalment = (from ins in context.tblInstallment
                                                  where (ins.ProjectId == projectid)
                                                  select ins).FirstOrDefault();
                                if (instalment != null)
                                {
                                    var balanceinstalment = instalment.InstallmentValue - totalinvoicevalue;
                                    model.Instalmentvalue = balanceinstalment;
                                    model.Instalmentnumber = 1;
                                    model.Instlmntyr = 1;
                                }
                            }
                            //var bankquery = (from Bank in context.tblIITMBankMaster
                            //                 join account in context.tblBankAccountMaster on Bank.BankId equals account.BankId
                            //                 join projectgroup in context.tblBankProjectGroup on account.BankAccountId equals projectgroup.BankAccountId
                            //                 join cc in context.tblCodeControl on
                            //                 new { pjctgroup = projectcategory, codeName = "ConsBankProjectGroup" } equals
                            //                 new { pjctgroup = cc.CodeValAbbr, codeName = cc.CodeName }
                            //                 select new { Bank, account, projectgroup, cc }).FirstOrDefault();
                            //if (bankquery != null)
                            //{
                            //    model.Bank = bankquery.Bank.BankId;
                            //    model.BankName = bankquery.Bank.BankName;
                            //    model.BankAccountNumber = bankquery.account.AccountNumber;
                            //    model.BankAccountId = bankquery.account.BankAccountId;
                            //}
                        }
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<MasterlistviewModel> LoadProformaInvoiceList(int ProjectId)
        {
            try
            {
                List<MasterlistviewModel> Title = new List<MasterlistviewModel>();

                Title.Add(new MasterlistviewModel()
                {
                    id = null,
                    name = "Select Any"
                });
                using (var context = new IOASDBEntities())
                {
                    var query = (from I in context.tblProformaInvoiceDraft
                                 join P in context.tblProject on I.ProjectId equals P.ProjectId
                                 join U in context.vwFacultyStaffDetails on P.PIName equals U.UserId
                                 where (I.ProjectId == ProjectId && I.Status == "Draft")
                                 orderby I.InvoiceDraftId
                                 select new { U.FirstName, P, I }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Title.Add(new MasterlistviewModel()
                            {
                                id = query[i].I.InvoiceDraftId,
                                name = query[i].I.DescriptionofServices + "-" + query[i].FirstName,
                            });
                        }
                    }
                }

                return Title;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}