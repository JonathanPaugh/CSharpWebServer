using System.IO;

namespace CSharpWebServer
{
    public class WebServer : JapeWeb.WebServer
    {
        public WebServer(int http, int https) : base(http, https) {}
    }
}
