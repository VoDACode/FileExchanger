using Core;
using Core.Enums;
using Core.Models;
using Core.Zip;
using FileExchanger.Helpers;
using FileExchanger.Interfaces;
using FileExchanger.Requests;
using FileExchanger.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Services
{
    public class DirectoryService : IDirectoryService
    {
        private readonly DbApp db;

        public DirectoryService(DbApp db)
        {
            this.db = db;
        }

        public async Task<DirectoryResponse> CreateDirectory(DirectoryEditRequest directoryRequest, int userId)
        {
            var user = await db.AuthClients.SingleOrDefaultAsync(p => p.Id == userId);
            if (user == null)
                return new DirectoryResponse()
                {
                    Success = false,
                    Error = "User not found.",
                    ErrorCode = "D_C4040"
                };
            var dir = await db.Directory.SingleOrDefaultAsync(p => p.Key == directoryRequest.Key);
            if (dir == null)
                return new DirectoryResponse()
                {
                    Success = false,
                    Error = "Directory not found.",
                    ErrorCode = "D_C4041"
                };
            if (dir.Owner != user)
                return new DirectoryResponse()
                {
                    Success = false,
                    Error = "Access is denied.",
                    ErrorCode = "D_C4010"
                };
            var item = new DirectoryModel()
            {
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Key = DirectoryHelper.GeneranionKey(dir, DateTime.Now),
                Name = directoryRequest.Name,
                Owner = user,
                Root = dir
            };
            await db.AddAsync(item);
            await db.SaveChangesAsync();
            return new DirectoryResponse()
            {
                Success = true,
                CreateDate = item.CreateDate,
                UpdateDate = item.UpdateDate,
                Key = item.Key,
                RootKey = dir.Key,
                OwnerId = userId,
                Name = item.Name
            };
        }

        public async Task<DirectoryResponse> DeleteDirectory(DirectoryRequest directoryRequest, int userId)
        {
            var user = await db.AuthClients.SingleOrDefaultAsync(p => p.Id == userId);
            if (user == null)
                return new DirectoryResponse()
                {
                    Success = false,
                    Error = "User not found.",
                    ErrorCode = "D_D4040"
                };
            var dir = await db.Directory.SingleOrDefaultAsync(p => p.Key == directoryRequest.Key);
            if (dir == null)
                return new DirectoryResponse()
                {
                    Success = false,
                    Error = "Directory not found.",
                    ErrorCode = "D_D4041"
                };
            if (dir.Owner != user || (dir.Name == "/" && dir.Root == default))
                return new DirectoryResponse()
                {
                    Success = false,
                    Error = "Access is denied.",
                    ErrorCode = "D_D4010"
                };

            await deleteDirectory(directoryRequest.Key);

            return new DirectoryResponse() { Success = true };

            async Task deleteDirectory(string key)
            {
                var dir_ = await db.Directory.SingleOrDefaultAsync(p => p.Key == key);
                var files = db.StorageFiles.Where(p => p.Directory == dir_).ToList();
                for (int i = 0; i < files.Count(); i++)
                {
                    db.StorageFiles.Remove(files[i]);
                    FtpService.Instance.DeleteFile(files[i], DefaultService.FileStorage);
                    FtpService.Instance.DeleteDir(files[i].Key, DefaultService.FileStorage);
                }
                var dirs = db.Directory.Where(p => p.Root == dir_).ToList();
                for (int i = 0; i < dirs.Count(); i++)
                {
                    await deleteDirectory(dirs[i].Key);
                }
                db.Directory.Remove(dir_);
                db.SaveChanges();
            }

        }

        public async Task<DirectoryDownloadResponse> DownloadDirectory(DirectoryRequest directoryRequest, int userId)
        {
            var user = await db.AuthClients.SingleOrDefaultAsync(p => p.Id == userId);
            if (user == null)
                return new DirectoryDownloadResponse()
                {
                    Success = false,
                    Error = "User not found.",
                    ErrorCode = "D_DOWNLOAD4040"
                };
            var dir = await db.Directory.SingleOrDefaultAsync(p => p.Key == directoryRequest.Key);
            if (dir == null)
                return new DirectoryDownloadResponse()
                {
                    Success = false,
                    Error = "Directory not found.",
                    ErrorCode = "D_DOWNLOAD4041"
                };
            if (dir.Owner != user)
                return new DirectoryDownloadResponse()
                {
                    Success = false,
                    Error = "Access is denied.",
                    ErrorCode = "D_DOWNLOAD4010"
                };
            var zip = new ZipService();
            var content = getContentInDir(dir.Key);
            await zip.Create();
            await zip.SetName(dir.Name);
            await zip.AddRage(content);
            await zip.Pack();
            return new DirectoryDownloadResponse()
            {
                Success = true,
                DownloadUrl = $"https://{Config.Instance.Services.ZipServer.Host}:{Config.Instance.Services.ZipServer.Port}/api/zip/download/{zip.Key}"
            };
        }

        public async Task<DirectoryResponse> GetInfo(DirectoryRequest directoryRequest, int userId)
        {
            var user = await db.AuthClients.SingleOrDefaultAsync(p => p.Id == userId);
            if (user == null)
                return new DirectoryResponse()
                {
                    Success = false,
                    Error = "User not found.",
                    ErrorCode = "D_GI4040"
                };
            var dir = await db.Directory.SingleOrDefaultAsync(p => p.Key == directoryRequest.Key);
            if (dir == null)
                return new DirectoryResponse()
                {
                    Success = false,
                    Error = "Directory not found.",
                    ErrorCode = "D_GI4040"
                };
            if(dir.Owner != user)
                return new DirectoryResponse()
                {
                    Success = false,
                    Error = "Access is denied.",
                    ErrorCode = "D_GI4010"
                };
            return new DirectoryResponse()
            {
                Success = true,
                CreateDate = dir.CreateDate,
                Key = dir.Key,
                UpdateDate = dir.UpdateDate,
                Name = dir.Name,
                OwnerId = dir.Owner.Id,
                RootKey = dir.Root?.Key
            };
        }

        public async Task<string> GetRootKey(int userId)
        {
            await db.AuthClients.AnyAsync(p => p.Id == userId);
            return (await db.Directory.SingleAsync(p => p.Owner.Id == userId && p.Name == "/" && p.Root == null)).Key;
        }

        public async Task<DirectoryResponse> RenameDirectory(DirectoryEditRequest directoryRequest, int userId)
        {
            var user = await db.AuthClients.SingleOrDefaultAsync(p => p.Id == userId);
            if (user == null)
                return new DirectoryResponse()
                {
                    Success = false,
                    Error = "User not found.",
                    ErrorCode = "D_R4040"
                };
            var dir = await db.Directory.SingleOrDefaultAsync(p => p.Key == directoryRequest.Key);
            if (dir == null)
                return new DirectoryResponse()
                {
                    Success = false,
                    Error = "Directory not found.",
                    ErrorCode = "D_R4041"
                };
            if (dir.Owner != user || (dir.Name == "/" && dir.Root == default))
                return new DirectoryResponse()
                {
                    Success = false,
                    Error = "Access is denied.",
                    ErrorCode = "D_R4010"
                };
            dir.Name = directoryRequest.Name;
            dir.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
            return new DirectoryResponse() { Success = true };
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
                        Key = folder.Key,
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
