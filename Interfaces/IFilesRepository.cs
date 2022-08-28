using TestApi.Models;

namespace TestApi.Interfaces
{
    public interface IFilesRepository
    {
        void Add(FileModel fileModel);
        FileModel GetFile(int id);
    }
}