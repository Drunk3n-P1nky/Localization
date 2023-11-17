using Pinky.Localization.Utility;
using UnityEngine;

namespace Pinky.Localization.Containers
{
    [CreateAssetMenu(fileName = "Locales Text Container", menuName = "Pinky/Localization/Create Locales Container")]
    public class LocalesTextContainer : ScriptableObject
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
    }
}