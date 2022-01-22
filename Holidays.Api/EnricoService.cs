using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Holidays.Api.Models;

namespace Holidays.Api;

class EnricoService {
    private const string BaseUrl = "https://kayaposoft.com/enrico/json/v2.0";

    public async Task<List<Country>> GetSupportedCountries() {
        var param = new Dictionary<string, string>();
        param.Add("action", "getSupportedCountries");
        var url = QueryHelpers.AddQueryString(BaseUrl, param);
        var res = new List<Country>();

        using(var client = new HttpClient()) {
            Console.WriteLine("Calling external API");
            using(var response = await client.GetAsync(url)) {
                var respText = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(respText);
                foreach(var country in doc.RootElement.EnumerateArray()) {
                    var code = country.GetProperty("countryCode").GetString();
                    var name = country.GetProperty("fullName").GetString();
                    res.Add(new Country(code, name));
                }

                return res;
            }
        }
    }
}