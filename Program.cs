using CK3_GEDCOM.FileReading;
using CK3_GEDCOM.FileWriting;
using CK3_GEDCOM.PrintableGameEntities;
using System;
using System.Collections.Generic;
using System.Linq;

using static CK3_GEDCOM.Helpers;

namespace CK3_GEDCOM
{
    public class Program
    {
        private static readonly List<string> currentErrors = new List<string>();
        private static readonly List<string> currentWarnings = new List<string>();
        private static readonly List<string> currentInfos = new List<string>();

        public static string GedcomFileName { get; set; }

        public void PrintMessages()
        {
            foreach(var line in currentErrors) Console.WriteLine(line);
            foreach(var line in currentWarnings) Console.WriteLine(line);
            foreach(var line in currentInfos) Console.WriteLine(line);
            currentWarnings.Clear();
            currentInfos.Clear();
        }

        public static void AddError(string error)
        {
            error = "ERROR: " + error;
            currentErrors.Add(error);
            LogFileWriter.WriteToLog(error);
        }

        public static void AddWarning(string warning)
        {
            warning = "WARNING: " + warning;
            currentWarnings.Add(warning);
            LogFileWriter.WriteToLog(warning);
        }

        public static void AddInfo(string info)
        {
            info = "Info: " + info;
            currentInfos.Add(info);
            LogFileWriter.WriteToLog(info);
        }

        public static bool AnyErrors() => currentErrors.Any();

        public static void Main(string[] args)
        {
            new Program();
        }

        public static List<string> traits;
        public static List<string> faiths;
        public static List<string> cultures;

        public Program()
        {
            Console.WriteLine("Welcome to Keizer Harm's GEDCOM to CKIII tool!" + NL +
                "I will transform a GEDCOM file into CKIII character and dynasty files." + NL +
                "Consult the readme for more specific guidelines." + NL);
            Console.WriteLine("Press any key to get started!" + NL);
            Console.ReadKey(true);
            LogFileWriter.DeleteLogFile();
            LogFileWriter.WriteToLog("Running tool...");

            try
            {

                LoadSettings();
                PrintMessages();
                if (AnyErrors()) return;

                LoadCharIdCache();
                PrintMessages();
                if (AnyErrors()) return;

                LoadTraits();
                PrintMessages();
                if (AnyErrors()) return;

                LoadFaiths();
                PrintMessages();
                if (AnyErrors()) return;

                LoadCultures();
                PrintMessages();
                if (AnyErrors()) return;

                LoadDynasties();
                PrintMessages();
                if (AnyErrors()) return;

                LoadHouses();
                PrintMessages();
                if (AnyErrors()) return;

                LoadCharacters();
                PrintMessages();
                if (AnyErrors()) return;


                DynastiesFileWriter.WriteAllDynasties();
                PrintMessages();
                if (AnyErrors()) return;

                HousesFileWriter.WriteAllHouses();
                PrintMessages();
                if (AnyErrors()) return;

                LocFileWriter.WriteLocFile();
                PrintMessages();
                if (AnyErrors()) return;

                CharFileWriter.WriteAllCharacters();
                PrintMessages();
                if (AnyErrors()) return;

                CharIdCacheFileWriter.WriteCharIdFile();
                PrintMessages();
                if (AnyErrors()) return;

                Console.WriteLine();
                Console.WriteLine("Looks like we're all done here! Press any key to close.");
                LogFileWriter.WriteToLog("Run complete.");
                Console.ReadKey(true);
            }
            catch(Exception e)
            {
                AddError(e.Message);
                Console.WriteLine("Something truly unexpected happened... check the log.");
            }
        }

        private void LoadCharIdCache()
        {
            GedcomFileName = FileReader.GetFileName("Input", "*.ged", "gedcom");
            if (AnyErrors()) return;
            var cacheFilePath = FileReader.TryFindFile("Cache", GedcomFileName + CharIdCacheFileWriter.CharIdCacheFileNameSuffix, "character id cache for " + GedcomFileName, false);
            if (cacheFilePath == null) return;
            var noOfCachedIds = CharIdCacheReader.GetCache(cacheFilePath);
            AddInfo($"Loaded in {noOfCachedIds} cached character ids for gedcom {GedcomFileName}");
        }

        private void LoadSettings()
        {
            var filePath = FileReader.TryFindFile("Input", "Settings.txt", "settings");
            if (AnyErrors()) return;
            var foundSettings = SettingsFileReader.GetSettings(filePath);
            if (AnyErrors()) return;
            AddInfo($"Loaded in the following settings:{NL}{foundSettings}");
        }

        private void LoadTraits()
        {
            var filePath = FileReader.TryFindFile("Input", "Traits.txt", "traits");
            if (AnyErrors()) return;
            List<string> foundTraits = FileReader.GetItemsFromFile(filePath);
            AddInfo($"The trait file contains {foundTraits.Count} traits, the last one being '{foundTraits.LastOrDefault()}'");
            traits = foundTraits;
        }

        private void LoadFaiths()
        {
            var filePath = FileReader.TryFindFile("Input", "Faiths.txt", "faiths");
            if (AnyErrors()) return;
            List<string> foundFaiths = FileReader.GetItemsFromFile(filePath);
            if(foundFaiths.Count == 0)
            {
                AddError("The faiths file contains no faiths! Add all vanilla faiths!");
                return;
            }
            AddInfo($"The faiths file contains {foundFaiths.Count} faiths, the last one being '{foundFaiths.LastOrDefault()}'");
            faiths = foundFaiths;
        }

        private void LoadCultures()
        {
            var filePath = FileReader.TryFindFile("Input", "Cultures.txt", "cultures");
            if (AnyErrors()) return;
            List<string> foundCultures = FileReader.GetItemsFromFile(filePath);
            if (foundCultures.Count == 0)
            {
                AddError("The cultures file contains no cultures! Add all vanilla cultures!");
                return;
            }
            AddInfo($"The cultures file contains {foundCultures.Count} cultures, the last one being '{foundCultures.LastOrDefault()}'");
            cultures = foundCultures;
        }

        private void LoadDynasties()
        {
            var filePath = FileReader.TryFindFile("Input", "Dynasties.csv", "dynasties");
            if (AnyErrors()) return;
            DynastiesFileReader.GetDynasties(filePath);
            if (AnyErrors()) return;
            AddInfo($"The dynasties file contains {Dynasty.AllDynasties.Count} dynasties, the last one being '{Dynasty.AllDynasties.LastOrDefault()?.BaseName}'");
        }

        private void LoadHouses()
        {
            var filePath = FileReader.TryFindFile("Input", "Houses.csv", "houses");
            if (AnyErrors()) return;
            HousesFileReader.GetHouses(filePath);
            if (AnyErrors()) return;
            AddInfo($"The houses file contains {House.AllHouses.Count} houses, the last one being '{House.AllHouses.LastOrDefault()?.BaseName}'");
        }

        private void LoadCharacters()
        {
            var filePath = FileReader.TryFindFile("Input", "*.ged", "gedcom");
            if (AnyErrors()) return;
            GedcomReader.ReadGedcom(filePath);
            if (AnyErrors()) return;
            AddInfo($"The gedcom file contains {Character.AllCharacters.Count} characters, the last one being '{Character.AllCharacters.LastOrDefault()?.FullName}'");
        }
    }
}
