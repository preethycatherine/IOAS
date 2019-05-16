using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Linq;
using System.IO;
namespace IOAS.Models
{
    public class RoleModel
    {
        
        public int Roleid { get; set; }
        [Required]
        [Display(Name = "Role name")]
        public string Rolename { get; set; }
        public int Departmentroleid { get; set; }
        [Required]
        [Display(Name = "Department name")]
        public int Departmentid { get; set; }
        public int sno { get; set; }
        public string Departmentname { get; set; }
        public string Createduser { get; set; }
    }
    public class DepartmentModel
    {
        public int Sno { get; set; }
        public int Departmentid { get; set; }
        [Required]
        [Display(Name = "Department name")]
        public string Departmentname { get; set; }
        [Required]
        [Display(Name = "HOD")]
        public string HOD { get; set; }
        public string Createduser { get; set; }

    }
    public class Functionviewmodel
    {
        [Required]
        [Display(Name = "Select Any one Function")]
        public int Functionid { get; set; }
        public string Rolename { get; set; }
        public int sno { get; set; }
        public int Roleid { get; set; }
        [Required]
        [Display(Name = "Select Any one Department")]
        public int Departmentid { get; set; }

        public bool Read { get; set; }
        public bool Add { get; set; }
        public bool Delete { get; set; }
        public bool Approve { get; set; }
        public bool Update { get; set; }
    }
    public class Functionlistmodel
    {
        public int Functionid { get; set; }
        public string Functionname { get; set; }
    }
    public class RegisterModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string Firstname { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string Lastname { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address / Username")]
        [Display(Name = "User name / Email")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        public Nullable<int> Department { get; set; }

        [Required]
        public Nullable<int> RoleId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public Nullable<int> Gender { get; set; }
        [Required]
        [Display(Name = "Date Of brith")]

        public DateTime Dateofbirth { get; set; }
        public string DateOfbrith { get; set; }
        public int[] SelectedRoles { get; set; }
        [Required]
        [Display(Name = "Institute")]
        public int InstituteId { get; set; }
        public string[] SelectedRolesName { get; set; }
        public int UserId { get; set; }
        [Required]
        [Display(Name = "User type")]
        public int UsertypeId { get; set; }

        public HttpPostedFileBase UserImage { get; set; }
        public string Image { get; set; }
        [Required]
        [Display(Name = "Employee code")]
        public string EMPCode { get; set; }
        [Required]
        [Display(Name = "Designation")]
        public Nullable<int> Designation { get; set; }
        public string Createuser { get; set; }
    }
    public class UserResultModels
    {
        public int Sno { get; set; }
        public Nullable<int> Userid { get; set; }
        [Display(Name = "First Name")]
        public string Firstname { get; set; }


        [Display(Name = "Last name")]
        public string Lastname { get; set; }


        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address / Username")]
        [Display(Name = "User name / Email")]
        public string Username { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }


        public Nullable<int> Department { get; set; }


        public Nullable<int> RoleId { get; set; }


        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Display(Name = "Gender")]
        public Nullable<int> Gender { get; set; }

        [Display(Name = "Date Of brith")]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> Dateofbirth { get; set; }

        public int[] SelectedRoles { get; set; }



