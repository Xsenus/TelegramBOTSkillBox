using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TelegramBOTSkillBox
{
    class Program
    {
        static Dictionary<string, string> Questions = new Dictionary<string, string>();

        static Dictionary<int, string> TimeOfDay
        {
            get
            {
                var res = new Dictionary<int, string>();
                for (int i = 0; i < 24; i++)
                {
                    if (i >= 0 && i <= 3)
                        res.Add(i, "Ночь");
                    else if (i >= 4 && i <= 11)
                        res.Add(i, "Утро");
                    else if (i >= 12 && i <= 16)
                        res.Add(i, "День");
                    else if (i >= 17 && i <= 23)
                        res.Add(i, "Вечер");
                }
                return res;
            }
        }

        static private bool flagLanguage = false;

        static void Main(string[] args)
        {
            var api = new TelegramAPI();
            var data = string.Empty;

            while (true)
            {
                if (!flagLanguage)
                {
                    data = File.ReadAllText("answerRu.json");
                    
                }
                else
                {
                    data = File.ReadAllText("answerEn.json");
                }

                Questions = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

                var updates = api.GetUpdate();

                foreach (var item in updates)
                {
                    var question = item?.message?.text;
                    var answer = AnserQuestion(question);
                    api.sendMessage(answer, item?.message?.chat?.id);
                }    
            }
        }

        static string AnserQuestion(string userQuestion)
        {
            if (userQuestion == string.Empty || userQuestion == null)
            {
                return default(string);
            }

            userQuestion = userQuestion.ToLower();
            var answer = new List<string>();

            foreach (var item in Questions)
            {
                if (userQuestion.Contains(item.Key))
                {
                    answer.Add(item.Value);
                }
            }

            if (userQuestion.Contains("давай поговорим по-английски"))
            {
                flagLanguage = true;
                answer.Add("Ok, let’s do it");
            }

            if (userQuestion.Contains("return russian"))
            {
                flagLanguage = false;
                answer.Add("Великий и могучий снова тут");
            }

            if ((userQuestion.Contains("сколько") && userQuestion.Contains("времени")) || (userQuestion.Contains("what") && userQuestion.Contains("time")))
            {
                var time = DateTime.Now.ToShortTimeString();
                answer.Add(time);
            }

            if ((userQuestion.Contains("какой") && userQuestion.Contains("сегодня") && userQuestion.Contains("день"))
                || (userQuestion.Contains("what") && userQuestion.Contains("day") && userQuestion.Contains("today")))
            {
                var time = DateTime.Now.ToShortDateString();
                answer.Add(time);
            }

            if (userQuestion.Contains("какой") && userQuestion.Contains("сегодня") && userQuestion.Contains("день") && userQuestion.Contains("недели"))
            {
                var day = DateTime.Now.DayOfWeek.ToString();
                answer.Add(day);
            }

            if (userQuestion.Contains("какое") && userQuestion.Contains("сейчас") && userQuestion.Contains("время") && userQuestion.Contains("суток"))
            {
                var timeOfDay = TimeOfDay.Where(w => w.Key == DateTime.Now.Hour).First().Value;
                answer.Add(timeOfDay);
            }

            if (userQuestion.StartsWith("какая погода в городе"))
            {
                var words = userQuestion.Split(' ');
                var city = words[words.Length - 1];

                var weather = new Weather();
                var forecast = weather.getWeatherInCity(city);

                answer.Add(forecast);
            }

            if (answer.Count == 0)
            {
                if (flagLanguage)
                {
                    answer.Add("Fuck");

                }
                else
                {
                    answer.Add("Моя твоя не понимать.");
                }
                
            }

            return string.Join(", ", answer);
        }
    }
}
