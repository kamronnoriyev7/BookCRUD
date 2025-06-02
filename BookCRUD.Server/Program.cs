using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BookCRUD.Server.Services;
// Potentially BookCRUD.Server.Models if User model is directly used here, but likely not.

namespace BookCRUD.Service // Keeping existing namespace, though BookCRUD.Server might be more consistent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Register InMemoryUserService
            builder.Services.AddSingleton<InMemoryUserService>();

            // Configure JWT authentication
            var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // For development, set to true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true, // Set to true if you want to validate the issuer
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidateAudience = true, // Set to true if you want to validate the audience
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    ValidateLifetime = true, // Validate token expiration
                    ClockSkew = TimeSpan.Zero // Optional: reduces grace period for token expiration
                };
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRequestLogging();
            app.UseHttpsRedirection();

            // Add Authentication middleware - BEFORE UseAuthorization
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}