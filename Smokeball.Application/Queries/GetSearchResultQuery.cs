using MediatR;
using Smokeball.Domain;

namespace Smokeball.Application.Queries
{
    public class GetSearchResultQuery : IRequest<SearchResult>
    {
        public string Keywords { get; set; }
        public string Url { get; set; }
    }
}
