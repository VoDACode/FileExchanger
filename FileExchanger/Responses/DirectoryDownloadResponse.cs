using System.Text.Json.Serialization;

namespace FileExchanger.Responses
{
    public class DirectoryDownloadResponse : BaseResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string DownloadUrl { get; set; }
    }
}
