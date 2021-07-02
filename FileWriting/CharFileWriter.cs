using CK3_GEDCOM.PrintableGameEntities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CK3_GEDCOM.Helpers;

namespace CK3_GEDCOM.FileWriting
{
    class CharFileWriter : FileWriter
    {
        public static void WriteAllCharacters()
        {
            switch (Settings.GroupCharactersBy)
            {
                case "none":
                    WriteCharacters(Character.AllCharacters, "characters.txt");
                    break;
                case "culture":
                    var allUsedCultures = Character.AllCharacters.Select(x => x.Culture).Distinct();

                    foreach(var culture in allUsedCultures)
                    {
                        WriteCharacters(Character.AllCharacters.Where(x => x.Culture == culture), culture + ".txt");
                    }
                    break;
                case "dynasty":
                    var allUsedDynasties = Character.AllCharacters.Select(x => x.DynastyAffiliation.FullName).Distinct();
                    foreach(var dyn in allUsedDynasties)
                    {
                        WriteCharacters(Character.AllCharacters.Where(x => dyn.Equals(x.DynastyAffiliation?.FullName)), Flatten(dyn) + ".txt");
                    }
                    var charactersWithoutLineage = Character.AllCharacters.Where(x => x.DynastyAffiliation == null);
                    if(charactersWithoutLineage.Any()) WriteCharacters(charactersWithoutLineage, "others.txt");

                    break;
            }
        }

        private static void WriteCharacters(IEnumerable<Character> characters, string fileName)
        {
            string path = GetPath("Output", fileName);
            var fileContents = new StringBuilder();
            var ordered = characters.OrderBy(x => x).ThenBy(x => x.Birth);

            foreach(var chara in ordered)
            {
                fileContents.AppendLine(chara.ToString());
            }
            Program.AddInfo($"Writing character file with {characters.Count()} characters to {path}");
            WriteFile(path, fileContents.ToString());
        }
    }
}
