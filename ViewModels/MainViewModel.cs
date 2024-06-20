using CryptoApp.Models;
using CryptoApp.Services;
using CryptoApp.Utilities;
using CryptoApp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CryptoApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double targetPrice;
        private string statusMessage;
        private ObservableCollection<CryptoCurrency> cryptos;
        private readonly MarketDataService marketDataService;
        private readonly DispatcherTimer _timer;

        public double TargetPrice
        {
            get => targetPrice;
            set
            {
                targetPrice = value;
                OnPropertyChanged(nameof(TargetPrice));
            }
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
            get => cryptos;
            set
            {
                cryptos = value;
                OnPropertyChanged(nameof(Cryptos));
            }
        }

        public ICommand SetAlertCommand { get; private set; }
        public ICommand OpenDetailViewCommand { get; private set; }
        public ICommand RefreshDataCommand { get; private set; }

        public MainViewModel()
        {
            SetAlertCommand = new RelayCommand(SetTargetPrice);
            OpenDetailViewCommand = new RelayCommand(OpenDetailView);
            RefreshDataCommand = new RelayCommand(_ => LoadData());
            marketDataService = new MarketDataService();
            Cryptos = new ObservableCollection<CryptoCurrency>();
            LoadData();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMinutes(5); // Set the refresh interval
            _timer.Tick += (sender, args) => LoadData();
            _timer.Start();
        }

        private void OpenDetailView(object parameter)
        {
            CryptoDetailView detailView = new CryptoDetailView();
            detailView.Show(); // Mostra a nova janela
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
                var cryptoData = await marketDataService.FetchCryptoData();
                Cryptos.Clear();
                foreach (var crypto in cryptoData)
                {
                    Cryptos.Add(crypto);
                }
                StatusMessage = "Data loaded successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load data: {ex.Message}";
            }
        }

        private void ShowNotification(string message)
        {
            var notification = new NotifyIcon()
            {
                Visible = true,
                Icon = SystemIcons.Information,
                BalloonTipTitle = "CryptoApp Notification",
                BalloonTipText = message
            };
            notification.ShowBalloonTip(3000);
            notification.Dispose(); // Dispose after showing the notification
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

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
