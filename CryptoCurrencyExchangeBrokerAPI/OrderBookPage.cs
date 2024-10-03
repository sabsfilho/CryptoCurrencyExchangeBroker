using CryptoCurrencyExchangeBrokerLib.exchange;
using CryptoCurrencyExchangeBrokerLib.orderbook;
using System.ComponentModel;
using System.Security.Cryptography.Xml;

namespace CryptoCurrencyExchangeBrokerAPI
{
    internal static class OrderBookPage
    {
        internal static string Get(OrderBookState? state)
        {
            string body = string.Empty;
            string cryptoKey = string.Empty;
            string moneyKey = string.Empty;
            if (state == null)
                body = @"<div class=""warn"">Crypto Order book is empty. You need to subsribe your Instrument first.</div>";
            else
            {
                string instrument = state!.Instrument;
                cryptoKey = instrument.Substring(0, 3).ToUpper();
                moneyKey = instrument.Substring(3).ToUpper();

                var xs = new List<string> {
@"<table>
<thead>
    <tr>
        <th colspan=""2"">Bids</th>
        <th colspan=""2"">Asks</th>
    </tr>
    <tr>
        <th>Amout (", cryptoKey, @")</th>
        <th>Price (", moneyKey, @")</th>
        <th>Amout (", cryptoKey, @")</th>
        <th>Price (", moneyKey, @")</th>
    </tr>
</thead>"
                };

                var bids = state.Bids;
                var asks = state.Asks;

                int len =
                    Math.Max(
                        bids == null ? 0 : bids.Length,
                        asks == null ? 0 : asks.Length
                    );

                for(int i = 0; i < len; i++)
                {
                    string cryptoBidAmount = string.Empty;
                    string cryptoBidPrice = string.Empty;
                    string cryptoAskAmount = string.Empty;
                    string cryptoAskPrice = string.Empty;

                    /*
                    if (bids != null && i < bids.Length)
                    {
                        cryptoBidAmount = bids[i].Amount.ToString
                    }*/
                    var bidr = GetCellValues(bids, i);
                    var askr = GetCellValues(asks, i);

                    xs.Add(string.Concat(@"
<tr>
<td>", bidr.Amount, @"</td>
<td>", bidr.Price, @"</td>
<td>", askr.Amount, @"</td>
<td>", askr.Price, @"</td>
</tr>
"));
                }

                xs.Add("</table>");

                body = string.Join(string.Empty, xs);
            }

            string tit = state == null ? string.Empty : string.Concat("<b>", cryptoKey, "@", moneyKey, "</b>");

            return string.Concat(
@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <meta http-equiv=""refresh"" content=""5"">
    <title>CryptoCurrencyExchangeBroker App</title>
    <style>
        body {
            font-family:arial;
            font-size:16px;
            margin: 10px 20px
        }
        .title{
            font-size:18px;
            font-weight: bold;
            text-align:center;
            margin-bottom:20px
        }
        .main {
          display: flex;
          justify-content: center;
          flex-direction: column;
        }
        .sidenote{
            margin-top: 50px
        }
        .obs{
            font-size:12px
        }
        .warn{
            font-size:16px;
            color: red;
            margin:20px
        }
        table tr:nth-child(even) {
            background: #adbecc;
        }
        table tr td
        {
            text-align:center
        }
    </style>
</head>
<body>
<div class=""main"">
<div class=""title"">Crypto Order Book", tit, @"</div>
<div class=""obs"">* five seconds refresh rate</div>
",
                body,
                @"
</div>
<p class=""sidenote""><a href=""https://github.com/sabsfilho/CryptoCurrencyExchangeBroker"">Click here to see this project code and artifacts on my GitHub repository.</a></p>
</body>
</html>"
            );
        }

        private static (string Amount, string Price) GetCellValues(OrderBookItem[]? xs, int i)
        {
            return
                xs == null || i >= xs.Length ?
                (
                    string.Empty, 
                    string.Empty
                ) :
                (
                    xs[i].Amount.ToString("f8"),
                    xs[i].Price.ToString("f8")
                );
        }
    }
}
