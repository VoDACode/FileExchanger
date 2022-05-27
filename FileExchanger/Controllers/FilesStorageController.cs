using Core;
using Core.Models;
using FileExchanger.Configs;
using FileExchanger.Helpers;
using FileExchanger.Models;
using FileExchanger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;

namespace FileExchanger.Controllers
{
    [Authorize(Policy = "AuthStorage")]
    [Route("api/files/s")]
    [ApiController]
    public class FilesStorageController : ControllerBase
    {
        private DbApp db;

        private AuthClientModel authClient => db.AuthClients.SingleOrDefault(p => p.Email == User.Identity.Name);
        private IMemoryCache cache;
        public FilesStorageController(DbApp db, IMemoryCache cache)
        {
            this.db = db;
            this.cache = cache;
            db.AuthClients.ToList();
        }

        [HttpGet("{dir}/{file}/info")]
        public IActionResult GetFileInfo(string dir, string file)
        {
            var fromDir = db.Directory.SingleOrDefault(p => p.Key == dir);
            if (fromDir == null || fromDir.Owner != authClient)
                return NotFound();
            var fileModel = db.StorageFiles.SingleOrDefault(p => p.Key == file);
            if (fileModel == null || fileModel.Owner != authClient)
                return NotFound();
            return Ok(new
            {
                key = file,
                dir = dir,
                createDate = fileModel.CreateDate,
                updateDate = fileModel.UpdateDate,
                name = fileModel.Name,
                size = fileModel.Size
            });
        }

        [HttpGet("{dir}/list")]
        public IActionResult GetFilesList(string dir, string mode)
        {
            var fromDir = db.Directory.SingleOrDefault(p => p.Key == dir);
            if (fromDir == null)
                return NotFound();
            var list = (from d in db.Directory
                        where d.Root == fromDir && d.Owner == authClient
                        select new
                        {
                            key = d.Key,
                            name = d.Name,
                            isHaveFolders = db.Directory.Any(p => p.Root == d),
                            isDir = true,
                            isFile = false,
                            createDate = d.CreateDate,
                            updateDate = d.UpdateDate
                        }).ToList();
            if (mode != "only_dir")
                list.AddRange((from f in db.StorageFiles
                               where f.Directory == fromDir && f.Owner == authClient
                               select new
                               {
                                   key = f.Key,
                                   name = f.Name,
                                   isHaveFolders = false,
                                   isDir = false,
                                   isFile = true,
                                   createDate = f.CreateDate,
                                   updateDate = f.UpdateDate
                               }).ToList());

            return Ok(list);
        }

        [HttpDelete("{dir}/{file}/delete")]
        public IActionResult DeleteFile(string dir, string file)
        {
            var fromDir = db.Directory.SingleOrDefault(p => p.Key == dir);
            if (fromDir == null || fromDir.Owner != authClient)
                return NotFound();
            var fileModel = db.StorageFiles.SingleOrDefault(p => p.Key == file);
            if (fileModel == null || fileModel.Owner != authClient)
                return NotFound();
            FtpService.Instance.DeleteFile(fileModel, DefaultService.FileStorage);
            FtpService.Instance.DeleteDir(fileModel.Key, DefaultService.FileStorage);
            db.StorageFiles.Remove(fileModel);
            db.SaveChanges();
            return Ok();
        }

        [RequestSizeLimit(100L * 1024L * 1024L * 1024L)]
        [RequestFormLimits(MultipartBodyLengthLimit = 100L * 1024L * 1024L * 1024L)]
        [HttpPost("{dir}/upload")]
        public IActionResult UploadFile(string dir, IFormFile file)
        {
            if (file == null)
                return BadRequest("file");
            if (file.Length > Config.Instance.Services.FileStorage.MaxUploadSize)
                return BadRequest("len");
            var fromDir = db.Directory.SingleOrDefault(p => p.Key == dir);
            if (fromDir == null || fromDir.Owner != authClient)
                return NotFound();
            var createDate = DateTime.Now;
            var fileModel = new StorageFileModel()
            {
                CreateDate = createDate,
                UpdateDate = createDate,
                Directory = fromDir,
                Key = FilesHelper.GeneranionKey(fromDir, createDate),
                Name = file.FileName,
                Owner = authClient,
                Size = file.Length
            };
            FtpService.Instance.Upload(file.OpenReadStream(), fileModel, DefaultService.FileStorage);
            db.StorageFiles.Add(fileModel);
            db.SaveChanges();
            return Ok(new
            {
                key = fileModel.Key,
                name = fileModel.Name
            });
        }

        [HttpGet("{dir}/{file}/get-disposable-key")]
        public IActionResult GetDisposableKey(string dir, string file)
        {
            var fromDir = db.Directory.SingleOrDefault(p => p.Key == dir);
            if (fromDir == null || fromDir.Owner != authClient)
                return NotFound();
            var fileModel = db.StorageFiles.SingleOrDefault(p => p.Key == file);
            if (fileModel == null || fileModel.Owner != authClient)
                return NotFound();
            string key = "".RandomString(256);
            cache.CreateEntry($"STRORAGE_FILE_DisposableKey_{key}");
            cache.Set($"STRORAGE_FILE_DisposableKey_{key}", $"{dir}_{file}", TimeSpan.FromMinutes(1));
            return Ok(key);
        }

        [AllowAnonymous]
        [HttpGet("download/{key}")]
        public IActionResult DownloadFile(string key)
        {
            if (!cache.TryGetValue($"STRORAGE_FILE_DisposableKey_{key}", out string data))
                return NotFound();
            cache.Remove($"STRORAGE_FILE_DisposableKey_{key}");
            string dir = "", file = "";
            {
                string[] tmp = data.Split('_');
                dir = tmp[0];
                file = tmp[1];
            }
            var fromDir = db.Directory.SingleOrDefault(p => p.Key == dir);
            if (fromDir == null)
                return NotFound();
            var fileModel = db.StorageFiles.SingleOrDefault(p => p.Key == file);
            if (fileModel == null)
                return NotFound();

            return File(FtpService.Instance.Download(fileModel, DefaultService.FileStorage), "application/octet-stream", fileModel.Name, true);
        }
        [HttpPost("{dir}/{file}/rename")]
        public IActionResult Rename(string dir, string file, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest();
            var fromDir = db.Directory.SingleOrDefault(p => p.Key == dir);
            if (fromDir == null || fromDir.Owner != authClient)
                return NotFound();
            var fileModel = db.StorageFiles.SingleOrDefault(p => p.Key == file);
            if (fileModel == null || fileModel.Owner != authClient)
                return NotFound();
            fileModel.Name = name;
            fileModel.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return Ok();
        }
    }
}