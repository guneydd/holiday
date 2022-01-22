using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Holidays.Api.Models
{
    public class CountContext : DbContext
    {
        public CountContext(DbContextOptions<CountContext> options)
            : base(options)
        {
        }

        public DbSet<HolidaysCount> Counts { get; set; } = null!;

        public HolidaysCount GetCountForYear(string code, int year) {
            var res = Counts.Where(s =>
                s.Country == code && s.Year == year).ToList();
            if(res.Count == 0) return null;
            return res[0];
        }
    }
}
