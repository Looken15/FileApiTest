using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TestApi.Interfaces
{
    public interface IFilesService
    {
        (string, int) AddFile(IFormFile file);
        FileStreamResult GetFile(int id);
    }
}