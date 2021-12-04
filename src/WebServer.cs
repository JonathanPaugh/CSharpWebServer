using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JapeCore;
using JapeDatabase;
using JapeHttp;
using JapeWeb;
using Microsoft.AspNetCore.Http;

namespace CSharpWebServer
{
    public partial class WebServer : JapeWeb.WebServer
    {
        private const int DatabasePort = 3000;

        private static string MongoConnectionString => $"mongodb+srv://admin:{MongoPassword}@cluster0.z6vns.mongodb.net/default?retryWrites=true&w=majority";
        private static string MongoPassword => Environment.GetEnvironmentVariable("API_CSHARP_WEBSERVER_MONGO");

        private readonly Templater templates;

        private readonly Dictionary<string, Middleware.ResponseAsync> responses;

        private DatabaseApi database;

        public WebServer(int http, int https) : base(http, https)
        {
            templates = CreateTemplater("./template");

            responses = GetResponses();

            Use(Middleware.UseAsync(Respond));
        }

        private async Task<Middleware.Result> Respond(Middleware.Request request)
        {
            if (!request.Path.StartsWithSegments("/request", out PathString requestPath))
            {
                return request.Next();
            }

            foreach (KeyValuePair<string, Middleware.ResponseAsync> response in responses)
            {
                if (requestPath.StartsWithSegments(response.Key))
                {
                     return await response.Value.Invoke(request);
                }
            }

            return request.Next();
        }

        protected override async Task OnStartAsync()
        {
            database = new DatabaseApi(DatabasePort);
            database.UseMongo(MongoConnectionString);
            await database.Start();
        }
    }
}
