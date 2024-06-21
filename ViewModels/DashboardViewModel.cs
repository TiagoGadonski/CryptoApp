using CryptoApp.Models;
using CryptoApp.Services;
using CryptoApp.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace CryptoApp.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<CryptoCurrency> _favoriteCryptos;
        private List<CryptoCurrency> _allCryptos; // Declaração da lista completa de criptomoedas
        public MarketDataService MarketDataService { get; private set; }

        public ObservableCollection<CryptoCurrency> FavoriteCryptos
        {
            get => _favoriteCryptos;
            set
            {
                _favoriteCryptos = value;
                OnPropertyChanged(nameof(FavoriteCryptos));
            }
        }

        public ICommand AddFavoriteCommand { get; private set; }
        private string searchText;
        private readonly MarketDataService marketDataService;

        public ICommand RemoveFavoriteCommand { get; private set; }

        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }

        public DashboardViewModel()
        {
            _allCryptos = new List<CryptoCurrency>(); // Inicialização da lista
            marketDataService = new MarketDataService(); // Inicialização do serviço de dados
            MarketDataService = marketDataService; // Atribuição da instância ao campo público
            LoadCryptos(); // Carrega as criptomoedas disponíveis após a inicialização do serviço
            AddFavoriteCommand = new RelayCommand(AddFavorite);
            _favoriteCryptos = new ObservableCollection<CryptoCurrency>();
            RemoveFavoriteCommand = new RelayCommand(RemoveFavoriteCrypto);
            FavoriteCryptos = new ObservableCollection<CryptoCurrency>();
        }
        private async void LoadCryptos()
        {
            try
            {
                var cryptoData = await MarketDataService.FetchCryptoData();
                _allCryptos.Clear();
                foreach (var crypto in cryptoData)
                {
                    _allCryptos.Add(crypto);
                    Debug.WriteLine($"Loaded: {crypto.Symbol} - Change 24h: {crypto.Quote["USD"].PercentChange24h}");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString()); // Provides more detail in the output window
            }
        }

        private void AddFavorite(object parameter)
        {
            string symbol = parameter as string;
            var cryptoToAdd = _allCryptos.FirstOrDefault(c => c.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
            if (cryptoToAdd != null && !FavoriteCryptos.Any(c => c.Symbol == cryptoToAdd.Symbol))
            {
                FavoriteCryptos.Add(cryptoToAdd);
            }
        }
        private void AddFavoriteCrypto(object parameter)
        {
            string symbol = parameter as string;
            var cryptoToAdd = _allCryptos.FirstOrDefault(c => c.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
            if (cryptoToAdd != null && !FavoriteCryptos.Any(c => c.Symbol == cryptoToAdd.Symbol))
            {
                FavoriteCryptos.Add(cryptoToAdd);
            }
        }

        private void RemoveFavoriteCrypto(object parameter)
        {
            var cryptoSymbol = parameter as string;
            var crypto = FavoriteCryptos.FirstOrDefault(c => c.Symbol == cryptoSymbol);
            if (crypto != null)
            {
                FavoriteCryptos.Remove(crypto);
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
