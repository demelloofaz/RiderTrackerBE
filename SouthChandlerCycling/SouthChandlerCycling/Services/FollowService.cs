using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SouthChandlerCycling.Data;
using SouthChandlerCycling.Models;

namespace SouthChandlerCycling.Services
{
    public class FollowService : IFollowService
    {
        private readonly SCCDataContext _context;
        private readonly IRiderService _riderService;

        public FollowService(SCCDataContext context, IRiderService riderService)
        {
            _context = context;
            _riderService = riderService;
        }

        public bool RiderExists(long id)
        {
            return _riderService.RiderExists(id);
            //return _context.Riders.Any(e => e.ID == id);
        }

        public bool FollowExists(FollowRequestData RequestData)
        {

              return _context.Follows.Any(e => e.FollowerID == RequestData.FollowerId && e.FollowingID == RequestData.FollowingId);
        }
        public Follow AddFollow(FollowRequestData RequestData)
        {
            Follow f = new Follow();
            f.FollowingID = RequestData.FollowingId;
            f.FollowerID = RequestData.FollowerId;
            f.FollowState = FollowStateType.FollowRequested;

            // Set up  the actual follow data
            //f.FollowerRider = _riderService.GetRiderByID(f.FollowerID);
            //f.FollowingRider = _riderService.GetRiderByID(f.FollowingID);
            _context.Add(f);
            _context.SaveChanges();
            return f;
        }

        public Follow GetFollow(FollowRequestData RequestData)
        {

            Follow follow = _context.Follows.SingleOrDefault<Follow>(f => f.FollowingID == RequestData.FollowingId && f.FollowerID == RequestData.FollowerId);
            if (follow != null) { 
                return follow;
            }
            return null;
        }
        public void UpdateFollow(Follow followToUpdate, FollowRequestData RequestData)
        {

            // can only change the stats.
            followToUpdate.FollowState = RequestData.State;
            _context.Follows.Update(followToUpdate);
            _context.SaveChanges();
        }
        public bool IsAuthorizedAdmin(FollowRequestData RequestData)
        {
            return _riderService.IsAuthorizedAdmin(RequestData.RequestingId, RequestData.Authorization);
            /*
            bool result = false;
            if (RiderExists(RequestData.RequestingId))
            {
                if (Auth.IsValidToken(RequestData.Authorization))
                {
                    Rider foundRider = _context.Riders.SingleOrDefault(m => m.ID == RequestData.RequestingId);
                    if (foundRider != null)
                    {
                        string userAuth = Auth.GenerateJWT(foundRider);
                        if ((RequestData.Authorization == userAuth) &&
                            (foundRider.Role == "Admin"))
                            result = true;
                    }
                }
            }
            return result;
            */
        }
        public bool IsAuthorizedRider(FollowRequestData RequestData)
        {
            return _riderService.IsAuthorizedRider(RequestData.RequestingId, RequestData.Authorization);
            /*
            bool result = false;
            if (RiderExists(RequestData.RequestingId))
            {
                if (Auth.IsValidToken(RequestData.Authorization))
                {
                    Rider foundRider = _context.Riders.SingleOrDefault(m => m.ID == RequestData.RequestingId);
                    if (foundRider != null)
                    {
                        string userAuth = Auth.GenerateJWT(foundRider);
                        if ((RequestData.Authorization == userAuth))
                            result = true;
                    }
                }
            }
            return result;
            */
        }

        public bool IsAuthorized(FollowRequestData RequestData)
        {
            if ((RequestData.RequestingId == RequestData.FollowerId) ||
                (RequestData.RequestingId == RequestData.FollowingId)) {

                return IsAuthorizedRider(RequestData);
            }
            else
            {
                return IsAuthorizedAdmin(RequestData);
            }
        }

    }
}
