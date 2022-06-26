namespace FileExchanger.Requests
{
    public class FileRenameRequest
    {
        public string DirectoryKey { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
    }
}
