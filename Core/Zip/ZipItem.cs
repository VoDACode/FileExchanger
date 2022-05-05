namespace Core.Zip
{
    public enum ContentType { File, Folder }
    public class ZipItem
    {
        public string Path { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public ContentType Type { get; set; }
    }
}
