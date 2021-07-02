using System;
using static CK3_GEDCOM.Helpers;

namespace CK3_GEDCOM.PrintableGameEntities
{
    public abstract class Lineage
    {
        public string IdString { get; set; }


        private string _baseName;
        public string BaseName
        {
            get
            {
                return _baseName;
            }
            set
            {
                _baseName = value;

                BaseNameKey = "dynn_" + Flatten(value);
            }
        }
        public string BaseNameKey { get; set; }

        private string _prefix;
        public string Prefix
        {
            get
            {
                return _prefix;
            }
            set
            {
                _prefix = value;
                if (!string.IsNullOrEmpty(value))
                {
                    PrefixKey = "dynnp_" + Flatten(value.Trim()) + "_1";
                }
            }
        }
        public string PrefixKey { get; set; }

        private string _fullName;
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(_fullName))
                {
                    if (!string.IsNullOrEmpty(Prefix))
                    {
                        _fullName = Prefix + BaseName;
                    }
                    else
                    {
                        _fullName = BaseName;
                    }
                }
                return _fullName;
            }
        }



        public string Culture { get; set; }
        public string Faith { get; set; } //Not used in game, just to make assigning faiths to characters easier

        public bool IsVanilla { get; set; }
    }
}