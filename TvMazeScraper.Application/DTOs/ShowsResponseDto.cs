using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using TvMazeScraper.Common;
using TvMazeScraper.Infrastructure.Models;

namespace TvMazeScraper.Application.DTOs
{
    public class ShowsResponseDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<CastMemberDto> Cast { get; set; } = new List<CastMemberDto>();
        public class CastMemberDto
        {
            public int Id { get; set; }
            public required string Name { get; set; }
            public DateOnly? Birthday { get; set; }
        }
    }
}
