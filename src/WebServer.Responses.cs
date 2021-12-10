using JapeHttp;
using System.Linq;
using System.Threading.Tasks;
using JapeWeb;

namespace CSharpWebServer
{
    public partial class WebServer
    {
        private ResponseTree.Response[] RequestResponses => new[]
        {
            ResponseTree.RelativeResponse("/mongo/get", ResponseMongoGet),
            ResponseTree.RelativeResponse("/mongo/insert", ResponseMongoInsert),
            ResponseTree.RelativeResponse("/mongo/update", ResponseMongoUpdate),
            ResponseTree.RelativeResponse("/mongo/remove", ResponseMongoRemove),
            ResponseTree.RelativeResponse("/mongo/delete", ResponseMongoDelete),
        };

        private async Task<Middleware.Result> ResponseMongoGet(Middleware.Request request)
        {
            try
            {
                JsonData result = database.MongoGet("AjaxDB", "AjaxCol", request.Json.GetString("id"));
                return await request.Complete(Status.SuccessCode.Ok, result);
            }
            catch
            {
                return await request.Abort(Status.ErrorCode.ServerError);
            }
            
        }

        private async Task<Middleware.Result> ResponseMongoInsert(Middleware.Request request)
        {
            try
            {
                string result = database.MongoInsert("AjaxDB", "AjaxCol", request.Json);
                return await request.Complete(Status.SuccessCode.Ok, result);
            }
            catch
            {
                return await request.Abort(Status.ErrorCode.ServerError);
            }
        }

        private async Task<Middleware.Result> ResponseMongoUpdate(Middleware.Request request)
        {
            try
            {
                JsonData result = database.MongoUpdate("AjaxDB", "AjaxCol", request.Json.GetString("id"), request.Json.Extract("data"));
                return await request.Complete(Status.SuccessCode.Ok, result);
            }
            catch
            {
                return await request.Abort(Status.ErrorCode.ServerError);
            }
        }

        private async Task<Middleware.Result> ResponseMongoRemove(Middleware.Request request)
        {
            try
            {
                JsonData result = database.MongoRemove("AjaxDB", "AjaxCol", request.Json.GetString("id"), request.Json.GetStringArray("data").ToArray());
                return await request.Complete(Status.SuccessCode.Ok, result);
            }
            catch
            {
                return await request.Abort(Status.ErrorCode.ServerError);
            }
        }

        private async Task<Middleware.Result> ResponseMongoDelete(Middleware.Request request)
        {
            try
            {
                JsonData result = database.MongoDelete("AjaxDB", "AjaxCol", request.Json.GetString("id"));
                return await request.Complete(Status.SuccessCode.Ok, result);
            }
            catch
            {
                return await request.Abort(Status.ErrorCode.ServerError);
            }
        }
    }
}
