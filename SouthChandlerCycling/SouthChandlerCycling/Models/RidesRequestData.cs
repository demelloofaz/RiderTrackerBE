using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SouthChandlerCycling.Models
{
    public class RidesRequestData
    {
        public int RiderId { get; set; }
        public int RideId { get; set; }
        public string Authorization { get; set; }
        public string RideName { get; set; }
        public string Description { get; set; }
        public DateTime RideStart { get; set; }
        public double Distance { get; set; }
        public int TargetMonth { get; set; }
        public int TargetDay { get; set; }
        public int TargetYear { get; set; }
    }
}
