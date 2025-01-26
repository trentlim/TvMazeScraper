using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using TvMazeScraper.Common;

namespace TvMazeScraper.Application.DTOs
{
    public class ShowsRequestDto
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, PaginationConstants.MaxPageSize)]
        public int PageSize { get; set; } = PaginationConstants.DefaultPageSize;
    }
}
