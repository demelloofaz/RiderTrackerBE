using SouthChandlerCycling.Models;

namespace SouthChandlerCycling.Services
{
    public interface IRideService
    {
        Ride AddRide(RidesRequestData RequestData);
        Ride GetRide(RidesRequestData RequestData);
        void UpdateRide(Ride rideToUpdate, RidesRequestData RequestData);
        bool IsAuthorizedAdmin(RidesRequestData RequestData);
        bool IsAuthorizedToEdit(RidesRequestData RequestData);
        bool IsAuthorizedRider(RidesRequestData RequestData);
    }
}