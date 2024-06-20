using CryptoApp.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace CryptoApp.Services
{
    public class MarketDataService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "145bcacc-f453-435b-81f4-e4a4f0cf1e8c";  // Substitua com sua chave real

        public MarketDataService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<CryptoCurrency>> FetchCryptoData()
        {
            string url = $"https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest?CMC_PRO_API_KEY={_apiKey}";
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accepts", "application/json");
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var cryptoResponse = JsonConvert.DeserializeObject<CryptoResponse>(responseBody);
                return cryptoResponse.Data;  // Returns a list of CryptoCurrency objects
            }
            catch (Exception ex)
            {
                // Handle exceptions or log them
                throw new Exception("Failed to fetch crypto data", ex);
            }
        }

        private Crypto[] ParseCryptoData(dynamic data)
        {
            var cryptos = new List<Crypto>();
            foreach (var item in data.data)
            {
                cryptos.Add(new Crypto
                {
                    Symbol = item.symbol,
                    Name = item.name,
                    Price = item.quote.USD.price,
                    Change = item.quote.USD.percent_change_24h,
                    ChangePercentage = item.quote.USD.percent_change_24h, // Assuming you want the same value here
                    Volume24h = item.quote.USD.volume_24h
                });
            }
            return cryptos.ToArray();
        }
    }
}
