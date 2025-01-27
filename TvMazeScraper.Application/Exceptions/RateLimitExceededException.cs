using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvMazeScraper.Application.Exceptions
{
    public class RateLimitExceededException : Exception
    {
        public TimeSpan? RetryAfter { get; set; }
        public RateLimitExceededException()
        {
        }

        public RateLimitExceededException(string message)
            : base(message)
        {
        }

        public RateLimitExceededException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
