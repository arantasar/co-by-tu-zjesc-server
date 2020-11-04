using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistence.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PhotoHelper : ControllerBase
    {
        public PhotoHelper(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }

        public string AddPhoto(PhotoBaseClass photoBaseClass, string directoryNameLev1, string directoryNameLev2 = null)
        {
            string uploadsFolder = Path.Combine(WebHostEnvironment.WebRootPath, directoryNameLev1, directoryNameLev2);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + photoBaseClass.Photo.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            photoBaseClass.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
            var prefix = HttpContext.Request.IsHttps ? "https://" : "http://";
            uniqueFileName = prefix + HttpContext.Request.Host.Value.ToString() + "/" + directoryNameLev1 + "/" + directoryNameLev2 + "/" + uniqueFileName;

            return uniqueFileName;
        }
    }
}
