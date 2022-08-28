using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestApi.Interfaces;
using TestApi.Models;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DownloadController : ControllerBase
    {
        private readonly IFilesService service;
        public DownloadController(IFilesService _service)
        {
            service = _service;
        }

        [HttpPost("upload")]
        public string Upload(IFormFile file)
        {
            var fileInfo = service.AddFile(file);
            return $"File {fileInfo.Item1} was successfully added with id {fileInfo.Item2}";
        }

        [HttpGet("download/{id}")]
        public FileStreamResult Download(int id)
        {
            return service.GetFile(id);
        }

        [HttpGet("listUploaded")]
        public FilesInfo[] GetListUploaded()
        {
            return service.GetAllFiles();
        }
    }
}
