﻿using SouthChandlerCycling.Models;
using System.Collections.Generic;

namespace SouthChandlerCycling.Services
{
    public interface ISignupService
    {
        Signup AddSignup(SignupRequestData signupRequest);
        bool IsAuthorizedRider(SignupRequestData RequestData);
        bool IsAuthorizedRider(int RiderId, string Authorization);
        bool IsAuthorizedAdmin(SignupRequestData RequestData);
        bool IsAuthorizedAdmin(int AdminId, string Authorization);
        bool IsAuthorizedRiderOrAdmin(SignupRequestData RequestData);
        bool IsAuthorizedRiderOrAdmin(int RequestingId, int TargetId, string Authorization);
        bool RiderExists(long RiderId);
        bool RideExists(int RideId);
        bool SignUpExists(int RiderId, int RideId);
    }
}
