using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _25DH230054_ShoeStore.Models.ViewModel
{
    public class ProductSearchVM
    {
            public string SearchTerm { get; set; }
            public string Brand { get; set; }
            public decimal? MinPrice { get; set; }
            public decimal? MaxPrice { get; set; }
            public string SortOrder { get; set; }
            public PagedList.IPagedList<Product> Products { get; set; }
    }
}

