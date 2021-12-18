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

        public static void Upload(Stream file, FileModel fileModel)
        {
            mkdir(fileModel.DownloadKey);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{Config.FtpPath}{fileModel.DownloadKey}/{fileModel.Name}");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(Config.FtpUsername, Config.FtpPassword);

            request.ContentLength = file.Length;

            Stream ftpStream = request.GetRequestStream();
            file.CopyTo(ftpStream);
            ftpStream.Close();
        }

        public static Stream Download(FileModel file)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{Config.FtpPath}/{file.DownloadKey}/{file.Name}");
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            request.Credentials = new NetworkCredential(Config.FtpUsername, Config.FtpPassword);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            return response.GetResponseStream();
        }

        public static FtpStatusCode DeleteFile(FileModel file)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{Config.FtpPath}/{file.DownloadKey}/{file.Name}");
                request.Method = WebRequestMethods.Ftp.DeleteFile;

                request.Credentials = new NetworkCredential(Config.FtpUsername, Config.FtpPassword);
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

        public static FtpStatusCode DeleteDir(string dir)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{Config.FtpPath}/{dir}");
                request.Method = WebRequestMethods.Ftp.RemoveDirectory;

                request.Credentials = new NetworkCredential(Config.FtpUsername, Config.FtpPassword);
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
            WebRequest request = WebRequest.Create($"{Config.FtpPath}/{dir}");
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(Config.FtpUsername, Config.FtpPassword);
            using (var resp = (FtpWebResponse)request.GetResponse())
            {
                return resp.StatusCode;
            }
        }
    }
}
