using UnityEngine;

namespace Pinky.Localization.Containers
{
    internal interface ILocalesAssetContainer<T> where T : Object
    {
        bool TryGetLocalizationAsset(SystemLanguage key, out T localizationAsset);

        T GetLocalizationAsset(SystemLanguage key);

        SystemLanguage[] GetKeys();
    }
}