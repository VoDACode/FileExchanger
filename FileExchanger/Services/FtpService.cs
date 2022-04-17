using FileExchanger.Configs;
using FileExchanger.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FileExchanger.Services
{
    public static class FtpService
    {
        static FtpService()
        {
            try
            {
                mkdir($"{Config.Instance.Ftp.Path}/exchange");
            }
            catch { }
            try
            {
                mkdir($"{Config.Instance.Ftp.Path}/storage");
            }
            catch { }
        }
        private static string getPath(DefaultService service, IFile file)
        {
            string path = service == DefaultService.FileStorage ? "storage" : "exchange";
            return $"{Config.Instance.Ftp.Path}{path}/{file.Key}/{file.Name}";
        }
        private static string getPath(DefaultService service, string path)
        {
            string p = service == DefaultService.FileStorage ? "storage" : "exchange";
            return $"{Config.Instance.Ftp.Path}{p}/{path}";
        }
        public static void Upload(Stream file, IFile fileModel, DefaultService service)
        {
            mkdir(getPath( service,fileModel.Key));
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(getPath(service, fileModel));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(Config.Instance.Ftp.Username, Config.Instance.Ftp.Password);

            request.ContentLength = file.Length;

            Stream ftpStream = request.GetRequestStream();
            file.CopyTo(ftpStream);
            ftpStream.Close();
        }

        public static Stream Download(IFile file, DefaultService service)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(getPath(service, file));
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            request.Credentials = new NetworkCredential(Config.Instance.Ftp.Username, Config.Instance.Ftp.Password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            return response.GetResponseStream();
        }

        public static FtpStatusCode DeleteFile(IFile file, DefaultService service)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(getPath(service, file));
                request.Method = WebRequestMethods.Ftp.DeleteFile;

                request.Credentials = new NetworkCredential(Config.Instance.Ftp.Username, Config.Instance.Ftp.Password);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                var code = response.StatusCode;
                response.Close();

                return code;
            }
            catch
            {
                return FtpStatusCode.Undefined;
            }
        }

        public static FtpStatusCode DeleteDir(string dir, DefaultService service)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(getPath(service, dir));
                request.Method = WebRequestMethods.Ftp.RemoveDirectory;

                request.Credentials = new NetworkCredential(Config.Instance.Ftp.Username, Config.Instance.Ftp.Password);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                var code = response.StatusCode;
                response.Close();
                return code;
            }
            catch
            {
                return FtpStatusCode.Undefined;
            }
}

        private static FtpStatusCode mkdir(string dir)
        {
            WebRequest request = WebRequest.Create(dir);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(Config.Instance.Ftp.Username, Config.Instance.Ftp.Password);
            using (var resp = (FtpWebResponse)request.GetResponse())
            {
                return resp.StatusCode;
            }
        }
    }
}
