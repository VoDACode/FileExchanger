using FileExchanger.Requests;
using FileExchanger.Responses;
using System.Threading.Tasks;

namespace FileExchanger.Interfaces
{
    public interface IDirectoryService
    {
        Task<string> GetRootKey(int userId);
        Task<DirectoryResponse> GetInfo(DirectoryRequest directoryRequest, int userId);
        Task<DirectoryResponse> CreateDirectory(DirectoryEditRequest directoryRequest, int userId);
        Task<DirectoryResponse> DeleteDirectory(DirectoryRequest directoryRequest, int userId);
        Task<DirectoryResponse> RenameDirectory(DirectoryEditRequest directoryRequest, int userId);
        Task<DirectoryDownloadResponse> DownloadDirectory(DirectoryRequest directoryRequest, int userId);
    }
}
