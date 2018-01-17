using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SouthChandlerCycling.Models
{
    public class AuthorizationResponseData
    {
        public int UserId { get; set; }
        public string Authorization { get; set; }
        public string FirstName { get; set; }
        public string UserName  { get; set; }
        public string Role { get; set; }
    }
}
