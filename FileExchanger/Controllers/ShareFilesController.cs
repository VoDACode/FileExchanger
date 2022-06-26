using FileExchanger.Interfaces;
using FileExchanger.Requests;
using FileExchanger.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FileExchanger.Controllers
{
    [Authorize(Policy = "AuthStorage")]
    [Route("api/share")]
    [ApiController]
    public class ShareFilesController : BaseController
    {
        private readonly IShareService shareService;
        public ShareFilesController(IShareService shareService)
        {
            this.shareService = shareService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShare(ShareItemRequest shareRequest)
        {
            if(string.IsNullOrWhiteSpace(shareRequest.Key))
            {
                return BadRequest(new ShareItemResponse()
                {
                    Error = "Item key is empty!",
                    ErrorCode = "SH11"
                });
            }

            var response = await shareService.StartShare(shareRequest, UserID);

            if (!response.Success)
            {
                return UnprocessableEntity(response);
            }
            
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("{key}")]
        public async Task<IActionResult> DownloadFile(string key)
        {
            var response = await shareService.GetShareItem(key);
            if (!response.Success)
                return UnprocessableEntity(response);
            return File(response.Stream, "application/octet-stream", response.Filename, true);
        }

        [HttpDelete]
        public async Task<IActionResult> StopShare(string key)
        {
            var response = await shareService.StorShare(key, UserID);
            if (!response.Success)
                return UnprocessableEntity(response);
            return Ok(response);
        }
    }
}
