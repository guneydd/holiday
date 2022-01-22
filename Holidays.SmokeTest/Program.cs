using System.Net.Http;
using System.Net;

string baseUrl = "https://guney-holiday.herokuapp.com";
string[] endpoints = { "/SupportedCountries", "/DayStatus?countryCode=svk&year=2022&month=7&day=4", 
    "/ConsecutiveHolidays?countryCode=tur&year=2022", "/HolidaysByMonth?countryCode=tur&year=2022" };

using(var client = new HttpClient()) {
    foreach(var endpoint in endpoints) {
        var url = $"{baseUrl}{endpoint}";
        Console.WriteLine($"GET {endpoint}");
        using(var response = await client.GetAsync(url)) {
            if(response.StatusCode != HttpStatusCode.OK) {
                Console.WriteLine($"smoke test failed for url: {endpoint}, code: {response.StatusCode}");
                System.Environment.Exit(1);
            }
        }
    }
}

Console.WriteLine("smoke test successful");