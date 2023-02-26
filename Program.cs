using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace LeaderboardBackend
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 3; i++)
            {
                int port = 8880 + i;
                new Thread(new ThreadStart(() =>
                {
                    PostHandler postHandler = new PostHandler();
                    GetHandler getHandler = new GetHandler();
                    HttpServer httpServer = new HttpServer(postHandler, getHandler, port.ToString());
                })).Start();
            }
        }
    }
}

// example post
//{
//    "players": [
//        {
//            "mods": [
//                {
//                    "mod": "rankme",
//                    "uid": "123123123",
//                    "name": "coolname",
//                    "kills": 12,
//                    "deaths": 2,
//                    "points": 1231,
//                    "track": true,
//                    "pointFeed": true
//                },
//                {
//                    "mod": "topspeed",
//                    "uid": "123123123",
//                    "name": "coolname",
//                    "speed": 123123123.3223
//                },
//                {
//                    "mod": "timewasted",
//                    "uid": "123123123",
//                    "name": "coolname",
//                    "minutesPlayed": 123
//                }
//            ]
//        }
//    ]
//}




// squirrel request
//HttpRequest request;
//request.method = HttpRequestMethod.GET;
//request.url = Spyglass_SanitizeUrl(format("%s/sanctions/lookup_uid", Spyglass_GetApiHostname()));
//request.queryParameters["uids"] < -uids;
//request.queryParameters["excludeMaintainers"] < - [excludeMaintainers.tostring()];
//request.queryParameters["withExpired"] < - [withExpired.tostring()];
//request.queryParameters["withPlayerInfo"] < - [withPlayerInfo.tostring()];