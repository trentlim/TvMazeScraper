using System;
using Microsoft.EntityFrameworkCore;
using TvMazeScraper.Infrastructure.Models;

namespace TvMazeScraper.Infrastructure.Data
{
    public class TvShowRepository
    {
        private readonly TvShowDbContext _context;

        public TvShowRepository(TvShowDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<TvShow>> GetPaginatedTvShowsWithCastAsync(int pageNumber, int pageSize)
        {
            return await _context.TvShows
                 .Include(tvShow => tvShow.Cast
                     .OrderByDescending(castMember => castMember.Birthday))
                 .OrderBy(tvShow => tvShow.Id)
                 .Where(tvShow => tvShow.Id > (pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
        }

        public async Task<TvShow?> FindAsync(int id)
        {
            return await _context.TvShows
                .FindAsync(id);
        }

        public async Task<IEnumerable<int>> GetAllTvShowIdsAsync()
        {
            return await _context.TvShows
                .Select(t=> t.Id)
                .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<TvShow> tvShows)
        {
            await _context.TvShows.AddRangeAsync(tvShows);
            await _context.SaveChangesAsync();
        }
    }
}
