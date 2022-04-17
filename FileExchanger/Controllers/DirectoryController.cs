using FileExchanger.Models;
using FileExchanger.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

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
            db.StorageFiles.RemoveRange(db.StorageFiles.Where(p => p.Directory == fromDir));
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
    }
}
