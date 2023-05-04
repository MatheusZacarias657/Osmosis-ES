using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Osmosis.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Osmosis.Services
{
    public class UserService
    {
        public static string GenerateSaltedHash(string password, string salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();
            byte[] plainText = Encoding.ASCII.GetBytes(password);
            byte[] byteSalt = Encoding.ASCII.GetBytes(salt);
            byte[] plainTextWithSaltBytes = new byte[plainText.Length + byteSalt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }

            for (int i = 0; i < byteSalt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = byteSalt[i];
            }

            return Encoding.ASCII.GetString(algorithm.ComputeHash(plainTextWithSaltBytes));
        }

        public static async void AuthorizeUser(string roleName, IHttpContextAccessor httpContextAccessor)
        {
            Claim[] claims = new[] { new Claim(ClaimTypes.Role, roleName) };
            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);
        }
    }
}
