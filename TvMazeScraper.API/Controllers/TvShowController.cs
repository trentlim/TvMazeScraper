using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TvMazeScraper.Application.DTOs;
using TvMazeScraper.Application.Services;
using TvMazeScraper.Infrastructure.Models;

namespace TvMazeScraper.API.Controllers
{
    [ApiController]
    public class TvShowController : ControllerBase
    {
        private readonly ILogger<TvShowController> _logger;
        private readonly TvShowService _tvShowService;
        public TvShowController(ILogger<TvShowController> logger, TvShowService tvShowService)
        {
            _logger = logger;
            _tvShowService = tvShowService;
        }

        // GET /shows
        [HttpGet]
        [Route("shows")]
        public async Task<ActionResult<IEnumerable<ShowsResponseDto>>> GetShows([FromQuery] ShowsRequestDto request)
        {
            try
            {
                var shows = await _tvShowService.GetPaginatedTvShowsWithCastAsync(request.PageNumber, request.PageSize);
                return Ok(shows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calling GET /shows endpoint.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
