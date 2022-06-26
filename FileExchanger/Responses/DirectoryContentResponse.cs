using System;

namespace FileExchanger.Responses
{
    public class DirectoryContentResponse : BaseResponse
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Dir { get; set; }
        public bool IsFile { get; set; }
        public bool IsDir { get; set; }
        public string ShareKey { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsHaveFolders { get; set; }
    }
}
