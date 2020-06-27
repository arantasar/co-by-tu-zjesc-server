using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Options
{
    public class JwtOptions
    {
        public const string Jwt = "Jwt";
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int DaysValid { get; set; }
    }
}
