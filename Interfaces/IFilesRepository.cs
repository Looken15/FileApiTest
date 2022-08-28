using System.Linq;
using System.Threading.Tasks;
using TestApi.Models;

namespace TestApi.Interfaces
{
    public interface IFilesRepository
    {
        void Add(FileModel fileModel);
        void Update(FileModel fileModel);
        FileModel GetFile(int id);
        IQueryable<FileModel> GetFiles();
    }
}