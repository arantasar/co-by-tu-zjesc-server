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
        public Guid Id { get; set; }
        [Required]
        [MaxLength(30)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
        public IFormFile Photo { get; set; }
        public string Description { get; set; }
    }
}
