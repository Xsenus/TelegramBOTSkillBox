using Newtonsoft.Json;
using RestSharp;
using System;

namespace TelegramBOTSkillBox
{
    public class TelegramAPI
    {
        public class Chat
        {
            public int id { get; set; }
            public string first_name { get; set; }
        }

        public class Message
        {
            public Chat chat { get; set; }
            public string text { get; set; }
        }

        public class Update
        {
            public int update_id { get; set; }
            public Message message { get; set; }
        }

        public class ApiResult
        {
            public Update[] result { get; set; }
        }

        const string apiKey = "603171840:AAGbPDHNJ8HWIErv-h04AR2F-hks-MIyBE4";
        const string apiUrl = "https://api.telegram.org/bot" + apiKey + "/";

        RestClient restClietn = new RestClient();

        private int? last_update_id = 0;

        public TelegramAPI()
        {

        }

        public void sendMessage(string text, int? chat_id)
        {
            SendApiRequest("sendMessage", $"chat_id={chat_id}&text={text}");
        }

        public Update[] GetUpdate()
        {
            var json = SendApiRequest("getUpdates", $"offset={last_update_id}");
            var result = JsonConvert.DeserializeObject<ApiResult>(json);

            foreach (var item in result.result)
            {
                Console.WriteLine($"Получен апдейт {item?.update_id}, сообщение от {item?.message?.chat?.first_name}, текст: {item?.message?.text}");
                last_update_id = item?.update_id + 1;
            }

            return result.result;
        }

        public string SendApiRequest(string apiMethod, string arg)
        {
            var url = apiUrl + apiMethod + "?" + arg;
            var request = new RestRequest(url);
            var response = restClietn.Get(request);

            return response.Content;
        }
    }
}
