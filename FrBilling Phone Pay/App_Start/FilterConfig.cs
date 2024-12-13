using System.Web;
using System.Web.Mvc;

namespace FrBilling_Phone_Pay
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
