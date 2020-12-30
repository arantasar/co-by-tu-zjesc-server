using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Models
{
    public class UserWithRecipeDto
    {
        public User User { get; set; }
        public RecipeForDisplayDto Recipe { get; set; }
    }
}
