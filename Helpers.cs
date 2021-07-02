using CK3_GEDCOM.FileWriting;
using CK3_GEDCOM.PrintableGameEntities;
using System;
using System.Globalization;
using System.Text;

namespace CK3_GEDCOM
{
    public static class Helpers
    {
        public static string NL = Environment.NewLine;

        /// <summary>
        /// Observed from localisation: diacritics are flattened by converting the base char to uppercase and appending an underscore. E.g.: Ribagorça => RibagorC_a, Rübsamen => RU_bsamen.
        /// Furthermore, spaces are also converted to underscores, but dashes are kept intact.
        /// </summary>
        public static string Flatten(string text)
        {
            // Decompose characters and diacritics to separate chars
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char thisChar = normalizedString[i];

                // If this character is the diacritic, skip it.
                if (IsADiacritic(thisChar)) continue;

                // If this is last character, just append it.
                if (i + 1 == normalizedString.Length)
                {
                    stringBuilder.Append(SpaceToUnderscore(thisChar));
                    continue;
                }

                char nextChar = normalizedString[i + 1];
                // If next character is the diacritic, append uppercase of this character and underscore.
                if (IsADiacritic(nextChar))
                {
                    stringBuilder.Append(thisChar.ToString().ToUpperInvariant() + '_');
                    continue;
                }

                // Otherwise just append this character.
                stringBuilder.Append(SpaceToUnderscore(thisChar));
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            static char SpaceToUnderscore(char thisChar)
            {
                if (thisChar.Equals(' ') || thisChar.Equals('\''))
                    return '_';
                return thisChar;
            }

            static bool IsADiacritic(char thisChar)
            {
                var thisCharUniCategory = CharUnicodeInfo.GetUnicodeCategory(thisChar);
                return thisCharUniCategory == UnicodeCategory.NonSpacingMark;
            }
        }

        public static void PrintEverything()
        {
            Console.WriteLine($"DYNASTIES:");
            foreach (Dynasty dyn in Dynasty.AllDynasties)
            {
                Console.WriteLine(dyn.ToString());
            }
            Console.WriteLine($"HOUSES:");
            foreach (House house in House.AllHouses)
            {
                Console.WriteLine(house.ToString());
            }
            Console.WriteLine($"CHARACTERS:");
            foreach (Character chara in Character.AllCharacters)
            {
                Console.WriteLine(chara.ToString());
            }
            Console.WriteLine($"LOCALISATION:");
            LocFileWriter.WriteLocFile();
        }
    }
}
