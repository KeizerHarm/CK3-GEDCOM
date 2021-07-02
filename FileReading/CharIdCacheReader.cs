using System;
using System.IO;
using System.Linq;
using Csv;
using static CK3_GEDCOM.Helpers;

namespace CK3_GEDCOM.FileReading
{
    class CharIdCacheReader
    {
        private readonly static string CK3_ID_HEADER = "CK3-id";
        private readonly static string GEDCOM_ID_HEADER = "Gedcom-id";

        public static int GetCache(string pathToCacheFile)
        {
            var csv = File.ReadAllText(pathToCacheFile);
            var lines = CsvReader.ReadFromText(csv).ToArray();
            int noOfCachedEntries = 0;

            if (lines.Length < 1)
            {
                //File exists but is empty.
                return 0;
            }

            if (!lines[0].Headers.Contains(CK3_ID_HEADER) ||
                !lines[0].Headers.Contains(GEDCOM_ID_HEADER))
            {
                Program.AddError($"Character id cache csv headers are incorrect. Consult the readme file for formatting guidelines.");
                return 0;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                ICsvLine line = lines[i];

                try
                {
                    if (!string.IsNullOrWhiteSpace(line[GEDCOM_ID_HEADER]))
                    {
                        CharacterIdCounter.AddIdPair(line[GEDCOM_ID_HEADER], line[CK3_ID_HEADER]);
                        noOfCachedEntries++;
                    }
                }
                catch (Exception e)
                {
                    Program.AddWarning($"Could not properly parse line {i} of the character id cache csv!{NL}{e.Message}");
                    continue;
                }
            }
            return noOfCachedEntries;
        }
    }
}
