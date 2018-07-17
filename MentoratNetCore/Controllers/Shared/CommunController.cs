using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mentorat.Controllers.Shared
{
    public class CommunController : Controller
    {
        // GET: Commun
        public ActionResult Rediriger(string pAction, string pController, bool? pLayout)
        {
            ViewBag.ActionRedirection = pAction;
            ViewBag.ControllerRedirection = pController;
            ViewBag.LayoutRedirection = pLayout;
            return View();
        }
    }



}