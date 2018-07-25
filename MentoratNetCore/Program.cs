using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MentoratNetCore.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MentoratNetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
                        // Ajout SB pour faire nos mappings entre Model et ViewModels
            AutoMapperConfig.RegisterMappings();

            var host = BuildWebHost(args);

            // Pour seedé la BD avec code C#
            using (var scope = host.Services.CreateScope())
            {
                //var configuration = host.Services.GetRequiredService<IConfiguration>();
                var services = scope.ServiceProvider;
                try
                {
                    SeedData.Initialize(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }

    public class AutoMapperConfig
    {
        // 
        public static void RegisterMappings()
        {

                //cfg.CreateMap<MentoratNetCore.ViewModels.AssignationViewModel, MentoratNetCore.Models.MentoratInscription>();

            

            // Exemples utilisés dans le contrôleur :
            //
            //Ex1 pour mapper:  
            // Article article = _articleManager.lstArticles.FirstOrDefault(p => p.Titre.Equals(titre));
            // ArticleViewModel model = Mapper.Map<Article, ArticleViewModel>(article); // conversion d'une entité Article en ArticleViewModel
            //return View(model)

            //Ex2 pour mapper:
            // IList<Article> lArt = _articleManager.lstArticles;
            // IList<ArticleViewModel> model = Mapper.Map<IList<Article>, IList<ArticleViewModel>>(lArt);
            // return PartialView(model);

        }

    }

}
