using CK3_GEDCOM.PrintableGameEntities;
using System.Linq;
using System.Text;

namespace CK3_GEDCOM.FileWriting
{
    class HousesFileWriter : FileWriter
    {
        public static void WriteAllHouses()
        {
            string fileName = "generated_dynasty_houses.txt";
            string path = GetPath("Output", fileName);
            var fileContents = new StringBuilder();
            var orderedHouses = House.AllHouses.Where(x => !x.IsVanilla).OrderBy(x => x.IdString);

            foreach (var house in orderedHouses)
            {
                fileContents.AppendLine(house.ToString());
            }
            Program.AddInfo($"Writing house file with {orderedHouses.Count()} houses to {path}");
            WriteFile(path, fileContents.ToString());
        }
    }
}
