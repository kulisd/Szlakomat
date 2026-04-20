using Szlakomat.Products.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddProductModule();

var app = builder.Build();
app.MapControllers();
app.Run();
