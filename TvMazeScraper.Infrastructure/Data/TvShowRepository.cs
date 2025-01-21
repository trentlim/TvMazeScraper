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

        public async Task<IEnumerable<TvShow>> GetTvShowsWithCastAsync()
        {
            return await _context.TvShows
                 .Include(t => t.Cast)
                 .ToListAsync();
        }
    }
}
