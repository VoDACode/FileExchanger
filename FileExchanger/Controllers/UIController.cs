using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FileExchanger.Controllers
{
    [ApiController]
    [Route("/api/ui")]
    public class UIController : ControllerBase
    {
        [HttpGet("save-patterns")]
        public IActionResult GetSavePatterns()
        {
            return Ok(Config.Instance.SavePatterns.Where(p => p.ToSecond() <= Config.Instance.Services.FileExchanger.MaxSaveTime));
        }

        [HttpGet("service/storage/enabled")]
        public IActionResult GetEnableStorage() =>
            Ok(Config.Instance.Services.FileStorage.Enable);
        [HttpGet("service/exchaner/enabled")]
        public IActionResult GetEnableExchanger() =>
            Ok(Config.Instance.Services.FileExchanger.Enable);

        #region Storage auth
        [HttpGet("auth/storage/use")]
        public IActionResult GetStorageUseAuth() => Ok(Config.Instance.Services.FileStorage.UseAuth);
        #endregion
        #region Exchange auth
        [HttpGet("auth/exchnge/use")]
        public IActionResult GetExchngeUseAuth() => Ok(Config.Instance.Services.FileExchanger.UseAuth);
        #endregion
        #region Main auth
        [HttpGet("default-service")]
        public IActionResult GetDefaultService()
        {
            if (Config.Instance.Services.FileStorage.Enable && !Config.Instance.Services.FileExchanger.Enable)
                return Ok(DefaultService.FileStorage.ToString());
            if (!Config.Instance.Services.FileStorage.Enable && Config.Instance.Services.FileExchanger.Enable)
                return Ok(DefaultService.FileExchanger.ToString());
            return Ok(Config.Instance.Services.DefaultService.ToString());
        }
        [HttpGet("auth/accounts/authorization")]
        public IActionResult GetAuthorization() => Ok(Config.Instance.Security.Authorization.ToString());
        [HttpGet("auth/accounts/enable")]
        public IActionResult GetAccountEnable() => Ok(Config.Instance.Security.Accounts.Enable);
        [HttpGet("auth/accounts/enable-registration")]
        public IActionResult GetAccountEnableRegistration() => Ok(Config.Instance.Security.Accounts.EnableRegistration);
        [HttpGet("auth/accounts/enable-telegram")]
        public IActionResult GetAccountEnableTelegram() => Ok(Config.Instance.Security.Telegram.Enable);
        #endregion
    }
}
