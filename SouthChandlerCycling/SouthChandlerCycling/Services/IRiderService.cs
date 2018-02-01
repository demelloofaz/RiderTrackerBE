using SouthChandlerCycling.Models;

namespace SouthChandlerCycling.Services
{
    public interface IRiderService
    {
        void AddRider(Rider rider);
        AuthorizationResponseData AddRiderWAuthorization(Rider rider);
        Rider GetRiderByID(int RiderId);
        Rider GetRider(RiderRequestData RequestData);
        void GetRiderLocation(Rider rider, RiderLocation LocationData);
        bool IsAuthorizedAdmin(int RiderId, string Authorization);
        bool IsAuthorizedAdmin(RiderRequestData RequestData);
        bool IsAuthorizedRider(int RiderId, string Authorization);
        bool IsAuthorizedRider(RiderRequestData RequestData);
        bool IsAuthorizedRiderOrAdmin(int RequestingId, int TargetId, string Authorization);
        bool IsAuthorizedRiderOrAdmin(RiderRequestData RequestData);
        bool RiderExists(long id);
        AuthorizationResponseData UpdatePassword(Rider rider, string NewPassword);
        void UpdateRiderLocation(Rider rider, RiderLocation LocationData);
        AuthorizationResponseData UpdateRiderProfile(Rider rider, UpdateRiderRequestData RequestData);
        bool UserNameExists(string Username);
    }
}