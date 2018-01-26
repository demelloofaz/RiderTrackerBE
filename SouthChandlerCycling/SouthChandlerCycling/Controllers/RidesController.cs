using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SouthChandlerCycling.Data;
using SouthChandlerCycling.Models;
using SouthChandlerCycling.Services;

namespace SouthChandlerCycling.Controllers
{
    public class RidesController : Controller
    {
        private readonly SCCDataContext _context;
        private IRideService _service;

        public RidesController(SCCDataContext context, IRideService service)
        { 
            _context = context;
            _service = service;
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
        [HttpGet]
        public IActionResult GetAllRides( RidesRequestData RequestData)
        {
            if (!_service.IsAuthorizedRider(RequestData))
            {
                return Unauthorized();
            }
            return Ok(_context.Rides.ToList());
        }
        // Secure Details Request...
        [HttpGet]
        public IActionResult GetRide(RidesRequestData RequestData)
        {

            if (!_service.IsAuthorizedRider(RequestData))
            {
                return Unauthorized();
            }

            Ride ride = _service.GetRide(RequestData);
            // Use the defualt Detials method...
            if (ride != null)
                return Ok(ride);

            return NotFound(); ;
        }

        // GET: Rides/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ride = await _context.Rides
                            //.Include(s => s.Signups)
                            //.ThenInclude(e => e.ActualRider)
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
                ride.CreatorId = 0;
                _context.Add(ride);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ride);
        }
        [HttpPost]
        public IActionResult CreateRide([FromBody] RidesRequestData RequestData)
        {
            if (!_service.IsAuthorizedRider(RequestData))
                return Unauthorized();

            if (ModelState.IsValid)
            {
                // convert time back to local time,
                RequestData.RideStart = TimeZoneInfo.ConvertTimeFromUtc(RequestData.RideStart, TimeZoneInfo.Local);
                var ride = _service.AddRide(RequestData);
                return Accepted(ride);
            }
            return Unauthorized();
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

        [HttpPost]
        public IActionResult EditRide([FromBody] RidesRequestData RequestData)
        {
            if (!_service.IsAuthorizedToEdit(RequestData))
                return Unauthorized();

            var rideToUpdate =  _context.Rides.SingleOrDefault(m => m.ID == RequestData.RideId);

            if (rideToUpdate != null)
            {
                try
                {
                    _service.UpdateRide(rideToUpdate, RequestData);
                    return Ok();
                }

                catch (DbUpdateException /* ex */)
                {
                    return NotFound();
                }
            }
            return NotFound();
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

        [HttpDelete]
        public async Task<IActionResult> DeleteRide([FromBody] RidesRequestData RequestData)
        {
            if (!_service.IsAuthorizedToEdit(RequestData))
            {
                return Unauthorized();
            }

            var ride = await _context.Rides.SingleOrDefaultAsync(m => m.ID == RequestData.RideId);

            if (ride == null)
            {
                return NotFound();
            }

            // Do the delete...
            _context.Rides.Remove(ride);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            return Ok();
        }

    }
}
