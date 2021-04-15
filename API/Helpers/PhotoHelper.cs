using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Persistence.Models;
using System;
using System.IO;

namespace API.Helpers
{
    public class PhotoHelper
    {
        public PhotoHelper(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            WebHostEnvironment = webHostEnvironment;
            Configuration = configuration;
            
        }

        public IWebHostEnvironment WebHostEnvironment { get; }
        private readonly IConfiguration Configuration;

        public string AddPhoto(IFormFile photo, HttpContext httpContext, string directoryNameLev1, string directoryNameLev2 = null)
        {
            string uploadsFolder = Path.Combine(WebHostEnvironment.WebRootPath, directoryNameLev1, directoryNameLev2);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            string prefix = Configuration["Host"] == "cobytuzjesc.pl" ? "https://" : "http://";

            photo.CopyTo(new FileStream(filePath, FileMode.Create));
            
            uniqueFileName = prefix + Configuration["Host"] + "/" + directoryNameLev1 + "/" + directoryNameLev2 + "/" + uniqueFileName;

            return uniqueFileName;
        }
    }
}
