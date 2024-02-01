using System.Text.Json.Serialization;
using OMS.Services.Queue;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}); 

//builder.Services.AddMvc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<NewOrderQueue>();
builder.Services.AddHostedService<NewOrderProcessor>();

builder.Services.AddSingleton<ClosedOrderQueue>();
builder.Services.AddHostedService<ClosedOrderProcessor>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

