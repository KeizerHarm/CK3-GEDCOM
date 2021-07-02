using System;
using System.Collections.Generic;
using System.Linq;
using CK3_GEDCOM.PrintableGameEntities;
using GeneGenie.Gedcom;
using GeneGenie.Gedcom.Parser;

using static CK3_GEDCOM.Helpers;

namespace CK3_GEDCOM.FileReading
{
    public class GedcomReader
    {
        private static GedcomRecordReader gedcomReader;

        public static void ReadGedcom(string pathToGedcom)
        {
            gedcomReader = GedcomRecordReader.CreateReader(pathToGedcom);
            gedcomReader.ReplaceXRefs = false;

            if (gedcomReader.Parser.ErrorState != GeneGenie.Gedcom.Enums.GedcomErrorState.NoError)
            {
                Program.AddError("Could not parse Gedcom! " + NL + gedcomReader.Parser.ErrorState);
                return;
            }

            foreach (var person in gedcomReader.Database.Individuals)
            {
                PersonToCharacter(person);
                if (Program.AnyErrors()) return;
            }

            foreach (var person in gedcomReader.Database.Individuals)
            {
                SetupFamily(person);
                if (Program.AnyErrors()) return;
            }
        }

        private static void PersonToCharacter(GedcomIndividualRecord person)
        {

            var chara = new Character(person.XRefID);

            if (person.SexChar == null)
            {
                Program.AddError("One character with ID {GetProperId(chara.GedcomId)} does not have a defined gender. I am sorry, but that is too progressive for mediæval times.");
                return;
            }
            chara.Female = person.SexChar.ToString().ToLower() == "f";
            if (string.IsNullOrWhiteSpace(person.Names.FirstOrDefault()?.Name))
            {
                Program.AddWarning($"One character with ID {GetProperId(chara.GedcomId)} does not have a defined name. You are horrible for putting them through this.");
            }
            chara.Name = person.Names.First().Name.Split('/')[0].Trim();


            string surname = GetSurname(person.Names.First());
            var dyn = Dynasty.AllDynasties.FirstOrDefault(x => x.FullName == surname);
            var house = House.AllHouses.FirstOrDefault(x => x.FullName == surname);
            chara.SetLineage(dyn, house);


            foreach (var evt in person.Events)
            {
                if (evt.GedcomTag == "BIRT" && evt.Date != null)
                {
                    chara.Birth = ParseDate(evt.Date.Date1);
                }
                if (evt.GedcomTag == "DEAT" && evt.Date != null)
                {
                    chara.Death = ParseDate(evt.Date.Date1);
                }
            }

            //If no birth date is defined, make one up:
            if (chara.Birth == null)
            {
                chara.Birth = "1000.1.1"; //No better way to make one atm
                Program.AddWarning($"Character {chara.Name} {GetSurname(person.Names.First())} does not have a birthdate!");
            }

            //If no death date defined, make one up.
            if (chara.Death == null)
            {
                var seed = GetSeedFromName(chara.Name + " " + GetSurname(person.Names.First()));
                var random = new Random(seed).Next(50, 80);
                chara.Death = int.Parse(chara.Birth.Substring(0, 4)) + random + ".1.1";
                Program.AddInfo($"Character {chara.Name} {GetSurname(person.Names.First())} does not have a death date! Randomly generating one: {chara.Death}");
            }

            //Notes
            foreach (var note in person.Notes)
            {
                var foundNote = person.Database.Notes.Find(x => x.XRefID == note);

                UseNotes(foundNote.Text, chara);
            }
        }

        private static string GetProperId(string xRefId)
        {
            var toString = gedcomReader.Parser.XrefCollection.ToString();
            var properKeyList = toString.Split(new char[] { '{', ',', '}' }).Where(x => !string.IsNullOrEmpty(x)).ToArray();

            var number = int.Parse(xRefId.Substring(4));
            var properId = properKeyList[number - 1];
            return properId;
        }

        private static int GetSeedFromName(string name)
        {
            int sum = 0;
            foreach (var chr in name)
            {
                sum += chr;
            }
            return sum;
        }

        private static void UseNotes(string text, Character chara)
        {
            foreach (var note in text.ToLower().Split(new char[] { ' ', '\n', '\r' }))
            {
                if (Program.traits.Contains(note)) chara.Traits.Add(note);
                if (Program.faiths.Contains(note)) chara.Faith = note;
                if (Program.cultures.Contains(note)) chara.Culture = note;
            }
        }

        private static void SetupFamily(GedcomIndividualRecord person)
        {
            var chara = FindCharacter(person.XRefID);

            foreach (var family in person.ChildIn)
            {
                string familyId = family.Family;

                var properfam = person.Database.Families.FirstOrDefault(x => x.XRefID == familyId);
                if (properfam == null)
                {
                    Program.AddWarning($"Character {chara.Name} is a child in the family {GetProperId(familyId)}, which does not seem to exist. Either they are a liar or your GEDCOM has issues...");
                    return;
                }

                var father = FindCharacter(properfam.Husband);
                var mother = FindCharacter(properfam.Wife);

                if (father != null) chara.Father = father;
                if (mother != null) chara.Mother = mother;
            }


            foreach (var family in person.SpouseIn)
            {
                string familyId = family.Family;

                var properfam = person.Database.Families.FirstOrDefault(x => x.XRefID == familyId);
                if (properfam == null)
                {
                    Program.AddWarning($"The family ID {GetProperId(familyId)}, which {chara.Name} is a spouse in, refers to a family that does not exist.");
                    return;
                }

                //Only add marriages it for the ladies, otherwise marriages are duplicated
                if (chara.Female)
                {
                    var father = FindCharacter(properfam.Husband);
                    if (father != null && properfam.Marriage != null)
                    {

                        var marryDate = properfam.Marriage.Date != null
                            ? ParseDate(properfam.Marriage.Date.Date1)
                            : int.Parse(chara.Birth.Substring(0, 4)) + 17 + ".1.1";

                        var divorce = properfam.Events.FirstOrDefault(x => x.GedcomTag == "DIV" || x.GedcomTag == "ANUL");
                        if (divorce != null)
                        {
                            chara.Marry(father, marryDate, ParseDate(divorce.Date.Date1));
                        }
                        else chara.Marry(father, marryDate);

                    }
                }
            }

        }

        private static Character FindCharacter(string xRefID)
        {
            return Character.AllCharacters.FirstOrDefault(x => x.GedcomId == xRefID);
        }

        private static readonly List<string> MONTHS = new List<string>(new string[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" });
        private static string ParseDate(string date)
        {
            //Full date
            var dateSplit = date.ToUpper().Split(" ");
            if (dateSplit.Length == 3 && MONTHS.Contains(dateSplit[1]))
            {
                return dateSplit[2] + "." + (MONTHS.IndexOf(dateSplit[1]) + 1) + "." + dateSplit[0];
            }

            //Just year
            if (int.TryParse(date, out int year))
            {
                return year + ".1.1";
            }

            return "";
        }

        private static string GetSurname(GedcomName name)
        {
            if (name.Name.Split('/').Length > 1)
            {
                return name.Name.Split('/')[1].Trim();
            }
            else if (!string.IsNullOrWhiteSpace(name.Surname))
            {
                return name.SurnamePrefix.Trim() + " " + name.Surname.Trim();
            }
            else
            {
                return "";
            }
        }
    }
}
