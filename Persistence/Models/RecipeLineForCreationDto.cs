using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Models
{
    public class RecipeLineForCreationDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Amount { get; set; }
    }
}
