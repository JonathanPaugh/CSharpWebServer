using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JapeCore;
using JapeDatabase;
using JapeHttp;
using JapeWeb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CSharpWebServer
{
    public partial class WebServer : JapeWeb.WebServer
    {
        private const int DatabasePort = 3000;

        private static string MongoConnectionString => $"mongodb+srv://admin:{MongoPassword}@cluster0.z6vns.mongodb.net/default?retryWrites=true&w=majority";
        private static string MongoPassword => Environment.GetEnvironmentVariable("API_CSHARP_WEBSERVER_MONGO");

        private readonly Templater templates;

        private DatabaseApi database;

        public WebServer(int http, int https) : base(http, https)
        {
            templates = CreateTemplater("./template");
            Route("/template", "./template");
            ResponseTree tree = CreateResponseTree("/request", RequestResponses); 
            tree.Write(Log.WriteConsole);
        }

        protected override async Task OnStartAsync()
        {
            database = new DatabaseApi(DatabasePort);
            database.UseMongo(MongoConnectionString);
            await database.Start();
        }
    }
}
