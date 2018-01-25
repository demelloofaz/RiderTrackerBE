using SouthChandlerCycling.Models;

namespace SouthChandlerCycling.Services
{
    public interface IRiderService
    {
        void AddRider(Rider rider);
        AuthorizationResponseData AddRiderWAuthorization(Rider rider);
        Rider GetRider(RiderRequestData RequestData);
        RiderLocation GetRiderLocation(Rider rider);
        bool IsAuthorizedAdmin(int RiderId, string Authorization);
        bool IsAuthorizedAdmin(RiderRequestData RequestData);
        bool IsAuthorizedRider(int RiderId, string Authorization);
        bool IsAuthorizedRider(RiderRequestData RequestData);
        bool IsAuthorizedRiderOrAdmin(int RequestingId, int TargetId, string Authorization);
        bool IsAuthorizedRiderOrAdmin(RiderRequestData RequestData);
        bool RiderExists(long id);
        AuthorizationResponseData UpdatePassword(Rider rider, string NewPassword);
        void UpdateRiderPosition(Rider rider, RiderLocation LocationData);
        AuthorizationResponseData UpdateRiderProfile(Rider rider, UpdateRiderRequestData RequestData);
        bool UserNameExists(string Username);
    }
}