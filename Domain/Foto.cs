using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Foto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }
}
