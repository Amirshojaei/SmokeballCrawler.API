using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Smokeball.Domain;
using Smokeball.Application.SearchEngines;

namespace Smokeball.Application.Queries.GetSearchResult
{
    public class GetSearchResultQueryHandlerTests
    {
        private readonly Mock<IGoogleSearchService> _searchServiceMock;
        private readonly GetSearchResultQueryHandler _handler;

        public GetSearchResultQueryHandlerTests()
        {
            _searchServiceMock = new Mock<IGoogleSearchService>();
            _handler = new GetSearchResultQueryHandler(_searchServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCallSearchService_WithCorrectParameters()
        {
            // Arrange
            var query = new GetSearchResultQuery
            {
                Keywords = "test",
                Url = "http://somesite.com"
            };
            var expectedResult = new SearchResult { Positions = new List<int> { 1, 2 } };

            _searchServiceMock.Setup(service => service.Search(query.Keywords, query.Url))
                              .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult, result);
            _searchServiceMock.Verify(service => service.Search(query.Keywords, query.Url), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSearchResult()
        {
            // Arrange
            var query = new GetSearchResultQuery
            {
                Keywords = "test keyword",
                Url = "http://somesite.com"
            };
            var expectedResult = new SearchResult { Positions = new List<int> { 3, 4 } };

            _searchServiceMock.Setup(service => service.Search(query.Keywords, query.Url))
                              .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult, result);
        }
    }
}
