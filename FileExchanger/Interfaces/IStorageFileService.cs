using FileExchanger.Responses;
using FileExchanger.Requests;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FileExchanger.Interfaces
{
    public interface IStorageFileService
    {
        Task<FileInfoResponse> GetFileInfo(FileInfoRequest request, int userID);
        Task<DirectoryContentListResponse> GetFilesList(string dirKey, int userID, string mode = default);
        Task<FileInfoResponse> DeleteFile(FileInfoRequest request, int userID);
        Task<FileInfoResponse> UploadFile(string dir, IFormFile file, int userID);
        Task<FileDisposableKeyResponse> GetDisposableKey(FileInfoRequest request, int userID);
        Task<DownloadFileResponse> DownloadFile(string key);
        Task<FileInfoResponse> Rename(FileRenameRequest request, int userID);
    }
}
