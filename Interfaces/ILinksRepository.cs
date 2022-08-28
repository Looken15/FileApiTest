using System.Linq;
using TestApi.Models;

namespace TestApi.Interfaces
{
    public interface ILinksRepository
    {
        void Add(Link link);
        void Update(Link link);
        Link GetLink(int id);
        IQueryable<Link> GetLinksByFileId(int fileId);
    }
}