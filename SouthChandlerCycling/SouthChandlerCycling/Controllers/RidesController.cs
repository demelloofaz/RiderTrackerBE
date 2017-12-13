using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SouthChandlerCycling.Data;
using SouthChandlerCycling.Models;

namespace SouthChandlerCycling.Controllers
{
    public class RidesController : Controller
    {
        private readonly SCCDataContext _context;

        public RidesController(SCCDataContext context)
        {
            _context = context;
        }

        // GET: Rides
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["DistSortParm"] = sortOrder == "Dist" ? "dist_desc" : "Dist";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var rides = from r in _context.Rides
                           select r;

            // search the ride name or description for text
            if (!String.IsNullOrEmpty(searchString))
            {
                rides = rides.Where(r => r.RideName.Contains(searchString) || 
                r.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    rides = rides.OrderByDescending(r => r.RideName);
                    break;
                case "Date":
                    rides = rides.OrderBy(r => r.StartDate);
                    break;
                case "date_desc":
                    rides = rides.OrderByDescending(r=>r.StartDate);
                    break;
                case "Dist":
                    rides = rides.OrderBy(r => r.Distance);
                    break;
                case "dist_desc":
                    rides = rides.OrderByDescending(r => r.Distance);
                    break;
                default:
                    rides = rides.OrderBy(s => s.RideName);
                    break;
            }
            //return View(await rides.AsNoTracking().ToListAsync());
            int pageSize = 5;
            return View(await PaginatedList<Ride>.CreateAsync(rides.AsNoTracking(), page ?? 1, pageSize));

        }

        // GET: Rides/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ride = await _context.Rides
                            .Include(s => s.Signups)
                                .ThenInclude(e => e.ActualRider)
                            .AsNoTracking()
                            .SingleOrDefaultAsync(m => m.ID == id);
            if (ride == null)
            {
                return NotFound();
            }

            return View(ride);
        }

        // GET: Rides/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rides/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,RideName,Description,StartDate,Distance")] Ride ride)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ride);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ride);
        }

        // GET: Rides/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ride = await _context.Rides.SingleOrDefaultAsync(m => m.ID == id);
            if (ride == null)
            {
                return NotFound();
            }
            return View(ride);
        }

        // POST: Rides/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var rideToUpdate = await _context.Rides.SingleOrDefaultAsync(r => r.ID == id);
            if (await TryUpdateModelAsync<Ride>(
                rideToUpdate,
                "",
                r => r.RideName, r=> r.Description, r=> r.StartDate, r=> r.Distance))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(rideToUpdate);
        }
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,RideName,Description,StartDate,Distance")] Ride ride)
        {
            if (id != ride.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ride);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RideExists(ride.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ride);
        }
        */

        // GET: Rides/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ride = await _context.Rides
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            if (ride == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(ride);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ride = await _context.Rides
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            if (ride == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Rides.Remove(ride);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
        /*
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ride = await _context.Rides
                .SingleOrDefaultAsync(m => m.ID == id);
            if (ride == null)
            {
                return NotFound();
            }

            return View(ride);
        }
        
        // POST: Rides/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ride = await _context.Rides.SingleOrDefaultAsync(m => m.ID == id);
            _context.Rides.Remove(ride);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        */
        private bool RideExists(int id)
        {
            return _context.Rides.Any(e => e.ID == id);
        }
    }
}
