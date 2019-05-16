using IOAS.DataModel;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace IOAS.Infrastructure
{
    public class Common
    {
        public static string GetRolename(int RoleId)
        {

            using (var context = new IOASDBEntities())
            {
                var query = context.tblRole.FirstOrDefault(dup => dup.RoleId == RoleId);
                var userRoleName = "";
                if (query != null)
                {
                    userRoleName = query.RoleName;
                }
                context.Dispose();
                return userRoleName;
            }
        }
        public static int GetRoleId(string username)
        {

            var context = new IOASDBEntities();
            var query = (from U in context.tblUser
                         where (U.UserName == username && U.Status == "Active")
                         select U).FirstOrDefault();
            var userRoleId = 0;
            if (query != null)
            {
                userRoleId = (Int32)query.RoleId;
            }


            return userRoleId;

        }

        public static List<MenuListViewModel> Getaccessrole(int UserId)
        {
            List<MenuListViewModel> addmenu = new List<MenuListViewModel>();

            using (var context = new IOASDBEntities())
            {
                var addtionalroles = (from R in context.tblUserRole
                                      where R.UserId == UserId
                                      select R.RoleId).ToArray();
                var defaultrole = (from R in context.tblUser
                                   where (R.UserId == UserId)
                                   select R.RoleId).FirstOrDefault();

                var roles = (from RA in context.tblRoleaccess
                             from F in context.tblFunction
                             from M in context.tblModules
                             from MG in context.tblMenuGroup
                             where (addtionalroles.Contains(RA.RoleId) || RA.RoleId == defaultrole) && RA.FunctionId == F.FunctionId && F.ModuleID == M.ModuleID && F.MenuGroupID == MG.MenuGroupID
                             select new { F.FunctionId, F.FunctionName, F.ActionName, F.ControllerName, M.ModuleID, M.ModuleName, MG.MenuGroup, MG.MenuGroupID }).Distinct().ToList();


                //This get rolewise menucount
                if (roles.Count > 0)
                {
                    //This query using get modules in roles count using distinct
                    var module = roles.Select(m => m.ModuleName).Distinct().ToArray();
                    var menugroup = roles.Select(m => m.MenuGroup).Distinct().ToArray();


                    for (int m = 0; m < module.Length; m++)
                    {
                        string moduleName = module[m];
                        List<submodulemenuviewmodel> submodules = new List<submodulemenuviewmodel>();
                        for (int i = 0; i < menugroup.Length; i++)
                        {

                            string submodule = menugroup[i];
                            var menu = roles.Where(S => S.ModuleName == moduleName && S.MenuGroup == submodule).FirstOrDefault();
                            if (menu != null)
                            {

                                submodules.Add(new submodulemenuviewmodel()
                                {

                                    Menugroupname = submodule,
                                    Submenu = (from sm in roles
                                               where sm.ModuleName == moduleName && sm.MenuGroup == submodule
                                               orderby sm.FunctionName
                                               select new SubmenuViewModel()
                                               {
                                                   FunctionId = sm.FunctionId,
                                                   Functioname = sm.FunctionName,
                                                   Actionname = sm.ActionName,
                                                   Controllername = sm.ControllerName
                                               }).Distinct().ToList()
                                });
                            }
                        }
                        addmenu.Add(new MenuListViewModel()
                        {
                            Modulename = moduleName,
                            submodule = submodules
                        });

                    }
                }

                return addmenu;
            }
        }
        public static int GetUserid(string Username)
        {

            var context = new IOASDBEntities();
            var query = (from U in context.tblUser
                         where (U.UserName == Username)
                         select U).FirstOrDefault();
            var userId = 0;
            if (query != null)
            {
                userId = (Int32)query.UserId;
            }


            return userId;

        }
       
        public static List<MasterlistviewModel> GetUserlist(int Roleid)
        {
            try
            {
                List<MasterlistviewModel> User = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from U in context.tblUser
                                 where (U.RoleId == Roleid && U.Status == "Active")
                                 select new { U.UserId, U.UserName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            User.Add(new MasterlistviewModel()
                            {
                                id = query[i].UserId,
                                name = query[i].UserName
                            });
                        }
                    }

                }
                return User;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> User = new List<MasterlistviewModel>();
                return User;
            }
        }
        public static List<CountryListViewModel> getCountryList()
        {
            try
            {
                List<CountryListViewModel> country = new List<CountryListViewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCountries
                                 orderby C.countryName
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            country.Add(new CountryListViewModel()
                            {
                                CountryID = query[i].countryID,
                                CountryCode = query[i].countryCode,
                                CountryName = query[i].countryName
                            });
                        }
                    }


                }

                return country;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

       
        public static List<MasterlistviewModel> GetPIWithDetails()
        {
            try
            {

                List<MasterlistviewModel> PI = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.vwFacultyStaffDetails
                                     //join ins in context.tblInstituteMaster on C.InstituteId equals ins.InstituteId
                                     //where (C.RoleId == 7)
                                 orderby C.FirstName
                                 select new { C.UserId, C.FirstName, C.EmployeeId }).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            PI.Add(new MasterlistviewModel()
                            {
                                id = query[i].UserId,
                                name = query[i].EmployeeId + "-" + query[i].FirstName,
                            });
                        }
                    }


                }

                return PI;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static Tuple<Int32, Int32> getUserIdAndRole(string username)
        {
            try
            {
                int userId = 0, role = 0;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblUser.FirstOrDefault(dup => dup.UserName == username);
                    if (query != null)
                    {
                        userId = query.UserId;
                        role = query.RoleId ?? 0;
                    }
                }
                return Tuple.Create(userId, role);
            }
            catch (Exception ex)
            {
                int userId = 0, role = 0;
                return Tuple.Create(userId, role);
            }
        }

        private static IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
             #region Big freaking list of mime types
        // combination of values from Windows 7 Registry and 
        // from C:\Windows\System32\inetsrv\config\applicationHost.config
        // some added, including .7z and .dat
        {".323", "text/h323"},
        {".3g2", "video/3gpp2"},
        {".3gp", "video/3gpp"},
        {".3gp2", "video/3gpp2"},
        {".3gpp", "video/3gpp"},
        {".7z", "application/x-7z-compressed"},
        {".aa", "audio/audible"},
        {".AAC", "audio/aac"},
        {".aaf", "application/octet-stream"},
        {".aax", "audio/vnd.audible.aax"},
        {".ac3", "audio/ac3"},
        {".aca", "application/octet-stream"},
        {".accda", "application/msaccess.addin"},
        {".accdb", "application/msaccess"},
        {".accdc", "application/msaccess.cab"},
        {".accde", "application/msaccess"},
        {".accdr", "application/msaccess.runtime"},
        {".accdt", "application/msaccess"},
        {".accdw", "application/msaccess.webapplication"},
        {".accft", "application/msaccess.ftemplate"},
        {".acx", "application/internet-property-stream"},
        {".AddIn", "text/xml"},
        {".ade", "application/msaccess"},
        {".adobebridge", "application/x-bridge-url"},
        {".adp", "application/msaccess"},
        {".ADT", "audio/vnd.dlna.adts"},
        {".ADTS", "audio/aac"},
        {".afm", "application/octet-stream"},
        {".ai", "application/postscript"},
        {".aif", "audio/x-aiff"},
        {".aifc", "audio/aiff"},
        {".aiff", "audio/aiff"},
        {".air", "application/vnd.adobe.air-application-installer-package+zip"},
        {".amc", "application/x-mpeg"},
        {".application", "application/x-ms-application"},
        {".art", "image/x-jg"},
        {".asa", "application/xml"},
        {".asax", "application/xml"},
        {".ascx", "application/xml"},
        {".asd", "application/octet-stream"},
        {".asf", "video/x-ms-asf"},
        {".ashx", "application/xml"},
        {".asi", "application/octet-stream"},
        {".asm", "text/plain"},
        {".asmx", "application/xml"},
        {".aspx", "application/xml"},
        {".asr", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".atom", "application/atom+xml"},
        {".au", "audio/basic"},
        {".avi", "video/x-msvideo"},
        {".axs", "application/olescript"},
        {".bas", "text/plain"},
        {".bcpio", "application/x-bcpio"},
        {".bin", "application/octet-stream"},
        {".bmp", "image/bmp"},
        {".c", "text/plain"},
        {".cab", "application/octet-stream"},
        {".caf", "audio/x-caf"},
        {".calx", "application/vnd.ms-office.calx"},
        {".cat", "application/vnd.ms-pki.seccat"},
        {".cc", "text/plain"},
        {".cd", "text/plain"},
        {".cdda", "audio/aiff"},
        {".cdf", "application/x-cdf"},
        {".cer", "application/x-x509-ca-cert"},
        {".chm", "application/octet-stream"},
        {".class", "application/x-java-applet"},
        {".clp", "application/x-msclip"},
        {".cmx", "image/x-cmx"},
        {".cnf", "text/plain"},
        {".cod", "image/cis-cod"},
        {".config", "application/xml"},
        {".contact", "text/x-ms-contact"},
        {".coverage", "application/xml"},
        {".cpio", "application/x-cpio"},
        {".cpp", "text/plain"},
        {".crd", "application/x-mscardfile"},
        {".crl", "application/pkix-crl"},
        {".crt", "application/x-x509-ca-cert"},
        {".cs", "text/plain"},
        {".csdproj", "text/plain"},
        {".csh", "application/x-csh"},
        {".csproj", "text/plain"},
        {".css", "text/css"},
        {".csv", "text/csv"},
        {".cur", "application/octet-stream"},
        {".cxx", "text/plain"},
        {".dat", "application/octet-stream"},
        {".datasource", "application/xml"},
        {".dbproj", "text/plain"},
        {".dcr", "application/x-director"},
        {".def", "text/plain"},
        {".deploy", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dgml", "application/xml"},
        {".dib", "image/bmp"},
        {".dif", "video/x-dv"},
        {".dir", "application/x-director"},
        {".disco", "text/xml"},
        {".dll", "application/x-msdownload"},
        {".dll.config", "text/xml"},
        {".dlm", "text/dlm"},
        {".doc", "application/msword"},
        {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".dot", "application/msword"},
        {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
        {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
        {".dsp", "application/octet-stream"},
        {".dsw", "text/plain"},
        {".dtd", "text/xml"},
        {".dtsConfig", "text/xml"},
        {".dv", "video/x-dv"},
        {".dvi", "application/x-dvi"},
        {".dwf", "drawing/x-dwf"},
        {".dwp", "application/octet-stream"},
        {".dxr", "application/x-director"},
        {".eml", "message/rfc822"},
        {".emz", "application/octet-stream"},
        {".eot", "application/octet-stream"},
        {".eps", "application/postscript"},
        {".etl", "application/etl"},
        {".etx", "text/x-setext"},
        {".evy", "application/envoy"},
        {".exe", "application/octet-stream"},
        {".exe.config", "text/xml"},
        {".fdf", "application/vnd.fdf"},
        {".fif", "application/fractals"},
        {".filters", "Application/xml"},
        {".fla", "application/octet-stream"},
        {".flr", "x-world/x-vrml"},
        {".flv", "video/x-flv"},
        {".fsscript", "application/fsharp-script"},
        {".fsx", "application/fsharp-script"},
        {".generictest", "application/xml"},
        {".gif", "image/gif"},
        {".group", "text/x-ms-group"},
        {".gsm", "audio/x-gsm"},
        {".gtar", "application/x-gtar"},
        {".gz", "application/x-gzip"},
        {".h", "text/plain"},
        {".hdf", "application/x-hdf"},
        {".hdml", "text/x-hdml"},
        {".hhc", "application/x-oleobject"},
        {".hhk", "application/octet-stream"},
        {".hhp", "application/octet-stream"},
        {".hlp", "application/winhlp"},
        {".hpp", "text/plain"},
        {".hqx", "application/mac-binhex40"},
        {".hta", "application/hta"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".htt", "text/webviewhtml"},
        {".hxa", "application/xml"},
        {".hxc", "application/xml"},
        {".hxd", "application/octet-stream"},
        {".hxe", "application/xml"},
        {".hxf", "application/xml"},
        {".hxh", "application/octet-stream"},
        {".hxi", "application/octet-stream"},
        {".hxk", "application/xml"},
        {".hxq", "application/octet-stream"},
        {".hxr", "application/octet-stream"},
        {".hxs", "application/octet-stream"},
        {".hxt", "text/html"},
        {".hxv", "application/xml"},
        {".hxw", "application/octet-stream"},
        {".hxx", "text/plain"},
        {".i", "text/plain"},
        {".ico", "image/x-icon"},
        {".ics", "application/octet-stream"},
        {".idl", "text/plain"},
        {".ief", "image/ief"},
        {".iii", "application/x-iphone"},
        {".inc", "text/plain"},
        {".inf", "application/octet-stream"},
        {".inl", "text/plain"},
        {".ins", "application/x-internet-signup"},
        {".ipa", "application/x-itunes-ipa"},
        {".ipg", "application/x-itunes-ipg"},
        {".ipproj", "text/plain"},
        {".ipsw", "application/x-itunes-ipsw"},
        {".iqy", "text/x-ms-iqy"},
        {".isp", "application/x-internet-signup"},
        {".ite", "application/x-itunes-ite"},
        {".itlp", "application/x-itunes-itlp"},
        {".itms", "application/x-itunes-itms"},
        {".itpc", "application/x-itunes-itpc"},
        {".IVF", "video/x-ivf"},
        {".jar", "application/java-archive"},
        {".java", "application/octet-stream"},
        {".jck", "application/liquidmotion"},
        {".jcz", "application/liquidmotion"},
        {".jfif", "image/pjpeg"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpb", "application/octet-stream"},
        {".jpe", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".json", "application/json"},
        {".jsx", "text/jscript"},
        {".jsxbin", "text/plain"},
        {".latex", "application/x-latex"},
        {".library-ms", "application/windows-library+xml"},
        {".lit", "application/x-ms-reader"},
        {".loadtest", "application/xml"},
        {".lpk", "application/octet-stream"},
        {".lsf", "video/x-la-asf"},
        {".lst", "text/plain"},
        {".lsx", "video/x-la-asf"},
        {".lzh", "application/octet-stream"},
        {".m13", "application/x-msmediaview"},
        {".m14", "application/x-msmediaview"},
        {".m1v", "video/mpeg"},
        {".m2t", "video/vnd.dlna.mpeg-tts"},
        {".m2ts", "video/vnd.dlna.mpeg-tts"},
        {".m2v", "video/mpeg"},
        {".m3u", "audio/x-mpegurl"},
        {".m3u8", "audio/x-mpegurl"},
        {".m4a", "audio/m4a"},
        {".m4b", "audio/m4b"},
        {".m4p", "audio/m4p"},
        {".m4r", "audio/x-m4r"},
        {".m4v", "video/x-m4v"},
        {".mac", "image/x-macpaint"},
        {".mak", "text/plain"},
        {".man", "application/x-troff-man"},
        {".manifest", "application/x-ms-manifest"},
        {".map", "text/plain"},
        {".master", "application/xml"},
        {".mda", "application/msaccess"},
        {".mdb", "application/x-msaccess"},
        {".mde", "application/msaccess"},
        {".mdp", "application/octet-stream"},
        {".me", "application/x-troff-me"},
        {".mfp", "application/x-shockwave-flash"},
        {".mht", "message/rfc822"},
        {".mhtml", "message/rfc822"},
        {".mid", "audio/mid"},
        {".midi", "audio/mid"},
        {".mix", "application/octet-stream"},
        {".mk", "text/plain"},
        {".mmf", "application/x-smaf"},
        {".mno", "text/xml"},
        {".mny", "application/x-msmoney"},
        {".mod", "video/mpeg"},
        {".mov", "video/quicktime"},
        {".movie", "video/x-sgi-movie"},
        {".mp2", "video/mpeg"},
        {".mp2v", "video/mpeg"},
        {".mp3", "audio/mpeg"},
        {".mp4", "video/mp4"},
        {".mp4v", "video/mp4"},
        {".mpa", "video/mpeg"},
        {".mpe", "video/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpf", "application/vnd.ms-mediapackage"},
        {".mpg", "video/mpeg"},
        {".mpp", "application/vnd.ms-project"},
        {".mpv2", "video/mpeg"},
        {".mqv", "video/quicktime"},
        {".ms", "application/x-troff-ms"},
        {".msi", "application/octet-stream"},
        {".mso", "application/octet-stream"},
        {".mts", "video/vnd.dlna.mpeg-tts"},
        {".mtx", "application/xml"},
        {".mvb", "application/x-msmediaview"},
        {".mvc", "application/x-miva-compiled"},
        {".mxp", "application/x-mmxp"},
        {".nc", "application/x-netcdf"},
        {".nsc", "video/x-ms-asf"},
        {".nws", "message/rfc822"},
        {".ocx", "application/octet-stream"},
        {".oda", "application/oda"},
        {".odc", "text/x-ms-odc"},
        {".odh", "text/plain"},
        {".odl", "text/plain"},
        {".odp", "application/vnd.oasis.opendocument.presentation"},
        {".ods", "application/oleobject"},
        {".odt", "application/vnd.oasis.opendocument.text"},
        {".one", "application/onenote"},
        {".onea", "application/onenote"},
        {".onepkg", "application/onenote"},
        {".onetmp", "application/onenote"},
        {".onetoc", "application/onenote"},
        {".onetoc2", "application/onenote"},
        {".orderedtest", "application/xml"},
        {".osdx", "application/opensearchdescription+xml"},
        {".p10", "application/pkcs10"},
        {".p12", "application/x-pkcs12"},
        {".p7b", "application/x-pkcs7-certificates"},
        {".p7c", "application/pkcs7-mime"},
        {".p7m", "application/pkcs7-mime"},
        {".p7r", "application/x-pkcs7-certreqresp"},
        {".p7s", "application/pkcs7-signature"},
        {".pbm", "image/x-portable-bitmap"},
        {".pcast", "application/x-podcast"},
        {".pct", "image/pict"},
        {".pcx", "application/octet-stream"},
        {".pcz", "application/octet-stream"},
        {".pdf", "application/pdf"},
        {".pfb", "application/octet-stream"},
        {".pfm", "application/octet-stream"},
        {".pfx", "application/x-pkcs12"},
        {".pgm", "image/x-portable-graymap"},
        {".pic", "image/pict"},
        {".pict", "image/pict"},
        {".pkgdef", "text/plain"},
        {".pkgundef", "text/plain"},
        {".pko", "application/vnd.ms-pki.pko"},
        {".pls", "audio/scpls"},
        {".pma", "application/x-perfmon"},
        {".pmc", "application/x-perfmon"},
        {".pml", "application/x-perfmon"},
        {".pmr", "application/x-perfmon"},
        {".pmw", "application/x-perfmon"},
        {".png", "image/png"},
        {".pnm", "image/x-portable-anymap"},
        {".pnt", "image/x-macpaint"},
        {".pntg", "image/x-macpaint"},
        {".pnz", "image/png"},
        {".pot", "application/vnd.ms-powerpoint"},
        {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
        {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
        {".ppa", "application/vnd.ms-powerpoint"},
        {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
        {".ppm", "image/x-portable-pixmap"},
        {".pps", "application/vnd.ms-powerpoint"},
        {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
        {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
        {".ppt", "application/vnd.ms-powerpoint"},
        {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
        {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {".prf", "application/pics-rules"},
        {".prm", "application/octet-stream"},
        {".prx", "application/octet-stream"},
        {".ps", "application/postscript"},
        {".psc1", "application/PowerShell"},
        {".psd", "application/octet-stream"},
        {".psess", "application/xml"},
        {".psm", "application/octet-stream"},
        {".psp", "application/octet-stream"},
        {".pub", "application/x-mspublisher"},
        {".pwz", "application/vnd.ms-powerpoint"},
        {".qht", "text/x-html-insertion"},
        {".qhtm", "text/x-html-insertion"},
        {".qt", "video/quicktime"},
        {".qti", "image/x-quicktime"},
        {".qtif", "image/x-quicktime"},
        {".qtl", "application/x-quicktimeplayer"},
        {".qxd", "application/octet-stream"},
        {".ra", "audio/x-pn-realaudio"},
        {".ram", "audio/x-pn-realaudio"},
        {".rar", "application/octet-stream"},
        {".ras", "image/x-cmu-raster"},
        {".rat", "application/rat-file"},
        {".rc", "text/plain"},
        {".rc2", "text/plain"},
        {".rct", "text/plain"},
        {".rdlc", "application/xml"},
        {".resx", "application/xml"},
        {".rf", "image/vnd.rn-realflash"},
        {".rgb", "image/x-rgb"},
        {".rgs", "text/plain"},
        {".rm", "application/vnd.rn-realmedia"},
        {".rmi", "audio/mid"},
        {".rmp", "application/vnd.rn-rn_music_package"},
        {".roff", "application/x-troff"},
        {".rpm", "audio/x-pn-realaudio-plugin"},
        {".rqy", "text/x-ms-rqy"},
        {".rtf", "application/rtf"},
        {".rtx", "text/richtext"},
        {".ruleset", "application/xml"},
        {".s", "text/plain"},
        {".safariextz", "application/x-safari-safariextz"},
        {".scd", "application/x-msschedule"},
        {".sct", "text/scriptlet"},
        {".sd2", "audio/x-sd2"},
        {".sdp", "application/sdp"},
        {".sea", "application/octet-stream"},
        {".searchConnector-ms", "application/windows-search-connector+xml"},
        {".setpay", "application/set-payment-initiation"},
        {".setreg", "application/set-registration-initiation"},
        {".settings", "application/xml"},
        {".sgimb", "application/x-sgimb"},
        {".sgml", "text/sgml"},
        {".sh", "application/x-sh"},
        {".shar", "application/x-shar"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".sitemap", "application/xml"},
        {".skin", "application/xml"},
        {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
        {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
        {".slk", "application/vnd.ms-excel"},
        {".sln", "text/plain"},
        {".slupkg-ms", "application/x-ms-license"},
        {".smd", "audio/x-smd"},
        {".smi", "application/octet-stream"},
        {".smx", "audio/x-smd"},
        {".smz", "audio/x-smd"},
        {".snd", "audio/basic"},
        {".snippet", "application/xml"},
        {".snp", "application/octet-stream"},
        {".sol", "text/plain"},
        {".sor", "text/plain"},
        {".spc", "application/x-pkcs7-certificates"},
        {".spl", "application/futuresplash"},
        {".src", "application/x-wais-source"},
        {".srf", "text/plain"},
        {".SSISDeploymentManifest", "text/xml"},
        {".ssm", "application/streamingmedia"},
        {".sst", "application/vnd.ms-pki.certstore"},
        {".stl", "application/vnd.ms-pki.stl"},
        {".sv4cpio", "application/x-sv4cpio"},
        {".sv4crc", "application/x-sv4crc"},
        {".svc", "application/xml"},
        {".swf", "application/x-shockwave-flash"},
        {".t", "application/x-troff"},
        {".tar", "application/x-tar"},
        {".tcl", "application/x-tcl"},
        {".testrunconfig", "application/xml"},
        {".testsettings", "application/xml"},
        {".tex", "application/x-tex"},
        {".texi", "application/x-texinfo"},
        {".texinfo", "application/x-texinfo"},
        {".tgz", "application/x-compressed"},
        {".thmx", "application/vnd.ms-officetheme"},
        {".thn", "application/octet-stream"},
        {".tif", "image/tiff"},
        {".tiff", "image/tiff"},
        {".tlh", "text/plain"},
        {".tli", "text/plain"},
        {".toc", "application/octet-stream"},
        {".tr", "application/x-troff"},
        {".trm", "application/x-msterminal"},
        {".trx", "application/xml"},
        {".ts", "video/vnd.dlna.mpeg-tts"},
        {".tsv", "text/tab-separated-values"},
        {".ttf", "application/octet-stream"},
        {".tts", "video/vnd.dlna.mpeg-tts"},
        {".txt", "text/plain"},
        {".u32", "application/octet-stream"},
        {".uls", "text/iuls"},
        {".user", "text/plain"},
        {".ustar", "application/x-ustar"},
        {".vb", "text/plain"},
        {".vbdproj", "text/plain"},
        {".vbk", "video/mpeg"},
        {".vbproj", "text/plain"},
        {".vbs", "text/vbscript"},
        {".vcf", "text/x-vcard"},
        {".vcproj", "Application/xml"},
        {".vcs", "text/plain"},
        {".vcxproj", "Application/xml"},
        {".vddproj", "text/plain"},
        {".vdp", "text/plain"},
        {".vdproj", "text/plain"},
        {".vdx", "application/vnd.ms-visio.viewer"},
        {".vml", "text/xml"},
        {".vscontent", "application/xml"},
        {".vsct", "text/xml"},
        {".vsd", "application/vnd.visio"},
        {".vsi", "application/ms-vsi"},
        {".vsix", "application/vsix"},
        {".vsixlangpack", "text/xml"},
        {".vsixmanifest", "text/xml"},
        {".vsmdi", "application/xml"},
        {".vspscc", "text/plain"},
        {".vss", "application/vnd.visio"},
        {".vsscc", "text/plain"},
        {".vssettings", "text/xml"},
        {".vssscc", "text/plain"},
        {".vst", "application/vnd.visio"},
        {".vstemplate", "text/xml"},
        {".vsto", "application/x-ms-vsto"},
        {".vsw", "application/vnd.visio"},
        {".vsx", "application/vnd.visio"},
        {".vtx", "application/vnd.visio"},
        {".wav", "audio/wav"},
        {".wave", "audio/wav"},
        {".wax", "audio/x-ms-wax"},
        {".wbk", "application/msword"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wcm", "application/vnd.ms-works"},
        {".wdb", "application/vnd.ms-works"},
        {".wdp", "image/vnd.ms-photo"},
        {".webarchive", "application/x-safari-webarchive"},
        {".webtest", "application/xml"},
        {".wiq", "application/xml"},
        {".wiz", "application/msword"},
        {".wks", "application/vnd.ms-works"},
        {".WLMP", "application/wlmoviemaker"},
        {".wlpginstall", "application/x-wlpg-detect"},
        {".wlpginstall3", "application/x-wlpg3-detect"},
        {".wm", "video/x-ms-wm"},
        {".wma", "audio/x-ms-wma"},
        {".wmd", "application/x-ms-wmd"},
        {".wmf", "application/x-msmetafile"},
        {".wml", "text/vnd.wap.wml"},
        {".wmlc", "application/vnd.wap.wmlc"},
        {".wmls", "text/vnd.wap.wmlscript"},
        {".wmlsc", "application/vnd.wap.wmlscriptc"},
        {".wmp", "video/x-ms-wmp"},
        {".wmv", "video/x-ms-wmv"},
        {".wmx", "video/x-ms-wmx"},
        {".wmz", "application/x-ms-wmz"},
        {".wpl", "application/vnd.ms-wpl"},
        {".wps", "application/vnd.ms-works"},
        {".wri", "application/x-mswrite"},
        {".wrl", "x-world/x-vrml"},
        {".wrz", "x-world/x-vrml"},
        {".wsc", "text/scriptlet"},
        {".wsdl", "text/xml"},
        {".wvx", "video/x-ms-wvx"},
        {".x", "application/directx"},
        {".xaf", "x-world/x-vrml"},
        {".xaml", "application/xaml+xml"},
        {".xap", "application/x-silverlight-app"},
        {".xbap", "application/x-ms-xbap"},
        {".xbm", "image/x-xbitmap"},
        {".xdr", "text/plain"},
        {".xht", "application/xhtml+xml"},
        {".xhtml", "application/xhtml+xml"},
        {".xla", "application/vnd.ms-excel"},
        {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
        {".xlc", "application/vnd.ms-excel"},
        {".xld", "application/vnd.ms-excel"},
        {".xlk", "application/vnd.ms-excel"},
        {".xll", "application/vnd.ms-excel"},
        {".xlm", "application/vnd.ms-excel"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
        {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".xlt", "application/vnd.ms-excel"},
        {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
        {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
        {".xlw", "application/vnd.ms-excel"},
        {".xml", "text/xml"},
        {".xmta", "application/xml"},
        {".xof", "x-world/x-vrml"},
        {".XOML", "text/plain"},
        {".xpm", "image/x-xpixmap"},
        {".xps", "application/vnd.ms-xpsdocument"},
        {".xrm-ms", "text/xml"},
        {".xsc", "application/xml"},
        {".xsd", "text/xml"},
        {".xsf", "text/xml"},
        {".xsl", "text/xml"},
        {".xslt", "text/xml"},
        {".xsn", "application/octet-stream"},
        {".xss", "application/xml"},
        {".xtp", "application/octet-stream"},
        {".xwd", "image/x-xwindowdump"},
        {".z", "application/x-compress"},
        {".zip", "application/x-zip-compressed"},
        #endregion

        };
        public static string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string mime;

            return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }

        public static string getPIusernamebyname(int PIName)
        {

            var context = new IOASDBEntities();
            var query = (from User in context.tblUser
                         where (User.UserId == PIName)
                         select User).FirstOrDefault();
            var username = "";

            if (query != null)
            {
                username = query.UserName;
            }

            return username;

        }
        
        public static string GetPIName(int Id)
        {
            try
            {
                string PIName = "";
                using (var context = new IOASDBEntities())
                {
                    var Query = context.tblUser.FirstOrDefault(m => m.UserId == Id);
                    if (Query != null)
                    {
                        PIName = Query.FirstName + ' ' + Query.LastName;
                    }
                }
                return PIName;
            }
            catch (Exception ex)
            {

                return "";
            }
        }

        public static List<MasterlistviewModel> GetDepartment()
        {
            try
            {
                List<MasterlistviewModel> Department = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblDepartment orderby C.DepartmentId ascending select C).ToList();
                    if (Query.Count > 0)
                    {
                        for (int i = 0; i < Query.Count; i++)
                        {
                            Department.Add(new MasterlistviewModel()
                            {
                                id = Query[i].DepartmentId,
                                name = Query[i].DepartmentName
                            });
                        }
                    }
                    return Department;
                }
            }
            catch (Exception)
            {
                List<MasterlistviewModel> Department = new List<MasterlistviewModel>();
                return Department;
            }
        }

        public static List<MasterlistviewModel> GetUserList()
        {
            try
            {
                List<MasterlistviewModel> UserList = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblUser orderby C.UserId ascending select C).ToList();
                    if (Query.Count > 0)
                    {
                        for (int i = 0; i < Query.Count; i++)
                        {
                            UserList.Add(new MasterlistviewModel()
                            {
                                id = Query[i].UserId,
                                name = Query[i].FirstName + ' ' + Query[i].LastName
                            });
                        }
                    }
                    return UserList;
                }
            }
            catch (Exception)
            {
                List<MasterlistviewModel> UserList = new List<MasterlistviewModel>();
                return UserList;
            }
        }
        public static List<MasterlistviewModel> GetUserListByDepId(int ID)
        {
            try
            {
                List<MasterlistviewModel> UserList = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblUser where C.DepartmentId == ID orderby C.UserId ascending select C).ToList();
                    if (Query.Count > 0)
                    {
                        for (int i = 0; i < Query.Count; i++)
                        {
                            UserList.Add(new MasterlistviewModel()
                            {
                                id = Query[i].UserId,
                                name = Query[i].FirstName + ' ' + Query[i].LastName
                            });
                        }
                    }
                    return UserList;
                }
            }
            catch (Exception)
            {
                List<MasterlistviewModel> UserList = new List<MasterlistviewModel>();
                return UserList;
            }
        }

        public static string GetDepartmentById(int Depid)
        {
            try
            {
                string Department = "";
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblDepartment where C.DepartmentId == Depid select C.DepartmentName).FirstOrDefault();
                    if (Query != null)
                    {

                        Department = Query;

                    }
                    return Department;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string GetUserListById(int UserId)
        {
            try
            {
                string User = "";
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblUser where C.UserId == UserId select C).FirstOrDefault();
                    if (Query != null)
                    {
                        User = Query.FirstName + ' ' + Query.LastName;
                    }
                    return User;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }        
        public static List<MasterlistviewModel> GetInstitute()
        {
            try
            {
                List<MasterlistviewModel> inusmodel = new List<MasterlistviewModel>();
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = (from I in context.tblInstituteMaster
                                     orderby I.Institutename
                                     select new { I.InstituteId, I.Institutename }).ToList();
                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                inusmodel.Add(new MasterlistviewModel()
                                {
                                    id = query[i].InstituteId,
                                    name = query[i].Institutename
                                });
                            }

                        }
                        return inusmodel;
                    }

                }
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> inusmodel = new List<MasterlistviewModel>();
                return inusmodel;
            }
        }

        public static List<MasterlistviewModel> GetPIByInstitute(int InstituteId)
        {
            try
            {
                List<MasterlistviewModel> Catagory = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var Query = (from user in context.tblUser
                                 where user.InstituteId == InstituteId && user.Status == "Active"
                                 orderby user.FirstName, user.LastName
                                 select new { user.UserId, user.FirstName, user.LastName, user.EMPCode }).ToList();
                    Catagory.Add(new MasterlistviewModel()
                    {
                        id = null,
                        name = "Select PI"
                    });
                    if (Query.Count > 0)
                    {
                        for (int i = 0; i < Query.Count; i++)
                        {
                            Catagory.Add(new MasterlistviewModel()
                            {
                                id = Query[i].UserId,
                                name = Query[i].EMPCode + "-" + Query[i].FirstName + " " + Query[i].LastName
                            });
                        }
                    }
                    return Catagory;
                }
            }
            catch (Exception)
            {
                List<MasterlistviewModel> name = new List<MasterlistviewModel>();
                return name;
            }
        }
        public static int Gcd(int a, int b)
        {
            if (a == 0)
                return b;
            else
                return Gcd(b % a, a);
        }
        public static string GetRatio(int a, int b)
        {
            int gcd = Gcd(a, b);
            return (a / gcd).ToString() + ":" + (b / gcd).ToString();
        }

        public static List<MasterlistviewModel> Getinstitute()
        {
            try
            {
                List<MasterlistviewModel> inusmodel = new List<MasterlistviewModel>();
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = (from I in context.tblInstituteMaster
                                     orderby I.Institutename
                                     where (I.Status == "Active")
                                     select new { I.InstituteId, I.Institutename }).ToList();
                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                inusmodel.Add(new MasterlistviewModel()
                                {
                                    id = query[i].InstituteId,
                                    name = query[i].Institutename
                                });
                            }

                        }
                        return inusmodel;
                    }

                }
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> inusmodel = new List<MasterlistviewModel>();
                return inusmodel;
            }
        }

        public static List<MasterlistviewModel> Getdesignation()
        {
            try
            {
                List<MasterlistviewModel> designation = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from D in context.tblCodeControl
                                 where (D.CodeName.Contains("FacultyCadre"))
                                 orderby D.CodeValDetail
                                 select new { D.CodeValAbbr, D.CodeValDetail }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            designation.Add(new MasterlistviewModel()
                            {

                                id = query[i].CodeValAbbr,
                                name = query[i].CodeValDetail

                            });
                        }
                    }
                }
                return designation;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> designation = new List<MasterlistviewModel>();
                return designation;
            }
        }
                
        public static string getfacultycode(int PIid)
        {
            try
            {
                var facultycode = " ";
                var context = new IOASDBEntities();
                var query = (from user in context.vwFacultyStaffDetails
                             where user.UserId == PIid
                             select user).FirstOrDefault();

                if (query != null)
                {
                    facultycode = query.EmployeeId;
                    return facultycode;
                }
                else
                {
                    return facultycode;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }

        public static string GetDefaultRoleName(int UserId)
        {
            try
            {
                string _roles = string.Empty;
                using (var context = new IOASDBEntities())
                {
                    var query = (from U in context.tblUser
                                 from R in context.tblRole
                                 where (U.UserId == UserId && U.RoleId == R.RoleId)
                                 select R.RoleName).FirstOrDefault();
                    if (query != null)
                    {
                        _roles = query;
                    }

                }
                return _roles;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string GetRoles(int UserId)
        {
            try
            {
                string _roles = string.Empty;
                using (var context = new IOASDBEntities())
                {
                    var query = (from ur in context.tblUserRole
                                 join r in context.tblRole on ur.RoleId equals r.RoleId
                                 where ur.UserId == UserId
                                 select r.RoleName).ToArray();
                    var defaultRole = GetDefaultRoleName(UserId);
                    if (query.Count() > 0)
                    {
                        _roles = string.Join(",", query);
                        _roles = "," + defaultRole;
                    }
                    else
                    {
                        return defaultRole;
                    }

                }
                return _roles;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string GetUserFirstName(string Username)
        {
            try
            {
                string _fName = string.Empty;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblUser.FirstOrDefault(m => m.UserName == Username && m.Status == "Active");
                    if (query != null)
                    {
                        _fName = query.FirstName + " " + query.LastName;
                    }

                }
                return _fName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string GetUserFirstName(int UserId)
        {
            try
            {
                string _fName = string.Empty;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblUser.FirstOrDefault(m => m.UserId == UserId);
                    if (query != null)
                    {
                        _fName = query.FirstName + " " + query.LastName;
                    }

                }
                return _fName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string GetLoginTS(int UserId)
        {
            try
            {
                string _ts = string.Empty;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblLoginDetails.Where(m => m.UserId == UserId).OrderByDescending(m => m.LoginDetailId).FirstOrDefault();
                    if (query != null)
                    {
                        _ts = String.Format("{0:ddd dd-MM-yy h:mm tt}", query.LoginTime);
                    }

                }
                return _ts;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static List<NotificationModel> GetNotification(int logged_in_userId, int user_Role)
        {
            try
            {
                List<NotificationModel> list = new List<NotificationModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from notify in context.tblNotification
                                 join user in context.tblUser on notify.FromUserId equals user.UserId
                                 where notify.ToUserId == logged_in_userId && notify.IsDeleted != true
                                 orderby notify.NotificationId descending
                                 select new { notify, user.FirstName, user.LastName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new NotificationModel()
                            {
                                FromUserName = query[i].FirstName + " " + query[i].LastName,
                                FunctionURL = query[i].notify.FunctionURL,
                                NotificationDateTime = String.Format("{0:ddd dd-MM-yy h:mm tt}", query[i].notify.Crt_Ts),
                                NotificationId = query[i].notify.NotificationId,
                                NotificationType = query[i].notify.NotificationType,
                                ReferenceId = query[i].notify.ReferenceId,
                                Userrole = user_Role,
                            });
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<NotificationModel>();
            }

        }
        public static List<MasterlistviewModel> GetProjecttitledetails()
        {
            try
            {

                List<MasterlistviewModel> Projectdetails = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblProject
                                 join user in context.tblUser on C.PIName equals user.UserId                                 
                                 select new { C, user.FirstName, user.LastName, user.EMPCode }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Projectdetails.Add(new MasterlistviewModel()
                            {
                                id = query[i].C.ProjectId,
                                name = query[i].C.ProjectNumber + "-" + query[i].C.ProjectTitle + "-" + query[i].FirstName + " " + query[i].LastName,
                            });
                        }
                    }
                }

                return Projectdetails;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> Projectdetails = new List<MasterlistviewModel>();
                return Projectdetails;
            }

        }
        public static List<MasterlistviewModel> GetPIProjectdetails(int PIId)
        {
            try
            {

                List<MasterlistviewModel> Projectdetails = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblProject
                                 join user in context.tblUser on C.PIName equals user.UserId
                                 where C.PIName == PIId
                                 select new { C,  }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Projectdetails.Add(new MasterlistviewModel()
                            {
                                id = query[i].C.ProjectId,
                                name = query[i].C.ProjectNumber + "-" + query[i].C.ProjectTitle
                            });
                        }
                    }
                }

                return Projectdetails;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> Projectdetails = new List<MasterlistviewModel>();
                return Projectdetails;
            }
        }
        public static List<CodeControllistviewModel> getprojecttype()
        {
            try
            {

                List<CodeControllistviewModel> Projecttype = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "Projecttype"
                                 orderby C.CodeValAbbr
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Projecttype.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return Projecttype;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<MasterlistviewModel> getservicetype()
        {
            try
            {
                List<MasterlistviewModel> taxtype = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblTaxMaster
                                 orderby C.TaxMasterId
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            taxtype.Add(new MasterlistviewModel()
                            {
                                id = query[i].TaxMasterId,
                                name = query[i].ServiceType
                            });
                        }
                    }
                }
                return taxtype;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<CodeControllistviewModel> getinvoicetype()
        {
            try
            {

                List<CodeControllistviewModel> Invoicetype = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "InvoiceType"
                                 orderby C.CodeValAbbr
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Invoicetype.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return Invoicetype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        //public static string getInvoiceId()
        //{
        //    try
        //    {
        //       // var lastinvoiceid = 0;
        //        var seqnum = 0;
        //        var context = new IOASDBEntities();
        //        var Invoicequery = (from invoice in context.tblProjectInvoice
        //                             orderby invoice.InvoiceId descending
        //                             select invoice.InvoiceNumber).FirstOrDefault();

        //        if (Invoicequery != null)
        //        {
        //            //var invoiceid = Invoicequery.InvoiceId;
        //            //lastinvoiceid = invoiceid + 1;
        //            //seqnum = lastinvoiceid.ToString("D6");
        //            //return seqnum;
        //            var value = Invoicequery.Split('/').Last();
        //            string number = Regex.Replace(value, @"\D", "");
        //            seqnum = Convert.ToInt32(number);
        //            seqnum += 1;
        //            return seqnum.ToString("000000");
        //        }
        //        else
        //        {
        //            return seqnum.ToString();
        //        }

        //    }

        //    catch (Exception ex)
        //    {

        //        throw ex;

        //    }
        //}
        public static string GetCurrentFinYear()
        {
            try
            {
                var year = "";
                using (var context = new IOASDBEntities())
                {
                    var financialYear = context.tblFinYear.FirstOrDefault(m => m.CurrentYearFlag == true);
                    if (financialYear != null)
                        year = financialYear.Year;
                }

                return year;
            }

            catch (Exception ex)
            {
                return "";
            }
        }
        public static string getInvoiceId()
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblProjectInvoice
                               orderby b.InvoiceId descending
                               select b.InvoiceNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('C').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        no = "C" + GetCurrentFinYear() + "C" + seqnum.ToString("000000");
                        return no;
                    }
                    else
                    {
                        no = "C" + GetCurrentFinYear() + "C" + "000001";
                        return no;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string getProformaInvoiceId()
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblProformaInvoice
                               orderby b.InvoiceId descending
                               select b.InvoiceNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('P').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        no = "C" + GetCurrentFinYear() + "P" + seqnum.ToString("000000");
                        return no;
                    }
                    else
                    {
                        no = "C" + GetCurrentFinYear() + "P" + "000001";
                        return no;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string getinvoicenumber(int invoiceid)
        {
            try
            {
                var invoicenumber = " ";
                var context = new IOASDBEntities();
                var query = (from inv in context.tblProjectInvoice
                             where inv.InvoiceId == invoiceid
                             select inv).FirstOrDefault();

                if (query != null)
                {
                    invoicenumber = query.InvoiceNumber;
                    return invoicenumber;
                }
                else
                {
                    return invoicenumber;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static List<MasterlistviewModel> GetInvoicedetails()
        {
            try
            {

                List<MasterlistviewModel> Invoicedetails = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblProjectInvoiceDraft
                                 join P in context.tblProject on C.ProjectId equals P.ProjectId
                                 join user in context.vwFacultyStaffDetails on P.PIName equals user.UserId
                                 select new { C, P, user.FirstName, user.EmployeeId }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Invoicedetails.Add(new MasterlistviewModel()
                            {
                                id = query[i].C.InvoiceDraftId,
                                name = query[i].C.InvoiceDate + "-" + query[i].C.DescriptionofServices + "-" + query[i].FirstName,
                            });
                        }
                    }
                }

                return Invoicedetails;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> Invoicedetails = new List<MasterlistviewModel>();
                return Invoicedetails;
            }

        }
        public static List<MasterlistviewModel> GetProformaInvoicedetails()
        {
            try
            {

                List<MasterlistviewModel> Invoicedetails = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblProformaInvoiceDraft
                                 join P in context.tblProject on C.ProjectId equals P.ProjectId
                                 join user in context.tblUser on P.PIName equals user.UserId
                                 select new { C, P, user.FirstName, user.LastName, user.EMPCode }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Invoicedetails.Add(new MasterlistviewModel()
                            {
                                id = query[i].C.InvoiceDraftId,
                                name = query[i].C.InvoiceDate + "-" + query[i].C.DescriptionofServices + "-" + query[i].FirstName + " " + query[i].LastName,
                            });
                        }
                    }
                }

                return Invoicedetails;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> Invoicedetails = new List<MasterlistviewModel>();
                return Invoicedetails;
            }

        }
        public static string getproformainvoicenumber(int invoiceid)
        {
            try
            {
                var invoicenumber = " ";
                var context = new IOASDBEntities();
                var query = (from inv in context.tblProformaInvoice
                             where inv.InvoiceId == invoiceid
                             select inv).FirstOrDefault();

                if (query != null)
                {
                    invoicenumber = query.InvoiceNumber;
                    return invoicenumber;
                }
                else
                {
                    return invoicenumber;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        
    }
}