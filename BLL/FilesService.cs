using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using TestApi.Interfaces;
using TestApi.Models;
using System.Linq;
using System.Net;
using System;
using Base62;

namespace TestApi.BLL
{
    public class FilesService : IFilesService
    {
        public enum LinkCheck
        {
            LinkUsed,
            WrongRequest,
            OK
        }

        private readonly string fileMimeType = "application/octet-stream";
        private readonly IFilesRepository repository;
        private readonly ILinksService linksService;
        public FilesService(IFilesRepository _repository, ILinksService _linksService)
        {
            repository = _repository;
            linksService = _linksService;
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

        public (LinkCheck, FileStreamResult) GetOneTimeLinkFile(string encoded)
        {
            var converter = new Base62Converter();
            var decoded = converter.Decode(encoded);

            var splitted = decoded.Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitted[0] != "Download")
                return (LinkCheck.WrongRequest, null);


            var parametersSplit = splitted[1].Split(new char[] { '&' });
            var idParamSplit = parametersSplit[0].Split(new char[] { '=' });
            if (idParamSplit[0] != "id" || !int.TryParse(idParamSplit[1], out int id))
                return (LinkCheck.WrongRequest, null);
            var guidParamSplit = parametersSplit[1].Split(new char[] { '=' });
            if (guidParamSplit[0] != "guid" || !Guid.TryParse(guidParamSplit[1], out Guid guid))
                return (LinkCheck.WrongRequest, null);

            var check = linksService.CheckLink(id, guid);
            if (!check.Item1)
                return (LinkCheck.LinkUsed, null);

            linksService.SetUsed(check.Item2);

            return (LinkCheck.OK, GetFile(id));
        }

        public FilesInfo[] GetAllFiles()
        {
            return repository.GetFiles()
                             .Select(x => new FilesInfo
                             {
                                 Id = x.Id,
                                 FileName = x.FileName
                             })
                             .ToArray();
        }
    }
}