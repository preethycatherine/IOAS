using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace IOAS.FTP
{
    public class Ftpservice
    {
        static bool CreateDirectory(string ftproot, string username, string password, string foldername)
        {
            WebRequest request = WebRequest.Create(string.Format("{0}/{1}", ftproot.Replace(" ", ""), foldername.Replace(" ", "")));
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(username, password);
            try
            {
                using (var resp = (FtpWebResponse)request.GetResponse())
                {
                    //Console.WriteLine(resp.StatusCode);
                }
                return CheckDirectory(ftproot, username, password, foldername);

            }
            catch (WebException ex)
            {
                return false;
            }

        }
        static bool CheckDirectory(string ftproot, string username, string password, string foldername)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format("{1}/{0}/testFile.txt", foldername, ftproot));
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(username, password);

            var fileBytes = System.IO.File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/App_Data/testFile.txt"));
            //file.InputStream.Read(fileBytes, 0, fileBytes.Length);
            // return fileBytes;
            // byte[] fileBytes = File.ReadAllBytes(fileName);
            // Copy the contents of the file to the request stream.
            //byte[] fileContents;
            //using (StreamReader sourceStream = new StreamReader(file.InputStream))
            //{
            //    fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            //}

            //request.ContentLength = fileContents.Length;

            //using (Stream requestStream = request.GetRequestStream())
            //{
            //    requestStream.Write(fileContents, 0, fileContents.Length);
            //}

            //using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            //{
            //    Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            //}
            request.ContentLength = fileBytes.Length;
            request.UsePassive = true;
            request.UseBinary = true;
            request.ServicePoint.ConnectionLimit = fileBytes.Length;
            request.EnableSsl = false;
            try
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                    requestStream.Close();
                    return true;
                }
            }
            catch (WebException ex)
            {

                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }
                return false;
            }
            return true;
        }

         static void FindAndCreate(string path)
        {
            string rootdir = "ftp://10.18.0.29/ICSRIS%20DOCUMENT";
            //var ftp = FtpDetails();
           // rootdir = ftp.rootDirectory;
            string[] patharr = path.Split('/');
            if (patharr.Length > 0)
            {
                foreach (var folder in patharr)
                {
                    if (!CheckDirectory(rootdir, "icsris", "IcsR@123#", folder))
                    {
                        CreateDirectory(rootdir, "icsris", "IcsR@123#", folder);
                    }
                    rootdir = rootdir + "/" + folder;
                }
            }
        }
        public static bool UploadFilen(string type, string filepath, string insuranceCode, string filename=null)
        {
            //var rec = FtpDetails();

            //var path = db.Database.SqlQuery<string>("select replace(filePath,'{fn}'" + string.Format(", '{0}')from FtpManager where fileType='{1}'", fileno, type)).SingleOrDefault();
            var path = string.Format(@"/TravelInsurance/{0}",insuranceCode);
                string path1 = path.TrimStart('/');
                path1 = path1.TrimEnd('/');
               
                if (!CheckDirectory("ftp://10.18.0.29/ICSRIS%20DOCUMENT", "icsris", "IcsR@123#", path1))
                {
                    FindAndCreate(path);
                }

                if (string.IsNullOrEmpty(filename))
                    return UploadFile("ftp://10.18.0.29/ICSRIS%20DOCUMENT" + path.TrimEnd('/'), "icsris", "IcsR@123#", filepath);
                else
                    return UploadFile("ftp://10.18.0.29/ICSRIS%20DOCUMENT" + path.TrimEnd('/'), "icsris", "IcsR@123#", filename, filepath);
            
        }

        static bool UploadFile(string ftproot, string username, string password, string filepath)
        {
            var rec = filepath.Split('/');
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format("{0}/{1}", ftproot, rec[rec.Length - 1]));
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = new NetworkCredential(username, password);

            var fileBytes = System.IO.File.ReadAllBytes(filepath);
            //file.InputStream.Read(fileBytes, 0, fileBytes.Length);

            request.ContentLength = fileBytes.Length;
            request.UsePassive = true;
            request.UseBinary = true;
            request.ServicePoint.ConnectionLimit = fileBytes.Length;
            request.EnableSsl = false;
            try
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                    requestStream.Close();
                    return true;
                }
            }
            catch (WebException ex)
            {
                //message = "file Not Uploaded";
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    //Does not exist
                }
                return false;
            }

            return true;
        }
        static bool UploadFile(string ftproot, string username, string password, string filename, string filepath)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format("{0}/{1}", ftproot.Replace(" ", ""), filename));
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = new NetworkCredential(username, password);

            var fileBytes = System.IO.File.ReadAllBytes(filepath);
            //file.InputStream.Read(fileBytes, 0, fileBytes.Length);

            request.ContentLength = fileBytes.Length;
            request.UsePassive = true;
            request.UseBinary = true;
            request.ServicePoint.ConnectionLimit = fileBytes.Length;
            request.EnableSsl = false;
            try
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                    requestStream.Close();
                    return true;
                }
            }
            catch (WebException ex)
            {
                // message = "file Not Uploaded";
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    //Does not exist
                }
                return false;
            }

            return true;
        }
    }
}