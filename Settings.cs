using System;
using System.Collections.Generic;
using static CK3_GEDCOM.Helpers;

namespace CK3_GEDCOM
{
    public class Settings
    {
        private static readonly string FIRST_CHARACTER_ID_KEY = "FirstCharacterId";
        private static readonly string CHARACTER_NAMESPACE_KEY = "CharacterNamespace";
        private static readonly string LANGUAGE_KEY = "Language";
        private static readonly string GROUP_CHARACTERS_BY_KEY = "group_characters_by";

        public static int FirstPersonId = 1;
        public static string CharacterNamespace = "";
        public static string Language = "";
        public static string GroupCharactersBy = "";

        public Settings(Dictionary<string, string> settingsDict)
        {
            SetSetting(ref FirstPersonId, FIRST_CHARACTER_ID_KEY, settingsDict, 1);
            SetSetting(ref CharacterNamespace, CHARACTER_NAMESPACE_KEY, settingsDict, "");
            SetSetting(ref Language, LANGUAGE_KEY, settingsDict, "english");
            SetSetting(ref GroupCharactersBy, GROUP_CHARACTERS_BY_KEY, settingsDict, "none");
        }

        public void SetSetting<TSetting>(ref TSetting settingToSet, string key, Dictionary<string, string> settingDict, TSetting defaultValue)
        {
            if(settingDict.TryGetValue(key, out string value) && value != null)
            {
                if(typeof(TSetting) != typeof(string))
                {
                    try
                    {
                        TSetting parsedValue = (TSetting)Convert.ChangeType(value, typeof(TSetting));
                        settingToSet = parsedValue;
                    }
                    catch (InvalidCastException)
                    {
                        Program.AddError($"Settings file contains setting {key} but improperly formatted - it should be a {typeof(TSetting).Name}");
                    }
                } 
                else
                {
                    settingToSet = (TSetting)Convert.ChangeType(value, typeof(TSetting));
                }
            } 
            else if(defaultValue != null)
            {
                settingToSet = defaultValue;
                Program.AddWarning("Settings file does not contain setting " + key + ", using default value '" + defaultValue.ToString() + "'");
            }
            else
            {
                Program.AddError("Settings file does not contain crucial setting " + key);
            }
        }

        public override string ToString()
        {
            return $"{FIRST_CHARACTER_ID_KEY}: {FirstPersonId + "" ?? "null"}{NL}" +
                $"{CHARACTER_NAMESPACE_KEY}: {CharacterNamespace ?? "null"}{NL}" +
                $"{LANGUAGE_KEY}: {Language ?? "null"}{NL}" +
                $"{GROUP_CHARACTERS_BY_KEY}: {GroupCharactersBy ?? "null"}{NL}";
        }
    }
    public class SettingsParsingException : Exception
    {
        public SettingsParsingException(string name) : base($"Cannot parse this setting: {name}") { }
    }
}
