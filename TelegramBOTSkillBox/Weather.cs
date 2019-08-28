using Newtonsoft.Json;
using RestSharp;

namespace TelegramBOTSkillBox
{
    public class Weather
    {

        public class WeatherData
        {
            public Current current { get; set; }
        }

        public class Current
        {
            public double temp_c { get; set; }
            public Condition condition { get; set; }
        }

        public class Condition
        {
            public string text { get; set; }
        }

        const string apiUrl = "https://api-cdn.apixu.com/v1/current.json";
        const string apiKey = "c25dea8c3a1e4e7189d172834192808";
        const string finalUrl = apiUrl + "?key=" + apiKey + "&lang=ru&q=";

        private RestClient restClient = new RestClient();

        public Weather()
        {
        }

        public string getWeatherInCity(string city)
        {
            var url = finalUrl + city;
            var request = new RestRequest(url);
            var response = restClient.Get(request);

            var data = JsonConvert.DeserializeObject<WeatherData>(response.Content);

            var temp = (int)data.current.temp_c;

            return $"В городе {city} сейчас {data.current.condition.text}, где-то {temp}";
        }
    }
}
