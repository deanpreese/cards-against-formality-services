
using System.Text.Json.Serialization;
using OMS.Data;
using Microsoft.EntityFrameworkCore;
using OMS.Core.Common;
using OMS.Core.Interfaces;
using OMS.Services.Trading;
using OMS.Services.Data;
using OMS.Services.Queue;
using OMS.Data.Repositories;
using OMS.Logging;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<OrderManagementDbContext>(options =>
{
    string conn =  "Host=127.0.0.1;Database=orders;Username=trading;Password=abc";
    options.UseNpgsql(conn);
    //options.EnableSensitiveDataLogging();
    //options.EnableDetailedErrors();

},ServiceLifetime.Scoped);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ITradingService, TradingService>();
builder.Services.AddScoped<IDataService, DataService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddMvc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpsRedirection(opt => opt.HttpsPort = 44300);

builder.Services.AddSingleton<NewOrderChannelService>();
builder.Services.AddHostedService<NewOrderProcessorService>();

builder.Services.AddLogging(logging =>
{  
    logging.ClearProviders();
    logging.AddSpectreConsole(); 
    //logging.AddConsole();   
});


var app = builder.Build();

/*
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

// Register a method to be called after the application has started
lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("The application has started.");
    // Your code here...
});
*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseStaticFiles();
app.UseRouting();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());


//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();



app.Run();