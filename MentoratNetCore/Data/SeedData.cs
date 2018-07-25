using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MentoratNetCore.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                bool created = context.Database.EnsureCreated();

                if (created)
                {
                    string sqlFile = Path.Combine(serviceProvider.GetRequiredService<HostingEnvironment>().WebRootPath,
                                                  @"Content\Scripts_SQL\InsertsPortingDataBD-EF6-à-EFCore.sql");

                    string script = File.ReadAllText(sqlFile);

                    var commandStrings = Regex.Split(script, @"^\s*GO\s*$",
                                             RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    foreach (string commandString in commandStrings)
                    {
                        if (commandString.Trim() != "")
                        {
                            var numRows = context.Database.ExecuteSqlCommand(commandString);
                        }
                    }
                    

                }
            }


          //  var configuration = serviceProvider.GetRequiredService<IConfiguration>();
          //  CreateUsers(serviceProvider, configuration).Wait();



            // Seeding ...
        }
        private static async Task CreateUsers(IServiceProvider serviceProvider, IConfiguration configuration)
        {

        }
    }
}
