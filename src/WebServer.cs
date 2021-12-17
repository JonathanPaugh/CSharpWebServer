using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JapeHttp;
using JapeWeb;
using Microsoft.AspNetCore.Http;

namespace CSharpWebServer
{
    public partial class WebServer : JapeWeb.WebServer
    {
        private const int DatabasePort = 3000;
        private const string DatabaseEnv = "API_CSHARP_WEBSERVER_DATABASE";

        private static string MongoConnectionString => $"mongodb+srv://admin:{DatabaseKey}@cluster0.z6vns.mongodb.net/default?retryWrites=true&w=majority";
        private static string DatabaseKey => Environment.GetEnvironmentVariable(DatabaseEnv);

        private Templater templates;
        private Authenticator authenticator;

        private DatabaseApi database;

        public WebServer(int http, int https) : base(http, https) {}

        protected override IEnumerator<IWebComponent> Components()
        { 
            templates = CreateTemplater("./template");
            yield return templates;
            yield return CreateResponseTree("/request", RequestResponses);
            yield return Route("/template", "./template");
            yield return Route("/private", "./private", ResponsePrivate);
            authenticator = CreateAuthenticator(Signup, 
                                                Login, 
                                                request => request.Json.GetString("user"), 
                                                request => request.Json.GetString("password"),
                                                AuthenticatorOptions.Default);

            authenticator.SignupSuccess((request, data) => request.Session.SetString("user", data.Id));
            yield return authenticator;
        }

        private Authenticator.LoginData Login(string user)
        {
            JsonData data = database.MongoGetWhere("authentication", "users", "user", user);
            return new Authenticator.LoginData(data.GetString("_id"), 
                                               data.GetString("user"), 
                                               data.GetString("password"), 
                                               data.GetString("salt"));
        }

        private Authenticator.SignupData Signup(string user, string password, string salt)
        {
            string data = database.MongoInsert("authentication", "users", new JsonData
            {
                { "user", user },
                { "password", password },
                { "salt", salt }
            });

            return new Authenticator.SignupData(data);
        }

        private async Task<Middleware.Result> ResponsePrivate(Middleware.Request request)
        {
            if (!request.Session.TryGetString("user", out string _))
            {
                return await request.Abort(Status.ErrorCode.Forbidden);
            }

            return request.Next();
        }

        protected override async Task OnStartAsync()
        {
            database = new DatabaseApi(DatabasePort, DatabaseEnv);
            database.UseMongo(MongoConnectionString);
            await database.Start();
        }
    }
}
