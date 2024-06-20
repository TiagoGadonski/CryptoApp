namespace CryptoApp.Models
{
    public class Crypto
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double ChangePercentage { get; set; } // Assuming you need this as well
        public double Change { get; set; }           // Change in price, make sure this matches what you need
        public double Volume24h { get; set; }
    }
}
