using FileExchanger.Models;
using FileExchanger.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using FileExchanger.Services;
using System.Collections.Generic;
using Core;
using Core.Models;
using Core.Zip;

namespace FileExchanger.Controllers
{
    [Authorize(Policy = "AuthStorage")]
    [Route("api/dir/s")]
    [ApiController]
    public class DirectoryController : ControllerBase
    {
        DbApp db;
        AuthClientModel authClient => db.AuthClients.SingleOrDefault(p => p.Email == User.Identity.Name);
        public DirectoryController(DbApp db)
        {
            this.db = db;
            db.AuthClients.ToList();
        }

        [HttpGet("get-root")]
        public IActionResult GetRootKey()
        {
            return Ok(db.Directory.Single(p => p.Owner == authClient && p.Name == "/" && p.Root == null).Key);
        }

        [HttpGet("{dir}/info")]
        public IActionResult GetInfo(string dir)
        {
            var fromDir = db.Directory.SingleOrDefault(p => p.Key == dir);
            if (fromDir == null || fromDir.Owner != authClient)
                return NotFound();
            return Ok(new
            {
                key = fromDir.Key,
                name = fromDir.Name,
                createDate = fromDir.CreateDate,
                updateDate = fromDir.UpdateDate
            });
        }

        [HttpPost("{dir}/create")]
        public IActionResult CreateDir(string dir, string name)
        {
            var fromDir = db.Directory.SingleOrDefault(p => p.Key == dir && p.Owner == authClient);
            if (fromDir == null || fromDir.Owner != authClient)
                return BadRequest();
            var createTime = DateTime.Now;
            var item = db.Directory.Add(new DirectoryModel()
            {
                CreateDate = createTime,
                UpdateDate = createTime,
                Name = name,
                Key = DirectoryHelper.GeneranionKey(fromDir, createTime),
                Owner = authClient,
                Root = fromDir
            }).Entity;
            db.SaveChanges();
            return Ok(new
            {
                Key = item.Key,
                Name = item.Name
            });
        }

        [HttpDelete("{dir}/delete")]
        public IActionResult DeleteDir(string dir)
        {
            var fromDir = db.Directory.SingleOrDefault(p => p.Key == dir);
            if (fromDir == null || fromDir.Owner != authClient)
                return BadRequest();
            var files = db.StorageFiles.Where(p => p.Directory == fromDir).ToList();
            for(int i = 0; i < files.Count(); i++)
            {
                db.StorageFiles.Remove(files[i]);
                FtpService.Instance.DeleteFile(files[i], Configs.DefaultService.FileStorage);
                FtpService.Instance.DeleteDir(files[i].Key, Configs.DefaultService.FileStorage);
            }
            var dirs = db.Directory.Where(p => p.Root == fromDir).ToList();
            for (int i = 0; i < dirs.Count(); i++)
            {
                DeleteDir(dirs[i].Key);
            }
            db.Directory.Remove(fromDir);
            db.SaveChanges();
            return Ok();
        }

        [HttpPost("{dir}/rename")]
        public IActionResult Rename(string dir, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest();
            var fromDir = db.Directory.SingleOrDefault(p => p.Key == dir);
            if (fromDir == null || fromDir.Owner != authClient)
                return NotFound();
            fromDir.Name = name;
            fromDir.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return Ok();
        }

        [HttpGet("{dir}/download")]
        public IActionResult Download(string dir)
        {
            var fromDir = db.Directory.SingleOrDefault(p => p.Key == dir);
            if (fromDir == null || fromDir.Owner != authClient)
                return NotFound();
            var zip = new ZipService();
            var content = getContentInDir(dir);
            zip.Create().Wait();
            zip.SetName(fromDir.Name).Wait();
            zip.AddRage(content).Wait();
            zip.Pack().Wait();
            return Ok($"https://{Config.Instance.Services.ZipServer.Host}:{Config.Instance.Services.ZipServer.Port}/api/zip/download/{zip.Key}");
        }

        private List<ZipItem> getContentInDir(string dirKey)
        {
            List<ZipItem> result = new List<ZipItem>();
            void fun(string d, string path)
            {
                var dir = db.Directory.SingleOrDefault(p => p.Key == d);
                if (dir == null)
                    return;
                var files = db.StorageFiles.Where(p => p.Directory == dir).ToList();
                var dirs = db.Directory.Where(p => p.Root == dir).ToList();
                foreach (var file in files)
                {
                    result.Add(new ZipItem()
                    {
                        Key = file.Key,
                        Size = file.Size,
                        Name = file.Name,
                        Type = ContentType.File,
                        Path = path
                    });
                }
                foreach (var folder in dirs)
                {
                    result.Add(new ZipItem()
                    {
                        Key=folder.Key,
                        Name = folder.Name,
                        Size = 0,
                        Path = path,
                        Type = ContentType.Folder
                    });
                    fun(folder.Key, $"{path}{folder.Name}/");
                }
            }
            fun(dirKey, "/");
            return result;
        }
    }
}
