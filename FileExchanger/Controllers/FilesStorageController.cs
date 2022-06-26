using FileExchanger.Interfaces;
using FileExchanger.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FileExchanger.Controllers
{
    [Authorize(Policy = "AuthStorage")]
    [Route("api/files/s")]
    [ApiController]
    public class FilesStorageController : BaseController
    {
        private readonly IStorageFileService storageFileService;
        public FilesStorageController(IStorageFileService storageFileService)
        {
            this.storageFileService = storageFileService;
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetFileInfo([FromQuery] FileInfoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FileKey))
                return BadRequest("'FileKey' is empty");
            if (string.IsNullOrWhiteSpace(request.DirectoryKey))
                return BadRequest("'DirectoryKey' is empty");
            var response = await storageFileService.GetFileInfo(request, UserID);
            if (!response.Success)
                return UnprocessableEntity(response);
            return Ok(response);
        }

        [HttpGet("{dir}/list")]
        public async Task<IActionResult> GetFilesList(string dir, string mode)
        {
            var response = await storageFileService.GetFilesList(dir, UserID, mode);
            if (!response.Success)
                return UnprocessableEntity(response);
            return Ok(response);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFile(FileInfoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FileKey))
                return BadRequest("'FileKey' is empty");
            if (string.IsNullOrWhiteSpace(request.DirectoryKey))
                return BadRequest("'DirectoryKey' is empty");
            var response = await storageFileService.DeleteFile(request, UserID);
            if (!response.Success)
                return UnprocessableEntity(response);
            return Ok(response);
        }

        [RequestSizeLimit(16L * 1024L * 1024L * 1024L)]
        [RequestFormLimits(MultipartBodyLengthLimit = 16L * 1024L * 1024L * 1024L)]
        [HttpPost("{dir}/upload")]
        public async Task<IActionResult> UploadFile(string dir, IFormFile file)
        {
            var response = await storageFileService.UploadFile(dir, file, UserID);
            if (!response.Success)
                return UnprocessableEntity(response);
            return Ok(response);
        }

        [HttpGet("get-disposable-key")]
        public async Task<IActionResult> GetDisposableKey([FromQuery] FileInfoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FileKey))
                return BadRequest("'FileKey' is empty");
            if (string.IsNullOrWhiteSpace(request.DirectoryKey))
                return BadRequest("'DirectoryKey' is empty");
            var response = await storageFileService.GetDisposableKey(request, UserID);
            if (!response.Success)
                return UnprocessableEntity(response);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("download/{key}")]
        public async Task<IActionResult> DownloadFile(string key)
        {
            var response = await storageFileService.DownloadFile(key);
            if (!response.Success)
                return NotFound(response);
            return File(response.Stream, "application/octet-stream", response.FileModel.Name, true);
        }

        [HttpPost("rename")]
        public async Task<IActionResult> Rename(FileRenameRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Key))
                return BadRequest("'FileKey' is empty");
            if (string.IsNullOrWhiteSpace(request.DirectoryKey))
                return BadRequest("'DirectoryKey' is empty");
            var response = await storageFileService.Rename(request, UserID);
            if (!response.Success)
                return UnprocessableEntity(response);
            return Ok(response);
        }
    }
}