using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Models
{
    public class WorkingGroupModel
    {
        public int Id { get; set; }
        public string JoinKey { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
