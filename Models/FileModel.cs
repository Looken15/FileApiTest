namespace TestApi.Models
{
    public class FileModel
    {
        public int Id { get; set; }
        public byte[] Content { get; set; }
        public string FileName { get; set; }
    }
}