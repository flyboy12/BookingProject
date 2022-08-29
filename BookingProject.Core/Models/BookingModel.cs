using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingProject.Core.Models
{
    public class BookingViewModel
    {
        [DisplayName("Name")]
        public string first_name { get; set; }
        [DisplayName("Last Name")]
        public string last_name { get; set; }
        [DisplayName("Email")]
        public string email { get; set; }
        [DisplayName("Phone")]
        public string phone { get; set; }
        [NotMapped]
        public int booking_id { get; set; }
        [DisplayName("Start At")]
        public string starts_at { get; set; }
        [DisplayName("Booked At")]
        public string booked_at { get; set; }
        [DisplayName("Confirmed")]
        public int confirmed { get; set; }
        [DisplayName("Apts. Name")]
        public string apartment_name { get; set; }
        [DisplayName("Apts. Addr.")]
        public string apartment_address { get; set; }
        [DisplayName("Apts. Code")]
        public string apartment_zip_code { get; set; }
        [DisplayName("Apts. City")]
        public string apartment_city { get; set; }
        [DisplayName("Apts. Country")]
        public string apartment_country { get; set; }

    }
    public class BookingModel
    {
        [DisplayName("User Name")]
        public string user_fullname { get; set; }
        [DisplayName("Email")]
        public string  user_email { set; get; }
        [DisplayName("Bookin Id")]
        public int booking_id { get; set; }
        [DisplayName("Start At")]
        public string starts_at { get; set; }
        [DisplayName("Booked At")]
        public string booked_at { get; set; }
        [DisplayName("Booked For")]
        public int booked_for { get; set; }
        [DisplayName("Apts. Name")]
        public string apartment_name { get; set; }
        [DisplayName("Confirmed")]

        public int confirmed { get; set; }
    }
    public class BookingUpdateModel
    {
        [DisplayName("Start At")]
        public string starts_at { get; set; }
        public int booked_for { get; set; }
        [DisplayName("Confirmed")]

        public int confirmed { get; set; }
    }
    public class BookingAddModel
    {
        [DisplayName("Email")]
        public string user_email { get; set; }
        [Required]

        public string starts_at { get; set; }
        [DisplayName("Booked For")]
        [Required]
        public int booked_for { get; set; }
        [Required]
        [DisplayName("Apts. Id")]
        public int  apartment_id { get; set; }
        [DisplayName("Confirmed")]

        public int confirmed { get; set; }
    }
}
