using HMS.Entities.CustomModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Service
{
    public class DeepSeekService : IDeepSeekService
    {
        private readonly HttpClient _httpClient;

        public DeepSeekService()
        {
            ServicePointManager.SecurityProtocol =
        SecurityProtocolType.Tls12 |
        SecurityProtocolType.Tls11 |
        SecurityProtocolType.Tls;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(2)
            };
        }
        public async Task<string> GeneratePrescription(DeepSeekRequest model)
        {
            try
            {
                var prompt = $@"

You are an experienced medical assistant.

Analyze the following patient data and suggest:

1. Possible diagnosis
2. Recommended medicines
3. Dosage
4. Precautions
5. Follow-up advice

Patient Details:
Chief Complaint:
{model.Complaint}

Clinical History:
{model.ClinicalHistory}

Vitals:
{model.Vital}

IMPORTANT:
- Use safe and standard medicines
- Keep response concise
- Mention warning if emergency symptoms exist
- Format response professionally
";

                return await AskAI(prompt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> AskAI(string message)
        {
            try
            {
                var apiKey = ConfigurationManager.AppSettings["DeepSeekSecretKey"];
                var apiUrl = ConfigurationManager.AppSettings["DeepSeekBaseURL"];
                var model = ConfigurationManager.AppSettings["DeepSeekModel"];

                _httpClient.DefaultRequestHeaders.Clear();

                _httpClient.DefaultRequestHeaders.Add(
                    "Authorization",
                    $"Bearer {apiKey}"
                );

                var requestBody = new
                {
                    model = model,
                    messages = new[]
                    {
                    new
                    {
                        role = "user",
                        content = message
                    }
                },
                    temperature = 0.7
                };

                var json = JsonConvert.SerializeObject(requestBody);

                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(apiUrl, content);

                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(responseString);
                }

                //var result = JsonConvert.DeserializeObject<DeepSeekApiResponse>(responseString);

                //return result.choices[0].message.content;

                dynamic result = JsonConvert.DeserializeObject(responseString);

                return result.choices[0].message.content.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("DeepSeek Error : " + ex.Message);
            }
        }
    }
    public class DeepSeekApiResponse
    {
        public List<Choice> choices { get; set; }
    }

    public class Choice
    {
        public Message message { get; set; }
    }

    public class Message
    {
        public string content { get; set; }
    }
}