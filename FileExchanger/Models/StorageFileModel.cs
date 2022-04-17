using System;

namespace FileExchanger.Models
{
    public class StorageFileModel : IFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public long Size { get; set; }
        public AuthClientModel Owner { get; set; }
        public DirectoryModel Directory { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
