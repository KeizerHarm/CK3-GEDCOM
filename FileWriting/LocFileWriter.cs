using CK3_GEDCOM.PrintableGameEntities;
using System.Collections.Generic;
using System.Text;

namespace CK3_GEDCOM.FileWriting
{
    class LocFileWriter : FileWriter
    {

        public static void WriteLocFile()
        {
            string fileName = $"generated_loc_l_{Settings.Language}.yml";
            string path = GetPath("Output", fileName);

            var prefixes = new Dictionary<string, string>();
            var baseNames = new Dictionary<string, string>();

            foreach (Lineage dyn in Dynasty.AllDynasties)
            {
                if (dyn.IsVanilla) continue;
                if (dyn.PrefixKey != null && !prefixes.ContainsKey(dyn.PrefixKey))
                {
                    prefixes.Add(dyn.PrefixKey, dyn.Prefix);
                }
                if (!baseNames.ContainsKey(dyn.BaseNameKey))
                {
                    baseNames.Add(dyn.BaseNameKey, dyn.BaseName);
                }
            }
            foreach (Lineage house in House.AllHouses)
            {
                if (house.IsVanilla) continue;
                if (house.PrefixKey != null && !prefixes.ContainsKey(house.PrefixKey))
                {
                    prefixes.Add(house.PrefixKey, house.Prefix);
                }
                if (!baseNames.ContainsKey(house.BaseNameKey))
                {
                    baseNames.Add(house.BaseNameKey, house.BaseName);
                }
            }

            var fileContents = new StringBuilder();
            fileContents.AppendLine($"l_{Settings.Language}:");

            if(prefixes.Count > 0) fileContents.AppendLine($" #Prefixes (many of these may be duplicated by vanilla, but I have no way to tell which, so these keys are suffixed with _1 so that they don't cause errors for duplicate keys.");
            foreach (var prefix in prefixes)
            {
                fileContents.AppendLine($" {prefix.Key}:0 \"{prefix.Value}\"");
            }

            fileContents.AppendLine(" #Dynasty names");

            foreach (var baseName in baseNames)
            {
                fileContents.AppendLine($" {baseName.Key}:0 \"{baseName.Value}\"");
            }

            Program.AddInfo($"Writing localisation file with {prefixes.Count} prefixes and {baseNames.Count} dynasty/house names to {path}");
            WriteFile(path, fileContents.ToString());
        }
    }
}
