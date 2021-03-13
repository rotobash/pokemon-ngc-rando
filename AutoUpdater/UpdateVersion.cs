using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace AutoUpdater
{
    public partial class SelfUpdater
    {
        const string GithubReleaseAssetEndpoint = "/repos/rotobash/pokemon-ngc-rando/releases/assets";
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
