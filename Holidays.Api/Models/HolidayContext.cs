using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Holidays.Api.Models
{
    public class HolidayContext : DbContext
    {
        public HolidayContext(DbContextOptions<HolidayContext> options)
            : base(options)
        {
        }

        public DbSet<Holiday> Holidays { get; set; } = null!;

        public Dictionary<string, List<Holiday>> GetHolidaysForYearByMonth(string code, int year) {
            var holidayCount = Holidays.Where(h => h.Country == code && h.Year == year).Count();
            if(holidayCount == 0) return null;

            string[] monthNames = { "january", "february", "march", "april", 
                "may", "june", "july", "august", "september",
                "october", "november", "december" };
            var holidayGroups = Holidays.Where(h => h.Country == code && h.Year == year).ToList()
                .GroupBy(h => h.Month);

            var res = new Dictionary<string, List<Holiday>>();
            foreach(var group in holidayGroups) {
                res.Add(monthNames[group.Key-1], group.ToList());
            }

            return res;
        }
    }
}