using SouthChandlerCycling.Models;

namespace SouthChandlerCycling.Services
{
    public interface IRideService
    {
        void AddRide(RidesRequestData RequestData);
        Ride GetRide(RidesRequestData RequestData);
        bool IsAuthorizedAdmin(RidesRequestData RequestData);
        bool IsAuthorizedToEdit(RidesRequestData RequestData);
        bool IsAuthorizedRider(RidesRequestData RequestData);
    }
}