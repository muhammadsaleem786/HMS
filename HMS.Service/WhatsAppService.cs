using HMS.Entities.CustomModel;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Service
{
    public class WhatsAppService : IWhatsAppService
    {

        public async Task SendAppointmentMessage(
          string phoneNumber,
          string SMSbody,
          string phoneNumberId,
          string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var url = $"https://graph.facebook.com/v23.0/{phoneNumberId}/messages";

                var body = new
                {
                    messaging_product = "whatsapp",
                    to = phoneNumber,
                    type = "text",
                    text = new
                    {
                        body = SMSbody
                    }
                };

                var json = JsonConvert.SerializeObject(body);

                var response = await httpClient.PostAsync(
                    url,
                    new StringContent(json, Encoding.UTF8, "application/json"));

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(result);
                }
            }
        }
    }
}