namespace CryptoApp.Models
{
    public class CryptoCurrency
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public Dictionary<string, CryptoQuote> Quote { get; set; }
    }
}
