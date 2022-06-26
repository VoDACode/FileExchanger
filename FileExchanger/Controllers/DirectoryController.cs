using FileExchanger.Models;
using FileExchanger.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using FileExchanger.Services;
using System.Collections.Generic;
using Core;
using Core.Models;
using Core.Zip;
using Core.Enums;
using FileExchanger.Requests;
using FileExchanger.Interfaces;
using FileExchanger.Responses;
using System.Threading.Tasks;

namespace FileExchanger.Controllers
{
    [Authorize(Policy = "AuthStorage")]
    [Route("api/dir/s")]
    [ApiController]
    public class DirectoryController : BaseController
    {
        private readonly IDirectoryService directoryService;
        public DirectoryController(IDirectoryService directoryService)
        {
            this.directoryService = directoryService;
        }

        [HttpGet("get-root")]
        public async Task<IActionResult> GetRootKey() => Ok(await directoryService.GetRootKey(UserID));

        [HttpGet("info")]
        public async Task<IActionResult> GetInfo(DirectoryRequest directoryRequest)
        {
            if (string.IsNullOrWhiteSpace(directoryRequest.Key))
                return BadRequest(new DirectoryResponse() { Success = false, Error = "The 'key' must not be empty!", ErrorCode = "D_GI4001" });
            var response = await directoryService.GetInfo(directoryRequest, UserID);
            if(!response.Success)
                return UnprocessableEntity(response);
            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateDir(DirectoryEditRequest directoryRequest)
        {
            if (string.IsNullOrWhiteSpace(directoryRequest.Key))
                return BadRequest(new DirectoryResponse() { Error = "The 'key' must not be empty!", ErrorCode = "D_C4001" });
            if (string.IsNullOrWhiteSpace(directoryRequest.Name))
                return BadRequest(new DirectoryResponse() { Error = "The 'name' must not be empty!", ErrorCode = "D_C4002" });
            var response = await directoryService.CreateDirectory(directoryRequest, UserID);
            if(!response.Success)
                return UnprocessableEntity(response);
            return Ok(response);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteDir(DirectoryRequest directoryRequest)
        {
            if (string.IsNullOrWhiteSpace(directoryRequest.Key))
                return BadRequest(new DirectoryResponse() { Success = false, Error = "The 'key' must not be empty!", ErrorCode = "D_D4001" });
            var response = await directoryService.DeleteDirectory(directoryRequest, UserID);
            if (!response.Success)
                return UnprocessableEntity(response);
            return Ok(response);
        }

        [HttpPost("rename")]
        public async Task<IActionResult> Rename(DirectoryEditRequest directoryRequest)
        {
            if (string.IsNullOrWhiteSpace(directoryRequest.Key))
                return BadRequest(new DirectoryResponse() { Error = "The 'key' must not be empty!", ErrorCode = "D_R4001" });
            if (string.IsNullOrWhiteSpace(directoryRequest.Name))
                return BadRequest(new DirectoryResponse() { Error = "The 'name' must not be empty!", ErrorCode = "D_R4002" });
            var response = await directoryService.RenameDirectory(directoryRequest, UserID);
            if (!response.Success)
                return UnprocessableEntity(response);
            return Ok(response);
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download([FromQuery]DirectoryRequest directoryRequest)
        {
            if (string.IsNullOrWhiteSpace(directoryRequest.Key))
                return BadRequest(new DirectoryResponse() { Success = false, Error = "The 'key' must not be empty!", ErrorCode = "D_DOWNLOAD4001" });
            var response = await directoryService.DownloadDirectory(directoryRequest, UserID);
            if (!response.Success)
                return UnprocessableEntity(response);
            return Ok(response);
        }
    }
}
