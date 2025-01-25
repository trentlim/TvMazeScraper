using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TvMazeScraper.Infrastructure.Models;

namespace TvMazeScraper.Infrastructure.Data
{
    public class TvShowDbContext : DbContext
    {
        public DbSet<TvShow> TvShows { get; set; }
        public DbSet<CastMember> CastMembers { get; set; }

        public TvShowDbContext(DbContextOptions<TvShowDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TvShow>()
                .HasMany(e => e.Cast)
                .WithMany(e => e.TvShows);
        }
    }
}
