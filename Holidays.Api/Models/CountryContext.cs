using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Holidays.Api.Models
{
    public class CountryContext : DbContext
    {
        public CountryContext(DbContextOptions<CountryContext> options)
            : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; } = null!;
    }
}
