using SouthChandlerCycling.Models;

namespace SouthChandlerCycling.Services
{
    public interface IFollowService
    {
        bool RiderExists(long id);
        bool FollowExists(FollowRequestData RequestData);
        Follow AddFollow(FollowRequestData RequestData);
        Follow GetFollow(FollowRequestData RequestData);
        void UpdateFollow(Follow followTOUpdate, FollowRequestData RequestData);
        bool IsAuthorizedAdmin(FollowRequestData RequestData);
        bool IsAuthorizedRider(FollowRequestData RequestData);
        bool IsAuthorized(FollowRequestData RequestData);
    }
}
