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
        [MaxLength(30)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        public string LastLogin { get; set; }
        [Required]
        public Role Role { get; set; }
        public List<Recipe> Favourites { get; set; }
        public List<Recipe> Recipes { get; set; }
        public Foto Foto { get; set; }

    }
}
