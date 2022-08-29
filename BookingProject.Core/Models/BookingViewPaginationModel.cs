using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingProject.Core.Models
{
    public class BookingViewPaginationModel
    {
        public  List<BookingViewModel> bookingList { get; set; }
        public int currentPageIndex { get; set; }
        public int pageCount { get; set; }
    }
}
