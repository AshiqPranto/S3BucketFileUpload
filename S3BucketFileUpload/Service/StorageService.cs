using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using S3BucketFileUpload.Models;
using S3Object = S3BucketFileUpload.Models.S3Object;

namespace S3BucketFileUpload.Service
{
    public class StorageService : IStorageService
    {

        public StorageService( ) 
        { 
        
        }
        public async Task<S3ResponseDto> UploadFileAsync(S3Object s3Object, AwsCredentials awsCredentials)
        {
            var credentials = new BasicAWSCredentials(awsCredentials.AccessKey, awsCredentials.SecretKey);

            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.APSoutheast1
            };
            var response = new S3ResponseDto();
            try
            {
                var uploadRequest = new TransferUtilityUploadRequest()
                {
                    BucketName = s3Object.BucketName,
                    Key = s3Object.Name,
                    InputStream = s3Object.InputStream,
                    CannedACL = S3CannedACL.NoACL
                };
                using var client = new AmazonS3Client(credentials, config);
                var transferUtility = new TransferUtility(client);
                await transferUtility.UploadAsync(uploadRequest);
                response.StatusCode = 201;
                response.Message = $"{s3Object.Name} has been uploaded successfully ";

                //Generate presigned url
                string urlString = string.Empty;
                var objMetadata = await client.GetObjectMetadataAsync(s3Object.BucketName, s3Object.Name);
                //var objMetadata = await client.GetObjectMetadataAsync("bucketfortestingfileupload", "55052b42-6252-4fbd-9060-80ce526eec6c..jpeg");
                var expirationDate = objMetadata.Expires.Add(TimeSpan.FromMinutes(1));
                response.Message += "expire date" + expirationDate + " ";
                var request = new GetPreSignedUrlRequest()
                {
                    BucketName = s3Object.BucketName,
                    Key = s3Object.Name,
                    //Expires = expirationDate
                    Expires = DateTime.UtcNow.AddMinutes(1)
                };
                urlString = client.GetPreSignedURL(request);
                response.Message += "presigned url -> " + urlString ;
            }
            catch(AmazonS3Exception ex)
            {
                response.StatusCode = (int)ex.StatusCode; 
                response.Message = ex.Message;
            }
            catch(Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message + ex.Source;
            }
            return response;
        }
    }
}
