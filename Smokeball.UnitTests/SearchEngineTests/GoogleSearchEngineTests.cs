using Moq;
using Xunit;

public class GoogleSearchServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly GoogleSearchService _service;

    public GoogleSearchServiceTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _service = new GoogleSearchService(_httpClientFactoryMock.Object);
    }

    private HttpClient CreateHttpClient(string htmlResponse)
    {
        var messageHandler = new InMemoryHttpMessageHandler(htmlResponse);
        return new HttpClient(messageHandler);
    }

    [Fact]
    public async Task Search_ShouldReturnPositions()
    {
        // Arrange
        var keywords = "test";
        var url = "http://Somesite.com";
        var htmlResponse = @"
            <html>
                <body>
                    <a href=""/url?q=http://Somesite.com&sa=U"">test</a>
                    <a href=""/url?q=http://other.com&sa=U"">Other</a>
                </body>
            </html>";

        var httpClient = CreateHttpClient(htmlResponse);
        _httpClientFactoryMock.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                              .Returns(httpClient);

        // Act
        var result = await _service.Search(keywords, url);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(1, result.Positions);
    }

    [Fact]
    public async Task Search_ShouldReturnEmptyPositions()
    {
        // Arrange
        var keywords = "test";
        var url = "http://Somesite.com";
        var htmlResponse = @"
            <html>
                <body>
                    <a href=""/url?q=http://Somesitetest.com&sa=U"">test</a>
                </body>
            </html>";

        var httpClient = CreateHttpClient(htmlResponse);
        _httpClientFactoryMock.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                              .Returns(httpClient);

        // Act
        var result = await _service.Search(keywords, url);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Positions);
    }

    [Fact]
    public async Task Search_ShouldThrowException()
    {
        // Arrange
        var keywords = "test";
        var url = "http://Somesite.com";
        var httpClient = CreateHttpClient("This will raise an error");

        _httpClientFactoryMock.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                              .Throws<HttpRequestException>();

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _service.Search(keywords, url));
    }
}


public class InMemoryHttpMessageHandler : HttpMessageHandler
{
    private readonly string _responseContent;

    public InMemoryHttpMessageHandler(string responseContent)
    {
        _responseContent = responseContent;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(_responseContent),
        };

        return Task.FromResult(response);
    }
}
