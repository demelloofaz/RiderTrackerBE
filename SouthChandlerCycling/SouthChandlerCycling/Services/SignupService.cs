using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SouthChandlerCycling.Data;
using SouthChandlerCycling.Models;




namespace SouthChandlerCycling.Services
{
    public class SignupService : ISignupService
    {
        private readonly SCCDataContext _context;
        private readonly IRiderService _riderService;
        private readonly IRideService _rideService;

        public SignupService(SCCDataContext context, IRiderService riderService, IRideService rideService)
        {
            _context = context;
            _riderService = riderService;
            _rideService = rideService;
        }
        public bool RideExists(int id)
        {
            return _riderService.RiderExists(id);
            //return _context.Rides.Any(e => e.ID == id);
        }

        public bool RiderExists(long id)
        {
            return _riderService.RiderExists(id);
            //return _context.Riders.Any(e => e.ID == id);
        }

        public bool SignUpExists(int RiderId, int RideId)
        {
            return _context.SignUps.Any(s => s.RiderID == RiderId && s.RideID == RideId);
        }
        public Signup AddSignup(SignupRequestData signupRequest)
        {
            Signup signup = new Signup();
            signup.RiderID = signupRequest.RiderId;
            signup.RideID = signupRequest.RideId;
            _context.Add(signup);
            _context.SaveChanges();
            return signup;
        }
        public bool IsAuthorizedRider(SignupRequestData RequestData)
        {
            return IsAuthorizedRider(RequestData.RequestingId, RequestData.Authorization);
        }
        public bool IsAuthorizedRider(int RiderId, string Authorization)
        {
            return _riderService.IsAuthorizedRider(RiderId, Authorization);
            
            /*
            bool result = false;
            if (RiderExists(RiderId))
            {
                if (Auth.IsValidToken(Authorization))
                {
                    Rider foundRider = _context.Riders.SingleOrDefault(m => m.ID == RiderId);

                    if (foundRider != null)
                    {
                        string UserAuth = Auth.GenerateJWT(foundRider);
                        if (Authorization == UserAuth)
                            result = true;
                    }
                }
            }
            return result;
            */
        }
        public bool IsAuthorizedAdmin(SignupRequestData RequestData)
        {
            return IsAuthorizedAdmin(RequestData.RequestingId, RequestData.Authorization);
        }

        public bool IsAuthorizedAdmin(int RiderId, string Authorization)
        {
            return _riderService.IsAuthorizedAdmin(RiderId, Authorization);
            
            /*
            bool result = false;
            if (RiderExists(RiderId))
            {
                if (Auth.IsValidToken(Authorization))
                {
                    Rider foundRider = _context.Riders.SingleOrDefault(m => m.ID == RiderId);
                    if (foundRider != null)
                    {
                        string userAuth = Auth.GenerateJWT(foundRider);
                        if ((Authorization == userAuth) &&
                           (foundRider.Role == "Admin"))
                            result = true;
                    }
                }
            }
            return result;
            */
        }

        public bool IsAuthorizedRiderOrAdmin(SignupRequestData RequestData)
        {
            return IsAuthorizedRiderOrAdmin(RequestData.RequestingId, RequestData.RiderId, RequestData.Authorization);
        }
        public bool IsAuthorizedRiderOrAdmin(int RequestingId, int RiderId, string Authorization)
        {
            return _riderService.IsAuthorizedRiderOrAdmin(RequestingId, RiderId, Authorization);
            /*
            bool result = true;
            if (RequestingId != RiderId)
            {
                if (!this.IsAuthorizedAdmin(RequestingId, Authorization))
                {
                    result = false;
                }
            }
            else if (!this.IsAuthorizedRider(RequestingId, Authorization))
            {
                result = false;
            }

            return result;
            */
        }

    }
}
