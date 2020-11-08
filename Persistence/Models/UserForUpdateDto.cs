using Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Persistence.Models
{
    public class UserForUpdateDto
    {
        [Required]
        [MaxLength(30)]
        [EmailAddress]
        public string Email { get; set; }
        public IFormFile Photo { get; set; }
        public string Description { get; set; }
    }
}
