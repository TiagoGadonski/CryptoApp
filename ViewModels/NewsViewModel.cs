using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;

public class NewsViewModel : INotifyPropertyChanged
{
    public ObservableCollection<NewsArticle> NewsArticles { get; set; }
    private HttpClient _httpClient = new HttpClient();

    public NewsViewModel()
    {
        NewsArticles = new ObservableCollection<NewsArticle>();
        LoadNews();
    }

    private async void LoadNews()
    {
        try
        {
            string apiKey = "your_api_key_here"; // Substitua pelo sua chave API real
            string url = $"https://newsapi.org/v2/everything?q=cryptocurrencies&apiKey={apiKey}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                NewsResponse newsResponse = JsonConvert.DeserializeObject<NewsResponse>(jsonString);
                foreach (var article in newsResponse.Articles)
                {
                    NewsArticles.Add(article);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error loading news: " + ex.Message);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class NewsResponse
{
    public List<NewsArticle> Articles { get; set; }
}

public class NewsArticle
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime PublishedAt { get; set; }
}
