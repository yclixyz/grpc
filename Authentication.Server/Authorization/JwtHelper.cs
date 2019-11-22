using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Server
{
    public class JwtHelper
    {
        public static string CreateAccessToken(string userName, JwtConfig jwtConfig)
        {
            var authTime = DateTime.UtcNow;

            var expiresAt = authTime.AddDays(7);

            var claims = new Claim[]
            {
                new Claim(JwtClaimTypes.Name,userName),
            };

            var keyByteArray = Encoding.ASCII.GetBytes(jwtConfig.Key);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            var jwt = new JwtSecurityToken(
                issuer: jwtConfig.Iss,
                audience: jwtConfig.Aud,
                claims: claims,
                notBefore: authTime,
                expires: expiresAt,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
