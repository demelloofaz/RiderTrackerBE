using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SouthChandlerCycling.Models
{
    public class Ride
    {
        public int ID { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Ride name cannot be longer than 50 characters.")]
        [Column("RideName")]
        [Display(Name = "Ride Name")]
        public string RideName { get; set; }
        [Required]
        [StringLength(120, ErrorMessage = "Description name cannot be longer than 120 characters.")]
        [Column("Description")]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [Column("StartDate")]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Column("Distance")]
        [Display(Name = "Distance in Miles")]
        public double Distance { get; set; }
        [Column("Creator")]
        [Display(Name = "Creator")]
        public int CreatorId { get; set; }

        public ICollection<Signup> Signups { get; set; }
        //public ICollection<Rider> RideLeader { get; set; }
    }
}

