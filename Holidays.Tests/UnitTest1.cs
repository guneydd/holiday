using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
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
}