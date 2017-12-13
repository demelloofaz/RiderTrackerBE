using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SouthChandlerCycling.Models
{
    public class Rider : User
    {
        [Display(Name = "Longitude")]
        public string LastLongitude { get; set; }
        [Display(Name = "Latitude")]
        public string LastLatitude { get; set; }

        //public ICollection<Bicycle> bicycles { get; set; }
        public ICollection<Signup> Signups { get; set; }
        // This is a collection of followers.  Some are ride buddies
        // and some are emergency contacts...
        //public ICollection<Buddy> Buddies { get; set; }
    }
}
