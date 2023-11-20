using Pinky.Localization.Utility;
using UnityEngine;

namespace Pinky.Localization.Containers
{
    public sealed class LocalesTextContainer : ScriptableObject
    {
        [SerializeField]
        private LocalizationTextMap localizationMap;

        public bool TryGetLocalizationFile(SystemLanguage language, out TextAsset localizationFile)
        {
            bool isSuccess = localizationMap.TryGetValue(language, out localizationFile);
            return isSuccess;
        }

        public TextAsset GetLocalizationFile(SystemLanguage language) 
        {
            return localizationMap[language];
        }

        public SystemLanguage[] GetKeys()
        {
            SystemLanguage[] keys = new SystemLanguage[localizationMap.Count];
            localizationMap.Keys.CopyTo(keys, 0);
            return keys;
        }

        #region Unity Editor Utility
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Pinky Localization/Locales Text Container")]
        public static void CreateAsset()
        {
            string directory = Application.dataPath + "/Resources/Localization";

            if(!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);

            string path = UnityEditor.EditorUtility.SaveFilePanelInProject("Create Locales Text Container", "Locales Text Container", "asset", string.Empty, directory);

            if (string.IsNullOrEmpty(path))
                return;

            var newLocalesTextContainer = CreateInstance<LocalesTextContainer>();
            UnityEditor.AssetDatabase.CreateAsset(newLocalesTextContainer, path);
            var preloadedAssets = new System.Collections.Generic.List<Object>(UnityEditor.PlayerSettings.GetPreloadedAssets());
            preloadedAssets.RemoveAll(obj => obj is LocalesTextContainer);
            preloadedAssets.Add(newLocalesTextContainer);
            UnityEditor.PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
        }
#endif
#endregion
    }
}