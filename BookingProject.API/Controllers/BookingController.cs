using BookingProject.API.DataAccess;
using BookingProject.API.Entities;
using BookingProject.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingProject.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly DatabaseContext _db;
        public BookingController(DatabaseContext context)
        {
            _db = context;
        }
        
        [HttpGet("list/{currentPage}")]
        [ProducesResponseType(200, Type = typeof(BookingViewPaginationModel))]
        public async  Task<IActionResult> BookingAllList([FromRoute] int currentPage = 1, [FromQuery] FilteringBookingModel filter = null )
        {
            BookingViewPaginationModel paginationModel = new BookingViewPaginationModel();
            int maxRowsPerPage = 10;paginationModel.currentPageIndex = currentPage;

            var query= _db.Bookings.OrderBy(x => x.id).Include(x => x.user).Include(x => x.apartment).AsQueryable();
            if (filter.user_name != null && filter.user_last_name != null)
            {
                query= query.Where(x => x.user.first_name.ToLower().Trim() == filter.user_name.ToLower().Trim() && x.user.last_name.ToLower().Trim() == filter.user_last_name.ToLower().Trim()).AsQueryable();
            }
            else if (filter.apartment_name != null && filter.confirmend != null)
            {
                query = query.Where(x => x.apartment.name.ToLower().Trim() == filter.apartment_name.ToLower().Trim() && x.confirmed == filter.confirmend).AsQueryable();
            }
            else if (filter.apartment_name != null && filter.confirmend != null && filter.user_name != null && filter.user_last_name != null)
            {
                query = query.Where(x => x.apartment.name.ToLower().Trim() == filter.apartment_name.ToLower().Trim() && x.confirmed == filter.confirmend && x.user.first_name.ToLower().Trim() == filter.user_name.ToLower().Trim() && x.user.last_name.ToLower().Trim() == filter.user_last_name.ToLower().Trim()).AsQueryable();
            }
            else if (filter.user_name != null && filter.user_last_name != null && filter.confirmend != null)
            {
                query = query.Where(x => x.user.first_name.ToLower().Trim() == filter.user_name.ToLower().Trim() && x.user.last_name.ToLower().Trim() == filter.user_last_name.ToLower().Trim()&& x.confirmed == filter.confirmend).AsQueryable();
            }
            else if (filter.confirmend != null)
            {
                query = query.Where(x => x.confirmed == filter.confirmend).AsQueryable();
            }
            else if (filter.apartment_name != null)
            {
                query = query.Where(x => x.apartment.name.ToLower().Trim() == filter.apartment_name.ToLower().Trim() ).AsQueryable();
            }
            paginationModel.bookingList = await query.Skip((currentPage - 1) * maxRowsPerPage).Take(maxRowsPerPage).Select(
            x => new BookingViewModel()
              {
                  email = x.user.email,
                  apartment_address = x.apartment.address,
                  apartment_city = x.apartment.city,
                  apartment_country = x.apartment.country,
                  apartment_name = x.apartment.name,
                  apartment_zip_code = x.apartment.zip_code,
                  booked_at = x.booked_at,
                  booking_id = x.id,
                  confirmed = x.confirmed,
                  first_name = x.user.first_name,
                  last_name = x.user.last_name,
                  phone = x.user.phone,
                  starts_at = x.starts_at,
              }).ToListAsync(); ;
            double pageCount = (double)((decimal)query.Count() / Convert.ToDecimal(maxRowsPerPage));
            paginationModel.pageCount = (int) Math.Ceiling(pageCount);

            return Ok(paginationModel);
        }
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(BookingModel))]
        [ProducesResponseType(404, Type = typeof(string))]
        public IActionResult getById([FromRoute] int id)
        {
            Bookings booking = _db.Bookings.SingleOrDefault(x => x.id == id);
            if (booking == null)
                return NotFound("Opps! Booking Id not found.");
            BookingModel bookingModel = _db.Bookings.Include(x => x.apartment).Include(x => x.user).Select(
                 x => new BookingModel()
                 {
                     apartment_name = x.apartment.name,
                     booking_id = x.id,
                     booked_at = x.booked_at,
                     booked_for = x.booked_for,
                     confirmed = x.confirmed,
                     starts_at = x.starts_at,
                     user_email = x.user.email,
                     user_fullname = x.user.full_name,
                 }).SingleOrDefault(x=> x.booking_id == id);
            return Ok(bookingModel);
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(string))]
        public IActionResult Delete([FromRoute] int id) {


            Bookings booking = _db.Bookings.SingleOrDefault(x=> x.id == id);
            if (booking == null) 
                return NotFound("Opps! Booking Id not found.");
            if (booking.confirmed == 0)
                return BadRequest("You cannot delete an approved booking");
            _db.Bookings.Remove(booking);
            _db.SaveChanges();
            return Ok("Delete is succsess!");
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(200, Type = typeof(BookingUpdateModel))]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(string))]
        public IActionResult Update([FromRoute] int id ,[FromBody] BookingUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Validation Error Occurred.");
            }
            Bookings booking = _db.Bookings.SingleOrDefault(x=> id == x.id);
            if (booking == null) return BadRequest("There Is No Such Appointment Id.");
            DateTime dateBookedAt = stringFormatDate(model.starts_at).AddDays(model.booked_for);
            string booked_at =  dateFormatString(dateBookedAt);
            booking.booked_at = booked_at;
            booking.booked_for = model.booked_for;
            booking.confirmed = model.confirmed;
            booking.starts_at = model.starts_at;
            _db.SaveChanges();
            return Ok(booking);
        }

        [HttpPost("create")]
        [ProducesResponseType(201, Type = typeof(BookingModel))]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(string))]
        public IActionResult Create([FromBody] BookingAddModel model)
        {
            if (!ModelState.IsValid)
            {
               return BadRequest("Validation Error Occurred.");
            }
            DateTime dateBookedAt = stringFormatDate(model.starts_at).AddDays(model.booked_for);
            string booked_at = dateFormatString(dateBookedAt);
            User user = _db.Users.SingleOrDefault(x=> x.email.Trim().ToLower() == model.user_email.Trim().ToLower());
            if (user == null) return BadRequest("There is no User for Email Information.");
            Apartment apartment = _db.Appartments.SingleOrDefault(x=> x.id== model.apartment_id);
            if (apartment == null) return BadRequest("There is no Apartment in these Coordinates.");
            int endBookingId = _db.Bookings.OrderByDescending(x => x.id).First().id +1;
            Bookings addBookings = new Bookings()
            {
                id = endBookingId,
                apartment_id = apartment.id,
                booked_at = booked_at,
                booked_for = model.booked_for,
                confirmed = model.confirmed,
                starts_at = model.starts_at,
                user_id = user.id,
            };
            _db.Bookings.Add(addBookings);
            _db.SaveChanges();
            BookingModel bookingModel = new BookingModel() { 
            apartment_name = apartment.name,
            booked_at=booked_at,
            booked_for=model.booked_for,
            booking_id = endBookingId,
            confirmed=model.confirmed,
            starts_at= model.starts_at,
            user_fullname = user.full_name,
            user_email= user.email
            };
            return Ok(bookingModel);
        }
       string dateFormatString(DateTime date )
        {
                string dateTime = date.ToString("yyyy-MM-dd HH:mm:ss.fff").Replace(" ", "T");
                return dateTime = string.Concat(dateTime, "Z");
        }
       DateTime stringFormatDate(string date)
        {
            return DateTime.ParseExact(date, "yyyy-MM-ddTHH:mm:ss.fffZ", null);
        }

    }
}

