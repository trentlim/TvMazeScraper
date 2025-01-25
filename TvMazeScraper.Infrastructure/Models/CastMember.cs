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
    }
}
