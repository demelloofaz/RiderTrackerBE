using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SouthChandlerCycling.Models
{
    public enum FollowStateType { FollowRequested, FollowAllowed, FollowDeclined, FollowBlocked};

    public class Follow
    {
        public int FollowID { get; set; }
        public int FollowingID { get; set; }
        public Rider FollowingRider { get; set; }
        public int FollowerID { get; set; }
        public Rider FollowerRider { get; set; }
        public FollowStateType FollowState { get; set; }
    }
}
