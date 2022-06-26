using FileExchanger.Requests;
using FileExchanger.Responses;
using System.Threading.Tasks;

namespace FileExchanger.Interfaces
{
    public interface IShareService
    {
        Task<ShareItemResponse> StartShare(ShareItemRequest request, int userId);
        Task<ShareItemResponse> StorShare(string shareKey, int userId);
        Task<ShareItemStreamResponse> GetShareItem(string key);
    }
}
