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
                return  bool.Parse(ConfigurationManager.AppSettings.Get(nameof(Verbose)) ?? "false");
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
                return bool.Parse(ConfigurationManager.AppSettings.Get(nameof(UseMemoryStreams)) ?? "true");
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
                if (!int.TryParse(ConfigurationManager.AppSettings.Get(nameof(ThreadCount)), out int threadCount) && threadCount <= 0)
                {
                    ThreadCount = 1;
                    return ThreadCount;
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
                if (!int.TryParse(ConfigurationManager.AppSettings.Get(nameof(GoodDamagingMovePower)), out int movePower) && movePower <= 0)
                {
                    GoodDamagingMovePower = 60;
                    return GoodDamagingMovePower;
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
                if (!int.TryParse(ConfigurationManager.AppSettings.Get(nameof(StrongPokemonBST)), out int bstTotal) && bstTotal <= 0)
                {
                    StrongPokemonBST = 300;
                    return StrongPokemonBST;
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
                if (!int.TryParse(ConfigurationManager.AppSettings.Get(nameof(BSTRange)), out int bstTotal) && bstTotal <= 0)
                {
                    BSTRange = 20;
                    return BSTRange;
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
                if (!int.TryParse(ConfigurationManager.AppSettings.Get(nameof(PokemonImpossibleEvolutionLevel)), out int evoLevel) && evoLevel <= 0)
                {
                    PokemonImpossibleEvolutionLevel = 40;
                    return PokemonImpossibleEvolutionLevel;
                }
                return evoLevel;
            }
            set
            {
                AddOrUpdateAppSettings(nameof(PokemonImpossibleEvolutionLevel), value.ToString());
            }
        }

        public static int PokemonEasierFinalEvolutionLevel 
        {
            get
            {
                if (!int.TryParse(ConfigurationManager.AppSettings.Get(nameof(PokemonEasierFinalEvolutionLevel)), out int evoLevel) && evoLevel <= 0)
                {
                    PokemonEasierFinalEvolutionLevel = 40;
                    return PokemonEasierFinalEvolutionLevel;
                }
                return evoLevel;
            }
            set
            {
                AddOrUpdateAppSettings(nameof(PokemonEasierFinalEvolutionLevel), value.ToString());
            }
        }

        public static int PokemonEasierSecondEvolutionLevel
        {
            get
            {
                if (!int.TryParse(ConfigurationManager.AppSettings.Get(nameof(PokemonEasierSecondEvolutionLevel)), out int evoLevel) && evoLevel <= 0)
                {
                    PokemonEasierSecondEvolutionLevel = 30;
                    return PokemonEasierSecondEvolutionLevel;
                }
                return evoLevel;
            }
            set
            {
                AddOrUpdateAppSettings(nameof(PokemonEasierSecondEvolutionLevel), value.ToString());
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

        public static string DolphinDirectory
        {
            get
            {
                return ConfigurationManager.AppSettings.Get(nameof(DolphinDirectory));
            }
            set
            {
                AddOrUpdateAppSettings(nameof(DolphinDirectory), value.ToString());
            }
        }

        public static string GameFilePath
        {
            get
            {
                return ConfigurationManager.AppSettings.Get(nameof(GameFilePath));
            }
            set
            {
                AddOrUpdateAppSettings(nameof(GameFilePath), value.ToString());
            }
        }

        public static string APUrl
        {
            get
            {
                return ConfigurationManager.AppSettings.Get(nameof(APUrl));
            }
            set
            {
                AddOrUpdateAppSettings(nameof(APUrl), value.ToString());
            }
        }

        public static string APSlotname
        {
            get
            {
                return ConfigurationManager.AppSettings.Get(nameof(APSlotname));
            }
            set
            {
                AddOrUpdateAppSettings(nameof(APSlotname), value.ToString());
            }
        }

        public static string APPassword
        {
            get
            {
                return ConfigurationManager.AppSettings.Get(nameof(APPassword));
            }
            set
            {
                AddOrUpdateAppSettings(nameof(APPassword), value.ToString());
            }
        }

        public static void AddOrUpdateAppSettings(string key, string value)
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
