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
        public string DateCreated { get; set; } = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        public Role Role { get; set; } = Role.USER;
        public List<RecipeForFavourite> Favourites { get; set; } = new List<RecipeForFavourite>();
        public List<RecipeForUser> Recipes { get; set; } = new List<RecipeForUser>();
        public string PhotoPath { get; set; }
        public int ReceivedLikes { get; set; }
        public string Description { get; set; }
    }
}
