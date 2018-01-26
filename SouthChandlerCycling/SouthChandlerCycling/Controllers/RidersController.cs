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
    public class RidersController : Controller
    {
       
        private readonly SCCDataContext _context;
        private IRiderService _service;

        public RidersController(SCCDataContext context, IRiderService service)
        {
            _context = context;
            _service = service;
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
        // GET: All Riders Requires Authorization and a 
        
        [HttpGet]
        public IActionResult GetRiders(RiderRequestData RequestData)
        {
            if (!_service.IsAuthorizedRider(RequestData))
            {
                return Unauthorized();
            }
            return Ok(_context.Riders.ToList());
        }
        // GET: Riders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var rider = await _context.Riders             
                //.Include(s => s.Signups)
                    //.ThenInclude(e => e.ActualRide)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            if (rider == null)
            {
                return NotFound();
            }

            return View(rider);
        }

        // Secure Details Request...
        [HttpGet]
        public IActionResult GetRider(RiderRequestData RequestData)
        {
            if (!_service.IsAuthorizedRider(RequestData))
            {
                return Unauthorized();
            }

            Rider rider = _service.GetRider(RequestData);
            if (rider != null)
                return Ok(rider);

            return NotFound() ;
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
                    _service.AddRider(rider);
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

        [HttpPost]
        public IActionResult Register([FromBody] Rider rider)
        {
            try
            {
                //if (ModelState.IsValid)
                {
                    if (_service.UserNameExists(rider.UserName))
                        return Unauthorized();

                    // A valid new rider to be added
                    AuthorizationResponseData ResponseData = _service.AddRiderWAuthorization(rider);
                    return Ok(ResponseData);
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return BadRequest("Unable to save changes. Try again.");
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequestData user)
        {
            User foundUser = _context.Riders.SingleOrDefault<Rider>(
                r => r.UserName == user.UserName && r.Password == Auth.Hash(user.Password, r.Salt)
                );
            if (foundUser != null)
            {
                AuthorizationResponseData ResponseData = new AuthorizationResponseData();
                ResponseData.UserId = foundUser.ID;
                ResponseData.Authorization = Auth.GenerateJWT(foundUser);
                ResponseData.FirstName = foundUser.FirstName;
                ResponseData.UserName = foundUser.UserName;
                ResponseData.Role = foundUser.Role;
                return Ok(ResponseData);
            }
   
            return NotFound();
        }

        [HttpPost]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest RequestData)
        {
            if (!_service.IsAuthorizedRiderOrAdmin(
                RequestData.RequestingId,
                RequestData.TargetId,
                RequestData.Authorization))
                return Unauthorized();


            Rider foundRider = _context.Riders.SingleOrDefault<Rider>(
                r =>  r.ID == RequestData.TargetId );

            if (foundRider != null)
            {
                AuthorizationResponseData ResponseData = _service.UpdatePassword(foundRider, RequestData.Password);
                return Ok(ResponseData);
            }

            return NotFound();
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
        [HttpPost]
        public IActionResult EditRider([FromBody] UpdateRiderRequestData RequestData)
        {
            if (!_service.IsAuthorizedRiderOrAdmin(
                    RequestData.RequestingId,
                    RequestData.TargetId,
                    RequestData.Authorization))
                return Unauthorized();

            var riderToUpdate = _context.Riders.SingleOrDefault(r => r.ID == RequestData.TargetId);

            if (riderToUpdate != null)
            {
                AuthorizationResponseData ResponseData = _service.UpdateRiderProfile(riderToUpdate, RequestData);
                return Ok(ResponseData);
            }
            return NotFound();
        }

        [HttpPost]
        public async  Task<IActionResult> UpdateRiderPosition([FromBody] RiderLocation LocationData)
        {
            if (!_service.IsAuthorizedRider(LocationData.RiderId, LocationData.Authorization))
            { 
                    return Unauthorized();
            }
            
             Rider  riderToUpdate = await _context.Riders.SingleOrDefaultAsync(r => r.ID == LocationData.RiderId);
            try
            {
                if (riderToUpdate != null)
                {
                    _service.UpdateRiderPosition(riderToUpdate, LocationData);
                }
            }

 
            catch (DbUpdateException)
            {
                    //Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
            }
            return Accepted();
        }
        [HttpGet]
        public IActionResult GetRiderPosition(RiderRequestData RequestData)
        {
 
            if (!_service.IsAuthorizedRiderOrAdmin(RequestData))
                return Unauthorized();

            Rider riderToGet = _context.Riders.SingleOrDefault(r => r.ID == RequestData.TargetId);

            if (riderToGet != null)
            {
                RiderLocation LocationData = _service.GetRiderLocation(riderToGet);
                return Ok(LocationData);

            }

            return NotFound();
            
        }
        private async Task<IActionResult> SetActiveRide(RiderRequestData RequestData)
        {
            if (!_service.IsAuthorizedRider(RequestData))
            {
                return Unauthorized();
            }
            var riderToUpdate = await _context.Riders.SingleOrDefaultAsync(r => r.ID == RequestData.RequestingId);

            if (await TryUpdateModelAsync<Rider>(
                riderToUpdate,
                "",
                 r => r.ActiveRide))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return Accepted();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return NotFound();

        }

        public async Task<IActionResult> ActivateRide(RiderRequestData RequestData)
        {
            return await SetActiveRide(RequestData);
        }
        public async Task<IActionResult> DeactivateRide(RiderRequestData RequestData)
        {
            RequestData.ActiveRide = 0;
            return await SetActiveRide(RequestData);
        }

        public IActionResult GetActiveRide(RiderRequestData RequestData)
        {
            if (!_service.IsAuthorizedRider(RequestData))
            {
                return Unauthorized();
            }
            Rider rider = _service.GetRider(RequestData);
            if (rider != null)
                return Ok(rider.ActiveRide);

            return NotFound();
        }

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

        [HttpDelete]
        public async Task<IActionResult> DeleteRider([FromBody] RiderRequestData RequestData)
        {
            if (!_service.IsAuthorizedRiderOrAdmin(RequestData))
                return Unauthorized();

            var rider = await _context.Riders.SingleOrDefaultAsync(m => m.ID == RequestData.TargetId);

            if (rider == null)
            {
                return NotFound();
            }

            // Do the delete...
            _context.Riders.Remove(rider);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
