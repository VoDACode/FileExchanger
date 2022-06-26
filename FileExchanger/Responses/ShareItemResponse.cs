using System.Text.Json.Serialization;

namespace FileExchanger.Responses
{
    public class ShareItemResponse : BaseResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int ShareId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ShareKey { get; set; }
    }
}
