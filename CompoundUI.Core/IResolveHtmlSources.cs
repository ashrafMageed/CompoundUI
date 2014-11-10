using System.Net;

namespace CompoundUI.Core
{
    public interface IResolveHtmlSources
    {
        string Resolve(string source);
    }

    public class UrlHtmlSourceResolver : IResolveHtmlSources
    {
        public string Resolve(string source)
        {
            using (var webClient = new WebClient())
            {
                return webClient.DownloadString(source);
            }
        }
    }

}