using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Service.Services
{
    public interface ICloudinaryService
    {
        Task<UploadResult> UploadImageAsync(IFormFile file);
    }
    public class CloudinaryService : ICloudinaryService
    {
        public async Task<UploadResult> UploadImageAsync(IFormFile file)
        {
            try
            {
                DotNetEnv.Env.Load();
                Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
                cloudinary.Api.Secure = true;
                var uploadResult = new ImageUploadResult();
                if (file.Length > 0)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(file.FileName, stream),
                            UseFilename = true,
                            UniqueFilename = false,
                            Overwrite = true,
                        };
                        uploadResult = await cloudinary.UploadAsync(uploadParams);
                    }
                }
                return uploadResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
