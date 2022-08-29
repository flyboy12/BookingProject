using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingProject.Core.Models
{
    public class FilteringBookingModel  
    {
        public string user_name { get; set; }
        public string user_last_name { get; set; }
        public string apartment_name { get; set; }
        public int? confirmend { get; set; }
    }
}
