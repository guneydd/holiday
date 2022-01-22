using Microsoft.AspNetCore.Mvc;
using Holidays.Api.Models;

namespace Holidays.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HolidaysByMonthController : ControllerBase {
    private readonly EnricoService _service;
    private readonly HolidayContext _context;

    public HolidaysByMonthController(HolidayContext context) {
        _context = context;
        _service = new EnricoService();
    }

    [HttpGet(Name = "GetCHolidaysByMonth")]
    public async Task<IActionResult> Get([FromQuery]string countryCode, [FromQuery]int year) {
        var res = _context.GetHolidaysForYearByMonth(countryCode, year);
        if(res == null) {
            try {
                var holidays = await _service.GetHolidaysForYear(countryCode, year);
                foreach(var holiday in holidays) {
                    _context.Holidays.Add(holiday);
                }
            } catch {
                return BadRequest();
            }
            await _context.SaveChangesAsync();
            res = _context.GetHolidaysForYearByMonth(countryCode, year);
        }
        
        return Ok(res);
    }
}