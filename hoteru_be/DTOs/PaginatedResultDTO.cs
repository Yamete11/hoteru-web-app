using System.Collections.Generic;

namespace hoteru_be.DTOs
{
    public class PaginatedResultDTO<T>
    {
        public IEnumerable<T> List { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }

        public int Limit { get; set; }
    }
}
