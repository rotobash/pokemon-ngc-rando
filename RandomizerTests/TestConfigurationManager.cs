using System;
using System.Collections.Generic;
using System.Configuration;
using XDCommon;

namespace RandomizerTests
{
    public static class TestConfiguration
    {
        public static string TestRomPath
        {
            get
            {
                return ConfigurationManager.AppSettings.Get(nameof(TestRomPath)) ?? string.Empty;
            }
            set
            {
                XDCommon.Configuration.AddOrUpdateAppSettings(nameof(TestRomPath), value.ToString());
            }
        }
    }
}
