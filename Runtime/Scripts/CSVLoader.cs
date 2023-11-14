using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Pinky.Localization
{
    public static class CSVLoader
    {
        private static readonly SystemLanguage defaultLanguage = SystemLanguage.English;
        private static LocalesContainer localesContainer;
        private static readonly char surround = '"';

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Initialize()
        {
            localesContainer = Resources.Load<LocalesContainer>("Locales Container");
            ParseCSV(SystemLanguage.English);
        }


        public static Dictionary<string, string> ParseCSV(SystemLanguage targetLanguage)
        {
            Dictionary<string, string> localizationMap = new();

            if (!localesContainer)
            {
                Debug.LogError($"{nameof(localesContainer)} is null. Check Resources folder.");
                return localizationMap;
            }

            bool isSuccess = localesContainer.TryGetLocalizationFile(targetLanguage, out TextAsset csvFile);

            if (!isSuccess)
                localesContainer.TryGetLocalizationFile(defaultLanguage, out csvFile);

            string[] lines = csvFile.text.Split(Environment.NewLine);

            Regex parser = new(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.Multiline);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] values = parser.Split(line);

                string key = values[0].TrimStart(surround).TrimEnd(surround);
                string value = values[1].TrimStart(' ', surround).TrimEnd(surround);
                localizationMap.Add(key, value);
            }

            return localizationMap;
        }
    }
}