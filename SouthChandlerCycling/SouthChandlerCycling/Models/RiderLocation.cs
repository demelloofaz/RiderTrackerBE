using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SouthChandlerCycling.Models
{
    public class RiderLocation
    {
        public int  RiderId { get; set; }
        public string Authorization { get; set; }
        public int RideId { get; set; } 
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
}
