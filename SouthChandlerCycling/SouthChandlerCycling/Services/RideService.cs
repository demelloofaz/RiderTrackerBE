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
        public List<Ride> GetUpcomingRides()
        {
            DateTime startDate = DateTime.Now.Date;

            return _context.Rides.
                Where(r => r.StartDate >= startDate )
                 .OrderBy(r => r.StartDate)
                 .ToList();
        }
        public List<Ride> GetPastRides()
        {
            DateTime startDate = DateTime.Now.Date;

            return _context.Rides.
                Where(r => r.StartDate < startDate)
                .OrderBy(r => r.StartDate)
                .ToList();
        }
        public List<Ride> GetTodaysRides()
        {
            DateTime startDate = DateTime.Now.Date;
            DateTime endDate = startDate.AddDays(1).AddTicks(-1);

            return _context.Rides.
                Where(r => r.StartDate >= startDate && r.StartDate <= endDate)
                 .OrderBy(r => r.StartDate)
                 .ToList();
        }

        private bool RideExists(int id)
        {
            return _context.Rides.Any(e => e.ID == id);
        }

        private bool RiderExists(long id)
        {
            return _riderService.RiderExists(id);
        }

        public bool IsAuthorizedRider(RidesRequestData RequestData)
        {
            return _riderService.IsAuthorizedRider(RequestData.RiderId, RequestData.Authorization);
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
            }
            return result;
        }
        public bool IsAuthorizedAdmin(RidesRequestData RequestData)
        {
            return _riderService.IsAuthorizedAdmin(RequestData.RiderId, RequestData.Authorization);
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
