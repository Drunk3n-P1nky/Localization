using Pinky.Localization.Utility;
using UnityEngine;

namespace Pinky.Localization.Containers
{
    public sealed class LocalesSpriteContainer : ScriptableObject, ILocalesAssetContainer<StringSpriteMapContainer>
    {
        [SerializeField]
        private LocalizationSpriteMap localizationMap;

        public bool TryGetLocalizationAsset(SystemLanguage key, out StringSpriteMapContainer container)
        {
            bool isSuccess = localizationMap.TryGetValue(key, out container);
            return isSuccess;
        }

        public StringSpriteMapContainer GetLocalizationAsset(SystemLanguage key)
        {
            return localizationMap[key];
        }

        public SystemLanguage[] GetKeys()
        {
            SystemLanguage[] keys = new SystemLanguage[localizationMap.Count];
            localizationMap.Keys.CopyTo(keys, 0);
            return keys;
        }

        #region Unity Editor Utility
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Pinky Localization/Locales Sprite Container")]
        public static void CreateAsset()
        {
            string directory = Application.dataPath + "/Resources/Localization";

            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);

            string path = UnityEditor.EditorUtility.SaveFilePanelInProject("Create Locales Sprite Container", "Locales Sprite Container", "asset", string.Empty, directory);

            if (string.IsNullOrEmpty(path))
                return;

            var newLocalesTextContainer = CreateInstance<LocalesSpriteContainer>();
            UnityEditor.AssetDatabase.CreateAsset(newLocalesTextContainer, path);
            var preloadedAssets = new System.Collections.Generic.List<Object>(UnityEditor.PlayerSettings.GetPreloadedAssets());
            preloadedAssets.RemoveAll(obj => obj is LocalesSpriteContainer);
            preloadedAssets.Add(newLocalesTextContainer);
            UnityEditor.PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
        }
#endif
        #endregion
    }
}
