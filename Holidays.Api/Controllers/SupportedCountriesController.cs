using Microsoft.AspNetCore.Mvc;
using Holidays.Api.Models;

namespace Holidays.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SupportedCountriesController : ControllerBase {
    private readonly EnricoService _service;
    private readonly CountryContext _context;

    public SupportedCountriesController(CountryContext context) {
        _context = context;
        _service = new EnricoService();
    }

    [HttpGet(Name = "GetSupportedCountries")]
    public async Task<IEnumerable<Country>> Get() {
        List<Country> countries;
        if(_context.Countries.Count() == 0) {
            Console.WriteLine("countries in db empty");
            countries = await _service.GetSupportedCountries();
            foreach(var country in countries) {
                _context.Countries.Add(country);
            }
            await _context.SaveChangesAsync();
        } else {
            countries = _context.Countries.ToList();
        }

        return countries;
    }
}
