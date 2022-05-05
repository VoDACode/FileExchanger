using Core.Context;
using Core.Models;
using FileExchanger.Configs;
using FileExchanger.Models;
using FileExchanger.Models.Ftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileExchanger.Services
{
    public class FtpService : FtpContext
    {
        private static FtpService instance;
        public static FtpService Instance => instance ?? (instance = new FtpService());
        protected override string Username => Config.Instance.Ftp.Username;
        protected override string Password => Config.Instance.Ftp.Password;
        protected override string[] SystemFolders => new string[]
        {
            $"{Config.Instance.Ftp.Path}exchange",
            $"{Config.Instance.Ftp.Path}storage"
        };
        private FtpService() { }

        #region fiels
        public void Upload(Stream file, IFile fileModel, DefaultService service)
        {
            mkdir(getPath(service, fileModel.Key));
            upload(file, getPath(service, fileModel));
        }

        public Stream Download(IFile file, DefaultService service)
        {
            return download(getPath(service, file));
        }

        public FtpStatusCode DeleteFile(IFile file, DefaultService service)
        {
            return deleteFile(getPath(service, file));
        }

        public FtpStatusCode DeleteDir(string dir, DefaultService service)
        {
            return deleteDir(getPath(service, dir));
        }
        #endregion

        #region Private
        private string getPath(DefaultService service, IFile file)
        {
            string path = service == DefaultService.FileStorage ? "storage" : "exchange";
            return $"{Config.Instance.Ftp.Path}{path}/{file.Key}/{file.Name}";
        }
        private string getPath(DefaultService service, string path)
        {
            string p = service == DefaultService.FileStorage ? "storage" : "exchange";
            return $"{Config.Instance.Ftp.Path}{p}/{path}";
        } 
        #endregion
    }
}
