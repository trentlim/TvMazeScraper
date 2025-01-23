using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TvMazeScraper.Application.DTOs;
using TvMazeScraper.Infrastructure.Data;
using TvMazeScraper.Infrastructure.Models;

namespace TvMazeScraper.Application.Services
{
    public class TvMazeScraperService
    {
        private readonly TvShowRepository _tvShowRepository;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TvMazeScraperService> _logger;

        public TvMazeScraperService(
            TvShowRepository tvShowRepository,
            HttpClient httpClient,
            ILogger<TvMazeScraperService> logger)
        {
            _tvShowRepository = tvShowRepository;
            _httpClient = httpClient;
            _logger = logger;
        }

        // Scrape shows
        private async Task ScrapeAllShowsAsync()
        {
            try
            {
                var pageNumber = 0;
                var tasks = new List<Task<List<TvMazeShowDto>>>();
                var hasMorePages = true;

                while (hasMorePages)
                {
                    var currentPage = pageNumber;
                    tasks.Add(FetchShowsForPageAsync(currentPage));

                    ++pageNumber;

                    if (tasks.Count >= 20)
                    {
                        var results = await Task.WhenAll(tasks);

                        // Check if the last task returned an empty list
                        if (results.Last().Count == 0)
                        {
                            hasMorePages = false;
                        }

                        await StoreTvShowsAsync(results.SelectMany(r => r));
                        tasks.Clear();
                    }
                }

                if (tasks.Count > 0)
                {
                    var results = await Task.WhenAll(tasks);
                    await StoreTvShowsAsync(results.SelectMany(r => r));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to scrape shows from TVMaze API.");
            }
        }

        private async Task<List<TvMazeShowDto>> FetchShowsForPageAsync(int pageNumber)
        {
            try
            {
                var response = await _httpClient.GetAsync($"shows?page={pageNumber}");
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new List<TvMazeShowDto>();
                }
                else if (response.IsSuccessStatusCode)
                {
                    var pageShows = await response.Content.ReadFromJsonAsync<List<TvMazeShowDto>>();
                    if (pageShows is not null && pageShows.Count > 0)
                    {
                        return pageShows;
                    }

                    return new List<TvMazeShowDto>();
                }
                else
                {
                    throw new HttpRequestException($"Failed to retrieve shows from TVMaze API (page {pageNumber}). Status code: {response.StatusCode}", null, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve shows from TVMaze API (page {pageNumber}).");
                return new List<TvMazeShowDto>();
            }
        }

        private async Task StoreTvShowsAsync(IEnumerable<TvMazeShowDto> showDtos)
        {
            var shows = showDtos.Select(s => new TvShow
            {
                Id = s.Id,
                Name = s.Name
            });
            if (shows.Any())
            {
                await _tvShowRepository.AddRangeAsync(shows);
            }
        }

        // Scrape casts
        private async Task ScrapeCastForShowAsync()
        {
            throw new NotImplementedException();
        }
        private async Task StoreCastMembersAsync(IEnumerable<TvMazeCastMemberDto> cast)
        {
            throw new NotImplementedException();
        }

        public async Task RunScraperAsync()
        {
            if (_httpClient.BaseAddress is null)
            {
                throw new InvalidOperationException("Base address for TVMaze API is not set.");
            }

            await ScrapeAllShowsAsync();
        }
    }
}
