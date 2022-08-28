using System.Linq;
using TestApi.Interfaces;
using TestApi.Models;

namespace TestApi.Repository
{
    public class LinksRepository : ILinksRepository
    {
        private readonly Context context;
        public LinksRepository(Context _context)
        {
            context = _context;
        }

        public void Add(Link link)
        {
            context.Links.Add(link);
            context.SaveChanges();
        }

        public void Update(Link link)
        {
            context.Links.Update(link);
            context.SaveChanges();
        }

        public Link GetLink(int id)
        {
            return context.Links.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<Link> GetLinksByFileId(int fileId)
        {
            return context.Links.Where(x => x.FileId == fileId);
        }
    }
}