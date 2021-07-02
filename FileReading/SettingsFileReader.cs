using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CK3_GEDCOM.FileReading
{
    public class SettingsFileReader
    {
        public static Settings GetSettings(string path)
        {
            List<string> lines = new List<string>(File.ReadAllLines(path).Select(x => x.Split('#')[0]));
            lines.RemoveAll(x => string.IsNullOrEmpty(x));
            lines.RemoveAll(x => !x.Contains('='));
            if (lines.Count == 0)
            {
               Program.AddWarning($"WARNING: No settings found! Will use defaults for everything then...");
            }

            Dictionary<string, string> settingsDict = new Dictionary<string, string>();
            foreach(var line in lines)
            {
                var splitLine = line.Split('=');
                var key = splitLine[0].Trim();
                string value;
                if(splitLine.Length >= 2)
                {
                    value = splitLine[1].Trim();
                } else
                {
                    value = null;
                }
                settingsDict.Add(key, value);
            }
            return new Settings(settingsDict);
        }
    }
}
