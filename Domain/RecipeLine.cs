using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace Domain
{
    public class RecipeLine
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Ingredient Ingredient { get; set; }
        public Unit Unit { get; set; }
        public float Amount { get; set; }

    }
}
