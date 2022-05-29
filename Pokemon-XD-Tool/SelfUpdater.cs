using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Randomizer
{
    /**
     * Models defined by Github API
     */

    public class GithubReleaseAsset
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int size { get; set; }
        public string url { get; set; }
        public string browser_download_url { get; set; }
    }

    public class GithubRelease
    {
        public int id { get; set; }
        public string name { get; set; }
        public string tag_name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime published_at { get; set; }
        public GithubReleaseAsset[] assets { get; set; }
        public string body { get; set; }
    }

    public class SelfUpdater
    {
        static readonly Uri GithubAPIURL = new Uri("https://api.github.com");
        const string GithubReleaseEndpoint = "/repos/rotobash/pokemon-ngc-rando/releases";
        const string GithubReleaseAssetEndpoint = "/repos/rotobash/pokemon-ngc-rando/releases/assets";

        public static GithubRelease CheckForUpdate(string toolName, Version currentVersion)
        {
            using HttpClient httpClient = new()
            {
                BaseAddress = GithubAPIURL
            };
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("AutoUpdater-by-rotobash");

            HttpResponseMessage response;
            try
            {
                Task<HttpResponseMessage> responseTask = httpClient.GetAsync(GithubReleaseEndpoint);
                responseTask.Wait();
                response = responseTask.Result;
            }
            catch
            {
                // GitHub is possibly not rersponding, so continue without updating
                return null;
            }

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

        public static void Update(GithubReleaseAsset release, IProgress<float> progressListener)
        {
            using var httpClient = new HttpClient()
            {
                BaseAddress = GithubAPIURL
            };
            // needed or github won't accept API requests, they claim it's to contact people if there's a problem
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("AutoUpdater-by-rotobash");
            // also github, adding this say we want the uploaded asset instead of metadata
            httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/octet-stream");

            // use HttpCompletionOptions to get progress reports
            var responseTask = httpClient.GetAsync($"{GithubReleaseAssetEndpoint}/{release.id}", HttpCompletionOption.ResponseHeadersRead);
            responseTask.Wait();
            var response = responseTask.Result;

            var bytesReadTotal = 0;
            var bytesTotal = release.size;
            if (response.IsSuccessStatusCode)
            {
                // get access to the underlying stream data, we requested the actual asset so we know we're getting a file
                var responseBodyTask = response.Content.ReadAsStreamAsync();
                responseBodyTask.Wait();
                using var httpStream = responseBodyTask.Result;
                using var fileStream = File.OpenWrite(release.name);

                // buffer http stream into file as we read it, report progress if it was passed
                byte[] buffer = new byte[0x8000];
                while (true)
                {
                    var bytesRead = httpStream.Read(buffer, 0, Math.Min(buffer.Length, bytesTotal - bytesReadTotal));
                    if (bytesRead == 0)
                    {
                        progressListener?.Report(1);
                        return;
                    }
                    bytesReadTotal += bytesRead;
                    fileStream.Write(buffer, 0, bytesRead);
                    progressListener?.Report((float)bytesReadTotal / bytesTotal);
                }
            }
        }
    }
}
