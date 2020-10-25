using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    public class Recipe
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }
        [Required]
        [MaxLength(3000)]
        public string Description { get; set; }
        public List<RecipeLine> RecipeLines = new List<RecipeLine>();

        public string DateAdded { get; set; } = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

        public int ViewCounter { get; set; }
        public int InFavourite { get; set; }
        public int Likes { get; set; }
        [Required]
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Diet> Diets { get; set; } = new List<Diet>();
        public List<Foto> Fotos { get; set; } = new List<Foto>();

        [ForeignKey("UserId")]
        public User User { get; set; }

        public Guid UserId { get; set; }

    }
}
