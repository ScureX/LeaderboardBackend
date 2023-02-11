using Modules;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using static Constants.Constants;

namespace LeaderboardBackend
{
    class GetHandler
    {
        public HttpListenerResponse HandleHttpGet(HttpListenerResponse response, WebHeaderCollection headers)
        {
            string result = "";
            // Get each header and display each value.
            foreach (string key in headers.AllKeys)
            {
                if(key == "t_querytype")
                {
                    switch (headers.GetValues(key)[0]) // in theory multiple values are possible but since i write the code im saying im not doing that
                    {
                        case "rankme_queryplayer":
                            result = RankmeResponse_QueryPlayer(headers);
                            break;
                        case "rankme_leaderboard":
                            result = RankmeResponse_Leaderboard(headers);
                            break;
                        case "topspeed_leaderboard":
                            result = TopspeedResponse_Leaderboard(headers);
                            break;
                        case "topspeed_queryplayer":
                            result = TopspeedResponse_QueryPlayer(headers);
                            break;
                        case "timewasted_leaderboard":
                            result = TimewastedResponse_Leaderboard(headers);
                            break;
                        case "timewasted_queryplayer":
                            result = TimewastedResponse_QueryPlayer(headers);
                            break;
                        case "timewasted_allwastedtime":
                            result = TimewastedResponse_AllWastedTime(headers);
                            break;
                    }
                }
            }

            if(result == "ERROR")
                response.StatusCode = 400;
            else
                response.StatusCode = 200;

            var output = response.OutputStream;
            byte[] b = Encoding.UTF8.GetBytes(result);
            output.Write(b, 0, b.Length);

            return response;
        }

        /* TIME WASTED */

        private string TimewastedResponse_AllWastedTime(WebHeaderCollection headers)
        {
            Console.WriteLine("GET for AllWastedTime");
            List<TimewastedData> data = new List<TimewastedData>();
            string jsonString = File.ReadAllText(PATH_TIMEWASTED_DATA);
            data = JsonConvert.DeserializeObject<List<TimewastedData>>(jsonString);

            string result = $"{COLOR_MODNAME}[TimeWasted] {COLOR_WHITE}{data.Count} players wasted {COLOR_GREEN}{Math.Round((float)(data.Sum(entry => entry.minutesPlayed)) / 60), 2} {COLOR_WHITE}Hours since {TimewastedData.START_DATE} on this Server!";

            return result.Length > 0 ? result : "ERROR"; // TODO this is shit obvsly
        }

        private string TimewastedResponse_QueryPlayer(WebHeaderCollection headers)
        {
            Console.WriteLine("GET for QueryPlayer");
            List<TimewastedData> data = new List<TimewastedData>(); // todo sort all players n shit for rank
            string jsonString = File.ReadAllText(PATH_TIMEWASTED_DATA);
            data = JsonConvert.DeserializeObject<List<TimewastedData>>(jsonString);

            bool returnRaw = false;

            // check if should return raw or formatted
            foreach (string key in headers.AllKeys)
                if (key == "t_returnraw")
                    returnRaw = bool.Parse(headers.GetValues(key)[0]);

            foreach (string key in headers.AllKeys)
            {
                if (key == "t_uid")
                {
                    foreach (var item in data)
                    {
                        if (item.uid == headers.GetValues(key)[0])
                        {
                            if (!returnRaw)
                                return $"{COLOR_MODNAME}[TimeWasted] {COLOR_WHITE}{item.name} wasted {COLOR_GREEN}{Math.Round((float)(item.minutesPlayed / 60), 2)} Hours since {TimewastedData.START_DATE} {COLOR_WHITE}on this Server!";
                            return JsonConvert.SerializeObject(item);
                        }
                    }
                    break;
                }
            }

            return "ERROR";
        }

        private string TimewastedResponse_Leaderboard(WebHeaderCollection headers)
        {
            Console.WriteLine("GET for Leaderboard");
            List<TimewastedData> data = new List<TimewastedData>();
            string jsonString = File.ReadAllText(PATH_TIMEWASTED_DATA);
            data = JsonConvert.DeserializeObject<List<TimewastedData>>(jsonString);
            data.Sort((p, q) => q.minutesPlayed.CompareTo(p.minutesPlayed));

            string result = $"{COLOR_MODNAME}[TopSpeed] {COLOR_GREEN}Top Leaderboard {COLOR_WHITE}[{data.Count} Ranked]\n";

            // go thru first 15 (or less) entries
            for (int i = 0; i <= (data.Count > 15 ? 15 : data.Count - 1); i++)
            {
                result += $"[{i + 1}] {COLOR_WHITE}{data[i].name} wasted {COLOR_GREEN}{Math.Round((float)(data[i].minutesPlayed / 60), 2)} Hours since {TimewastedData.START_DATE} {COLOR_WHITE}on this Server!\n";
            }

            return result.Length > 0 ? result : "ERROR"; // TODO this is shit obvsly
        }

        /* RANK ME */

