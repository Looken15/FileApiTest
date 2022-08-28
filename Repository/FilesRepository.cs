using TestApi.Interfaces;
using TestApi.Models;
using System.Linq;
using System.Threading.Tasks;

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
            _ = context.Files.AddAsync(fileModel).ConfigureAwait(false);
            context.SaveChanges();
        }

        public void Update(FileModel fileModel)
        {
            context.Files.Update(fileModel);
            context.SaveChanges();
        }

        public FileModel GetFile(int id)
        {
            return context.Files.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<FileModel> GetFiles()
        {
            return context.Files;
        }
    }
}