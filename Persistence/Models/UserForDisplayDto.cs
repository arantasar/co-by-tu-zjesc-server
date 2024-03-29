﻿using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Persistence.Models
{
    public class UserForDisplayDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(30)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        public string LastLogin { get; set; }
        public string DateCreated { get; set; }
        [Required]
        public Role Role { get; set; }
        public List<RecipeForFavourite> Favourites { get; set; }
        public List<RecipeForUser> Recipes { get; set; }
        public string PhotoPath { get; set; }
        public int DaysInService { get; set; }
        public int ReceivedLikes { get; set; }
        public string Description { get; set; }
        public int RecipesAddedCount { get; set; }

    }
}
