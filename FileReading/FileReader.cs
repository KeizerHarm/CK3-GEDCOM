using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CK3_GEDCOM.FileReading
{
    public class FileReader
    {
        public static List<string> GetItemsFromFile(string filePath)
        {
            var foundItems = new List<string>(File.ReadAllLines(filePath).Select(x => x.Split('#')[0]).SelectMany(x => x.Split(' ').Select(y => y.Trim())));
            foundItems.RemoveAll(x => string.IsNullOrEmpty(x));
            return foundItems;
        }

        public static string TryFindFile(string dir, string fileName, string name, bool isCrucial = true) 
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), dir);
            if (!Directory.Exists(filePath))
            {
                if (isCrucial) Program.AddError($"Could not find directory {filePath} when looking for {name} file!");
                else Program.AddWarning($"Could not find directory {filePath} when looking for {name} file!");
                return null;
            }

            var possibleFiles = Directory.GetFiles(filePath, fileName);
            if (possibleFiles.Length < 1)
            {
                if(isCrucial) Program.AddError($"Could find no {name} file titled {fileName} in {filePath}!");
                else Program.AddWarning($"Could find no {name} file titled {fileName} in {filePath}!");
                return null;
            }
            Program.AddInfo($"Found {name} file at {possibleFiles[0]}");
            return possibleFiles[0];
        }

        public static string GetFileName(string dir, string fileName, string name)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), dir);
            var possibleFiles = Directory.GetFiles(filePath, fileName);
            if (possibleFiles.Length < 1)
            {
                Program.AddError($"Could find no {name} file titled {fileName} in {filePath}!");
                return null;
            }
            return Path.GetFileNameWithoutExtension(possibleFiles[0]);
        }
    }
}
