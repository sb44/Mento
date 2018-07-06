using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MentoratNetCore.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();

            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            CreateUsers(serviceProvider, configuration).Wait();



            // Seeding ...
        }
        private static async Task CreateUsers(IServiceProvider serviceProvider, IConfiguration configuration)
        {

        }
    }
}
