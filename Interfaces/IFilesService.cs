using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestApi.Models;
using static TestApi.BLL.FilesService;

namespace TestApi.Interfaces
{
    public interface IFilesService
    {
        (string, int) AddFile(IFormFile file);
        FileStreamResult GetFile(int id);
        FilesInfo[] GetAllFiles();
        (LinkCheck, FileStreamResult) GetOneTimeLinkFile(string encoded);
    }
}