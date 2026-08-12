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
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    var url =
                        $"https://graph.facebook.com/v23.0/{phoneNumberId}/messages";

                    var body = new
                    {
                        messaging_product = "whatsapp",

                        to = phoneNumber,

                        type = "template",

                        template = new
                        {
                            name = "appointment_confirmation",

                            language = new
                            {
                                code = "en_US"
                            },

                            components = new[]
                            {
                        new
                        {
                            type = "body",

                            parameters = new[]
                            {
                                new
                                {
                                    type = "text",
                                    text = "ali"
                                },

                                new
                                {
                                    type = "text",
                                    text = "saleem"
                                },

                                new
                                {
                                    type = "text",
                                    text = DateTime.UtcNow.ToString()
                                },

                                new
                                {
                                    type = "text",
                                    text = DateTime.UtcNow.ToString()
                                }
                            }
                        }
                    }
                        }
                    };

                    var json = JsonConvert.SerializeObject(body);

                    var content = new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json");

                    var response = await httpClient.PostAsync(
                        url,
                        content);

                    var result = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(
                            $"WhatsApp API Error: {result}");
                    }
                }
            }
            catch (Exception ex)
            {

                
            }
        }
    }
}