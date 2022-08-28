using System;
using TestApi.Models;

namespace TestApi.Interfaces
{
    public interface ILinksService
    {
        string GenerateLink(int id, string host);
        void SetUsed(int id);
        (bool, int) CheckLink(int fileId, Guid guid);
    }
}