using HMS.FollowUp.Job.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HMS.FollowUp.Job.Implementations
{
    public class SmsService : ISmsService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string AuthUrl = "https://telenorcsms.com.pk:27677/corporate_sms2/api/auth.jsp";
        private const string SendSmsUrl = "https://telenorcsms.com.pk:27677/corporate_sms2/api/sendsms.jsp";
        public async Task AuthenticateAsync(string msisdn, string password, string mobile, string message, string mask, bool isUnicode)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string authRequestUrl = $"{AuthUrl}?msisdn={msisdn}&password={password}";

                var sessionId = await sendRequest(authRequestUrl);
                if (!string.IsNullOrEmpty(sessionId))
                {
                    await SendQuickMessageAsync(sessionId, mobile, message, mask, isUnicode);
                }
            }
            catch (Exception ex)
            {
                // Optionally log the error
            }
        }
        public async Task SendQuickMessageAsync(string sessionId, string recipients, string message, string mask, bool isUnicode)
        {
            try
            {
                string url = $"{SendSmsUrl}?session_id={sessionId}&text={Uri.EscapeDataString(message)}&to={recipients}";

                if (!string.IsNullOrEmpty(mask))
                {
                    url += $"&mask={Uri.EscapeDataString(mask)}";
                }

                await sendRequest(url);
            }
            catch (Exception ex)
            {
                // Optionally log the error
            }
        }
        private async Task<string> sendRequest(String url)
        {
            String response = null;
            try
            {
                var client = new WebClient();
                response = client.DownloadString(url);

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(response);

                XmlNodeList responseType = xmldoc.GetElementsByTagName("response");
                XmlNodeList data = xmldoc.GetElementsByTagName("data");

                if (responseType.Equals("Error"))
                {
                    return null;
                }

                response = data[0].InnerText;

                return response;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
