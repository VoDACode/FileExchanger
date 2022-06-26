using Core;
using Core.Enums;
using Core.Models;
using FileExchanger.Interfaces;
using FileExchanger.Requests;
using FileExchanger.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Services
{
    public class ShareService : IShareService
    {
        private readonly DbApp db;

        public ShareService(DbApp db)
        {
            this.db = db;
        }

        public async Task<ShareItemStreamResponse> GetShareItem(string key)
        {
            var shareItem = await db.ShareItems.SingleOrDefaultAsync(p => p.ShaeKey == key);
            if (shareItem == null)
                return new ShareItemStreamResponse() { Error = "'key' not found.", ErrorCode = "SH_GET4040" };
            if (shareItem.ItemType == ItemType.File)
            {
                var fileModel = await db.StorageFiles.SingleOrDefaultAsync(p => p.Id == shareItem.ShareObjectId);
                if (fileModel == null)
                {
                    db.ShareItems.Remove(shareItem);
                    await db.SaveChangesAsync();
                    return new ShareItemStreamResponse() { Error = "File not found.", ErrorCode = "SH_GET4041" };
                }
                return new ShareItemStreamResponse()
                {
                    Success = true,
                    Stream = FtpService.Instance.Download(fileModel, DefaultService.FileStorage),
                    Filename = fileModel.Name
                };
            }
            return new ShareItemStreamResponse() { Error = "Folder not supported", ErrorCode = "SH_GET5000" };
        }

        public async Task<ShareItemResponse> StartShare(ShareItemRequest request, int userId)
        {
            int id = -1;

            var user = await db.AuthClients.SingleOrDefaultAsync(p => p.Id == userId);
            if (user == null)
                return new ShareItemResponse()
                {
                    Success = false,
                    Error = "User not found!",
                    ErrorCode = "SH00"
                };

            var tmp = await db.StorageFiles.SingleOrDefaultAsync(p => p.Key == request.Key);
            if (tmp == default)
                return new ShareItemResponse()
                {
                    Success = false,
                    Error = "File not found!",
                    ErrorCode = "SH10",
                };
            if (tmp.Owner != user)
                return new ShareItemResponse()
                {
                    Success = false,
                    Error = "You do not have permission to access the file!",
                    ErrorCode = "SH20"
                };
            id = tmp.Id;
            if (await db.ShareItems.AnyAsync(p => p.ShareObjectId == id))
                return new ShareItemResponse() { Error = "This object is already public.", ErrorCode = "SH30" };
            var item = (await db.ShareItems.AddAsync(new ShareItemModel()
            {
                ItemType = ItemType.File,
                ShareObjectId = id,
                ShaeKey = "".RandomString(96),
                Owner = user,
                CreateDate = DateTime.Now,
            })).Entity;

            await db.SaveChangesAsync();
            return new ShareItemResponse()
            {
                ShareId = item.Id,
                ShareKey = item.ShaeKey,
                Success = true
            };
        }

        public async Task<ShareItemResponse> StorShare(string shareKey, int userId)
        {
            var shareItem = await db.ShareItems.SingleOrDefaultAsync(p => p.ShaeKey == shareKey);
            if (shareItem == null)
                return new ShareItemResponse() { Error = "'key' not found.", ErrorCode = "SH_GET4040" };
            var user = await db.AuthClients.SingleOrDefaultAsync(p => p.Id == userId);
            if (user == null)
                return new ShareItemResponse() { Error = "User not found", ErrorCode = "SH_DELETE4041" };
            if (shareItem.Owner != user)
                return new ShareItemResponse() { Error = "Access denied", ErrorCode = "SH_DELETE4030" };
            db.ShareItems.Remove(shareItem);
            await db.SaveChangesAsync();
            return new ShareItemResponse()
            {
                Success = true,
                ShareId = shareItem.Id,
                ShareKey = shareKey
            };
        }
    }
}
