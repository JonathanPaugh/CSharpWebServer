using System.Threading.Tasks;
using JapeDatabase;
using JapeHttp;
using JapeWeb;

namespace CSharpWebServer
{
    public class WebServer : JapeWeb.WebServer
    {
        private const int DatabasePort = 3000;

        private static string MongoConnectionString => $"mongodb+srv://admin:{MongoPassword}@cluster0.z6vns.mongodb.net/myFirstDatabase?retryWrites=true&w=majority";
        private static string MongoPassword => System.Environment.GetEnvironmentVariable("API_CSHARP_WEBSERVER_MONGO");

        private readonly Api databaseApi = new($"http://127.0.0.1:{DatabasePort}");
        private Database database;

        private readonly Templater templates;

        public WebServer(int http, int https) : base(http, https)
        {
            templates = CreateTemplater("./template");
        }

        protected override async Task OnStartAsync()
        {
            database = new(DatabasePort, 0, true, false);
            database.UseMongo(MongoConnectionString);

            await database.Start();

            MongoInsert("newinsertTest", "colcolcol", new JsonData
            {
                { "Hiya", "There" }
            });
        }

        /// <summary>
        /// Inserts a data into a collection.
        /// </summary>
        /// <param name="database">The database name.</param>
        /// <param name="collection">The collection name.</param>
        /// <param name="data">The data to insert.</param>
        /// <returns>Id for the inserted data.</returns>
        private string MongoInsert(string database, string collection, JsonData data)
        {
            return MongoRequest("insert", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "data", data }
            }).Read();
        }

        private ApiResponse MongoRequest(string command, JsonData data)
        {
            JsonData mongoData = new(data)
            {
                { "index", Database.MongoIndex },
                { "id", command }
            };
            return DatabaseRequest(mongoData);
        }

        private ApiResponse DatabaseRequest(JsonData data)
        {
            return databaseApi.BaseRequest()
                              .SetMethod(Request.Method.Post)
                              .WriteJson(data)
                              .GetResponse();
        }
    }
}
