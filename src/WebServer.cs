using System.Threading.Tasks;
using JapeHttp;
using JapeWeb;

namespace CSharpWebServer
{
    public class WebServer : JapeWeb.WebServer
    {
        private readonly Templater templater;

        public WebServer(int http, int https) : base(http, https)
        {
            templater = CreateTemplater("./template");
        }
    }
}
