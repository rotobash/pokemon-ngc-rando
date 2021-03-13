using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater
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
}
