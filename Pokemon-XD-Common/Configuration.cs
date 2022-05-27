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
                if (!int.TryParse(ConfigurationManager.AppSettings.Get(nameof(ThreadCount)), out int threadCount))
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
                if (!int.TryParse(ConfigurationManager.AppSettings.Get(nameof(GoodDamagingMovePower)), out int movePower))
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

        public static int StrongPokemonBST 
        {
            get
            {
                if (!int.TryParse(ConfigurationManager.AppSettings.Get(nameof(StrongPokemonBST)), out int bstTotal))
                {
                    StrongPokemonBST = 300;
                }
                return bstTotal;
            }
            set
            {
                AddOrUpdateAppSettings(nameof(StrongPokemonBST), value.ToString());
            }
        }
        public static int BSTRange
        {
            get
            {
                if (!int.TryParse(ConfigurationManager.AppSettings.Get(nameof(BSTRange)), out int bstTotal))
                {
                    BSTRange = 20;
                }
                return bstTotal;
            }
            set
            {
                AddOrUpdateAppSettings(nameof(BSTRange), value.ToString());
            }
        }

        public static int PokemonImpossibleEvolutionLevel 
        {
            get
            {
                if (!int.TryParse(ConfigurationManager.AppSettings.Get(nameof(PokemonImpossibleEvolutionLevel)), out int evoLevel))
                {
                    PokemonImpossibleEvolutionLevel = 40;
                }
                return evoLevel;
            }
            set
            {
                AddOrUpdateAppSettings(nameof(PokemonImpossibleEvolutionLevel), value.ToString());
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
