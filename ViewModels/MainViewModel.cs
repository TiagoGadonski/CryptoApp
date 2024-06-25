using CryptoApp.Models;
using CryptoApp.Services;
using CryptoApp.Utilities;
using CryptoApp.Views;
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace CryptoApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double targetPrice;
        public SeriesCollection HistoricalChartSeries { get; set; }
        public string[] HistoricalLabels { get; set; }
        public Func<double, string> PriceFormatter { get; set; }
        private string _priceAlertText;
        private string _searchText;
        private string statusMessage;
        private ObservableCollection<CryptoCurrency> _cryptos;
        public MarketDataService MarketDataService { get; private set; }
        private readonly DispatcherTimer _timer;
        private Notifier _notifier;

        private string selectedPeriod;
        public string SelectedPeriod
        {
            get => selectedPeriod;
            set
            {
                selectedPeriod = value;
                OnPropertyChanged(nameof(SelectedPeriod));
                if (!string.IsNullOrEmpty(SelectedCryptoSymbol))
                {
                    LoadHistoricalData(SelectedCryptoSymbol, selectedPeriod);
                }
            }
        }

        private string selectedCryptoSymbol;
        public string SelectedCryptoSymbol
        {
            get => selectedCryptoSymbol;
            set
            {
                selectedCryptoSymbol = value;
                OnPropertyChanged(nameof(SelectedCryptoSymbol));
                if (!string.IsNullOrEmpty(selectedPeriod))
                {
                    LoadHistoricalData(selectedCryptoSymbol, selectedPeriod);
                }
            }
        }

        public ObservableCollection<string> PeriodOptions { get; set; }

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(nameof(SearchText)); FilterCommand.Execute(value); }
        }
        public double TargetPrice
        {
            get => targetPrice;
            set
            {
                targetPrice = value;
                OnPropertyChanged(nameof(TargetPrice));
            }
        }
        public string PriceAlertText
        {
            get => _priceAlertText;
            set { _priceAlertText = value; OnPropertyChanged(nameof(PriceAlertText)); }
        }
        public string StatusMessage
        {
            get => statusMessage;
            set
            {
                statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public ObservableCollection<CryptoCurrency> Cryptos
        {
            get => _cryptos;
            set { _cryptos = value; OnPropertyChanged(nameof(Cryptos)); }
        }
        public ObservableCollection<Stock> Stocks { get; set; }
        private bool _isStockMode;

        public ICommand SetAlertCommand { get; private set; }
        public ICommand OpenDetailViewCommand { get; private set; }
        public ICommand RefreshDataCommand { get; private set; }
        public ICommand FilterCommand { get; private set; }
        public ICommand OpenNewsCommand { get; private set; }

        public MainViewModel()
        {
            SetAlertCommand = new RelayCommand(SetTargetPrice);
            OpenDetailViewCommand = new RelayCommand(OpenDetailView);
            RefreshDataCommand = new RelayCommand(_ => LoadData());
            MarketDataService = new MarketDataService();
            OpenNewsCommand = new RelayCommand(OpenNews);
            Cryptos = new ObservableCollection<CryptoCurrency>();
            Stocks = new ObservableCollection<Stock>();
            LoadData();
            FilterCommand = new RelayCommand(FilterCryptos);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMinutes(5);
            _timer.Tick += (sender, args) => LoadData();
            _timer.Start();
            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });

            PeriodOptions = new ObservableCollection<string>
            {
                "1d", "1w", "1m", "1y"
            };
        }
        public bool IsStockMode
        {
            get => _isStockMode;
            set
            {
                _isStockMode = value;
                OnPropertyChanged(nameof(IsStockMode));
                if (_isStockMode)
                    LoadStocks();
                else
                    LoadCryptos();
            }
        }

        private void LoadStocks()
        {
        }

        private void LoadCryptos()
        {
        }

        private void OpenNews(object parameter)
        {
            NewsView newsView = new NewsView();
            newsView.DataContext = new NewsViewModel();
            newsView.Show();
        }

        private void FilterCryptos(object parameter)
        {
            string filter = parameter as string;
            if (string.IsNullOrEmpty(filter))
            {
                LoadData();
            }
            else
            {
                var filtered = MarketDataService.FilterCryptos(filter);
                Cryptos = new ObservableCollection<CryptoCurrency>(filtered);
            }
        }

        private void FilterStocks(object parameter)
        {
            string filter = parameter as string;
            if (string.IsNullOrEmpty(filter))
            {
                LoadData();
            }
            else
            {
                LoadData();
            }
        }

        private void InitializeChartData()
        {
            HistoricalChartSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Price",
                    Values = new ChartValues<decimal>(Cryptos.Select(c => c.Quote["USD"].Price))
                }
            };

            HistoricalLabels = Cryptos.Select(c => c.Symbol).ToArray();
            OnPropertyChanged(nameof(HistoricalChartSeries));
            OnPropertyChanged(nameof(HistoricalLabels));
        }

        private async void LoadHistoricalData(string cryptoSymbol, string period)
        {
            try
            {
                var historicalData = await MarketDataService.FetchHistoricalData(cryptoSymbol, period);
                HistoricalChartSeries = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Price",
                        Values = new ChartValues<decimal>(historicalData.Select(d => d.Price))
                    }
                };

                HistoricalLabels = historicalData.Select(d => d.Date.ToShortDateString()).ToArray();
                OnPropertyChanged(nameof(HistoricalChartSeries));
                OnPropertyChanged(nameof(HistoricalLabels));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load historical data: {ex.Message}";
            }
        }

        private void OpenDetailView(object parameter)
        {
            CryptoDetailView detailView = new CryptoDetailView();
            detailView.Show();
        }

        private void SetTargetPrice(object parameter)
        {
            if (double.TryParse(parameter.ToString(), out double result))
            {
                TargetPrice = result;
                StatusMessage = $"Target price set to {result:C}";
            }
        }

        private async void LoadData()
        {
            try
            {
                var cryptoData = await MarketDataService.FetchCryptoData();
                Cryptos.Clear();
                foreach (var crypto in cryptoData)
                {
                    Cryptos.Add(crypto);
                    Debug.WriteLine($"Loaded: {crypto.Symbol} - Change 24h: {crypto.Quote["USD"].PercentChange24h}");
                }
                InitializeChartData();
                StatusMessage = "Data loaded successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load data: {ex.Message}";
                Debug.WriteLine(ex.ToString());
            }
        }

        private void ShowNotification(string message)
        {
            _notifier.ShowInformation(message);
        }

        public void Dispose()
        {
            _notifier.Dispose();
        }

        public void CheckPriceAlerts(double currentPrice)
        {
            if (currentPrice >= TargetPrice)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    StatusMessage = $"Alert: Price reached {currentPrice:C}";
                    ShowNotification(StatusMessage);
                });
            }
        }

        private void FilterData(object parameter)
        {
            string? filter = parameter as string;
            if (string.IsNullOrEmpty(filter))
            {
                LoadData();
            }
            else
            {
                if (IsStockMode)
                {
                }
                else
                {
                    var filteredCryptos = MarketDataService.FilterCryptos(filter);
                    Cryptos = new ObservableCollection<CryptoCurrency>(filteredCryptos);
                }
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
