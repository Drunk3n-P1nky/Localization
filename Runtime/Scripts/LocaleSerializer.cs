using System.Collections.Generic;
using UnityEngine;
using Pinky.Localization.Containers;

namespace Pinky.Localization
{
    public static class LocaleSerializer
    {
        public static SystemLanguage DefaultLanguage = SystemLanguage.English;
        private static LocalesTextContainer s_localesTextContainer;

        public static byte[] Serialize(Dictionary<string, string> localizationMap)
        {
            return MessagePack.MessagePackSerializer.Serialize(localizationMap);
        }

        public static Dictionary<string, string> Deserialize(SystemLanguage targetLanguage)
        {
            if (!s_localesTextContainer)
            {
                Debug.LogError($"{nameof(s_localesTextContainer)} is null. Check Resources folder.");
                return new Dictionary<string, string>();
            }

            bool isSuccess = s_localesTextContainer.TryGetLocalizationAsset(targetLanguage, out TextAsset binaryFile);

            if (!isSuccess)
                s_localesTextContainer.TryGetLocalizationAsset(DefaultLanguage, out binaryFile);

           return Deserialize(binaryFile);
        }

        public static Dictionary<string, string> Deserialize(SystemLanguage targetLanguage, LocalesTextContainer localesTextContainer) 
        {
            s_localesTextContainer = localesTextContainer;
            return Deserialize(targetLanguage);
        }

        public static Dictionary<string, string> Deserialize(TextAsset binaryFile)
        {
            Dictionary<string, string> localizationMap = MessagePack.MessagePackSerializer.Deserialize<Dictionary<string, string>>(binaryFile.bytes);
            return localizationMap;
        }

        public static Dictionary<SystemLanguage, Dictionary<string, string>> DeserializeLocales()
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
                s_localesTextContainer.TryGetLocalizationAsset(currentLanguage, out TextAsset file);

                if (!file)
                    continue;

                allLocales.Add(currentLanguage, Deserialize(file));
            }

            return allLocales;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            s_localesTextContainer = Resources.Load<LocalesTextContainer>("Localization/Locales Text Container");
        }

        #region Unity Editor
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void InitializeInEditor() => Initialize();
#endif
#endregion
    }
}