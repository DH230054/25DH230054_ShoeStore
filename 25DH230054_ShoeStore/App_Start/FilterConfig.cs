using System.Web;
using System.Web.Mvc;

namespace _25DH230054_ShoeStore
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
