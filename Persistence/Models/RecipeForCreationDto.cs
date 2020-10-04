﻿using Domain;
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
        public List<RecipeLine> RecipeLines = new List<RecipeLine>();
        [Required]
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}