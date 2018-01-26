using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SouthChandlerCycling.Models
{
    public class AttendeePositionInfo
    {
        public int RiderId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
}
