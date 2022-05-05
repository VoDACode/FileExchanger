using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class UserInWorkingGroupModel
    {
        public int Id { get; set; }
        public WorkingGroupModel WorkingGroup { get; set; }
        public UserModel User { get; set; }
        public string Title { get; set; }
        public bool IsCreator { get; set; }
        public bool IsPermissionAddUser { get; set; }
        public DateTime JoinDate { get; set; }
    }
}
