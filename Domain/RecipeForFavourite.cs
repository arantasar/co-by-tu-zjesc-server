using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    public class RecipeForFavourite
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }  
        [Required]
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Diet> Diets { get; set; } = new List<Diet>();
        // to do
        //public List<string> PhotosPath { get; set; } = new List<string>();
        public string PhotoPath { get; set; }
        [ForeignKey("UserId")]
        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public int PrepareTime { get; set; }
    }
}
