using CryptoApp.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace CryptoApp.Services
{
    public class MarketDataService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "145bcacc-f453-435b-81f4-e4a4f0cf1e8c";
        private List<CryptoCurrency> _allCryptos = new List<CryptoCurrency>();

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
                _allCryptos = cryptoResponse.Data;  // Update the internal list
                return _allCryptos;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to fetch crypto data", ex);
            }
        }

        public async Task<List<HistoricalData>> FetchHistoricalData(string cryptoSymbol, string period)
        {
            // Substitua pela URL correta e pelo método para buscar os dados históricos da API que você está usando
            string url = $"https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/historical?symbol={cryptoSymbol}&interval={period}&CMC_PRO_API_KEY={_apiKey}";
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accepts", "application/json");
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var historicalResponse = JsonConvert.DeserializeObject<HistoricalResponse>(responseBody);
                return historicalResponse.Data;  // Retorna uma lista de dados históricos
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to fetch historical data", ex);
            }
        }

        public IEnumerable<CryptoCurrency> FilterCryptos(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return _allCryptos;
            }

            filter = filter.ToLower();
            return _allCryptos.Where(crypto =>
                crypto.Name.ToLower().Contains(filter) ||
                crypto.Symbol.ToLower().Contains(filter));
        }

        public CryptoCurrency GetCryptoBySymbol(string symbol)
        {
            return _allCryptos.FirstOrDefault(c => c.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
        }
    }
}

public class HistoricalData
{
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
}

public class HistoricalResponse
{
    public List<HistoricalData> Data { get; set; }
}
