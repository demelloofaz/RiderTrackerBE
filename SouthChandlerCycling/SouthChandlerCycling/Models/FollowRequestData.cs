using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SouthChandlerCycling.Models
{
    public class FollowRequestData
    {
        public int RequestingId { get; set; }
        public string Authorization { get; set; }
        public int FollowingId { get; set; }
        public int FollowerId { get; set; }
        public int FollowId { get; set; }
        public FollowStateType State { get; set; }
    }
}
