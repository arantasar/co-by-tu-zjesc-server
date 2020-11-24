using Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Persistence.Models
{
    public class RecipeForCreationDto
    {
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }
        [Required]
        [MaxLength(3000)]
        public string Description { get; set; }
        public string RecipeLines { get; set; }
        [Required]
        public string Categories { get; set; }
        public string Diets { get; set; }
        public IFormFile Photo { get; set; }
        public int PrepareTime { get; set; }
        public int Size { get; set; }
    }
}
