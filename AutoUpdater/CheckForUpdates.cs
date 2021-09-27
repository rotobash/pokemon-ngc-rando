using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace AutoUpdater
{

    public partial class SelfUpdater
    {
        static readonly Uri GithubAPIURL = new Uri("https://api.github.com");
        const string GithubReleaseEndpoint = "/repos/rotobash/pokemon-ngc-rando/releases";
        public static GithubRelease CheckForUpdate(string toolName, Version currentVersion)
        {
            using HttpClient httpClient = new()
            {
                BaseAddress = GithubAPIURL
            };
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("AutoUpdater-by-rotobash");

            Task<HttpResponseMessage> responseTask = httpClient.GetAsync(GithubReleaseEndpoint);
            responseTask.Wait();
            HttpResponseMessage response = responseTask.Result;

            if (response.IsSuccessStatusCode)
            {
                Task<string> responseBodyTask = response.Content.ReadAsStringAsync();
                responseBodyTask.Wait();
                GithubRelease[] releases = JsonSerializer.Deserialize<GithubRelease[]>(responseBodyTask.Result);
                foreach (GithubRelease release in releases)
                {
                    if (release.name != toolName)
                    {
                        continue;
                    }

                    var tag = release.tag_name;
                    if (tag.Contains("-"))
                    {
                        string[] tagSplit = tag.Split("-");
                        tag = tagSplit[0];
                    }

                    var potentialVersion = new Version(tag);
                    if (potentialVersion > currentVersion)
                    {
                        return release;
                    }
                }
            }

            return null;
        }
    }
}
