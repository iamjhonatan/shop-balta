using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Shop.Models;
using Microsoft.IdentityModel.Tokens;

namespace Shop.Services
{
    public static class TokenService
    {
        // Classe estática que gera o Token
        public static string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler(); // variável que manipulará o Token
            var key = Encoding.ASCII.GetBytes(Settings.Secret); // chave
            var tokenDescriptor = new SecurityTokenDescriptor // descrição do que terá dentro do Token
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) // Usando a chave, é gerado baseado no algoritmo Sha256
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token); // Retornando a String do token
        }
    }
}
