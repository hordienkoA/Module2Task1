using CartService.BLL.Interfaces;
using CartService.Contracts.Interfaces;
using CartService.DAL.Repositories;
using CartService.BLL.Services;
using CartService.API.MessageBroker;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHostedService<RabbitMqListener>();
builder.Services.AddScoped<ICartRepository>(sp=> new CartRepository("cart.db"));
builder.Services.AddScoped<ICartService, CartService.BLL.Services.CartService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
