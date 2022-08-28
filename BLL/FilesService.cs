using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using TestApi.Interfaces;
using TestApi.Models;

namespace TestApi.BLL
{
    public class FilesService : IFilesService
    {
        private readonly string fileMimeType = "application/octet-stream";
        private readonly IFilesRepository repository;
        public FilesService(IFilesRepository _repository)
        {
            repository = _repository;
        }

        public (string, int) AddFile(IFormFile file)
        {
            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                var fileModel = new FileModel
                {
                    FileName = file.FileName,
                    Content = reader.ReadBytes((int)reader.BaseStream.Length)
                };
                repository.Add(fileModel);
                return (fileModel.FileName, fileModel.Id);
            }
        }

        public FileStreamResult GetFile(int id)
        {
            var dbfile = repository.GetFile(id);
            var stream = new MemoryStream(dbfile.Content);
            return new FileStreamResult(stream, fileMimeType)
            {
                FileDownloadName = dbfile.FileName
            };
        }
    }
}