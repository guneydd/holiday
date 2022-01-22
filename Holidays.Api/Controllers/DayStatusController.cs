using Microsoft.AspNetCore.Mvc;
using Holidays.Api.Models;

namespace Holidays.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DayStatusController : ControllerBase {
    private readonly EnricoService _service;
    private readonly StatusContext _context;

    public DayStatusController(StatusContext context) {
        _context = context;
        _service = new EnricoService();
    }

    [HttpGet(Name = "GetDayStatus")]
    public async Task<DayStatus> Get([FromQuery]string countryCode, 
        [FromQuery]int year, [FromQuery]int month, [FromQuery]int day) {
        DayStatus res = _context.GetStatus(countryCode, year, month, day);
        if(res == null) {
            Console.WriteLine("res not found in db");
            res = await _service.GetDayStatus(countryCode, year, month, day);
            _context.Statuses.Add(res);
            await _context.SaveChangesAsync();
        }
        return res;
    }
}