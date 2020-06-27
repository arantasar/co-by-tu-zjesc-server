using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Email { get; set; }
        [Required]
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public Role Role { get; set; } = Role.USER;
    }
}
