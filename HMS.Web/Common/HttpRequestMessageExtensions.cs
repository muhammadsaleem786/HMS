using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace HMS.Web.API.Common
{
    public static class HttpRequestMessageExtensions
    {
        public static int CompanyID(this HttpRequestMessage request)
        {
            IEnumerable<string> HeaderValues;

            if (request.Headers.TryGetValues("CompanyID", out HeaderValues))
                return Convert.ToInt32(HeaderValues.FirstOrDefault());

            return 0;
        }

        public static int RetailPointID(this HttpRequestMessage request)
        {
            IEnumerable<string> HeaderValues;

            if (request.Headers.TryGetValues("RetailPointID", out HeaderValues))
                return Convert.ToInt32(HeaderValues.FirstOrDefault());

            return 0;
        }
        public static int LoginID(this HttpRequestMessage request)
        {
            IEnumerable<string> HeaderValues;

            if (request.Headers.TryGetValues("UserID", out HeaderValues))
                return Convert.ToInt32(HeaderValues.FirstOrDefault());

            return 0;
        }
        public static string PortalType(this HttpRequestMessage request)
        {
            IEnumerable<string> HeaderValues;
            if (request.Headers.TryGetValues("PortalType", out HeaderValues))
                return Convert.ToString(HeaderValues.FirstOrDefault());
            return "";
        }
        public static int ReferenceID(this HttpRequestMessage request)
        {
            IEnumerable<string> HeaderValues;

            if (request.Headers.TryGetValues("ReferenceID", out HeaderValues))
                return Convert.ToInt32(HeaderValues.FirstOrDefault());

            return 0;
        }

        public static string MachineName(this HttpRequestMessage request)
        {
            return HttpContext.Current.Server.MachineName;
        }

        public static DateTime DateTimes(this HttpRequestMessage request)
        {
            return DateTime.Now;
        }
        public static T GetHeaderValue<T>(this HttpRequestMessage request, string HeaderKey)
        {
            IEnumerable<string> HeaderValues;

            if (request.Headers.TryGetValues(HeaderKey, out HeaderValues))
                return (T)Convert.ChangeType(HeaderValues.FirstOrDefault(), typeof(T));
            return default(T);
        }
    }
}