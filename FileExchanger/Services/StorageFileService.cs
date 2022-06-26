using Core;
using Core.Enums;
using Core.Models;
using FileExchanger.Helpers;
using FileExchanger.Interfaces;
using FileExchanger.Requests;
using FileExchanger.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Services
{
    public class StorageFileService : IStorageFileService
    {
        private readonly DbApp db;
        private readonly IMemoryCache cache;
        public StorageFileService(DbApp db, IMemoryCache cache)
        {
            this.db = db;
            this.cache = cache;
            db.AuthClients.ToList();
        }

        public async Task<FileInfoResponse> DeleteFile(FileInfoRequest request, int userID)
        {
            var user = await db.AuthClients.SingleOrDefaultAsync(p => p.Id == userID);
            if (user == null)
                return new FileInfoResponse() { Error = "User not found", ErrorCode = "SF_DELF_4040" };
            var dir = await db.Directory.SingleOrDefaultAsync(p => p.Key == request.DirectoryKey);
            if (dir == null)
                return new FileInfoResponse() { Error = "Directory not found", ErrorCode = "SF_DELF_4041" };
            var file = await db.StorageFiles.SingleOrDefaultAsync(p => p.Key == request.FileKey && p.Directory == dir);
            if (dir == null)
                return new FileInfoResponse() { Error = "File not found", ErrorCode = "SF_DELF_4042" };
            // TO DO Add chack user rights
            if (file.Owner != user)
                return new FileInfoResponse() { Error = "Access denied.", ErrorCode = "SF_DELF_4030" };
            FtpService.Instance.DeleteFile(file, DefaultService.FileStorage);
            FtpService.Instance.DeleteDir(file.Key, DefaultService.FileStorage);
            db.StorageFiles.Remove(file);
            await db.SaveChangesAsync();
            return new FileInfoResponse() { Success = true };
        }

        public async Task<DownloadFileResponse> DownloadFile(string key)
        {
            if (!cache.TryGetValue($"STRORAGE_FILE_DisposableKey_{key}", out string data))
                return new DownloadFileResponse() { Error = "Key not found", ErrorCode = "SF_DF_4040" };
            cache.Remove($"STRORAGE_FILE_DisposableKey_{key}");
            string dir = "", file = "";
            {
                string[] tmp = data.Split('_');
                dir = tmp[0];
                file = tmp[1];
            }
            var fromDir = await db.Directory.SingleOrDefaultAsync(p => p.Key == dir);
            if (fromDir == null)
                return new DownloadFileResponse() { Error = "Directory not found", ErrorCode = "SF_DF_4041" };
            var fileModel = await db.StorageFiles.SingleOrDefaultAsync(p => p.Key == file);
            if (fileModel == null)
                return new DownloadFileResponse() { Error = "File not found", ErrorCode = "SF_DF_4040" }; ;
            return new DownloadFileResponse()
            {
                Success = true,
                FileModel = fileModel,
                Stream = FtpService.Instance.Download(fileModel, DefaultService.FileStorage)
            };
        }

        public async Task<FileDisposableKeyResponse> GetDisposableKey(FileInfoRequest request, int userID)
        {
            var user = await db.AuthClients.SingleOrDefaultAsync(p => p.Id == userID);
            if (user == null)
                return new FileDisposableKeyResponse() { Error = "User not found", ErrorCode = "SF_GDK_4040" };
            var dir = await db.Directory.SingleOrDefaultAsync(p => p.Key == request.DirectoryKey);
            if (dir == null)
                return new FileDisposableKeyResponse() { Error = "Directory not found", ErrorCode = "SF_GDK_4041" };
            var file = await db.StorageFiles.SingleOrDefaultAsync(p => p.Key == request.FileKey && p.Directory == dir);
            if (dir == null)
                return new FileDisposableKeyResponse() { Error = "File not found", ErrorCode = "SF_GDK_4042" };
            var key = "".RandomString(256);
            cache.CreateEntry($"STRORAGE_FILE_DisposableKey_{key}");
            cache.Set($"STRORAGE_FILE_DisposableKey_{key}", $"{dir.Key}_{file.Key}", TimeSpan.FromMinutes(1));
            return new FileDisposableKeyResponse() { Success = true, Key = key };
        }

        public async Task<FileInfoResponse> GetFileInfo(FileInfoRequest request, int userID)
        {
            var user = await db.AuthClients.SingleOrDefaultAsync(p => p.Id == userID);
            if (user == null)
                return new FileInfoResponse() { Error = "User not found", ErrorCode = "SF_GFI_4040" };
            var dir = await db.Directory.SingleOrDefaultAsync(p => p.Key == request.DirectoryKey);
            if (dir == null)
                return new FileInfoResponse() { Error = "Directory not found", ErrorCode = "SF_GFI_4041" };
            var file = await db.StorageFiles.SingleOrDefaultAsync(p => p.Key == request.FileKey && p.Directory == dir);
            if (dir == null)
                return new FileInfoResponse() { Error = "File not found", ErrorCode = "SF_GFI_4042" };

            // TO DO Add check user rights.

            return new FileInfoResponse()
            {
                Success = true,
                Dir = dir.Key,
                CreateDate = file.CreateDate,
                UpdateDate = file.UpdateDate,
                Key = file.Key,
                Name = file.Name,
                Size = file.Size,
            };
        }

        public async Task<DirectoryContentListResponse> GetFilesList(string dirKey, int userID, string mode = null)
        {
            var user = await db.AuthClients.SingleOrDefaultAsync(p => p.Id == userID);
            if (user == null)
                return new DirectoryContentListResponse() { Error = "User not found", ErrorCode = "SF_GFL_4040" };

            List<DirectoryContentResponse> list = new List<DirectoryContentResponse>();

            var fromDir = await db.Directory.SingleOrDefaultAsync(p => p.Key == dirKey && p.Owner == user);
            if (fromDir == null)
            {
                var share = (await db.ShareItems.SingleOrDefaultAsync(p => p.ShaeKey == dirKey));
                fromDir = await db.Directory.SingleOrDefaultAsync(p => p.Id == share.ShareObjectId);
            }
            if (fromDir == null)
                return new DirectoryContentListResponse() { Error = "Directory not found", ErrorCode = "SF_GFL_4041" };
            list = (from d in db.Directory
                    where d.Root == fromDir
                    select new DirectoryContentResponse()
                    {
                        Key = d.Key,
                        Name = d.Name,
                        IsHaveFolders = db.Directory.Any(p => p.Root == d),
                        IsDir = true,
                        IsFile = false,
                        ShareKey = "",
                        CreateDate = d.CreateDate,
                        UpdateDate = d.UpdateDate
                    }).ToList();
            if (mode != "only_dir")
                list.AddRange((from f in db.StorageFiles
                               where f.Directory == fromDir
                               select new DirectoryContentResponse()
                               {
                                   Key = f.Key,
                                   Name = f.Name,
                                   IsHaveFolders = false,
                                   IsDir = false,
                                   IsFile = true,
                                   ShareKey = db.ShareItems.Any(p => p.ShareObjectId == f.Id) ?
                                              db.ShareItems.SingleOrDefault(p => p.ShareObjectId == f.Id).ShaeKey : "", 
                                   CreateDate = f.CreateDate,
                                   UpdateDate = f.UpdateDate
                               }).ToList());
            return new DirectoryContentListResponse()
            {
                Success = true,
                Content = list
            };
        }

        public async Task<FileInfoResponse> Rename(FileRenameRequest request, int userID)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return new FileInfoResponse() { Error = "'FileName' is empty", ErrorCode = "SF_R_4000" };
            var user = await db.AuthClients.SingleOrDefaultAsync(p => p.Id == userID);
            if (user == null)
                return new FileInfoResponse() { Error = "User not found", ErrorCode = "SF_R_4040" };
            var dir = await db.Directory.SingleOrDefaultAsync(p => p.Key == request.DirectoryKey);
            if (dir == null)
                return new FileInfoResponse() { Error = "Directory not found", ErrorCode = "SF_R_4041" };
            var file = await db.StorageFiles.SingleOrDefaultAsync(p => p.Key == request.Key && p.Directory == dir);
            if (dir == null)
                return new FileInfoResponse() { Error = "File not found", ErrorCode = "SF_R_4042" };
            file.Name = request.Name;
            file.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
            return new FileInfoResponse()
            {
                Success = true,
                Dir = dir.Key,
                CreateDate = file.CreateDate,
                UpdateDate = file.UpdateDate,
                Key = file.Key,
                Name = file.Name,
                Size = file.Size,
            };
        }

        public async Task<FileInfoResponse> UploadFile(string dir, IFormFile fileForm, int userID)
        {
            if (fileForm == null)
                return new FileInfoResponse() { Error = "No content to upload", ErrorCode = "SF_UF_4000" };
            if (fileForm.Length > Config.Instance.Services.FileStorage.MaxUploadSize)
                return new FileInfoResponse() { Error = "The file is too large", ErrorCode = "SF_UF_4001" };
            var user = await db.AuthClients.SingleOrDefaultAsync(p => p.Id == userID);
            if (user == null)
                return new FileInfoResponse() { Error = "User not found", ErrorCode = "SF_UF_4040" };
            var directory = db.Directory.SingleOrDefault(p => p.Key == dir);
            if (directory == null)
                return new FileInfoResponse() { Error = "Directory not found", ErrorCode = "SF_UF_4041" };
            if (directory.Owner != user)
                return new FileInfoResponse() { Error = "Access denied.", ErrorCode = "SF_UF_4030" };
            var createDate = DateTime.Now;
            var file = new StorageFileModel()
            {
                CreateDate = createDate,
                UpdateDate = createDate,
                Directory = directory,
                Key = FilesHelper.GeneranionKey(directory, createDate),
                Name = fileForm.FileName,
                Owner = user,
                Size = fileForm.Length
            };
            FtpService.Instance.Upload(fileForm.OpenReadStream(), file, DefaultService.FileStorage);
            file = (await db.StorageFiles.AddAsync(file)).Entity;
            await db.SaveChangesAsync();
            return new FileInfoResponse()
            {
                Success = true,
                Dir = directory.Key,
                CreateDate = file.CreateDate,
                UpdateDate = file.UpdateDate,
                Key = file.Key,
                Name = file.Name,
                Size = file.Size,
            };
        }
    }
}
