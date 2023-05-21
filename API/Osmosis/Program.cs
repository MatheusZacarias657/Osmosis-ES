using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Osmosis.Authorization;
using Osmosis.DAO.Base;

namespace Osmosis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //adição de uma instância global de banco
            builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection")));

            //adiciona o contexto HTTP da requisição pra poder capturar todas as informações que venham
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddTransient<IAuthorizationHandler, APIAuthorizationHandler>();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdmin", policy => policy.AddRequirements(new APIAuthorization()).RequireRole("admin"));
                options.AddPolicy("RequireUser", policy => policy.AddRequirements(new APIAuthorization()).RequireRole("user"));
                options.AddPolicy("AllUsers", policy => policy.AddRequirements(new APIAuthorization()).RequireRole("admin", "user"));
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

            //builder.Services.AddScoped(typeof(IBaseDAO<>), Activator.CreateInstance(typeof(BaseDAO<>)).GetType());
            builder.Services.AddScoped(typeof(IBaseDAO<>), typeof(BaseDAO<>));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            //app.UseCustomRoleRedirect();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}