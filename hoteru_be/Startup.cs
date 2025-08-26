using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using hoteru_be.Context;
using hoteru_be.Services.Commands;
using hoteru_be.Services.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace hoteru_be
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MyDbContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddHttpContextAccessor();

            services.AddScoped<IReservationQueryService, ReservationQueryService>();
            services.AddScoped<IReservationCommandService, ReservationCommandService>();
            services.AddScoped<IHotelCommandService, HotelCommandService>();
            services.AddScoped<IHotelQueryService, HotelQueryService>();
            services.AddScoped<IRoomQueryService, RoomQueryService>();
            services.AddScoped<IRoomCommandService, RoomCommandService>();
            services.AddScoped<IServiceCommandService, ServiceCommandService>();
            services.AddScoped<IServiceQueryService, ServiceQueryService>();
            services.AddScoped<IUserQueryService, UserQueryService>();
            services.AddScoped<IUserCommandService, UserCommandService>();
            services.AddScoped<IGuestCommandService, GuestCommandService>();
            services.AddScoped<IGuestQueryService, GuestQueryService>();
            services.AddScoped<IRoomTypeQueryService, RoomTypeQueryService>();
            services.AddScoped<IRoomStatusQueryService, RoomStatusQueryService>();
            services.AddTransient<IEmailCommandService, EmailCommandService>();
            services.AddScoped<IGuestStatusQueryService, GuestStatusQueryService>();
            services.AddScoped<IDepositTypeQueryService, DepositTypeQueryService>();
            services.AddScoped<IUserTypeQueryService, UserTypeQueryService>();
            services.AddScoped<IDepositQueryService, DepositQueryService>();
            services.AddScoped<IAuthCommandService, AuthCommandService>();
            services.AddScoped<IPasswordHasher<Entities.User>, PasswordHasher<Entities.User>>();

            services.AddControllers();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("HasHotelId", policy =>
                    policy.RequireAuthenticatedUser()
                          .RequireClaim("hotelId"));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "hoteru_be", Version = "v1" });
                var jwt = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Bearer {token}"
                };
                c.AddSecurityDefinition("Bearer", jwt);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwt, Array.Empty<string>() } });
            });

            var allowedOrigins = Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            services.AddCors(o =>
            {
                o.AddPolicy("Default", p =>
                {
                    if (allowedOrigins.Length > 0)
                    {
                        p.WithOrigins(allowedOrigins)
                         .AllowAnyHeader()
                         .AllowAnyMethod()
                         .AllowCredentials();
                    }
                    else
                    {
                        p.SetIsOriginAllowed(_ => false);
                    }
                    p.SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                });
            });

            var keys = Configuration.GetSection("Jwt:Keys").Get<string[]>();
            if (keys == null || keys.Length == 0)
            {
                var single = Configuration["Jwt:Key"];
                if (string.IsNullOrWhiteSpace(single))
                    throw new InvalidOperationException("Configure either Jwt:Keys[] or Jwt:Key");
                keys = new[] { single };
            }

            var signingKeys = keys.Select((k, i) =>
            {
                var sk = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(k));
                sk.KeyId = $"k{i}";
                return sk;
            }).ToArray();

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = false;
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = signingKeys,

                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"],

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "hoteru_be v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("Default");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
