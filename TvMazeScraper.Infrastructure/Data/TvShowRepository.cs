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

        public async Task<IEnumerable<TvShow>> GetAllTvShowsWithCastAsync()
        {
            return await _context.TvShows
                 .Include(t => t.Cast)
                 .ToListAsync();
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
