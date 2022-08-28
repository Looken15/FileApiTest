using System;
using TestApi.Interfaces;
using Base62;
using TestApi.Models;
using System.Linq;

namespace TestApi.BLL
{
    public class LinksService : ILinksService
    {
        private readonly ILinksRepository linksRepository;
        private readonly IFilesRepository filesRepository;
        public LinksService(ILinksRepository _linksRepository, IFilesRepository _filesRepository)
        {
            linksRepository = _linksRepository;
            filesRepository = _filesRepository;
        }

        public string GenerateLink(int id, string host)
        {
            var file = filesRepository.GetFile(id);
            if (file == null)
                return "File is missing from the database";

            var guid = Guid.NewGuid();
            var parameters = $"Download?id={id}&guid={guid}";

            var dbLink = new Link
            {
                Guid = guid,
                IsLinkUsed = false,
                FileId = id
            };
            linksRepository.Add(dbLink);

            var converter = new Base62Converter();
            return $"{host}/FilesApi/download/true/" + converter.Encode(parameters);
        }

        public (bool, int) CheckLink(int fileId, Guid guid)
        {
            var links = linksRepository.GetLinksByFileId(fileId);
            var res = links.FirstOrDefault(x => x.Guid == guid && !x.IsLinkUsed);

            if (res == default)
                return (false, -1);
            return (true, res.Id);
        }

        public void SetUsed(int id)
        {
            var link = linksRepository.GetLink(id);
            link.IsLinkUsed = true;
            linksRepository.Update(link);
        }
    }
}