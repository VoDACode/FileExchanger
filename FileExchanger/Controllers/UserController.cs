using FileExchanger.Models;
using FileExchanger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Controllers
{
    [Authorize(Policy = "AuthExchanger")]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private AuthClientModel? authClient => db.AuthClients.SingleOrDefault(p => p.Email == User.Identity.Name);
        private UserModel getUser => db.Users.FirstOrDefault(p => p.Key == HttpContext.Request.Cookies["u_key"]);
        private DbApp db;
        public UserController(DbApp db)
        {
            this.db = db;
            Cleaner.ClearUsers(db);
        }

        [HttpPost("create")]
        public IActionResult Create()
        {
            if (getUser != null)
            {
                getUser.LastActive = DateTime.Now;
                db.SaveChanges();
                return Ok(getUser);
            }
            else
                HttpContext.Response.Cookies.Delete("u_key");
            var user = new UserModel()
            {
                Key = "".RandomString(256),
                RegistrationDate = DateTime.Now,
                LastActive = DateTime.Now
            };
            db.Users.Add(user);
            db.SaveChanges();
            HttpContext.Response.Cookies.Append("u_key", user.Key);
            return Ok(user);
        }

        [HttpGet("detect")]
        public IActionResult Detect()
        {
            if (db.Users.Any(p => p.Key == HttpContext.Request.Cookies["u_key"]))
                return Ok("User found!");
            return BadRequest("User not found!");
        }

        [HttpGet("my")]
        public IActionResult GetMyInfo()
        {
            if (getUser == null)
                return Unauthorized("Are you not authorized!");
            return Ok(new
            {
                id = getUser.Id,
                registrationDate = getUser.RegistrationDate
            });
        }
        [HttpGet("is-admin")]
        public IActionResult GetIsAdmin()
        {
            return Ok(db.Admins.Any(p => p.AuthClient == authClient));
        }
    }
}
