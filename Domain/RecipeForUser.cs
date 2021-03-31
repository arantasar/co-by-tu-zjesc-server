using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    public class RecipeForUser
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }
        [Required]
        // to do
        //public List<string> PhotosPath { get; set; } = new List<string>();
        public string PhotoPath { get; set; }
        public int Likes { get; set; }
        public int InFavourite { get; set; }

        public int ViewCounter { get; set; }
    }
}
