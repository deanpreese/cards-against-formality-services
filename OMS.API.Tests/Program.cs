using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using OMS.Data;
using OMS.Data.Repositories;
using OMS.Core.Interfaces;
using OMS.Services.Trading;
using Microsoft.Extensions.Logging;
using OMS.Logging;

namespace OMS.API.Tests;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting API Tester...");

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var optionsBuilder = new DbContextOptionsBuilder<OrderManagementDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();

        OrderManagementDbContext context = new OrderManagementDbContext(optionsBuilder.Options, configuration);

        ILogger<TradingService>? _logger = null;
        using (var factory = LoggerFactory.Create(b => b.AddSpectreConsole())) {
            _logger = factory.CreateLogger<TradingService>();
        }

        IUnitOfWork unitOfWork = new UnitOfWork(context);
        TradingService service = new TradingService(unitOfWork, _logger);

        Console.WriteLine("Running Tests...");

        OrderGen orderGen = new OrderGen(context);

        Console.WriteLine("Done.");

    }
}