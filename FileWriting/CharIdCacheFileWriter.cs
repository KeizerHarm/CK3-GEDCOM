using System.Text;
using static CK3_GEDCOM.Helpers;

namespace CK3_GEDCOM.FileWriting
{
    class CharIdCacheFileWriter : FileWriter
    {
        public static string CharIdCacheFileNameSuffix = "_CharIdCache.csv";

        public static string COMMENT = $"#This file stores relationships between GEDCOM Ids and CK3 character ids once it makes them. This one in particular is for the GEDCOM file by the name of '{Program.GedcomFileName}'. It will not be used for generation from a GEDCOM file of any other name.{NL}";
        public static string HEADERS = "CK3-id;Gedcom-id";

        public static void WriteCharIdFile()
        {
            string fileName = Program.GedcomFileName + CharIdCacheFileNameSuffix;
            var fileContents = new StringBuilder();

            fileContents.AppendLine(COMMENT);
            fileContents.AppendLine(HEADERS);

            var idPairs = CharacterIdCounter.GetIdPairs();

            foreach(var idPair in idPairs)
            {
                fileContents.AppendLine($"{idPair.Ck3Id};{idPair.GedcomId}");
            }

            string path = GetPath("Cache", fileName);
            Program.AddInfo($"Writing character id cache file with {idPairs.Count} cached ids to {path}");
            WriteFile(path, fileContents.ToString());
        }
    }
}
