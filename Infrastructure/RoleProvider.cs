using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Infrastructure
{
    public class RoleProvider
    {
        public static string[] Get(string controller, string action)
        {
            // get your roles based on the controller and the action name 
            return new string[] { "Office Admin", "PI", "IOAS Admin", "Office DA","Facility Admin","Facility DA" };
        }
    }
}