using Core.Context;
using Core.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ZipServer.Services
{
    public class FtpService : FtpContext
    {
        private static FtpService instance;
        public static FtpService Instance => instance ?? (instance = new FtpService());
        protected override string Username => Config.Instance.Ftp.Username;
        protected override string Password => Config.Instance.Ftp.Password;
        private FtpService() { }

        public Stream Download(string file) => download(file);
        public void Upload(Stream stream, string filePath) => upload(stream, filePath);
        public void Upload(byte[] data, string filePath) => upload(data, filePath);
    }
}
