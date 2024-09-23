using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Reflection;

namespace SatisfactorySaveEditor.Util;

public static class UpdateChecker
{
    private const string ReleasesEndpoint = "https://api.github.com/repos/Goz3rr/SatisfactorySaveEditor/releases";

    public static async Task<VersionInfo> GetLatestReleaseInfo()
    {
        var json = await GetLatestReleaseInfoJSON();
        var versions = JsonConvert.DeserializeObject<IList<VersionInfo>>(json);
        return versions.FirstOrDefault();
    }

    private static async Task<string> GetLatestReleaseInfoJSON() => await GetHttpResponseAsync(ReleasesEndpoint);
    private static async Task<string> GetHttpResponseAsync(string url)
    {
        var request = (HttpWebRequest)WebRequest.Create(url);
        request.UserAgent = "SatisfactorySaveEditor";
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        using(var response = (HttpWebResponse)await request.GetResponseAsync())
        using(var stream = response.GetResponseStream())
        using(var reader = new StreamReader(stream))
        {
            return await reader.ReadToEndAsync();
        }
    }

    public class VersionInfo
    {
        [JsonProperty(PropertyName = "html_url")]
        public string ReleaseUrl;

        [JsonProperty(PropertyName = "tag_name")]
        public string TagName;

        [JsonProperty(PropertyName = "name")]
        public string Name;

        [JsonProperty(PropertyName = "body")]
        public string Changelog;

        [JsonProperty(PropertyName = "published_at")]
        public DateTime ReleaseDateTime;

        public bool IsNewer()
        {
            var version = Assembly.GetEntryAssembly().GetName().Version;
            var newVersion = new Version(TagName.TrimStart('v', 'V'));

            return newVersion > version;
        }
    }
}
