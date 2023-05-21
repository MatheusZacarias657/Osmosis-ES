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
    public class OAuthService
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
        
        public static async void SendEmail(IConfiguration configuration, Users user, List<Users> admins)
        {
            try
            {
                Uri url = new Uri(configuration.GetValue<string>("Sendingblue:Url"));
                HttpClient request = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });

                foreach (Users admin in admins)
                {
                    EmailRequest body = new EmailRequest()
                    {
                        sender = new Email() { email = configuration.GetValue<string>("Sendingblue:Email"), name = configuration.GetValue<string>("Sendingblue:Name") },
                        to = new List<Email> { new Email { email = admin.email, name = admin.name } },
                        subject = configuration.GetValue<string>("Sendingblue:Subject"),
                        htmlContent = string.Format(configuration.GetValue<string>("Sendingblue:Content"), admin.name, user.login)
                    };

                    StringContent content = new StringContent(JsonSerializer.Serialize(body));
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    request.DefaultRequestHeaders.Add("api-key", configuration["email-api-key"]);

                    HttpResponseMessage response = await request.PostAsync(url, content);
                    Stream streamData = await response.Content.ReadAsStreamAsync();

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"A API da retornou um erro:\n {response.ToString()}");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
