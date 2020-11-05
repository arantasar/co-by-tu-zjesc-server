using Domain;
using Microsoft.AspNetCore.Http;
using Persistence.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class IngredientForCreationDto
    {
        [Required]
        public string Name { get; set; }
        public string Units { get; set; }
        public string Alternatives { get; set; }
        public IFormFile Photo { get; set; }
    }
}
