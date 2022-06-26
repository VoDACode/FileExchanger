using System.IO;

namespace FileExchanger.Responses
{
    public class ShareItemStreamResponse : BaseResponse
    {
        public Stream Stream { get; set; }
        public string Filename { get; set; }
    }
}