        // get info about one player
        private string RankmeResponse_QueryPlayer(WebHeaderCollection headers)
        {
            Console.WriteLine("GET for QueryPlayer");
            List<RankmeData> data = new List<RankmeData>(); // todo sort all players n shit for rank
            string jsonString = File.ReadAllText(PATH_RANKME_DATA);
            data = JsonConvert.DeserializeObject<List<RankmeData>>(jsonString);

            bool returnRaw = false;

            // check if should return raw or formatted
            foreach (string key in headers.AllKeys)
                if (key == "t_returnraw")
                    returnRaw = bool.Parse(headers.GetValues(key)[0]);

            foreach (string key in headers.AllKeys)
            {
                if(key == "t_uid")
                {
                    foreach (var item in data)
                    {
                        if (item.uid == headers.GetValues(key)[0])
                        {
                            if(!returnRaw)
                                return $"{COLOR_MODNAME}[RankMe] {COLOR_WHITE}{item.name} {COLOR_GREEN}{item.kills}/{COLOR_RED}{item.deaths} {COLOR_WHITE}({Math.Round((float)item.kills/item.deaths, 2)}) with {COLOR_GREEN}{item.points} {COLOR_WHITE}points";
                            return JsonConvert.SerializeObject(item);
                        }
                    }
                    break;
                }
            }

            return "ERROR";
        }

        // get rankme leaderboard
        private string RankmeResponse_Leaderboard(WebHeaderCollection headers) // leave in params maybe diff leaderboard sortings idk
        {
            Console.WriteLine("GET for Leaderboard");
            List<RankmeData> data = new List<RankmeData>(); 
            string jsonString = File.ReadAllText(PATH_RANKME_DATA);
            data = JsonConvert.DeserializeObject<List<RankmeData>>(jsonString);
            data.Sort((p, q) => q.points.CompareTo(p.points));

            string result = $"{COLOR_MODNAME}[RankMe] {COLOR_GREEN}Top Leaderboard {COLOR_WHITE}[{data.Count} Ranked]\n";

            // go thru first 15 (or less) entries
            for(int i = 0; i <= (data.Count > 15 ? 15 : data.Count-1);  i++)
            {
                result += $"[{i+1}] {data[i].name}: [{COLOR_GREEN}{data[i].kills}/{COLOR_RED}{data[i].deaths}{COLOR_WHITE}] ({Math.Round((float)data[i].kills / data[i].deaths, 2)}) {COLOR_GREEN}{data[i].points} {COLOR_WHITE}Points\n";
            }
            
            return result.Length > 0 ? result : "ERROR"; // TODO this is shit obvsly
        }

        /* TOP SPEED */

        private string TopspeedResponse_QueryPlayer(WebHeaderCollection headers)
        {
            Console.WriteLine("GET for QueryPlayer");
            List<TopspeedData> data = new List<TopspeedData>(); // todo sort all players n shit for rank
            string jsonString = File.ReadAllText(PATH_TOPSPEED_DATA);
            data = JsonConvert.DeserializeObject<List<TopspeedData>>(jsonString);

            bool returnRaw = false;

            // check if should return raw or formatted
            foreach (string key in headers.AllKeys)
                if (key == "t_returnraw")
                    returnRaw = bool.Parse(headers.GetValues(key)[0]);

            foreach (string key in headers.AllKeys)
            {
                if (key == "t_uid")
                {
                    foreach (var item in data)
                    {
                        if (item.uid == headers.GetValues(key)[0])
                        {
                            if (!returnRaw)
                                return $"{COLOR_MODNAME}[TopSpeed] {item.name}: {COLOR_IDK}{Modules.Helper.SpeedToKmh(item.speed)}kmh{COLOR_WHITE}/{COLOR_IDK}{Modules.Helper.SpeedToMph(item.speed)}mph";
                            return JsonConvert.SerializeObject(item);
                        }
                    }
                    break;
                }
            }

            return "ERROR";
        }

        private string TopspeedResponse_Leaderboard(WebHeaderCollection headers)
        {
            Console.WriteLine("GET for Leaderboard");
            List<TopspeedData> data = new List<TopspeedData>();
            string jsonString = File.ReadAllText(PATH_TOPSPEED_DATA);
            data = JsonConvert.DeserializeObject<List<TopspeedData>>(jsonString);
            data.Sort((p, q) => q.speed.CompareTo(p.speed));

            string result = $"{COLOR_MODNAME}[TopSpeed] {COLOR_GREEN}Top Leaderboard {COLOR_WHITE}[{data.Count} Ranked]\n";

            // go thru first 15 (or less) entries
            for (int i = 0; i <= (data.Count > 15 ? 15 : data.Count - 1); i++)
            {
                result += $"[{i + 1}] {data[i].name}: {COLOR_IDK}{Modules.Helper.SpeedToKmh(data[i].speed)}kmh{COLOR_WHITE}/{COLOR_IDK}{Modules.Helper.SpeedToMph(data[i].speed)}mph{COLOR_WHITE}\n";
            }

            return result.Length > 0 ? result : "ERROR"; // TODO this is shit obvsly
        }

    }
}
