using System.Text.RegularExpressions;
using Smokeball.Application.SearchEngines;
using Smokeball.Domain;

public class GoogleSearchService : IGoogleSearchService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private const int MaxResults = 100;

    public GoogleSearchService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<SearchResult> Search(string keywords, string url)
    {
        var searchResult = new SearchResult { Positions = new List<int>() };
        var searchUrl = $"https://www.google.com/search?q={Uri.EscapeDataString(keywords)}&num={MaxResults}";

        MatchCollection? matches = null;
        using (var client = _httpClientFactory.CreateClient())
        {
            var response = await client.GetStringAsync(searchUrl);

            var regex = new Regex(@"<a href=""\/url\?q=(https?:\/\/[^\&""]+)", RegexOptions.Compiled);
            matches = regex.Matches(response);
        }
        if (matches != null)
        {
            int position = 1;
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    var resultUrl = match.Groups[1].Value;

                    if (resultUrl.Contains(url, StringComparison.OrdinalIgnoreCase))
                    {
                        searchResult.Positions.Add(position);
                    }

                    position++;
                }
            }
        }


        return searchResult;
    }
}
