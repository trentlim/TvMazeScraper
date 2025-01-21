using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvMazeScraper.Infrastructure.Models
{
    public class TvShow
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<CastMember> Cast { get; set; } = new List<CastMember>();
    }
}
