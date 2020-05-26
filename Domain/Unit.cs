using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Unit
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; }
    }
}