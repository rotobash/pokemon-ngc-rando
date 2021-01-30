using System;
using System.Configuration;

namespace XDCommon
{
    public static class Configuration
    {
        public static bool Verbose 
        {
            get
            {
                return  bool.Parse(ConfigurationManager.AppSettings.Get(nameof(Verbose)));
            }
            set
            {
                ConfigurationManager.AppSettings.Set(nameof(Verbose), value.ToString());
            }
        }
        public static bool UseMemoryStreams
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings.Get(nameof(UseMemoryStreams)));
            }
            set
            {
                ConfigurationManager.AppSettings.Set(nameof(UseMemoryStreams), value.ToString());
            }
        }

        public static int ThreadCount 
        {
            get
            {
                var threadCount = 0;
                if (!int.TryParse(ConfigurationManager.AppSettings.Get(nameof(ThreadCount)), out threadCount))
                {
                    threadCount = 1;
                }
                return threadCount;
            }
            set
            {
                ConfigurationManager.AppSettings.Set(nameof(ThreadCount), value.ToString());
            }
        }
        public static string ExtractDirectory
        {
            get
            {
                return ConfigurationManager.AppSettings.Get(nameof(ExtractDirectory));
            }
            set
            {
                ConfigurationManager.AppSettings.Set(nameof(ExtractDirectory), value.ToString());
            }
        }
    }
}
