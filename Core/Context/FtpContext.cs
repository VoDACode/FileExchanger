using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Core.Context
{
    public abstract class FtpContext
    {
        protected abstract string Username { get; }
        protected abstract string Password { get; }
        protected virtual string[] SystemFolders { get; } = new string[0];

        public FtpContext()
        {
            foreach (var dir in SystemFolders)
                mkdir(dir);
        }

        protected long getFileSize(string path)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            request.Credentials = new NetworkCredential(Username, Password);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            return response.ContentLength;
        }
        protected FtpStatusCode deleteDir(string dir)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(dir);
                request.Method = WebRequestMethods.Ftp.RemoveDirectory;

                request.Credentials = new NetworkCredential(Username, Password);
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
        protected FtpStatusCode deleteFile(string path)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
                request.Method = WebRequestMethods.Ftp.DeleteFile;

                request.Credentials = new NetworkCredential(Username, Password);
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
        protected FtpStatusCode mkdir(string dir)
        {
            WebRequest request = WebRequest.Create(dir);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                request.Credentials = new NetworkCredential(Username, Password);
                var response = (FtpWebResponse)request.GetResponse();
                return response.StatusCode;
            }
            catch (WebException ex)
            {
                return FtpStatusCode.ActionNotTakenFileUnavailable;
            }
        }
        protected void upload(byte[] buffer, string filePath)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(filePath);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(Username, Password);

            request.ContentLength = buffer.Length;

            Stream ftpStream = request.GetRequestStream();
            ftpStream.Write(buffer, 0, buffer.Length);
            ftpStream.Close();
        }
        protected void upload(Stream stream, string filePath)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(filePath);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(Username, Password);

            request.ContentLength = stream.Length;

            Stream ftpStream = request.GetRequestStream();
            stream.CopyTo(ftpStream);
            ftpStream.Close();
        }
        protected Stream download(string file)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(file);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            request.Credentials = new NetworkCredential(Username, Password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            return response.GetResponseStream();
        }
    }
}
