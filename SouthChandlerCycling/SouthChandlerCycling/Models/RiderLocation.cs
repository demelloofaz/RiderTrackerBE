using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SouthChandlerCycling.Models
{
    public class RiderLocation
    {
        public int RequestingId { get; set; }
        public int  RiderId { get; set; }
        public string Authorization { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; } 
        public int RideId { get; set; } 
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
}
