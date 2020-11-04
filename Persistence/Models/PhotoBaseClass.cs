using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Models
{
    public class PhotoBaseClass
    {
        public IFormFile Photo { get; set; }
    }
}
