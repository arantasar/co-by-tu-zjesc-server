using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Persistence.Models
{
    public class UserForDisplayDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        [Required]
        public Role Role { get; set; }
    }
}
