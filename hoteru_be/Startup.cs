using System;
using System.Text;
using System.Security.Claims;
using hoteru_be.Context;
using hoteru_be.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using hoteru_be.Services.Queries;
using hoteru_be.Services.Commands;

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
                        p.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
                    else
                        p.SetIsOriginAllowed(_ => false);
                    p.SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                });
            });

            var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);
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
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"],

                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name,

                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
