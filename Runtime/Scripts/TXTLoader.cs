using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Pinky.Localization.Containers;

namespace Pinky.Localization
{
    public static class TXTLoader
    {
        private const char SURROUND = '"';
        public static SystemLanguage DefaultLanguage = SystemLanguage.English;
        private static LocalesTextContainer s_localesTextContainer;

        public static Dictionary<string, string> ParseTXT(SystemLanguage targetLanguage)
        {
            if (!s_localesTextContainer)
            {
                Debug.LogError($"{nameof(s_localesTextContainer)} is null. Check Resources folder.");
                return new Dictionary<string, string>();
            }

            bool isSuccess = s_localesTextContainer.TryGetLocalizationFile(targetLanguage, out TextAsset csvFile);

            if (!isSuccess)
                s_localesTextContainer.TryGetLocalizationFile(DefaultLanguage, out csvFile);

           return ParseTXT(csvFile);
        }

        public static Dictionary<string, string> ParseTXT(SystemLanguage targetLanguage, LocalesTextContainer localesTextContainer) 
        {
            s_localesTextContainer = localesTextContainer;
            return ParseTXT(targetLanguage);
        }

        public static Dictionary<string, string> ParseTXT(TextAsset csvFile)
        {
            Dictionary<string, string> localizationMap = new();

            string[] lines = Regex.Split(csvFile.text, @"(?<="")\s*;\s*(?:\r\n|\n|\r)", RegexOptions.Compiled);

            Regex parser = new(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.Compiled);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] values = parser.Split(line);

                string key = values[0].TrimStart(SURROUND).TrimEnd(SURROUND);
                string value = values[1].TrimStart(' ', SURROUND).TrimEnd(SURROUND, ';');
                localizationMap.Add(key, value);
            }

            return localizationMap;
        }

        public static Dictionary<SystemLanguage, Dictionary<string, string>> ParseAllLocales()
        {
            Dictionary<SystemLanguage, Dictionary<string, string>> allLocales = new();

#if UNITY_EDITOR
            if (!s_localesTextContainer)
                Initialize();
#endif

            SystemLanguage[] systemLanguages = s_localesTextContainer.GetKeys();

            for (int i = 0; i < systemLanguages.Length; i++)
            {
                SystemLanguage currentLanguage = systemLanguages[i];
                s_localesTextContainer.TryGetLocalizationFile(currentLanguage, out TextAsset file);

                if (!file)
                    continue;

                allLocales.Add(currentLanguage, ParseTXT(file));
            }

            return allLocales;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            s_localesTextContainer = Resources.Load<LocalesTextContainer>("Localization/Locales Text Container");
        }
    }
}