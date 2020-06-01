using System.Web;
using System.Web.Mvc;
using Task2.Filters;

namespace Task2
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
