using Microsoft.AspNetCore.Mvc;
using Holidays.Api.Models;

namespace Holidays.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ConsecutiveHolidaysController : ControllerBase {
    private readonly EnricoService _service;
    private readonly CountContext _context;

    public ConsecutiveHolidaysController(CountContext context) {
        _context = context;
        _service = new EnricoService();
    }

    [HttpGet(Name = "GetConsecutiveHolidaysController")]
    public async Task<IActionResult> Get([FromQuery]string countryCode, [FromQuery]int year) {
        var res = _context.GetCountForYear(countryCode, year);
        if(res == null) {
            try {
                res = await _service.GetConsecutiveHolidays(countryCode, year);
                } catch {
                    return BadRequest();
                }
            _context.Counts.Add(res);
            await _context.SaveChangesAsync();
        }
        
        return Ok(res);
    }
}