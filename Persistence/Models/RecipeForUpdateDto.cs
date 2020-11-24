using Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Persistence.Models
{
    public class RecipeForUpdateDto
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }
        [Required]
        [MaxLength(3000)]
        public string Description { get; set; }
        public List<RecipeLine> RecipeLines { get; set; }
        [Required]
        public List<Category> Categories { get; set; }
        public List<Diet> Diets { get; set; }
        public IFormFile Photo { get; set; }
        public string PrepareTime { get; set; }
        public int Size { get; set; }
    }
}

