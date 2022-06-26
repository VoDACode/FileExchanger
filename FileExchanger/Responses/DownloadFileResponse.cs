using Core.Models;
using System.IO;
using System.Text.Json.Serialization;

namespace FileExchanger.Responses
{
    public class DownloadFileResponse : BaseResponse
    {
        [JsonIgnore]
        public Stream Stream { get; set; }
        [JsonIgnore]
        public StorageFileModel FileModel { get; set; }
    }
}
