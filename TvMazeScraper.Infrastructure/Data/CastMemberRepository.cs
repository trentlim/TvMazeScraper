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

        public async Task<CastMember?> FindAsync(int id)
        {
            return await _context.CastMembers
                .FindAsync(id);
        }
        public async Task AddNewCastMembersAsync(IEnumerable<CastMember> castMembers)
        {
            var castMemberIds = castMembers.Select(c => c.Id).ToList();

            var existingIds = await _context.CastMembers
                .Where(c => castMemberIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync();

            var newCastMembers = castMembers.Where(c => !existingIds.Contains(c.Id));
            if (newCastMembers.Any())
            {
                await _context.CastMembers.AddRangeAsync(newCastMembers);
                await _context.SaveChangesAsync();
            }
        }
    }
}
