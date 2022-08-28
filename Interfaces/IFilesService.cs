using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestApi.Models;
using static TestApi.BLL.FilesService;

namespace TestApi.Interfaces
{
    public interface IFilesService
    {
        Task<string> AddFiles(List<IFormFile> files);
        Task<(string, int)> AddFile(IFormFile file);
        (bool, FileStreamResult) GetFile(int id);
        FilesInfo[] GetAllFiles();
        (LinkCheck, FileStreamResult) GetOneTimeLinkFile(string encoded);
    }
}