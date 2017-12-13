using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SouthChandlerCycling.Models
{
    public class Signup
    {
        public int SignupID { get; set; }
        public int RideID { get; set; }
        public int RiderID { get; set; }

        public Ride ActualRide { get; set; }
        public Rider ActualRider { get; set; }
    }
}
