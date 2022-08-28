using System.Linq;
using TestApi.Models;

namespace TestApi.Interfaces
{
    public interface IFilesRepository
    {
        void Add(FileModel fileModel);
        FileModel GetFile(int id);
        IQueryable<FileModel> GetFiles();
    }
}