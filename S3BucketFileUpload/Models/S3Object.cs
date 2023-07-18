namespace S3BucketFileUpload.Models
{
    public class S3Object
    {
        public string BucketName { get; set; }
        public MemoryStream InputStream { get; set; } = null;
        public string Name { get; set; }
    }
}
