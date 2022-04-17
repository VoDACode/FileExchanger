using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Models
{
    public interface IFile
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
