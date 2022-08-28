using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TestApi.Interfaces;
using TestApi.Models;
using static TestApi.BLL.FilesService;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesApiController : ControllerBase
    {
        private readonly IFilesService filesService;
        private readonly ILinksService linksService;
        public FilesApiController(IFilesService _filesService, ILinksService _linksService)
        {
            filesService = _filesService;
            linksService = _linksService;
        }

        [HttpPost("upload")]
        public async Task<string> Upload(List<IFormFile> files)
        {
            return await filesService.AddFiles(files);
        }

        [HttpPost("uploadAndGenerateLink")]
        public async Task<string> UploadAndGenerateLink(IFormFile file)
        {
            var fileInfo = await filesService.AddFile(file);
            var link = linksService.GenerateLink(fileInfo.Item2, Request.Host.Value);
            return $"File {fileInfo.Item1} was successfully added with id {fileInfo.Item2}\n" +
                $"Link to download: {link}";
        }

        [HttpGet("progress/{id}")]
        public string Progress(int id)
        {
            if (Startup.FilesProgress.ContainsKey(id))
                return Startup.FilesProgress[id].ToString() + "%";
            else
                return "File has either not been uploaded yet or is missing from the database";
        }

        [HttpGet("download/{id}")]
        public IActionResult Download(int id)
        {
            var result = filesService.GetFile(id);
            if (!result.Item1)
                return Content("File is missing from the database");
            return result.Item2;
        }

        [HttpGet("download/{link}/{encoded}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Download(string encoded, bool link)
        {
            var result = filesService.GetOneTimeLinkFile(encoded);
            if (result.Item1 == LinkCheck.WrongRequest)
                return Content("Wrong one-time link");
            if (result.Item1 == LinkCheck.LinkUsed)
                return Content("This link has already been used");
            if (result.Item1 == LinkCheck.MissingFile)
                return Content("File is missing from the database");
            return result.Item2;
        }

        [HttpGet("listUploaded")]
        public FilesInfo[] GetListUploaded()
        {
            return filesService.GetAllFiles();
        }

        [HttpGet("generateLink/{id}")]
        public string GenerateLink(int id)
        {
            return linksService.GenerateLink(id, Request.Host.Value);
        }
    }
}
