using SouthChandlerCycling.Models;
using System;
using System.Collections.Generic;

namespace SouthChandlerCycling.Services
{
    public interface IRideService
    {
        Ride AddRide(RidesRequestData RequestData);
        Ride GetRide(RidesRequestData RequestData);
        List<Ride> GetUpcomingRides(RidesRequestData RequestData);
        List<Ride> GetTodaysRides(RidesRequestData RequestData);
        List<Ride> GetPastRides(RidesRequestData RequestData);
        void UpdateRide(Ride rideToUpdate, RidesRequestData RequestData);
        bool IsAuthorizedAdmin(RidesRequestData RequestData);
        bool IsAuthorizedToEdit(RidesRequestData RequestData);
        bool IsAuthorizedRider(RidesRequestData RequestData);
    }
}