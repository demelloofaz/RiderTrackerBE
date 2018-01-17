using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SouthChandlerCycling.Models
{
    public class UpdateRiderRequestData
    {
        public int RequestingId { get; set; }
        public int TargetId { get; set; }
        public string Authorization { get; set; }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Role { get; set; }
    }
}
