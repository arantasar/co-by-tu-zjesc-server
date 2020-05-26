using Domain;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class IngredientForCreationDto
    {
        [Required]
        public string Name { get; set; }
        public List<Unit> Units { get; set; } = new List<Unit>();
        public List<Ingredient> Alternatives { get; set; } = new List<Ingredient>();
        public FormFile Photo { get; set; }
    }
}
