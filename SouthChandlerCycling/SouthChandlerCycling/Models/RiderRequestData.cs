using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SouthChandlerCycling.Models
{
    public class RiderRequestData
    {
        public int RequestingId { get; set; }
        public string Authorization { get; set; }
        public int TargetId { get; set; }
    }
}
