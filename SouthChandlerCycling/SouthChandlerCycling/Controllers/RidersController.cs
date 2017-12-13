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
    public class RidersController : Controller
    {
        private readonly SCCDataContext _context;

        public RidersController(SCCDataContext context)
        {
            _context = context;
        }

        // GET: Riders
        public async Task<IActionResult> Index(
                                                string sortOrder,
                                                string currentFilter,
                                                string searchString,
                                                int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["UserSortParm"] = sortOrder == "User" ? "user_desc" : "User";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var riders = from r in _context.Riders
                           select r;

            if (!String.IsNullOrEmpty(searchString))
            {
                riders = riders.Where(r => r.LastName.Contains(searchString)
                                       || r.UserName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    riders = riders.OrderByDescending(r => r.LastName);
                    break;
                case "User":
                    riders = riders.OrderBy(r => r.UserName);
                    break;
                case "user_desc":
                    riders = riders.OrderByDescending(r => r.UserName);
                    break;
                default:
                    riders = riders.OrderBy(r => r.LastName);
                    break;
            }
            //return View(await riders.AsNoTracking().ToListAsync());
            int pageSize = 5;
            return View(await PaginatedList<Rider>.CreateAsync(riders.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: Riders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rider = await _context.Riders             
                .Include(s => s.Signups)
                    .ThenInclude(e => e.ActualRide)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            if (rider == null)
            {
                return NotFound();
            }

            return View(rider);
        }

        // GET: Riders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Riders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Password,LastName,FirstName,UserName,PhoneNumber,EmailAddress")] Rider rider)
        {
            try {
                if (ModelState.IsValid)
                {
                    rider.Salt = Auth.GenerateSalt();
                    rider.Password = Auth.Hash(rider.Password, rider.Salt);
                    rider.LastLatitude = "";
                    rider.LastLongitude = "";
                    rider.Role = "User";
                    _context.Add(rider);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(rider);
        }

        // GET: Riders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rider = await _context.Riders.SingleOrDefaultAsync(m => m.ID == id);
            if (rider == null)
            {
                return NotFound();
            }
            return View(rider);
        }

        // POST: Riders/Edit/5
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
            var riderToUpdate = await _context.Riders.SingleOrDefaultAsync(r => r.ID == id);
            if (await TryUpdateModelAsync<Rider>(
                riderToUpdate,
                "",
                r => r.FirstName, r => r.LastName, r => r.PhoneNumber, r=> r.EmailAddress, r =>r.Role))
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
            return View(riderToUpdate);
        }

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LastLongitude,LastLatitude,Role,LastName,FirstName,UserName,PhoneNumber,EmailAddress")] Rider rider)
        {

            // TBD fail this for now...

            return NotFound();

            if (id != rider.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rider);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RiderExists(rider.ID))
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
            return View(rider);
        }
        */
        // GET: Riders/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rider = await _context.Riders
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            if (rider == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(rider);
        }
        // POST: Riders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rider = await _context.Riders
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            if (rider == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Riders.Remove(rider);
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
        // GET: Riders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rider = await _context.Riders
                .SingleOrDefaultAsync(m => m.ID == id);
            if (rider == null)
            {
                return NotFound();
            }

            return View(rider);
        }

        // POST: Riders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rider = await _context.Riders.SingleOrDefaultAsync(m => m.ID == id);
            _context.Riders.Remove(rider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        */
        private bool RiderExists(int id)
        {
            return _context.Riders.Any(e => e.ID == id);
        }
    }
}
