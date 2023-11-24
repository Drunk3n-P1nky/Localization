using Pinky.Localization.Containers;
using Pinky.Localization.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Pinky.Localization
{
    public static class LocalizationService
    {
        private static Dictionary<string, string> localizationTextMap;
        private static StringSpriteMapContainer localizationSpriteMap;

        public static string GetLocalizedText(string key)
        {
            bool isSuccess = localizationTextMap.TryGetValue(key, out string localizedText);

            if (!isSuccess)
            {
                Debug.LogError($"There is no localization for key {key}");
                return string.Empty;
            }

            return localizedText;
        }

        public static Sprite GetLocalizedSprite(string key)
        {
            bool isSuccess = localizationSpriteMap.TryGetSprite(key, out Sprite sprite);

            if(!isSuccess)
                Debug.LogError($"There is no localization for key {key}");

            return sprite;
        }

        public static void ChangeLanguage(SystemLanguage language)
        {
            localizationTextMap = LocaleSerializer.Deserialize(language);

            LocalesSpriteContainer container = Resources.Load<LocalesSpriteContainer>("Localization/Locales Sprite Container");

            if(container)
                container.TryGetLocalizationAsset(language, out localizationSpriteMap);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            ChangeLanguage(Application.systemLanguage);
        }

        #region Editor Utility
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void InitializeInEditor() => Initialize();
#endif
#endregion
    }
}