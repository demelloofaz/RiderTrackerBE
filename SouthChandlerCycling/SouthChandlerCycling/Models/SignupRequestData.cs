﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SouthChandlerCycling.Models
{
    public class SignupRequestData
    {
        public int RiderId { get; set; }
        public string Authorization { get; set; }
        public int RideId { get; set; }
    }
}
