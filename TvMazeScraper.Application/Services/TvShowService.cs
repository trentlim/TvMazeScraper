using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvMazeScraper.Application.DTOs;
using TvMazeScraper.Common;
using TvMazeScraper.Infrastructure.Data;
using TvMazeScraper.Infrastructure.Models;

namespace TvMazeScraper.Application.Services
{
    public class TvShowService
    {
        private readonly TvShowRepository _tvShowRepository;
        public TvShowService(TvShowRepository tvShowRepository)
        {
            _tvShowRepository = tvShowRepository;
        }
        public async Task<IEnumerable<ShowsResponseDto>> GetPaginatedTvShowsWithCastAsync(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            if (pageSize < 1) { pageSize = 1; }
            else if (pageSize > PaginationConstants.MaxPageSize)
            {
                pageSize = PaginationConstants.MaxPageSize;
            }

            var shows = await _tvShowRepository.GetPaginatedTvShowsWithCastAsync(pageNumber, pageSize);
            return shows.Select(show =>
            {
                var castDtos = show.Cast.Select(cast => new ShowsResponseDto.CastMemberDto
                {
                    Id = cast.Id,
                    Name = cast.Name,
                    Birthday = cast.Birthday
                })
                .ToList();

                return new ShowsResponseDto
                {
                    Id = show.Id,
                    Name = show.Name,
                    Cast = castDtos
                };
            });
        }
    }
}
