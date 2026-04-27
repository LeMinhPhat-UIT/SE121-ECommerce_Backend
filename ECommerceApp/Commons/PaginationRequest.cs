using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceApp.Commons
{
    public class PaginationRequest
    {
        public int PageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 1;
    }
}
