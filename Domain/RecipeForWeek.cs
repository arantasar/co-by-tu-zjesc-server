using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain
{
    public class RecipeForWeek
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }
        public List<RecipeLine> RecipeLines { get; set; } = new List<RecipeLine>();
        public int Size { get; set; }
        public string PhotoPath { get; set; }
    }
}
