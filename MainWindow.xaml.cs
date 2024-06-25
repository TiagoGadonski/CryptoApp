using CryptoApp.ViewModels;
using System.Windows;

namespace CryptoApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            if (textBox != null)
            {
                (DataContext as MainViewModel)?.FilterCommand.Execute(textBox.Text);
            }
        }

        private void ToggleMode_Checked(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainViewModel;
            if (viewModel != null)
            {
                viewModel.IsStockMode = true;
            }
        }

        private void ToggleMode_Unchecked(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainViewModel;
            if (viewModel != null)
            {
                viewModel.IsStockMode = false;
            }
        }
    }
}
