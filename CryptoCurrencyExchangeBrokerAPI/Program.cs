using CryptoCurrencyExchangeBrokerLib;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

if (builder.Configuration["AutoStart"] == "true")
{
    MarketDataControl.MarketDataInstance.Start();
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/", () => "Hello CryptoCurrencyExchangeBroker User!");

app.MapGet("/start", () => MarketDataControl.MarketDataInstance.Start());

app.MapGet("/stop", () => MarketDataControl.MarketDataInstance.Stop());

app.MapPost("/subscribe-order-book", (string instrument) => MarketDataControl.MarketDataInstance.Subscribe(ChannelEnum.OrderBook, instrument));

app.MapPost("/unsubscribe-order-book", (string instrument) => MarketDataControl.MarketDataInstance.Unsubscribe(ChannelEnum.OrderBook, instrument));


app.Run();
