using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Smokeball.Domain;
using Xunit;


namespace Smokeball.IntegrationTests
{
    public class SearchControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public SearchControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Search_ShouldReturnOk_WithValidInput()
        {
            // Arrange
            var keyword = "dummykeyword";
            var url = "https://www.somesite.com";

            // Act
            var response = await _client.GetAsync($"/api/search?Keyword={keyword}&Url={url}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
         
            var result = JsonConvert.DeserializeObject<SearchResult>(responseContent);
            Assert.NotNull(result);
            Assert.NotNull(result.Positions);
        }

        [Theory]
        [InlineData("", "https://www.TEstSite.com")]
        [InlineData("mykeyword", "")]
        [InlineData("ab", "https://www.TEstSite.com")]
        [InlineData("mykeyword", "invalidurl")]
        public async Task Search_ShouldReturnBadRequest_WithInvalidInput(string keyword, string url)
        {
            // Act
            var response = await _client.GetAsync($"/api/search?Keyword={keyword}&Url={url}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
