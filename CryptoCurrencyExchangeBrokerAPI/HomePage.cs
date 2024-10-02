namespace CryptoCurrencyExchangeBrokerAPI
{
    internal class HomePage
    {
        public static string Get()
        {
            return @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <title>CryptoCurrencyExchangeBroker App</title>
</head>
<body>
    <div><b>Hello CryptoCurrencyExchangeBroker User!</b></div>
    <div>
        <div>API Endpoints</div>
        <div>
            <ul>
                <li><a href=""/start"">start</a></li>
                <li><a href=""/stop"">stop</a></li>
                <li><a href=""/subscribe-order-book?instrument=btcusd"">subscribe-order-book-btcusd</a></li>
                <li><a href=""/unsubscribe-order-book?instrument=btcusd"">unsubscribe-order-book-btcusd</a></li>
                <li><a href=""/order-book?instrument=btcusd"">order-book-btcusd</a></li>
                <li><a href=""/best-price?instrument=btcusd&buy=true&cryptoAmount=0.5"">best-price-btcusd buy=true&cryptoAmount=0.5</a></li>
                <li><a href=""/order-book-cosmosdb?instrument=btcusd"">order-book-cosmosdb-btcusd</a></li>
            </ul>
        </div>
        <p>CryptoCurrency .NET 8 Minimal API solution using the Bitstamp.net crypto exchange Websocket API. Adopted GitHub Actions CI/CD pipeline to Azure Web App and also consuming Azure Cosmos DB with Entity Framework. Development based on Parallel Programming, SOLID, Clean Code, Object Oriented and Domain Driven Design</p>
        <p><a href=""https://github.com/sabsfilho/CryptoCurrencyExchangeBroker"">Clique aqui para ver a estrutura desse projeto no GitHub.</a></p>
        <div>
            <p><b>CryptoCurrencyExchangeBrokerConsole</b>: .NET 8 Console Application created to test all the functionalities</p>
            <p><b>CryptoCurrencyExchangeBrokerAPI</b>: .NET 8 Minimal API</p>
        </div>
        <p>Se tiver alguma dúvida, favor entrar em contato.</p>
        <p>
            Cordialmente,<br />
            Samuel<br /><a href=""https://sabsfilho.github.io/dev/"">https://sabsfilho.github.io/dev/</a>
        </p>
    </div>
</body>
</html>
";
        }
    }
}
