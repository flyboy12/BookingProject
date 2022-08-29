using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingProject.API.Entities
{
    [Table("bookings")]
    public class Bookings
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string starts_at { get; set; }
        public string booked_at { get; set; }
        public int booked_for { get; set; }
        public int apartment_id { get; set; }
        public int confirmed { get; set; }
        public  User user { get; set; }
        public  Apartment apartment { get; set ; }


    }

}
