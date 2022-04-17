using FileExchanger.Configs;
using FileExchanger.Helpers;
using FileExchanger.Models;
using FileExchanger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Controllers
{
    [Authorize(Policy = "AuthExchanger")]
    [Route("api/files/e")]
    [ApiController]
    public class FilesExchangeController : ControllerBase
    {
        private UserModel getUser => db.Users.FirstOrDefault(p => p.Key == HttpContext.Request.Cookies["u_key"]);
        private UserInWorkingGroupModel getUserWorkingGroup
        {
            get
            {
                db.WorkingGroups.ToList();
                return db.UserInWorkingGroups.FirstOrDefault(p => p.User == getUser);
            }
        }
        private WorkingGroupModel getWorkingGroup
        {
            get
            {
                var uwg = getUserWorkingGroup;
                if (uwg == null)
                    return null;
                return uwg.WorkingGroup;
            }
        }
        private DbApp db;
        public FilesExchangeController(DbApp db)
        {
            this.db = db;
            Cleaner.ClearFiles(db);
        }
        [RequestSizeLimit(100L * 1024L * 1024L * 1024L)]
        [RequestFormLimits(MultipartBodyLengthLimit = 100L * 1024L * 1024L * 1024L)]
        [HttpPost("upload")]
        public IActionResult UploadFile(IFormFile file, FileAccessMode m, string p, double st, int dc)
        {
            var user = getUser;
            if (user == null)
                return NotFound("User not found!");

            if(getUser.MaxFileCount <= db.UserFiles.Count(p => p.User == getUser))
                return BadRequest("Too many uploaded files!");

            if(file.Length > getUser.MaxFileSize)
                return BadRequest($"Your file is too large (your limit {getUser.MaxFileSize.ByteSizeToString()})");

            if (m != FileAccessMode.Limited)
                p = null;
            var fileModel = new ExchangeFileModel()
            {
                Name = file.FileName,
                Size = file.Length,
                Key = "".RandomString(96),
                CreateDate = DateTime.Now,
                AccessMode = m,
                Password = p
            };

            if (st > user.MaxSaveFileTime)
                return BadRequest($"Incorrect save time! Your limit time = {user.MaxSaveFileTime}");
            fileModel.SaveTime = st;

            if (dc != -1)
            {
                if (dc > 5 && dc <= 0)
                    return BadRequest("Incorrect download count! Your limit count = 10");
            }
            fileModel.DownloadCount = 0;
            fileModel.MaxDownloadCount = dc;

            FtpService.Upload(file.OpenReadStream(), fileModel, DefaultService.FileExchanger);
            db.ExchangeFiles.Add(fileModel);
            db.UserFiles.Add(new UserFilesModel()
            {
                File = fileModel,
                User = user
            });
            db.SaveChanges();
            return Ok(fileModel);
        }

        [HttpGet("download/{key}/{name}")]
        public IActionResult Download(string key, string name, string p)
        {
            if (getUser == null)
                return NotFound("User not found!");
            var file = db.ExchangeFiles.FirstOrDefault(p => p.Key == key && p.Name == name);
            if (file == null)
                return NotFound("File not found!");
            var fileOwner = db.UserFiles.FirstOrDefault(p => p.File == file);
            if (!db.UserInWorkingGroups.Any(p => p.WorkingGroup == getWorkingGroup && p.User == fileOwner.User) 
                && db.UserFiles.Any(p => p.File == file && p.User != getUser))
                if (file.AccessMode == FileAccessMode.Limited && file.Password != p)
                    return Unauthorized("Incorrect password!");

            if (file.IsDeleteFile)
            {
                FtpService.DeleteFile(file, DefaultService.FileExchanger);
                FtpService.DeleteDir(file.Key, DefaultService.FileExchanger);

                db.UserFiles.Remove(db.UserFiles.FirstOrDefault(p => p.File == file && p.User == getUser));
                db.ExchangeFiles.Remove(file);
                db.SaveChanges();
                return NotFound("File not found!");
            }

            file.DownloadCount++;
            db.SaveChanges();
            return File(FtpService.Download(file, DefaultService.FileExchanger), "application/octet-stream", file.Name);
        }

        [HttpPost("delete/{key}/{name}")]
        public IActionResult Delete(string key, string name)
        {
            if (getUser == null)
                return NotFound("User not found!");
            var file = db.ExchangeFiles.FirstOrDefault(p => p.Key == key && p.Name == name);
            if (file == null)
                return NotFound("File not found!");
            db.Users.ToList();
            db.WorkingGroups.ToList();
            var fileOwner = db.UserFiles.FirstOrDefault(p => p.File == file);
            if (getUser != fileOwner.User)
            {
                var ownGroup = db.UserInWorkingGroups.FirstOrDefault(p => p.User == fileOwner.User);
                if (ownGroup == null || ownGroup.WorkingGroup != getWorkingGroup)
                    return Unauthorized("Access denied!");
            }

            FtpService.DeleteFile(file, DefaultService.FileExchanger);
            FtpService.DeleteDir(file.Key, DefaultService.FileExchanger);

            db.UserFiles.Remove(db.UserFiles.FirstOrDefault(p => p.File == file));
            db.ExchangeFiles.Remove(file);
            db.SaveChanges();
            return Ok();
        }

        [HttpGet("info/{key}/{name}")]
        public IActionResult Info(string key, string name)
        {
            if (getUser == null)
                return NotFound("User not found!");
            var file = db.ExchangeFiles.FirstOrDefault(p => p.Key == key && p.Name == name);
            if (file == null)
                return NotFound("File not found!");

            if (file.IsDeleteFile)
            {
                FtpService.DeleteFile(file, DefaultService.FileExchanger);
                FtpService.DeleteDir(file.Key, DefaultService.FileExchanger);

                db.UserFiles.Remove(db.UserFiles.FirstOrDefault(p => p.File == file && p.User == getUser));
                db.ExchangeFiles.Remove(file);
                db.SaveChanges();
                return NotFound("File not found!");
            }

            return Ok(new
            {
                name = file.Name,
                key = file.Key,
                access = file.AccessMode,
                size = file.Size,
                file.DownloadCount
            });
        }

        [HttpGet("check/pin/{key}/{name}")]
        public IActionResult CkeckPin(string key, string name, string p)
        {
            if (getUser == null)
                return NotFound("User not found!");
            var file = db.ExchangeFiles.FirstOrDefault(p => p.Key == key && p.Name == name);
            if (file == null)
                return NotFound("File not found!");
            if (file.AccessMode == FileAccessMode.Limited && file.Password != p)
                return Unauthorized("Incorrect password!");

            return Ok();
        }

        [HttpGet("check/limit")]
        public IActionResult CheckLimit(long fs)
        {
            if (getUser == null)
                return NotFound("User not found!");
            if (db.UserFiles.Count(p => p.User == getUser) >= getUser.MaxFileCount)
                return BadRequest($"Your limit {getUser.MaxFileCount} files!");
            if (fs >= getUser.MaxFileSize)
                return BadRequest($"Your file is too large (your limit {getUser.MaxFileSize.ByteSizeToString()})");
            return Ok();
        }

        [HttpGet("list")]
        public IActionResult List()
        {
            if(getUser == null)
                return NotFound("User not found!");
            db.ExchangeFiles.ToList();
            db.UserInWorkingGroups.ToList();
            var files = from f in db.UserFiles.Where(p => p.User == getUser).Select(p => p.File)
                        select new
                        {
                            f.Id,
                            f.Name,
                            f.Size,
                            f.AccessMode,
                            f.CreateDate,
                            f.DownloadCount,
                            f.Key,
                            f.MaxDownloadCount,
                            f.SaveTime
                        };
            var users = db.UserInWorkingGroups.Where(p => p.WorkingGroup == getWorkingGroup && p.User != getUser).Select(p => p.User).ToList();
            var res = files.ToList();
            foreach(var user in users)
            {
                var userFiles = from f in db.UserFiles.Where(p => p.User == user).Select(p => p.File)
                                select new
                                {
                                    f.Id,
                                    f.Name,
                                    f.Size,
                                    f.AccessMode,
                                    f.CreateDate,
                                    f.DownloadCount,
                                    f.Key,
                                    f.MaxDownloadCount,
                                    f.SaveTime
                                };
                res.AddRange(userFiles);
            }

            return Ok(res);
        }
    }
}
