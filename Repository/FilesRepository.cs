using TestApi.Interfaces;
using TestApi.Models;
using System.Linq;

namespace TestApi.Repository
{
    public class FilesRepository : IFilesRepository
    {
        private readonly Context context;
        public FilesRepository(Context _context)
        {
            context = _context;
        }

        public void Add(FileModel fileModel)
        {
            context.Files.Add(fileModel);
            context.SaveChanges();
        }

        public FileModel GetFile(int id)
        {
            return context.Files.FirstOrDefault(x => x.Id == id);
        }
    }
}