using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Holidays.Api.Models;

namespace Holidays.Api;

public class Program {
    public string ConnStr { get; set; }

    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        var prog = new Program();

        prog.ConnStr = builder.Configuration.GetConnectionString("Database");

        prog.ConfigureServices(builder.Services);
        var app = builder.Build();
        prog.Configure(app, app.Environment);
        app.Run();
    }

    public void ConfigureServices(IServiceCollection services) {
        // Add services to the container.
        services.AddControllers().AddJsonOptions(opts => {
            var enumConverter = new JsonStringEnumConverter();
            opts.JsonSerializerOptions.Converters.Add(enumConverter);
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        if(ConnStr == null) {
            services.AddDbContext<CountryContext>(opt => 
                opt.UseInMemoryDatabase("Holidays"));
            services.AddDbContext<StatusContext>(opt => 
                opt.UseInMemoryDatabase("Holidays"));
            services.AddDbContext<CountContext>(opt => 
                opt.UseInMemoryDatabase("Holidays"));
            services.AddDbContext<HolidayContext>(opt => 
                opt.UseInMemoryDatabase("Holidays"));
        } else {
            services.AddDbContext<CountryContext>(opt => 
                opt.UseSqlServer(ConnStr));
            services.AddDbContext<CountContext>(opt => 
                opt.UseSqlServer(ConnStr));
            services.AddDbContext<StatusContext>(opt => 
                opt.UseSqlServer(ConnStr));
            services.AddDbContext<HolidayContext>(opt => 
                opt.UseSqlServer(ConnStr));
        }

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
