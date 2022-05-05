using System;

namespace Core.Models
{
    public class DirectoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public DirectoryModel Root { get; set; }
        public AuthClientModel Owner { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
