using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AmethystWindows.Settings
{
    public abstract class SettingsManager<T> where T : SettingsManager<T>, new()
    {
        private static readonly string filePath = GetLocalFilePath($"{typeof(T).Name}.json");

        public static T Instance { get; private set; }

        private static string GetLocalFilePath(string fileName)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var companyName = Assembly.GetEntryAssembly().GetCustomAttributes<AssemblyCompanyAttribute>().FirstOrDefault();
            return Path.Combine(appData, companyName?.Company ?? Assembly.GetEntryAssembly().GetName().Name, fileName);
        }

        public static void Load()
        {
            if (File.Exists(filePath))
                Instance = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
            else
                Instance = new T();
        }

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(Instance);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, json);
        }
    }
}
