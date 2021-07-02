using CK3_GEDCOM.PrintableGameEntities;
using System.Linq;
using System.Text;

namespace CK3_GEDCOM.FileWriting
{
    class DynastiesFileWriter : FileWriter
    {
        public static void WriteAllDynasties()
        {
            string fileName = "generated_dynasties.txt";
            string path = GetPath("Output", fileName);
            var fileContents = new StringBuilder();
            var orderedDyns = Dynasty.AllDynasties.Where(x => !x.IsVanilla).OrderBy(x => x.IdString);

            foreach (var dyn in orderedDyns)
            {
                fileContents.AppendLine(dyn.ToString());
            }
            Program.AddInfo($"Writing dynasty file with {orderedDyns.Count()} dynasties to {path}");
            WriteFile(path, fileContents.ToString());
        }
    }
}
