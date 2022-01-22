using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;

using Holidays.Api;
using Holidays.Api.Controllers;

namespace Holidays.Tests;

public class UnitTest1
{
    private readonly TestServer _server;
    private readonly HttpClient _client;

    public UnitTest1() {
        _server = new TestServer(new WebHostBuilder()
           .UseStartup<Program>());
        _client = _server.CreateClient();
    }

    [Fact]
    public async Task TestCountryCount()
    {
        var response = await _client.GetAsync("/SupportedCountries");
        response.EnsureSuccessStatusCode();
        var respText = await response.Content.ReadAsStringAsync();
        var root = JsonDocument.Parse(respText).RootElement;
        var numCountries = 0;
        foreach(var c in root.EnumerateArray()) numCountries++;
        Assert.Equal(54, numCountries);
    }

    [Fact]
    public async Task TestWorkDay()
    {
        var response = await _client.GetAsync("/DayStatus?countryCode=svk&year=2022&month=7&day=4");
        response.EnsureSuccessStatusCode();
        var respText = await response.Content.ReadAsStringAsync();
        var root = JsonDocument.Parse(respText).RootElement;
        var status = root.GetProperty("status").GetString();
        Assert.Equal("WorkDay", status);
    }

    [Fact]
    public async Task TestHoliday()
    {
        var response = await _client.GetAsync("/DayStatus?countryCode=svk&year=2022&month=7&day=5");
        response.EnsureSuccessStatusCode();
        var respText = await response.Content.ReadAsStringAsync();
        var root = JsonDocument.Parse(respText).RootElement;
        var status = root.GetProperty("status").GetString();
        Assert.Equal("Holiday", status);
    }

    [Fact]
    public async Task TestFreeDay()
    {
        var response = await _client.GetAsync("/DayStatus?countryCode=svk&year=2022&month=7&day=3");
        response.EnsureSuccessStatusCode();
        var respText = await response.Content.ReadAsStringAsync();
        var root = JsonDocument.Parse(respText).RootElement;
        var status = root.GetProperty("status").GetString();
        Assert.Equal("FreeDay", status);
    }

    [Fact]
    public async Task TestInvalidDay()
    {
        var response = await _client.GetAsync("/DayStatus?countryCode=nope&year=2022&month=7&day=3");
        response.EnsureSuccessStatusCode();
        var respText = await response.Content.ReadAsStringAsync();
        var root = JsonDocument.Parse(respText).RootElement;
        var status = root.GetProperty("status").GetString();
        Assert.Equal("Invalid", status);
    }

    [Fact]
    public async Task TestConsecutiveHolidays()
    {
        var response = await _client.GetAsync("/ConsecutiveHolidays?countryCode=tur&year=2022");
        response.EnsureSuccessStatusCode();
        var respText = await response.Content.ReadAsStringAsync();
        var root = JsonDocument.Parse(respText).RootElement;
        var count = root.GetProperty("count").GetInt32();
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task TestConsecutiveHolidaysForInvalidCountry()
    {
        var response = await _client.GetAsync("/ConsecutiveHolidays?countryCode=nope&year=2022");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}