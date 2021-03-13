using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AutoUpdater
{

    public partial class SelfUpdater
    {
        static readonly Uri GithubAPIURL = new Uri("https://api.github.com");
        const string GithubReleaseEndpoint = "/repos/rotobash/pokemon-ngc-rando/releases";
        public static GithubRelease CheckForUpdate(string toolName, int lastMajor, int lastMinor, int lastPatch)
        {
            using var httpClient = new HttpClient()
            {
                BaseAddress = GithubAPIURL
            };
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("AutoUpdater-by-rotobash");

            var responseTask = httpClient.GetAsync(GithubReleaseEndpoint);
            responseTask.Wait();
            var response = responseTask.Result;

            if (response.IsSuccessStatusCode)
            {
                var responseBodyTask = response.Content.ReadAsStringAsync();
                responseBodyTask.Wait();
                var releases = JsonSerializer.Deserialize<GithubRelease[]>(responseBodyTask.Result);
                foreach (var release in releases)
                {
                    if (release.name != toolName)
                        continue;

                    var versionSplit = release.tag_name.Split(".", 3);
                    if (versionSplit.Length != 3 
                        || !int.TryParse(versionSplit[0], out int major)
                        || !int.TryParse(versionSplit[1], out int minor))
                        continue;


                    string patchStr = versionSplit[2];
                    if (patchStr.Contains("-"))
                    {
                        var patchSplit = patchStr.Split("-");
                        patchStr = patchSplit[0];
                    }

                    if (int.TryParse(patchStr, out int patch)
                        && ((major > lastMajor) 
                        || (major == lastMajor && minor > lastMinor) 
                        || (major == lastMajor && minor == lastMinor && patch > lastPatch)))
                    {
                        return release;
                    }
                }
            }

            return null;
        }
    }
}
