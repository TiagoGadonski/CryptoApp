namespace CryptoApp.Models
{
    public class Stock
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double Change { get; set; } // Variação percentual
        public double ChangePercentage { get; set; }
    }
}
