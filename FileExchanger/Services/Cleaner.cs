using FileExchanger.Configs;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileExchanger.Services
{
    public static class Cleaner
    {
        public static void ClearUsers(DbApp db)
        {
            long nT = DateTime.Now.Ticks;
            var users = db.Users.ToList().Where(p => p.LastActive.Ticks + TimeSpan.FromDays(30).Ticks <= nT);
            db.ExchangeFiles.ToList();
            foreach (var user in users)
            {
                var userFiles = db.UserFiles.Where(p => p.User == user);
                var files = userFiles.Select(p => p.File);
                db.UserFiles.RemoveRange(userFiles);
                foreach (var file in files)
                {
                    FtpService.DeleteFile(file, DefaultService.FileExchanger);
                    FtpService.DeleteDir(file.Key, DefaultService.FileExchanger);
                }
                db.ExchangeFiles.RemoveRange(files);
                db.SaveChanges();
                db.WorkingGroups.ToList();
                var userGroups = db.UserInWorkingGroups.Where(p => p.User == user).ToList();
                foreach (var group in userGroups)
                {
                    if (group.IsCreator)
                    {
                        var firstUser = db.UserInWorkingGroups.FirstOrDefault(p => p == group && p.User != user);
                        if (firstUser != null)
                        {
                            firstUser.IsCreator = true;
                            firstUser.IsPermissionAddUser = true;
                        }
                        else
                        {
                            db.UserInWorkingGroups.Remove(group);
                            db.WorkingGroups.Remove(group.WorkingGroup);
                        }
                    }
                    else
                    {
                        db.UserInWorkingGroups.Remove(group);
                    }
                }
            }
            db.Users.RemoveRange(users);
            db.SaveChanges();
        }

        public static void ClearFiles(DbApp db)
        {
            var files = db.ExchangeFiles.Where(p => p.CreateDate.AddSeconds(p.SaveTime) <= DateTime.Now || (p.MaxDownloadCount != -1 && p.DownloadCount >= p.MaxDownloadCount));
            if (files == null)
                return;
            var userFiles = db.UserFiles.Where(p => p.File.CreateDate.AddSeconds(p.File.SaveTime) <= DateTime.Now || (p.File.MaxDownloadCount != -1 && p.File.DownloadCount >= p.File.MaxDownloadCount));
            db.UserFiles.RemoveRange(userFiles);
            db.SaveChanges();

            foreach (var file in files)
            {
                FtpService.DeleteFile(file, DefaultService.FileExchanger);
                FtpService.DeleteDir(file.Key, DefaultService.FileExchanger);
            }

            db.ExchangeFiles.RemoveRange(files);
            db.SaveChanges();
        }

    }
}
