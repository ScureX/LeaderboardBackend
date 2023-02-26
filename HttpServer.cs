using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;


namespace LeaderboardBackend
{
    class HttpServer
    {
        HttpListener listener;
        PostHandler postHandler;
        GetHandler getHandler;

        public HttpServer(PostHandler postHandler, GetHandler getHandler, string port)
        {
            this.postHandler = postHandler;
            this.getHandler = getHandler;

            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{port}/");
            listener.Prefixes.Add($"http://127.0.0.1:{port}/");
            listener.Start();

            Console.WriteLine($"Listening to {port}...");

            while (true)
            {
                var context = listener.GetContext();
                var request = context.Request;
                var response = context.Response;

                WebHeaderCollection headers = (WebHeaderCollection)request.Headers;

                // handle post request 
                if (request.HttpMethod == "POST")
                    using (var reader = new StreamReader(request.InputStream))
                        postHandler.HandleHttpPost(reader.ReadToEnd(), headers);

                // handle get request
                if (request.HttpMethod == "GET")
                    response = getHandler.HandleHttpGet(response, headers);

                response.Close();
            }
        }
    }
}
