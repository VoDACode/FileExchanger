using FileExchanger.Models;
using FileExchanger.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Controllers
{
    [Route("api/group")]
    [ApiController]
    public class WorkingGroupController : ControllerBase
    {
        private UserModel getUser => db.Users.FirstOrDefault(p => p.Key == HttpContext.Request.Cookies["u_key"]);
        private UserInWorkingGroupModel getUserWorkingGroup
        {
            get
            {
                db.WorkingGroups.ToList();
                return db.UserInWorkingGroups.FirstOrDefault(p => p.User == getUser);
            }
        }
        private WorkingGroupModel getWorkingGroup
        {
            get
            {
                var uwg = getUserWorkingGroup;
                if (uwg == null)
                    return null;
                return uwg.WorkingGroup;
            }
        }

        private DbApp db;
        public WorkingGroupController(DbApp db)
        {
            this.db = db;
            Cleaner.ClearUsers(db);
        }
        #region Actions
        [HttpPost("create")]
        public IActionResult Create(string userName)
        {
            if (existUserInGroups)
                return BadRequest("You are already in a group! Exit the group if you want to create your own.");
            var group = new WorkingGroupModel()
            {
                CreateDate = DateTime.Now,
                JoinKey = "".RandomString(64)
            };
            db.WorkingGroups.Add(group);
            var userInGroup = new UserInWorkingGroupModel()
            {
                IsPermissionAddUser = true,
                IsCreator = true,
                JoinDate = DateTime.Now,
                User = getUser,
                WorkingGroup = group,
                Title = userName
            };
            db.UserInWorkingGroups.Add(userInGroup);
            db.SaveChanges();
            return Ok(new
            {
                group = new { 
                    key = group.JoinKey,
                    id = group.Id,
                    createDate = group.CreateDate
                },
                user = userInGroup
            });
        }

        [HttpPost("remove")]
        public IActionResult Remove()
        {
            db.Users.ToList();
            db.WorkingGroups.ToList();
            db.UserInWorkingGroups.ToList();
            if (!existUserInGroups)
                return BadRequest("You are not in any group!");
            if (!getUserWorkingGroup.IsCreator)
            {
                return Leave();
            }
            var group = getWorkingGroup;
            var users = db.UserInWorkingGroups.Where(p => p.WorkingGroup == group);
            db.UserInWorkingGroups.RemoveRange(users);
            db.WorkingGroups.Remove(group);
            db.SaveChanges();
            return Ok();
        }

        [HttpGet("info")]
        public IActionResult Info()
        {
            if (!existUserInGroups)
                return BadRequest("You are not in any group!");
            if (!getUserWorkingGroup.IsCreator && !getUserWorkingGroup.IsPermissionAddUser)
                return Ok(new
                {
                    group = new
                    {
                        accessLevel = 2,
                        id = getWorkingGroup.Id
                    }
                });
            if (getUserWorkingGroup.IsPermissionAddUser && !getUserWorkingGroup.IsCreator)
                return Ok(new
                {
                    group = new
                    {
                        accessLevel = 1,
                        id = getWorkingGroup.Id,
                        key = getWorkingGroup.JoinKey
                    }
                });
            var users = from u in db.UserInWorkingGroups
                        select new
                        {
                            lockData = u.User.Id == getUser.Id,
                            id = u.User.Id,
                            isPermissionAddUser = u.IsPermissionAddUser,
                            joinDate = u.JoinDate.ToString("dd.MM.yyy HH:mm:ss"),
                            title = u.Title,
                            lastActive = u.User.LastActive.ToString("dd.MM.yyy HH:mm:ss"),
                        };
            return Ok(new
            {
                group = new
                {
                    accessLevel = 0,
                    id = getWorkingGroup.Id,
                    key = getWorkingGroup.JoinKey
                },
                users = users
            });
        }

        [HttpPost("join/{key}")]
        public IActionResult Join(string key, string userName)
        {
            if (existUserInGroups)
                return BadRequest("You are already in a group! Exit the group if you want to create your own.");
            var group = db.WorkingGroups.FirstOrDefault(p => p.JoinKey == key);
            if (group == null)
                return BadRequest("Incorrect join key!");
            var user = new UserInWorkingGroupModel()
            {
                IsCreator = false,
                IsPermissionAddUser = false,
                JoinDate = DateTime.Now,
                Title = userName,
                User = getUser,
                WorkingGroup = group
            };
            db.UserInWorkingGroups.Add(user);
            db.SaveChanges();
            return Ok();
        }

        [HttpPost("leave")]
        public IActionResult Leave()
        {
            if (!existUserInGroups)
                return BadRequest("You are not in any group!");
            if (getUserWorkingGroup.IsCreator)
                Remove();
            else
            {
                db.UserInWorkingGroups.Remove(getUserWorkingGroup);
                db.SaveChanges();
            }
            return Ok();
        }

        [HttpPost("set/session-name")]
        public IActionResult SetSessionName(string n)
        {
            if (string.IsNullOrWhiteSpace(n))
                return BadRequest("'n' isn`t null!");
            if (!existUserInGroups)
                return BadRequest("You are not in any group!");

            getUserWorkingGroup.Title = n;
            db.SaveChanges();

            return Ok();
        }

        [HttpPost("users/kik")]
        public IActionResult UserKik(int uid)
        {
            if(!existUserInGroups)
                return BadRequest("You are not in any group!");
            if (!getUserWorkingGroup.IsCreator)
                return Unauthorized();
            if (uid == getUserWorkingGroup.User.Id)
                return BadRequest("You can`t kik yourself!");
            
            var user = db.UserInWorkingGroups.FirstOrDefault(p => p.User.Id == uid);
            if (user == null)
                return BadRequest("User not found!");
            db.UserInWorkingGroups.Remove(user);            
            db.SaveChanges();
            return Ok();
        }

        [HttpPost("users/permissions/add-users")]
        public IActionResult SetUserPermissionAddUser(int uid, bool m)
        {
            if (!existUserInGroups)
                return BadRequest("You are not in any group!");
            if (!getUserWorkingGroup.IsCreator)
                return Unauthorized();
            if(uid == getUser.Id)
                return BadRequest("You can`t set your permissions.");

            var user = db.UserInWorkingGroups.FirstOrDefault(p => p.User.Id == uid);
            if (user == null)
                return BadRequest("User not found!");

            user.IsPermissionAddUser = m;
            db.SaveChanges();

            return Ok();
        }

        [HttpPost("join/url/re-creation")]
        public IActionResult JoinUrlReCreation()
        {
            if (!existUserInGroups)
                return BadRequest("You are not in any group!");
            if (!getUserWorkingGroup.IsCreator)
                return Unauthorized();
            getWorkingGroup.JoinKey = "".RandomString(64);
            db.SaveChanges();
            return Ok(getWorkingGroup.JoinKey);
        }

        [HttpGet("join/utl")]
        public IActionResult GetJoinUrl()
        {
            if (!existUserInGroups)
                return BadRequest("You are not in any group!");
            if (!getUserWorkingGroup.IsPermissionAddUser)
                return Unauthorized();
            return Ok(getWorkingGroup.JoinKey);
        }
        #endregion

        #region Private functions
        private bool existUserInGroups => getUserWorkingGroup != null;
        #endregion
    }
}
