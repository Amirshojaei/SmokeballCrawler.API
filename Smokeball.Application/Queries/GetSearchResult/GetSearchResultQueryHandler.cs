using MediatR;
using Smokeball.Application.SearchEngines;
using Smokeball.Domain;

namespace Smokeball.Application.Queries.GetSearchResult
{
    public class GetSearchResultQueryHandler : IRequestHandler<GetSearchResultQuery, SearchResult>
    {
        private readonly IGoogleSearchService _searchService;

        public GetSearchResultQueryHandler(IGoogleSearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<SearchResult> Handle(GetSearchResultQuery request, CancellationToken cancellationToken)
        {
            var result = await _searchService.Search(request.Keywords, request.Url);
            return result;
        }
    }
}
