using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain
{
    public class Ingredient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; }
        [Required]
        public List<Unit> Units { get; set; } = new List<Unit>();
        public List<Ingredient> Alternatives { get; set; } = new List<Ingredient>();
        public string PhotoPath { get; set; }
    }
}
