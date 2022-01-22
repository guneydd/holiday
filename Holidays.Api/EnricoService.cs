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

    private async Task<List<DateTime>> GetHolidayDatesForYear(string code, int year) {
        var param = new Dictionary<string, string>();
        param.Add("action", "getHolidaysForYear");
        param.Add("holidayType", "public_holiday");
        param.Add("year", year.ToString());
        param.Add("country", code);
        var url = QueryHelpers.AddQueryString(BaseUrl, param);

        using(var client = new HttpClient()) {
            Console.WriteLine("Calling external API");
            using(var response = await client.GetAsync(url)) {
                var respText = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(respText);
                var root = doc.RootElement;

                var dates = new List<DateTime>();
                foreach(var holiday in root.EnumerateArray()) {
                    var date = holiday.GetProperty("date");
                    var y = date.GetProperty("year").GetInt32();
                    var m = date.GetProperty("month").GetInt32();
                    var d = date.GetProperty("day").GetInt32();

                    dates.Add(new DateTime(y, m, d));
                }

                return dates;
            }
        }
    }

    public async Task<HolidaysCount> GetConsecutiveHolidays(string code, int year) {
        HolidaysCount res = new HolidaysCount();
        res.Country = code;
        res.Year = year;

        var holidays = await GetHolidayDatesForYear(code, year);
        int max = 1;
        int count = 1;
        DateTime prev = holidays[0];
        for(int i=1; i<holidays.Count; i++) {
            var curr = holidays[i];
            if(curr.Subtract(prev) <= TimeSpan.FromDays(1.0)) {
                count++;
                if(count > max) max = count;
            } else {
                count = 1;
            }
            prev = curr;
        }

        res.Count = max;

        return res;
    }

    public async Task<List<Holiday>> GetHolidaysForYear(string code, int year) {
        var param = new Dictionary<string, string>();
        param.Add("action", "getHolidaysForYear");
        param.Add("holidayType", "public_holiday");
        param.Add("year", year.ToString());
        param.Add("country", code);
        var url = QueryHelpers.AddQueryString(BaseUrl, param);

        using(var client = new HttpClient()) {
            Console.WriteLine("Calling external API");
            using(var response = await client.GetAsync(url)) {
                var respText = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(respText);
                var root = doc.RootElement;

                var res = new List<Holiday>();
                foreach(var holiday in root.EnumerateArray()) {
                    Holiday curr = new Holiday();

                    var date = holiday.GetProperty("date");
                    curr.Country = code;
                    curr.Year = date.GetProperty("year").GetInt32();
                    curr.Month = date.GetProperty("month").GetInt32();
                    curr.Day= date.GetProperty("day").GetInt32();

                    var names = holiday.GetProperty("name");
                    curr.Name = names[0].GetProperty("text").GetString();

                    res.Add(curr);
                }

                return res;
            }
        }
    }
}