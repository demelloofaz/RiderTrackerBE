using System;
using System.ComponentModel.DataAnnotations;


namespace SouthChandlerCycling.Models
{
    public class SCCRidesGroup
    {
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        public int RiderCount { get; set; }
    }
}
