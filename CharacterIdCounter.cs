using System.Collections.Generic;
using System.Linq;

namespace CK3_GEDCOM
{
    class CharacterIdCounter
    {

        public int LatestIdNr { get; set; }
        public string Namespace { get; set; }

        public string GetId()
        {
            var attempt = "";
            while (true)
            {
                if (!string.IsNullOrEmpty(Namespace)) attempt += Namespace + ".";
                attempt += LatestIdNr++;
                if (!IdAlreadyExists(attempt)) return attempt;
            }
        }

        public string GetNextIdOrCached(string gedcomId)
        {
            if (TryGetCachedCk3Id(gedcomId, out string ck3Id)) return ck3Id;

            var newId = "";
            while (true)
            {
                if (!string.IsNullOrEmpty(Namespace)) newId += Namespace + ".";
                newId += LatestIdNr++;
                if (!IdAlreadyExists(newId))
                {
                    AddIdPair(gedcomId, newId);
                    return newId;
                }
            }
        }

        private static List<IdPair> cachedIds = new List<IdPair>();
        public bool IdAlreadyExists(string ck3_id)
        {
            return cachedIds.Any(x => x.Ck3Id.Equals(ck3_id));
        }
        public bool TryGetCachedCk3Id(string gedcomId, out string ck3Id)
        {
            ck3Id = cachedIds.FirstOrDefault(x => x.GedcomId.Equals(gedcomId))?.Ck3Id;
            return ck3Id != null;
        }
        public static void AddIdPair(string gedcomId, string ck3Id)
        {
            cachedIds.Add(new IdPair(ck3Id, gedcomId));
        }

        public static List<IdPair> GetIdPairs() => new List<IdPair>(cachedIds);
    }

    class IdPair
    {
        public IdPair(string ck3Id, string gedcomId)
        {
            Ck3Id = ck3Id;
            GedcomId = gedcomId;
        }

        public string Ck3Id { get; set; }
        public string GedcomId { get; set; }
    }
}
