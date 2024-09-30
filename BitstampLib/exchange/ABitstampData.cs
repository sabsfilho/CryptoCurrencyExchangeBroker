using CryptoCurrencyExchangeBrokerLib.exchange;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BitstampLib.exchange;
internal abstract class ABitstampData<T> 
    where T : AExchangeData
{
    public abstract T Load(string instrument);
    protected Object Data { get; private set; }
    protected JsonElement DataJson => (JsonElement)Data;
    public T ExchangeData { get; set; }
    
    public ABitstampData(string instrument, object data)
    {
        Data = data;
        ExchangeData = Load(instrument);
    }
    protected OrderBookItem[] GetOffers(string fieldName)
    {
        return
            DataJson
                .GetProperty(fieldName)
                .EnumerateArray()
                .Select(x =>
                {
                    var arr = x.EnumerateArray().ToArray();
                    return
                        new OrderBookItem()
                        {
                            Amount = decimal.Parse(x[1].GetString()!),
                            Price = decimal.Parse(x[0].GetString()!)
                        };
                })
                .ToArray();
    }

    protected DateTime GetMicrotimestamp()
    {
        var v = DataJson.GetProperty("microtimestamp").GetString();

        if (v == null)
            throw new BitstampException("microtimestamp is undefined");

        return ConvertFromUnixDate(v);
    }

    private static DateTime ConvertFromUnixDate(string v)
    {
        long unixDate = long.Parse(v.PadRight(13, '0').Substring(0, 13));
        
        return
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            .AddMilliseconds(unixDate)
            .ToLocalTime();

    }
}
