using System;
using System.IO;
using System.Linq;
using CK3_GEDCOM.PrintableGameEntities;
using Csv;

using static CK3_GEDCOM.Helpers;

namespace CK3_GEDCOM.FileReading
{
    public class DynastiesFileReader
    {
        private readonly static string PREFIX_HEADER = "Prefix";
        private readonly static string NAME_HEADER = "Name";
        private readonly static string CULTURE_HEADER = "Culture";
        private readonly static string FAITH_HEADER = "Faith";
        private readonly static string ID_HEADER = "Id";
        private readonly static string IS_VANILLA_HEADER = "IsVanilla";

        public static void GetDynasties(string pathToExistingDyns)
        {
            var csv = File.ReadAllText(pathToExistingDyns);
            var lines = CsvReader.ReadFromText(csv).ToArray();

            if (lines.Length < 1)
            {
                return;
            }

            if (!lines[0].Headers.Contains(PREFIX_HEADER) ||
                !lines[0].Headers.Contains(NAME_HEADER) ||
                !lines[0].Headers.Contains(CULTURE_HEADER) ||
                !lines[0].Headers.Contains(FAITH_HEADER) ||
                !lines[0].Headers.Contains(ID_HEADER) ||
                !lines[0].Headers.Contains(IS_VANILLA_HEADER))
            {
                Program.AddError($"Dynasty csv headers are incorrect. Consult the readme file for formatting guidelines.");
                return;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                ICsvLine line = lines[i];

                try
                {
                    if (!string.IsNullOrWhiteSpace(line[NAME_HEADER]))
                    {
                        Dynasty dyn = new Dynasty()
                        {
                            Prefix = line[PREFIX_HEADER],
                            BaseName = line[NAME_HEADER],
                            Culture = line[CULTURE_HEADER].ToLowerInvariant(),
                            Faith = line[FAITH_HEADER].ToLowerInvariant(),
                            IdString = line[ID_HEADER],
                            IsVanilla = line[IS_VANILLA_HEADER] == "yes"
                        };
                    }
                }
                catch (Exception e)
                {
                    Program.AddWarning($"Could not properly parse line {i} of the dynasty csv!{NL}{e.Message}");
                    continue;
                }
            }
            return;
        }
    }
}
