using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MentoratNetCore.Controllers
{
    /// <summary>
    /// SB: 2018-07-31
    /// Code pour gestion d'erreur (Mauvaise saisie URL au navigateur / liens brisés) adapté de: " https://gooroo.io/GoorooTHINK/Article/17086/Creating-Custom-Error-Pages-in-ASPNET-core-10/32407#.W2B1BtVKi70 "
    /// </summary>
    public class StatusCodeController : Controller
    {
        private readonly ILogger<AccueilController> _logger;
        public StatusCodeController(ILogger<AccueilController> logger)
        {
            _logger = logger;
        }
        // GET: /<controller>/
        [HttpGet("/StatusCode/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            var reExecute = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            _logger.LogInformation($"Unexpected Status Code: {statusCode}, OriginalPath: {reExecute.OriginalPath}");
            return View(statusCode);
        }
    }
}