using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CryptoApp.ViewModels
{
    public class CryptoDetailViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public SeriesCollection ChartSeries { get; set; }
        public ObservableCollection<string> XLabels { get; set; }

        public CryptoDetailViewModel()
        {
            ChartSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<double> { 3, 5, 7, 4 }, // Example values
                    Title = "Price"
                }
            };
            XLabels = new ObservableCollection<string> { "Jan", "Feb", "Mar", "Apr" };
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
