using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Holidays.Api.Models
{
    public class StatusContext : DbContext
    {
        public StatusContext(DbContextOptions<StatusContext> options)
            : base(options)
        {
        }

        public DbSet<DayStatus> Statuses { get; set; } = null!;

        public DayStatus GetStatus(string code, int year, int month, int day) {
            var res = Statuses.Where(s =>
                s.Country == code && s.Year == year && s.Month == month && s.Day == day).ToList();

            if(res.Count == 0) return null;
            return res[0];
        }
    }
}
