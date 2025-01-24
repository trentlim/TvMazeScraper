using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvMazeScraper.Application.DTOs
{
    public class TvMazeCastMemberDto
    {
        public required PersonDto Person { get; set; }
        public int ShowId { get; set; }
        public class PersonDto
        {
            public int Id { get; set; }
            public required string Name { get; set; }
            public DateOnly Birthday { get; set; }
        }
    }

}
