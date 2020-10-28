using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
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
        public string LastLogin { get; set; } = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        public Role Role { get; set; } = Role.USER;
        public List<Recipe> Favourites { get; set; } = new List<Recipe>();

        public List<Recipe> Recipes = new List<Recipe>();
        public string PhotoPath { get; set; }
    }
}
