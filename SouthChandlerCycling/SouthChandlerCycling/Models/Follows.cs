using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SouthChandlerCycling.Models
{
    public enum FollowStateType { FollowRequested, FollowAllowed, FollowDeclined, FollowBlocked};

    public class Follows
    {
        int FollowsId;
        public int FollowingId;
        Rider FollowingRider;
        public int FollowerId;
        Rider FollowerRider;
        FollowStateType FollowsState;
    }
}
