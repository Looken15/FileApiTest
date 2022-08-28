using System;
using TestApi.Interfaces;
using Base62;
using TestApi.Models;
using System.Linq;

namespace TestApi.BLL
{
    public class LinksService : ILinksService
    {
        private readonly ILinksRepository repository;
        public LinksService(ILinksRepository _repository)
        {
            repository = _repository;
        }

        public string GenerateLink(int id, string host)
        {
            var guid = Guid.NewGuid();
            var parameters = $"Download?id={id}&guid={guid}";

            var dbLink = new Link
            {
                Guid = guid,
                IsLinkUsed = false,
                FileId = id
            };
            repository.Add(dbLink);

            var converter = new Base62Converter();
            return $"{host}/FilesApi/download/true/" + converter.Encode(parameters);
        }

        public (bool, int) CheckLink(int fileId, Guid guid)
        {
            var links = repository.GetLinksByFileId(fileId);
            var res = links.FirstOrDefault(x => x.Guid == guid && !x.IsLinkUsed);

            if (res == default)
                return (false, -1);
            return (true, res.Id);
        }

        public void SetUsed(int id)
        {
            var link = repository.GetLink(id);
            link.IsLinkUsed = true;
            repository.Update(link);
        }
    }
}