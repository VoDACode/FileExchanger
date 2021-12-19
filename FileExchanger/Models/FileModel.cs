using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Models
{
    public enum FileAccessMode
    {
        /// <summary>
        /// Access, only to this user 
        /// </summary>
        Private,
        /// <summary>
        /// Access by link 
        /// </summary>
        Public,
        /// <summary>
        /// Access by link, requires a password 
        /// </summary>
        Limited
    }
    public class FileModel : IFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime CreateDate { get; set; }
        public FileAccessMode AccessMode { get; set; } = FileAccessMode.Private;
        public string DownloadKey { get; set; }
        public string Password { get; set; }
        public double SaveTime { get; set; } = TimeSpan.FromHours(1).Seconds;
        public int DownloadCount { get; set; } = 0;
        public int MaxDownloadCount { get; set; } = 0;

        public bool IsDeleteFile => this.CreateDate.AddSeconds(this.SaveTime) <= DateTime.Now
                                || (this.MaxDownloadCount != -1 && this.DownloadCount >= this.MaxDownloadCount);
    }
}
