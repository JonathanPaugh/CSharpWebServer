using JapeCore;
using JapeHttp;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSharpWebServer
{
    public partial class WebServer
    {
        private Dictionary<string, Middleware.ResponseAsync> GetResponses() => new()
        {
            { "/mongo", RespondMongo },
        };

        private async Task<Middleware.Result> RespondMongo(Middleware.Request request)
        {
            JsonData d = request.Json;
            string result = database.MongoInsert("AjaxDB", "AjaxCol", request.Json);
            return (Middleware.Result)await request.Complete(Status.SuccessCode.Ok, result);
        }

    }
}
