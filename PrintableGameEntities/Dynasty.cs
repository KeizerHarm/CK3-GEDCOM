using System.Collections.Generic;
using static CK3_GEDCOM.Helpers;

namespace CK3_GEDCOM.PrintableGameEntities
{
    public class Dynasty : Lineage
    {
        public static List<Dynasty> AllDynasties = new List<Dynasty>();

        public override string ToString()
        {
            if (IsVanilla) return "";
            var text = $"{IdString} = {{ #{FullName}{NL}";
            if (!string.IsNullOrEmpty(PrefixKey))
            {
                text += $"    prefix = \"{PrefixKey}\"{NL}";
            }
            text +=
                $"    name = \"{BaseNameKey}\"{NL}" +
                $"    culture = \"{Culture}\"{NL}" +
                $"}}{NL}";

            return text;
        }

        public Dynasty()
        {
            AllDynasties.Add(this);
        }
    }
}
