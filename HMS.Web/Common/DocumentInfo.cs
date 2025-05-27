using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HMS.Web.API.Common
{
    public class DocumentInfo
    {
        public static string getTempDocumentPathInfo(bool WithName = false)
        {
            try
            {
                return ConfigurationManager.AppSettings["ApiTempUrl"].ToString() + (WithName ? DateTime.Now.ToString("yyyyMMddHHmmsss"):"");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string getPurchaseReturnPathInfo()
        {
            try
            {
                return ConfigurationManager.AppSettings["PurchaseReturnPath"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string getProfilePathInfo()
        {
            try
            {
                return ConfigurationManager.AppSettings["ProfilePath"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string getVendorCustomerPathInfo()
        {
            try
            {
                return ConfigurationManager.AppSettings["VendorCustomerPath"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string getCompanyPathInfo()
        {
            try
            {
                return ConfigurationManager.AppSettings["CompanyPath"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string getProductPathInfo()
        {
            try
            {
                return ConfigurationManager.AppSettings["AttachmentPath"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string getReportPathInfo()
        {
            try
            {
                return ConfigurationManager.AppSettings["ReportPath"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string getCoupanPathInfo()
        {
            try
            {
                return ConfigurationManager.AppSettings["CoupanPath"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}