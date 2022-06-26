using System.Collections.Generic;

namespace FileExchanger.Responses
{
    public class DirectoryContentListResponse : BaseResponse
    {
        public List<DirectoryContentResponse> Content { get; set; }
    }
}
