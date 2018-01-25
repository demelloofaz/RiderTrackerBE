using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SouthChandlerCycling.Models;


namespace SouthChandlerCycling.Data
{
    public class DBInit
    {
        public static void Initialize(SCCDataContext context)
        {
            context.Database.EnsureCreated();

            // prevent multiple inits of the DB...
            if (context.Riders.Any())
                return;
            string AdminSalt = Auth.GenerateSalt();
            string AdminPassword = Auth.Hash("Pavio6463", AdminSalt);

            string DefaultSalt = Auth.GenerateSalt();
            string DefaultPassword = Auth.Hash("Password", DefaultSalt);
            // add data!!!
            var riders = new Rider[] {
            new Rider{FirstName ="The", LastName="Admin", PhoneNumber="(480) 555-1212", EmailAddress="none@msn.com", UserName="Admin", Role="Admin", Salt = AdminSalt, Password = AdminPassword, ActiveRide = -1, LastRide = -1 },

            new Rider{FirstName ="David", LastName="Mello", PhoneNumber="(480) 313-3875", EmailAddress="DavidMelloofaz@gmail.com",UserName="DavidMelloOfAz", Role="Admin",
            Salt = AdminSalt, Password = AdminPassword, ActiveRide = -1, LastRide = -1},

            new Rider{FirstName ="Rich", LastName="Linde", PhoneNumber="(303) 330-5757", EmailAddress="RichLinde@gmail.com",UserName="BigTex", Role="User", Salt = DefaultSalt, Password = DefaultPassword, ActiveRide = -1, LastRide = -1  },

            new Rider{FirstName ="Ron", LastName="Sprague", PhoneNumber="(480) 555-1234", EmailAddress="RonSprague@gmail.com",UserName="CaptainRon", Role="User", Salt = DefaultSalt, Password = DefaultPassword, ActiveRide = -1, LastRide = -1},

            new Rider{FirstName ="Damien", LastName="Smith", PhoneNumber="(469) 964-3424", EmailAddress="DamiemSmith@gmail.com",UserName="Damien", Role="User", Salt = DefaultSalt, Password = DefaultPassword, ActiveRide = -1, LastRide = -1  },

            new Rider{FirstName ="Steve", LastName="Van Cott", PhoneNumber="(480) 555-6789", EmailAddress="BigSteve@gmail.com",UserName="BigSteve", Role="User", Salt = DefaultSalt, Password = DefaultPassword, ActiveRide = -1, LastRide = -1  },

             new Rider{FirstName ="Steve", LastName="Zimmerman", PhoneNumber="(480) 555-8989", EmailAddress="SteveZ@gmail.com",UserName="LittleSteve", Role="User", Salt = DefaultSalt, Password = DefaultPassword, ActiveRide = -1, LastRide = -1  }};

            foreach (Rider r in riders)
            {
                context.Riders.Add(r);
            }
            context.SaveChanges();

            var rides = new Ride[]
            {
                new Ride{RideName="Test", Description="Test", Distance = 10.3, StartDate=DateTime.Parse("2017-12-23T07:30:00.0"), CreatorId = 0 },

                new Ride{RideName="Beginner Ride", Description="Easy flat ride", Distance = 26.1, StartDate=DateTime.Parse("2017-12-23T07:30:00.0"), CreatorId = 0 },

                new Ride{RideName="Red Mountain/Las Sendas", Description="Hilly Ride Thru Las Sendas Area", Distance = 50.3, StartDate=DateTime.Parse("2017-12-23T07:30:00.0"), CreatorId = 0 },
            };

            foreach (Ride r in rides)
            {
                context.Rides.Add(r);
            }
            context.SaveChanges();

            var signUps = new Signup[]
            {
                new Signup{RiderID = 2, RideID=3},
                new Signup{RiderID = 3, RideID=3},
                new Signup{RiderID = 4, RideID=3},
                new Signup{RiderID = 5, RideID=3},
                new Signup{RiderID = 7, RideID=3},
                new Signup{RiderID = 3, RideID=2},
                new Signup{RiderID = 5, RideID=2},
                new Signup{RiderID = 6, RideID=2}
            };
            foreach (Signup s in signUps)
            {
                context.SignUps.Add(s);
            }
            context.SaveChanges();
        }
    }
}
