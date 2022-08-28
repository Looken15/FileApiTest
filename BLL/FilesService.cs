using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using TestApi.Interfaces;
using TestApi.Models;
using System.Linq;
using System.Net;
using System;
using Base62;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestApi.BLL
{
    public class FilesService : IFilesService
    {
        public enum LinkCheck
        {
            LinkUsed,
            WrongRequest,
            MissingFile,
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

        public async Task<(string, int)> AddFile(IFormFile file)
        {
            var fileModel = new FileModel
            {
                FileName = file.FileName,
                Content = Enumerable.Empty<byte>().ToArray()
            };
            repository.Add(fileModel);
            var t = Upload(file, fileModel.Id).ConfigureAwait(false);
            return (fileModel.FileName, fileModel.Id);
        }

        public async Task<string> AddFiles(List<IFormFile> files)
        {
            var sb = new StringBuilder();
            foreach (var file in files)
            {
                var fileInfo = await AddFile(file).ConfigureAwait(false);
                sb.AppendLine($"File {fileInfo.Item1} was successfully added with id {fileInfo.Item2}\n");
            }
            return sb.ToString();
        }

        public (bool, FileStreamResult) GetFile(int id)
        {
            var dbfile = repository.GetFile(id);
            if (dbfile == null)
                return (false, null);
            var stream = new MemoryStream(dbfile.Content);
            return (true,
                new FileStreamResult(stream, fileMimeType)
                {
                    FileDownloadName = dbfile.FileName
                });
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

            var file = GetFile(id);
            if (!file.Item1)
                return (LinkCheck.MissingFile, null);
            return (LinkCheck.OK, file.Item2);
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

        private async Task Upload(IFormFile file, int id)
        {
            byte[] buffer = new byte[16 * 1024];

            FileModel fileModel = repository.GetFile(id);

            long totalBytes = file.Length;
            long totalReadBytes = 0;
            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                int readBytes;
                while ((readBytes = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileModel.Content = fileModel.Content.Concat(buffer).ToArray();
                    //repository.Update(fileModel);
                    repository.Update(fileModel);
                    totalReadBytes += readBytes;

                    if (Startup.FilesProgress.ContainsKey(fileModel.Id))
                        Startup.FilesProgress[fileModel.Id] = (int)((float)totalReadBytes / (float)totalBytes * 100.0);
                    else
                        Startup.FilesProgress.Add(fileModel.Id, (int)((float)totalReadBytes / (float)totalBytes * 100.0));
                }
            }
        }
    }
}