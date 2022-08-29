using BookingApp.Helper;
using BookingApp.Models;
using BookingProject.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookingApp.Controllers
{
    public class HomeController : Controller
    {

        BookingAPI _api = new BookingAPI();
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.mssg = TempData["mssg"] as String;
            ViewBag.info = TempData["info"] as String;
            BookingViewPaginationModel paginationModel = new BookingViewPaginationModel();
            paginationModel.bookingList = new List<BookingViewModel>();
            HttpClient client = _api.Initial();
            HttpResponseMessage res =    await client.GetAsync( "Booking/List/1");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                paginationModel = JsonConvert.DeserializeObject<BookingViewPaginationModel>(result);
            }
            return View(paginationModel);
        }
        [HttpPost]
        public async Task<IActionResult> Index(int currentPageIndex)
       {
            BookingViewPaginationModel paginationModel = new BookingViewPaginationModel();
            paginationModel.bookingList = new List<BookingViewModel>();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Booking/List/" + currentPageIndex.ToString());
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                paginationModel = JsonConvert.DeserializeObject<BookingViewPaginationModel>(result);
            }
            return View(paginationModel);
        }
        
        public ActionResult Create()
        {
            return View(new BookingAddModel());
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            BookingModel updateModel = new BookingModel();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = client.GetAsync("Booking/getById/" + id).Result;
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                updateModel = JsonConvert.DeserializeObject<BookingModel>(result);
            }
            return View(updateModel);
        }
        [HttpPost]
        public IActionResult Edit(BookingModel model)
        {
            BookingUpdateModel updateModel = new BookingUpdateModel() {booked_for = model.booked_for,confirmed=model.confirmed,starts_at=model.starts_at };
            HttpClient client = _api.Initial();
            StringContent requestContent = new StringContent(JsonConvert.SerializeObject(updateModel), Encoding.UTF8, "application/json");
            HttpResponseMessage res = client.PutAsync("Booking/update/" + model.booking_id, requestContent).Result;
            HttpClientServiceResponse<BookingUpdateModel> result = new HttpClientServiceResponse<BookingUpdateModel>();
            result.StatusCode = res.StatusCode;
            result.ResponseContent = res.Content.ReadAsStringAsync().Result;
            if (res.IsSuccessStatusCode)
            {
                TempData["mssg"] = result.ResponseContent;
                TempData["info"] = "success";
            }
            TempData["mssg"] = result.ResponseContent;
            TempData["info"] = "error";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Create(BookingAddModel bookingAddModel)
        {
           
            HttpClient client = _api.Initial();
            StringContent requestContent = new StringContent(JsonConvert.SerializeObject(bookingAddModel), Encoding.UTF8, "application/json");
            HttpResponseMessage response =  client.PostAsync("Booking/create", requestContent).Result;
            HttpClientServiceResponse<BookingModel> result = new HttpClientServiceResponse<BookingModel>();
            result.StatusCode = response.StatusCode;
            result.ResponseContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                result.Data = JsonConvert.DeserializeObject<BookingModel>( result.ResponseContent);
                return RedirectToAction("GetBooking", result.Data.booking_id);
            }
            else
            {
               TempData["mssg"] = result.ResponseContent;
               TempData["info"] = "error";
               return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public IActionResult GetBooking(int id)
        {
            BookingModel bookingModel = new BookingModel();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = client.GetAsync("Booking/getById/" + id).Result;
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                bookingModel = JsonConvert.DeserializeObject<BookingModel>(result);
            }
            return View(bookingModel);
        }

        public IActionResult Delete(int id)
        {
            HttpClientServiceResponse<string> result = new HttpClientServiceResponse<string>();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = client.DeleteAsync("Booking/delete/" + id).Result;
            result.StatusCode = res.StatusCode;
            result.ResponseContent = res.Content.ReadAsStringAsync().Result;

            if (res.IsSuccessStatusCode)
            {
                TempData["mssg"] = result.ResponseContent;
                TempData["info"] = "success";
            }
            else
            {
                TempData["mssg"] = result.ResponseContent;
                TempData["info"] = "error";
            }
            return RedirectToAction("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    } 
}
public class HttpClientServiceResponse<T>
{
    public T Data { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string ResponseContent { get; set; }
}
