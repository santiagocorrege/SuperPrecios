using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

namespace SuperPrecios.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCookieAuthentication(this IServiceCollection services)
        {            
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath         = "/Home/Login";
                    options.LogoutPath        = "/Home/Logout";
                    options.AccessDeniedPath  = "/Home/AccessDenied";
                    options.ExpireTimeSpan    = TimeSpan.FromMinutes(30);
                    options.SlidingExpiration = true;
                    options.Cookie.Name       = "SuperPrecios.Auth";
                    options.Cookie.HttpOnly   = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite   = SameSiteMode.Lax;
                });

            return services;
        }
    }
}
