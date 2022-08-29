using System;
using System.Net.Http;

namespace BookingApp.Helper
{
    public class BookingAPI
    {
        public HttpClient Initial()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44347/");
            return client;
        }
    }
}
