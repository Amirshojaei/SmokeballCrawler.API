using MediatR;
using Microsoft.AspNetCore.Mvc;
using Smokeball.Application.Queries;
using System.Text.RegularExpressions;

namespace Smokeball.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IMediator _mediator;
        private static readonly Regex UrlRegex = new Regex(@"^((http|https)://)?(www\.)[^\s/$.?#].[^\s]*$", RegexOptions.Compiled);

        public SearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string Keyword, string Url)
        {
            if (string.IsNullOrWhiteSpace(Keyword) || Keyword.Length < 3)
            {
                return BadRequest("Keyword is required and must be at least 3 characters long.");
            }

            if (string.IsNullOrWhiteSpace(Url) || !UrlRegex.IsMatch(Url))
            {
                return BadRequest("Url is required and must be a valid URL.");
            }

            var query = new GetSearchResultQuery
            {
                Keywords = Keyword,
                Url = Url,
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
