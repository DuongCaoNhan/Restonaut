using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using APIs.Services;
using APIs.Entities.Extensions;
using APIs.Entities.Data;

namespace APIs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Entity Framework services with In-Memory database
            builder.Services.AddEntitiesServicesWithInMemoryDatabase("AppDb");
              // Configure JWT authentication using user secrets
            var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured in user secrets");
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured in user secrets");
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured in user secrets");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            builder.Services.AddAuthorization();
            builder.Services.AddIdentityApiEndpoints<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();            // Register configuration service
            builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
            
            // Register additional services that use user secrets
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IStorageService, AzureBlobStorageService>();
            
            // Register JWT token service
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

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

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapIdentityApi<IdentityUser>();
            app.MapSwagger().RequireAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
