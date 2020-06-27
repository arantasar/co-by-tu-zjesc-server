using API.Options;
using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Services
{
    public class TokenHelper
    {
        public TokenHelper(JwtOptions jwtOptions)
        {
            JwtOptions = jwtOptions;
        }

        public JwtOptions JwtOptions { get; }

        public string CreateToken(User user)
        {
            var handler = new JwtSecurityTokenHandler();
            var secret = Encoding.ASCII.GetBytes(JwtOptions.Key);
            var claims = new Claim[] {
                        new Claim(ClaimTypes.Role, user.Role.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    };
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature
            );
            var expires = DateTime.Now.AddDays(JwtOptions.DaysValid);

            var token = new JwtSecurityToken(
                JwtOptions.Issuer,
                JwtOptions.Audience,
                claims,
                null,
                expires,
                signingCredentials);

            return handler.WriteToken(token);
        }
    }
}
