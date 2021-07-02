using System;
using System.Collections.Generic;
using System.Linq;
using static CK3_GEDCOM.Helpers;

namespace CK3_GEDCOM.PrintableGameEntities
{
    class Character : IComparable
    {
        private static CharacterIdCounter _charIdCounter;
        public static CharacterIdCounter CharIdCounter
        {
            get
            {
                _charIdCounter ??= new CharacterIdCounter
                {
                    Namespace = Settings.CharacterNamespace,
                    LatestIdNr = Settings.FirstPersonId
                };
                return _charIdCounter;
            }
        }
        public string IdString { get; }

        public string GedcomId { get; set; }

        public string Birth { get; set; }
        public string Death { get; set; }
        public string Name { get; set; }

        private string _fullName;
        public string FullName { 
            get
            {
                if(Lineage.FullName != "")
                {
                    _fullName = Name + " " + Lineage.FullName;
                }
                else
                {
                    _fullName = Name;
                }
                return _fullName;
            } 
        }

        private string _faith;
        public string Faith
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_faith))
                {
                    if (Lineage != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Lineage.Faith))
                        {
                            _faith = Lineage.Faith;
                            return _faith;
                        }
                    }
                    Program.AddWarning($"Character {FullName} (gedcom-id {GedcomId}, CK3-id {IdString}) cannot be assigned a faith from their dynasty, house, or own description");
                    return "nofaith";
                }
                else return _faith;
            }
            set
            {
                _faith = value;
            }
        }
        private string _culture;
        public string Culture
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_culture))
                {
                    if (Lineage != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Lineage.Culture))
                        {
                            _culture = Lineage.Culture;
                            return _culture;
                        }
                    }
                    Program.AddWarning($"Character {FullName} (gedcom-id {GedcomId}, CK3-id {IdString}) cannot be assigned a culture from their dynasty, house, or own description");
                    return "noculture";
                }
                else return _culture;
            }
            set
            {
                _culture = value;
            }
        }
        public string DNA { get; set; }

        public Lineage Lineage { get; set; }

        private Dynasty _dynastyAffiliation;
        public Dynasty DynastyAffiliation {
            get
            {
                if(_dynastyAffiliation == null)
                {
                    if (Lineage != null)
                    {
                        if (Lineage is Dynasty dyn)
                        {
                            _dynastyAffiliation = dyn;
                        }
                        else
                        {
                            _dynastyAffiliation = ((House)Lineage).Dynasty;
                        }
                    }
                    else if (Marriages.Count > 0)
                    {
                        var spouseWithLineage = Marriages.LastOrDefault(x => x.GetOtherCharacter(this).Lineage != null).GetOtherCharacter(this);
                        if (spouseWithLineage != null)
                        {
                            _dynastyAffiliation = spouseWithLineage.DynastyAffiliation;
                        }
                    }
                    else if (Mother?.Lineage != null) { _dynastyAffiliation = Mother.DynastyAffiliation; }
                    else if (Father?.Lineage != null) { _dynastyAffiliation = Mother.DynastyAffiliation; }
                }

                return _dynastyAffiliation;
            } 
        }

        public void SetLineage(Dynasty dynasty, House house)
        {
            if (house != null) Lineage = house;
            else Lineage = dynasty;
        }

        public List<string> Traits { get; set; } = new List<string>();
        public bool Female { get; set; }

        public Character Father { get; set; }
        public Character Mother { get; set; }
        public List<Marriage> Marriages { get; set; } = new List<Marriage>();
        public Marriage CurrentMarriage
        {
            get
            {
                return Marriages.Find(x => x.EndDate == null);
            }
        }

        public static List<Character> AllCharacters { get; set; } = new List<Character>();

        public Character(string gedcomId)
        {
            GedcomId = gedcomId;
            IdString = CharIdCounter.GetNextIdOrCached(GedcomId);
            AllCharacters.Add(this);
        }

        public void Marry(Character spouse, string startDate, string endDate = null)
        {
            Marriages.Add(new Marriage()
            {
                Char1 = this,
                Char2 = spouse,
                StartDate = startDate,
                EndDate = endDate
            });

            if (spouse.Marriages.Find(x => x.StartDate == startDate) == null)
            {
                spouse.Marry(this, startDate, endDate);
            }
        }

        public override string ToString()
        {

            string txt =
                $"{IdString} = {{{NL}" +
                $"    name = \"{Name}\"{NL}" +
                $"    culture = \"{Culture}\"{NL}" +
                $"    faith = \"{Faith}\"{NL}";

            if (Female) txt += $"    female = yes{NL}";
            if(Lineage != null)
            {
                if(Lineage is Dynasty) txt += $"    dynasty = {Lineage.IdString} #{Lineage.FullName}{NL}";
                if(Lineage is House) txt += $"    dynasty_house = {Lineage.IdString} #{Lineage.FullName}{NL}";
            }

            if (Father != null) txt += $"    father = {Father.IdString} #{Father.Name}{NL}";
            if (Mother != null) txt += $"    mother = {Mother.IdString} #{Mother.Name}{NL}";
            if (DNA != null) txt += $"    dna = \"{DNA}\"{NL}";

            foreach (var trait in Traits)
            {
                txt += $"    trait = {trait}{NL}";
            }

            txt +=
                $"    {Birth} = {{{NL}" +
                $"        birth = yes{NL}" +
                $"    }}{NL}";

            foreach (var marriage in Marriages)
            {
                txt +=
                    $"    {marriage.StartDate} = {{{NL}" +
                    $"        add_spouse = {marriage.Char2.IdString} #{marriage.Char2.Name}{NL}" +
                    $"    }}{NL}";
                if (marriage.EndDate != null)
                {
                    txt +=
                        $"    {marriage.EndDate} = {{{NL}" +
                        $"        remove_spouse = {marriage.Char2.IdString} #{marriage.Char2.Name}{NL}" +
                        $"    }}{NL}";
                }
            }

            if (Death != null)
            {
                txt +=
                    $"    {Death} = {{{NL}" +
                    $"        death = yes{NL}" +
                    $"    }}{NL}";
            }
            return txt + "}" + NL;
        }



        public int CompareTo(object obj)
        {
            if (obj is Character chara)
            {
                if (tryGetNumericComponent(IdString, out int thisNo) && tryGetNumericComponent(chara.IdString, out int otherNo))
                {
                    return thisNo.CompareTo(otherNo);
                }
                return IdString.CompareTo(chara.IdString);
            }
            return 0;

            static bool tryGetNumericComponent(string str, out int number)
            {
                if (int.TryParse(str, out number))
                {
                    return true;
                }

                if (str.Split('.').Length > 1
                    && int.TryParse(str.Split('.')[1], out number))
                {
                    return true;
                }

                return false;
            }
        }
    }

    class Marriage
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public Character Char1 { get; set; }
        public Character Char2 { get; set; }

        public Character GetOtherCharacter(Character thisChar)
        {
            if (Char1 == thisChar) return Char2;
            return Char1;
        }
    }
}
