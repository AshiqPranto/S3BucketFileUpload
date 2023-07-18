using S3BucketFileUpload.Models;

namespace S3BucketFileUpload.Service
{
    public interface IStorageService
    {
        Task<S3ResponseDto> UploadFileAsync(S3Object s3Object, AwsCredentials awsCredentials);
    }
}
