using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Persistence.Models
{
    public class RecipeForDisplayDto
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }
        [Required]
        [MaxLength(3000)]
        public string Description { get; set; }
        public List<RecipeLine> RecipeLines { get; set; }

        public string DateAdded { get; set; }

        public int ViewCounter { get; set; }
        public int InFavourite { get; set; }
        public int Likes { get; set; }
        [Required]
        public List<Category> Categories { get; set; }
        public List<Diet> Diets { get; set; }
        // to do
        //public List<string> PhotosPath { get; set; } = new List<string>(); //to samo co w Recipe.cs
        public string PhotoPath { get; set; }
        [ForeignKey("UserId")]
        public UserForRecipeDto User { get; set; }

        public Guid UserId { get; set; }
        public int PrepareTime { get; set; }
        public int Size { get; set; }

    }
}