        public int Addtionaldepartment { get; set; }
        public string RoleName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Image { get; set; }
        public string Usertype { get; set; }
    }
    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class Gendermodel
    {
        public int GenderId { get; set; }
        public string Gendername { get; set; }
    }
    public class ForgotPasswordModel
    {
        [Required]
        [Display(Name = "User name")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email ID")]
        public string Email { get; set; }

        public string password { get; set; }


    }
    public class ResetPassword
    {
        [Required]
        [Display(Name = "Select Role name")]
        public int Roleid { get; set; }
        public string Rolename { get; set; }
        [Required]
        [Display(Name = "Select User name")]
        public int Userid { get; set; }
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class CreateInstituteModel
    {
        public int Sno { get; set; }
        public int InstituteId { get; set; }
        public string Countryname { get; set; }
        [Required]
        [Display(Name = "Institutename")]
        public string Institutename { get; set; }

        [Required]
        [Display(Name = "Contact first name")]
        [StringLength(250)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Contact last name")]
        [StringLength(150)]
        public string lastName { get; set; }

        [Required]
        [Display(Name = "Contact designation")]
        [StringLength(250)]
        public string contactDES { get; set; }

        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email address")]
        public string Email { get; set; }
        public string Location { get; set; }

        [Required]
        [Display(Name = "Address line1")]
        public string Address1 { get; set; }

        [Required]
        [Display(Name = "Address line2")]
        public string Address2 { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "state")]
        public string State { get; set; }


        public CountryListViewModel Country { get; set; }

        [Required]
        [Display(Name = "Country")]
        public Nullable<int> selCountry { get; set; }

        [Display(Name = "Post code / Zip")]
        [StringLength(25)]
        public string zipCode { get; set; }
        
        [StringLength(25)]
        public string ContactMobile { get; set; }

        [Display(Name = "Institute logo :")]
        public HttpPostedFileBase logoURL { get; set; }
        public string logo { get; set; }
        
        public string InstituteCode { get; set; }
    }
    public class UserlistModel
    {
        public int Userid { get; set; }
        public string Username { get; set; }
    }
    public class NotificationModel
    {
        public Int32 NotificationId { get; set; }
        public int Userrole { get; set; }
        public Int32 ReferenceId { get; set; }
        public string NotificationType { get; set; }
        public string FunctionURL { get; set; }
        public string NotificationDateTime { get; set; }
        public string FromUserName { get; set; }
    }
    public class AgencyModel
    {
        public int AgencyId { get; set; }
        [Required]
        [Display(Name = "Agency name")]
        public string AgencyName { get; set; }
        [Required]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        [Required]
        [Display(Name = "Contact Number")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact number must be Number")]
        public string ContactNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email address")]
        public string ContactEmail { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "state")]
        public string State { get; set; }
        [Required]
        [Display(Name = "country")]
        public int  Country { get; set; }
        [Required]
        [Display(Name = "Agency code")]
        [StringLength(200)]
        public string AgencyCode { get; set; }
        [Required]
        [Display(Name = "Agency type")]
        public int AgencyType { get; set; }
        [Required]
        [Display(Name = "Scheme")]
        public int Scheme { get; set; }
        public int UserId { get; set; }
        public int sno { get; set; }
        [MaxLength(100)]
        public string GSTIN { get; set; }
        [MaxLength(100)]
        public string TAN { get; set; }
        [MaxLength(10)]
        public string PAN { get; set; }
     }
    public class Allocationheadmodel
    {
        public int AllocationheadId { get; set; }
        [Required]
        [Display(Name ="Allocation head")]
        public string Allocationhead { get; set; }
        public int userid { get; set; }
        public int sno { get; set; }
    }
    public class Projectstaffcategorymodel
    {
        public int ProjectstaffcategoryId { get; set; }
        [Required]
        [Display(Name = "Project staff category")]
        public string ProjectstaffCategory { get; set; }
        public int userid { get; set; }
        public int sno { get; set; }
    }
    public class ConsultancyFundingcategorymodel
    {
        public int ConsultancyFundingcategoryid { get; set; }
        [Required]
        [Display(Name = "Consultancy Funding Category")]
        public string ConsultancyFundingcategory { get; set; }
        public int userid { get; set; }
        public int sno { get; set; }
    }
    public class Schemeviewmodel
    {
        public int SchemeId { get; set; }
        [Required]
        [Display(Name = "Scheme Name")]
        public string SchemeName { get; set; }
        [Required]
        [Display(Name = "Project type")]
        public int ProjectType { get; set; }
        [Required]
        [Display(Name = "Sheme code")]
        public string Schemecode { get; set; }
        public int sno { get; set; }
        public int userId { get; set; }
    }
    public class Accountgroupmodel
    {
        public int AccountGroupId { get; set; }
        [MaxLength(50)]
        [Required]

        [Display(Name = "Account group")]
        public string AccountGroup { get; set; }
        public int AccountType { get; set; }
        public string Accounttypename { get; set; }
        [Required]
        [Display(Name = "Account group")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Accountgroupcode must be Number")]
        public int AccountGroupCode { get; set; }
        public int userid { get; set; }
        public int sno { get; set; }
    }
    public class SRBItemcategory
    {
        public int sno { get; set; }
        public int SRBItemCategotyId { get; set; }
        public string Category { get; set; }
        public bool Asset_f { get; set; }
        public int userid { get; set; }
    }
}
