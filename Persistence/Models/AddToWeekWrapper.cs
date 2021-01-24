using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Models
{
    public class AddToWeekWrapper
    {
        public Guid RecipeId { get; set; }
        public int? SizeFromClient { get; set; }
    }
}
