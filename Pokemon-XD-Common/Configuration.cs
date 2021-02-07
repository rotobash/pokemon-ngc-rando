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
                AddOrUpdateAppSettings(nameof(Verbose), value.ToString());
                
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
                AddOrUpdateAppSettings(nameof(UseMemoryStreams), value.ToString());
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
                AddOrUpdateAppSettings(nameof(ThreadCount), value.ToString());
            }
        }

        public static int GoodDamagingMovePower 
        {
            get
            {
                var movePower = 0;
                if (!int.TryParse(ConfigurationManager.AppSettings.Get(nameof(GoodDamagingMovePower)), out movePower))
                {
                    GoodDamagingMovePower = 60;
                }
                return movePower;
            }
            set
            {
                AddOrUpdateAppSettings(nameof(GoodDamagingMovePower), value.ToString());
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
                AddOrUpdateAppSettings(nameof(ExtractDirectory), value.ToString());
            }
        }
        static void AddOrUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }
}
