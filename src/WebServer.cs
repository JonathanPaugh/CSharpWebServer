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

        private static string DatabaseKey => Environment.GetEnvironmentVariable("API_CSHARP_WEBSERVER_DATABASE");
        private static string MongoConnectionString => $"mongodb+srv://admin:{DatabaseKey}@cluster0.z6vns.mongodb.net/default?retryWrites=true&w=majority";

        private Templater templates;
        private Authenticator authenticator;

        private DatabaseApi databaseApi;
        private Database database;

        public WebServer(int http, int https) : base(http, https) {} 
        
        protected override async Task OnStartAsync()
        {
            database = new Database(DatabasePort, 0, DatabaseKey);
            database.UseMongo(MongoConnectionString);
            await database.Start();

            databaseApi = new DatabaseApi(DatabasePort, DatabaseKey);
        }

        protected override IEnumerator<WebComponent> Components() 
        { 
            yield return CreateResponseTree("/request", RequestResponses);
            
            authenticator = SetupAuthenticator();
            yield return CreateResponseTree
            (
                "/authenticate", 
                ResponseTree.RelativeResponse("/signup", authenticator.ResponseSignup), 
                ResponseTree.RelativeResponse("/login", authenticator.ResponseLogin)
            );

            templates = CreateTemplater("./template");

            PathString templatesRequestPath = "/template"; 
            yield return Route(templatesRequestPath, "./template", async request => await ResponseTemplate(request, templatesRequestPath));

            yield return Route("/private", "./private", ResponsePrivate);
        }

        private Authenticator SetupAuthenticator()
        {
            Authenticator authenticator = new(Signup, 
                                              Login, 
                                              request => request.Json.GetString("user"), 
                                              request => request.Json.GetString("password"));

            authenticator.SignupSuccess((request, data) => request.Session.SetString("user", data.Id));

            return authenticator;

            async Task<Authenticator.LoginData> Login(string user)
            {
                JsonData data = await databaseApi.MongoQuery("authentication", new JsonData
                {
                    { "find", "users" },
                    { 
                        "filter", 
                        new JsonData
                        {
                            {
                                "user",
                                new JsonData
                                {
                                    { "$eq", user }
                                }
                            }
                        }
                    }
                }, DatabaseApi.MongoQueryResult.First);

                return new Authenticator.LoginData(data.Extract("_id").GetString("$oid"), 
                                                   data.GetString("user"), 
                                                   data.GetString("password"), 
                                                   data.GetString("salt"));
            }

            async Task<Authenticator.SignupData> Signup(string user, string password, string salt)
            {
                JsonData data = await databaseApi.MongoInsert("authentication", "users", new JsonData
                {
                    { "user", user },
                    { "password", password },
                    { "salt", salt }
                });

                return new Authenticator.SignupData(data.GetString("$oid"));
            }
        }

        private async Task<Middleware.Result> ResponseTemplate(Middleware.Request request, PathString requestPath)
        {
            if (request.GetMethod() == Request.Method.Get)
            {
                return await request.Abort(Status.ErrorCode.Forbidden);
            }

            request.Path.StartsWithSegments(requestPath, out PathString remainingPath);
            string file = await templates.ReadFileAsync($".{remainingPath}");
            return await request.Complete(Status.SuccessCode.Ok, file);
        }

        private async Task<Middleware.Result> ResponsePrivate(Middleware.Request request)
        {
            if (!request.Session.TryGetString("user", out string _))
            {
                return await request.Abort(Status.ErrorCode.Forbidden);
            }

            return request.Next();
        }
    }
}
