using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MyPersonalBot
{
    public class Coord
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Main
    {
        public double temp { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
    }

    public class Wind
    {
        public double speed { get; set; }
        public int deg { get; set; }
    }

    public class Rain
    {
        
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public double message { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class LocationInfo
    {
        public Coord coord { get; set; }
        public List<Weather> weather { get; set; }
        public string @base { get; set; }
        public Main main { get; set; }
        public Wind wind { get; set; }
        public Rain rain { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }

        public static async Task<LocationInfo> GetLocationInfo(string city)
        {
            LocationInfo locInfo = new LocationInfo();

            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri("http://api.openweathermap.org/")
            };
            var response = await client.GetAsync($"data/2.5/weather?q={city}&APPID=--replace with your own--&units=metric");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                locInfo = JsonConvert.DeserializeObject<LocationInfo>(json);
            }

            return locInfo;
        }

        public async static Task<Address> GetAddress()
        {
            HttpClient client = new HttpClient();

            var response = Task.Run(() =>
                client.GetAsync("http://svdhlocationapi.azurewebsites.net/api/location")
            ).Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonBingLocation = await response.Content.ReadAsStringAsync();
                var address = JsonConvert.DeserializeObject<Address>(jsonBingLocation);
                return address;
            }
            return new Address();
        }
    }
}