using System;

namespace TestApi.Models
{
    public class Link
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public bool IsLinkUsed { get; set; }
        public FileModel File { get; set; }
        public int FileId { get; set; }
    }
}