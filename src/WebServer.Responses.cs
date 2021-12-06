using JapeHttp;
using System.Collections.Generic;
using System.Threading.Tasks;
using JapeWeb;

namespace CSharpWebServer
{
    public partial class WebServer
    {
        private ResponseTree.Response[] RequestResponses => new[]
        {
            ResponseTree.RelativeResponse("/mongo"),
            ResponseTree.RelativeResponse("/mongo/insert", ResponseMongoInsert),
            ResponseTree.RelativeResponse("/redis"),
            ResponseTree.RelativeResponse("/mongo/create"),
            ResponseTree.RelativeResponse("/redis/poggies")
        };

        private async Task<Middleware.Result> ResponseMongoInsert(Middleware.Request request)
        {
            string result = database.MongoInsert("AjaxDB", "AjaxCol", request.Json);
            return await request.Complete(Status.SuccessCode.Ok, result);
        }
    }
}
