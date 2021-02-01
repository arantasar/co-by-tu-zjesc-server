using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain
{
    public class ResultItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }
        public int PrepareTime { get; set; }
        [Required]
        [MaxLength(3000)]
        public string Description { get; set; }
        public List<Ingredient> MissingIngredients { get; set; }
        public Guid RecipeId { get; set; }
        public string PhotoPath { get; set; }
    }
}
