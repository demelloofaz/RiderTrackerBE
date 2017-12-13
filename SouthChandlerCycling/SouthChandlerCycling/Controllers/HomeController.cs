using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SouthChandlerCycling.Models;
using Microsoft.EntityFrameworkCore;
using SouthChandlerCycling.Data;

namespace SouthChandlerCycling.Controllers
{
    public class HomeController : Controller
    {
        // Set up the dependency injection for the DB Context
        private readonly SCCDataContext _context;

        public HomeController(SCCDataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> About()
        {
            IQueryable<SCCRidesGroup> data =
                from ride in _context.Rides
                group ride by ride.StartDate into dateGroup
                select new SCCRidesGroup()
                {
                    StartDate = dateGroup.Key,
                    RiderCount = dateGroup.Count()
                };
            return View(await data.AsNoTracking().ToListAsync());
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
