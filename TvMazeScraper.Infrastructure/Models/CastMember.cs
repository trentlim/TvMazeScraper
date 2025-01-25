using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvMazeScraper.Infrastructure.Models
{
    public class CastMember
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateOnly? Birthday { get; set; }
        public ICollection<TvShow> TvShows { get; set; } = new List<TvShow>();
        public override bool Equals(object obj)
        {
            CastMember? c = obj as CastMember;
            return c != null 
                && c.Id == Id
                && c.Name == Name
                && c.Birthday == Birthday;

        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Name.GetHashCode();
        }
    }
}
