﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Modules;
using static Constants.Constants;


namespace LeaderboardBackend
{
    class PostHandler
    {

        public void HandleHttpPost(string jsonString)
        {
            // Parse the JSON body from the POST request
            Root jsonBody = JsonSerializer.Deserialize<Root>(jsonString);

            // Loop through each player and each mod to save the values to a file
            foreach (var player in jsonBody.players)
            {
                foreach (var mod in player.mods)
                {
                    switch (mod.mod)
                    {
                        case "rankme":
                            SaveRankmeData(mod);
                            break;
                        case "topspeed":
                            SaveTopspeedData(mod);
                            break;
                        case "timewasted":
                            SaveTimewastedData(mod);
                            break;
                    }
                }
            }
        }

        private void EnsureFileExistence(string path)
        {
            if (!File.Exists(path))
                File.WriteAllText(path, "[]");
        }

        private void SaveRankmeData(Mod mod)
        {
            // Get the file path for the rankme mod
            string path = PATH_RANKME_DATA;
            string uid = mod.uid;
            string name = mod.name;
            int kills = mod.kills;
            int deaths = mod.deaths;
            int points = mod.points;
            bool track = mod.track;
            bool pointFeed = mod.pointFeed;

            EnsureFileExistence(path);

            List<RankmeData> data = new List<RankmeData>();

            if (File.Exists(path))
            {
                // Read the existing data from the file
                string jsonString = File.ReadAllText(path);
                data = JsonConvert.DeserializeObject<List<RankmeData>>(jsonString);
            }

            // Check if the uid already exists
            bool uidFound = false;
            foreach (var item in data)
            {
                if (item.uid == uid)
                {
                    // Update the values for the uid
                    item.name = name;
                    item.kills = kills;
                    item.deaths = deaths;
                    item.points = points;
                    item.track = track;
                    item.pointFeed = pointFeed;
                    uidFound = true;
                    break;
                }
            }
            if (!uidFound)
            {
                // Add a new item to the list for the uid
                data.Add(new RankmeData
                {
                    uid = uid,
                    name = name,
                    kills = kills,
                    deaths = deaths,
                    points = points,
                    track = track,
                    pointFeed = pointFeed
                });
            }

            // Serialize the updated data and save it to the file
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }


        private void SaveTopspeedData(Mod mod)
        {
            // Get the file path for the topspeed mod
            string path = PATH_TOPSPEED_DATA;
            string uid = mod.uid;
            string name = mod.name;
            double speed = mod.speed;


            EnsureFileExistence(path);

            List<TopspeedData> data = new List<TopspeedData>();

            if (File.Exists(path))
            {
                // Read the existing data from the file
                string jsonString = File.ReadAllText(path);
                data = JsonConvert.DeserializeObject<List<TopspeedData>>(jsonString);
            }

            // Update the values for the uid if it already exists
            bool uidFound = false;
            foreach (var item in data)
            {
                if (item.uid == uid)
                {
                    item.name = name;
                    item.speed = speed;
                    uidFound = true;
                    break;
                }
            }

            if (!uidFound)
            {
                // Add a new TopspeedData with the data for the uid
                data.Add(new TopspeedData { uid = uid, name = name, speed = speed });
            }

            // Write the updated data to the file
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }


        private void SaveTimewastedData(Mod mod)
        {
            // Get the file path for the timewasted mod
            string path = PATH_TIMEWASTED_DATA;
            string uid = mod.uid;
            string name = mod.name;
            int minutesPlayed = mod.minutesPlayed;

            EnsureFileExistence(path);

            List<TimewastedData> data = new List<TimewastedData>();

            if (File.Exists(path))
            {
                // Read the existing data from the file
                string jsonString = File.ReadAllText(path);
                data = JsonConvert.DeserializeObject<List<TimewastedData>>(jsonString);
            }

            // Update the values for the uid if it already exists
            bool uidFound = false;
            foreach (var item in data)
            {
                if (item.uid == uid)
                {
                    item.name = name;
                    item.minutesPlayed = minutesPlayed;
                    uidFound = true;
                    break;
                }
            }

            if (!uidFound)
            {
                // Add a new mod with the data for the uid
                data.Add(new TimewastedData { uid = uid, name = name, minutesPlayed = minutesPlayed });
            }

            // Write the updated data to the file
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
