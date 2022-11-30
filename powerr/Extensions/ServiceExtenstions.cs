using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using powerr.Api.Models.Entities.User;
using powerr.Api.repository;
using powerr.Interfaces;
using powerr.repository;
using System.Text;

namespace powerr.Extensions
{
    public static class ServiceExtenstions 

    {
        //cors
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy(name: "power_cors_policy", policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            ));
        }

        //RepositoryManager
        public static void ConfigureRepository(this IServiceCollection services)
        {
            services.AddScoped<IMeterRequestRepository, MeterRequestRepository>();
            services.AddScoped<IMeterRepository, MeterRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IRechargeTokenRepository, RechargeTokenRepository>();
        }

        //dbcontext 
        public static void ConfigureDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string not found")));
        }

        // identity
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.User.RequireUniqueEmail = true; 
                
            })
            .AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();

        }

        //configure jwt 
        public static void ConfigureJwtAuthentication (this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(options =>
             {
                 options.SaveToken = true;
                 options.RequireHttpsMetadata = false;
                 options.TokenValidationParameters = new TokenValidationParameters()
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidAudience = configuration["JWT:ValidAudience"],
                     ValidIssuer = configuration["JWT:ValidIssuer"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                 };
             });
        }
    }
}
