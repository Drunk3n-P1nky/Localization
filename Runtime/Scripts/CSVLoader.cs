using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Pinky.Localization.Containers;

namespace Pinky.Localization
{
    public static class CSVLoader
    {
        public static SystemLanguage DefaultLanguage = SystemLanguage.English;
        private static LocalesTextContainer s_localesTextContainer;
        private static readonly char s_surround = '"';

        public static Dictionary<string, string> ParseCSV(SystemLanguage targetLanguage)
        {
            if (!s_localesTextContainer)
            {
                Debug.LogError($"{nameof(s_localesTextContainer)} is null. Check Resources folder.");
                return new Dictionary<string, string>();
            }

            bool isSuccess = s_localesTextContainer.TryGetLocalizationFile(targetLanguage, out TextAsset csvFile);

            if (!isSuccess)
                s_localesTextContainer.TryGetLocalizationFile(DefaultLanguage, out csvFile);

           return ParseCSV(csvFile);
        }

        public static Dictionary<string, string> ParseCSV(SystemLanguage targetLanguage, LocalesTextContainer localesTextContainer) 
        {
            s_localesTextContainer = localesTextContainer;
            return ParseCSV(targetLanguage);
        }

        public static Dictionary<string, string> ParseCSV(TextAsset csvFile)
        {
            Dictionary<string, string> localizationMap = new();

            string[] lines = Regex.Split(csvFile.text, @"(?<="")\s*;\s*(?:\r\n|\n|\r)", RegexOptions.Compiled);

            Regex parser = new(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.Compiled);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] values = parser.Split(line);

                string key = values[0].TrimStart(s_surround).TrimEnd(s_surround);
                string value = values[1].TrimStart(' ', s_surround).TrimEnd(s_surround, ';');
                localizationMap.Add(key, value);
            }

            return localizationMap;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            s_localesTextContainer = Resources.Load<LocalesTextContainer>("Locales Container");
        }
    }
}