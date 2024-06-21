namespace CryptoApp.Models
{
    public class HistoricalPrice
    {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
    }

    public class HistoricalPriceResponse
    {
        public HistoricalData Data { get; set; }
    }

    public class HistoricalData
    {
        public List<Quote> Quotes { get; set; }
    }

    public class Quote
    {
        public DateTime TimeClose { get; set; }
        public decimal Close { get; set; }
    }
}
