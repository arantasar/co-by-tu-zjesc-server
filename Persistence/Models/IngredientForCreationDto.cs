using Domain;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class IngredientForCreationDto
    {
        [Required]
        public string Name { get; set; }
        public List<Unit> Units { get; set; } = new List<Unit>();
        public List<Ingredient> Alternatives { get; set; } = new List<Ingredient>();
        public IFormFile Photo { get; set; }
    }
}
