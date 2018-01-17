using SouthChandlerCycling.Data;
using SouthChandlerCycling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SouthChandlerCycling.Services
{
    public class RiderService : IRiderService
    {
        private readonly SCCDataContext _context;

        public RiderService(SCCDataContext context)
        {
            _context = context;
        }
        public void AddRider(Rider rider)
        {
            rider.Salt = Auth.GenerateSalt();
            rider.Password = Auth.Hash(rider.Password, rider.Salt);
            rider.LastLatitude = "";
            rider.LastLongitude = "";
            rider.ActiveRide = -1;
            rider.LastRide = -1;
            rider.Role = "User";
            _context.Add(rider);
            _context.SaveChanges();
        }
        public AuthorizationResponseData  AddRiderWAuthorization(Rider newRider)
        {
            AuthorizationResponseData ResponseData = new AuthorizationResponseData();

            /*
            Rider newRider = new Rider();
            newRider.LastName = rider.LastName;
            newRider.Password = rider.Password;
            newRider.FirstName = rider.FirstName;
            newRider.UserName = rider.UserName;
            newRider.PhoneNumber = rider.PhoneNumber;
            newRider.EmailAddress = rider.EmailAddress;
            */

            AddRider(newRider);
            Rider riderToGet = _context.Riders.SingleOrDefault(r => r.UserName == newRider.UserName);
            ResponseData.UserId = riderToGet.ID;
            ResponseData.FirstName = riderToGet.FirstName;
            ResponseData.UserName = riderToGet.UserName;
            ResponseData.Role = riderToGet.Role;
            ResponseData.Authorization = Auth.GenerateJWT(newRider);
            return ResponseData;
        }
        public Rider GetRider(RiderRequestData RequestData)
        {
            // Use the defualt Detials method...
            // return await Details(RequestData.TargetId);
            if (RequestData.TargetId > 0)
            {
                Rider rider = _context.Riders.SingleOrDefault(r => r.ID == RequestData.TargetId);
                return rider;
            }
            return null;
        }
        public AuthorizationResponseData UpdatePassword(Rider rider, string NewPassword)
        {
            AuthorizationResponseData ResponseData = new AuthorizationResponseData();
            rider.Password = Auth.Hash(NewPassword, rider.Salt);
            _context.Riders.Update(rider);
            _context.SaveChanges();

            ResponseData.UserId = rider.ID;
            ResponseData.FirstName = rider.FirstName;
            ResponseData.UserName = rider.UserName;
            ResponseData.Role = rider.Role;
            ResponseData.Authorization = Auth.GenerateJWT(rider);
            return ResponseData;
        }
        public void UpdateRiderPosition(Rider rider, RiderLocation LocationData)
        {
            rider.LastLatitude = LocationData.Latitude;
            rider.LastLongitude = LocationData.Longitude;
            rider.ActiveRide = LocationData.RideId;
            _context.Riders.Update(rider);
            _context.SaveChanges();
        }
        public RiderLocation GetRiderLocation(Rider rider)
        {
            RiderLocation LocationData = new RiderLocation();
            LocationData.RiderId = rider.ID;
            LocationData.Longitude = rider.LastLongitude;
            LocationData.Latitude = rider.LastLatitude;
            LocationData.RideId = rider.ActiveRide;
            return LocationData;
        }
        public AuthorizationResponseData UpdateRiderProfile(Rider rider, UpdateRiderRequestData requestData)
        {
            AuthorizationResponseData ResponseData = new AuthorizationResponseData();
            rider.FirstName = requestData.FirstName;
            rider.LastName = requestData.LastName;

            // only change the role if found in the request.
            if (requestData.Role != "")
                rider.Role = requestData.Role;

            // not going to allow anyone to change username!!!

            rider.PhoneNumber = requestData.PhoneNumber;
            rider.EmailAddress = requestData.EmailAddress;

            // save to database
            _context.Riders.Update(rider);
            _context.SaveChanges();

            // set up the response data 
            ResponseData.UserId = rider.ID;
            ResponseData.FirstName = rider.FirstName;
            ResponseData.UserName = rider.UserName;
            ResponseData.Role = rider.Role;
            ResponseData.Authorization = Auth.GenerateJWT(rider);
            return ResponseData;

        }

        public bool RiderExists(long id)
        {
            return _context.Riders.Any(e => e.ID == id);
        }

        public bool UserNameExists(string Username)
        {
            Rider foundRider = _context.Riders.SingleOrDefault<Rider>(r => r.UserName == Username);
            if (foundRider != null)
                return true;
            else
                return false;
        }

        public bool IsAuthorizedRider(RiderRequestData RequestData)
        {
            return IsAuthorizedRider(RequestData.RequestingId, RequestData.Authorization);
        }

        public bool IsAuthorizedRider(int RiderId, string Authorization)
        {
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
        }
        public bool IsAuthorizedAdmin(RiderRequestData RequestData)
        {
            return IsAuthorizedAdmin(RequestData.RequestingId, RequestData.Authorization);
        }

        public bool IsAuthorizedAdmin(int RiderId, string Authorization)
        {
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
        }
    }
}
