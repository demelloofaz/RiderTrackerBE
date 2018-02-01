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
    public class FollowsController : Controller
    {
        private readonly SCCDataContext _context;
        private IFollowService _service;

        public FollowsController(SCCDataContext context, IFollowService service)
        {
            _context = context;
            _service = service;
        }

        // GET: Follows
        public async Task<IActionResult> Index()
        {
            return View(await _context.Follows.ToListAsync());
        }

        // GET: Follows/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var follow = await _context.Follows
                .SingleOrDefaultAsync(m => m.FollowID == id);
            if (follow == null)
            {
                return NotFound();
            }

            return View(follow);
        }

        // GET: Follows/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Follows/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FollowingId,FollowerId,FollowState")] Follow follow)
        {
            if (ModelState.IsValid)
            {
                _context.Add(follow);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(follow);
        }

        // GET: Follows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var follow = await _context.Follows.SingleOrDefaultAsync(m => m.FollowID == id);
            if (follow == null)
            {
                return NotFound();
            }
            return View(follow);
        }

        // POST: Follows/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FollowingId,FollowerId,FollowState")] Follow follow)
        {
            if (id != follow.FollowID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(follow);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FollowExists(follow.FollowID))
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
            return View(follow);
        }

        // GET: Follows/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var follow = await _context.Follows
                .SingleOrDefaultAsync(m => m.FollowID == id);
            if (follow == null)
            {
                return NotFound();
            }

            return View(follow);
        }

        // POST: Follows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var follow = await _context.Follows.SingleOrDefaultAsync(m => m.FollowID == id);
            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FollowExists(int id)
        {
            return _context.Follows.Any(e => e.FollowID == id);
        }

        // Backend methods start here...
        //backend endpoints
        [HttpPost]
        public IActionResult CreateFollow([FromBody] FollowRequestData RequestData)
        {

            if (!_service.IsAuthorizedRider(RequestData))
                return Unauthorized();

            if (!_service.RiderExists(RequestData.FollowerId))
                return NotFound();
            if (!_service.RiderExists(RequestData.FollowingId))
                return NotFound();

            if (_service.FollowExists(RequestData))
            {
                return Ok(_service.GetFollow(RequestData));
            }

            if (ModelState.IsValid)
            {
                var follow = _service.AddFollow(RequestData);
                return Ok(follow);
            }
            return Unauthorized();
        }
        [HttpGet]
        public IActionResult GetAllFollowers(FollowRequestData RequestData)
        {
            if (!_service.IsAuthorized(RequestData))
                return Unauthorized();

            return Ok(_context.Follows.ToList());
        }
        [HttpGet]
        public IActionResult GetMyFollowers(FollowRequestData RequestData)
        {
            if (!_service.IsAuthorized(RequestData))
            {
                return Unauthorized();
            }

            var follows = _context.Follows.Where(f => f.FollowingID == RequestData.FollowingId).ToList();

            return Ok(follows);
        }

        public IActionResult GetMyFollowing(FollowRequestData RequestData)
        {
            if (!_service.IsAuthorized(RequestData))
            {
                return Unauthorized();
            }

            var follows = _context.Follows.Where(f => f.FollowerID == RequestData.FollowerId).ToList();

            return Ok(follows);
        }

        public IActionResult GetFollow(FollowRequestData RequestData)
        {
            if (!_service.IsAuthorized(RequestData))
            {
                return Unauthorized();
            }

            var follows = _service.GetFollow(RequestData);
            return Ok(follows);
        }
        public IActionResult GetFollowById(FollowRequestData RequestData)
        {
            if (!_service.IsAuthorized(RequestData))
            {
                return Unauthorized();
            }

            var follows = _context.Follows.Where(f => f.FollowID  == RequestData.FollowId).SingleOrDefault();

            return Ok(follows);
        }

        [HttpPost]
        public IActionResult EditFollow([FromBody] FollowRequestData RequestData)
        {
            if (!_service.IsAuthorized(RequestData))
                return Unauthorized();

            var followToUpdate = _context.Follows.SingleOrDefault(f =>f.FollowID == RequestData.FollowId);

            if (followToUpdate != null)
            {
                try
                {
                    _service.UpdateFollow(followToUpdate, RequestData);
                    return Ok();
                }

                catch (DbUpdateException /* ex */)
                {
                    return NotFound();
                }
            }
            return NotFound();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFollowById([FromBody] FollowRequestData RequestData)
        {
            if (!_service.IsAuthorized(RequestData))
            {
                return Unauthorized();
            }

            var follow = await _context.Follows.SingleOrDefaultAsync(
                f => f.FollowID == RequestData.FollowId);

            if (follow == null)
            {
                return NotFound();
            }

            // Do the delete...
            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
            return Ok();
        }
        public async Task<IActionResult> DeleteFollow([FromBody] FollowRequestData RequestData)
        {
            if (!_service.IsAuthorized(RequestData))
                return Unauthorized();

            var follow = await _context.Follows.SingleOrDefaultAsync(
                f =>f.FollowingID == RequestData.FollowingId &&
                f.FollowerID == RequestData.FollowerId);

            if (follow == null)
            {
                return NotFound();
            }

            // Do the delete...
            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Delete a range of follows by Following
        [HttpDelete]
        public IActionResult DeleteMyFollowers([FromBody] FollowRequestData RequestData)
        {
            if (!_service.IsAuthorizedRider(RequestData))
                return Unauthorized();

            // verify there is at least one signup!
            var follow = _context.Follows.SingleOrDefault(
                 f => f.FollowingID == RequestData.FollowingId);

            if (follow != null)
            {
                // here we do a range delete on the RiderID...
                _context.Follows.RemoveRange(
                    _context.Follows.Where(
                        f => f.FollowingID == RequestData.FollowingId
                   )
                );
                _context.SaveChangesAsync();
            }
            return Ok();
        }
        // Delete a range of follows by Follower
        [HttpDelete]
        public IActionResult DeleteMyFollowings([FromBody] FollowRequestData RequestData)
        {
            if (!_service.IsAuthorizedRider(RequestData))
                return Unauthorized();

            // verify there is at least one signup!
            var follow = _context.Follows.SingleOrDefault(
                 f => f.FollowerID == RequestData.FollowerId);

            if (follow != null)
            {
                // here we do a range delete on the RiderID...
                _context.Follows.RemoveRange(
                    _context.Follows.Where(
                        f => f.FollowerID == RequestData.FollowerId
                   )
                );
                _context.SaveChangesAsync();
            }
            return Ok();
        }
    }
}
