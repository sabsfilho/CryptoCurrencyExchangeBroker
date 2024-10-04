# CryptoCurrencyExchangeBroker<br/>

## Service Oriented Architecture using AZURE cloud resources<br/>

- CryptoCurrency .NET 8 Minimal API solution using the [Bitstamp.net crypto exchange Websocket API](https://www.bitstamp.net/websocket/v2/).<br/>
- Adopted GitHub Actions CI/CD pipeline to Azure Web App and also consuming Azure Cosmos DB with Entity Framework.<br/>
- Development based on Parallel Programming, SOLID, Separation of Concerns, Clean Code, Object Oriented and Domain Driven Design best practices.<br/>

### Project modules detail
__CryptoCurrencyExchangeBrokerConsole__: .NET 8 Console Application created to test all the API functionalities and other features.<br/>
- I created a questionnaire flow to help the user explore all functionalities, including logs and tests.<br/>

__CryptoCurrencyExchangeBrokerAPI__: .NET 8 Minimal API<br/>
_Environment Configuration_<br/>
  - AutoStart: initiate the Default (btcusd,ethusd) Instruments subscription at the beginning of the Application initialization.<br/>
  - AutoStop: to reduce cloud costs automatically ends the signal streaming channels after 5 minutes (configured).<br/>
  - DatabaseEnabled: also to reduce cloud costs, the persistence layer can be turned off.<br/>
  - WriteLimitPerSession: limits the number of writer per session to control database unexpected growth<br/>
  
__CryptoCurrencyExchangeBrokerLib__: .NET 8 Business Logic Decoupled Layer following the Separation of Concerns principles.<br/>
- Websocket control implemented (start,restart,stop,error handling)<br/>
- Handling each Instrument on a separate channel to optimize processor speed individually<br/>
- Parallel programing carefully using locks to avoid deadlocks or procesing rate issues<br/>

__BitstampLib__: .NET 8 Library to handle the signal and metadata from the Bitstamp crypto exchange Websocket provider<br/>

__PersistenceLayerCosmosDBLib__: .NET 8 Persistence Layer Library to handle the Cosmos DB NoSQL Database resources using Entity Framework to hold the object relational mapping model.<br/>
- Consuming Microsoft.EntityFrameworkCore.Cosmos Library<br/>
- Using Azure Key Vault service to secure the connection string and sensitive environment parameters. It's very important to store these parameters encrypted and outside from any project files. We need to keep in mind GitHub is a great code control product but it's a cloud solution and our sensitive information can be exploited.<br/>

I've created a GitHub Actions CI/CD pipeline to Azure Web App and I also configured it to be public.<br/>
So, it's possible to consume this API using [Postman](https://www.postman.com/) or [SWAGGER UI](https://swagger.io/tools/swagger-ui/)<br/>

__API Endpoints__<br/>
/start<br/>
/stop<br/>
/subscribe-order-book-btcusd<br/>
/unsubscribe-order-book-btcusd<br/>
/order-book-btcusd<br/>
/best-price-btcusd buy=true&cryptoAmount=0.5<br/>
/order-book-cosmosdb-btcusd<br/>
/order-book-report-btcusd __==> html user interface__<br/>

_*These API's examples are using the Bitcoin (BTC) to Dollar (USD) instrument._<br/>
_*You can change to other instrument as __ethusd__ to query Ethereum (ETH) to Dollar (USD) instrument._<br/>
_*I recommend click on /subscribe-order-book-btcusd and then /order-book-report-btcusd_<br/>
_*The /start endpoint initiates the Websocket streaming and subscribe the default instruments (__BTCUSD,ETHUSD__)_<br/>

![SWAGGER API DOC](https://sabsfilho.github.io/dev/assets/img/crypto/CryptoCurrencyExchangeBrokerSwaggerUI.png)

If you have any ideas to improve this project, please let me know.<br/>
Let's team up!<br/>

Cheers,<br/>
Samuel<br/>
https://sabsfilho.github.io/dev/<br/>

**document under construction**
