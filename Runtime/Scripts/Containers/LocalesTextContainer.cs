using Pinky.Localization.Utility;
using UnityEngine;

namespace Pinky.Localization.Containers
{
    [CreateAssetMenu(fileName = "Locales Container", menuName = "Pinky/Localization/Create Locales Container")]
    public class LocalesTextContainer : ScriptableObject
    {
        [SerializeField]
        private LocalizationTextMap localizationMap;

        public bool TryGetLocalizationFile(SystemLanguage language, out TextAsset localizationFile)
        {
            bool isSuccess = localizationMap.TryGetValue(language, out localizationFile);
            return isSuccess;
        }
    }
}