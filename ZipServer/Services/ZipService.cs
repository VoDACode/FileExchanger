using Core.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ZipServer.Managers;

namespace ZipServer.Services
{
    public class ZipService : ZipContext
    {
        private ZipContent content => Storage.Instance.ZipStorage.SingleOrDefault(p => p.Key == Key).Value;
        public override async Task<ZipContext> AddRage(List<ZipItem> items)
        {
            foreach (var item in items)
                await Add(item);
            return this;
        }
        public override async Task<ZipContext> Add(ZipItem item)
        {
            item.Path = item.Path.Replace('/', Path.DirectorySeparatorChar);
            content.Content.Add(item);
            if(item.Path.Length > 0 && item.Path.Last() == Path.DirectorySeparatorChar)
                item.Path = item.Path.Substring(0, item.Path.Length - 1);
            if (item.Path.Length > 0 && item.Path.First() == Path.DirectorySeparatorChar)
                item.Path = item.Path.Substring(1, item.Path.Length - 1);
            var rootPath = Path.Combine(FileManager.Instance.TempPath, Key, (item.Path == Path.DirectorySeparatorChar.ToString() ? "" : item.Path));
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);
            if (item.Type == ContentType.Folder)
            {
                var path = Path.Combine(rootPath, item.Name);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            else if (item.Type == ContentType.File)
            {
                var ftpStream = FtpService.Instance.Download($"{Config.Instance.Ftp.Path}storage/{item.Key}/{item.Name}");
                using (var fileStream = File.Create(Path.Combine(rootPath, item.Name)))
                {
                    byte[] buffer = new byte[1024];
                    int size = 0;
                    while ((size = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, size);
                    }
                }
                ftpStream.Close();
            }
            return this;
        }

        public override async Task<ZipContext> Create()
        {
            string key = "";
            do
            {
                key = "".RandomString(128);
            } while (Storage.Instance.ZipStorage.ContainsKey(key));
            _key = key;
            Storage.Instance.ZipStorage.Add(key, new ZipContent());
            Directory.CreateDirectory(Path.Combine(FileManager.Instance.TempPath, key));
            return this;
        }

        public override FileStream GetStream()
        {
            return File.OpenRead(Path.Combine(FileManager.Instance.TempPath, $"{Key}_zip", "result.zip"));
        }

        public override async Task<ZipContext> Pack()
        {
            var resultPath = Path.Combine(FileManager.Instance.TempPath, $"{Key}_zip");
            Directory.CreateDirectory(resultPath);
            ZipFile.CreateFromDirectory(Path.Combine(FileManager.Instance.TempPath, Key), Path.Combine(resultPath, "result.zip"), CompressionLevel.Optimal, false);
            content.Done = true;
            Directory.Delete(Path.Combine(FileManager.Instance.TempPath, Key), true);
            return this;
        }
    }
}
