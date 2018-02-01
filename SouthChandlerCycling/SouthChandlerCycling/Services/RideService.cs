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
    public class RideService : IRideService
    {
        private readonly SCCDataContext _context;
        private IRiderService _riderService;

        public RideService(SCCDataContext context, IRiderService riderService)
        {
            _context = context;
            _riderService = riderService;
        }

        public Ride AddRide(RidesRequestData RequestData)
        {
            Ride ride = new Ride();
            ride.RideName = RequestData.RideName;
            ride.Description = RequestData.Description;
            ride.Distance = RequestData.Distance;
            ride.StartDate = RequestData.RideStart;
            ride.CreatorId = RequestData.RiderId;
            _context.Add(ride);
            _context.SaveChanges();
            return ride;
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
            return _riderService.RiderExists(id);
            //return _context.Riders.Any(e => e.ID == id);
        }

        public bool IsAuthorizedRider(RidesRequestData RequestData)
        {
            return _riderService.IsAuthorizedRider(RequestData.RiderId, RequestData.Authorization);

            /*
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
            */
        }
        public bool IsAuthorizedToEdit(RidesRequestData RequestData)
        {
            bool result = false;
            if (!RideExists(RequestData.RideId))
                return result;

            Ride ride = _context.Rides.SingleOrDefault(m => m.ID == RequestData.RideId);

            if (RiderExists(RequestData.RiderId))
            {
                // 2 cases either the rider created the ride or the rider
                // has admin rights.
                if(RequestData.RiderId == ride.CreatorId)
                {
                    if (IsAuthorizedRider(RequestData))
                        result = true;
                }
                else
                {
                    if (IsAuthorizedAdmin(RequestData))
                        result = true;
                }
                /*
                if (Auth.IsValidToken(RequestData.Authorization))
                {
                    Rider foundRider = _context.Riders.SingleOrDefault(m => m.ID == RequestData.RiderId);
                    if (foundRider != null)
                    {
                        string userAuth = Auth.GenerateJWT(foundRider);
                        if (RequestData.Authorization == userAuth)
                        {
                            // A rider is allowed to edit a ride if 
                            // the rider creted the ride or the rider is an admin
                            if (foundRider.Role=="Admin")
                                result = true;
                            else
                            {
                                // get the ride and then see if the created id 
                                if (RequestData.RiderId == ride.CreatorId)
                                    result = true;
                            }
                        }
                    }
                }
                */
            }
            return result;
        }
        public bool IsAuthorizedAdmin(RidesRequestData RequestData)
        {
            return _riderService.IsAuthorizedAdmin(RequestData.RiderId, RequestData.Authorization);
            /*
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
            */
        }
        public void UpdateRide(Ride rideToUpdate, RidesRequestData RequestData)
        {
            rideToUpdate.RideName = RequestData.RideName;
            rideToUpdate.Description = RequestData.Description;
            rideToUpdate.Distance = RequestData.Distance;
            rideToUpdate.StartDate = RequestData.RideStart;

            _context.Rides.Update(rideToUpdate);
            _context.SaveChanges();
        }
    }
}
