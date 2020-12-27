using System;
using System.ComponentModel.DataAnnotations;

namespace Persistence.Models
{
    public class UserForRecipeDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string PhotoPath { get; set; }
    }
}
