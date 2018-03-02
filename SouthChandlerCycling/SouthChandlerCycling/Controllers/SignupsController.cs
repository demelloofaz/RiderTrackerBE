using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SouthChandlerCycling.Data;
using SouthChandlerCycling.Models;
using SouthChandlerCycling.Services;

namespace SouthChandlerCycling.Controllers
{
    public class SignupsController : Controller
    {
        private readonly SCCDataContext _context;
        private ISignupService _service;

        public SignupsController(SCCDataContext context, ISignupService service)
        {
            _context = context;
            _service = service;
        }

        // GET: Signups
        public async Task<IActionResult> Index()
        {
            var sCCDataContext = _context.SignUps;
                //.Include(s => s.ActualRide).Include(s => s.ActualRider);
            return View(await sCCDataContext.ToListAsync());
        }

        // GET: Signups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var signup = await _context.SignUps
                .Include(s => s.ActualRide)
                .Include(s => s.ActualRider)
                .SingleOrDefaultAsync(m => m.SignupID == id);
            if (signup == null)
            {
                return NotFound();
            }

            return View(signup);
        }

        // GET: Signups/Create
        public IActionResult Create()
        {
            ViewData["RideID"] = new SelectList(_context.Rides, "ID", "Description");
            ViewData["RiderID"] = new SelectList(_context.Riders, "ID", "EmailAddress");
            return View();
        }

