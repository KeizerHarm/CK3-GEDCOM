using System.Collections.Generic;
using System.Linq;
using static CK3_GEDCOM.Helpers;

namespace CK3_GEDCOM.PrintableGameEntities
{
    public class House : Lineage
    {
        public static List<House> AllHouses = new List<House>();
        public string DynastyIdString { get; set; }

        private Dynasty _dynasty;

        public Dynasty Dynasty
        {
            get
            {
                if(_dynasty == null)
                {
                    _dynasty = Dynasty.AllDynasties.FirstOrDefault(x => x.IdString == DynastyIdString);
                }
                return _dynasty;
            }
        }

        
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
                $"    dynasty = \"{DynastyIdString}\"";
            string dynasty = Dynasty.AllDynasties.FirstOrDefault(x => x.IdString == DynastyIdString)?.FullName;
            if (dynasty != null)
            {
                text += $" #{dynasty}";
            }
            text +=    $"{NL}}}{NL}";

            return text;
        }

        public House()
        {
            AllHouses.Add(this);
        }
    }
}
