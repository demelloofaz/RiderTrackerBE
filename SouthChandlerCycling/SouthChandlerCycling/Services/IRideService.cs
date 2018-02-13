using SouthChandlerCycling.Models;
using System.Collections.Generic;

namespace SouthChandlerCycling.Services
{
    public interface IRideService
    {
        Ride AddRide(RidesRequestData RequestData);
        Ride GetRide(RidesRequestData RequestData);
        List<Ride> GetUpcomingRides();
        List<Ride> GetTodaysRides();
        List<Ride> GetPastRides();
        void UpdateRide(Ride rideToUpdate, RidesRequestData RequestData);
        bool IsAuthorizedAdmin(RidesRequestData RequestData);
        bool IsAuthorizedToEdit(RidesRequestData RequestData);
        bool IsAuthorizedRider(RidesRequestData RequestData);
    }
}