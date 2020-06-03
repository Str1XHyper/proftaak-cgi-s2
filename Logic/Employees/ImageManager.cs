using Logic.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class ImageManager
    {
        public static async Task SaveImage(IFormFile image, string path, string filename)
        {
            var uploads = path;
            if (image != null && image.Length > 0)
            {
                var filePath = Path.Combine(uploads, filename);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
            }
        }
        public static void DeleteImage(string filename)
        {
            string filePath = $"wwwroot/uploadedimages/{filename}";
            File.Delete(filePath);
        }
        public static string GetImageName(string imagetype)
        {
            if (imagetype != null && imagetype.Length > 0)
            {
                string imageID = GenerateAuthToken.GetUniqueKey(11);
                string filetype = '.' + imagetype.Split('/')[1];
                string filename = imageID + filetype;
                return filename;
            }
            return null;
        }
    }
}