        // POST: Signups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SignupID,RideID,RiderID")] Signup signup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(signup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RideID"] = new SelectList(_context.Rides, "ID", "Description", signup.RideID);
            ViewData["RiderID"] = new SelectList(_context.Riders, "ID", "EmailAddress", signup.RiderID);
            return View(signup);
        }


        // GET: Signups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var signup = await _context.SignUps.SingleOrDefaultAsync(m => m.SignupID == id);
            if (signup == null)
            {
                return NotFound();
            }
            ViewData["RideID"] = new SelectList(_context.Rides, "ID", "Description", signup.RideID);
            ViewData["RiderID"] = new SelectList(_context.Riders, "ID", "EmailAddress", signup.RiderID);
            return View(signup);
        }

        // POST: Signups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SignupID,RideID,RiderID")] Signup signup)
        {
            if (id != signup.SignupID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(signup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SignupExists(signup.SignupID))
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
            ViewData["RideID"] = new SelectList(_context.Rides, "ID", "Description", signup.RideID);
            ViewData["RiderID"] = new SelectList(_context.Riders, "ID", "EmailAddress", signup.RiderID);
            return View(signup);
        }

        // GET: Signups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var signup = await _context.SignUps
                .Include(s => s.ActualRide)
                .Include(s => s.ActualRider)
                .SingleOrDefaultAsync(m => m.SignupID == id);
            if (signup == null)
            {
                return NotFound();
            }

            return View(signup);
        }

        // POST: Signups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var signup = await _context.SignUps.SingleOrDefaultAsync(m => m.SignupID == id);
            _context.SignUps.Remove(signup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SignupExists(int id)
        {
            return _context.SignUps.Any(e => e.SignupID == id);
        }

        //backend endpoints
        [HttpPost]
        public IActionResult CreateSignup([FromBody] SignupRequestData RequestData)
        {
            if (!_service.IsAuthorizedRider(RequestData))
                return Unauthorized();

            if (!_service.RiderExists(RequestData.RiderId))
                return NotFound();

            if (!_service.RideExists(RequestData.RideId))
                return NotFound();

            if (_service.SignUpExists(RequestData.RiderId, RequestData.RideId))
                return Ok();

            if (ModelState.IsValid)
            {
                var signup = _service.AddSignup(RequestData);
                return Ok(signup);
            }
            return Unauthorized();
        }
        [HttpGet]
        public IActionResult GetAllSignups(SignupRequestData RequestData)
        {
            if (!_service.IsAuthorizedRiderOrAdmin(RequestData))
                return Unauthorized();

            return Ok(_context.SignUps.ToList());
        }
        [HttpGet]
        public IActionResult GetRiderSignups(SignupRequestData RequestData)
        {
            if (!_service.IsAuthorizedRiderOrAdmin(RequestData))
            {
                return Unauthorized();
            }

            var signups = _context.SignUps.Where(signup => signup.RiderID == RequestData.RiderId).ToList();

            return Ok(signups);
        }

        [HttpGet]
        public IActionResult GetRideSignups( SignupRequestData RequestData)
        {
            if (!_service.IsAuthorizedRider(RequestData))
            {
                return Unauthorized();
            }
            return Ok(_context.SignUps.Where(signup => signup.RideID == RequestData.RideId).ToList());
        }

        [HttpGet]
        public IActionResult GetRideAttendees(SignupRequestData RequestData)
        {
            if (!_service.IsAuthorizedRider(RequestData))
            {
                return Unauthorized();
            }
            return Ok(_context.SignUps.Where(signup => signup.RideID == RequestData.RideId)
                .Select(x => new AttendeeInfo {
                    RiderId = x.ActualRider.ID,
                    FullName = x.ActualRider.FullName,
                    UserName = x.ActualRider.UserName })
                 .OrderByDescending(x => x.FullName)
                .ToList());
        }

        [HttpGet]
        public IActionResult GetRideAttendeesPositions(SignupRequestData RequestData)
        {
            if (!_service.IsAuthorizedRider(RequestData))
            {
                return Unauthorized();
            }
            return Ok(_context.SignUps.Where(signup => signup.RideID == RequestData.RideId)
                .Select(x => new AttendeePositionInfo
                {
                    RiderId = x.ActualRider.ID,
                    FullName = x.ActualRider.FullName,
                    UserName = x.ActualRider.UserName,
                    Longitude = x.ActualRider.LastLongitude,
                    Latitude = x.ActualRider.LastLatitude
                })
                 .OrderByDescending(x => x.FullName)
                .ToList());
        }

        [HttpGet]
        public IActionResult GetSignup(SignupRequestData RequestData)
        {
            if (!_service.IsAuthorizedRiderOrAdmin(RequestData))
                return Unauthorized();

            var signUp = _context.SignUps.SingleOrDefault (
                s => s.RiderID == RequestData.RiderId &&
                     s.RideID == RequestData.RideId);
                return Ok(signUp);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSignupById([FromBody] SignupRequestData RequestData)
        {
            if (!_service.IsAuthorizedRider(RequestData))
            {
                return Unauthorized();
            }

            var SignUp = await _context.SignUps.SingleOrDefaultAsync(
                s => s.SignupID == RequestData.SignupId);

            if (SignUp == null)
            {
                return NotFound();
            }

            // determine if the requestor is the rider in the sign up 
            // if not then the requestor needs to be an admin...
            if (SignUp.RiderID != RequestData.RequestingId)
            {
                if (!_service.IsAuthorizedAdmin(RequestData))
                {
                    return Unauthorized();
                }
            }

            // Do the delete...
            _context.SignUps.Remove(SignUp);
            await _context.SaveChangesAsync();
            return Ok();
        }
        public async Task<IActionResult> DeleteSignup([FromBody] SignupRequestData RequestData)
        {
            if (!_service.IsAuthorizedRiderOrAdmin(RequestData))
                return Unauthorized();

            var SignUp = await _context.SignUps.SingleOrDefaultAsync(
                s => s.RiderID == RequestData.RiderId && 
                s.RideID == RequestData.RideId);

            if (SignUp == null)
            {
                return NotFound();
            }

            // Do the delete...
            _context.SignUps.Remove(SignUp);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Delete a range of signups by Rider
        [HttpDelete]
        public IActionResult DeleteRiderSignups([FromBody] SignupRequestData RequestData)
        {
            if (!_service.IsAuthorizedRiderOrAdmin(RequestData))
                return Unauthorized();

            // verify there is at least one signup!
            var SignUp = _context.SignUps.SingleOrDefault(
                 s => s.RiderID == RequestData.RiderId);

            if (SignUp != null)
            {
                // here we do a range delete on the RiderID...
                _context.SignUps.RemoveRange(
                    _context.SignUps.Where(
                        s => s.RiderID == RequestData.RiderId
                   )
                );
                _context.SaveChangesAsync();
            }

            // Do the delete...
            return Ok();
        }

        public IActionResult DeleteRideSignups([FromBody] SignupRequestData RequestData)
        {
            if (!_service.IsAuthorizedRider(RequestData))
            {
                return Unauthorized();
            }

            // verify there is at least one signup!
            var SignUp = _context.SignUps.SingleOrDefault(
                 s => s.RideID == RequestData.RideId);

            if (SignUp != null)
            {
                // here we do a range delete on the RiderID...
                _context.SignUps.RemoveRange(
                    _context.SignUps.Where(
                        s => s.RideID == RequestData.RideId
                   )
                );
                _context.SaveChangesAsync();
            }

            // Do the delete...
            return Ok();
        }
    }
}
