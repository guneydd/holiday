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

    private async Task<DayStatus.EStatus> GetStatusEnum(string code, int year, int month, int day) {
        using(var client = new HttpClient()) {
            string date=$"{day}-{month}-{year}";

            var param = new Dictionary<string, string>();
            param.Add("action", "isPublicHoliday");
            param.Add("date", date);
            param.Add("country", code);
            var url = QueryHelpers.AddQueryString(BaseUrl, param);

            using(var response = await client.GetAsync(url)) {
                var respText = await response.Content.ReadAsStringAsync();
                var root = JsonDocument.Parse(respText).RootElement;
                try {
                    bool isHoliday = root.GetProperty("isPublicHoliday").GetBoolean();
                    if(isHoliday) return DayStatus.EStatus.Holiday;
                } catch(KeyNotFoundException ex) {
                    return DayStatus.EStatus.Invalid;
                }
            }

            param.Remove("action");
            param.Add("action", "isWorkDay");
            url = QueryHelpers.AddQueryString(BaseUrl, param);

            using(var response = await client.GetAsync(url)) {
                var respText = await response.Content.ReadAsStringAsync();
                var root = JsonDocument.Parse(respText).RootElement;
                try {
                    bool isWorkDay = root.GetProperty("isWorkDay").GetBoolean();
                    if(isWorkDay) return DayStatus.EStatus.WorkDay;
                } catch(KeyNotFoundException ex) {
                    return DayStatus.EStatus.Invalid;
                }
            }

            return DayStatus.EStatus.FreeDay;
        }
    }

    public async Task<DayStatus> GetDayStatus(string code, int year, int month, int day) {
        DayStatus res = new DayStatus();
        res.Country = code;
        res.Year = year;
        res.Month = month;
        res.Day = day;
        res.Status = await GetStatusEnum(code, year, month, day);

        return res;
    }
}