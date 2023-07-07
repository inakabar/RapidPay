using Microsoft.EntityFrameworkCore;
using RapidPay.DataAccess.Data;
using RapidPay.Api.Configuration;
using RapidPay.Api.Services.Card;
using RapidPay.Api.Services.FeeService;
using RapidPay.Api.Services.Notify;
using RapidPay.DataAccess.Repository;
using RapidPay.Api.Validators;
using RapidPay.Api.Models;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


builder.Services.AddHealthChecks()
    .AddSqlServer(
    connectionString: builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddUrlGroup(
        new Uri(builder.Configuration.GetValue<string>("IdentityAuthorityHealthCheckUrl")),
        "Identity Server"
        );

builder.Services.AddAuthConfig(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddDbContext<RapidPayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerConfig();

builder.Services.AddSingleton<IFeeService, RandomFeeService>();
builder.Services.AddScoped<IDataAccessLayer, RapidPayDbContext>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<INotification, Notification>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<IPaymentHistoryRepository, PaymentHistoryRepository>();

builder.Services.AddScoped<ICustomValidator<CreateCardModel>, CreateCardModelManager>();
builder.Services.AddScoped<ICustomValidator<CardAddBalanceModel>, CardAddBalanceModelManager>();
builder.Services.AddScoped<ICustomValidator<MakePaymentModel>, MakePaymentModelManager>();
builder.Services.AddScoped<ICustomValidator<GetBalanceModel>, GetBalanceModelManager>();

builder.Services.AddScoped<IValidator<CreateCardModel>, CreateCardModelValidator>();
builder.Services.AddScoped<IValidator<CardAddBalanceModel>, CardAddBalanceModelValidator>();
builder.Services.AddScoped<IValidator<MakePaymentModel>, MakePaymentModelValidator>();
builder.Services.AddScoped<IValidator<GetBalanceModel>, GetBalanceModelValidator>();
builder.Services.AddLogging();

var app = builder.Build();

app.UseSwagger();
app.UseAuthorization();
app.UseAuthorization();

app.UseMiddleware<CustomExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("../swagger/v1/swagger.json", "RapidPay - Payment API");
    c.RoutePrefix = String.Empty;
});

app.UseHealthChecks("/health");

app.Logger.LogInformation("Starting the app");
app.Run();
