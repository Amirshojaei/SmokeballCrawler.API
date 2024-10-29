using Smokeball.Domain;
namespace Smokeball.Application.SearchEngines
{
    public interface IGoogleSearchService
    {
        Task<SearchResult> Search(string keywords, string url);
    }
}
