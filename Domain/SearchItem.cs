using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain
{
    public class SearchItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }
    }
}
