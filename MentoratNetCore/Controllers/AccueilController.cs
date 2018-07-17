using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MentoratNetCore.Models;

namespace MentoratNetCore.Controllers
{
    public class AccueilController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Message = "Bienvenue au service de mentorat de la CSC!";

            return View();
        }

        public IActionResult Inscription()
        {
            ViewBag.Message = "Formulaire pour s'inscrire.";

            return View();
        }

        public IActionResult Mentors()
        {
            ViewBag.Message = "Les profils.";

            return View();
        }

        public ActionResult Fonctionnement()
        {
            ViewBag.Message = "Les profils.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
