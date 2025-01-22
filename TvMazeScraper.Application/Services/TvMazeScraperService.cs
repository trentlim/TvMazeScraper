using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TvMazeScraper.Application.DTOs;
using TvMazeScraper.Infrastructure.Data;
using TvMazeScraper.Infrastructure.Models;

namespace TvMazeScraper.Application.Services
{
    public class TvMazeScraperService
    {
        private readonly TvShowRepository _tvShowRepository;
        private readonly HttpClient _httpClient;

        public TvMazeScraperService(TvShowRepository tvShowRepository, HttpClient httpClient)
        {
            _tvShowRepository = tvShowRepository;
            _httpClient = httpClient;
        }

        private async Task ScrapeAllShowsAsync()
        {
            var shows = new List<TvMazeShowDto>();
            var baseAddress = _httpClient.BaseAddress;
            var pageNumber = 0;
            var hasMorePages = true;

            while (hasMorePages)
            {
                var response = await _httpClient.GetAsync($"shows?page={pageNumber}");
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    hasMorePages = false;
                }
                else if (response.IsSuccessStatusCode)
                {
                    var pageShows = await response.Content.ReadFromJsonAsync<List<TvMazeShowDto>>();
                    if (pageShows is not null && pageShows.Count > 0)
                    {
                        shows.AddRange(pageShows);
                    }
                }
                else
                {
                    throw new HttpRequestException($"Failed to retrieve shows from TVMaze API. Status code: {response.StatusCode}");
                }

                ++pageNumber;
            }

            await StoreTvShowsAsync(shows);
        }

        private async Task ScrapeCastForShowAsync()
        {
            throw new NotImplementedException();
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
