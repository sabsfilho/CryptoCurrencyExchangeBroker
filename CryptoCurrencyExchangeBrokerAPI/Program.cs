using CryptoCurrencyExchangeBrokerAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var cryptoHandlerListener = new CryptoHandlerListener(app.Logger);

var cryptoHandler = new CryptoHandler(cryptoHandlerListener);

if (builder.Configuration["AutoStart"] == "true")
{
    cryptoHandler.Start();
}

app.MapGet("/", () => "Hello CryptoCurrencyExchangeBroker User!");

app.MapGet("/start", () => cryptoHandler.Start());

app.MapGet("/stop", () => cryptoHandler.Stop());

app.MapGet("/subscribe-order-book", (string instrument) => cryptoHandler.SubscribeOrderBook(instrument));

app.MapGet("/unsubscribe-order-book", (string instrument) => cryptoHandler.UnsubscribeOrderBook(instrument));

app.MapGet("/order-book", (string instrument) => cryptoHandler.GetOrderBookState(instrument));

app.MapGet("/best-price", (string instrument, bool buy, decimal cryptoAmount) => cryptoHandler.GetBestPrice(instrument, buy, cryptoAmount));

app.Run();
