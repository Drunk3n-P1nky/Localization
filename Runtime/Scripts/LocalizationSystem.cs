using System.Collections.Generic;
using UnityEngine;

namespace Pinky.Localization
{
    public static class LocalizationSystem
    {
        private static Dictionary<string, string> localizationMap;

        public static string GetLocalizedText(string key)
        {
            bool isSuccess = localizationMap.TryGetValue(key, out string localizedText);

            if (!isSuccess)
            {
                Debug.LogError($"There is no localization for key {key}");
                return string.Empty;
            }

            return localizedText;
        }

        public static void ChangeLanguage(SystemLanguage language)
        {
            localizationMap = TXTLoader.ParseTXT(language);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            ChangeLanguage(Application.systemLanguage);
        }
    }
}