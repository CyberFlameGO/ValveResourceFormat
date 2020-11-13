using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using ValveKeyValue;

namespace GUI.Utils
{
    internal static class Settings
    {
        public class AppConfig
        {
            public List<string> GameSearchPaths { get; set; }
            public string BackgroundColor { get; set; } = string.Empty;
            public string OpenDirectory { get; set; } = string.Empty;
            public string SaveDirectory { get; set; } = string.Empty;
            public Dictionary<string, float[]> SavedCameras { get; set; }
            public List<string> RecentPackages { get; set; }
        }

        private static string SettingsFilePath;

        public static AppConfig Config { get; set; } = new AppConfig();

        public static Color BackgroundColor { get; set; } = Color.FromArgb(60, 60, 60);

        public static void Load()
        {
            SettingsFilePath = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName), "settings.txt");

            if (!File.Exists(SettingsFilePath))
            {
                Save();
                return;
            }

            using (var stream = new FileStream(SettingsFilePath, FileMode.Open, FileAccess.Read))
            {
                Config = KVSerializer.Create(KVSerializationFormat.KeyValues1Text).Deserialize<AppConfig>(stream, KVSerializerOptions.DefaultOptions);
            }

            BackgroundColor = ColorTranslator.FromHtml(Config.BackgroundColor);

            Config.GameSearchPaths ??= new List<string>();
            Config.RecentPackages ??= new List<string>();
            Config.SavedCameras ??= new Dictionary<string, float[]>();
        }

        public static void Save()
        {
            Config.BackgroundColor = ColorTranslator.ToHtml(BackgroundColor);

            using (var stream = new FileStream(SettingsFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                KVSerializer.Create(KVSerializationFormat.KeyValues1Text).Serialize(stream, Config, nameof(ValveResourceFormat));
            }
        }

        public static void AddRecentPackage(string path)
        {
            Config.RecentPackages.Remove(path);
            Config.RecentPackages.Add(path);

            if (Config.RecentPackages.Count > 10)
            {
                Config.RecentPackages.RemoveAt(0);
            }

            Save();
        }
    }
}
