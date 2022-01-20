using JapeHttp;
using System.Linq;
using System.Threading.Tasks;
using JapeCore;
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
            ResponseTree.RelativeResponse("/get-user", ResponseGetUser),
        };

        private async Task<Middleware.Result> ResponseMongoGet(Middleware.Request request)
        {
            try
            {
                JsonData result = await databaseApi.MongoGet(request.Json.GetString("database"), 
                                                             request.Json.GetString("collection"), 
                                                             request.Json.GetString("id"));

                return await request.Complete(Status.SuccessCode.Ok, result);
            }
            catch (ResponseException exception)
            {
                return await request.Abort(exception.StatusCode);
            } 
        }

        private async Task<Middleware.Result> ResponseMongoInsert(Middleware.Request request)
        {
            try
            {
                JsonData result = await databaseApi.MongoInsert(request.Json.GetString("database"), request.Json.GetString("collection"), request.Json.Extract("data"));
                return await request.Complete(Status.SuccessCode.Ok, result.GetString("$oid"));
            }
            catch (ResponseException exception)
            {
                return await request.Abort(exception.StatusCode);
            } 
        }

        private async Task<Middleware.Result> ResponseMongoUpdate(Middleware.Request request)
        {
            try
            {
                JsonData result = await databaseApi.MongoUpdate(request.Json.GetString("database"), request.Json.GetString("collection"), request.Json.GetString("id"), request.Json.Extract("data"));
                return await request.Complete(Status.SuccessCode.Ok, result);
            }
            catch (ResponseException exception)
            {
                return await request.Abort(exception.StatusCode);
            } 
        }

        private async Task<Middleware.Result> ResponseMongoRemove(Middleware.Request request)
        {
            try
            {
                JsonData result = await databaseApi.MongoRemove(request.Json.GetString("database"), request.Json.GetString("collection"), request.Json.GetString("id"), request.Json.GetStringArray("data").ToArray());
                return await request.Complete(Status.SuccessCode.Ok, result);
            }
            catch (ResponseException exception)
            {
                return await request.Abort(exception.StatusCode);
            } 
        }

        private async Task<Middleware.Result> ResponseMongoDelete(Middleware.Request request)
        {
            try
            {
                JsonData result = await databaseApi.MongoDelete(request.Json.GetString("database"), request.Json.GetString("collection"), request.Json.GetString("id"));
                return await request.Complete(Status.SuccessCode.Ok, result);
            }
            catch (ResponseException exception)
            {
                return await request.Abort(exception.StatusCode);
            } 
        }

        private async Task<Middleware.Result> ResponseGetUser(Middleware.Request request)
        {
            if (!request.Session.TryGetString("user", out string user))
            {
                return await request.Abort(Status.ErrorCode.NotFound);
            }

            JsonData result;
            try
            {
                result = await databaseApi.MongoGet("authentication", "users", user);
            }
            catch (ResponseException exception)
            {
                return await request.Abort(exception.StatusCode);
            } 

            return await request.Complete(Status.SuccessCode.Ok, result);
        }
    }
}
