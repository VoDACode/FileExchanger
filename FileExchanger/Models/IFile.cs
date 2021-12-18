using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Models
{
    interface IFile
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
