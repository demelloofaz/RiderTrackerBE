using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SouthChandlerCycling.Data;
using SouthChandlerCycling.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace SouthChandlerCycling.Services
{
    public class RideService
    {
        private readonly SCCDataContext _context;

        public RideService(SCCDataContext context)
        {
            _context = context;
        }

        public void AddRide(RidesRequestData RequestData)
        {
            Ride ride = new Ride();
            ride.RideName = RequestData.RideName;
            ride.Description = RequestData.Description;
            ride.Distance = RequestData.Distance;
            ride.StartDate = RequestData.RideStart;
            _context.Add(ride);
            _context.SaveChanges();
        }

       public Ride GetRide(RidesRequestData RequestData)
        {
            if (RequestData.RideId > 0)
            {
                Ride ride = _context.Rides.SingleOrDefault<Ride>(r => r.ID == RequestData.RideId);
                return ride;
            }
            return null;
        }

        private bool RideExists(int id)
        {
            return _context.Rides.Any(e => e.ID == id);
        }

        private bool RiderExists(long id)
        {
            return _context.Riders.Any(e => e.ID == id);
        }

        public bool IsAuthorizedRider(RidesRequestData RequestData)
        {
            bool result = false;
            if (RiderExists(RequestData.RiderId))
            {
                if (Auth.IsValidToken(RequestData.Authorization))
                {
                    Rider foundRider = _context.Riders.SingleOrDefault(m => m.ID == RequestData.RiderId);
                    if (foundRider != null)
                    {
                        string userAuth = Auth.GenerateJWT(foundRider);
                        if (RequestData.Authorization == userAuth)
                            result = true;
                    }
                }
            }
            return result;
        }
        public bool IsAuthorizedAdmin(RidesRequestData RequestData)
        {
            bool result = false;
            if (RiderExists(RequestData.RiderId))
            {
                if (Auth.IsValidToken(RequestData.Authorization))
                {
                    Rider foundRider = _context.Riders.SingleOrDefault(m => m.ID == RequestData.RiderId);
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
        }
    }
}
