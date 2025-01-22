using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvMazeScraper.Application.DTOs
{
    public class TvMazeCastMemberDto
    {
        public required PersonInfo Person { get; set; }
        public class PersonInfo
        {
            public int Id { get; set; }
            public required string Name { get; set; }
            public DateOnly Birthday { get; set; }
        }
    }

}
