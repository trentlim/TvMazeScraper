using System;
using Microsoft.EntityFrameworkCore;
using TvMazeScraper.Infrastructure.Models;

namespace TvMazeScraper.Infrastructure.Data
{
    public class CastMemberRepository
    {
        private readonly TvShowDbContext _context;

        public CastMemberRepository(TvShowDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddRangeAsync(IEnumerable<CastMember> castMembers)
        {
            await _context.CastMembers.AddRangeAsync(castMembers);
            await _context.SaveChangesAsync();
        }
    }
}
