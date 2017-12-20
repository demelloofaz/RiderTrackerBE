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
        // GET: All Riders Requires Authorization and a 
        public IEnumerable<Rider> GetRiders(RiderRequestData RequestData)
        {
            if (!IsAuthorizedRider(RequestData))
            {
                return null;
            }
            return _context.Riders.ToList();
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

        // Secure Details Request...
        public Rider GetRider(RiderRequestData RequestData)
        {
            Rider Result = new Rider
            {
                ID = -1
            };

            if (!IsAuthorizedRider(RequestData))
            {
                return Result;
            }

            // Use the defualt Detials method...
            // return await Details(RequestData.TargetId);
            if (RequestData.TargetId > 0)
            {
                Rider rider = _context.Riders.SingleOrDefault(r => r.ID == RequestData.TargetId);
                if (rider != null)
                {
                    Result = rider;
                }

            }
            return Result;
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
                    rider.ActiveRide = -1;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public AuthorizationResponseData CreateRider([Bind("Password,LastName,FirstName,UserName,PhoneNumber,EmailAddress")] Rider rider)
        {
            AuthorizationResponseData ResponseData = new AuthorizationResponseData
            {
                UserId = -1,
                Authorization = "Unauthorized"
            };

            try
            {
                if (ModelState.IsValid)
                {
                    if (UserNameExists(rider.UserName))
                        return ResponseData;
                    rider.Salt = Auth.GenerateSalt();
                    rider.Password = Auth.Hash(rider.Password, rider.Salt);
                    rider.LastLatitude = "";
                    rider.LastLongitude = "";
                    rider.ActiveRide = -1;
                    rider.LastRide = -1;
                    rider.Role = "User";
                    _context.Add(rider);
                    _context.SaveChangesAsync();

                    ResponseData.UserId = rider.ID;
                    ResponseData.Authorization =  Auth.GenerateJWT(rider);
                    return ResponseData;
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return ResponseData;
        }

        [HttpPost]
        public AuthorizationResponseData Login([FromBody] User user)
        {
            AuthorizationResponseData ResponseData = new AuthorizationResponseData
            {
                UserId = -1,
                Authorization = "Unauthorized"
            };
            User foundUser = _context.Riders.SingleOrDefault<Rider>(
                r => r.UserName == user.UserName && r.Password == Auth.Hash(user.Password, r.Salt)
                );
            if (foundUser != null)
            {
                ResponseData.UserId = foundUser.ID;
                ResponseData.Authorization = Auth.GenerateJWT(foundUser);
            }
   
            return ResponseData;
        }

        [HttpPost]
        public AuthorizationResponseData ChangePassword([FromBody] Rider rider)
        {
            AuthorizationResponseData ResponseData = new AuthorizationResponseData
            {
                UserId = -1,
                Authorization = "Unauthorized"
            };

            Rider foundRider = _context.Riders.SingleOrDefault<Rider>(
                r => r.UserName == rider.UserName && r.ID == rider.ID );

            if (foundRider != null)
            {
                foundRider.Password = Auth.Hash(rider.Password, foundRider.Salt);
                _context.Riders.Update(foundRider);
                _context.SaveChanges();

                ResponseData.UserId = foundRider.ID;
                ResponseData.Authorization = Auth.GenerateJWT(foundRider);
            }

            return ResponseData;
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
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRider(RiderRequestData RequestData)
        {

            if (RequestData.RequestingId == RequestData.TargetId)
            {
                if (!IsAuthorizedRider(RequestData))
                    return Unauthorized();
            }
            else if (!IsAuthorizedAdmin(RequestData))
            {
                return Unauthorized();
            }

            var riderToUpdate = await _context.Riders.SingleOrDefaultAsync(r => r.ID == RequestData.TargetId);

            if (await TryUpdateModelAsync<Rider>(
                riderToUpdate,
                "",
                r => r.FirstName, r => r.LastName, r => r.PhoneNumber, r => r.EmailAddress, r => r.Role))
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

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> UpdateRiderPosition([FromBody] RiderLocation LocationData)
        {
            if (!IsAuthorizedRider(LocationData))
            { 
                    return Unauthorized();
            }
            
             Rider  riderToUpdate = await _context.Riders.SingleOrDefaultAsync(r => r.ID == LocationData.RiderId);
            try
            {
                if (riderToUpdate != null)
                {
                    riderToUpdate.LastLatitude = LocationData.Latitude;
                    riderToUpdate.LastLongitude = LocationData.Longitude;
                    riderToUpdate.ActiveRide = LocationData.RideId;
                    _context.Riders.Update(riderToUpdate);
                    _context.SaveChanges();
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
        [ValidateAntiForgeryToken]
        public RiderLocation GetRiderLocation(RiderRequestData RequestData)
        {
            RiderLocation LocationData = new RiderLocation
            {
                RiderId = -1,
                Authorization = "",
                Latitude = "",
                Longitude = ""
        };

            if (RequestData.RequestingId == RequestData.TargetId)
            {
                if (!IsAuthorizedRider(RequestData))
                {
                    return LocationData;
                }
            }
            else
            {
               if (IsAuthorizedAdmin(RequestData))
                {
                    return LocationData;
                }
            }

            var riderToGet = _context.Riders.SingleOrDefault(r => r.ID == RequestData.TargetId);

            if (riderToGet != null)
            {
                LocationData.RiderId = riderToGet.ID;
                LocationData.Longitude = riderToGet.LastLongitude;
                LocationData.Longitude = riderToGet.LastLatitude;
                LocationData.RideId = riderToGet.ActiveRide;
            }

            return LocationData;
            
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRider(RiderRequestData RequestData)
        {
            if (RequestData.RequestingId == RequestData.TargetId)
            {
                if (!IsAuthorizedRider(RequestData))
                    return Unauthorized();
            }
            else if (!IsAuthorizedAdmin(RequestData))
            {
                return Unauthorized();
            }

            var rider = await _context.Riders.SingleOrDefaultAsync(m => m.ID == RequestData.TargetId);

            if (rider == null)
            {
                return NotFound();
            }

            // Do the delete...
            _context.Riders.Remove(rider);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            return Accepted();
        }


        private bool RiderExists(long id)
        {
            return _context.Riders.Any(e => e.ID == id);
        }

        private bool UserNameExists(string Username)
        {
            Rider foundRider = _context.Riders.SingleOrDefault<Rider>(r => r.UserName == Username);
            if (foundRider != null)
                return true;
            else
               return false;
        }

        public bool IsAuthorizedRider(RiderRequestData RequestData)
        {
            bool result = false;
            if (RiderExists(RequestData.RequestingId))
            {
                if (Auth.IsValidToken(RequestData.Authorization)) {
                    var foundRider = _context.Riders.SingleOrDefaultAsync(m => m.ID == RequestData.RequestingId && RequestData.Authorization == Auth.GenerateJWT(m));
                    if (foundRider != null)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
        public bool IsAuthorizedRider(RiderLocation RequestData)
        {
            bool result = false;
            if (RiderExists(RequestData.RiderId))
            {
                if (Auth.IsValidToken(RequestData.Authorization))
                {
                    var foundRider = _context.Riders.SingleOrDefaultAsync(m => m.ID == RequestData.RiderId && RequestData.Authorization == Auth.GenerateJWT(m));
                    if (foundRider != null)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
        public bool IsAuthorizedAdmin(RiderRequestData RequestData)
        {
            bool result = false;
            if (RiderExists(RequestData.RequestingId))
            {
                if (Auth.IsValidToken(RequestData.Authorization))
                {
                    var foundRider = _context.Riders.SingleOrDefaultAsync(m => m.ID == RequestData.RequestingId && 
                    m.Role == "Admin" &&
                    RequestData.Authorization == Auth.GenerateJWT(m));
                    if (foundRider != null)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
    }
}
