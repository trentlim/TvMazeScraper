using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<IEnumerable<TvShow>> GetAllTvShowsWithCastAsync(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 1 : pageSize;
            return await _tvShowRepository.GetPaginatedTvShowsWithCastAsync(pageNumber, pageSize);
        }
    }
}
