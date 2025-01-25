using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly CastMemberRepository _castMemberRepository;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TvMazeScraperService> _logger;

        public TvMazeScraperService(
            TvShowRepository tvShowRepository,
            CastMemberRepository castMemberRepository,
            HttpClient httpClient,
            ILogger<TvMazeScraperService> logger
        )
        {
            _tvShowRepository = tvShowRepository;
            _castMemberRepository = castMemberRepository;
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
            if (!showDtos.Any()) { return; }

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
        private async Task ScrapeCastForAllShowsAsync(IEnumerable<int> showIds)
        {
            if (!showIds.Any()) { return; }

            var tasks = new List<Task<List<TvMazeCastMemberDto>>>();
            foreach (var showId in showIds)
            {
                tasks.Add(FetchCastForShowAsync(showId));

                if (tasks.Count >= 20)
                {
                    var results = await Task.WhenAll(tasks);
                    await StoreCastMembersAsync(results.SelectMany(r => r));
                    tasks.Clear();
                }
            }

            if (tasks.Count > 0)
            {
                var results = await Task.WhenAll(tasks);
                await StoreCastMembersAsync(results.SelectMany(r => r));
            }
        }
        private async Task<List<TvMazeCastMemberDto>> FetchCastForShowAsync(int showId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"shows/{showId}/cast");
                if (response.IsSuccessStatusCode)
                {
                    var cast = await response.Content.ReadFromJsonAsync<List<TvMazeCastMemberDto>>();
                    if (cast is not null && cast.Count > 0)
                    {
                        foreach (var member in cast)
                        {
                            member.ShowId = showId;
                        }
                        return cast;
                    }

                    return new List<TvMazeCastMemberDto>();
                }
                else
                {
                    throw new HttpRequestException($"Failed to retrieve cast from TVMaze API (showId: {showId}). Status code: {response.StatusCode}", null, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve cast from TVMaze API (showId: {showId}).");
                return new List<TvMazeCastMemberDto>();
            }
        }

        private async Task StoreCastMembersAsync(IEnumerable<TvMazeCastMemberDto> castDtos)
        {
            try
            {
                if (!castDtos.Any()) { return; }

                var cast = new HashSet<CastMember>();
                foreach (var member in castDtos)
                {
                    var show = await _tvShowRepository.FindAsync(member.ShowId)
                        ?? throw new KeyNotFoundException($"TV show with ID {member.ShowId} not found when attempting to add cast to show");

                    var existingCastMember = await _castMemberRepository.FindAsync(member.Person.Id);
                    if (existingCastMember is not null)
                    {
                        existingCastMember.TvShows.Add(show);
                        continue;
                    }

                    cast.Add(new CastMember
                    {
                        Id = member.Person.Id,
                        Name = member.Person.Name,
                        Birthday = member.Person.Birthday,
                        TvShows = new List<TvShow> { show }
                    });
                }

                await _castMemberRepository.AddNewCastMembersAsync(cast);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when storing cast members in database.");
            }
        }

        public async Task RunScraperAsync()
        {
            if (_httpClient.BaseAddress is null)
            {
                throw new InvalidOperationException("Base address for TVMaze API is not set.");
            }

            await ScrapeAllShowsAsync();
            var showIds = await _tvShowRepository.GetAllTvShowIdsAsync();
            await ScrapeCastForAllShowsAsync(showIds);
            var x = await _tvShowRepository.GetAllTvShowsWithCastAsync();
            var y = x.Last();
        }
    }
}
