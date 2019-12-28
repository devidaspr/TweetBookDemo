using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TweetBook.Data;
using TweetBook.Services;

namespace TweetBook.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            //Connection string to be used for running the app in a dokcer container
            //services.AddDbContext<DataContext>(options =>
            //    options.UseSqlServer(
            //        configuration.GetConnectionString("DefaultConnectionForDocker")));

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DataContext>();

            services.AddScoped<IPostService, PostService>();
            //services.AddSingleton<IPostService, CosmosPostService>();
        }
    }
}
