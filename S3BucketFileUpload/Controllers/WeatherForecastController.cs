using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using S3BucketFileUpload.Models;
using S3BucketFileUpload.Service;
using S3Object = S3BucketFileUpload.Models.S3Object;

namespace S3BucketFileUpload.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration, IStorageService storageService)
        {
            _logger = logger;
            _config = configuration;
            _storageService = storageService;
        }
        [HttpPost(Name = "UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            
            var fileExt = Path.GetExtension(file.FileName);
            var docName = $"{Guid.NewGuid()}.{fileExt}";

            Models.S3Object s3Obj = new S3Object()
            {
                InputStream = memoryStream,
                BucketName = "bucketfortestingfileupload",
                Name = docName
            };
            var cred = new AwsCredentials()
            {
                AccessKey = _config["AwsConfiguration:AwsAccessKey"],
                SecretKey = _config["AwsConfiguration:AwsSecretkey"]
            };
            var result = await _storageService.UploadFileAsync(s3Obj, cred);
            return Ok(result);
        }
    }
}