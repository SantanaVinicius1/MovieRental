using MovieRental.Customer;
using MovieRental.Data;
using MovieRental.Middlewares;
using MovieRental.Movie;
using MovieRental.PaymentProviders;
using MovieRental.Rental;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEntityFrameworkSqlite().AddDbContext<MovieRentalDbContext>();

builder.Services.AddExceptionHandler<GlobalExceptionHandlerMiddleware>();
builder.Services.AddProblemDetails();

builder.Services
    .AddScoped<IRentalFeatures, RentalFeatures>()
    .AddScoped<IMovieFeatures, MovieFeatures>()
    .AddScoped<ICustomerFeatures, CustomerFeatures>();

builder.Services.AddKeyedScoped<IPaymentProvider, MbWayProvider>("MBWAY");
builder.Services.AddKeyedScoped<IPaymentProvider, PayPalProvider>("PAYPAL");
builder.Services.AddKeyedScoped<IPaymentProvider, FaultyPaymentProvider>("FAULTY");



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

using (var client = new MovieRentalDbContext())
{
	client.Database.EnsureCreated();
}

app.Run();
