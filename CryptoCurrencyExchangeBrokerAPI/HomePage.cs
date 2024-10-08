﻿namespace CryptoCurrencyExchangeBrokerAPI
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
    <style>
        body {
            font-family:arial;
            margin: 10px 20px
        }
        .title{
            font-size:18px;
            font-weight: bold;
            text-align:center
        }
        .subtitle {
            font-size: 20px;
            font-weight: bold;
            text-align: center;
            text-decoration:underline double
        }
    </style>
</head>
<body>
    <div class=""title"">Hello CryptoCurrencyExchangeBroker User!</div>
    <div>
        <div><b>API Endpoints</b></div>
        <div>
            <ul>
                <li><a href=""/start"" target=""_blank"">start</a></li>
                <li><a href=""/stop"" target=""_blank"">stop</a></li>
                <li><a href=""/subscribe-order-book?instrument=btcusd"" target=""_blank"">subscribe-order-book-btcusd</a></li>
                <li><a href=""/unsubscribe-order-book?instrument=btcusd"" target=""_blank"">unsubscribe-order-book-btcusd</a></li>
                <li><a href=""/order-book?instrument=btcusd"" target=""_blank"">order-book-btcusd</a></li>
                <li><a href=""/best-price?instrument=btcusd&buy=true&cryptoAmount=0.5"" target=""_blank"">best-price-btcusd buy=true&cryptoAmount=0.5</a></li>
                <li><a href=""/order-book-cosmosdb?instrument=btcusd"" target=""_blank"">order-book-cosmosdb-btcusd</a></li>
                <li><a href=""/order-book-report?instrument=btcusd"" target=""_blank"">order-book-report-btcusd</a> <b>==> html user interface</b></li>
            </ul>
            <p>
                <i>
                    <p>side notes:</p>
                    <ul>
                        <li>
                            I recommend click on /subscribe-order-book-btcusd and then /order-book-report-btcusd
                        </li>
                        <li>
                            These API's examples are using the Bitcoin (BTC) to Dollar (USD) instrument.<br />
                            You can change to other instrument as <b>ethusd</b> to query Ethereum (ETH) to Dollar (USD) instrument.
                        </li>
                        <li>As I have a limited individual developer budget on my personal Azure account, I let these channel subscriptions disconnected.<br/>
                        But you can click on the ""/subscribe-order-book-btcusd"" to start the streaming process.</li>
                        <li>
                            I configured it to automatically disconnect after 2 minutes on the server side.<br/>
                            After connecting, you can click on the best price quote link:
                            ""/best-price-btcusd buy=true&cryptoAmount=0.5""
                        </li>
                        <li>I turned off the Cosmos DB persistence feature of this microservice on its configuration file because the Crypto metadata is a huge volume of real time data.</li>
                    </ul>
                </i>
            </p>
        </div>
        <div class=""subtitle"">Service Oriented Architecture using AZURE cloud resources</div>
        <p>CryptoCurrency .NET 8 Minimal API solution using the <a href=""https://www.bitstamp.net/websocket/v2/"">Bitstamp.net crypto exchange Websocket API</a>. Adopted GitHub Actions CI/CD pipeline to Azure Web App and also consuming Azure Cosmos DB with Entity Framework. Development based on Parallel Programming, SOLID, Separation of Concerns, Clean Code, Object Oriented and Domain Driven Design best practices.</p>
        <div>
            <p><b>Project modules detail</b></p>
            <p>
                <b>CryptoCurrencyExchangeBrokerConsole</b>: .NET 8 Console Application created to test all the API functionalities and other features.
                <ul>
                    <li>I created a questionnaire flow to help the user explore all functionalities, including logs and tests.</li>
                </ul>
            </p>
            <p>
                <b>CryptoCurrencyExchangeBrokerAPI</b>: .NET 8 Minimal API
                <div><i>Environment Configuration</i></div>
                <ul>
                    <li>AutoStart: initiate the Default (btcusd,ethusd) Instruments subscription at the beginning of the Application initialization.</li>
                    <li>AutoStop: to reduce cloud costs automatically ends the signal streaming channels after 5 minutes (configured).</li>
                    <li>DatabaseEnabled: also to reduce cloud costs, the persistence layer can be turned off.</li>
                    <li>WriteLimitPerSession: limits the number of writer per session to control database unexpected growth</li>
                </ul>
            </p>
            <p>
                <b>CryptoCurrencyExchangeBrokerLib</b>: .NET 8 Business Logic Decoupled Layer following the Separation of Concerns principles.
                <ul>
                    <li>Websocket control implemented (start,restart,stop,error handling)</li>
                    <li>Handling each Instrument on a separate channel to optimize processor speed individually</li>
                    <li>Parallel programing carefully using locks to avoid deadlocks or procesing rate issues</li>
                </ul>
            </p>
            <p>
                <b>BitstampLib</b>: .NET 8 Library to handle the signal and metadata from the <a href=""https://www.bitstamp.net/websocket/v2/"">Bitstamp crypto exchange Websocket provider</a>
            </p>
            <p>
            <p>
                <b>PersistenceLayerCosmosDBLib</b>: .NET 8 Persistence Layer Library to handle the Cosmos DB NoSQL Database resources using Entity Framework to hold the object relational mapping model.<br />
                <ul>
                    <li>Consuming Microsoft.EntityFrameworkCore.Cosmos Library</li>
                    <li>Using Azure Key Vault service to secure the connection string and sensitive environment parameters. It's very important to store these parameters encrypted and outside from any project files. We need to keep in mind GitHub is a great code control product but it's a cloud solution and our sensitive information can be exploited.</li>
                </ul>
            </p>
            <p>I've created a GitHub Actions CI/CD pipeline to Azure Web App and I also configured it to be public.<br/>
            So, it's possible to consume this API using <a href=""https://www.postman.com/"" target=""_blank"">Postman</a> or <a href=""https://swagger.io/tools/swagger-ui/"" target=""_blank"">SWAGGER UI</a>
            </p>
        </div>
        <p><a href=""https://github.com/sabsfilho/CryptoCurrencyExchangeBroker"">Click here to see this project code and artifacts on my GitHub repository.</a></p>
        <p>If you have any concerns, please let me know.</p>
        <p>
            Cordially,<br />
            Samuel<br /><a href=""https://sabsfilho.github.io/dev/"" target=""_blank"">https://sabsfilho.github.io/dev/</a>
        </p>
    </div>
</body>
</html>
";
        }
    }
}
