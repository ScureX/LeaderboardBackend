using Modules;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
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
                    }
                }
            }

            var output = response.OutputStream;
            byte[] b = Encoding.UTF8.GetBytes(result);
            output.Write(b, 0, b.Length);

            return response;
        }

        // get info about one player
        private string RankmeResponse_QueryPlayer(WebHeaderCollection headers)
        {
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
                                return $"{COLOR_RANKME}[RankMe] {COLOR_WHITE}{item.name} {item.kills}/{item.deaths}/{Math.Round((float)item.kills/item.deaths, 2)} (K/D) with {item.points} points";
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
            List<RankmeData> data = new List<RankmeData>(); 
            string jsonString = File.ReadAllText(PATH_RANKME_DATA);
            data = JsonConvert.DeserializeObject<List<RankmeData>>(jsonString);
            data.Sort((p, q) => q.points.CompareTo(p.points));

            string result = $"{COLOR_RANKME}[RankMe] {COLOR_GREEN}Top Leaderboard {COLOR_WHITE}[{data.Count} Ranked]\n";

            // go thru first 15 (or less) entries
            for(int i = 0; i <= (data.Count > 15 ? 15 : data.Count-1);  i++)
            {
                result += $"[{i+1}] {data[i].name}: [{COLOR_GREEN}{data[i].kills}/{COLOR_RED}{data[i].deaths}{COLOR_WHITE}] ({Math.Round((float)data[i].kills / data[i].deaths, 2)}) {COLOR_GREEN}{data[i].points} {COLOR_WHITE}Points\n";
            }
            
            return result.Length > 0 ? result : "ERROR";
        }
    }
}
