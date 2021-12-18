using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Models
{
    public class UserFilesModel
    {
        public int Id { get; set; }
        public UserModel User { get; set; }
        public FileModel File { get; set; }
    }
}
