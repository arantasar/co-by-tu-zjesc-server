using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistence.Models;
using System;
using System.IO;

namespace API.Helpers
{
    public class PhotoHelper
    {
        public PhotoHelper(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }

        public string AddPhoto(IFormFile photo, HttpContext httpContext, string directoryNameLev1, string directoryNameLev2 = null)
        {
            string uploadsFolder = Path.Combine(WebHostEnvironment.WebRootPath, directoryNameLev1, directoryNameLev2);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            string prefix = httpContext.Request.IsHttps ? "https://" : "http://";

            photo.CopyTo(new FileStream(filePath, FileMode.Create));
            
            uniqueFileName = prefix + httpContext.Request.Host.Value.ToString() + "/" + directoryNameLev1 + "/" + directoryNameLev2 + "/" + uniqueFileName;

            return uniqueFileName;
        }
    }
}
